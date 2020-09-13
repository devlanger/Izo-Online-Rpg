using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LoginRequestPacket : Packet
{
    public LoginRequestPacket(string username, string password) : base()
    {
        writer.Write((byte)1);
        writer.Write(username);
        writer.Write(password);
    }
}