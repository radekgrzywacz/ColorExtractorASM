using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RegularLib
{
    public class AnalyzerRegular
    {
        public unsafe void AnalyzeBrightnessRegular(byte* pixels, int pixelCount, int* sum)
        {
            int vectorSize = Vector<byte>.Count; // Number of bytes the vector can handle at once
            int localSum = 0;

            // Accumulator for intermediate sums
            Vector<int> vectorSum = Vector<int>.Zero;

            int totalBytes = pixelCount * 4; // Total number of bytes (BGRA format)
            int i = 0;

            // Process as many pixels as possible in chunks of `vectorSize`
            for (; i <= totalBytes - vectorSize; i += vectorSize)
            {
                // Create a temporary array to load data into the vector
                byte[] tempBytes = new byte[vectorSize];
                for (int j = 0; j < vectorSize; j++)
                {
                    tempBytes[j] = pixels[i + j];
                }

                // Load bytes into a vector
                Vector<byte> pixelVector = new Vector<byte>(tempBytes);

                // Accumulate RGB values
                for (int k = 0; k < vectorSize; k += 4) // BGRA format, process every 4 bytes
                {
                    int blue = pixelVector[k];
                    int green = pixelVector[k + 1];
                    int red = pixelVector[k + 2];

                    vectorSum += new Vector<int>(blue + green + red);
                }
            }

            // Sum up the elements of vectorSum into localSum
            for (int j = 0; j < Vector<int>.Count; j++)
            {
                localSum += vectorSum[j];
            }

            // Process remaining pixels that didn't fit into the vectorized loop
            for (; i < totalBytes; i += 4)
            {
                localSum += pixels[i] + pixels[i + 1] + pixels[i + 2]; // Add Blue, Green, Red
            }

            // Store the result in the sum pointer
            *sum = localSum;
        }



        public string AnalyzeTemperatureRegular(Bitmap bitmap, int threadCount)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            byte[] bits = ImageToBitArray(bitmap);
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
                    localSum += r - b;
                }

                Interlocked.Add(ref temperatureSum, localSum);
            });

            return temperatureSum > 0 ? $"Warm {temperatureSum}" : $"Cold {temperatureSum}";
        }

        public string AnalyzeDominantChannelRegular(Bitmap bitmap, int threadCount)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            byte[] bits = ImageToBitArray(bitmap);
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
                        bits[index++] = ptr[2];
                        bits[index++] = ptr[1];
                        bits[index++] = ptr[0];
                        ptr += 3;
                    }
                    ptr += bmpData.Stride - (width * 3);
                }
            }

            bitmap.UnlockBits(bmpData);
            return bits;
        }
    }
}
