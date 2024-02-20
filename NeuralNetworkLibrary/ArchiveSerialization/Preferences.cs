using System;

namespace NeuralNetworkLibrary;

public class Preferences
{
    public const int g_cImageSize = 28;
    public const int g_cVectorSize = 29;

    public int m_cNumBackpropThreads;

    public uint m_nMagicTrainingLabels;
    public uint m_nMagicTrainingImages;

    public uint m_nItemsTrainingLabels;
    public uint m_nItemsTrainingImages;

    public int m_cNumTestingThreads;

    public int m_nMagicTestingLabels;
    public int m_nMagicTestingImages;

    public uint m_nItemsTestingLabels;
    public uint m_nItemsTestingImages;

    public uint m_nRowsImages;
    public uint m_nColsImages;

    public int m_nMagWindowSize;
    public int m_nMagWindowMagnification;

    public double m_dInitialEtaLearningRate;
    public double m_dLearningRateDecay;
    public double m_dMinimumEtaLearningRate;
    public uint m_nAfterEveryNBackprops;

    // for limiting the step size in backpropagation, since we are using second order
    // "Stochastic Diagonal Levenberg-Marquardt" update algorithm.  See Yann LeCun 1998
    // "Gradianet-Based Learning Applied to Document Recognition" at page 41

    public double m_dMicronLimitParameter;
    public uint m_nNumHessianPatterns;

    // for distortions of the input image, in an attempt to improve generalization

    public double m_dMaxScaling;  // as a percentage, such as 20.0 for plus/minus 20%
    public double m_dMaxRotation;  // in degrees, such as 20.0 for plus/minus rotations of 20 degrees
    public double m_dElasticSigma;  // one sigma value for randomness in Simard's elastic distortions
    public double m_dElasticScaling;  // after-smoohting scale factor for Simard's elastic distortions
    private IniFile m_Inifile;
    ////////////
    public Preferences()
    {
        // set default values

        m_nMagicTrainingLabels = 0x00000801;
        m_nMagicTrainingImages = 0x00000803;

        m_nItemsTrainingLabels = 60000;
        m_nItemsTrainingImages = 60000;

        m_nMagicTestingLabels = 0x00000801;
        m_nMagicTestingImages = 0x00000803;

        m_nItemsTestingLabels = 10000;
        m_nItemsTestingImages = 10000;

        m_nRowsImages = g_cImageSize;
        m_nColsImages = g_cImageSize;

        m_nMagWindowSize = 5;
        m_nMagWindowMagnification = 8;

        m_dInitialEtaLearningRate = 0.001;
        m_dLearningRateDecay = 0.794328235;  // 0.794328235 = 0.001 down to 0.00001 in 20 epochs 
        m_dMinimumEtaLearningRate = 0.00001;
        m_nAfterEveryNBackprops = 60000;
        m_cNumBackpropThreads = 2;

        m_cNumTestingThreads = 1;

        // parameters for controlling distortions of input image

        m_dMaxScaling = 15.0;  // like 20.0 for 20%
        m_dMaxRotation = 15.0;  // like 20.0 for 20 degrees
        m_dElasticSigma = 8.0;  // higher numbers are more smooth and less distorted; Simard uses 4.0
        m_dElasticScaling = 0.5;  // higher numbers amplify the distortions; Simard uses 34 (sic, maybe 0.34 ??)

        // for limiting the step size in backpropagation, since we are using second order
        // "Stochastic Diagonal Levenberg-Marquardt" update algorithm.  See Yann LeCun 1998
        // "Gradient-Based Learning Applied to Document Recognition" at page 41

        m_dMicronLimitParameter = 0.10;  // since we divide by this, update can never be more than 10x current eta
        m_nNumHessianPatterns = 500;  // number of patterns used to calculate the diagonal Hessian
        String path = System.IO.Directory.GetCurrentDirectory() + "\\Data\\Default-ini.ini";
        m_Inifile = new IniFile(path);
        ReadIniFile();
    }
    public void ReadIniFile()
    {
        // now read values from the ini file

        String tSection;

        // Neural Network parameters

        tSection = "Neural Network Parameters";

        Get(tSection, "Initial learning rate (eta)", ref m_dInitialEtaLearningRate);
        Get(tSection, "Minimum learning rate (eta)", ref m_dMinimumEtaLearningRate);
        Get(tSection, "Rate of decay for learning rate (eta)", ref m_dLearningRateDecay);
        Get(tSection, "Decay rate is applied after this number of backprops", ref m_nAfterEveryNBackprops);
        Get(tSection, "Number of backprop threads", ref m_cNumBackpropThreads);
        Get(tSection, "Number of testing threads", ref m_cNumTestingThreads);
        Get(tSection, "Number of patterns used to calculate Hessian", ref m_nNumHessianPatterns);
        Get(tSection, "Limiting divisor (micron) for learning rate amplification (like 0.10 for 10x limit)", ref m_dMicronLimitParameter);


        // Neural Network Viewer parameters

        tSection = "Neural Net Viewer Parameters";

        Get(tSection, "Size of magnification window", ref m_nMagWindowSize);
        Get(tSection, "Magnification factor for magnification window", ref m_nMagWindowMagnification);


        // MNIST data collection parameters

        tSection = "MNIST Database Parameters";

        Get(tSection, "Training images magic number", ref m_nMagicTrainingImages);
        Get(tSection, "Training images item count", ref m_nItemsTrainingImages);
        Get(tSection, "Training labels magic number", ref m_nMagicTrainingLabels);
        Get(tSection, "Training labels item count", ref m_nItemsTrainingLabels);

        Get(tSection, "Testing images magic number", ref m_nMagicTestingImages);
        Get(tSection, "Testing images item count", ref m_nItemsTestingImages);
        Get(tSection, "Testing labels magic number", ref m_nMagicTestingLabels);
        Get(tSection, "Testing labels item count", ref m_nItemsTestingLabels);

        // these two are basically ignored

        uint uiCount = g_cImageSize;
        if(Get(tSection, "Rows per image", ref uiCount))
            m_nRowsImages = uiCount;

        uiCount = g_cImageSize;
        if(Get(tSection, "Columns per image", ref uiCount))
            m_nColsImages = uiCount;


        // parameters for controlling pattern distortion during backpropagation

        tSection = "Parameters for Controlling Pattern Distortion During Backpropagation";

        Get(tSection, "Maximum scale factor change (percent, like 20.0 for 20%)", ref m_dMaxScaling);
        Get(tSection, "Maximum rotational change (degrees, like 20.0 for 20 degrees)", ref m_dMaxRotation);
        Get(tSection, "Sigma for elastic distortions (higher numbers are more smooth and less distorted; Simard uses 4.0)", ref m_dElasticSigma);
        Get(tSection, "Scaling for elastic distortions (higher numbers amplify distortions; Simard uses 0.34)", ref m_dElasticScaling);
    }
    private bool Get(string lpAppName, string lpKeyName, ref int nDefault)
    {
        var r = int.TryParse(m_Inifile.IniReadValue(lpAppName, lpKeyName), out var d);
        if (r)
        {
            nDefault = d;
        }
        return false;
    }
    private bool Get(string lpAppName, string lpKeyName, ref uint nDefault)
    {
        var r = uint.TryParse(m_Inifile.IniReadValue(lpAppName, lpKeyName), out var d);
        if (r)
        {
            nDefault = d;
        }
        return false;
    }

    private bool Get(string lpAppName, string lpKeyName, ref double nDefault) 
    {
        var r = double.TryParse(m_Inifile.IniReadValue(lpAppName, lpKeyName), out var d);
        if (r)
        {
            nDefault = d;
        }
        return false;
    }
    private bool Get(string lpAppName, string lpKeyName, ref byte nDefault) {
        var r = byte.TryParse(m_Inifile.IniReadValue(lpAppName, lpKeyName), out var d);
        if (r)
        {
            nDefault = d;
        }
        return false;
    }

    private bool Get(string lpAppName, string lpKeyName, ref string nDefault) => !string.IsNullOrEmpty(nDefault = m_Inifile.IniReadValue(lpAppName, lpKeyName));
    private bool Get(string lpAppName, string lpKeyName, ref bool nDefault) 
    {
        var r = bool.TryParse(m_Inifile.IniReadValue(lpAppName, lpKeyName), out var d);
        if (r)
        {
            nDefault = d;
        }
        return false;
    }

}
