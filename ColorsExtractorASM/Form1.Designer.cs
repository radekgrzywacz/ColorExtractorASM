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
            cSharp_button = new RadioButton();
            asm_button = new RadioButton();
            title = new Label();
            photo_infos_chooser = new ComboBox();
            info_chooser_label = new Label();
            run_simple = new Button();
            ms_counter_run_simp = new Label();
            run_with_everything_button = new Button();
            run_tests_label = new Label();
            run_simple_label = new Label();
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
            image_loader_button.Location = new Point(400, 124);
            image_loader_button.Margin = new Padding(3, 4, 3, 4);
            image_loader_button.Name = "image_loader_button";
            image_loader_button.Size = new Size(121, 41);
            image_loader_button.TabIndex = 0;
            image_loader_button.Text = "Load image";
            image_loader_button.UseVisualStyleBackColor = true;
            image_loader_button.Click += button1_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Location = new Point(294, 173);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(317, 349);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(25, 142);
            trackBar1.Margin = new Padding(3, 4, 3, 4);
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(187, 56);
            trackBar1.TabIndex = 2;
            trackBar1.Scroll += trackBar1_Scroll_1;
            // 
            // threads_label
            // 
            threads_label.AutoSize = true;
            threads_label.Location = new Point(47, 129);
            threads_label.Name = "threads_label";
            threads_label.Size = new Size(134, 20);
            threads_label.TabIndex = 3;
            threads_label.Text = "Number of threads";
            // 
            // threads_number
            // 
            threads_number.AutoSize = true;
            threads_number.Location = new Point(117, 192);
            threads_number.Name = "threads_number";
            threads_number.Size = new Size(17, 20);
            threads_number.TabIndex = 4;
            threads_number.Text = "1";
            // 
            // library_groupBox
            // 
            library_groupBox.Controls.Add(cSharp_button);
            library_groupBox.Controls.Add(asm_button);
            library_groupBox.Location = new Point(33, 237);
            library_groupBox.Margin = new Padding(3, 4, 3, 4);
            library_groupBox.Name = "library_groupBox";
            library_groupBox.Padding = new Padding(3, 4, 3, 4);
            library_groupBox.Size = new Size(179, 95);
            library_groupBox.TabIndex = 5;
            library_groupBox.TabStop = false;
            library_groupBox.Text = "Choose the library";
            library_groupBox.Enter += library_groupBox_Enter;
            // 
            // cSharp_button
            // 
            cSharp_button.AutoSize = true;
            cSharp_button.Location = new Point(7, 63);
            cSharp_button.Margin = new Padding(3, 4, 3, 4);
            cSharp_button.Name = "cSharp_button";
            cSharp_button.Size = new Size(48, 24);
            cSharp_button.TabIndex = 1;
            cSharp_button.TabStop = true;
            cSharp_button.Text = "C#";
            cSharp_button.UseVisualStyleBackColor = true;
            // 
            // asm_button
            // 
            asm_button.AutoSize = true;
            asm_button.Location = new Point(7, 29);
            asm_button.Margin = new Padding(3, 4, 3, 4);
            asm_button.Name = "asm_button";
            asm_button.Size = new Size(59, 24);
            asm_button.TabIndex = 0;
            asm_button.TabStop = true;
            asm_button.Text = "Asm";
            asm_button.UseVisualStyleBackColor = true;
            // 
            // title
            // 
            title.AutoSize = true;
            title.Font = new Font("Microsoft Sans Serif", 30F, FontStyle.Italic, GraphicsUnit.Point, 0);
            title.ForeColor = Color.Purple;
            title.Location = new Point(231, 12);
            title.Name = "title";
            title.Size = new Size(384, 58);
            title.TabIndex = 6;
            title.Text = "Colors Extractor";
            // 
            // photo_infos_chooser
            // 
            photo_infos_chooser.FormattingEnabled = true;
            photo_infos_chooser.Location = new Point(33, 401);
            photo_infos_chooser.Margin = new Padding(3, 4, 3, 4);
            photo_infos_chooser.Name = "photo_infos_chooser";
            photo_infos_chooser.Size = new Size(179, 28);
            photo_infos_chooser.TabIndex = 7;
            photo_infos_chooser.SelectedIndexChanged += photo_infos_chooser_SelectedIndexChanged;
            // 
            // info_chooser_label
            // 
            info_chooser_label.AutoSize = true;
            info_chooser_label.Location = new Point(33, 357);
            info_chooser_label.Name = "info_chooser_label";
            info_chooser_label.Size = new Size(191, 40);
            info_chooser_label.TabIndex = 8;
            info_chooser_label.Text = "What do you want to know \nabout this photo?";
            // 
            // run_simple
            // 
            run_simple.Location = new Point(729, 216);
            run_simple.Margin = new Padding(3, 4, 3, 4);
            run_simple.Name = "run_simple";
            run_simple.Size = new Size(99, 33);
            run_simple.TabIndex = 9;
            run_simple.Text = "Run";
            run_simple.UseVisualStyleBackColor = true;
            run_simple.Click += run_simple_Click;
            // 
            // ms_counter_run_simp
            // 
            ms_counter_run_simp.AutoSize = true;
            ms_counter_run_simp.Location = new Point(651, 267);
            ms_counter_run_simp.Name = "ms_counter_run_simp";
            ms_counter_run_simp.Size = new Size(36, 20);
            ms_counter_run_simp.TabIndex = 10;
            ms_counter_run_simp.Text = "0ms";
            // 
            // run_with_everything_button
            // 
            run_with_everything_button.Location = new Point(729, 419);
            run_with_everything_button.Margin = new Padding(3, 4, 3, 4);
            run_with_everything_button.Name = "run_with_everything_button";
            run_with_everything_button.Size = new Size(99, 33);
            run_with_everything_button.TabIndex = 11;
            run_with_everything_button.Text = "Test";
            run_with_everything_button.UseVisualStyleBackColor = true;
            run_with_everything_button.Click += run_with_everything_button_Click;
            // 
            // run_tests_label
            // 
            run_tests_label.AutoSize = true;
            run_tests_label.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            run_tests_label.Location = new Point(687, 361);
            run_tests_label.Name = "run_tests_label";
            run_tests_label.Size = new Size(193, 50);
            run_tests_label.TabIndex = 12;
            run_tests_label.Text = "This button will run\nall the required tests";
            run_tests_label.Click += label1_Click;
            // 
            // run_simple_label
            // 
            run_simple_label.AutoSize = true;
            run_simple_label.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            run_simple_label.Location = new Point(687, 159);
            run_simple_label.Name = "run_simple_label";
            run_simple_label.Size = new Size(184, 50);
            run_simple_label.TabIndex = 13;
            run_simple_label.Text = "This button will run\njust chosen options";
            run_simple_label.Click += run_simple_label_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(run_simple_label);
            Controls.Add(run_tests_label);
            Controls.Add(run_with_everything_button);
            Controls.Add(ms_counter_run_simp);
            Controls.Add(run_simple);
            Controls.Add(info_chooser_label);
            Controls.Add(photo_infos_chooser);
            Controls.Add(title);
            Controls.Add(library_groupBox);
            Controls.Add(threads_number);
            Controls.Add(threads_label);
            Controls.Add(trackBar1);
            Controls.Add(pictureBox1);
            Controls.Add(image_loader_button);
            Margin = new Padding(3, 4, 3, 4);
            Name = "Form1";
            Text = "Colors Extractor ASM";
            Load += Form1_Load;
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
        private RadioButton cSharp_button;
        private Label title;
        private ComboBox photo_infos_chooser;
        private Label info_chooser_label;
        private Button run_simple;
        private Label ms_counter_run_simp;
        private Button run_with_everything_button;
        private Label run_tests_label;
        private Label run_simple_label;
    }
}
