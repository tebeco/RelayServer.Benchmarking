public void Write(IBufferWriter<byte> bufferWriter)
{
    PrefixBufferWriter prefixWriter = new PrefixBufferWriter(bufferWriter);    
    Utf8JsonWriter jsonWriter = new Utf8JsonWriter(prefixWriter);
    
    // serialize object
    
    prefixWriter.Complete();
}

private class PrefixBufferWriter : IBufferWriter<byte>
{
    private const int MaxMessageLength = 4096;
    
    private Memory<byte> _memory;
    private IBufferWriter<byte> _writer;
    
    private int _count;
    
    public PrefixBufferWriter(IBufferWriter<byte> writer)
    {
        _writer = writer;
        _memory = writer.GetMemory(MaxMessageLength);
    }
    
    public void Advance(int count)
    {
        _count += count;
    }

    public Memory<byte> GetMemory(int sizeHint)
    {
        int start = _count + 4;
        
        if (sizeHint + start > MaxMessageLength)
            throw new InternalBufferOverflowException();
            
        return _memory.Slice(start);
    }

    public Span<byte> GetSpan(int sizeHint)
    {
        int start = _count + 4;
        
        if (sizeHint + start > MaxMessageLength)
            throw new InternalBufferOverflowException();
            
        return _memory.Span.Slice(start);
    }
    
    public void Complete()
    {
        BinaryPrimitives.WriteInt32LittleEndian(_memory.Span, _count);
        _writer.Advance(_count + 4);
    }
}