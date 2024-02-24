using System;
using System.Text;
using System.IO;

namespace ArchiveSerialization;

public enum ArchiveOp
{
    Load = 0,
    Store = 1
}

public class Archive
{
    protected BinaryWriter writer = null;
    protected BinaryReader reader = null;
    protected ArchiveOp op = ArchiveOp.Load;
    private const int Index = 0; // actually never changes

    public Archive(Stream _stream, ArchiveOp _op)
    {
        switch (op = _op)
        {
            case ArchiveOp.Load:
                reader = new BinaryReader(_stream);
                break;
            default:
                writer = new BinaryWriter(_stream);
                break;
        }
    }

    public bool IsStoring => op == ArchiveOp.Store;

    public void Serialize(IArchiveSerialization s) => s.Serialize(this);

    //////////////////////////////////////////////////////
    // write functions

    public void Write(char ch) =>
        //writer.Write(ch);
        writer.Write((short)ch);

    public void Write(ushort n) => writer.Write(n);

    public void Write(short n) => writer.Write(n);

    public void Write(uint n) => writer.Write(n);

    public void Write(int n) => writer.Write(n);

    public void Write(ulong n)
    => writer.Write(n);

    public void Write(long n) => writer.Write(n);

    public void Write(float d) => writer.Write(d);

    public void Write(double d)
        => writer.Write(d);

    public void Write(decimal d) =>
        // store decimals as Int64
        writer.Write(decimal.ToOACurrency(d));

    public void Write(DateTime dt) => writer.Write(dt.ToBinary());

    public void Write(bool b) => writer.Write(b);

    public void Write(string s)
    {
        writer.Write(s.Length);
        writer.Write(s.ToCharArray());
    }

    public void Write(Guid guid) => Write(guid.ToByteArray());

    public void Write(byte[] buffer) => writer.Write(buffer);

    ///////////////////////////////////////////////////
    // Read functions

    public void Read(out string s)
    {
        Read(out int length);

        var chars = new char[length];

        reader.Read(chars, Index, length);

        var builder = new StringBuilder();
        builder.Append(chars);
        s = builder.ToString();
    }

    public void Read(out UInt16 n)
    {
        var bytes = new byte[2];
        reader.Read(bytes, Index, 2);
        n = BitConverter.ToUInt16(bytes, 0);
    }

    public void Read(out short n)
    {
        var bytes = new byte[2];
        reader.Read(bytes, Index, 2);
        n = BitConverter.ToInt16(bytes, 0);
    }

    public void Read(out uint n)
    {
        var bytes = new byte[4];
        reader.Read(bytes, Index, 4);
        n = BitConverter.ToUInt32(bytes, 0);
    }

    public void Read(out int n)
    {
        var bytes = new byte[4];
        reader.Read(bytes, Index, 4);
        n = BitConverter.ToInt32(bytes, 0);
    }

    public void Read(out ulong n)
    {
        var bytes = new byte[8];
        reader.Read(bytes, Index, 8);
        n = BitConverter.ToUInt64(bytes, 0);
    }

    public void Read(out long n)
    {
        var bytes = new byte[8];
        reader.Read(bytes, Index, 8);
        n = BitConverter.ToInt64(bytes, 0);
    }

    public void Read(out char ch)
    {
        Read(out short n);
        ch = (char)n;

        /* direct reading as char doesn't work for some reason
			Sometimes it works, but sometimes the character
		  takes up only one byte in the buffer and it seems
		  to depend on what comes before and after the item in the buffer
	 
		*/

        //			byte[] bytes = new byte[2];
        //			reader.Read(bytes, m_Index, 2);
        //			ch = BitConverter.ToChar(bytes, 0);
    }

    public void Read(out float d)
    {
        var bytes = new byte[4];
        reader.Read(bytes, Index, 4);
        d = BitConverter.ToSingle(bytes, 0);
    }

    public void Read(out double d)
    {
        var bytes = new byte[8];
        reader.Read(bytes, Index, 8);
        d = BitConverter.ToDouble(bytes, 0);
    }

    public void Read(out decimal d)
    {
        var bytes = new byte[8];
        reader.Read(bytes, Index, 8);

        // BitConverter does not support direct conversion to Decimal so use Int64
        Int64 n = BitConverter.ToInt64(bytes, 0);
        d = decimal.FromOACurrency(n);
    }

    public void Read(out DateTime dt)
    {
        Read(out long l);
        dt = DateTime.FromBinary(l);
    }

    public void Read(out Boolean b)
    {
        byte[] bytes = new byte[1];
        reader.Read(bytes, Index, 1);
        b = BitConverter.ToBoolean(bytes, 0);
    }

    public void Read(out Guid guid)
    {
        var bytes = new byte[16];
        Read(out bytes, 16);
        guid = new Guid(bytes);
    }

    public void Read(out byte[] buffer, int bufferSize)
    {
        buffer = new byte[bufferSize];
        reader.Read(buffer, Index, bufferSize);
    }
} // end of class
  // end of namespace
