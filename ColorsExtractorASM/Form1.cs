using System.Runtime.InteropServices;

namespace ColorsExtractorASM
{
    public partial class Form1 : Form
    {

        [DllImport(@"C:\Users\grzyw\source\repos\ColorsExtractorASM\x64\Debug\JAAsm.dll")]
        static extern int MyProc1(int a, int b);


        private readonly int[] sliderValues = { 1, 2, 4, 8, 16, 32, 64 };

        public Form1()
        {
            InitializeComponent();



            trackBar1.Minimum = 0; // First index
            trackBar1.Maximum = sliderValues.Length - 1; // Last index
            trackBar1.TickFrequency = 1;
            trackBar1.SmallChange = 1;
            trackBar1.LargeChange = 1;

            string[] items = { "Dark / Bright", "Warm / Cold", "Red / Green / Blue" };
            photo_infos_chooser.Items.AddRange(items);

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
            int currentValue = sliderValues[trackBar1.Value]; int x = 5, y = 3;
            int retVal = MyProc1(x, y);

            threads_number.Text = $"Value: {currentValue}, Value from asm: {retVal}";
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void library_groupBox_Enter(object sender, EventArgs e)
        {

        }

        private void photo_infos_chooser_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
