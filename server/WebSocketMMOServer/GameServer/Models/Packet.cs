using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class Packet
{
    public BinaryWriter writer;
    public MemoryStream stream;

    public byte[] Data
    {
        get
        {
            return stream.ToArray();
        }
    }

    public Packet()
    {
        stream = new MemoryStream();
        writer = new BinaryWriter(stream);
    }
}
