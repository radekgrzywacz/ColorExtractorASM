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

        [DllImport(@"C:\Users\SQ299\Source\Repos\ColorExtractorASM\x64\Debug\JAAsm.dll")]
        private static extern void CalculateAverageBrightness(byte* pixels, int pixelCount, int* sum);

        [DllImport(@"C:\Users\SQ299\Source\Repos\ColorExtractorASM\x64\Debug\JAAsm.dll")]
        private static extern int CalculateTemperature(byte* data, int length);

        [DllImport(@"C:\Users\SQ299\Source\Repos\ColorExtractorASM\x64\Debug\JAAsm.dll")]
        private static extern int CalculateDominantChannel(byte* data, int length);

        public bool asmChecked { get; set; } = false;

        private unsafe string AnalyzeBrightness(Bitmap bitmap, int threadCount)
        {
            double totalBrightness = 0.0;
            if (!asmChecked)
            {
                totalBrightness = ProcessImageSegments(bitmap, threadCount, analyzerRegular.AnalyzeBrightnessRegular);
            }

            totalBrightness = ProcessImageSegments(bitmap, threadCount, CalculateAverageBrightness);
            //double averageBrightness = totalBrightness / (double)(bitmap.Width * bitmap.Height * 3);

            return totalBrightness < 128 ? $"Dark {totalBrightness:F2}" : $"Bright {totalBrightness:F2}";
        }

        //private unsafe string AnalyzeTemperature(Bitmap bitmap, int threadCount)
        //{
        //    if (!asmChecked)
        //    {
        //        return analyzerRegular.AnalyzeTemperatureRegular(bitmap, threadCount);
        //    }

        //    int temperatureSum = ProcessImageSegments(bitmap, threadCount, CalculateTemperature);

        //    return temperatureSum > 0 ? $"Warm {temperatureSum}" : $"Cold {temperatureSum}";
        //}

        private unsafe double ProcessImageSegments(Bitmap image, int threadCount, AsmProcessDelegate processFunction)
        {
            // Validate and adjust thread count
            if (threadCount <= 0)
                threadCount = Environment.ProcessorCount;
            else
                threadCount = Math.Min(threadCount, Environment.ProcessorCount);

            int imageWidth = image.Width;
            int imageHeight = image.Height;

            BitmapData bitmapData = image.LockBits(
                new Rectangle(0, 0, imageWidth, imageHeight),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            try
            {
                int totalPixels = imageWidth * imageHeight;
                int* sum = stackalloc int[threadCount]; // Separate accumulator for each thread

                // Split the image into chunks for parallel processing
                int chunkSize = (imageHeight + threadCount - 1) / threadCount; // Ceiling division
                Parallel.For(0, threadCount, i =>
                {
                    int startY = i * chunkSize;
                    int endY = Math.Min((i + 1) * chunkSize, imageHeight);
                    int pixelsInChunk = (endY - startY) * imageWidth;

                    if (pixelsInChunk > 0) // Only process if there are pixels in this chunk
                    {
                        byte* startPtr = (byte*)bitmapData.Scan0 + (startY * bitmapData.Stride);
                        processFunction(startPtr, pixelsInChunk, &sum[i]);
                    }
                });

                // Sum up results from all threads
                int totalSum = 0;
                for (int i = 0; i < threadCount; i++)
                {
                    totalSum += sum[i];
                }

                // Calculate final average
                return totalSum / (double)(totalPixels * 3);
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
                //"Warm / Cold" => AnalyzeTemperature(bitmap, threadCount),
                // "Red / Green / Blue" => AnalyzeDominantChannel(bitmap, threadCount),
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
                        // "Warm / Cold" => AnalyzeTemperature(ImageToBitArray(bitmap), bitmap.Width, bitmap.Height, threadCount),
                        // "Red / Green / Blue" => AnalyzeDominantChannel(ImageToBitArray(bitmap), bitmap.Width, bitmap.Height, threadCount),
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
