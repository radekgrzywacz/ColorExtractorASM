using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using RegularLib;
using static System.Net.Mime.MediaTypeNames;

namespace ColorsExtractorASM
{
    public unsafe partial class Analyzer
    {
        private const int SEGMENT_HEIGHT = 8;
        private readonly object lockObject = new object();
        private AnalyzerRegular analyzerRegular = new AnalyzerRegular();

        private delegate void AsmProcessDelegate(byte* pixels, int pixelCount, int* sum);
        private delegate void AsmProcessDelegateDominant(byte* pixels, int pixelCount, int* totalRed, int* totalGreen, int* totalBlue);


        [DllImport(@"C:\Users\SQ299\Source\Repos\ColorExtractorASM\x64\Debug\JAAsm.dll")]
        private static extern void CalculateAverageBrightness(byte* pixels, int pixelCount, int* sum);

        [DllImport(@"C:\Users\SQ299\Source\Repos\ColorExtractorASM\x64\Debug\JAAsm.dll")]
        private static extern void CalculateTemperature(byte* pixels, int pixelCount, int* sum);

        [DllImport(@"C:\Users\SQ299\Source\Repos\ColorExtractorASM\x64\Debug\JAAsm.dll")]
        private static extern void CalculateDominantChannel(byte* pixels, int pixelCount, int* redSum, int* greenSum, int* blueSum);


        public bool asmChecked { get; set; } = false;

        private unsafe string AnalyzeBrightness(Bitmap bitmap, int threadCount)
        {
            double totalBrightness = asmChecked
                ? ProcessImageSegments(bitmap, threadCount, CalculateAverageBrightness, "Brightness")
                : ProcessImageSegments(bitmap, threadCount, analyzerRegular.AnalyzeBrightnessRegular, "Brightness");

            return totalBrightness < 128 ? $"Dark" : $"Bright";
        }

        private unsafe string AnalyzeTemperature(Bitmap bitmap, int threadCount)
        {
            double temperature = asmChecked
                ? ProcessImageSegments(bitmap, threadCount, CalculateTemperature, "Temperature")
                : ProcessImageSegments(bitmap, threadCount, analyzerRegular.AnalyzeTemperatureRegular, "Temperature");

            return temperature < 0 ? $"Cold" : $"Warm";
        }


        private unsafe string AnalyzeDominantChannel(Bitmap bitmap, int threadCount)
        {
            var (totalRed, totalGreen, totalBlue) = asmChecked
                ? ProcessImageSegmentsDominant(bitmap, threadCount, CalculateDominantChannel, "DominantChannel")
                : ProcessImageSegmentsDominant(bitmap, threadCount, analyzerRegular.AnalyzeDominantChannelRegular, "DominantChannel");

            int maxChannelIndex = totalRed > totalGreen && totalRed > totalBlue ? 0 :
                                  totalGreen > totalRed && totalGreen > totalBlue ? 1 : 2;

            string dominantChannel = maxChannelIndex switch
            {
                0 => "Red",
                1 => "Green",
                _ => "Blue"
            };

            return $"The most used channel is {dominantChannel}";
        }

        private unsafe (long red, long green, long blue) ProcessImageSegmentsDominant(Bitmap image, int threadCount, AsmProcessDelegateDominant processFunction, string analysisType)
        {
            int imageWidth = image.Width;
            int imageHeight = image.Height;

            if (imageWidth == 0 || imageHeight == 0)
                return (0, 0, 0);

            BitmapData bitmapData = image.LockBits(
                new Rectangle(0, 0, imageWidth, imageHeight),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            try
            {
                ConcurrentBag<(long red, long green, long blue)> channelSums = new ConcurrentBag<(long, long, long)>();

                int chunkHeight = 10; // Process in 10-row chunks
                int totalChunks = (imageHeight + chunkHeight - 1) / chunkHeight;

                Parallel.For(0, totalChunks, chunkIndex =>
                {
                    int startY = chunkIndex * chunkHeight;
                    int endY = Math.Min(startY + chunkHeight, imageHeight);
                    int pixelsInChunk = (endY - startY) * imageWidth;

                    if (pixelsInChunk > 0)
                    {
                        byte* startPtr = (byte*)bitmapData.Scan0 + (startY * bitmapData.Stride);
                        int localRed = 0, localGreen = 0, localBlue = 0;
                        processFunction(startPtr, pixelsInChunk, &localRed, &localGreen, &localBlue);
                        channelSums.Add((localRed, localGreen, localBlue));
                    }
                });

                long totalRed = channelSums.Sum(c => c.red);
                long totalGreen = channelSums.Sum(c => c.green);
                long totalBlue = channelSums.Sum(c => c.blue);

                return (totalRed, totalGreen, totalBlue);
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
        }


        private unsafe double ProcessImageSegments(Bitmap image, int threadCount, AsmProcessDelegate processFunction, string analysisType)
        {
            int imageWidth = image.Width;
            int imageHeight = image.Height;

            // Check for edge cases
            if (imageWidth == 0 || imageHeight == 0)
                return 0.0;

            BitmapData bitmapData = image.LockBits(
                new Rectangle(0, 0, imageWidth, imageHeight),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            try
            {
                int totalPixels = imageWidth * imageHeight;
                ConcurrentBag<long> sums = new ConcurrentBag<long>();

                // Define chunk size for dynamic load balancing
                int chunkHeight = 10;
                int totalChunks = (imageHeight + chunkHeight - 1) / chunkHeight;

                Parallel.For(0, totalChunks, chunkIndex =>
                {
                    int startY = chunkIndex * chunkHeight;
                    int endY = Math.Min(startY + chunkHeight, imageHeight);
                    int pixelsInChunk = (endY - startY) * imageWidth;

                    if (pixelsInChunk > 0)
                    {
                        byte* startPtr = (byte*)bitmapData.Scan0 + (startY * bitmapData.Stride);
                        int localSum = 0; 
                        processFunction(startPtr, pixelsInChunk, &localSum);
                        sums.Add(localSum);
                    }
                });

                long totalSum = sums.Sum();

                return analysisType switch
                {
                    "Brightness" => totalSum / (double)(totalPixels * 3),
                    "Temperature" => totalSum / (double)(totalPixels),    
                    "DominantChannel" => totalSum,                        
                    _ => throw new InvalidOperationException("Invalid analysis type")
                };
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }
        }




        private class ImageSegment
        {
            public int StartY { get; set; }
            public int Height { get; set; }
            public int Width { get; set; }
        }

        public AnalysisResult AnalyzeImageSimple(Bitmap bitmap, int threadCount, string analysisType)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string result = analysisType switch
            {
                "Dark / Bright" => AnalyzeBrightness(bitmap, threadCount),
                "Warm / Cold" => AnalyzeTemperature(bitmap, threadCount),
                "Red / Green / Blue" => AnalyzeDominantChannel(bitmap, threadCount),
                _ => throw new InvalidOperationException("Invalid analysis type")
            };

            stopwatch.Stop();
            return new AnalysisResult
            {
                Result = result,
                ProcessingTime = stopwatch.Elapsed
            };
        }

        public AnalysisResult AnalyzeImageWithTests(Bitmap bitmap, int[] threadCountArray, string analysisType, bool asmCheckedBox)
        {
            string libName = asmChecked ? "Asm" : "x64";
            StringBuilder results = new StringBuilder();
            TimeSpan totalProcessingTime = TimeSpan.Zero;
            asmChecked = asmCheckedBox;

            foreach (int threadCount in threadCountArray)
            {
                double totalTime = 0;
                string result = null;

                for (int i = 0; i < 5; i++)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    result = analysisType switch
                    {
                        "Dark / Bright" => AnalyzeBrightness(bitmap, threadCount),
                        "Warm / Cold" => AnalyzeTemperature(bitmap, threadCount),
                        "Red / Green / Blue" => AnalyzeDominantChannel(bitmap, threadCount),
                        _ => throw new InvalidOperationException("Invalid analysis type")
                    };

                    stopwatch.Stop();
                    totalTime += stopwatch.ElapsedMilliseconds;
                }

                double averageTime = totalTime / 5;
                totalProcessingTime += TimeSpan.FromMilliseconds(averageTime);

                results.AppendLine($"Threads: {threadCount}, Result: {result}, Average Time: {averageTime} ms");
            }

            return new AnalysisResult
            {
                Result = results.ToString(),
                ProcessingTime = totalProcessingTime
            };
        }
    }
}
