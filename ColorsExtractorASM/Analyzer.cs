using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ColorsExtractorASM.ColorAnalyzer;

namespace ColorsExtractorASM
{
    public partial class Analyzer
    {

        public AnalysisResult Analyze(Bitmap image, int threadCount, AnalysisType analysisType) {

            var stopwatch = Stopwatch.StartNew();

            

            string result;

            // Perform the selected analysis
            switch (analysisType)
            {
                case AnalysisType.Temperature:
                   // result = WarmnessAnalysis(regions, threadCount).ToString();
                    break;
                case AnalysisType.Brightness:
                    //result = BrightnessAnalysis(regions, threadCount).ToString();
                    break;
                case AnalysisType.DominantColor:
                    //result = DominantColorAnalysis(regions, threadCount).ToString();
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

    }
}
