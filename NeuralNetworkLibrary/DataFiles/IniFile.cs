using System.Text;
using System.Runtime.InteropServices;


namespace NeuralNetworkLibrary;

/// <PARAM name="Path"></PARAM>
public class IniFile(string Path)
{
    public string Path = Path;

    [DllImport("kernel32")]
    private static extern bool WritePrivateProfileString(string section,
        string key, string val, string filePath);
    [DllImport("kernel32")]
    private static extern int GetPrivateProfileString(string section,
             string key, string def, StringBuilder retVal,
        int size, string filePath);

    /// <summary>

    /// Write Data to the INI File

    /// </summary>

    /// <PARAM name="Section"></PARAM>

    /// Section name

    /// <PARAM name="Key"></PARAM>

    /// Key Name

    /// <PARAM name="Value"></PARAM>

    /// Value Name

    public bool IniWriteValue(string Section, string Key, string Value)
        => WritePrivateProfileString(Section, Key, Value, Path);

    /// <summary>

    /// Read Data Value From the Ini File

    /// </summary>

    /// <PARAM name="Section"></PARAM>

    /// <PARAM name="Key"></PARAM>

    /// <PARAM name="Path"></PARAM>

    /// <returns></returns>

    public string IniReadValue(string Section, string Key)
    {
        var builder = new StringBuilder(4096);
        GetPrivateProfileString(Section, Key, "", builder, 4096, this.Path);
        return builder.ToString();
    }
}
