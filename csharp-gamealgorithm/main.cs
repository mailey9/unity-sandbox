using System;
using System.Diagnostics;
using System.IO;

namespace minorlife
{   
    class Program
    {
        static void Main(string[] args)
        {
            string runPath = AppDomain.CurrentDomain.BaseDirectory;

            MapGenerateConfig mapGenConfig = MapGenerateConfig.DEFAULT;//new MapGenerateConfig();
            mapGenConfig.Width = 250;
            mapGenConfig.Height = 250;
            mapGenConfig.DivideLevel = 7;
            mapGenConfig.DivideRatioMin = 0.35f;
            mapGenConfig.DivideRatioMax = 0.50f;
            mapGenConfig.RoomRectFillRatioMin = 0.25f;
            mapGenConfig.RoomRectFillRatioMax = 0.70f;
            mapGenConfig.RoomRectFillCount = 3;
            mapGenConfig.DiscardLessThanWidth = 5;
            mapGenConfig.DiscardLessThanHeight = 5;

            Stopwatch sw_genOnly = new Stopwatch();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < 5; ++i)
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