using System;
using System.IO;

namespace ArchiveSerialization;

public enum OleDateTimeStatus
{
    Valid = 0,
    Invalid = 1,
    Null = 2
};

public enum OleCurrencyStatus
{
    Valid = 0,
    Invalid = 1,
    Null = 2
}

/// <summary>
/// Class allows reading objects serialize using MFC CArchive
/// </summary>
public class MfcArchive : Archive
{
    public MfcArchive(Stream _stream, ArchiveOp _op)
        : base(_stream, _op)
    {
        if (_op == ArchiveOp.Store)
        {
            throw new NotImplementedException("Writing to MFC compatible serialization is not supported.");
        }
    }

    new public void Read(out decimal d)
    {
        // MFC stores decimal as 32-bit status value, 32-bit high value, and 32-bit low value
        Read(out int status);
        Read(out int high);
        Read(out uint low);

        if (status != (int)OleCurrencyStatus.Valid)
        {
            d = 0;
        }
        else
        {
            Int64 final = MakeInt64((int)low, high);
            d = Decimal.FromOACurrency(final);
        }

    }

    new public void Read(out bool b)
    {
        // MFC stores bools as 32-bit "long"
        base.Read(out int l);
        if (l == 0) b = false;
        else b = true;
    }


    new public void Read(out DateTime dt)
    {
        base.Read(out uint status); // status is a 32-bit "long" in C++

        // MFC stores dates as 8-byte double
        base.Read(out double d);
        dt = DateTime.FromOADate(d);

        if (status == (UInt32)OleDateTimeStatus.Null ||
            status == (UInt32)OleDateTimeStatus.Invalid)
        {
            // in this situation, the date is not valid.  
            // One option is to set to the initialized OLE Date value of 0.0
            dt = DateTime.FromOADate(0.0);
        }
    }

    public void Read(out DateTime? dt)
    {
        base.Read(out uint status); // status is a 32-bit "long" in C++

        base.Read(out double d);
        dt = DateTime.FromOADate(d);

        // read in nullable type
        if (status == (uint)OleDateTimeStatus.Null ||
            status == (uint)OleDateTimeStatus.Invalid)
        {
            dt = null;
        }
    }

    new public void Read(out string s) => s = MFCStringReader.ReadCString(this.reader);
    public void ReadUnicodeString(out string s) => s = reader.ReadString();

    // Convert current low and high to 8-Byte C++ CURRENCY structure 
    static public long MakeInt64(int l1, int l2) => ((uint)l1) | ((uint)l2 << 32);

} // end of class
