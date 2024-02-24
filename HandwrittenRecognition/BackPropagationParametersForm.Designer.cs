namespace HandwrittenRecogniration
{
    partial class BackPropagationParametersForm
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
            this.textBoxBackThreads = new System.Windows.Forms.TextBox();
            this.textBoxILearningRateEta = new System.Windows.Forms.TextBox();
            this.textBoxMinimumLearningRate = new System.Windows.Forms.TextBox();
            this.textBoxLearningRateDecayRate = new System.Windows.Forms.TextBox();
            this.textBoxAfterEveryNBackPropagations = new System.Windows.Forms.TextBox();
            this.textBoxStartingPatternNumber = new System.Windows.Forms.TextBox();
            this.textBoxEstimateofCurrentMSE = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.StartButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.checkBoxDistortPatterns = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textBoxBackThreads
            // 
            this.textBoxBackThreads.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxBackThreads.Location = new System.Drawing.Point(416, 16);
            this.textBoxBackThreads.Name = "textBoxBackThreads";
            this.textBoxBackThreads.Size = new System.Drawing.Size(100, 21);
            this.textBoxBackThreads.TabIndex = 0;
            // 
            // textBoxILearningRateEta
            // 
            this.textBoxILearningRateEta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxILearningRateEta.Location = new System.Drawing.Point(416, 49);
            this.textBoxILearningRateEta.Name = "textBoxILearningRateEta";
            this.textBoxILearningRateEta.Size = new System.Drawing.Size(100, 21);
            this.textBoxILearningRateEta.TabIndex = 0;
            // 
            // textBoxMinimumLearningRate
            // 
            this.textBoxMinimumLearningRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMinimumLearningRate.Location = new System.Drawing.Point(416, 84);
            this.textBoxMinimumLearningRate.Name = "textBoxMinimumLearningRate";
            this.textBoxMinimumLearningRate.Size = new System.Drawing.Size(100, 21);
            this.textBoxMinimumLearningRate.TabIndex = 0;
            // 
            // textBoxLearningRateDecayRate
            // 
            this.textBoxLearningRateDecayRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLearningRateDecayRate.Location = new System.Drawing.Point(416, 121);
            this.textBoxLearningRateDecayRate.Name = "textBoxLearningRateDecayRate";
            this.textBoxLearningRateDecayRate.Size = new System.Drawing.Size(100, 21);
            this.textBoxLearningRateDecayRate.TabIndex = 0;
            // 
            // textBoxAfterEveryNBackPropagations
            // 
            this.textBoxAfterEveryNBackPropagations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxAfterEveryNBackPropagations.Location = new System.Drawing.Point(416, 156);
            this.textBoxAfterEveryNBackPropagations.Name = "textBoxAfterEveryNBackPropagations";
            this.textBoxAfterEveryNBackPropagations.Size = new System.Drawing.Size(100, 21);
            this.textBoxAfterEveryNBackPropagations.TabIndex = 0;
            // 
            // textBoxStartingPatternNumber
            // 
            this.textBoxStartingPatternNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxStartingPatternNumber.Location = new System.Drawing.Point(416, 190);
            this.textBoxStartingPatternNumber.Name = "textBoxStartingPatternNumber";
            this.textBoxStartingPatternNumber.Size = new System.Drawing.Size(100, 21);
            this.textBoxStartingPatternNumber.TabIndex = 0;
            // 
            // textBoxEstimateofCurrentMSE
            // 
            this.textBoxEstimateofCurrentMSE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxEstimateofCurrentMSE.Location = new System.Drawing.Point(416, 224);
            this.textBoxEstimateofCurrentMSE.Name = "textBoxEstimateofCurrentMSE";
            this.textBoxEstimateofCurrentMSE.Size = new System.Drawing.Size(100, 21);
            this.textBoxEstimateofCurrentMSE.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(293, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Number of Backprop threads (one per CPU is best)";
            // 
            // StartButton
            // 
            this.StartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.StartButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.StartButton.Location = new System.Drawing.Point(52, 314);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(131, 21);
            this.StartButton.TabIndex = 2;
            this.StartButton.Text = "Start BackPropagation";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton.Location = new System.Drawing.Point(345, 314);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(140, 21);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel BackPropagation";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(335, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Initial Learning Rate eta (currently, eta = 0.00000001)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "Minimum Learning Rate";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(257, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "Learning Rate Decay Rate (multiply eta by)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 162);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(239, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "After Every N Backpropagations: N =    ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 197);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(275, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "Starting Pattern Number (currently at 100000)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(24, 231);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(299, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "Estimate of current MSE (enter 0.10 if uncertain)";
            // 
            // checkBoxDistortPatterns
            // 
            this.checkBoxDistortPatterns.AutoSize = true;
            this.checkBoxDistortPatterns.Location = new System.Drawing.Point(27, 265);
            this.checkBoxDistortPatterns.Name = "checkBoxDistortPatterns";
            this.checkBoxDistortPatterns.Size = new System.Drawing.Size(372, 16);
            this.checkBoxDistortPatterns.TabIndex = 3;
            this.checkBoxDistortPatterns.Text = "Distort Patterns (recommended for improved generalization)";
            this.checkBoxDistortPatterns.UseVisualStyleBackColor = true;
            // 
            // BackPropagationParametersForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 341);
            this.Controls.Add(this.checkBoxDistortPatterns);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxEstimateofCurrentMSE);
            this.Controls.Add(this.textBoxStartingPatternNumber);
            this.Controls.Add(this.textBoxAfterEveryNBackPropagations);
            this.Controls.Add(this.textBoxLearningRateDecayRate);
            this.Controls.Add(this.textBoxMinimumLearningRate);
            this.Controls.Add(this.textBoxILearningRateEta);
            this.Controls.Add(this.textBoxBackThreads);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BackPropagationParametersForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Back Propagation Parameters";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxBackThreads;
        private System.Windows.Forms.TextBox textBoxILearningRateEta;
        private System.Windows.Forms.TextBox textBoxMinimumLearningRate;
        private System.Windows.Forms.TextBox textBoxLearningRateDecayRate;
        private System.Windows.Forms.TextBox textBoxAfterEveryNBackPropagations;
        private System.Windows.Forms.TextBox textBoxStartingPatternNumber;
        private System.Windows.Forms.TextBox textBoxEstimateofCurrentMSE;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox checkBoxDistortPatterns;
    }
}