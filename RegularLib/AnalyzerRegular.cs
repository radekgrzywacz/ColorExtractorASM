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
        // Method to calculate average brightness
        public unsafe void AnalyzeBrightnessRegular(byte* pixels, int pixelCount, int* sum)
        {
            int localSum = 0;

            // Process each pixel
            for (int i = 0; i < pixelCount; i++)
            {
                // BGRA format: blue, green, red, alpha
                byte blue = pixels[i * 4];      // Blue channel
                byte green = pixels[i * 4 + 1]; // Green channel
                byte red = pixels[i * 4 + 2];   // Red channel

                // Sum the RGB components for brightness calculation
                localSum += blue + green + red;
            }

            // Store the result in the sum pointer
            *sum = localSum;
        }

        // Method to calculate temperature (Red - Blue)
        public unsafe void AnalyzeTemperatureRegular(byte* pixels, int pixelCount, int* sum)
        {
            int localSum = 0;

            // Process each pixel
            for (int i = 0; i < pixelCount; i++)
            {
                // BGRA format: blue, green, red, alpha
                byte blue = pixels[i * 4];      // Blue channel
                byte red = pixels[i * 4 + 2];   // Red channel

                // Calculate temperature (Red - Blue)
                localSum += red - blue;
            }

            // Store the result in the sum pointer
            *sum = localSum;
        }

        // Method to calculate dominant channel
        public unsafe void AnalyzeDominantChannelRegular(byte* pixels, int pixelCount, int* result)
        {
            int totalRed = 0, totalGreen = 0, totalBlue = 0;

            // Process each pixel
            for (int i = 0; i < pixelCount; i++)
            {
                // BGRA format: blue, green, red, alpha
                byte blue = pixels[i * 4];      // Blue channel
                byte green = pixels[i * 4 + 1]; // Green channel
                byte red = pixels[i * 4 + 2];   // Red channel

                // Accumulate the sum of each channel
                totalBlue += blue;
                totalGreen += green;
                totalRed += red;
            }

            // Determine the dominant channel
            if (totalRed > totalGreen && totalRed > totalBlue)
                *result = 0; // Red is dominant
            else if (totalGreen > totalRed && totalGreen > totalBlue)
                *result = 1; // Green is dominant
            else
                *result = 2; // Blue is dominant
        }
    }
}

