﻿using System;
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

        public unsafe void AnalyzeDominantChannelRegular(byte* pixels, int pixelCount, int* totalRed, int* totalGreen, int* totalBlue)
        {
            int redSum = 0, greenSum = 0, blueSum = 0;

            for (int i = 0; i < pixelCount; i++)
            {
                byte blue = pixels[i * 4];
                byte green = pixels[i * 4 + 1];
                byte red = pixels[i * 4 + 2];

                blueSum += blue;
                greenSum += green;
                redSum += red;
            }

            *totalRed = redSum;
            *totalGreen = greenSum;
            *totalBlue = blueSum;
        }
    }
}

