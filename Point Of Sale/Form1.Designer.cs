namespace EMVExampleDotNet
{
    partial class EMVExample
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.BEMVRun = new System.Windows.Forms.Button();
            this.BClear = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.Open_USB_button = new System.Windows.Forms.Button();
            this.USB_Readers_comboBox = new System.Windows.Forms.ComboBox();
            this.Readers_comboBox = new System.Windows.Forms.ComboBox();
            this.Com_Port_comboBox = new System.Windows.Forms.ComboBox();
            this.button_CloseReader = new System.Windows.Forms.Button();
            this.button_Open_Reader = new System.Windows.Forms.Button();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Location = new System.Drawing.Point(0, 0);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(538, 286);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // BEMVRun
            // 
            this.BEMVRun.Location = new System.Drawing.Point(361, 0);
            this.BEMVRun.Name = "BEMVRun";
            this.BEMVRun.Size = new System.Drawing.Size(91, 69);
            this.BEMVRun.TabIndex = 1;
            this.BEMVRun.Text = "Run";
            this.BEMVRun.UseVisualStyleBackColor = true;
            this.BEMVRun.Click += new System.EventHandler(this.BEMVRun_Click);
            // 
            // BClear
            // 
            this.BClear.Location = new System.Drawing.Point(455, 0);
            this.BClear.Name = "BClear";
            this.BClear.Size = new System.Drawing.Size(80, 69);
            this.BClear.TabIndex = 3;
            this.BClear.Text = "Clear";
            this.BClear.UseVisualStyleBackColor = true;
            this.BClear.Click += new System.EventHandler(this.BClear_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.Open_USB_button);
            this.splitContainer1.Panel2.Controls.Add(this.USB_Readers_comboBox);
            this.splitContainer1.Panel2.Controls.Add(this.Readers_comboBox);
            this.splitContainer1.Panel2.Controls.Add(this.Com_Port_comboBox);
            this.splitContainer1.Panel2.Controls.Add(this.button_CloseReader);
            this.splitContainer1.Panel2.Controls.Add(this.button_Open_Reader);
            this.splitContainer1.Panel2.Controls.Add(this.BEMVRun);
            this.splitContainer1.Panel2.Controls.Add(this.BClear);
            this.splitContainer1.Size = new System.Drawing.Size(538, 365);
            this.splitContainer1.SplitterDistance = 286;
            this.splitContainer1.TabIndex = 4;
            // 
            // Open_USB_button
            // 
            this.Open_USB_button.Location = new System.Drawing.Point(177, 43);
            this.Open_USB_button.Name = "Open_USB_button";
            this.Open_USB_button.Size = new System.Drawing.Size(114, 27);
            this.Open_USB_button.TabIndex = 9;
            this.Open_USB_button.Text = "Open USB Reader";
            this.Open_USB_button.UseVisualStyleBackColor = true;
            this.Open_USB_button.Click += new System.EventHandler(this.Open_USB_button_Click);
            // 
            // USB_Readers_comboBox
            // 
            this.USB_Readers_comboBox.FormattingEnabled = true;
            this.USB_Readers_comboBox.Location = new System.Drawing.Point(-1, 47);
            this.USB_Readers_comboBox.Name = "USB_Readers_comboBox";
            this.USB_Readers_comboBox.Size = new System.Drawing.Size(174, 21);
            this.USB_Readers_comboBox.TabIndex = 8;
            this.USB_Readers_comboBox.SelectedIndexChanged += new System.EventHandler(this.USB_Readers_comboBox_SelectedIndexChanged);
            // 
            // Readers_comboBox
            // 
            this.Readers_comboBox.FormattingEnabled = true;
            this.Readers_comboBox.Items.AddRange(new object[] {
            "Sankyo ICT3K5"});
            this.Readers_comboBox.Location = new System.Drawing.Point(0, 22);
            this.Readers_comboBox.Name = "Readers_comboBox";
            this.Readers_comboBox.Size = new System.Drawing.Size(174, 21);
            this.Readers_comboBox.TabIndex = 7;
            this.Readers_comboBox.SelectedIndexChanged += new System.EventHandler(this.Readers_comboBox_SelectedIndexChanged);
            // 
            // Com_Port_comboBox
            // 
            this.Com_Port_comboBox.FormattingEnabled = true;
            this.Com_Port_comboBox.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9",
            "COM10",
            "COM11",
            "COM12"});
            this.Com_Port_comboBox.Location = new System.Drawing.Point(3, -2);
            this.Com_Port_comboBox.Name = "Com_Port_comboBox";
            this.Com_Port_comboBox.Size = new System.Drawing.Size(171, 21);
            this.Com_Port_comboBox.TabIndex = 6;
            this.Com_Port_comboBox.SelectedIndexChanged += new System.EventHandler(this.Com_Port_comboBox_SelectedIndexChanged);
            // 
            // button_CloseReader
            // 
            this.button_CloseReader.Location = new System.Drawing.Point(292, 0);
            this.button_CloseReader.Name = "button_CloseReader";
            this.button_CloseReader.Size = new System.Drawing.Size(66, 69);
            this.button_CloseReader.TabIndex = 5;
            this.button_CloseReader.Text = "Close Reader";
            this.button_CloseReader.UseVisualStyleBackColor = true;
            this.button_CloseReader.Click += new System.EventHandler(this.button_CloseReader_Click);
            // 
            // button_Open_Reader
            // 
            this.button_Open_Reader.Location = new System.Drawing.Point(177, 0);
            this.button_Open_Reader.Name = "button_Open_Reader";
            this.button_Open_Reader.Size = new System.Drawing.Size(114, 44);
            this.button_Open_Reader.TabIndex = 4;
            this.button_Open_Reader.Text = "Open COM Reader";
            this.button_Open_Reader.UseVisualStyleBackColor = true;
            this.button_Open_Reader.Click += new System.EventHandler(this.button_Open_Reader_Click);
            // 
            // EMVExample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 365);
            this.Controls.Add(this.splitContainer1);
            this.Name = "EMVExample";
            this.Text = "EMVExample";
            this.Load += new System.EventHandler(this.EMVExample_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button BEMVRun;
        private System.Windows.Forms.Button BClear;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button button_Open_Reader;
        private System.Windows.Forms.Button button_CloseReader;
        private System.Windows.Forms.ComboBox Com_Port_comboBox;
        private System.Windows.Forms.ComboBox Readers_comboBox;
        private System.Windows.Forms.ComboBox USB_Readers_comboBox;
        private System.Windows.Forms.Button Open_USB_button;
    }
}

