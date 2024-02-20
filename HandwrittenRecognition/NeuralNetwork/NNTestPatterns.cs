using System;
using System.Collections.Generic;
using System.Threading;
using NeuralNetworkLibrary;
namespace HandwrittenRecogniration;

public class NNTestPatterns : NNForwardPropagation
{

    #region Parametters
    private readonly MnistDatabase MnistDataSet;
    private uint MnistNum;
    private uint NextPattern;
    readonly Mainform MainForm;
    #endregion
    public NNTestPatterns(NeuralNetwork neuronNet, MnistDatabase testtingSet, Preferences preferences, bool testingDataReady,
                        ManualResetEvent eventStop,
                        ManualResetEvent eventStopped,
                        Mainform form, List<Mutex> mutexs)
    {
        CurrentPatternIndex = 0;
        IsDataReady = testingDataReady;
        Network = neuronNet;
        NextPattern = 0;
        StopEvent = eventStop;
        StoppedEvent = eventStopped;
        MainForm = form;
        Timer = new HiPerfTimer();
        ImageCount = (uint)testtingSet.m_pImagePatterns.Count;

        //Initialize Gaussian Kernel
        Preferences = preferences;
        GetGaussianKernel(preferences.ElasticSigma);
        MnistDataSet = testtingSet;
        Mutexs = mutexs;
    }
    public NNTestPatterns(NeuralNetwork neuronNet, Preferences preferences,
                        HandwrittenRecogniration.Mainform form, List<Mutex> mutexs)
    {
        CurrentPatternIndex = 0;
        IsDataReady = true;
        Network = neuronNet;
        NextPattern = 0;
        StopEvent = null;
        StoppedEvent = null;
        MainForm = form;
        Timer = new ();
        ImageCount = 0;
        MnistNum = 0;

        //Initialize Gaussian Kernel
        Preferences = preferences;
        GetGaussianKernel(preferences.ElasticSigma);
        MnistDataSet = null;
        Mutexs = mutexs;
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

        Timer.Start();

        while (NextPattern < iPatternNum)
        {
            Mutexs[1].WaitOne();

            byte[] grayLevels = new byte[Preferences.RowsImages * Preferences.ColsImages];
            //iSequentialNum = m_MnistDataSet.GetCurrentPatternNumber(m_MnistDataSet.m_bFromRandomizedPatternSequence);
            MnistDataSet.m_pImagePatterns[(int)NextPattern].pPattern.CopyTo(grayLevels, 0);
            label = MnistDataSet.m_pImagePatterns[(int)NextPattern].nLabel;
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

                MnistNum++;
                s = "Pattern No:" + NextPattern.ToString() + " Recognized value:" + iBestIndex.ToString() + " Actual value:" + label.ToString();
                if (MainForm != null)
                    MainForm.Invoke(MainForm._DelegateAddObject, new Object[] { 6, s });


            }
            else
            {
                s = NextPattern.ToString() + ", Mis Nums:" + MnistNum.ToString();
                if (MainForm != null)
                    MainForm.Invoke(MainForm._DelegateAddObject, new Object[] { 7, s });
            }
            // check if thread is cancelled
            if (StopEvent.WaitOne(0, true))
            {
                // clean-up operations may be placed here
                // ...
                s = String.Format("Mnist Testing thread: {0} stoped", Thread.CurrentThread.Name);
                // Make synchronous call to main form.
                // MainForm.AddString function runs in main thread.
                // To make asynchronous call use BeginInvoke
                if (MainForm != null)
                {
                    MainForm.Invoke(MainForm._DelegateAddObject, new Object[] { 8, s });
                }

                // inform main thread that this thread stopped
                StoppedEvent.Set();
                Mutexs[1].ReleaseMutex();
                return;
            }
            NextPattern++;
            Mutexs[1].ReleaseMutex();
        }
        {
            string s = String.Format("Mnist Testing thread: {0} stoped", Thread.CurrentThread.Name);
            MainForm.Invoke(MainForm._DelegateAddObject, new Object[] { 8, s });
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
        NextPattern = 0;
        MnistNum = 0;


        Mutexs[1].WaitOne();
        if (NextPattern == 0)
        {
            Timer.Start();
        }
        byte[] grayLevels = new byte[Preferences.RowsImages * Preferences.ColsImages];
        MnistDataSet.m_pImagePatterns[iPatternNo].pPattern.CopyTo(grayLevels, 0);
        label = MnistDataSet.m_pImagePatterns[iPatternNo].nLabel;
        NextPattern++;

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
        MainForm.Invoke(MainForm._DelegateAddObject, new Object[] { 2, s });
        // check if thread is cancelled
        Mutexs[1].ReleaseMutex();

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


        Mutexs[1].WaitOne();
        if (NextPattern == 0)
        {
            Timer.Start();
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
        MainForm.Invoke(MainForm._DelegateAddObject, new Object[] { 1, s });
        // check if thread is cancelled

        Mutexs[1].ReleaseMutex();

    }
}
