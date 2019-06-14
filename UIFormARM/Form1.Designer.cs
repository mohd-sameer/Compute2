namespace UIFormARM
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btn_LoadSettingsFile = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.textBoxParameterSet = new System.Windows.Forms.TextBox();
            this.labelParameterSet = new System.Windows.Forms.Label();
            this.textBoxOutputFolder = new System.Windows.Forms.TextBox();
            this.labelOutputFolder = new System.Windows.Forms.Label();
            this.labelLandscapeFile = new System.Windows.Forms.Label();
            this.textBoxLandscapeFile = new System.Windows.Forms.TextBox();
            this.labelPopulationFile = new System.Windows.Forms.Label();
            this.textBoxPopulationFile = new System.Windows.Forms.TextBox();
            this.labelNumbIterations = new System.Windows.Forms.Label();
            this.labelNumbYears = new System.Windows.Forms.Label();
            this.textBoxNumbIterations = new System.Windows.Forms.TextBox();
            this.textBoxNumbYears = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_LoadSettingsFile
            // 
            this.btn_LoadSettingsFile.BackColor = System.Drawing.SystemColors.Control;
            this.btn_LoadSettingsFile.Location = new System.Drawing.Point(17, 17);
            this.btn_LoadSettingsFile.Name = "btn_LoadSettingsFile";
            this.btn_LoadSettingsFile.Size = new System.Drawing.Size(103, 23);
            this.btn_LoadSettingsFile.TabIndex = 0;
            this.btn_LoadSettingsFile.Text = "Load Settings File";
            this.btn_LoadSettingsFile.UseVisualStyleBackColor = false;
            this.btn_LoadSettingsFile.Click += new System.EventHandler(this.btn_LoadSettingsFile_Click);
            // 
            // btnRun
            // 
            this.btnRun.Enabled = false;
            this.btnRun.Location = new System.Drawing.Point(17, 290);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 1;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(124, 290);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // textBoxParameterSet
            // 
            this.textBoxParameterSet.Location = new System.Drawing.Point(17, 72);
            this.textBoxParameterSet.Name = "textBoxParameterSet";
            this.textBoxParameterSet.ReadOnly = true;
            this.textBoxParameterSet.Size = new System.Drawing.Size(437, 20);
            this.textBoxParameterSet.TabIndex = 3;
            this.textBoxParameterSet.TextChanged += new System.EventHandler(this.textBoxParameterSet_TextChanged);
            // 
            // labelParameterSet
            // 
            this.labelParameterSet.AutoSize = true;
            this.labelParameterSet.Location = new System.Drawing.Point(17, 56);
            this.labelParameterSet.Name = "labelParameterSet";
            this.labelParameterSet.Size = new System.Drawing.Size(101, 13);
            this.labelParameterSet.TabIndex = 4;
            this.labelParameterSet.Text = "Parameter set name";
            // 
            // textBoxOutputFolder
            // 
            this.textBoxOutputFolder.Location = new System.Drawing.Point(17, 259);
            this.textBoxOutputFolder.Name = "textBoxOutputFolder";
            this.textBoxOutputFolder.ReadOnly = true;
            this.textBoxOutputFolder.Size = new System.Drawing.Size(437, 20);
            this.textBoxOutputFolder.TabIndex = 5;
            this.textBoxOutputFolder.TextChanged += new System.EventHandler(this.textBoxOutputFolder_TextChanged);
            // 
            // labelOutputFolder
            // 
            this.labelOutputFolder.AutoSize = true;
            this.labelOutputFolder.Location = new System.Drawing.Point(17, 240);
            this.labelOutputFolder.Name = "labelOutputFolder";
            this.labelOutputFolder.Size = new System.Drawing.Size(68, 13);
            this.labelOutputFolder.TabIndex = 6;
            this.labelOutputFolder.Text = "Output folder";
            // 
            // labelLandscapeFile
            // 
            this.labelLandscapeFile.AutoSize = true;
            this.labelLandscapeFile.Location = new System.Drawing.Point(17, 102);
            this.labelLandscapeFile.Name = "labelLandscapeFile";
            this.labelLandscapeFile.Size = new System.Drawing.Size(79, 13);
            this.labelLandscapeFile.TabIndex = 7;
            this.labelLandscapeFile.Text = "Landscape File";
            this.labelLandscapeFile.Click += new System.EventHandler(this.labelLandscapeFile_Click);
            // 
            // textBoxLandscapeFile
            // 
            this.textBoxLandscapeFile.Location = new System.Drawing.Point(17, 117);
            this.textBoxLandscapeFile.Name = "textBoxLandscapeFile";
            this.textBoxLandscapeFile.ReadOnly = true;
            this.textBoxLandscapeFile.Size = new System.Drawing.Size(437, 20);
            this.textBoxLandscapeFile.TabIndex = 8;
            this.textBoxLandscapeFile.TextChanged += new System.EventHandler(this.textBoxLandscapeFile_TextChanged);
            // 
            // labelPopulationFile
            // 
            this.labelPopulationFile.AutoSize = true;
            this.labelPopulationFile.Location = new System.Drawing.Point(17, 148);
            this.labelPopulationFile.Name = "labelPopulationFile";
            this.labelPopulationFile.Size = new System.Drawing.Size(76, 13);
            this.labelPopulationFile.TabIndex = 9;
            this.labelPopulationFile.Text = "Population File";
            this.labelPopulationFile.Click += new System.EventHandler(this.labelPopulationFile_Click);
            // 
            // textBoxPopulationFile
            // 
            this.textBoxPopulationFile.Location = new System.Drawing.Point(17, 162);
            this.textBoxPopulationFile.Name = "textBoxPopulationFile";
            this.textBoxPopulationFile.ReadOnly = true;
            this.textBoxPopulationFile.Size = new System.Drawing.Size(437, 20);
            this.textBoxPopulationFile.TabIndex = 10;
            this.textBoxPopulationFile.TextChanged += new System.EventHandler(this.textBoxPopulationFile_TextChanged);
            // 
            // labelNumbIterations
            // 
            this.labelNumbIterations.AutoSize = true;
            this.labelNumbIterations.Location = new System.Drawing.Point(17, 194);
            this.labelNumbIterations.Name = "labelNumbIterations";
            this.labelNumbIterations.Size = new System.Drawing.Size(102, 13);
            this.labelNumbIterations.TabIndex = 11;
            this.labelNumbIterations.Text = "Number of Iterations";
            // 
            // labelNumbYears
            // 
            this.labelNumbYears.AutoSize = true;
            this.labelNumbYears.Location = new System.Drawing.Point(124, 195);
            this.labelNumbYears.Name = "labelNumbYears";
            this.labelNumbYears.Size = new System.Drawing.Size(84, 13);
            this.labelNumbYears.TabIndex = 12;
            this.labelNumbYears.Text = "Number of years";
            // 
            // textBoxNumbIterations
            // 
            this.textBoxNumbIterations.Location = new System.Drawing.Point(17, 211);
            this.textBoxNumbIterations.Name = "textBoxNumbIterations";
            this.textBoxNumbIterations.ReadOnly = true;
            this.textBoxNumbIterations.Size = new System.Drawing.Size(101, 20);
            this.textBoxNumbIterations.TabIndex = 13;
            this.textBoxNumbIterations.TextChanged += new System.EventHandler(this.textBoxNumbIterations_TextChanged);
            // 
            // textBoxNumbYears
            // 
            this.textBoxNumbYears.Location = new System.Drawing.Point(124, 211);
            this.textBoxNumbYears.Name = "textBoxNumbYears";
            this.textBoxNumbYears.ReadOnly = true;
            this.textBoxNumbYears.Size = new System.Drawing.Size(77, 20);
            this.textBoxNumbYears.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(240, 295);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 15;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 325);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxNumbYears);
            this.Controls.Add(this.textBoxNumbIterations);
            this.Controls.Add(this.labelNumbYears);
            this.Controls.Add(this.labelNumbIterations);
            this.Controls.Add(this.textBoxPopulationFile);
            this.Controls.Add(this.labelPopulationFile);
            this.Controls.Add(this.textBoxLandscapeFile);
            this.Controls.Add(this.labelLandscapeFile);
            this.Controls.Add(this.labelOutputFolder);
            this.Controls.Add(this.textBoxOutputFolder);
            this.Controls.Add(this.labelParameterSet);
            this.Controls.Add(this.textBoxParameterSet);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btn_LoadSettingsFile);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "Arctic fox rabies model";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_LoadSettingsFile;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox textBoxParameterSet;
        private System.Windows.Forms.Label labelParameterSet;
        private System.Windows.Forms.TextBox textBoxOutputFolder;
        private System.Windows.Forms.Label labelOutputFolder;
        private System.Windows.Forms.Label labelLandscapeFile;
        private System.Windows.Forms.TextBox textBoxLandscapeFile;
        private System.Windows.Forms.Label labelPopulationFile;
        private System.Windows.Forms.TextBox textBoxPopulationFile;
        private System.Windows.Forms.Label labelNumbIterations;
        private System.Windows.Forms.Label labelNumbYears;
        private System.Windows.Forms.TextBox textBoxNumbIterations;
        private System.Windows.Forms.TextBox textBoxNumbYears;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}

