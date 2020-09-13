using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Packet
{
    public BinaryWriter writer;
    private MemoryStream stream;

    public byte[] Data
    {
        get
        {
            return stream.ToArray();
        }
    }

    public Packet()
    {
        this.stream = new MemoryStream();
        this.writer = new BinaryWriter(stream);
    }
}
