using System;
using System.Windows.Forms;

namespace HandwrittenRecogniration;

public struct BackPropagationParameters
{
    public uint AfterEvery;
    public double EtaDecay;
    public double InitialEta;
    public double MinimumEta;
    public string InitialEtaMessage;
    public string StartingPatternNum;
    public uint StartingPattern;
    public uint NumThreads;
    public bool UseDistortPatterns;
    public double EstimatedCurrentMSE;
}

public partial class BackPropagationParametersForm : Form
{

    private BackPropagationParameters Parameters;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public BackPropagationParameters BackProParameters
    {
        get => Parameters;
        set
        {
            Parameters = value;
            textBoxAfterEveryNBackPropagations.Text = Parameters.AfterEvery.ToString();
            textBoxBackThreads.Text = Parameters.NumThreads.ToString();
            textBoxEstimateofCurrentMSE.Text = Parameters.EstimatedCurrentMSE.ToString();
            textBoxILearningRateEta.Text = Parameters.InitialEta.ToString();
            textBoxLearningRateDecayRate.Text = Parameters.EtaDecay.ToString();
            textBoxMinimumLearningRate.Text = Parameters.MinimumEta.ToString();
            textBoxStartingPatternNumber.Text = Parameters.StartingPattern.ToString();
            checkBoxDistortPatterns.Checked = Parameters.UseDistortPatterns;
        }
    }

    public BackPropagationParametersForm()
    {
        InitializeComponent();
        Parameters.AfterEvery = 0;
        Parameters.UseDistortPatterns = true;
        Parameters.NumThreads = 0;
        Parameters.EstimatedCurrentMSE = 0;
        Parameters.EtaDecay = 0;
        Parameters.InitialEta = 0;
        Parameters.MinimumEta = 0;
        Parameters.StartingPattern = 0;
        Parameters.InitialEtaMessage = "";
        Parameters.StartingPatternNum = "";
    }


    private void StartButton_Click(object sender, EventArgs e)
    {
        uint.TryParse(textBoxAfterEveryNBackPropagations.Text,out Parameters.AfterEvery);
        uint.TryParse(textBoxBackThreads.Text,out Parameters.NumThreads);
        double.TryParse(textBoxEstimateofCurrentMSE.Text,out Parameters.EstimatedCurrentMSE);
        double.TryParse(textBoxILearningRateEta.Text, out Parameters.InitialEta);
        double.TryParse(textBoxLearningRateDecayRate.Text, out Parameters.EtaDecay);
        double.TryParse(textBoxMinimumLearningRate.Text, out Parameters.MinimumEta);
        uint.TryParse(textBoxStartingPatternNumber.Text, out Parameters.StartingPattern);
        Parameters.UseDistortPatterns = checkBoxDistortPatterns.Checked;
    }
}
