using System.Text;
using System.Runtime.InteropServices;


namespace NeuralNetworkLibrary;

/// <PARAM name="INIPath"></PARAM>
public class IniFile(string INIPath)
{
    public string path = INIPath;

    [DllImport("kernel32")]
    private static extern long WritePrivateProfileString(string section,
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

    public void IniWriteValue(string Section, string Key, string Value)
    {
        WritePrivateProfileString(Section, Key, Value, this.path);
    }

    /// <summary>

    /// Read Data Value From the Ini File

    /// </summary>

    /// <PARAM name="Section"></PARAM>

    /// <PARAM name="Key"></PARAM>

    /// <PARAM name="Path"></PARAM>

    /// <returns></returns>

    public string IniReadValue(string Section, string Key)
    {
        var builder = new StringBuilder(255);
        int i = GetPrivateProfileString(Section, Key, "", builder, 255, this.path);
        return builder.ToString();

    }
}
