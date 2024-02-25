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
public delegate void DelegateAddText(int i, string s);
public delegate void DelegateThreadFinished();
#endregion
public partial class MainForm : Form
{
    //MNIST Data set
    private readonly MnistDatabase MnistTrainingDatabase;
    private readonly MnistDatabase MnistTestingDatabase;
    private MnistDatabase Mnistdatabase;
    private readonly Preferences Preference;

    private bool IsTrainingDataReady;
    private bool IsTestingDataReady;
    private bool IsDatabaseReady;
    private bool IsTrainingThreadRuning;
    private bool IsTestingThreadRuning;
    private readonly NeuralNetwork NN;
    private readonly NeuralNetwork TrainingNN;
    /// <summary>
    /// 
    /// </summary>
    /// 
    private int CurrentMnistPattern;
    //static uint _iBackpropThreadIdentifier;  // static member used by threads to identify themselves
    //
    //Thread

    // events used to stop worker thread
    private readonly ManualResetEvent EventTrainingStopThread;
    private readonly ManualResetEvent EventTrainingThreadStopped;
    private readonly ManualResetEvent EventTestingStopThread;
    private readonly ManualResetEvent EventTestingThreadStopped;
    //    
    private readonly Mutex MainMutex;
    private List<Thread> TrainerThreads;
    private List<Thread> TestingThreads;
    // Delegate instances used to cal user interface functions 
    // from worker thread:
    public DelegateAddText DelegateAddText;
    public DelegateThreadFinished DelegateThreadFinished;

    /// <summary>
    /// My Defines
    /// </summary>
    private string MnistWeightsFile;

    public MainForm()
    {
        InitializeComponent();
        Preference = new ();
        MnistTrainingDatabase = new ();
        MnistTestingDatabase = new ();
        Mnistdatabase = MnistTestingDatabase;
        CurrentMnistPattern = 0;
        IsTrainingDataReady = false;
        IsTestingDataReady = false;
        IsDatabaseReady = IsTestingDataReady;
        radioButtonMnistTestDatabase.Checked = true;
        radioButtonMnistTrainDatabase.Checked = false;
        PictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

        //Create Neural net work
        NN = new ();
        TrainingNN = new ();
        CreateNNNetWork(NN);
        // initialize delegates
        DelegateAddText = new (this.AddText);

        // initialize events
        EventTrainingStopThread = new (false);
        EventTrainingThreadStopped = new (false);
        EventTestingStopThread = new (false);
        EventTestingThreadStopped = new (false);
        TrainerThreads = null;
        MainMutex = new ();
        MnistWeightsFile = "";
        IsTrainingThreadRuning = false;
        IsTestingThreadRuning = false;

        foreach(var pd in new string[] { ".", "..", "..\\.." })
        {
            var cd = Path.GetFullPath(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pd));
            var ti = Path.Combine(cd, "train-images.idx3-ubyte");
            var tl = Path.Combine(cd, "train-labels.idx1-ubyte");
            var ki = Path.Combine(cd, "t10k-images.idx3-ubyte");
            var kl = Path.Combine(cd, "t10k-labels.idx1-ubyte");
            if (File.Exists(ti)
                && File.Exists(tl)
                && File.Exists(ki)
                && File.Exists(kl)
                )
            {
                if (IsTrainingDataReady = MnistTrainingDatabase.LoadMinstFiles(ti, tl))
                {
                    //update Preferences parametters
                    if (MnistTrainingDatabase.ImagePatterns.Count != Preference.ItemsTrainingImages)
                    {
                        Preference.ItemsTrainingImages = (uint)MnistTrainingDatabase.ImagePatterns.Count;
                        Preference.ItemsTrainingLabels = (uint)MnistTrainingDatabase.ImagePatterns.Count;
                    }
                    radioButtonMnistTrainDatabase.Enabled = true;
                    radioButtonTrainingdatabase.Enabled = true;
                    buttonMnistNext.Enabled = true;
                    buttonMnistPrevious.Enabled = true;
                    IsDatabaseReady = IsTrainingDataReady;
                    Mnistdatabase = MnistTrainingDatabase;
                }
                else
                {
                    radioButtonMnistTrainDatabase.Enabled = false;
                }
                if (IsTestingDataReady = MnistTestingDatabase.LoadMinstFiles(ki, kl))
                {
                    //update Preferences parametters
                    if (MnistTestingDatabase.ImagePatterns.Count != Preference.ItemsTestingImages)
                    {
                        Preference.ItemsTestingImages = (uint)MnistTestingDatabase.ImagePatterns.Count;
                        Preference.ItemsTestingLabels = (uint)MnistTestingDatabase.ImagePatterns.Count;
                    }
                    radioButtonMnistTestDatabase.Enabled = true;
                    radioButtonMnistTestDatabase.Checked = true;
                    radioButtonTestingdatabase.Enabled = true;
                    radioButtonTestingdatabase.Checked = true;
                    buttonMnistNext.Enabled = true;
                    buttonMnistPrevious.Enabled = true;
                    IsDatabaseReady = IsTestingDataReady;
                    Mnistdatabase = MnistTestingDatabase;
                }
                else
                {
                    radioButtonMnistTestDatabase.Enabled = false;
                }

                break;
            }
        }
    }
    private void AddText(int iCondition, string value)
    {
        switch (iCondition)
        {
            case 1:
                labelRecognizedValue.Text = value;
                break;
            case 2:
                label7.Text = value;
                break;
            case 3:
                listBoxProgress.Items.Add(value);
                break;
            case 4:
                label2.Text = value;
                break;
            case 5:
                label3.Text = value;
                break;
            case 6:
                listBox2.Items.Add(value);
                break;
            case 7:
                label14.Text = value;
                break;
            case 8:
                listBox2.Items.Add(value);
                IsTestingThreadRuning = false;
                buttonMnistTest.Enabled = true;
                radioButtonTestingdatabase.Enabled = true;
                radioButtonTrainingdatabase.Enabled = true;
                break;
            case 9:
                label7.Text = value;
                break;
            default:
                break;

        };
    }
    //draw training pattern to picturebox
    private void Next_Click(object sender, EventArgs e)
    {
        if (IsDatabaseReady)
        {
            if (CurrentMnistPattern < Mnistdatabase.ImagePatterns.Count - 1)
            {
                CurrentMnistPattern++;
                var bitmap = new Bitmap((int)Defaults.Global_ImageSize, (int)Defaults.Global_ImageSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var pArray = Mnistdatabase.ImagePatterns[CurrentMnistPattern].Pattern;
                uint label = Mnistdatabase.ImagePatterns[CurrentMnistPattern].Label;
                label6.Text = label.ToString();
                var colors = new byte[4];
                for (int i = 0; i < 28; i++)
                {

                    for (int j = 0; j < 28; j++)
                    {
                        colors[0] = 255;
                        colors[1] = (byte)(pArray[i * 28 + j]);
                        colors[2] = (byte)(pArray[i * 28 + j]);
                        colors[3] = (byte)(pArray[i * 28 + j]);
                        int m_ARGB = BitConverter.ToInt32(colors, 0);
                        bitmap.SetPixel(j, i, Color.FromArgb((int)m_ARGB));
                    }
                }
                PictureBox.Image = bitmap;
                ImagePatternRecognization(CurrentMnistPattern);
                label10.Text = CurrentMnistPattern.ToString();
            }
        }

    }
    private void ImagePatternRecognization(int index)
    {
        List<Mutex> mutexs = new(2);
        for (int i = 0; i < 2; i++)
        {
            var mutex = new Mutex();
            mutexs.Add(mutex);
        }

        var NNTessing = new NNTestPatterns(NN, Mnistdatabase, Preference, IsDatabaseReady, null, null, this, mutexs);
        var thread = new Thread(() => NNTessing.PatternRecognizingThread(index));
        thread.Start();
    }
    private void Previous_Click(object sender, EventArgs e)
    {
        if (IsDatabaseReady)
        {
            if (CurrentMnistPattern > 1)
            {
                CurrentMnistPattern -= 1;
                var bitmap = new Bitmap(Defaults.Global_ImageSize, Defaults.Global_ImageSize, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                var pArray = Mnistdatabase.ImagePatterns[CurrentMnistPattern].Pattern;
                uint ulabel = Mnistdatabase.ImagePatterns[CurrentMnistPattern].Label;
                label6.Text = ulabel.ToString();
                var colors = new byte[4];
                for (int i = 0; i < 28; i++)
                {
                    for (int j = 0; j < 28; j++)
                    {
                        colors[0] = 255;
                        colors[1] = pArray[i * 28 + j];
                        colors[2] = pArray[i * 28 + j];
                        colors[3] = pArray[i * 28 + j];
                        int rgb = BitConverter.ToInt32(colors, 0);
                        bitmap.SetPixel(j, i, Color.FromArgb(rgb));
                    }
                }
                PictureBox.Image = bitmap;
                ImagePatternRecognization(CurrentMnistPattern);
                label10.Text = CurrentMnistPattern.ToString();
            }
        }
    }
    private void StartBackPropagationbutton_Click(object sender, EventArgs e)
    {
        if (IsTrainingDataReady)
            OnStartBackpropagation();
    }
    /// <summary>
    /// 
    /// </summary>
    void OnStartBackpropagation()
    {
        if ((IsTrainingDataReady) && (!IsTrainingThreadRuning) 
            && (!IsTestingThreadRuning))
        {
            var parameters = new BackPropagationParameters
            {
                NumThreads = (uint)Preference.NumBackpropThreads,
                InitialEta = Preference.InitialEtaLearningRate,
                MinimumEta = Preference.MinimumEtaLearningRate,
                EtaDecay = Preference.LearningRateDecay,
                AfterEvery = Preference.AfterEveryNBackprops,
                StartingPattern = 0,
                EstimatedCurrentMSE = 0.10,
                UseDistortPatterns = true
            };
            parameters.InitialEtaMessage = string.Format("Initial Learning Rate eta (currently, eta = {0})", parameters.InitialEta);
            parameters.StartingPatternNum = string.Format("Starting Pattern Number (currently at {0})", 0);
            if (this.checkBoxUseDialog.Checked)
            {
                using var dlg = new BackPropagationParametersForm();
                dlg.BackProParameters = parameters;

                var result = dlg.ShowDialog();
                if (result == DialogResult.OK)
                {
                    parameters = dlg.BackProParameters;
                }
            }
            { 
                var ret = StartBackpropagation(parameters.StartingPattern, parameters.NumThreads, parameters.InitialEta,
                    parameters.MinimumEta, parameters.EtaDecay, parameters.AfterEvery, parameters.UseDistortPatterns, parameters.EstimatedCurrentMSE);
                if (ret)
                {
                    //do some thing
                    IsTrainingThreadRuning = true;
                    this.StartButton.Enabled = false;
                    this.StopButton.Enabled = true;
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
        NN.EtaLearningRate = initialEta;
        NN.EtaLearningRatePrevious = initialEta;

        //run thread here
        EventTrainingStopThread.Reset();
        EventTrainingThreadStopped.Reset();
        TrainerThreads = new List<Thread>(2);
        MnistTrainingDatabase.RandomizePatternSequence();
        //cleare mutex before run threads.
        var mutexs = new List<Mutex>(2);
        for (int i = 0; i < 4; i++)
        {
            mutexs.Add(new());
        }

        //create neural network
        try
        {
            CreateNNNetWork(TrainingNN);
            //initialize weight parameters to the network
            if (MnistWeightsFile != "")
            {
                MainMutex.WaitOne();
                var fsIn = new FileStream(MnistWeightsFile, FileMode.Open);
                var arIn = new Archive(fsIn, ArchiveOp.Load);
                TrainingNN.Serialize(arIn);
                fsIn.Close();
                MainMutex.ReleaseMutex();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
            return false;
        }
        //
        var ntraining = new NNTrainPatterns(TrainingNN, MnistTrainingDatabase, Preference, IsTrainingDataReady, EventTrainingStopThread,
            EventTrainingThreadStopped, this, mutexs)
        {
            MinimumEta = minimumEta,
            EtaDecay = etaDecay,
            AfterEveryNBackprops = nAfterEvery,
            ShouldDistortPatterns = bDistortPatterns,
            EstimatedCurrentMSE = estimatedCurrentMSE
            /* estimated number that will define whether a forward calculation's error is significant enough to warrant backpropagation*/
        };

        for (int i = 0; i < iNumThreads; i++)
        {
            var trainer_thread = new Thread(ntraining.BackpropagationThread)
            {
                Name = string.Format("Thread{0}", i + 1)
            };
            TrainerThreads.Add(trainer_thread);
            trainer_thread.Start();

        }

        return true;

    }
    /////////////////////////
    private bool CreateNNNetWork(NeuralNetwork network)
    {
        this.treeViewDigits.Nodes.Clear();

        NNLayer pLayer;
        int ii, jj, kk;
        int icNeurons = 0;
        int icWeights = 0;
        double initWeight;
        string sLabel;
        var random = new Random();
        // layer zero, the input layer.
        // Create neurons: exactly the same number of neurons as the input
        // vector of 29x29=841 pixels, and no weights/connections
        pLayer = new NNLayer("Layer00", null);
        network.Layers.Add(pLayer);

        var layerNode = new TreeNode(pLayer.Label) { Tag = pLayer };
        this.treeViewDigits.Nodes.Add(layerNode);
        for (ii = 0; ii < 841; ii++)
        {
            sLabel = string.Format("Layer00_Neuro{0}_Num{1}", ii, icNeurons);
            var neuron = new NNNeuron(sLabel);
            pLayer.Neurons.Add(neuron);
            icNeurons++;

            var neuronNode = new TreeNode(sLabel) { Tag = neuron };
            layerNode.Nodes.Add(neuronNode);
        }

        //double UNIFORM_PLUS_MINUS_ONE= (double)(2.0 * m_rdm.Next())/Constants.RAND_MAX - 1.0 ;

        // layer one:
        // This layer is a convolutional layer that has 6 feature maps.  Each feature 
        // map is 13x13, and each unit in the feature maps is a 5x5 convolutional kernel
        // of the input layer.
        // So, there are 13x13x6 = 1014 neurons, (5x5+1)x6 = 156 weights

        pLayer = new NNLayer("Layer01", pLayer);
        network.Layers.Add(pLayer);

        layerNode = new TreeNode(pLayer.Label) { Tag = pLayer };
        this.treeViewDigits.Nodes.Add(layerNode);

        for (ii = 0; ii < 1014; ii++)
        {
            sLabel = string.Format("Layer01_Neuron{0}_Num{1}", ii, icNeurons);
            var neuron = new NNNeuron(sLabel);
            pLayer.Neurons.Add(neuron);
            icNeurons++;

            var neuronNode = new TreeNode(sLabel) { Tag = neuron };
            layerNode.Nodes.Add(neuronNode);
        }

        for (ii = 0; ii < 156; ii++)
        {
            sLabel = string.Format("Layer01_Weigh{0}_Num{1}", ii, icWeights);
            initWeight = 0.05 * (2.0 * random.NextDouble() - 1.0);
            pLayer.Weights.Add(new (sLabel, initWeight));
        }

        // interconnections with previous layer: this is difficult
        // The previous layer is a top-down bitmap image that has been padded to size 29x29
        // Each neuron in this layer is connected to a 5x5 kernel in its feature map, which 
        // is also a top-down bitmap of size 13x13.  We move the kernel by TWO pixels, i.e., we
        // skip every other pixel in the input image

        int[] kernelTemplate = [
                0,  1,  2,  3,  4,
                29, 30, 31, 32, 33,
                58, 59, 60, 61, 62,
                87, 88, 89, 90, 91,
                116,117,118,119,120 ];

        int iNumWeight;

        int fm;

        for (fm = 0; fm < 6; fm++)
        {
            for (ii = 0; ii < 13; ii++)
            {
                for (jj = 0; jj < 13; jj++)
                {
                    iNumWeight = fm * 26;  // 26 is the number of weights per feature map
                    var n = pLayer.Neurons[jj + ii * 13 + fm * 169];

                    n.AddConnection((uint)Defaults.ULONG_MAX, (uint)iNumWeight++);  // bias weight

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
        network.Layers.Add(pLayer);
        layerNode = new TreeNode(pLayer.Label) { Tag = pLayer };
        this.treeViewDigits.Nodes.Add(layerNode);

        for (ii = 0; ii < 1250; ii++)
        {
            sLabel = string.Format("Layer02_Neuron{0}_Num{1}", ii, icNeurons);
            var neuron = new NNNeuron(sLabel);
            pLayer.Neurons.Add(neuron);
            icNeurons++;

            var neuronNode = new TreeNode(sLabel) { Tag = neuron };
            layerNode.Nodes.Add(neuronNode);

        }

        for (ii = 0; ii < 7800; ii++)
        {
            sLabel = string.Format("Layer02_Weight{0}_Num{1}", ii, icWeights);
            initWeight = 0.05 * (2.0 * random.NextDouble() - 1.0);
            pLayer.Weights.Add(new (sLabel, initWeight));
        }

        // Interconnections with previous layer: this is difficult
        // Each feature map in the previous layer is a top-down bitmap image whose size
        // is 13x13, and there are 6 such feature maps.  Each neuron in one 5x5 feature map of this 
        // layer is connected to a 5x5 kernel positioned correspondingly in all 6 parent
        // feature maps, and there are individual weights for the six different 5x5 kernels.  As
        // before, we move the kernel by TWO pixels, i.e., we
        // skip every other pixel in the input image.  The result is 50 different 5x5 top-down bitmap
        // feature maps

        int[] kernelTemplate2 = [
                0,  1,  2,  3,  4,
                13, 14, 15, 16, 17,
                26, 27, 28, 29, 30,
                39, 40, 41, 42, 43,
                52, 53, 54, 55, 56   ];


        for (fm = 0; fm < 50; fm++)
        {
            for (ii = 0; ii < 5; ii++)
            {
                for (jj = 0; jj < 5; jj++)
                {
                    iNumWeight = fm * 156;  // 26 is the number of weights per feature map
                    var n = pLayer.Neurons[jj + ii * 5 + fm * 25];

                    n.AddConnection((uint)Defaults.ULONG_MAX, (uint)iNumWeight++);  // bias weight

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
        network.Layers.Add(pLayer);
        layerNode = new TreeNode(pLayer.Label) { Tag = pLayer };
        this.treeViewDigits.Nodes.Add(layerNode);

        for (ii = 0; ii < 100; ii++)
        {
            sLabel = string.Format("Layer03_Neuron{0}_Num{1}", ii, icNeurons);

            var neuron = new NNNeuron(sLabel);
            pLayer.Neurons.Add(neuron);
            icNeurons++;

            var neuronNode = new TreeNode(sLabel) { Tag = neuron };
            layerNode.Nodes.Add(neuronNode);

        }

        for (ii = 0; ii < 125100; ii++)
        {
            sLabel = string.Format("Layer03_Weight{0}_Num{1}", ii, icWeights);
            initWeight = 0.05 * (2.0 * random.NextDouble() - 1.0);
            pLayer.Weights.Add(new (sLabel, initWeight));
        }

        // Interconnections with previous layer: fully-connected

        iNumWeight = 0;  // weights are not shared in this layer

        for (fm = 0; fm < 100; fm++)
        {
            var n = pLayer.Neurons[fm];
            n.AddConnection((uint)Defaults.ULONG_MAX, (uint)iNumWeight++);  // bias weight

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
        network.Layers.Add(pLayer);
        layerNode = new TreeNode(pLayer.Label) { Tag = pLayer };
        this.treeViewDigits.Nodes.Add(layerNode);

        for (ii = 0; ii < 10; ii++)
        {
            sLabel = string.Format("Layer04_Neuron{0}_Num{1}", ii, icNeurons);
            var neuron = new NNNeuron(sLabel);
            pLayer.Neurons.Add(neuron);
            icNeurons++;

            var neuronNode = new TreeNode(sLabel) { Tag = neuron };
            layerNode.Nodes.Add(neuronNode);
        }

        for (ii = 0; ii < 1010; ii++)
        {
            sLabel = string.Format("Layer04_Weight{0}_Num{1}", ii, icWeights);
            initWeight = 0.05 * (2.0 * random.NextDouble() - 1.0);
            pLayer.Weights.Add(new (sLabel, initWeight));
        }

        // Interconnections with previous layer: fully-connected

        iNumWeight = 0;  // weights are not shared in this layer

        for (fm = 0; fm < 10; fm++)
        {
            var n = pLayer.Neurons[fm];
            n.AddConnection((uint)Defaults.ULONG_MAX, (uint)iNumWeight++);  // bias weight

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
        if (IsTrainingThreadRuning)
        {
            if (StopTheads(TrainerThreads, EventTrainingStopThread, EventTrainingThreadStopped))
            {
                BackPropagationThreadsFinished();       // set initial state of buttons
                this.StartButton.Enabled = true;
                this.StopButton.Enabled = false;
            }
        }
    }

    void BackPropagationThreadsFinished()
    {
        if (IsTrainingThreadRuning)
        {
            var msResult = MessageBox.Show("Do you want to save Neural Network data ?", "Save Neural Network Data", MessageBoxButtons.OKCancel);
            if (msResult == DialogResult.OK)
            {
                using var dialog = new System.Windows.Forms.SaveFileDialog { Filter = "Mnist Neural network file (*.nnt)|*.nnt", Title = "Save Neural network File" };
                if (dialog.ShowDialog() == DialogResult.OK)
                {

                    var fsIn = dialog.OpenFile();
                    var arIn = new Archive(fsIn, ArchiveOp.Store);
                    TrainingNN.Serialize(arIn);
                    fsIn.Close();
                }
            }
            IsTrainingThreadRuning = false;
        }
        return;
    }
    // Load Image from file
    private Bitmap CreateNonIndexedImage(Bitmap src)
    {
        var newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using (var gfx = Graphics.FromImage(newBmp))
        {
            gfx.DrawImage(src, 0, 0);
        }
        return newBmp;
    }

    private void NetworkParametersToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var dialog = new System.Windows.Forms.OpenFileDialog { Filter = "Mnist Neural network file (*.nnt)|*.nnt", Title = "Open Neural network File" };
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            MainMutex.WaitOne();
            MnistWeightsFile = dialog.FileName;
            var fsIn = dialog.OpenFile();
            var arIn = new Archive(fsIn, ArchiveOp.Load);
            NN.Serialize(arIn);
            fsIn.Close();
            MainMutex.ReleaseMutex();
        }
    }

    private void MNISTDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
    {
        IsTrainingDataReady = MnistTrainingDatabase.LoadMinstFiles();
        if (IsTrainingDataReady)
        {

            //update Preferences parametters
            if (MnistTrainingDatabase.ImagePatterns.Count != Preference.ItemsTrainingImages)
            {
                Preference.ItemsTrainingImages = (uint)MnistTrainingDatabase.ImagePatterns.Count;
                Preference.ItemsTrainingLabels = (uint)MnistTrainingDatabase.ImagePatterns.Count;
            }
            radioButtonMnistTrainDatabase.Enabled = true;
            radioButtonTrainingdatabase.Enabled = true;
            buttonMnistNext.Enabled = true;
            buttonMnistPrevious.Enabled = true;
            IsDatabaseReady = IsTrainingDataReady;
            Mnistdatabase = MnistTrainingDatabase;
        }
        else
        {
            radioButtonMnistTrainDatabase.Enabled = false;
            return;
        }
        IsTestingDataReady = MnistTestingDatabase.LoadMinstFiles();
        if (IsTestingDataReady)
        {
            //update Preferences parametters
            if (MnistTestingDatabase.ImagePatterns.Count != Preference.ItemsTestingImages)
            {
                Preference.ItemsTestingImages = (uint)MnistTestingDatabase.ImagePatterns.Count;
                Preference.ItemsTestingLabels = (uint)MnistTestingDatabase.ImagePatterns.Count;
            }
            radioButtonMnistTestDatabase.Enabled = true;
            radioButtonMnistTestDatabase.Checked = true;
            radioButtonTestingdatabase.Enabled = true;
            radioButtonTestingdatabase.Checked = true;
            buttonMnistNext.Enabled = true;
            buttonMnistPrevious.Enabled = true;
            IsDatabaseReady = IsTestingDataReady;
            Mnistdatabase = MnistTestingDatabase;
        }
        else
        {
            radioButtonMnistTestDatabase.Enabled = false;
            return;
        }
    }

    private void ButtonMnistTest_Click(object sender, EventArgs e)
    {
        if ((IsTestingThreadRuning == false) && (IsTrainingThreadRuning == false))
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
                if (MnistWeightsFile != "")
                {
                    MainMutex.WaitOne();
                    var fsIn = new FileStream(MnistWeightsFile, FileMode.Open);
                    var arIn = new Archive(fsIn, ArchiveOp.Load);
                    nnNetwork.Serialize(arIn);
                    fsIn.Close();
                    MainMutex.ReleaseMutex();
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
                if (IsTestingDataReady)
                {
                    nnTesting = new NNTestPatterns(nnNetwork, MnistTestingDatabase, Preference, IsTestingDataReady, EventTestingStopThread, EventTestingThreadStopped, this, mutexs);
                    bDatabaseforTest = IsTestingDataReady;
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (IsTrainingDataReady)
                {
                    nnTesting = new NNTestPatterns(nnNetwork, MnistTrainingDatabase, Preference, IsTrainingDataReady, EventTestingStopThread, EventTestingThreadStopped, this, mutexs);
                    bDatabaseforTest = IsTrainingDataReady;
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
                EventTestingStopThread.Reset();
                EventTestingThreadStopped.Reset();
                TestingThreads = new List<Thread>(2);

                try
                {
                    for (int i = 0; i < theadsNum; i++)
                    {
                        var thread = new Thread(delegate ()
                                                    {
                                                        nnTesting.PatternsTestingThread((int)numericUpDownNumberofTestPattern.Value);
                                                    });
                        TestingThreads.Add(thread);
                        thread.Start();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
                IsTestingThreadRuning = true;
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
    private void ButtonStopMnistTest_Click(object sender, EventArgs e)
    {
        if (IsTestingThreadRuning)
        {
            if (StopTheads(TestingThreads, EventTestingStopThread, EventTestingThreadStopped))
            {
                IsTestingThreadRuning = false;
                radioButtonTestingdatabase.Enabled = true;
                radioButtonTrainingdatabase.Enabled = true;
                buttonMnistTest.Enabled = true;

            }
        }
        //grayscale bitmap
    }

    private void RadioButtonTestingdatabase_CheckedChanged(object sender, EventArgs e)
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

    private void RadioButton2_CheckedChanged(object sender, EventArgs e)
    {
        if (radioButtonMnistTestDatabase.Checked)
        {
            Mnistdatabase = MnistTestingDatabase;
            IsDatabaseReady = IsTestingDataReady;
            CurrentMnistPattern = 0;
        }
        else
        {
            Mnistdatabase = MnistTestingDatabase;
            IsDatabaseReady = IsTrainingDataReady;
            CurrentMnistPattern = 0;
        }
    }

    private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (IsTestingThreadRuning || IsTrainingThreadRuning)
        {
            _ = MessageBox.Show("Sorry, some threads are running. Please stop them before  you can close the program", "", MessageBoxButtons.OK);
            e.Cancel = true;
        }

    }

    private void ViewHelpToolStripMenuItem_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Handwritten character recognition program Vesion 0.1,\nCopyright (C) 2010-2011, \nPham Viet Dung, Vietnam Maritime University" +
            "\nEmail:vietdungiitb@vimaru.edu.vn",
            "About Handwritten character recognition program", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}