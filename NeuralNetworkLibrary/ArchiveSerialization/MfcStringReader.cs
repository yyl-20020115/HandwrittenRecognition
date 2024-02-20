using System;
using System.Text;
using System.IO;

namespace ArchiveSerialization;

public static class MFCStringReader
{
    public static string ReadCString(BinaryReader reader)
    {
        var text = "";
        var convert = 1; // if we get ANSI, convert

        var length = ReadStringLength(reader);
        if (length == unchecked((uint)(-1)))
        {
            convert = 1 - convert;
            length = ReadStringLength(reader);
            if (length == unchecked((uint)(-1)))
                return text;
        }

        // set length of string to new length
        var bytes = length;
        bytes += (uint)(bytes * (1 - convert)); // bytes to read

        // read in the characters
        if (length != 0)
        {
            // read new data
            var buffer = reader.ReadBytes((int)bytes);

            // convert the data if as necessary
            var builder = new StringBuilder();
            if (convert != 0)
            {
                for (int i = 0; i < length; i++)
                    builder.Append((char)buffer[i]);
            }
            else
            {
                for (int i = 0; i < length; i++)
                    builder.Append((char)(buffer[i * 2] + buffer[i * 2 + 1] * 256));
            }

            text = builder.ToString();
        }

        return text;
    }

    private static uint ReadStringLength(BinaryReader reader)
    {
        // attempt BYTE length first
        var length = reader.ReadByte();

        if (length < 0xff)
            return length;

        // attempt WORD length
        ushort length2 = reader.ReadUInt16();
        return length2 switch
        {
            0xfffe => unchecked((uint)-1),// UNICODE string prefix (length will follow)
            0xffff => reader.ReadUInt32(),// read DWORD of length
            _ => length2,
        };
    }
}
