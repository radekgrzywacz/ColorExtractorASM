using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ColorsExtractorASM
{
    public partial class Form1 : Form
    {

        //private readonly int[] threadNumbers = { 1, 2, 4, 8, 16, 32, 64 };
        private Bitmap selectedImage;
        private Analyzer analyzer;

        public Form1()
        {
            InitializeComponent();

            // Configure TrackBar
            trackBar1.Minimum = 1;
            trackBar1.Maximum = 64;
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
            int currentValue = trackBar1.Value;

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
            int threadCount = trackBar1.Value;

            if (!asm_button.Checked && !cSharp_button.Checked)
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

            analyzer.asmChecked = asm_button.Checked ? true : false;

            try
            {
                //result = colorAnalyzer.AnalyzeImage(selectedImage, threadCount,
                //    (ColorAnalyzer.AnalysisType)action);
                result = analyzer.AnalyzeImageSimple(selectedImage, threadCount, action);
                // MessageBox.Show($"Analyzing in cSharp");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error during analysis: {ex.Message}");
                return;
            }


            // Display the result
            ms_counter_run_simp.Text = $"Result: {result.Result}\n" +
                                          $"Processing Time: {result.ProcessingTime.TotalMilliseconds:F2} ms\n" +
                                          $"Threads: {threadCount}";

        }

        private void photo_infos_chooser_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void run_with_everything_button_Click(object sender, EventArgs e)
        {
            if (photo_infos_chooser.SelectedItem == null)
            {
                MessageBox.Show("Please select an analysis type!");
                return;
            }

            string analysisType = photo_infos_chooser.SelectedItem.ToString();
            int[] threadCounts = { 1, 2, 4, 8, 16, 32, 64 };

            // Paths for the three photos
            string projectDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\"));

            // Paths for the three photos (relative to the project directory)
            string[] photoPaths = {
                Path.Combine(projectDirectory, @"zielony.jpg"),
                Path.Combine(projectDirectory, @"zdjecie_medium_jakosc.jpg"),
                Path.Combine(projectDirectory, @"zdjecie_top_jakosc.jpg")
            };

            // List to hold the results
            List<(string PhotoName, AnalysisResult cSharpResult, AnalysisResult asmResult)> consolidatedResults = new();

            foreach (string path in photoPaths)
            {
                if (!File.Exists(path))
                {
                    MessageBox.Show($"Photo at {path} not found!");
                    continue;
                }

                Bitmap photo = new Bitmap(path);

                // Perform analyses
                AnalysisResult cSharpResult = analyzer.AnalyzeImageWithTests(photo, threadCounts, analysisType, false);
                AnalysisResult asmResult = analyzer.AnalyzeImageWithTests(photo, threadCounts, analysisType, true);

                // Add to the consolidated results
                consolidatedResults.Add((Path.GetFileName(path), cSharpResult, asmResult));
            }

            // Display the consolidated results in one window
            CreateResultWindow("Consolidated Analysis Results", consolidatedResults);
        }


        private void CreateResultWindow(string title, List<(string PhotoName, AnalysisResult cSharpResult, AnalysisResult asmResult)> results)
        {
            // Create a new form to display the results
            Form resultWindow = new Form
            {
                Text = title,
                Size = new Size(800, 600)
            };

            // Add a RichTextBox for displaying the results
            RichTextBox rtb = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true
            };

            // Build the text content for the result window
            StringBuilder resultText = new StringBuilder();

            resultText.AppendLine("Consolidated Analysis Results");
            resultText.AppendLine(new string('-', 50));

            foreach (var resultEntry in results)
            {
                resultText.AppendLine($"Photo: {resultEntry.PhotoName}");
                resultText.AppendLine("C# Analysis Results:");
                resultText.AppendLine(resultEntry.cSharpResult.Result);
                //resultText.AppendLine($"Total Processing Time: {resultEntry.cSharpResult.ProcessingTime.TotalMilliseconds} ms");
                resultText.AppendLine();

                resultText.AppendLine("ASM Analysis Results:");
                resultText.AppendLine(resultEntry.asmResult.Result);
                // resultText.AppendLine($"Total Processing Time: {resultEntry.asmResult.ProcessingTime.TotalMilliseconds} ms");
                resultText.AppendLine(new string('-', 50));
            }

            // Set the content of the RichTextBox
            rtb.Text = resultText.ToString();

            // Add the RichTextBox to the form
            resultWindow.Controls.Add(rtb);

            // Show the result window
            resultWindow.Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void run_simple_label_Click(object sender, EventArgs e)
        {

        }

        private void library_groupBox_Enter(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll_1(object sender, EventArgs e)
        {

        }
    }
}