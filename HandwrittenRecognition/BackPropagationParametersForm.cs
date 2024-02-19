using System;
using System.Windows.Forms;

namespace HandwrittenRecogniration;

public struct BackPropagationParameters
{
    public uint m_AfterEvery;
    public double m_EtaDecay;
    public double m_InitialEta;
    public double m_MinimumEta;
    public string m_strInitialEtaMessage;
    public string m_strStartingPatternNum;
    public uint m_StartingPattern;
    public uint m_cNumThreads;
    public bool m_bDistortPatterns;
    public double m_EstimatedCurrentMSE;
}

public partial class BackPropagationParametersForm : Form
{

    private BackPropagationParameters m_Parameters;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    public void SetBackProParameters(BackPropagationParameters value)
    {
        m_Parameters = value;
        textBoxAfterEveryNBackPropagations.Text = m_Parameters.m_AfterEvery.ToString();
        textBoxBackThreads.Text = m_Parameters.m_cNumThreads.ToString();
        textBoxEstimateofCurrentMSE.Text = m_Parameters.m_EstimatedCurrentMSE.ToString();
        textBoxILearningRateEta.Text = m_Parameters.m_InitialEta.ToString();
        textBoxLearningRateDecayRate.Text = m_Parameters.m_EtaDecay.ToString();
        textBoxMinimumLearningRate.Text = m_Parameters.m_MinimumEta.ToString();
        textBoxStartingPatternNumber.Text = m_Parameters.m_StartingPattern.ToString();
        checkBoxDistortPatterns.Checked = m_Parameters.m_bDistortPatterns;
    }
    public BackPropagationParameters GetBackProParameters()
    {
        return m_Parameters;
    }
    public BackPropagationParametersForm()
    {
        InitializeComponent();
        m_Parameters.m_AfterEvery = 0;
        m_Parameters.m_bDistortPatterns = true;
        m_Parameters.m_cNumThreads = 0;
        m_Parameters.m_EstimatedCurrentMSE = 0;
        m_Parameters.m_EtaDecay = 0;
        m_Parameters.m_InitialEta = 0;
        m_Parameters.m_MinimumEta = 0;
        m_Parameters.m_StartingPattern = 0;
        m_Parameters.m_strInitialEtaMessage = "";
        m_Parameters.m_strStartingPatternNum = "";
    }


    private void Button1_Click(object sender, EventArgs e)
    {
        m_Parameters.m_AfterEvery = Convert.ToUInt32(textBoxAfterEveryNBackPropagations.Text);
        m_Parameters.m_cNumThreads = Convert.ToUInt32(textBoxBackThreads.Text);
        m_Parameters.m_EstimatedCurrentMSE = Convert.ToDouble(textBoxEstimateofCurrentMSE.Text);
        m_Parameters.m_InitialEta = Convert.ToDouble(textBoxILearningRateEta.Text);
        m_Parameters.m_EtaDecay = Convert.ToDouble(textBoxLearningRateDecayRate.Text);
        m_Parameters.m_MinimumEta = Convert.ToDouble(textBoxMinimumLearningRate.Text);
        m_Parameters.m_StartingPattern = Convert.ToUInt32(textBoxStartingPatternNumber.Text);
        m_Parameters.m_bDistortPatterns = checkBoxDistortPatterns.Checked;
    }
}
