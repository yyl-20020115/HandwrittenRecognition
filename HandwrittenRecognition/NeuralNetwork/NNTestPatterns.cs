using System;
using System.Collections.Generic;
using System.Threading;
using NeuralNetworkLibrary;
namespace HandwrittenRecogniration;

public class NNTestPatterns : NNForwardPropagation
{

    #region Parametters
    private readonly MnistDatabase _MnistDataSet;
    private uint _iMisNum;
    private uint _iNextPattern;
    readonly Mainform _form;
    #endregion
    public NNTestPatterns(NeuralNetwork neuronNet, MnistDatabase testtingSet, Preferences preferences, bool testingDataReady,
                        ManualResetEvent eventStop,
                        ManualResetEvent eventStopped,
                        Mainform form, List<Mutex> mutexs)
    {
        m_currentPatternIndex = 0;
        _bDataReady = testingDataReady;
        _NN = neuronNet;
        _iNextPattern = 0;
        m_EventStop = eventStop;
        m_EventStopped = eventStopped;
        _form = form;
        m_HiPerfTime = new HiPerfTimer();
        m_nImages = (uint)testtingSet.m_pImagePatterns.Count;

        //Initialize Gaussian Kernel
        m_Preferences = preferences;
        GetGaussianKernel(preferences.m_dElasticSigma);
        _MnistDataSet = testtingSet;
        m_Mutexs = mutexs;
    }
    public NNTestPatterns(NeuralNetwork neuronNet, Preferences preferences,
                        HandwrittenRecogniration.Mainform form, List<Mutex> mutexs)
    {
        m_currentPatternIndex = 0;
        _bDataReady = true;
        _NN = neuronNet;
        _iNextPattern = 0;
        m_EventStop = null;
        m_EventStopped = null;
        _form = form;
        m_HiPerfTime = new HiPerfTimer();
        m_nImages = 0;
        _iMisNum = 0;

        //Initialize Gaussian Kernel
        m_Preferences = preferences;
        GetGaussianKernel(preferences.m_dElasticSigma);
        _MnistDataSet = null;
        m_Mutexs = mutexs;
    }
    public void PatternsTestingThread(int iPatternNum)
    {
        // thread for backpropagation training of NN
        //
        // thread is "owned" by the doc, and accepts a pointer to the doc
        // continuously backpropagates until m_bThreadAbortFlag is set to TRUE  	
        double[] inputVector = new double[841];  // note: 29x29, not 28x28
        double[] targetOutputVector = new double[10];
        double[] actualOutputVector = new double[10];
        //
        for (int i = 0; i < 841; i++)
        {
            inputVector[i] = 0.0;
        }
        for (int i = 0; i < 10; i++)
        {
            targetOutputVector[i] = 0.0;
            actualOutputVector[i] = 0.0;

        }
        //
        byte label = 0;
        int ii, jj;


        var memorizedNeuronOutputs = new NNNeuronOutputsList();
        //prepare for training

        m_HiPerfTime.Start();

        while (_iNextPattern < iPatternNum)
        {
            m_Mutexs[1].WaitOne();

            byte[] grayLevels = new byte[m_Preferences.m_nRowsImages * m_Preferences.m_nColsImages];
            //iSequentialNum = m_MnistDataSet.GetCurrentPatternNumber(m_MnistDataSet.m_bFromRandomizedPatternSequence);
            _MnistDataSet.m_pImagePatterns[(int)_iNextPattern].pPattern.CopyTo(grayLevels, 0);
            label = _MnistDataSet.m_pImagePatterns[(int)_iNextPattern].nLabel;
            if (label < 0) label = 0;
            if (label > 9) label = 9;

            // pad to 29x29, convert to double precision

            for (ii = 0; ii < 841; ++ii)
            {
                inputVector[ii] = 1.0;  // one is white, -one is black
            }

            // top row of inputVector is left as zero, left-most column is left as zero 

            for (ii = 0; ii < DefaultDefinations.g_cImageSize; ++ii)
            {
                for (jj = 0; jj < DefaultDefinations.g_cImageSize; ++jj)
                {
                    inputVector[1 + jj + 29 * (ii + 1)] = (double)((int)(byte)grayLevels[jj + DefaultDefinations.g_cImageSize * ii]) / 128.0 - 1.0;  // one is white, -one is black
                }
            }

            // desired output vector

            for (ii = 0; ii < 10; ++ii)
            {
                targetOutputVector[ii] = -1.0;
            }
            targetOutputVector[label] = 1.0;
            // forward calculate through the neural net

            CalculateNeuralNet(inputVector, 841, actualOutputVector, 10, memorizedNeuronOutputs, false);

            int iBestIndex = 0;
            double maxValue = -99.0;

            for (ii = 0; ii < 10; ++ii)
            {
                if (actualOutputVector[ii] > maxValue)
                {
                    iBestIndex = ii;
                    maxValue = actualOutputVector[ii];
                }
            }
            string s = "";
            if (iBestIndex != label)
            {

                _iMisNum++;
                s = "Pattern No:" + _iNextPattern.ToString() + " Recognized value:" + iBestIndex.ToString() + " Actual value:" + label.ToString();
                if (_form != null)
                    _form.Invoke(_form._DelegateAddObject, new Object[] { 6, s });


            }
            else
            {
                s = _iNextPattern.ToString() + ", Mis Nums:" + _iMisNum.ToString();
                if (_form != null)
                    _form.Invoke(_form._DelegateAddObject, new Object[] { 7, s });
            }
            // check if thread is cancelled
            if (m_EventStop.WaitOne(0, true))
            {
                // clean-up operations may be placed here
                // ...
                s = String.Format("Mnist Testing thread: {0} stoped", Thread.CurrentThread.Name);
                // Make synchronous call to main form.
                // MainForm.AddString function runs in main thread.
                // To make asynchronous call use BeginInvoke
                if (_form != null)
                {
                    _form.Invoke(_form._DelegateAddObject, new Object[] { 8, s });
                }

                // inform main thread that this thread stopped
                m_EventStopped.Set();
                m_Mutexs[1].ReleaseMutex();
                return;
            }
            _iNextPattern++;
            m_Mutexs[1].ReleaseMutex();
        }
        {
            string s = String.Format("Mnist Testing thread: {0} stoped", Thread.CurrentThread.Name);
            _form.Invoke(_form._DelegateAddObject, new Object[] { 8, s });
        }
    }
    public void PatternRecognizingThread(int iPatternNo)
    {
        // thread for backpropagation training of NN
        //
        // thread is "owned" by the doc, and accepts a pointer to the doc
        // continuously backpropagates until m_bThreadAbortFlag is set to TRUE  	
        double[] inputVector = new double[841];  // note: 29x29, not 28x28
        double[] targetOutputVector = new double[10];
        double[] actualOutputVector = new double[10];
        //
        for (int i = 0; i < 841; i++)
        {
            inputVector[i] = 0.0;
        }
        for (int i = 0; i < 10; i++)
        {
            targetOutputVector[i] = 0.0;
            actualOutputVector[i] = 0.0;

        }


        byte label = 0;
        int ii, jj;


        var memorizedNeuronOutputs = new NNNeuronOutputsList();
        //prepare for training
        _iNextPattern = 0;
        _iMisNum = 0;


        m_Mutexs[1].WaitOne();
        if (_iNextPattern == 0)
        {
            m_HiPerfTime.Start();
        }
        byte[] grayLevels = new byte[m_Preferences.m_nRowsImages * m_Preferences.m_nColsImages];
        _MnistDataSet.m_pImagePatterns[iPatternNo].pPattern.CopyTo(grayLevels, 0);
        label = _MnistDataSet.m_pImagePatterns[iPatternNo].nLabel;
        _iNextPattern++;

        if (label < 0) label = 0;
        if (label > 9) label = 9;

        // pad to 29x29, convert to double precision

        for (ii = 0; ii < 841; ++ii)
        {
            inputVector[ii] = 1.0;  // one is white, -one is black
        }

        // top row of inputVector is left as zero, left-most column is left as zero 

        for (ii = 0; ii < DefaultDefinations.g_cImageSize; ++ii)
        {
            for (jj = 0; jj < DefaultDefinations.g_cImageSize; ++jj)
            {
                inputVector[1 + jj + 29 * (ii + 1)] = (double)((int)(byte)grayLevels[jj + DefaultDefinations.g_cImageSize * ii]) / 128.0 - 1.0;  // one is white, -one is black
            }
        }

        // desired output vector

        for (ii = 0; ii < 10; ++ii)
        {
            targetOutputVector[ii] = -1.0;
        }
        targetOutputVector[label] = 1.0;
        // forward calculate through the neural net

        CalculateNeuralNet(inputVector, 841, actualOutputVector, 10, memorizedNeuronOutputs, false);
        int iBestIndex = 0;
        double maxValue = -99.0;

        for (ii = 0; ii < 10; ++ii)
        {
            if (actualOutputVector[ii] > maxValue)
            {
                iBestIndex = ii;
                maxValue = actualOutputVector[ii];
            }
        }

        string s = iBestIndex.ToString();
        _form.Invoke(_form._DelegateAddObject, new Object[] { 2, s });
        // check if thread is cancelled
        m_Mutexs[1].ReleaseMutex();

    }
    public void PatternRecognizingThread(byte[] grayLevels)
    {
        // thread for backpropagation training of NN
        //
        // thread is "owned" by the doc, and accepts a pointer to the doc
        // continuously backpropagates until m_bThreadAbortFlag is set to TRUE  	
        double[] inputVector = new double[841];  // note: 29x29, not 28x28
        double[] targetOutputVector = new double[10];
        double[] actualOutputVector = new double[10];
        //
        for (int i = 0; i < 841; i++)
        {
            inputVector[i] = 0.0;
        }
        for (int i = 0; i < 10; i++)
        {
            targetOutputVector[i] = 0.0;
            actualOutputVector[i] = 0.0;

        }
        //

        byte label = 0;
        int ii, jj;


        var memorizedNeuronOutputs = new NNNeuronOutputsList();


        m_Mutexs[1].WaitOne();
        if (_iNextPattern == 0)
        {
            m_HiPerfTime.Start();
        }
        if (label < 0) label = 0;
        if (label > 9) label = 9;

        // pad to 29x29, convert to double precision

        for (ii = 0; ii < 841; ++ii)
        {
            inputVector[ii] = 1.0;  // one is white, -one is black
        }

        // top row of inputVector is left as zero, left-most column is left as zero 

        for (ii = 0; ii < DefaultDefinations.g_cImageSize; ++ii)
        {
            for (jj = 0; jj < DefaultDefinations.g_cImageSize; ++jj)
            {
                inputVector[1 + jj + 29 * (ii + 1)] = (double)((int)(byte)grayLevels[jj + DefaultDefinations.g_cImageSize * ii]) / 128.0 - 1.0;  // one is white, -one is black
            }
        }

        // desired output vector

        for (ii = 0; ii < 10; ++ii)
        {
            targetOutputVector[ii] = -1.0;
        }
        targetOutputVector[label] = 1.0;
        // forward calculate through the neural net

        CalculateNeuralNet(inputVector, 841, actualOutputVector, 10, memorizedNeuronOutputs, false);
        int iBestIndex = 0;
        double maxValue = -99.0;

        for (ii = 0; ii < 10; ++ii)
        {
            if (actualOutputVector[ii] > maxValue)
            {
                iBestIndex = ii;
                maxValue = actualOutputVector[ii];
            }
        }

        string s = iBestIndex.ToString();
        _form.Invoke(_form._DelegateAddObject, new Object[] { 1, s });
        // check if thread is cancelled

        m_Mutexs[1].ReleaseMutex();

    }
}
