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

    public override string ToString() => $"{this.Label}: " + string.Join(", ", Pattern);
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
    protected bool _IsDatabaseReady;
    protected bool _IsFromRandomizedPatternSequence;
    protected BinaryReader ImageFileStream;
    protected BinaryReader LabelFileStream;

    public List<ImagePattern> ImagePatterns;
    public bool IsFromRandomizedPatternSequence 
        => _IsFromRandomizedPatternSequence;
    public bool IsDatabaseReady 
        => _IsDatabaseReady;

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
        _IsDatabaseReady = false;
        RandomizedPatternSequence = null;
        _IsFromRandomizedPatternSequence = false;
    }
    public bool LoadMinstFiles(string image,string label)
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
        //load Mnist Images files.
        if (!MnistImageFileHeader(image))
        {
            MnistImageFileName = null;
            IsImageFileOpen = false;
            _IsDatabaseReady = false;
            return false;
        }
        if (!MnistLabelFileHeader(label))
        {
            MnistLabelFileName = null;
            IsLabelFileOpen = false;
            _IsDatabaseReady = false;
            return false;
        }
        //check the value if image file and label file have been opened successfully
        if (LabelFileHeader.ItemsCount != ImageFileHeader.ItemsCount)
        {
            CloseMinstFiles();
            _IsDatabaseReady = false;
            return false;
        }
        ImagePatterns = new (ImageFileHeader.ItemsCount);
        RandomizedPatternSequence = new int[ImageFileHeader.ItemsCount];
        for (int i = 0; i < ImageFileHeader.ItemsCount; i++)
        {
            var PatternArray = new byte[Defaults.Global_ImageSize * Defaults.Global_ImageSize];
            GetNextPattern(PatternArray, out byte pattern_label, i, true);
            var ImagePattern = new ImagePattern
            {
                Pattern = PatternArray,
                Label = pattern_label
            };
            ImagePatterns.Add(ImagePattern);
        }
        _IsDatabaseReady = true;
        CloseMinstFiles();
        return true;
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
        //load Mnist Images files.
        if (!MnistImageFileHeader())
        {
            MessageBox.Show("Can not open Image file");
            MnistImageFileName = null;
            IsImageFileOpen = false;
            _IsDatabaseReady = false;
            return false;
        }
        if (!MnistLabelFileHeader())
        {
            MessageBox.Show("Can not open label file");
            MnistLabelFileName = null;
            IsLabelFileOpen = false;
            _IsDatabaseReady = false;
            return false;
        }
        //check the value if image file and label file have been opened successfully
        if (LabelFileHeader.ItemsCount != ImageFileHeader.ItemsCount)
        {
            MessageBox.Show("Item numbers are different");
            CloseMinstFiles();
            _IsDatabaseReady = false;
            return false;
        }
        ImagePatterns = new (ImageFileHeader.ItemsCount);
        RandomizedPatternSequence = new int[ImageFileHeader.ItemsCount];
        for (int i = 0; i < ImageFileHeader.ItemsCount; i++)
        {
            var PatternArray = new byte[Defaults.Global_ImageSize * Defaults.Global_ImageSize];
            GetNextPattern(PatternArray, out byte label, i, true);
            var ImagePattern = new ImagePattern
            {
                Pattern = PatternArray,
                Label = label
            };
            ImagePatterns.Add(ImagePattern);
        }
        _IsDatabaseReady = true;
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

    protected bool MnistImageFileHeader(string file)
    {
        try
        {
            var signature = new byte[4];

            ImageFileStream = new BinaryReader(new FileStream(file,FileMode.Open));
            //Magic number 
            ImageFileStream.Read(signature, 0, 4);
            Array.Reverse(signature, 0, 4);
            ImageFileHeader.Magic = BitConverter.ToInt32(signature, 0);
            //number of images 
            ImageFileStream.Read(signature, 0, 4);
            //High-Endian format to Low-Endian format
            Array.Reverse(signature, 0, 4);
            ImageFileHeader.ItemsCount = BitConverter.ToInt32(signature, 0);
            ItemCount = (uint)ImageFileHeader.ItemsCount;
            //number of rows 
            ImageFileStream.Read(signature, 0, 4);
            Array.Reverse(signature, 0, 4);
            ImageFileHeader.Rows = BitConverter.ToInt32(signature, 0);
            //number of columns 
            ImageFileStream.Read(signature, 0, 4);
            Array.Reverse(signature, 0, 4);
            ImageFileHeader.Cols = BitConverter.ToInt32(signature, 0);
            IsImageFileOpen = true;
            return true;
        }
        catch
        {
            IsImageFileOpen = false;
            return false;
        }
    }
    /// <summary>
    /// //Get MNIST Image file'header
    /// </summary>
    protected bool MnistImageFileHeader()
    {

        if (IsImageFileOpen == false)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Mnist Image file (*.idx3-ubyte)|*.idx3-ubyte",
                Title = "Open Minist Image File",
                InitialDirectory = Environment.CurrentDirectory
            };
            return dialog.ShowDialog() == DialogResult.OK && MnistImageFileHeader
                    (MnistImageFileName = dialog.FileName);
        }
        return true;

    }

    protected bool MnistLabelFileHeader(string file)
    {
        try
        {
            var signature = new byte[4];

            LabelFileStream = new System.IO.BinaryReader(new FileStream(file, FileMode.Open));
            //Magic number 
            LabelFileStream.Read(signature, 0, 4);
            Array.Reverse(signature, 0, 4);
            LabelFileHeader.Magic = BitConverter.ToInt32(signature, 0);
            //number of images 
            LabelFileStream.Read(signature, 0, 4);
            //High-Endian format to Low-Endian format
            Array.Reverse(signature, 0, 4);
            LabelFileHeader.ItemsCount = BitConverter.ToInt32(signature, 0);
            IsLabelFileOpen = true;
            return true;
        }
        catch
        {
            IsLabelFileOpen = false;
            return false;
        }
    }
    /// <summary>
    /// Get MNIST Label file's header
    /// </summary>
    protected bool MnistLabelFileHeader()
    {
        if (IsLabelFileOpen == false)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Mnist Label file (*.idx1-ubyte)|*.idx1-ubyte",
                Title = "Open MNIST Label file"
            };
            return dialog.ShowDialog() == DialogResult.OK
                && this.MnistLabelFileHeader(
                    MnistLabelFileName = dialog.FileName);
        }
        return true;
    }
    /// <summary>
    /// // get current pattern number
    /// </summary>
    /// <param name="bFromRandomizedPatternSequence"></param>
    /// <returns></returns>
    public int GetCurrentPatternNumber(bool bFromRandomizedPatternSequence /* =FALSE */ ) =>
        // returns the current number of the training pattern, either from the straight sequence, or from
        // the randomized sequence

        bFromRandomizedPatternSequence ? RandomizedPatternSequence[NextPattern] : (int)NextPattern;
    public int GetNextPatternNumber(bool bFromRandomizedPatternSequence /* =FALSE */ )
    {
        // returns the current number of the training pattern, either from the straight sequence, or from
        // the randomized sequence
        NextPattern = NextPattern < ItemCount - 1 ? NextPattern + 1 : 0;
        return bFromRandomizedPatternSequence ? RandomizedPatternSequence[NextPattern] : (int)NextPattern;
    }
    private readonly Random random = new ();

    public int GetRandomPatternNumber() 
        => (int)(random.NextDouble() * (ItemCount - 1));
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
        for (ii = 0; ii < iiMax; ii++)
        {
            //gives a uniformly-distributed number between zero (inclusive) and one (exclusive):(uint)(rdm.Next() / (0x7fff + 1))
            jj = (int)(random.NextDouble() * iiMax);
            iiTemp = RandomizedPatternSequence[ii];
            RandomizedPatternSequence[ii] = RandomizedPatternSequence[jj];
            RandomizedPatternSequence[jj] = iiTemp;
        }
        _IsFromRandomizedPatternSequence = true;
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
        if (IsImageFileOpen)
        {
            if (pArray != null)
            {
                fPos = 16 + iNumImage * cCount;  // 16 compensates for file header info
                //load_ImageFile_stream.Read(pArray,(int)fPos,(int)cCount);
                ImageFileStream.Read(pArray, 0, (int)cCount);
                if (bFlipGrayscale)
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
        if (IsLabelFileOpen)
        {
            fPos = 8 + iNumImage;
            var signature = new byte[1];
            LabelFileStream.Read(signature, 0, 1);
            pLabel = signature[0];

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
