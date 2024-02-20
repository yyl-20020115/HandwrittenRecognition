using System;
using System.IO;

namespace NeuralNetworkLibrary;

public class Preferences
{
    public const int Global_ImageSize = 28;
    public const int Global_VectorSize = 29;

    public int NumBackpropThreads;

    public uint MagicTrainingLabels;
    public uint MagicTrainingImages;

    public uint ItemsTrainingLabels;
    public uint ItemsTrainingImages;

    public int NumTestingThreads;

    public int MagicTestingLabels;
    public int MagicTestingImages;

    public uint ItemsTestingLabels;
    public uint ItemsTestingImages;

    public uint RowsImages;
    public uint ColsImages;

    public int MagWindowSize;
    public int MagWindowMagnification;

    public double InitialEtaLearningRate;
    public double LearningRateDecay;
    public double MinimumEtaLearningRate;
    public uint AfterEveryNBackprops;

    // for limiting the step size in backpropagation, since we are using second order
    // "Stochastic Diagonal Levenberg-Marquardt" update algorithm.  See Yann LeCun 1998
    // "Gradianet-Based Learning Applied to Document Recognition" at page 41

    public double MicronLimitParameter;
    public uint NumHessianPatterns;

    // for distortions of the input image, in an attempt to improve generalization

    public double MaxScaling;  // as a percentage, such as 20.0 for plus/minus 20%
    public double MaxRotation;  // in degrees, such as 20.0 for plus/minus rotations of 20 degrees
    public double ElasticSigma;  // one sigma value for randomness in Simard's elastic distortions
    public double ElasticScaling;  // after-smoohting scale factor for Simard's elastic distortions
    private readonly IniFile Inifile;
    ////////////
    public Preferences()
    {
        // set default values

        MagicTrainingLabels = 0x00000801;
        MagicTrainingImages = 0x00000803;

        ItemsTrainingLabels = 60000;
        ItemsTrainingImages = 60000;

        MagicTestingLabels = 0x00000801;
        MagicTestingImages = 0x00000803;

        ItemsTestingLabels = 10000;
        ItemsTestingImages = 10000;

        RowsImages = Global_ImageSize;
        ColsImages = Global_ImageSize;

        MagWindowSize = 5;
        MagWindowMagnification = 8;

        InitialEtaLearningRate = 0.001;
        LearningRateDecay = 0.794328235;  // 0.794328235 = 0.001 down to 0.00001 in 20 epochs 
        MinimumEtaLearningRate = 0.00001;
        AfterEveryNBackprops = 60000;
        NumBackpropThreads = 2;

        NumTestingThreads = 1;

        // parameters for controlling distortions of input image

        MaxScaling = 15.0;  // like 20.0 for 20%
        MaxRotation = 15.0;  // like 20.0 for 20 degrees
        ElasticSigma = 8.0;  // higher numbers are more smooth and less distorted; Simard uses 4.0
        ElasticScaling = 0.5;  // higher numbers amplify the distortions; Simard uses 34 (sic, maybe 0.34 ??)

        // for limiting the step size in backpropagation, since we are using second order
        // "Stochastic Diagonal Levenberg-Marquardt" update algorithm.  See Yann LeCun 1998
        // "Gradient-Based Learning Applied to Document Recognition" at page 41

        MicronLimitParameter = 0.10;  // since we divide by this, update can never be more than 10x current eta
        NumHessianPatterns = 500;  // number of patterns used to calculate the diagonal Hessian
        var path = Path.Combine(Directory.GetCurrentDirectory(),"Data","Default-ini.ini");
        Inifile = new IniFile(path);
        ReadIniFile();
    }
    public void ReadIniFile()
    {
        // now read values from the ini file

        var section = "";

        // Neural Network parameters

        section = "Neural Network Parameters";

        Get(section, "Initial learning rate (eta)", ref InitialEtaLearningRate);
        Get(section, "Minimum learning rate (eta)", ref MinimumEtaLearningRate);
        Get(section, "Rate of decay for learning rate (eta)", ref LearningRateDecay);
        Get(section, "Decay rate is applied after this number of backprops", ref AfterEveryNBackprops);
        Get(section, "Number of backprop threads", ref NumBackpropThreads);
        Get(section, "Number of testing threads", ref NumTestingThreads);
        Get(section, "Number of patterns used to calculate Hessian", ref NumHessianPatterns);
        Get(section, "Limiting divisor (micron) for learning rate amplification (like 0.10 for 10x limit)", ref MicronLimitParameter);


        // Neural Network Viewer parameters

        section = "Neural Net Viewer Parameters";

        Get(section, "Size of magnification window", ref MagWindowSize);
        Get(section, "Magnification factor for magnification window", ref MagWindowMagnification);


        // MNIST data collection parameters

        section = "MNIST Database Parameters";

        Get(section, "Training images magic number", ref MagicTrainingImages);
        Get(section, "Training images item count", ref ItemsTrainingImages);
        Get(section, "Training labels magic number", ref MagicTrainingLabels);
        Get(section, "Training labels item count", ref ItemsTrainingLabels);

        Get(section, "Testing images magic number", ref MagicTestingImages);
        Get(section, "Testing images item count", ref ItemsTestingImages);
        Get(section, "Testing labels magic number", ref MagicTestingLabels);
        Get(section, "Testing labels item count", ref ItemsTestingLabels);

        // these two are basically ignored

        uint uiCount = Global_ImageSize;
        Get(section, "Rows per image", ref uiCount);

        uiCount = Global_ImageSize;
        Get(section, "Columns per image", ref uiCount);

        // parameters for controlling pattern distortion during backpropagation

        section = "Parameters for Controlling Pattern Distortion During Backpropagation";

        Get(section, "Maximum scale factor change (percent, like 20.0 for 20%)", ref MaxScaling);
        Get(section, "Maximum rotational change (degrees, like 20.0 for 20 degrees)", ref MaxRotation);
        Get(section, "Sigma for elastic distortions (higher numbers are more smooth and less distorted; Simard uses 4.0)", ref ElasticSigma);
        Get(section, "Scaling for elastic distortions (higher numbers amplify distortions; Simard uses 0.34)", ref ElasticScaling);
    }
    private bool Get(string lpAppName, string lpKeyName, ref int nDefault)
    {
        var r = int.TryParse(Inifile.IniReadValue(lpAppName, lpKeyName), out var d);
        if (r)
        {
            nDefault = d;
        }
        return false;
    }
    public bool Get(string lpAppName, string lpKeyName, ref uint nDefault)
    {
        var r = uint.TryParse(Inifile.IniReadValue(lpAppName, lpKeyName), out var d);
        if (r)
        {
            nDefault = d;
        }
        return false;
    }

    public bool Get(string lpAppName, string lpKeyName, ref double nDefault) 
    {
        var r = double.TryParse(Inifile.IniReadValue(lpAppName, lpKeyName), out var d);
        if (r)
        {
            nDefault = d;
        }
        return false;
    }
    public bool Get(string lpAppName, string lpKeyName, ref byte nDefault) {
        var r = byte.TryParse(Inifile.IniReadValue(lpAppName, lpKeyName), out var d);
        if (r)
        {
            nDefault = d;
        }
        return false;
    }

    public bool Get(string lpAppName, string lpKeyName, ref string nDefault) => !string.IsNullOrEmpty(nDefault = Inifile.IniReadValue(lpAppName, lpKeyName));
    public bool Get(string lpAppName, string lpKeyName, ref bool nDefault) 
    {
        var r = bool.TryParse(Inifile.IniReadValue(lpAppName, lpKeyName), out var d);
        if (r)
        {
            nDefault = d;
        }
        return false;
    }

}
