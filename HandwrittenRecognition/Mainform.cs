using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using ArchiveSerialization;
using System.Threading;
using NeuralNetworkLibrary;

namespace HandwrittenRecogniration;

#region Public Delegates

// delegates used to call MainForm functions from worker thread
public delegate void DelegateAddObject(int i, Object s);
public delegate void DelegateThreadFinished();
#endregion
public partial class Mainform : Form
{
    //MNIST Data set
    readonly MnistDatabase _MnistTrainingDatabase;
    readonly MnistDatabase _MinstTestingDatabase;
    private MnistDatabase _Mnistdatabase;
    readonly Preferences _Preference;

    bool _bTrainingDataReady;
    bool _bTestingDataReady;
    bool _bDatabaseReady;
    bool _bTrainingThreadRuning;
    bool _bTestingThreadRuning;
    NeuralNetwork _NN;
    NeuralNetwork _TrainingNN;
    /// <summary>
    /// 
    /// </summary>
    /// 
    int _icurrentMnistPattern;
    //static uint _iBackpropThreadIdentifier;  // static member used by threads to identify themselves


    //
    //Thread

    // events used to stop worker thread
    ManualResetEvent _EventTrainingStopThread;
    ManualResetEvent _EventTrainingThreadStopped;
    ManualResetEvent _EventTestingStopThread;
    ManualResetEvent _EventTestingThreadStopped;
    //    
    Mutex _MainMutex;
    List<Thread> _trainer_threads;
    List<Thread> _testing_threads;
    // Delegate instances used to cal user interface functions 
    // from worker thread:
    public DelegateAddObject _DelegateAddObject;
    public DelegateThreadFinished _DelegateThreadFinished;

    /// <summary>
    /// My Defines
    /// </summary>
    string _mnistWeightsFile;

    public Mainform()
    {

        InitializeComponent();
        _Preference = new Preferences();
        _MnistTrainingDatabase = new MnistDatabase();
        _MinstTestingDatabase = new MnistDatabase();
        _Mnistdatabase = _MinstTestingDatabase;
        _icurrentMnistPattern = 0;
        _bTrainingDataReady = false;
        _bTestingDataReady = false;
        _bDatabaseReady = _bTestingDataReady;
        radioButtonMnistTestDatabase.Checked = true;
        radioButtonMnistTrainDatabase.Checked = false;
        pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;

        //Create Neural net work
        _NN = new NeuralNetwork();
        _TrainingNN = new NeuralNetwork();
        CreateNNNetWork(_NN);
        // initialize delegates
        _DelegateAddObject = new DelegateAddObject(this.AddObject);

        // initialize events
        _EventTrainingStopThread = new ManualResetEvent(false);
        _EventTrainingThreadStopped = new ManualResetEvent(false);
        _EventTestingStopThread = new ManualResetEvent(false);
        _EventTestingThreadStopped = new ManualResetEvent(false);
        _trainer_threads = null;
        _MainMutex = new Mutex();
        _mnistWeightsFile = "";
        _bTrainingThreadRuning = false;
        _bTestingThreadRuning = false;
    }
    private void AddObject(int iCondition, object value)
    {
        switch (iCondition)
        {
            case 1:
                labelRecognizedValue.Text = (string)value;
                break;
            case 2:
                label7.Text = (string)value;
                break;
            case 3:
                listBox1.Items.Add((string)value);
                break;
            case 4:
                label2.Text = (string)value;
                break;
            case 5:
                label3.Text = (string)value;
                break;
            case 6:
                listBox2.Items.Add((string)value);
                break;
            case 7:
                label14.Text = (string)value;
                break;
            case 8:
                listBox2.Items.Add((string)value);
                _bTestingThreadRuning = false;
                buttonMnistTest.Enabled = true;
                radioButtonTestingdatabase.Enabled = true;
                radioButtonTrainingdatabase.Enabled = true;
                break;
            case 9:
                label7.Text = (string)value;
                break;
            default:
                break;

        };
    }
    //draw training pattern to picturebox
    private void next_Click(object sender, EventArgs e)
    {
        if (_bDatabaseReady)
        {

            if (_icurrentMnistPattern < _Mnistdatabase.m_pImagePatterns.Count - 1)
            {
                _icurrentMnistPattern++;
                var bitmap = new Bitmap((int)DefaultDefinations.g_cImageSize, (int)DefaultDefinations.g_cImageSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                byte[] pArray = _Mnistdatabase.m_pImagePatterns[_icurrentMnistPattern].pPattern;
                uint label = _Mnistdatabase.m_pImagePatterns[_icurrentMnistPattern].nLabel;
                label6.Text = label.ToString();
                byte[] colors = new byte[4];
                for (int i = 0; i < 28; i++)
                {

                    for (int j = 0; j < 28; j++)
                    {

                        colors[0] = 255;
                        colors[1] = Convert.ToByte(pArray[i * 28 + j]);
                        colors[2] = Convert.ToByte(pArray[i * 28 + j]);
                        colors[3] = Convert.ToByte(pArray[i * 28 + j]);
                        int m_ARGB = BitConverter.ToInt32(colors, 0);
                        bitmap.SetPixel(j, i, Color.FromArgb((int)m_ARGB));
                    }
                }
                pictureBox2.Image = bitmap;
                ImagePatternRecognization(_icurrentMnistPattern);
                label10.Text = _icurrentMnistPattern.ToString();


            }
        }

    }
    private void ImagePatternRecognization(int index)
    {
        List<Mutex> mutexs = new List<Mutex>(2);
        for (int i = 0; i < 2; i++)
        {
            var mutex = new Mutex();
            mutexs.Add(mutex);
        }

        var NNTessing = new NNTestPatterns(_NN, _Mnistdatabase, _Preference, _bDatabaseReady, null, null, this, mutexs);
        var thread = new Thread(() => NNTessing.PatternRecognizingThread(index));
        thread.Start();
    }
    private void previous_Click(object sender, EventArgs e)
    {
        if (_bDatabaseReady)
        {
            if (_icurrentMnistPattern > 1)
            {
                _icurrentMnistPattern -= 1;
                var bitmap = new Bitmap((int)DefaultDefinations.g_cImageSize, (int)DefaultDefinations.g_cImageSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                byte[] pArray = _Mnistdatabase.m_pImagePatterns[_icurrentMnistPattern].pPattern;
                uint ulabel = _Mnistdatabase.m_pImagePatterns[_icurrentMnistPattern].nLabel;
                label6.Text = ulabel.ToString();
                byte[] colors = new byte[4];
                for (int i = 0; i < 28; i++)
                {

                    for (int j = 0; j < 28; j++)
                    {

                        colors[0] = 255;
                        colors[1] = Convert.ToByte(pArray[i * 28 + j]);
                        colors[2] = Convert.ToByte(pArray[i * 28 + j]);
                        colors[3] = Convert.ToByte(pArray[i * 28 + j]);
                        int m_ARGB = BitConverter.ToInt32(colors, 0);
                        bitmap.SetPixel(j, i, Color.FromArgb((int)m_ARGB));
                    }
                }
                pictureBox2.Image = bitmap;
                ImagePatternRecognization(_icurrentMnistPattern);
                label10.Text = _icurrentMnistPattern.ToString();
            }
        }
    }
    private void StartBackPropagationbutton_Click(object sender, EventArgs e)
    {
        if (_bTrainingDataReady)
            OnStartBackpropagation();
    }
    /// <summary>
    /// 
    /// </summary>
    void OnStartBackpropagation()
    {
        if ((_bTrainingDataReady) && (_bTrainingThreadRuning != true) && (_bTestingThreadRuning != true))
        {
            using (var dlg = new BackPropagationParametersForm())
            {
                BackPropagationParameters parameters = new BackPropagationParameters
                {
                    m_cNumThreads = (uint)_Preference.m_cNumBackpropThreads,
                    m_InitialEta = _Preference.m_dInitialEtaLearningRate,
                    m_MinimumEta = _Preference.m_dMinimumEtaLearningRate,
                    m_EtaDecay = _Preference.m_dLearningRateDecay,
                    m_AfterEvery = _Preference.m_nAfterEveryNBackprops,
                    m_StartingPattern = 0,
                    m_EstimatedCurrentMSE = 0.10,
                    m_bDistortPatterns = true
                };
                double eta = parameters.m_InitialEta;
                parameters.m_strInitialEtaMessage = String.Format("Initial Learning Rate eta (currently, eta = {0})", eta);
                int curPattern = 0;
                parameters.m_strStartingPatternNum = String.Format("Starting Pattern Number (currently at {0})", curPattern);
                dlg.SetBackProParameters(parameters);
                var m_result = dlg.ShowDialog();
                if (m_result == DialogResult.OK)
                {
                    parameters = dlg.GetBackProParameters();
                    bool bRet = StartBackpropagation(parameters.m_StartingPattern, parameters.m_cNumThreads, parameters.m_InitialEta,
                        parameters.m_MinimumEta, parameters.m_EtaDecay, parameters.m_AfterEvery, parameters.m_bDistortPatterns, parameters.m_EstimatedCurrentMSE);
                    if (bRet != false)
                    {
                        //do some thing
                        _bTrainingThreadRuning = true;
                    }
                }
            }
        }

    }
    private bool StartBackpropagation(uint iStartPattern /* =0 */, uint iNumThreads /* =2 */, double initialEta /* =0.005 */, double minimumEta /* =0.000001 */, double etaDecay /* =0.990 */,
                                 uint nAfterEvery  /* =1000 */, bool bDistortPatterns /* =TRUE */, double estimatedCurrentMSE /* =1.0 */)
    {

        if (iNumThreads < 1)
            iNumThreads = 1;
        if (iNumThreads > 10)  // 10 is arbitrary upper limit
            iNumThreads = 10;
        //initialize BackPropagation before process
        _NN.m_etaLearningRate = initialEta;
        _NN.m_etaLearningRatePrevious = initialEta;

        //run thread here
        _EventTrainingStopThread.Reset();
        _EventTrainingThreadStopped.Reset();
        _trainer_threads = new List<Thread>(2);
        _MnistTrainingDatabase.RandomizePatternSequence();
        //cleare mutex before run threads.
        var mutexs = new List<Mutex>(2);
        for (int i = 0; i < 4; i++)
        {
            Mutex mutex = new Mutex();
            mutexs.Add(mutex);
        }

        //create neural network
        try
        {
            CreateNNNetWork(_TrainingNN);
            //initialize weight parameters to the network
            if (_mnistWeightsFile != "")
            {
                _MainMutex.WaitOne();
                var fsIn = new FileStream(_mnistWeightsFile, FileMode.Open);
                var arIn = new Archive(fsIn, ArchiveOp.Load);
                _TrainingNN.Serialize(arIn);
                fsIn.Close();
                _MainMutex.ReleaseMutex();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
            return false;
        }
        //
        var ntraining = new NNTrainPatterns(_TrainingNN, _MnistTrainingDatabase, _Preference, _bTrainingDataReady, _EventTrainingStopThread,
            _EventTrainingThreadStopped, this, mutexs)
        {
            m_dMinimumEta = minimumEta,
            m_dEtaDecay = etaDecay,
            m_nAfterEveryNBackprops = nAfterEvery,
            m_bDistortPatterns = bDistortPatterns,
            m_dEstimatedCurrentMSE = estimatedCurrentMSE
            /* estimated number that will define whether a forward calculation's error is significant enough to warrant backpropagation*/
        };

        for (int i = 0; i < iNumThreads; i++)
        {
            var trainer_thread = new Thread(ntraining.BackpropagationThread);
            trainer_thread.Name = String.Format("Thread{0}", i + 1);
            _trainer_threads.Add(trainer_thread);
            trainer_thread.Start();

        }

        return true;

    }
    /////////////////////////
    private bool CreateNNNetWork(NeuralNetwork network)
    {

        NNLayer pLayer;

        int ii, jj, kk;
        int icNeurons = 0;
        int icWeights = 0;
        double initWeight;
        String sLabel;
        var m_rdm = new Random();
        // layer zero, the input layer.
        // Create neurons: exactly the same number of neurons as the input
        // vector of 29x29=841 pixels, and no weights/connections

        pLayer = new NNLayer("Layer00", null);
        network.m_Layers.Add(pLayer);

        for (ii = 0; ii < 841; ii++)
        {
            sLabel = String.Format("Layer00_Neuro{0}_Num{1}", ii, icNeurons);
            pLayer.m_Neurons.Add(new NNNeuron(sLabel));
            icNeurons++;
        }

        //double UNIFORM_PLUS_MINUS_ONE= (double)(2.0 * m_rdm.Next())/Constants.RAND_MAX - 1.0 ;

        // layer one:
        // This layer is a convolutional layer that has 6 feature maps.  Each feature 
        // map is 13x13, and each unit in the feature maps is a 5x5 convolutional kernel
        // of the input layer.
        // So, there are 13x13x6 = 1014 neurons, (5x5+1)x6 = 156 weights

        pLayer = new NNLayer("Layer01", pLayer);
        network.m_Layers.Add(pLayer);

        for (ii = 0; ii < 1014; ii++)
        {
            sLabel = String.Format("Layer01_Neuron{0}_Num{1}", ii, icNeurons);
            pLayer.m_Neurons.Add(new NNNeuron(sLabel));
            icNeurons++;
        }

        for (ii = 0; ii < 156; ii++)
        {

            sLabel = String.Format("Layer01_Weigh{0}_Num{1}", ii, icWeights);
            initWeight = 0.05 * (2.0 * m_rdm.NextDouble() - 1.0);
            pLayer.m_Weights.Add(new NNWeight(sLabel, initWeight));
        }

        // interconnections with previous layer: this is difficult
        // The previous layer is a top-down bitmap image that has been padded to size 29x29
        // Each neuron in this layer is connected to a 5x5 kernel in its feature map, which 
        // is also a top-down bitmap of size 13x13.  We move the kernel by TWO pixels, i.e., we
        // skip every other pixel in the input image

        int[] kernelTemplate = new int[25] {
                0,  1,  2,  3,  4,
                29, 30, 31, 32, 33,
                58, 59, 60, 61, 62,
                87, 88, 89, 90, 91,
                116,117,118,119,120 };

        int iNumWeight;

        int fm;

        for (fm = 0; fm < 6; fm++)
        {
            for (ii = 0; ii < 13; ii++)
            {
                for (jj = 0; jj < 13; jj++)
                {
                    iNumWeight = fm * 26;  // 26 is the number of weights per feature map
                    NNNeuron n = pLayer.m_Neurons[jj + ii * 13 + fm * 169];

                    n.AddConnection((uint)DefaultDefinations.ULONG_MAX, (uint)iNumWeight++);  // bias weight

                    for (kk = 0; kk < 25; kk++)
                    {
                        // note: max val of index == 840, corresponding to 841 neurons in prev layer
                        n.AddConnection((uint)(2 * jj + 58 * ii + kernelTemplate[kk]), (uint)iNumWeight++);
                    }
                }
            }
        }


        // layer two:
        // This layer is a convolutional layer that has 50 feature maps.  Each feature 
        // map is 5x5, and each unit in the feature maps is a 5x5 convolutional kernel
        // of corresponding areas of all 6 of the previous layers, each of which is a 13x13 feature map
        // So, there are 5x5x50 = 1250 neurons, (5x5+1)x6x50 = 7800 weights

        pLayer = new NNLayer("Layer02", pLayer);
        network.m_Layers.Add(pLayer);

        for (ii = 0; ii < 1250; ii++)
        {
            sLabel = String.Format("Layer02_Neuron{0}_Num{1}", ii, icNeurons);
            pLayer.m_Neurons.Add(new NNNeuron(sLabel));
            icNeurons++;
        }

        for (ii = 0; ii < 7800; ii++)
        {

            sLabel = String.Format("Layer02_Weight{0}_Num{1}", ii, icWeights);
            initWeight = 0.05 * (2.0 * m_rdm.NextDouble() - 1.0);
            pLayer.m_Weights.Add(new NNWeight(sLabel, initWeight));
        }

        // Interconnections with previous layer: this is difficult
        // Each feature map in the previous layer is a top-down bitmap image whose size
        // is 13x13, and there are 6 such feature maps.  Each neuron in one 5x5 feature map of this 
        // layer is connected to a 5x5 kernel positioned correspondingly in all 6 parent
        // feature maps, and there are individual weights for the six different 5x5 kernels.  As
        // before, we move the kernel by TWO pixels, i.e., we
        // skip every other pixel in the input image.  The result is 50 different 5x5 top-down bitmap
        // feature maps

        int[] kernelTemplate2 = new int[25]{
                0,  1,  2,  3,  4,
                13, 14, 15, 16, 17,
                26, 27, 28, 29, 30,
                39, 40, 41, 42, 43,
                52, 53, 54, 55, 56   };


        for (fm = 0; fm < 50; fm++)
        {
            for (ii = 0; ii < 5; ii++)
            {
                for (jj = 0; jj < 5; jj++)
                {
                    iNumWeight = fm * 156;  // 26 is the number of weights per feature map
                    NNNeuron n = pLayer.m_Neurons[jj + ii * 5 + fm * 25];

                    n.AddConnection((uint)DefaultDefinations.ULONG_MAX, (uint)iNumWeight++);  // bias weight

                    for (kk = 0; kk < 25; kk++)
                    {
                        // note: max val of index == 1013, corresponding to 1014 neurons in prev layer
                        n.AddConnection((uint)(2 * jj + 26 * ii + kernelTemplate2[kk]), (uint)iNumWeight++);
                        n.AddConnection((uint)(169 + 2 * jj + 26 * ii + kernelTemplate2[kk]), (uint)iNumWeight++);
                        n.AddConnection((uint)(338 + 2 * jj + 26 * ii + kernelTemplate2[kk]), (uint)iNumWeight++);
                        n.AddConnection((uint)(507 + 2 * jj + 26 * ii + kernelTemplate2[kk]), (uint)iNumWeight++);
                        n.AddConnection((uint)(676 + 2 * jj + 26 * ii + kernelTemplate2[kk]), (uint)iNumWeight++);
                        n.AddConnection((uint)(845 + 2 * jj + 26 * ii + kernelTemplate2[kk]), (uint)iNumWeight++);
                    }
                }
            }
        }


        // layer three:
        // This layer is a fully-connected layer with 100 units.  Since it is fully-connected,
        // each of the 100 neurons in the layer is connected to all 1250 neurons in
        // the previous layer.
        // So, there are 100 neurons and 100*(1250+1)=125100 weights

        pLayer = new NNLayer("Layer03", pLayer);
        network.m_Layers.Add(pLayer);

        for (ii = 0; ii < 100; ii++)
        {
            sLabel = String.Format("Layer03_Neuron{0}_Num{1}", ii, icNeurons);
            pLayer.m_Neurons.Add(new NNNeuron(sLabel));
            icNeurons++;
        }

        for (ii = 0; ii < 125100; ii++)
        {

            sLabel = String.Format("Layer03_Weight{0}_Num{1}", ii, icWeights);
            initWeight = 0.05 * (2.0 * m_rdm.NextDouble() - 1.0);
            pLayer.m_Weights.Add(new NNWeight(sLabel, initWeight));
        }

        // Interconnections with previous layer: fully-connected

        iNumWeight = 0;  // weights are not shared in this layer

        for (fm = 0; fm < 100; fm++)
        {
            NNNeuron n = pLayer.m_Neurons[fm];
            n.AddConnection((uint)DefaultDefinations.ULONG_MAX, (uint)iNumWeight++);  // bias weight

            for (ii = 0; ii < 1250; ii++)
            {
                n.AddConnection((uint)ii, (uint)iNumWeight++);
            }
        }



        // layer four, the final (output) layer:
        // This layer is a fully-connected layer with 10 units.  Since it is fully-connected,
        // each of the 10 neurons in the layer is connected to all 100 neurons in
        // the previous layer.
        // So, there are 10 neurons and 10*(100+1)=1010 weights

        pLayer = new NNLayer("Layer04", pLayer);
        network.m_Layers.Add(pLayer);

        for (ii = 0; ii < 10; ii++)
        {
            sLabel = String.Format("Layer04_Neuron{0}_Num{1}", ii, icNeurons);
            pLayer.m_Neurons.Add(new NNNeuron(sLabel));
            icNeurons++;
        }

        for (ii = 0; ii < 1010; ii++)
        {

            sLabel = String.Format("Layer04_Weight{0}_Num{1}", ii, icWeights);
            initWeight = 0.05 * (2.0 * m_rdm.NextDouble() - 1.0);
            pLayer.m_Weights.Add(new NNWeight(sLabel, initWeight));
        }

        // Interconnections with previous layer: fully-connected

        iNumWeight = 0;  // weights are not shared in this layer

        for (fm = 0; fm < 10; fm++)
        {
            var n = pLayer.m_Neurons[fm];
            n.AddConnection((uint)DefaultDefinations.ULONG_MAX, (uint)iNumWeight++);  // bias weight

            for (ii = 0; ii < 100; ii++)
            {
                n.AddConnection((uint)ii, (uint)iNumWeight++);
            }
        }

        return true;
    }

    private void Mainform_Load(object sender, EventArgs e)
    {

    }
    //stop threads.
    private void StopBackPropagationbutton_Click(object sender, EventArgs e)
    {
        if (_bTrainingThreadRuning)
        {
            if (StopTheads(_trainer_threads, _EventTrainingStopThread, _EventTrainingThreadStopped))
            {
                BackPropagationThreadsFinished();		// set initial state of buttons
            }
        }
    }

    void BackPropagationThreadsFinished()
    {
        if (_bTrainingThreadRuning)
        {
            var msResult = MessageBox.Show("Do you want to save Neural Network data ?", "Save Neural Network Data", MessageBoxButtons.OKCancel);
            if (msResult == DialogResult.OK)
            {
                using (var saveFileDialog1 = new System.Windows.Forms.SaveFileDialog { Filter = "Mnist Neural network file (*.nnt)|*.nnt", Title = "Save Neural network File" })
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {

                        var fsIn = saveFileDialog1.OpenFile();
                        var arIn = new Archive(fsIn, ArchiveOp.Store);
                        _TrainingNN.Serialize(arIn);
                        fsIn.Close();
                    }
                }
            }
            _bTrainingThreadRuning = false;
        }
        return;
    }
    // Load Image from file
    private Bitmap CreateNonIndexedImage(Bitmap src)
    {
        Bitmap newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using (var gfx = Graphics.FromImage(newBmp))
        {
            gfx.DrawImage(src, 0, 0);
        }
        return newBmp;
    }

    private void networkParametersToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using (var openFileDialog1 = new System.Windows.Forms.OpenFileDialog { Filter = "Mnist Neural network file (*.nnt)|*.nnt", Title = "Open Neural network File" })
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _MainMutex.WaitOne();
                _mnistWeightsFile = openFileDialog1.FileName;
                var fsIn = openFileDialog1.OpenFile();
                var arIn = new Archive(fsIn, ArchiveOp.Load);
                _NN.Serialize(arIn);
                fsIn.Close();
                _MainMutex.ReleaseMutex();
            }
        }
    }

    private void mNISTDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _bTrainingDataReady = _MnistTrainingDatabase.LoadMinstFiles();
        if (_bTrainingDataReady)
        {

            //update Preferences parametters
            if (_MnistTrainingDatabase.m_pImagePatterns.Count != _Preference.m_nItemsTrainingImages)
            {
                _Preference.m_nItemsTrainingImages = (uint)_MnistTrainingDatabase.m_pImagePatterns.Count;
                _Preference.m_nItemsTrainingLabels = (uint)_MnistTrainingDatabase.m_pImagePatterns.Count;
            }
            radioButtonMnistTrainDatabase.Enabled = true;
            radioButtonTrainingdatabase.Enabled = true;
            buttonMnistNext.Enabled = true;
            buttonMnistPrevious.Enabled = true;
            _bDatabaseReady = _bTrainingDataReady;
            _Mnistdatabase = _MnistTrainingDatabase;
        }
        else
        {
            radioButtonMnistTrainDatabase.Enabled = false;
            return;
        }
        _bTestingDataReady = _MinstTestingDatabase.LoadMinstFiles();
        if (_bTestingDataReady)
        {
            //update Preferences parametters
            if (_MinstTestingDatabase.m_pImagePatterns.Count != _Preference.m_nItemsTestingImages)
            {
                _Preference.m_nItemsTestingImages = (uint)_MinstTestingDatabase.m_pImagePatterns.Count;
                _Preference.m_nItemsTestingLabels = (uint)_MinstTestingDatabase.m_pImagePatterns.Count;
            }
            radioButtonMnistTestDatabase.Enabled = true;
            radioButtonMnistTestDatabase.Checked = true;
            radioButtonTestingdatabase.Enabled = true;
            radioButtonTestingdatabase.Checked = true;
            buttonMnistNext.Enabled = true;
            buttonMnistPrevious.Enabled = true;
            _bDatabaseReady = _bTestingDataReady;
            _Mnistdatabase = _MinstTestingDatabase;
        }
        else
        {
            radioButtonMnistTestDatabase.Enabled = false;
            return;
        }
    }

    private void buttonMnistTest_Click(object sender, EventArgs e)
    {
        if ((_bTestingThreadRuning == false) && (_bTrainingThreadRuning == false))
        {
            var mutexs = new List<Mutex>(2);
            int theadsNum = (int)numericUpDownThreads.Value;
            var nnTesting = (NNTestPatterns)null; ;
            var nnNetwork = new NeuralNetwork();
            bool bDatabaseforTest = false;
            //create neural network
            try
            {
                CreateNNNetWork(nnNetwork);
                //initialize weight parameters to the network
                if (_mnistWeightsFile != "")
                {
                    _MainMutex.WaitOne();
                    var fsIn = new FileStream(_mnistWeightsFile, FileMode.Open);
                    var arIn = new Archive(fsIn, ArchiveOp.Load);
                    nnNetwork.Serialize(arIn);
                    fsIn.Close();
                    _MainMutex.ReleaseMutex();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            //
            if (radioButtonTestingdatabase.Checked)
            {
                if (_bTestingDataReady)
                {
                    nnTesting = new NNTestPatterns(nnNetwork, _MinstTestingDatabase, _Preference, _bTestingDataReady, _EventTestingStopThread, _EventTestingThreadStopped, this, mutexs);
                    bDatabaseforTest = _bTestingDataReady;
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (_bTrainingDataReady)
                {
                    nnTesting = new NNTestPatterns(nnNetwork, _MnistTrainingDatabase, _Preference, _bTrainingDataReady, _EventTestingStopThread, _EventTestingThreadStopped, this, mutexs);
                    bDatabaseforTest = _bTrainingDataReady;
                }
                else
                {
                    return;
                }
            }
            if (bDatabaseforTest)
            {
                //
                listBox2.Items.Clear();
                for (int i = 0; i < 2; i++)
                {
                    var mutex = new Mutex();
                    mutexs.Add(mutex);
                }
                _EventTestingStopThread.Reset();
                _EventTestingThreadStopped.Reset();
                _testing_threads = new List<Thread>(2);

                try
                {
                    for (int i = 0; i < theadsNum; i++)
                    {
                        var thread = new Thread(delegate ()
                                                    {
                                                        nnTesting.PatternsTestingThread((int)numericUpDownNumberofTestPattern.Value);
                                                    });
                        _testing_threads.Add(thread);
                        thread.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
                _bTestingThreadRuning = true;
                radioButtonTestingdatabase.Enabled = false;
                radioButtonTrainingdatabase.Enabled = false;
                buttonMnistTest.Enabled = false;

            }
        }

    }
    private bool StopTheads(List<Thread> threads, ManualResetEvent eventStopThread, ManualResetEvent eventThreadStopped)
    {
        try
        {
            if (threads != null)
            {

                if ((threads.Count > 0) && (threads[0].IsAlive)) // thread is active
                {
                    // set event "Stop"
                    eventStopThread.Set();
                    foreach (var thread in threads)
                    {
                        // wait when thread  will stop or finish

                        while (thread.IsAlive || thread.IsAlive)
                        {
                            // We cannot use here infinite wait because our thread
                            // makes syncronous calls to main form, this will cause deadlock.
                            // Instead of this we wait for event some appropriate time
                            // (and by the way give time to worker thread) and
                            // process events. These events may contain Invoke calls.
                            if (WaitHandle.WaitAll(
                                (new ManualResetEvent[] { eventThreadStopped }),
                                100,
                                true))
                            {
                                break;
                            }

                            Application.DoEvents();
                        }
                    }

                }

            }
            threads.Clear();
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
            return false;

        }
    }
    private void buttonStopMnistTest_Click(object sender, EventArgs e)
    {
        if (_bTestingThreadRuning)
        {
            if (StopTheads(_testing_threads, _EventTestingStopThread, _EventTestingThreadStopped))
            {
                _bTestingThreadRuning = false;
                radioButtonTestingdatabase.Enabled = true;
                radioButtonTrainingdatabase.Enabled = true;
                buttonMnistTest.Enabled = true;

            }
        }
        //grayscale bitmap
    }

    private void radioButtonTestingdatabase_CheckedChanged(object sender, EventArgs e)
    {
        if (radioButtonTestingdatabase.Checked)
        {
            numericUpDownNumberofTestPattern.Maximum = 9999;


        }
        else
        {
            numericUpDownNumberofTestPattern.Maximum = 59999;

        }
    }

    private void radioButton2_CheckedChanged(object sender, EventArgs e)
    {
        if (radioButtonMnistTestDatabase.Checked)
        {
            _Mnistdatabase = _MinstTestingDatabase;
            _bDatabaseReady = _bTestingDataReady;
            _icurrentMnistPattern = 0;
        }
        else
        {
            _Mnistdatabase = _MinstTestingDatabase;
            _bDatabaseReady = _bTrainingDataReady;
            _icurrentMnistPattern = 0;
        }
    }

    private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_bTestingThreadRuning || _bTrainingThreadRuning)
        {
            var result = MessageBox.Show("Sorry, some threads are running. Please stop them before  you can close the program", "", MessageBoxButtons.OK);
            e.Cancel = true;
        }

    }

    private void viewHelpToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Handwritten character recognition program Vesion 0.1,\nCopyright (C) 2010-2011, \nPham Viet Dung, Vietnam Maritime University" +
            "\nEmail:vietdungiitb@vimaru.edu.vn",
            "About Handwritten character recognition program", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }


}


