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



        public unsafe void AnalyzeTemperatureRegular(byte* pixels, int pixelCount, int* sum)
        {
            int vectorSize = Vector<byte>.Count;
            int localSum = 0;
            Vector<int> vectorSum = Vector<int>.Zero;

            int totalBytes = pixelCount * 4; // BGRA format
            int i = 0;

            // Process vectors
            for (; i <= totalBytes - vectorSize; i += vectorSize)
            {
                byte[] tempBytes = new byte[vectorSize];
                for (int j = 0; j < vectorSize; j++)
                {
                    tempBytes[j] = pixels[i + j];
                }

                Vector<byte> pixelVector = new Vector<byte>(tempBytes);

                for (int k = 0; k < vectorSize; k += 4) // BGRA format
                {
                    int blue = pixelVector[k];      // Blue channel
                    int red = pixelVector[k + 2];   // Red channel
                    vectorSum += new Vector<int>(red - blue); // Temperature calculation (R-B)
                }
            }

            // Sum vector elements
            for (int j = 0; j < Vector<int>.Count; j++)
            {
                localSum += vectorSum[j];
            }

            // Handle remaining pixels
            for (; i < totalBytes; i += 4)
            {
                localSum += pixels[i + 2] - pixels[i]; // Red - Blue
            }

            *sum = localSum;
        }

        public unsafe void AnalyzeDominantChannelRegular(byte* pixels, int pixelCount, int* result)
        {
            int vectorSize = Vector<byte>.Count;
            Vector<int> redSum = Vector<int>.Zero;
            Vector<int> greenSum = Vector<int>.Zero;
            Vector<int> blueSum = Vector<int>.Zero;

            int totalBytes = pixelCount * 4; // BGRA format
            int i = 0;

            // Process vectors
            for (; i <= totalBytes - vectorSize; i += vectorSize)
            {
                byte[] tempBytes = new byte[vectorSize];
                for (int j = 0; j < vectorSize; j++)
                {
                    tempBytes[j] = pixels[i + j];
                }

                Vector<byte> pixelVector = new Vector<byte>(tempBytes);

                for (int k = 0; k < vectorSize; k += 4) // BGRA format
                {
                    blueSum += new Vector<int>(pixelVector[k]);
                    greenSum += new Vector<int>(pixelVector[k + 1]);
                    redSum += new Vector<int>(pixelVector[k + 2]);
                }
            }

            // Sum up each channel
            int totalRed = 0, totalGreen = 0, totalBlue = 0;
            for (int j = 0; j < Vector<int>.Count; j++)
            {
                totalRed += redSum[j];
                totalGreen += greenSum[j];
                totalBlue += blueSum[j];
            }

            // Handle remaining pixels
            for (; i < totalBytes; i += 4)
            {
                totalBlue += pixels[i];
                totalGreen += pixels[i + 1];
                totalRed += pixels[i + 2];
            }

            // Determine dominant channel (0=Red, 1=Green, 2=Blue)
            if (totalRed > totalGreen && totalRed > totalBlue)
                *result = 0;
            else if (totalGreen > totalRed && totalGreen > totalBlue)
                *result = 1;
            else
                *result = 2;
        }
    }
}
