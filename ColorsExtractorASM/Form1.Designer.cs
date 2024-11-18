namespace ColorsExtractorASM
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            openFileDialog1 = new OpenFileDialog();
            image_loader_button = new Button();
            pictureBox1 = new PictureBox();
            trackBar1 = new TrackBar();
            threads_label = new Label();
            threads_number = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            SuspendLayout();
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // image_loader_button
            // 
            image_loader_button.Location = new Point(348, 15);
            image_loader_button.Name = "image_loader_button";
            image_loader_button.Size = new Size(118, 51);
            image_loader_button.TabIndex = 0;
            image_loader_button.Text = "Load image";
            image_loader_button.UseVisualStyleBackColor = true;
            image_loader_button.Click += button1_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Location = new Point(244, 72);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(326, 307);
            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(29, 114);
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(164, 45);
            trackBar1.TabIndex = 2;
            // 
            // threads_label
            // 
            threads_label.AutoSize = true;
            threads_label.Location = new Point(41, 97);
            threads_label.Name = "threads_label";
            threads_label.Size = new Size(107, 15);
            threads_label.TabIndex = 3;
            threads_label.Text = "Number of threads";
            threads_label.Click += label1_Click;
            // 
            // threads_number
            // 
            threads_number.AutoSize = true;
            threads_number.Location = new Point(102, 144);
            threads_number.Name = "threads_number";
            threads_number.Size = new Size(13, 15);
            threads_number.TabIndex = 4;
            threads_number.Text = "1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(threads_number);
            Controls.Add(threads_label);
            Controls.Add(trackBar1);
            Controls.Add(pictureBox1);
            Controls.Add(image_loader_button);
            Name = "Form1";
            Text = "Colors Extractor ASM";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private OpenFileDialog openFileDialog1;
        private Button image_loader_button;
        private PictureBox pictureBox1;
        private TrackBar trackBar1;
        private Label threads_label;
        private Label threads_number;
    }
}
