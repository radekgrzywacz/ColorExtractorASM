using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace ColorsExtractorASM
{
    public class ColorAnalyzer
    {
        [DllImport(@"C:\Users\SQ299\source\repos\ColorExtractorASM\x64\Debug\JAAsm.dll")]
        private static extern void ProcessPixelsASM(
            byte[] region, int length, int[] results);

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

        public enum ColorTemperature
        {
            Cold,
            Neutral,
            Warm
        }

        public bool UseASM = false;

        public void setUseASM()
        {
            UseASM = true;
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
                    result = BrightnessAnalysis(regions, threadCount).ToString();
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

        private TResult ProcessPixelsCommon<TResult>(
            byte[] region, Func<byte, byte, byte, TResult> pixelProcessor,
            Func<TResult, TResult, TResult> resultAggregator,
            TResult initialValue)
        {
            if (UseASM)
            {
                return ProcessPixelsASMWrapper(region, resultAggregator, initialValue);
            }
            else
            {
                return ProcessPixelsStandard(region, pixelProcessor, resultAggregator, initialValue);
            }
        }

        private TResult ProcessPixelsStandard<TResult>(
            byte[] region, Func<byte, byte, byte, TResult> pixelProcessor,
            Func<TResult, TResult, TResult> resultAggregator,
            TResult initialValue)
        {
            TResult aggregatedResult = initialValue;

            for (int i = 0; i < region.Length; i += 4)
            {
                if (i + 3 >= region.Length) break;

                byte b = region[i];     // Blue
                byte g = region[i + 1]; // Green
                byte r = region[i + 2]; // Red

                TResult pixelResult = pixelProcessor(r, g, b);
                aggregatedResult = resultAggregator(aggregatedResult, pixelResult);
            }

            return aggregatedResult;
        }

        private TResult ProcessPixelsASMWrapper<TResult>(
            byte[] region, Func<TResult, TResult, TResult> resultAggregator,
            TResult initialValue)
        {
            int[] results = new int[4]; // Array for ASM results (modify according to your needs).
            ProcessPixelsASM(region, region.Length, results);

            // Convert the ASM results to the appropriate TResult.
            return (TResult)Convert.ChangeType(results[0], typeof(TResult)); // Adjust logic based on TResult type.
        }

        // Temperature Analysis
        private ColorTemperature WarmnessAnalysis(byte[][] regions, int threadCount)
        {
            var temperatures = new ConcurrentBag<ColorTemperature>();
            Parallel.ForEach(regions, new ParallelOptions { MaxDegreeOfParallelism = threadCount }, region =>
            {
                var regionTemperature = ProcessPixelsCommon(
                    region,
                    (r, g, b) =>
                    {
                        double warmthScore = (r + g) / 2.0 - b;
                        if (warmthScore > 50) return ColorTemperature.Warm;
                        if (warmthScore < -50) return ColorTemperature.Cold;
                        return ColorTemperature.Neutral;
                    },
                    (temp1, temp2) => temp1 == temp2 ? temp1 : ColorTemperature.Neutral,
                    ColorTemperature.Neutral);

                temperatures.Add(regionTemperature);
            });

            return temperatures
                .GroupBy(t => t)
                .OrderByDescending(g => g.Count())
                .First()
                .Key;
        }

        // Brightness Analysis
        private string BrightnessAnalysis(byte[][] regions, int threadCount)
        {
            double totalBrightness = 0;
            int totalPixels = 0;

            Parallel.ForEach(regions, new ParallelOptions { MaxDegreeOfParallelism = threadCount }, region =>
            {
                var regionBrightness = ProcessPixelsCommon(
                    region,
                    (r, g, b) => 0.2126 * r + 0.7152 * g + 0.0722 * b,
                    (brightness1, brightness2) => brightness1 + brightness2,
                    0.0);

                lock (this)
                {
                    totalBrightness += regionBrightness;
                    totalPixels += region.Length / 4;
                }
            });

            double averageBrightness = totalPixels > 0 ? totalBrightness / totalPixels : 0;
            return averageBrightness > 128 ? "Bright" : "Dark";
        }

        // Dominant Color Analysis
        private string DominantColorAnalysis(byte[][] regions, int threadCount)
        {
            var channelSums = new ConcurrentDictionary<string, long>
            {
                ["R"] = 0,
                ["G"] = 0,
                ["B"] = 0
            };

            Parallel.ForEach(regions, new ParallelOptions { MaxDegreeOfParallelism = threadCount }, region =>
            {
                var regionChannels = ProcessPixelsCommon(
                    region,
                    (r, g, b) => new { R = (long)r, G = (long)g, B = (long)b },
                    (ch1, ch2) => new { R = ch1.R + ch2.R, G = ch1.G + ch2.G, B = ch1.B + ch2.B },
                    new { R = 0L, G = 0L, B = 0L });

                lock (channelSums)
                {
                    channelSums["R"] += regionChannels.R;
                    channelSums["G"] += regionChannels.G;
                    channelSums["B"] += regionChannels.B;
                }
            });

            var mostUsedChannel = channelSums
                .OrderByDescending(channel => channel.Value)
                .First()
                .Key;

            return $"From RGB, the most used is {mostUsedChannel}";
        }
    }
}
