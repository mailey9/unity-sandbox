using System;
using System.Diagnostics;
using System.IO;

//TODO(용택): Rect , Coord 같은 건 대체 고려

namespace minorlife
{   
    class Program
    {
        static void Main(string[] args)
        {
            string runPath = AppDomain.CurrentDomain.BaseDirectory;

            MapGenerateConfig mapGenConfig = MapGenerateConfig.DEFAULT;//new MapGenerateConfig();
            mapGenConfig.Width = 400;
            mapGenConfig.Height = 300;
            mapGenConfig.DivideLevel = 6;
            mapGenConfig.DivideRatioMin = 0.35f;
            mapGenConfig.DivideRatioMax = 0.45f;
            mapGenConfig.RectFillRatioMin = 0.25f;
            mapGenConfig.RectFillRatioMax = 0.70f;
            mapGenConfig.RectFillCount = 4;
            mapGenConfig.DiscardLessThanWidth = 5;
            mapGenConfig.DiscardLessThanHeight = 5;

            Stopwatch sw_genOnly = new Stopwatch();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int runCount = 10000;
            for (int i = 0; i < runCount; ++i)
            {
                sw_genOnly.Start();

                Map randomMap = new Map(mapGenConfig.Width, mapGenConfig.Height);
                randomMap.Apply( MapGenerator.Generate(mapGenConfig) );
                
                sw_genOnly.Stop();

                string filename = runPath + "RandGenMap_" + mapGenConfig.Width + "x" + mapGenConfig.Height + "_Level" + mapGenConfig.DivideLevel + "_" + i + ".txt";
                File.WriteAllText(@filename, mapGenConfig.ToString() + "\n" + randomMap.ToString());
            }

            sw.Stop();

            Console.WriteLine("TotalElapsed={0}, GenOnlyElapsed={1}", sw.Elapsed, sw_genOnly.Elapsed);
            //Console.ReadKey();
        }
    }
}