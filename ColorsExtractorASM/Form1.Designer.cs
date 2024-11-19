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
            library_groupBox = new GroupBox();
            x64_button = new RadioButton();
            asm_button = new RadioButton();
            label1 = new Label();
            photo_infos_chooser = new ComboBox();
            info_chooser_label = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            library_groupBox.SuspendLayout();
            SuspendLayout();
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // image_loader_button
            // 
            image_loader_button.Location = new Point(350, 93);
            image_loader_button.Name = "image_loader_button";
            image_loader_button.Size = new Size(106, 31);
            image_loader_button.TabIndex = 0;
            image_loader_button.Text = "Load image";
            image_loader_button.UseVisualStyleBackColor = true;
            image_loader_button.Click += button1_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Location = new Point(257, 130);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(278, 262);
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
            // library_groupBox
            // 
            library_groupBox.Controls.Add(x64_button);
            library_groupBox.Controls.Add(asm_button);
            library_groupBox.Location = new Point(29, 178);
            library_groupBox.Name = "library_groupBox";
            library_groupBox.Size = new Size(157, 71);
            library_groupBox.TabIndex = 5;
            library_groupBox.TabStop = false;
            library_groupBox.Text = "Choose the library";
            library_groupBox.Enter += library_groupBox_Enter;
            // 
            // x64_button
            // 
            x64_button.AutoSize = true;
            x64_button.Location = new Point(6, 47);
            x64_button.Name = "x64_button";
            x64_button.Size = new Size(42, 19);
            x64_button.TabIndex = 1;
            x64_button.TabStop = true;
            x64_button.Text = "x64";
            x64_button.UseVisualStyleBackColor = true;
            // 
            // asm_button
            // 
            asm_button.AutoSize = true;
            asm_button.Location = new Point(6, 22);
            asm_button.Name = "asm_button";
            asm_button.Size = new Size(49, 19);
            asm_button.TabIndex = 0;
            asm_button.TabStop = true;
            asm_button.Text = "Asm";
            asm_button.UseVisualStyleBackColor = true;
            asm_button.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Stencil", 30F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label1.ForeColor = Color.Purple;
            label1.Location = new Point(202, 9);
            label1.Name = "label1";
            label1.Size = new Size(398, 47);
            label1.TabIndex = 6;
            label1.Text = "Colors Extractor";
            // 
            // photo_infos_chooser
            // 
            photo_infos_chooser.FormattingEnabled = true;
            photo_infos_chooser.Location = new Point(29, 301);
            photo_infos_chooser.Name = "photo_infos_chooser";
            photo_infos_chooser.Size = new Size(157, 23);
            photo_infos_chooser.TabIndex = 7;
            photo_infos_chooser.SelectedIndexChanged += photo_infos_chooser_SelectedIndexChanged;
            // 
            // info_chooser_label
            // 
            info_chooser_label.AutoSize = true;
            info_chooser_label.Location = new Point(29, 268);
            info_chooser_label.Name = "info_chooser_label";
            info_chooser_label.Size = new Size(153, 30);
            info_chooser_label.TabIndex = 8;
            info_chooser_label.Text = "What do you want to know \nabout this photo?";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(info_chooser_label);
            Controls.Add(photo_infos_chooser);
            Controls.Add(label1);
            Controls.Add(library_groupBox);
            Controls.Add(threads_number);
            Controls.Add(threads_label);
            Controls.Add(trackBar1);
            Controls.Add(pictureBox1);
            Controls.Add(image_loader_button);
            Name = "Form1";
            Text = "Colors Extractor ASM";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            library_groupBox.ResumeLayout(false);
            library_groupBox.PerformLayout();
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
        private GroupBox library_groupBox;
        private RadioButton asm_button;
        private RadioButton x64_button;
        private Label label1;
        private ComboBox photo_infos_chooser;
        private Label info_chooser_label;
    }
}
