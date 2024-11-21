using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;
using System.Linq;

namespace ColorsExtractorASM
{
    public class ColorAnalyzer
    {
        public enum AnalysisType
        {
            Temperature,
            Brightness,
            DominantColor
        }

        public class AnalysisResult
        {
            public string Result { get; set; }
            public TimeSpan ProcessingTime { get; set; }
        }

        public AnalysisResult AnalyzeImage(Bitmap image, int threadCount, AnalysisType analysisType)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Convert bitmap to pixel data
            byte[] pixelData = ConvertBitmapToPixelArray(image);

            // Divide image into regions
            var regions = SplitImageIntoRegions(pixelData, image.Width, image.Height, threadCount);

            string result;

            // Perform the selected analysis
            switch (analysisType)
            {
                case AnalysisType.Temperature:
                    result = WarmnessAnalysis(regions, threadCount).ToString();
                    break;
                case AnalysisType.Brightness:
                    result = BrightnessAnalysis(regions, threadCount).ToString("0.00");
                    break;
                case AnalysisType.DominantColor:
                    result = DominantColorAnalysis(regions, threadCount).ToString();
                    break;
                default:
                    throw new ArgumentException("Unsupported analysis type.");
            }

            stopwatch.Stop();

            return new AnalysisResult
            {
                Result = result,
                ProcessingTime = stopwatch.Elapsed
            };
        }

        private byte[] ConvertBitmapToPixelArray(Bitmap image)
        {
            BitmapData bmpData = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb
            );

            byte[] pixelData = new byte[Math.Abs(bmpData.Stride) * image.Height];
            Marshal.Copy(bmpData.Scan0, pixelData, 0, pixelData.Length);
            image.UnlockBits(bmpData);

            return pixelData;
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

        // Temperature Analysis
        private ColorTemperature WarmnessAnalysis(byte[][] regions, int threadCount)
        {
            var temperatures = new ConcurrentBag<ColorTemperature>();
            Parallel.ForEach(regions, new ParallelOptions { MaxDegreeOfParallelism = threadCount }, region =>
            {
                var regionTemperature = AnalyzeRegionTemperature(region);
                temperatures.Add(regionTemperature);
            });

            return AggregateSingleColorTemperature(temperatures);
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

        // Brightness Analysis
        private double BrightnessAnalysis(byte[][] regions, int threadCount)
        {
            double totalBrightness = 0;
            int totalPixels = 0;

            Parallel.ForEach(regions, new ParallelOptions { MaxDegreeOfParallelism = threadCount }, region =>
            {
                double regionBrightness = 0;
                int regionPixels = region.Length / 4;

                for (int i = 0; i < region.Length; i += 4)
                {
                    int r = region[i + 2];
                    int g = region[i + 1];
                    int b = region[i];

                    // Compute brightness (perceptual luminance)
                    regionBrightness += 0.2126 * r + 0.7152 * g + 0.0722 * b;
                }

                lock (this)
                {
                    totalBrightness += regionBrightness;
                    totalPixels += regionPixels;
                }
            });

            return totalBrightness / totalPixels;
        }

        // Dominant Color Analysis
        private Color DominantColorAnalysis(byte[][] regions, int threadCount)
        {
            var colorCounts = new ConcurrentDictionary<(int R, int G, int B), int>();

            Parallel.ForEach(regions, new ParallelOptions { MaxDegreeOfParallelism = threadCount }, region =>
            {
                for (int i = 0; i < region.Length; i += 4)
                {
                    int b = region[i];
                    int g = region[i + 1];
                    int r = region[i + 2];

                    var colorKey = (r, g, b);

                    colorCounts.AddOrUpdate(colorKey, 1, (key, count) => count + 1);
                }
            });

            var dominantColor = colorCounts.OrderByDescending(c => c.Value).First().Key;
            return Color.FromArgb(dominantColor.R, dominantColor.G, dominantColor.B);
        }

        public enum ColorTemperature
        {
            Cold,
            Neutral,
            Warm
        }
    }
}
