namespace ColorsExtractorASM
{
    public partial class Form1 : Form
    {

        private readonly int[] sliderValues = { 1, 2, 4, 8, 16, 32, 64 };

        public Form1()
        {
            InitializeComponent();

            trackBar1.Minimum = 0; // First index
            trackBar1.Maximum = sliderValues.Length - 1; // Last index
            trackBar1.TickFrequency = 1;
            trackBar1.SmallChange = 1;
            trackBar1.LargeChange = 1;

            // Attach event handler
            trackBar1.Scroll += TrackBar1_Scroll;

            // Set initial value
            UpdateLabelWithValue();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            string filePath = openFileDialog1.FileName;
            pictureBox1.Image = Image.FromFile(filePath);
            pictureBox1.BorderStyle = BorderStyle.None;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            UpdateLabelWithValue();
        }

        private void UpdateLabelWithValue()
        {
            // Get the current value based on the trackBar1.Value index
            int currentValue = sliderValues[trackBar1.Value];
            threads_number.Text = $"Value: {currentValue}";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
