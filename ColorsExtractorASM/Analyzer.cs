using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ColorsExtractorASM
{
    public class Analyzer
    {
        //[DllImport("BitmapConverter.dll", CallingConvention = CallingConvention.StdCall)]
        //public static extern void ImageToBitArrayASM(IntPtr bitmapData, [Out] byte[] bits, int width, int height);

        
        public AnalysisResult AnalyzeImage(Bitmap bitmap, int threadCount, string analysisType, bool asmChecked)
        {
            // Otwórz obraz
            byte[] imageBits = ImageToBitArray(bitmap);

            // Wielowątkowość i analiza
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string result = analysisType switch
            {
                "Dark / Bright" => AnalyzeBrightness(imageBits, bitmap.Width, bitmap.Height, threadCount),
                "Warm / Cold" => AnalyzeTemperature(imageBits, bitmap.Width, bitmap.Height, threadCount),
                "Red / Green / Blue" => AnalyzeDominantChannel(imageBits, bitmap.Width, bitmap.Height, threadCount),
                _ => throw new InvalidOperationException("Invalid analysis type")
            };

            stopwatch.Stop();
            AnalysisResult analysisResult = new AnalysisResult();
            analysisResult.Result = result;
            analysisResult.ProcessingTime = stopwatch.Elapsed;
            return analysisResult;
        }

        // Funkcja konwertująca obraz do tablicy bitów RGB
        private byte[] ImageToBitArray(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            byte[] bits = new byte[width * height * 3];

            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);

            unsafe
            {
                byte* ptr = (byte*)bmpData.Scan0;
                int index = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        bits[index++] = ptr[2]; // Red
                        bits[index++] = ptr[1]; // Green
                        bits[index++] = ptr[0]; // Blue
                        ptr += 3;
                    }
                    ptr += bmpData.Stride - (width * 3);
                }
            }

            bitmap.UnlockBits(bmpData);
            return bits;
        }

        // Analiza jasności z wykorzystaniem tablicy bitów
        private string AnalyzeBrightness(byte[] bits, int width, int height, int threadCount)
        {
            int totalPixels = width * height;
            long brightnessSum = 0;

            Parallel.For(0, threadCount, thread =>
            {
                int start = thread * totalPixels / threadCount;
                int end = (thread + 1) * totalPixels / threadCount;
                long localSum = 0;

                for (int i = start; i < end; i++)
                {
                    int index = i * 3;
                    byte r = bits[index];
                    byte g = bits[index + 1];
                    byte b = bits[index + 2];
                    localSum += (r + g + b) / 3; // Średnia jasność piksela
                }

                Interlocked.Add(ref brightnessSum, localSum);
            });

            double averageBrightness = brightnessSum / (double)totalPixels;
            return averageBrightness > 128 ? "Bright" : "Dark";
        }

        // Analiza temperatury kolorów (ciepłe/zimne)
        private string AnalyzeTemperature(byte[] bits, int width, int height, int threadCount)
        {
            int totalPixels = width * height;
            long temperatureSum = 0;

            Parallel.For(0, threadCount, thread =>
            {
                int start = thread * totalPixels / threadCount;
                int end = (thread + 1) * totalPixels / threadCount;
                long localSum = 0;

                for (int i = start; i < end; i++)
                {
                    int index = i * 3;
                    byte r = bits[index];
                    byte b = bits[index + 2];
                    localSum += r - b; // Różnica pomiędzy czerwoną a niebieską składową
                }

                Interlocked.Add(ref temperatureSum, localSum);
            });

            return temperatureSum > 0 ? "Warm" : "Cool";
        }

        // Analiza dominującego kanału RGB
        private string AnalyzeDominantChannel(byte[] bits, int width, int height, int threadCount)
        {
            int totalPixels = width * height;
            long redSum = 0, greenSum = 0, blueSum = 0;

            Parallel.For(0, threadCount, thread =>
            {
                int start = thread * totalPixels / threadCount;
                int end = (thread + 1) * totalPixels / threadCount;
                long localRed = 0, localGreen = 0, localBlue = 0;

                for (int i = start; i < end; i++)
                {
                    int index = i * 3;
                    localRed += bits[index];
                    localGreen += bits[index + 1];
                    localBlue += bits[index + 2];
                }

                Interlocked.Add(ref redSum, localRed);
                Interlocked.Add(ref greenSum, localGreen);
                Interlocked.Add(ref blueSum, localBlue);
            });

            // Porównanie kanałów
            string message;
            if (redSum > greenSum && redSum > blueSum)
            {
                message = "Red";
            }
            else if (greenSum > redSum && greenSum > blueSum)
            {
                message = "Green";
            }
            else
            {
                message = "Blue";
            }

            return "The most used channel is " + message;
        }

    }
}
