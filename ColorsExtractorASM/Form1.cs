using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;
using System.Windows.Forms;

namespace ColorsExtractorASM
{
    public partial class Form1 : Form
    {
        private readonly int[] sliderValues = { 1, 2, 4, 8, 16, 32, 64 };
        private Bitmap selectedImage;
        private ColorAnalyzer colorAnalyzer;
        private Label resultLabel;
        private ComboBox colorInfos;

        public Form1()
        {
            InitializeComponent();

            // Konfiguracja TrackBar
            trackBar1.Minimum = 0;
            trackBar1.Maximum = sliderValues.Length - 1;
            trackBar1.TickFrequency = 1;
            trackBar1.SmallChange = 1;
            trackBar1.LargeChange = 1;

            // Dodanie ComboBox do wyświetlania informacji o kolorach
            colorInfos = new ComboBox
            {
                Items = { "Dark / Bright", "Warm / Cold", "Red / Green / Blue" },
                Location = new Point(12, 47),
                Size = new Size(200, 23)
            };
            Controls.Add(colorInfos);

            // Dodanie obsługi zdarzeń
            trackBar1.Scroll += TrackBar1_Scroll;
            UpdateLabelWithValue();

            // Inicjalizacja ColorAnalyzer
            colorAnalyzer = new ColorAnalyzer();

            // Dodaj przycisk do analizy
            Button btnAnalyze = new Button
            {
                Text = "Analyze Colors",
                Location = new Point(12, 75),
                Size = new Size(100, 30)
            };
            btnAnalyze.Click += BtnAnalyze_Click;
            Controls.Add(btnAnalyze);

            // Dodaj etykietę do wyświetlania wyniku
            resultLabel = new Label
            {
                Text = "",
                Location = new Point(12, 110),
                Size = new Size(400, 23)
            };
            Controls.Add(resultLabel);
        }

        private void BtnAnalyze_Click(object sender, EventArgs e)
        {
            if (selectedImage == null)
            {
                MessageBox.Show("Proszę najpierw wczytać zdjęcie!");
                return;
            }

            // Pobierz aktualną liczbę wątków z TrackBar
            int threadCount = sliderValues[trackBar1.Value];

            // Przeprowadź analizę kolorów
            var result = colorAnalyzer.AnalyzeImage(selectedImage, threadCount);

            // Wyświetl wynik
            resultLabel.Text = $"Color Temperature: {result.Temperature}, " +
                              $"Processing Time: {result.ProcessingTime.TotalMilliseconds:F2} ms, " +
                              $"Threads: {threadCount}";

            // Zaktualizuj informacje o zdjęciu
            colorInfos.SelectedIndex = (int)result.Temperature;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Wczytywanie obrazu
            openFileDialog1.ShowDialog();
            string filePath = openFileDialog1.FileName;
            selectedImage = (Bitmap)Image.FromFile(filePath);
            pictureBox1.Image = selectedImage;
            pictureBox1.BorderStyle = BorderStyle.None;
        }

        private void TrackBar1_Scroll(object sender, EventArgs e)
        {
            UpdateLabelWithValue();
        }

        private void UpdateLabelWithValue()
        {
            int currentValue = sliderValues[trackBar1.Value];
            int x = 5, y = 3;
            // Wywołanie oryginalnej metody z DLL
            int retVal = MyProc1(x, y);
            threads_number.Text = $"Value: {currentValue}, Value from asm: {retVal}";
        }

        [DllImport(@"C:\Users\grzyw\source\repos\ColorsExtractorASM\x64\Debug\JAAsm.dll")]
        static extern int MyProc1(int a, int b);
    }

    public class ColorAnalyzer
    {
        public enum ColorTemperature
        {
            Cold,
            Neutral,
            Warm
        }

        public class AnalysisResult
        {
            public ColorTemperature Temperature { get; set; }
            public TimeSpan ProcessingTime { get; set; }
        }

        public AnalysisResult AnalyzeImage(Bitmap image, int threadCount)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Konwersja bitmapy do tablicy pikseli
            BitmapData bmpData = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb
            );

            byte[] pixelData = new byte[Math.Abs(bmpData.Stride) * image.Height];
            Marshal.Copy(bmpData.Scan0, pixelData, 0, pixelData.Length);
            image.UnlockBits(bmpData);

            // Podział obrazu na regiony
            var regions = SplitImageIntoRegions(pixelData, image.Width, image.Height, threadCount);

            // Analiza temperatury kolorów równolegle
            var temperatures = new ConcurrentBag<ColorTemperature>();
            Parallel.ForEach(regions, new ParallelOptions { MaxDegreeOfParallelism = threadCount }, region =>
            {
                var regionTemperature = AnalyzeRegionTemperature(region);
                temperatures.Add(regionTemperature);
            });

            // Agregacja wyniku
            var finalTemperature = AggregateSingleColorTemperature(temperatures);

            stopwatch.Stop();

            return new AnalysisResult
            {
                Temperature = finalTemperature,
                ProcessingTime = stopwatch.Elapsed
            };
        }

        private byte[][] SplitImageIntoRegions(byte[] pixelData, int width, int height, int threadCount)
        {
            var regions = new byte[threadCount][];
            int regionSize = pixelData.Length / threadCount;

            for (int i = 0; i < threadCount; i++)
            {
                int start = i * regionSize;
                int length = (i == threadCount - 1) ?
                    (pixelData.Length - start) :
                    regionSize;

                regions[i] = new byte[length];
                Array.Copy(pixelData, start, regions[i], 0, length);
            }

            return regions;
        }

        private ColorTemperature AnalyzeRegionTemperature(byte[] regionPixels)
        {
            int totalR = 0, totalG = 0, totalB = 0;
            for (int i = 0; i < regionPixels.Length; i += 4)
            {
                totalB += regionPixels[i];     // Blue
                totalG += regionPixels[i + 1]; // Green
                totalR += regionPixels[i + 2]; // Red
            }

            int pixelCount = regionPixels.Length / 4;
            double avgR = totalR / (double)pixelCount;
            double avgG = totalG / (double)pixelCount;
            double avgB = totalB / (double)pixelCount;

            // Prosta heurystyka temperatury kolorów
            double warmthScore = (avgR + avgG) / 2 - avgB;

            if (warmthScore > 50) return ColorTemperature.Warm;
            if (warmthScore < -50) return ColorTemperature.Cold;
            return ColorTemperature.Neutral;
        }

        private ColorTemperature AggregateSingleColorTemperature(ConcurrentBag<ColorTemperature> temperatures)
        {
            var temperatureGroups = temperatures
                .GroupBy(t => t)
                .OrderByDescending(g => g.Count())
                .ToList();

            return temperatureGroups.First().Key;
        }
    }
}