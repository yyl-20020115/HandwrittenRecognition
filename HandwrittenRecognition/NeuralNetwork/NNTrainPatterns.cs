using System;
using System.Collections.Generic;
using System.Threading;
using NeuralNetworkLibrary;
namespace HandwrittenRecogniration;

public class NNTrainPatterns : NNForwardPropagation
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    #region Parametters

    //backpropagation and training-related members

    public uint m_nAfterEveryNBackprops;
    public double m_dEtaDecay;
    public double m_dMinimumEta;
    public double m_dEstimatedCurrentMSE;  // this number will be changed by one thread and used by others
    public uint m_cMisrecognitions;
    public bool m_bNeedHessian;
    public int m_cBackprops;

    /// <summary>
    /// 
    /// </summary>

    uint _iEpochsCompleted;
    int _iNextPattern;
    double _dMSE = 0;
    int _nn;
    double _dMSE200 = 0;
    private MnistDatabase _MnistDataSet;
    Mainform _form;
    /// <summary>
    /// 
    /// </summary>

    #endregion

    #region Main function
    public NNTrainPatterns(NeuralNetwork neuronNet, MnistDatabase trainingSet, Preferences preferences, bool trainingDataReady,
                        ManualResetEvent eventStop,
                        ManualResetEvent eventStopped,
                        HandwrittenRecogniration.Mainform form, List<Mutex> mutexs)
    {
        m_currentPatternIndex = 0;
        _bDataReady = trainingDataReady;
        _NN = neuronNet;
        _MnistDataSet = trainingSet;
        m_Preferences = preferences;
        m_nImages = (uint)_MnistDataSet.m_pImagePatterns.Count;
        _form = form;
        m_EventStop = eventStop;
        m_EventStopped = eventStopped;
        m_Mutexs = mutexs;
        m_cMisrecognitions = 0;
        _iNextPattern = 0;
        m_bNeedHessian = true;
        m_cBackprops = 0;
        _dMSE = 0;
        _nn = 0;
        _dMSE200 = 0;
        m_HiPerfTime = new HiPerfTimer();
        GetGaussianKernel(m_Preferences.m_dElasticSigma);
    }
    #endregion
    public void Initialize(uint cBackprops, uint nAfterEveryNBackprops,
            double dEtaDecay, double dMinimumEta, double dEstimatedCurrentMSE,  // this number will be changed by one thread and used by others
            bool bDistortTrainingPatterns)
    {

        m_nAfterEveryNBackprops = nAfterEveryNBackprops;
        m_dEtaDecay = dEtaDecay;
        m_dMinimumEta = dMinimumEta;
        m_dEstimatedCurrentMSE = dEstimatedCurrentMSE;  // this number will be changed by one thread and used by others
        m_bDistortPatterns = bDistortTrainingPatterns;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputVector"></param>
    void CalculateHessian()
    {
        // controls the Neural network's calculation if the diagonal Hessian for the Neural net
        // This will be called from a thread, so although the calculation is lengthy, it should not interfere
        // with the UI

        // we need the neural net exclusively during this calculation, so grab it now

        double[] inputVector = new double[841];  // note: 29x29, not 28x28

        double[] targetOutputVector = new double[10];
        double[] actualOutputVector = new double[10];
        m_Mutexs[1].WaitOne();

        for (int i = 0; i < 841; i++)
        {
            inputVector[i] = 0.0;
        }
        for (int j = 0; j < 10; j++)
        {
            targetOutputVector[j] = 0.0;
            actualOutputVector[j] = 0.0;
        }

        byte label = 0;
        int ii, jj;
        uint kk;

        // calculate the diagonal Hessian using 500 random patterns, per Yann LeCun 1998 "Gradient-Based Learning
        // Applied To Document Recognition"
        string s = "Commencing Caculation of Hessian...";
        // Make synchronous call to main form.
        // MainForm.AddString function runs in main thread.
        // To make asynchronous call use BeginInvoke
        if (_form != null)
            _form.Invoke(_form._DelegateAddObject, new Object[] { 3, s });

        // some of this code is similar to the BackpropagationThread() code

        _NN.EraseHessianInformation();

        uint numPatternsSampled = m_Preferences.m_nNumHessianPatterns;

        for (kk = 0; kk < numPatternsSampled; ++kk)
        {
            int iRandomPatternNum;

            iRandomPatternNum = _MnistDataSet.GetRandomPatternNumber();
            label = _MnistDataSet.m_pImagePatterns[iRandomPatternNum].nLabel;

            if (label < 0) label = 0;
            if (label > 9) label = 9;
            byte[] grayLevels = _MnistDataSet.m_pImagePatterns[iRandomPatternNum].pPattern;

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


            // apply distortion map to inputVector.  It's not certain that this is needed or helpful.
            // The second derivatives do NOT rely on the output of the neural net (i.e., because the 
            // second derivative of the MSE function is exactly 1 (one), regardless of the actual output
            // of the net).  However, since the backpropagated second derivatives rely on the outputs of
            // each neuron, distortion of the pattern might reveal previously-unseen information about the
            // nature of the Hessian.  But I am reluctant to give the full distortion, so I set the
            // severityFactor to only 2/3 approx

            GenerateDistortionMap(0.65);
            ApplyDistortionMap(inputVector);
            // forward calculate the neural network

            _NN.Calculate(inputVector, 841, actualOutputVector, 10, null);


            // backpropagate the second derivatives

            _NN.BackpropagateSecondDervatives(actualOutputVector, targetOutputVector, 10);

            //
            // check if thread is cancelled
            if (m_EventStop.WaitOne(0, true))
            {
                // clean-up operations may be placed here
                // ...
                string ss = "BackPropagation stoped";
                // Make synchronous call to main form.
                // MainForm.AddString function runs in main thread.
                // To make asynchronous call use BeginInvoke
                if (_form != null)
                    _form.Invoke(_form._DelegateAddObject, new Object[] { 3, ss });
                // inform main thread that this thread stopped
                m_EventStopped.Set();

                return;
            }
        }

        _NN.DivideHessianInformationBy((double)numPatternsSampled);
        s = " Caculation of Hessian...completed";
        if (_form != null)
            _form.Invoke(_form._DelegateAddObject, new Object[] { 3, s });
        m_Mutexs[1].ReleaseMutex();
    }

    /////////////////////////
    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputVector"></param>
    /// <param name="count"></param>
    /// <param name="outputVector"></param>
    /// <param name="oCount"></param>
    /// <param name="pNeuronOutputs"></param>
    /// <param name="bDistort"></param>

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputVector"></param>
    /// <param name="iCount"></param>
    /// <param name="targetOutputVector"></param>
    /// <param name="actualOutputVector"></param>
    /// <param name="oCount"></param>
    /// <param name="pMemorizedNeuronOutputs"></param>
    /// <param name="bDistort"></param>
    void BackpropagateNeuralNet(double[] inputVector, int iCount, double[] targetOutputVector,
                                   double[] actualOutputVector, int oCount,
                                   NNNeuronOutputsList pMemorizedNeuronOutputs,
                                   bool bDistort)
    {
        // function to backpropagate through the neural net. 

        //ASSERT( (inputVector != NULL) && (targetOutputVector != NULL) && (actualOutputVector != NULL) );

        ///////////////////////////////////////////////////////////////////////
        //
        // CODE REVIEW NEEDED:
        //
        // It does not seem worthwhile to backpropagate an error that's very small.  "Small" needs to be defined
        // and for now, "small" is set to a fixed size of pattern error ErrP <= 0.10 * MSE, then there will
        // not be a backpropagation of the error.  The current MSE is updated from the neural net dialog CDlgNeuralNet

        bool bWorthwhileToBackpropagate;  /////// part of code review


        // local scope for capture of the neural net, only during the forward calculation step,
        // i.e., we release neural net for other threads after the forward calculation, and after we
        // have stored the outputs of each neuron, which are needed for the backpropagation step



        // determine if it's time to adjust the learning rate
        m_Mutexs[2].WaitOne();
        if (m_nAfterEveryNBackprops == 0) m_nAfterEveryNBackprops = 1;
        if (((m_cBackprops % m_nAfterEveryNBackprops) == 0) && (m_cBackprops != 0))
        {
            double eta = _NN.m_etaLearningRate;
            eta *= m_dEtaDecay;
            if (eta < m_dMinimumEta)
                eta = m_dMinimumEta;
            _NN.m_etaLearningRatePrevious = _NN.m_etaLearningRate;
            _NN.m_etaLearningRate = eta;
        }


        // determine if it's time to adjust the Hessian (currently once per epoch)

        if ((m_bNeedHessian != false) || ((m_cBackprops % m_Preferences.m_nItemsTrainingImages) == 0))
        {
            // adjust the Hessian.  This is a lengthy operation, since it must process approx 500 labels
            m_bNeedHessian = false;
            CalculateHessian();



        }
        // increment counter for tracking number of backprops

        m_cBackprops++;

        // determine if it's time to randomize the sequence of training patterns (currently once per epoch)

        if ((m_cBackprops % m_Preferences.m_nItemsTrainingImages) == 0)
        {
            _MnistDataSet.RandomizePatternSequence();
        }

        m_Mutexs[2].ReleaseMutex();



        // forward calculate through the neural net

        CalculateNeuralNet(inputVector, iCount, actualOutputVector, oCount, pMemorizedNeuronOutputs, bDistort);

        m_Mutexs[2].WaitOne();
        // calculate error in the output of the neural net
        // note that this code duplicates that found in many other places, and it's probably sensible to 
        // define a (global/static ??) function for it

        double dMSE = 0.0;
        for (int ii = 0; ii < 10; ++ii)
        {
            dMSE += (actualOutputVector[ii] - targetOutputVector[ii]) * (actualOutputVector[ii] - targetOutputVector[ii]);
        }
        dMSE /= 2.0;

        if (dMSE <= (0.10 * m_dEstimatedCurrentMSE))
        {
            bWorthwhileToBackpropagate = false;
        }
        else
        {
            bWorthwhileToBackpropagate = true;
        }

        if ((bWorthwhileToBackpropagate != false) && (pMemorizedNeuronOutputs == null))
        {
            // the caller has not provided a place to store neuron outputs, so we need to
            // backpropagate now, while the neural net is still captured.  Otherwise, another thread
            // might come along and call CalculateNeuralNet(), which would entirely change the neuron
            // outputs and thereby inject errors into backpropagation 

            _NN.Backpropagate(actualOutputVector, targetOutputVector, oCount, null);
            // we're done, so return

            return;
        }




        // if we have reached here, then the mutex for the neural net has been released for other 
        // threads.  The caller must have provided a place to store neuron outputs, which we can 
        // use to backpropagate, even if other threads call CalculateNeuralNet() and change the outputs
        // of the neurons

        if ((bWorthwhileToBackpropagate != false))
        {
            _NN.Backpropagate(actualOutputVector, targetOutputVector, oCount, pMemorizedNeuronOutputs);


        }
        m_Mutexs[2].ReleaseMutex();
    }
    /// <summary>
    /// StopBackpropagation function
    /// </summary>

    /// <summary>
    /// 
    /// </summary>
    public void BackpropagationThread()
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
        double dMSE;
        byte label = 0;
        int ii, jj;
        NNNeuronOutputsList _memorizedNeuronOutputs = new NNNeuronOutputsList();
        //prepare for training

        while (true)
        {
            m_Mutexs[3].WaitOne();
            if (_iNextPattern == 0)
            {
                m_HiPerfTime.Start();
                _MnistDataSet.RandomizePatternSequence();
            }
            byte[] grayLevels = new byte[m_Preferences.m_nRowsImages * m_Preferences.m_nColsImages];
            int ipattern = _MnistDataSet.GetNextPatternNumber(_MnistDataSet.m_bFromRandomizedPatternSequence);
            _MnistDataSet.m_pImagePatterns[ipattern].pPattern.CopyTo(grayLevels, 0);
            label = _MnistDataSet.m_pImagePatterns[ipattern].nLabel;
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

            // now backpropagate
            m_Mutexs[3].ReleaseMutex();

            BackpropagateNeuralNet(inputVector, 841, targetOutputVector, actualOutputVector, 10,
                _memorizedNeuronOutputs, m_bDistortPatterns);

            m_Mutexs[3].WaitOne();
            // calculate error for this pattern and post it to the hwnd so it can calculate a running 
            // estimate of MSE

            dMSE = 0.0;
            for (ii = 0; ii < 10; ++ii)
            {
                dMSE += (actualOutputVector[ii] - targetOutputVector[ii]) * (actualOutputVector[ii] - targetOutputVector[ii]);
            }
            dMSE /= 2.0;
            _dMSE += dMSE;
            _dMSE200 += dMSE;
            // determine the neural network's answer, and compare it to the actual answer.
            // Post a message if the answer was incorrect, so the dialog can display mis-recognition
            // statistics
            _nn++;
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

            if (iBestIndex != label)
            {
                m_cMisrecognitions++;
            }
            //
            // make step
            string s = "";
            if (_nn >= 200)
            {
                _dMSE200 /= 200;
                s = "MSE:" + _dMSE200.ToString();
                _form.Invoke(_form._DelegateAddObject, new Object[] { 4, s });
                _dMSE200 = 0;
                _nn = 0;
            }

            s = String.Format("{0} Miss Number:{1}", Convert.ToString(_iNextPattern), m_cMisrecognitions); ;
            // Make synchronous call to main form.
            // MainForm.AddString function runs in main thread.
            // To make asynchronous call use BeginInvoke
            if (_form != null)
                _form.Invoke(_form._DelegateAddObject, new Object[] { 5, s });

            if (_iNextPattern >= _MnistDataSet.m_pImagePatterns.Count - 1)
            {
                m_HiPerfTime.Stop();
                _dMSE /= _iNextPattern;
                s = String.Format("Completed Epochs:{0}, MisPatterns:{1}, MSE:{2}, Ex. time: {3}, eta:{4} ",
                    Convert.ToString(_iEpochsCompleted + 1), Convert.ToString(m_cMisrecognitions), _dMSE.ToString(), m_HiPerfTime.Duration, _NN.m_etaLearningRate.ToString());
                // Make synchronous call to main form.
                // MainForm.AddString function runs in main thread.
                // To make asynchronous call use BeginInvoke
                if (_form != null)
                    _form.Invoke(_form._DelegateAddObject, new Object[] { 3, s });
                m_cMisrecognitions = 0;
                _iEpochsCompleted++;
                _iNextPattern = 0;
                _dMSE = 0;
            }
            //
            // check if thread is cancelled
            if (m_EventStop.WaitOne(0, true))
            {
                // clean-up operations may be placed here

                // ...
                s = String.Format("BackPropagation thread: {0} stoped", Thread.CurrentThread.Name);
                // Make synchronous call to main form.
                // MainForm.AddString function runs in main thread.
                // To make asynchronous call use BeginInvoke
                if (_form != null)
                {
                    _form.Invoke(_form._DelegateAddObject, new Object[] { 3, s });
                }
                // inform main thread that this thread stopped
                m_EventStopped.Set();
                m_Mutexs[3].ReleaseMutex();
                return;
            }
            m_Mutexs[3].ReleaseMutex();
        }  // end of main "while not abort flag" loop

    }

}
