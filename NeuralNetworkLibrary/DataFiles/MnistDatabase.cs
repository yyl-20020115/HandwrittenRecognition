using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace NeuralNetworkLibrary;

public struct ImageFileHeader
{
    public int Magic;
    public int ItemsCount;
    public int Rows;
    public int Cols;
}
public struct LabelFileHeader
{
    public int Magic;
    public int ItemsCount;
}
/// <summary>
/// Image Pattern Class
/// </summary>
public class ImagePattern
{
    public byte[] Pattern = new byte[Defaults.Global_ImageSize * Defaults.Global_ImageSize];
    public byte Label;
}
/// <summary>
/// MNIST Data Class (Image+Label)
/// </summary>
public class MnistDatabase
{
    protected ImageFileHeader ImageFileHeader;
    protected LabelFileHeader LabelFileHeader;
    private uint NextPattern;
    protected uint ItemCount;
    protected int[] RandomizedPatternSequence;
    protected string MnistImageFileName;
    protected string MnistLabelFileName;
    protected bool IsImageFileOpen;
    protected bool IsLabelFileOpen;
    protected bool HasDatabase;
    protected bool IsFromRandomizedPatternSequence;
    protected BinaryReader ImageFileStream;
    protected BinaryReader LabelFileStream;

    public List<ImagePattern> ImagePatterns;
    public bool BFromRandomizedPatternSequence 
        => IsFromRandomizedPatternSequence;
    public bool BDatabaseReady 
        => HasDatabase;

    public MnistDatabase()
    {
        MnistImageFileName = null;
        MnistLabelFileName = null;
        NextPattern = 0;

        IsImageFileOpen = false;
        IsLabelFileOpen = false;
        ImagePatterns = null;
        ImageFileStream = null;
        LabelFileStream = null;
        HasDatabase = false;
        RandomizedPatternSequence = null;
        IsFromRandomizedPatternSequence = false;
    }
    public bool LoadMinstFiles()
    {
        //clear Image Pattern List
        ImagePatterns?.Clear();
        //close files if opened
        if (IsImageFileOpen)
        {

            ImageFileStream.Close();
            IsImageFileOpen = false;

        }
        if (IsLabelFileOpen)
        {
            LabelFileStream.Close();
            IsLabelFileOpen = false;
        }
        Environment.CurrentDirectory 
            = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "..", "..");

        //load Mnist Images files.
        if (!MnistImageFileHeader())
        {
            MessageBox.Show("Can not open Image file");
            MnistImageFileName = null;
            IsImageFileOpen = false;
            HasDatabase = false;
            return false;
        }
        if (!MnistLabelFileHeader())
        {
            MessageBox.Show("Can not open label file");
            MnistLabelFileName = null;
            IsLabelFileOpen = false;
            HasDatabase = false;
            return false;
        }
        //check the value if image file and label file have been opened successfully
        if (LabelFileHeader.ItemsCount != ImageFileHeader.ItemsCount)
        {
            MessageBox.Show("Item numbers are different");
            CloseMinstFiles();
            HasDatabase = false;
            return false;
        }
        ImagePatterns = new List<ImagePattern>(ImageFileHeader.ItemsCount);
        RandomizedPatternSequence = new int[ImageFileHeader.ItemsCount];
        for (int i = 0; i < ImageFileHeader.ItemsCount; i++)
        {
            var PatternArray = new byte[Defaults.Global_ImageSize * Defaults.Global_ImageSize];
            ImagePattern ImagePattern = new ImagePattern();
            GetNextPattern(PatternArray, out byte m_nlabel, i, true);
            ImagePattern.Pattern = PatternArray;
            ImagePattern.Label = m_nlabel;
            ImagePatterns.Add(ImagePattern);
        }
        HasDatabase = true;
        CloseMinstFiles();
        return true;


    }
    public void CloseMinstFiles()
    {
        LabelFileStream.Close();
        ImageFileStream.Close();
        IsImageFileOpen = false;
        IsLabelFileOpen = false;
    }
    /// <summary>
    /// //Get MNIST Image file'header
    /// </summary>
    protected bool MnistImageFileHeader()
    {

        if (IsImageFileOpen == false)
        {
            var m_byte = new byte[4];
            var dialog = new OpenFileDialog
            {
                Filter = "Mnist Image file (*.idx3-ubyte)|*.idx3-ubyte",
                Title = "Open Minist Image File",
                InitialDirectory = Environment.CurrentDirectory
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                MnistImageFileName = dialog.FileName;

                try
                {
                    ImageFileStream = new BinaryReader(dialog.OpenFile());
                    //Magic number 
                    ImageFileStream.Read(m_byte, 0, 4);
                    Array.Reverse(m_byte, 0, 4);
                    ImageFileHeader.Magic = BitConverter.ToInt32(m_byte, 0);
                    //number of images 
                    ImageFileStream.Read(m_byte, 0, 4);
                    //High-Endian format to Low-Endian format
                    Array.Reverse(m_byte, 0, 4);
                    ImageFileHeader.ItemsCount = BitConverter.ToInt32(m_byte, 0);
                    ItemCount = (uint)ImageFileHeader.ItemsCount;
                    //number of rows 
                    ImageFileStream.Read(m_byte, 0, 4);
                    Array.Reverse(m_byte, 0, 4);
                    ImageFileHeader.Rows = BitConverter.ToInt32(m_byte, 0);
                    //number of columns 
                    ImageFileStream.Read(m_byte, 0, 4);
                    Array.Reverse(m_byte, 0, 4);
                    ImageFileHeader.Cols = BitConverter.ToInt32(m_byte, 0);
                    IsImageFileOpen = true;
                    return true;
                }
                catch
                {
                    IsImageFileOpen = false;
                    return false;
                }

            }
            return false;

        }
        return true;

    }
    /// <summary>
    /// Get MNIST Label file's header
    /// </summary>
    protected bool MnistLabelFileHeader()
    {

        if (IsLabelFileOpen == false)
        {
            var m_byte = new byte[4];
            var openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            openFileDialog1.Filter = "Mnist Label file (*.idx1-ubyte)|*.idx1-ubyte";
            openFileDialog1.Title = "Open MNIST Label file";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    MnistLabelFileName = openFileDialog1.FileName;
                    LabelFileStream = new System.IO.BinaryReader(openFileDialog1.OpenFile());
                    //Magic number 
                    LabelFileStream.Read(m_byte, 0, 4);
                    Array.Reverse(m_byte, 0, 4);
                    LabelFileHeader.Magic = BitConverter.ToInt32(m_byte, 0);
                    //number of images 
                    LabelFileStream.Read(m_byte, 0, 4);
                    //High-Endian format to Low-Endian format
                    Array.Reverse(m_byte, 0, 4);
                    LabelFileHeader.ItemsCount = BitConverter.ToInt32(m_byte, 0);
                    IsLabelFileOpen = true;
                    return true;
                }
                catch
                {
                    IsLabelFileOpen = false;
                    return false;
                }
            }
            return false;

        }
        return true;
    }
    /// <summary>
    /// // get current pattern number
    /// </summary>
    /// <param name="bFromRandomizedPatternSequence"></param>
    /// <returns></returns>
    public int GetCurrentPatternNumber(bool bFromRandomizedPatternSequence /* =FALSE */ )
    {
        // returns the current number of the training pattern, either from the straight sequence, or from
        // the randomized sequence

        return bFromRandomizedPatternSequence ? RandomizedPatternSequence[NextPattern] : (int)NextPattern;
    }
    public int GetNextPatternNumber(bool bFromRandomizedPatternSequence /* =FALSE */ )
    {
        // returns the current number of the training pattern, either from the straight sequence, or from
        // the randomized sequence
        if (NextPattern < ItemCount - 1)
        {
            NextPattern++;
        }
        else
        {
            NextPattern = 0;
        }
        int iRet;

        if (bFromRandomizedPatternSequence == false)
        {
            iRet = (int)NextPattern;
        }
        else
        {
            iRet = RandomizedPatternSequence[NextPattern];
        }

        return iRet;
    }
    public int GetRandomPatternNumber()
    {
        var rdm = new Random();
        int patternNum = (int)(rdm.NextDouble() * (ItemCount - 1));
        return patternNum;

    }
    public void RandomizePatternSequence()
    {
        // randomizes the order of m_iRandomizedTrainingPatternSequence, which is a UINT array
        // holding the numbers 0..59999 in random order
        //reset iNextPattern to 0
        NextPattern = 0;
        int ii, jj, iiMax;
        int iiTemp;

        iiMax = (int)ItemCount;
        // initialize array in sequential order

        for (ii = 0; ii < iiMax; ii++)
        {
            RandomizedPatternSequence[ii] = ii;
        }


        // now at each position, swap with a random position
        Random rdm = new Random();
        for (ii = 0; ii < iiMax; ii++)
        {
            //gives a uniformly-distributed number between zero (inclusive) and one (exclusive):(uint)(rdm.Next() / (0x7fff + 1))

            jj = (int)(rdm.NextDouble() * iiMax);

            iiTemp = RandomizedPatternSequence[ii];
            RandomizedPatternSequence[ii] = RandomizedPatternSequence[jj];
            RandomizedPatternSequence[jj] = iiTemp;
        }
        IsFromRandomizedPatternSequence = true;
    }
    /// <summary>
    /// //get value of pattern
    /// </summary>
    /// <param name="iNumImage"></param>
    /// <param name="pArray"></param>
    /// <param name="pLabel"></param>
    /// <param name="bFlipGrayscale"></param>
    protected void GetPatternArrayValues(out byte pLabel, int iNumImage = 0, byte[] pArray = null, bool bFlipGrayscale = true)
    {
        ////////
        uint cCount = Defaults.Global_ImageSize * Defaults.Global_ImageSize;
        long fPos;
        //
        if (IsImageFileOpen != false)
        {
            if (pArray != null)
            {
                fPos = 16 + iNumImage * cCount;  // 16 compensates for file header info
                //load_ImageFile_stream.Read(pArray,(int)fPos,(int)cCount);
                ImageFileStream.Read(pArray, 0, (int)cCount);
                if (bFlipGrayscale != false)
                {
                    for (int ii = 0; ii < cCount; ++ii)
                    {
                        pArray[ii] = (byte)(255 - pArray[ii]);
                    }
                }
            }
        }
        else  // no files are open: return a simple gray wedge
        {
            if (pArray != null)
            {
                for (int ii = 0; ii < cCount; ++ii)
                {
                    pArray[ii] = (byte)(ii * 255 / cCount);
                }
            }
        }
        //read label
        if (IsLabelFileOpen != false)
        {
            fPos = 8 + iNumImage;
            var m_byte = new byte[1];
            LabelFileStream.Read(m_byte, 0, 1);
            pLabel = m_byte[0];

        }
        else
        {

            pLabel = 255;
        }
    }
    protected uint GetNextPattern(byte[] pArray /* =NULL */, out byte pLabel /* =NULL */, int index, bool bFlipGrayscale /* =TRUE */ )
    {
        // returns the number of the pattern corresponding to the pattern stored in pArray
        GetPatternArrayValues(out pLabel, index, pArray, bFlipGrayscale);
        uint iRet = NextPattern;
        NextPattern++;
        if (NextPattern >= ItemCount)
        {
            NextPattern = 0;
        }
        return iRet;
    }

}
