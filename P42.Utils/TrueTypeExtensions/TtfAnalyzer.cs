using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using P42.Serilog.QuickLog;

namespace P42.Utils;

public class TtfAnalyzer
{

    #region Private
    // Font file; must be seekable
    //RandomAccessFile m_file;
    private Stream? _stream;

    // Helper I/O functions
    private int ReadByte()
    {
        return _stream?.ReadByte() & 0xFF ?? throw new IncompleteInitialization();
        //return m_file.Read() & 0xFF;
    }

    private int ReadWord()
    {
        var b1 = ReadByte();
        var b2 = ReadByte();
        return b1 << 8 | b2;
    }

    private int ReadDword()
    {
        var b1 = ReadByte();
        var b2 = ReadByte();
        var b3 = ReadByte();
        var b4 = ReadByte();
        return b1 << 24 | b2 << 16 | b3 << 8 | b4;
    }

    private void Read(byte[] array)
    {
        if (_stream is null)
            throw new IncompleteInitialization();
        if (_stream.Read(array, 0, array.Length) != array.Length)
            throw new IOException();
    }

    // Helper
    private static int GetWord(byte[] array, int offset)
    {
        var b1 = array[offset] & 0xFF;
        var b2 = array[offset + 1] & 0xFF;
        return b1 << 8 | b2;
    }

    private void Seek(int offset)
    {
        if (_stream is null)
            throw new IncompleteInitialization();
        _stream.Seek(offset, SeekOrigin.Begin);
    }
    #endregion

    
    #region FontFamily
    /// <summary>
    /// FontFamily (as text) from TTF or OTF file
    /// </summary>
    /// <param name="fontFilePath"></param>
    /// <returns></returns>
    public static string FontFamily(string fontFilePath)
    {
        using var stream = File.Open(fontFilePath, FileMode.Open);
        return FontFamily(stream);
    }

    /// <summary>
    /// FontFamily (as text) from TTF or OTF stream
    /// </summary>
    /// <param name="fontFileStream"></param>
    /// <returns></returns>
    public static string FontFamily(Stream fontFileStream)
    {
        try
        {
            var analyzer = new TtfAnalyzer
            {

                // Parses the TTF file format.
                // See http://developer.apple.com/fonts/ttrefman/rm06/Chap6.html
                //_stream = new RandomAccessFile(fontFilename, "r");
                _stream = fontFileStream
            };

            // Read the version first
            var version = analyzer.ReadDword();

            // The version must be either 'true' (0x74727565) or 0x00010000 or 'OTTO' (0x4f54544f) for CFF style fonts.
            if (version != 0x74727565 && version != 0x00010000 && version != 0x4f54544f)
            {
                QLog.Error($"FontStream does not have valid version [{version}] value");
                return string.Empty;
            }

            // The TTF file consist of several sections called "tables", and we need to know how many of them are there.
            var numTables = analyzer.ReadWord();

            // Skip the rest in the header
            analyzer.ReadWord(); // skip searchRange
            analyzer.ReadWord(); // skip entrySelector
            analyzer.ReadWord(); // skip rangeShift

            // Now we can read the tables
            for (var i = 0; i < numTables; i++)
            {
                // Read the table entry
                var tag = analyzer.ReadDword();
                analyzer.ReadDword(); // skip checksum
                var offset = analyzer.ReadDword();
                var length = analyzer.ReadDword();

                // Now here' the trick. 'name' field actually contains the textual string name.
                // So the 'name' string in characters equals to 0x6E616D65
                if (tag != 0x6E616D65)
                    continue;

                // Here's the name section. Read it completely into the allocated buffer
                var table = new byte[length];

                analyzer.Seek(offset);
                analyzer.Read(table);

                // This is also a table. See http://developer.apple.com/fonts/ttrefman/rm06/Chap6name.html
                // According to Table 36, the total number of table records is stored in the second word, at the offset 2.
                // Getting the count and string offset - remembering it's big endian.
                var count = GetWord(table, 2);
                var stringOffset = GetWord(table, 4);

                //List<string> names = new List<string>();

                // Record starts from offset 6
                for (var record = 0; record < count; record++)
                {
                    // Table 37 tells us that each record is 6 words -> 12 bytes, and that the nameID is 4th word so its offset is 6.
                    // We also need to account for the first 6 bytes of the header above (Table 36), so...
                    var nameIdOffset = record * 12 + 6;
                    var platformId = GetWord(table, nameIdOffset);
                    var nameIdValue = GetWord(table, nameIdOffset + 6);


                    // Table 42 lists the valid name Identifiers. We're interested in 1 (Font Family Name) but not in Unicode encoding (for simplicity).
                    // The encoding is stored as PlatformID and we're interested in Mac encoding
                    if (nameIdValue != 1) // && platformID == 1)
                        continue;

                    // We need the string offset and length, which are the word 6 and 5 respectively
                    var nameLength = GetWord(table, nameIdOffset + 8);
                    var nameOffset = GetWord(table, nameIdOffset + 10);

                    // The real name string offset is calculated by adding the string_offset
                    nameOffset += stringOffset;

                    // Make sure it is inside the array
                    if (nameOffset >= 0 && nameOffset + nameLength < table.Length)
                    {
                        var chars = new byte[nameLength];
                        Buffer.BlockCopy(table, nameOffset, chars, 0, nameLength);
                        switch (platformId)
                        {
                            case 1:
                                return Encoding.UTF8.GetString(chars, 0, nameLength);
                            case 2:
                                break;
                            case 3:
                                return Encoding.BigEndianUnicode.GetString(chars, 0, nameLength);
                        }
                    }
                }
            }

            return string.Empty;
        }
#pragma warning disable 0168
        catch (FileNotFoundException e)
#pragma warning restore 0168
        {
            QLog.Error(e);
            // Permissions?
            return string.Empty;
        }
#pragma warning disable 0168
        catch (IOException eIo)
#pragma warning restore 0168
        {
            QLog.Error(eIo);
            // Most likely a corrupted font file
            return string.Empty;
        }
    }
    #endregion

    
    #region FontAttributes
    /// <summary>
    /// Font Attributes (as string) from TTF or OTF file
    /// </summary>
    /// <param name="fontFilePath"></param>
    /// <returns></returns>
    public static string FontAttributes(string fontFilePath)
    {
        using var stream = File.Open(fontFilePath, FileMode.Open);
        return FontAttributes(stream);
    }

    /// <summary>
    /// Font Attributes (as string) from TTF or OTF stream
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static string FontAttributes(Stream stream)
    {
        try
        {
            var analyzer = new TtfAnalyzer
            {

                // Parses the TTF file format.
                // See http://developer.apple.com/fonts/ttrefman/rm06/Chap6.html
                //_stream = new RandomAccessFile(fontFilename, "r");
                _stream = stream
            };

            // Read the version first
            var version = analyzer.ReadDword();

            // The version must be either 'true' (0x74727565) or 0x00010000 or 'OTTO' (0x4f54544f) for CFF style fonts.
            if (version != 0x74727565 && version != 0x00010000 && version != 0x4f54544f)
            {
                QLog.Error($"FontStream does not have valid version [{version}] value");
                return string.Empty;
            }

            // The TTF file consist of several sections called "tables", and we need to know how many of them are there.
            var numTables = analyzer.ReadWord();

            // Skip the rest in the header
            analyzer.ReadWord(); // skip searchRange
            analyzer.ReadWord(); // skip entrySelector
            analyzer.ReadWord(); // skip rangeShift

            // Now we can read the tables
            for (var i = 0; i < numTables; i++)
            {
                // Read the table entry
                var tag = analyzer.ReadDword();
                analyzer.ReadDword(); // skip checksum
                var offset = analyzer.ReadDword();
                var length = analyzer.ReadDword();

                // Now here' the trick. 'name' field actually contains the textual string name.
                // So the 'name' string in characters equals to 0x6E616D65
                if (tag != 0x6E616D65)
                    continue;

                // Here's the name section. Read it completely into the allocated buffer
                var table = new byte[length];

                //_stream.Seek(offset);
                analyzer.Seek(offset);
                analyzer.Read(table);

                // This is also a table. See http://developer.apple.com/fonts/ttrefman/rm06/Chap6name.html
                // According to Table 36, the total number of table records is stored in the second word, at the offset 2.
                // Getting the count and string offset - remembering it's big endian.
                var count = GetWord(table, 2);
                var stringOffset = GetWord(table, 4);

                // Record starts from offset 6
                for (var record = 0; record < count; record++)
                {
                    // Table 37 tells us that each record is 6 words -> 12 bytes, and that the nameID is 4th word so its offset is 6.
                    // We also need to account for the first 6 bytes of the header above (Table 36), so...
                    var nameIdOffset = record * 12 + 6;
                    var platformId = GetWord(table, nameIdOffset);
                    var nameIdValue = GetWord(table, nameIdOffset + 6);

                    // Table 42 lists the valid name Identifiers. We're interested in 1 (Font Subfamily Name) but not in Unicode encoding (for simplicity).
                    // The encoding is stored as PlatformID and we're interested in Mac encoding
                    if (nameIdValue != 2 || platformId != 1)
                        continue;

                    // We need the string offset and length, which are the word 6 and 5 respectively
                    var nameLength = GetWord(table, nameIdOffset + 8);
                    var nameOffset = GetWord(table, nameIdOffset + 10);

                    // String offset for NAME is calculated by adding the string_offset
                    nameOffset += stringOffset;

                    // Make sure it is inside the array
                    if (nameOffset < 0 || nameOffset + nameLength >= table.Length)
                        continue;

                    //return new String( table, name_offset, name_length );
                    //char[] chars = new char[name_length];
                    /*
                                System.Buffer.BlockCopy(table, name_offset, chars, 0, name_length);
                                */
                    /*
                                for(int nameI=0;nameI<name_length;nameI++) {
                                    chars [nameI] = (char)table [name_offset + nameI];
                                }
                                */
                    var chars = new byte[nameLength];
                    Buffer.BlockCopy(table, nameOffset, chars, 0, nameLength);
                    //var str = new string(chars);
                    //var str = System.Text.Encoding.Default.GetString(chars);
                    var str = Encoding.UTF8.GetString(chars, 0, nameLength);
                    return str;
                }
            }

            return string.Empty;
        }
        catch (FileNotFoundException e)
        {
            // Permissions?
            QLog.Error(e);
            return string.Empty;
        }
        catch (IOException eIo)
        {
            QLog.Error(eIo);
            // Most likely a corrupted font file
            return string.Empty;
        }
    }
    #endregion
    
}
