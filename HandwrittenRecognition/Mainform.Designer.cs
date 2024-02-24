namespace HandwrittenRecogniration
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                MainMutex.Dispose();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.networkParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mNISTDatabaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.labelRecognizedValue = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.buttonPRPrevious = new System.Windows.Forms.Button();
            this.buttonPRNext = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.radioButtonMnistTestDatabase = new System.Windows.Forms.RadioButton();
            this.radioButtonMnistTrainDatabase = new System.Windows.Forms.RadioButton();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonMnistNext = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonMnistPrevious = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.numericUpDownThreads = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownNumberofTestPattern = new System.Windows.Forms.NumericUpDown();
            this.radioButtonTestingdatabase = new System.Windows.Forms.RadioButton();
            this.radioButtonTrainingdatabase = new System.Windows.Forms.RadioButton();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.buttonMnistStopTest = new System.Windows.Forms.Button();
            this.buttonMnistTest = new System.Windows.Forms.Button();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.StopButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.StartButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBoxMain = new System.Windows.Forms.PictureBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.checkBoxUseDialog = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThreads)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumberofTestPattern)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1203, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.networkParametersToolStripMenuItem,
            this.mNISTDatabaseToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // networkParametersToolStripMenuItem
            // 
            this.networkParametersToolStripMenuItem.Name = "networkParametersToolStripMenuItem";
            this.networkParametersToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.networkParametersToolStripMenuItem.Text = "Network Parameters";
            this.networkParametersToolStripMenuItem.Click += new System.EventHandler(this.NetworkParametersToolStripMenuItem_Click);
            // 
            // mNISTDatabaseToolStripMenuItem
            // 
            this.mNISTDatabaseToolStripMenuItem.Name = "mNISTDatabaseToolStripMenuItem";
            this.mNISTDatabaseToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.mNISTDatabaseToolStripMenuItem.Text = "MNIST Database";
            this.mNISTDatabaseToolStripMenuItem.Click += new System.EventHandler(this.MNISTDatabaseToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewHelpToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(47, 21);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // viewHelpToolStripMenuItem
            // 
            this.viewHelpToolStripMenuItem.Name = "viewHelpToolStripMenuItem";
            this.viewHelpToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.viewHelpToolStripMenuItem.Text = "About";
            this.viewHelpToolStripMenuItem.Click += new System.EventHandler(this.viewHelpToolStripMenuItem_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox3);
            this.splitContainer2.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer2.Size = new System.Drawing.Size(509, 539);
            this.splitContainer2.SplitterDistance = 192;
            this.splitContainer2.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.labelRecognizedValue);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.pictureBox1);
            this.groupBox3.Controls.Add(this.buttonPRPrevious);
            this.groupBox3.Controls.Add(this.buttonPRNext);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(256, 192);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Pattern Recognition";
            // 
            // labelRecognizedValue
            // 
            this.labelRecognizedValue.AutoSize = true;
            this.labelRecognizedValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRecognizedValue.ForeColor = System.Drawing.Color.Red;
            this.labelRecognizedValue.Location = new System.Drawing.Point(174, 24);
            this.labelRecognizedValue.Name = "labelRecognizedValue";
            this.labelRecognizedValue.Size = new System.Drawing.Size(62, 37);
            this.labelRecognizedValue.TabIndex = 6;
            this.labelRecognizedValue.Text = "     ";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(3, 30);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(167, 24);
            this.label9.TabIndex = 5;
            this.label9.Text = "Recognized value:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(6, 80);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(121, 106);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // buttonPRPrevious
            // 
            this.buttonPRPrevious.Enabled = false;
            this.buttonPRPrevious.Location = new System.Drawing.Point(143, 80);
            this.buttonPRPrevious.Name = "buttonPRPrevious";
            this.buttonPRPrevious.Size = new System.Drawing.Size(38, 48);
            this.buttonPRPrevious.TabIndex = 4;
            this.buttonPRPrevious.Text = "Previous";
            this.buttonPRPrevious.UseVisualStyleBackColor = true;
            // 
            // buttonPRNext
            // 
            this.buttonPRNext.Enabled = false;
            this.buttonPRNext.Location = new System.Drawing.Point(203, 80);
            this.buttonPRNext.Name = "buttonPRNext";
            this.buttonPRNext.Size = new System.Drawing.Size(38, 48);
            this.buttonPRNext.TabIndex = 2;
            this.buttonPRNext.Text = "Next";
            this.buttonPRNext.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.radioButtonMnistTestDatabase);
            this.groupBox2.Controls.Add(this.radioButtonMnistTrainDatabase);
            this.groupBox2.Controls.Add(this.pictureBox2);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.buttonMnistNext);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.buttonMnistPrevious);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox2.Location = new System.Drawing.Point(256, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(253, 192);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "MNIST manual test";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(126, 63);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(23, 12);
            this.label10.TabIndex = 9;
            this.label10.Text = "0  ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(32, 63);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 12);
            this.label8.TabIndex = 9;
            this.label8.Text = "Pattern No.:";
            // 
            // radioButtonMnistTestDatabase
            // 
            this.radioButtonMnistTestDatabase.AutoSize = true;
            this.radioButtonMnistTestDatabase.Checked = true;
            this.radioButtonMnistTestDatabase.Enabled = false;
            this.radioButtonMnistTestDatabase.Location = new System.Drawing.Point(134, 173);
            this.radioButtonMnistTestDatabase.Name = "radioButtonMnistTestDatabase";
            this.radioButtonMnistTestDatabase.Size = new System.Drawing.Size(119, 16);
            this.radioButtonMnistTestDatabase.TabIndex = 8;
            this.radioButtonMnistTestDatabase.TabStop = true;
            this.radioButtonMnistTestDatabase.Text = "Testing Database";
            this.radioButtonMnistTestDatabase.UseVisualStyleBackColor = true;
            this.radioButtonMnistTestDatabase.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButtonMnistTrainDatabase
            // 
            this.radioButtonMnistTrainDatabase.AutoSize = true;
            this.radioButtonMnistTrainDatabase.Enabled = false;
            this.radioButtonMnistTrainDatabase.Location = new System.Drawing.Point(134, 143);
            this.radioButtonMnistTrainDatabase.Name = "radioButtonMnistTrainDatabase";
            this.radioButtonMnistTrainDatabase.Size = new System.Drawing.Size(119, 16);
            this.radioButtonMnistTrainDatabase.TabIndex = 7;
            this.radioButtonMnistTrainDatabase.TabStop = true;
            this.radioButtonMnistTrainDatabase.Text = "Traning Database";
            this.radioButtonMnistTrainDatabase.UseVisualStyleBackColor = true;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Location = new System.Drawing.Point(6, 80);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(121, 106);
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(167, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 12);
            this.label7.TabIndex = 6;
            this.label7.Text = "0   ";
            // 
            // buttonMnistNext
            // 
            this.buttonMnistNext.Enabled = false;
            this.buttonMnistNext.Location = new System.Drawing.Point(200, 80);
            this.buttonMnistNext.Name = "buttonMnistNext";
            this.buttonMnistNext.Size = new System.Drawing.Size(38, 48);
            this.buttonMnistNext.TabIndex = 2;
            this.buttonMnistNext.Text = "Next";
            this.buttonMnistNext.UseVisualStyleBackColor = true;
            this.buttonMnistNext.Click += new System.EventHandler(this.Next_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(167, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(11, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "0";
            // 
            // buttonMnistPrevious
            // 
            this.buttonMnistPrevious.Enabled = false;
            this.buttonMnistPrevious.Location = new System.Drawing.Point(140, 80);
            this.buttonMnistPrevious.Name = "buttonMnistPrevious";
            this.buttonMnistPrevious.Size = new System.Drawing.Size(38, 48);
            this.buttonMnistPrevious.TabIndex = 4;
            this.buttonMnistPrevious.Text = "Previous";
            this.buttonMnistPrevious.UseVisualStyleBackColor = true;
            this.buttonMnistPrevious.Click += new System.EventHandler(this.Previous_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(29, 36);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 6;
            this.label5.Text = "Recognized No:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "Actual No:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tabControl1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(509, 343);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Neural network training";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(3, 17);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(503, 266);
            this.tabControl1.TabIndex = 10;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Controls.Add(this.label13);
            this.tabPage2.Controls.Add(this.numericUpDownThreads);
            this.tabPage2.Controls.Add(this.numericUpDownNumberofTestPattern);
            this.tabPage2.Controls.Add(this.radioButtonTestingdatabase);
            this.tabPage2.Controls.Add(this.radioButtonTrainingdatabase);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.buttonMnistStopTest);
            this.tabPage2.Controls.Add(this.buttonMnistTest);
            this.tabPage2.Controls.Add(this.listBox2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(495, 240);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "NN Testing";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(94, 74);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(11, 12);
            this.label14.TabIndex = 7;
            this.label14.Text = "0";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(7, 74);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(101, 12);
            this.label13.TabIndex = 6;
            this.label13.Text = "Current Pattern:";
            // 
            // numericUpDownThreads
            // 
            this.numericUpDownThreads.Location = new System.Drawing.Point(368, 35);
            this.numericUpDownThreads.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownThreads.Name = "numericUpDownThreads";
            this.numericUpDownThreads.Size = new System.Drawing.Size(40, 21);
            this.numericUpDownThreads.TabIndex = 5;
            this.numericUpDownThreads.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // numericUpDownNumberofTestPattern
            // 
            this.numericUpDownNumberofTestPattern.Location = new System.Drawing.Point(318, 12);
            this.numericUpDownNumberofTestPattern.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.numericUpDownNumberofTestPattern.Name = "numericUpDownNumberofTestPattern";
            this.numericUpDownNumberofTestPattern.Size = new System.Drawing.Size(90, 21);
            this.numericUpDownNumberofTestPattern.TabIndex = 5;
            this.numericUpDownNumberofTestPattern.Value = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            // 
            // radioButtonTestingdatabase
            // 
            this.radioButtonTestingdatabase.AutoSize = true;
            this.radioButtonTestingdatabase.Checked = true;
            this.radioButtonTestingdatabase.Location = new System.Drawing.Point(35, 35);
            this.radioButtonTestingdatabase.Name = "radioButtonTestingdatabase";
            this.radioButtonTestingdatabase.Size = new System.Drawing.Size(119, 16);
            this.radioButtonTestingdatabase.TabIndex = 4;
            this.radioButtonTestingdatabase.TabStop = true;
            this.radioButtonTestingdatabase.Text = "Testing database";
            this.radioButtonTestingdatabase.UseVisualStyleBackColor = true;
            this.radioButtonTestingdatabase.CheckedChanged += new System.EventHandler(this.RadioButtonTestingdatabase_CheckedChanged);
            // 
            // radioButtonTrainingdatabase
            // 
            this.radioButtonTrainingdatabase.AutoSize = true;
            this.radioButtonTrainingdatabase.Location = new System.Drawing.Point(35, 12);
            this.radioButtonTrainingdatabase.Name = "radioButtonTrainingdatabase";
            this.radioButtonTrainingdatabase.Size = new System.Drawing.Size(125, 16);
            this.radioButtonTrainingdatabase.TabIndex = 4;
            this.radioButtonTrainingdatabase.Text = "Training database";
            this.radioButtonTrainingdatabase.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(193, 37);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(137, 12);
            this.label12.TabIndex = 3;
            this.label12.Text = "Number of Test thread:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(193, 14);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(143, 12);
            this.label11.TabIndex = 3;
            this.label11.Text = "Number of Test pattern:";
            // 
            // buttonMnistStopTest
            // 
            this.buttonMnistStopTest.Location = new System.Drawing.Point(352, 65);
            this.buttonMnistStopTest.Name = "buttonMnistStopTest";
            this.buttonMnistStopTest.Size = new System.Drawing.Size(135, 21);
            this.buttonMnistStopTest.TabIndex = 2;
            this.buttonMnistStopTest.Text = "Stop Mnist Test";
            this.buttonMnistStopTest.UseVisualStyleBackColor = true;
            this.buttonMnistStopTest.Click += new System.EventHandler(this.ButtonStopMnistTest_Click);
            // 
            // buttonMnistTest
            // 
            this.buttonMnistTest.Location = new System.Drawing.Point(209, 65);
            this.buttonMnistTest.Name = "buttonMnistTest";
            this.buttonMnistTest.Size = new System.Drawing.Size(126, 21);
            this.buttonMnistTest.TabIndex = 1;
            this.buttonMnistTest.Text = "Start Mnist Test";
            this.buttonMnistTest.UseVisualStyleBackColor = true;
            this.buttonMnistTest.Click += new System.EventHandler(this.ButtonMnistTest_Click);
            // 
            // listBox2
            // 
            this.listBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listBox2.FormattingEnabled = true;
            this.listBox2.ItemHeight = 12;
            this.listBox2.Location = new System.Drawing.Point(3, 89);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(489, 148);
            this.listBox2.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.checkBoxUseDialog);
            this.tabPage1.Controls.Add(this.listBox1);
            this.tabPage1.Controls.Add(this.StopButton);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.StartButton);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(495, 240);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "NN Training";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(3, 89);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(489, 148);
            this.listBox1.TabIndex = 9;
            // 
            // StopButton
            // 
            this.StopButton.Enabled = false;
            this.StopButton.Location = new System.Drawing.Point(304, 61);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(145, 21);
            this.StopButton.TabIndex = 1;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopBackPropagationbutton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(115, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 8;
            this.label3.Text = "0            ";
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(113, 61);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(136, 21);
            this.StartButton.TabIndex = 0;
            this.StartButton.Text = "Start BackPropagation";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartBackPropagationbutton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "Current pattern: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(252, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "MSE: 0";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 8F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.pictureBoxMain, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 542F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(690, 539);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // pictureBoxMain
            // 
            this.pictureBoxMain.BackColor = System.Drawing.Color.White;
            this.pictureBoxMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxMain.Location = new System.Drawing.Point(11, 3);
            this.pictureBoxMain.Name = "pictureBoxMain";
            this.pictureBoxMain.Size = new System.Drawing.Size(676, 533);
            this.pictureBoxMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxMain.TabIndex = 1;
            this.pictureBoxMain.TabStop = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1203, 539);
            this.splitContainer1.SplitterDistance = 690;
            this.splitContainer1.TabIndex = 0;
            // 
            // checkBoxUseDialog
            // 
            this.checkBoxUseDialog.AutoSize = true;
            this.checkBoxUseDialog.Location = new System.Drawing.Point(8, 63);
            this.checkBoxUseDialog.Name = "checkBoxUseDialog";
            this.checkBoxUseDialog.Size = new System.Drawing.Size(84, 16);
            this.checkBoxUseDialog.TabIndex = 10;
            this.checkBoxUseDialog.Text = "Use Dialog";
            this.checkBoxUseDialog.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1203, 564);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Handwritten character recognition system";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Mainform_FormClosing);
            this.Load += new System.EventHandler(this.Mainform_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownThreads)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumberofTestPattern)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMain)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem networkParametersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mNISTDatabaseToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.RadioButton radioButtonMnistTestDatabase;
        private System.Windows.Forms.RadioButton radioButtonMnistTrainDatabase;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonMnistNext;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonMnistPrevious;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBoxMain;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonPRPrevious;
        private System.Windows.Forms.Button buttonPRNext;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.Button buttonMnistTest;
        private System.Windows.Forms.Label labelRecognizedValue;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button buttonMnistStopTest;
        private System.Windows.Forms.NumericUpDown numericUpDownThreads;
        private System.Windows.Forms.NumericUpDown numericUpDownNumberofTestPattern;
        private System.Windows.Forms.RadioButton radioButtonTestingdatabase;
        private System.Windows.Forms.RadioButton radioButtonTrainingdatabase;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewHelpToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxUseDialog;
    }
}

