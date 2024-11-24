using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ColorsExtractorASM
{
    public partial class Form1 : Form
    {

        private readonly int[] sliderValues = { 1, 2, 4, 8, 16, 32, 64 };
        private Bitmap selectedImage;
        private Analyzer analyzer;

        public Form1()
        {
            InitializeComponent();

            // Configure TrackBar
            trackBar1.Minimum = 0;
            trackBar1.Maximum = sliderValues.Length - 1;
            trackBar1.TickFrequency = 1;
            trackBar1.SmallChange = 1;
            trackBar1.LargeChange = 1;

            // Populate dropdown with analysis options
            string[] items = { "Warm / Cold", "Dark / Bright", "Red / Green / Blue" };
            photo_infos_chooser.Items.AddRange(items);

            // Add event handlers
            trackBar1.Scroll += TrackBar1_Scroll;
            UpdateLabelWithValue();

            // Initialize ColorAnalyzer
            analyzer = new Analyzer();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Load image
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                selectedImage = (Bitmap)Image.FromFile(filePath);
                pictureBox1.Image = selectedImage;
                pictureBox1.BorderStyle = BorderStyle.None;
            }
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            UpdateLabelWithValue();
        }

        private void UpdateLabelWithValue()
        {
            int currentValue = sliderValues[trackBar1.Value];
            int x = 5, y = 3;

            // Call the external ASM method
            // int retVal = MyProc1(x, y);

            threads_number.Text = $"Threads: {currentValue}";
        }

        private void run_simple_Click(object sender, EventArgs e)
        {
            if (selectedImage == null)
            {
                MessageBox.Show("Please select an image!");
                return;
            }

            // Get thread count
            int threadCount = sliderValues[trackBar1.Value];

            if (!asm_button.Checked && !x64_button.Checked)
            {
                MessageBox.Show("Please select a library!");
                return;
            }

            string action = photo_infos_chooser.Text;
            if (action.Equals("") || action == null)
            {
                MessageBox.Show("Please select an action!");
                return;
            }

            AnalysisResult? result = null;

            if (x64_button.Checked)
            {
                try
                {
                    //result = colorAnalyzer.AnalyzeImage(selectedImage, threadCount,
                    //    (ColorAnalyzer.AnalysisType)action);
                    result = analyzer.AnalyzeImage(selectedImage, threadCount, action);
                    // MessageBox.Show($"Analyzing in x64");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during analysis: {ex.Message}");
                    return;
                }
            }
            else if (asm_button.Checked)
            {
                try
                {
                    // colorAnalyzer.setUseASM();
                    //result = colorAnalyzer.AnalyzeImage(selectedImage, threadCount,
                    //    (ColorAnalyzer.AnalysisType)action);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during analysis: {ex.Message}");
                    return;
                }
            }
            // Display the result
            ms_counter_run_simp.Text = $"Result: {result.Result}\n" +
                                          $"Processing Time: {result.ProcessingTime.TotalMilliseconds:F2} ms\n" +
                                          $"Threads: {threadCount}";

        }

        private void photo_infos_chooser_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
