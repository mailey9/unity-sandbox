using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace minorlife
{   
    class Program
    {
        static void Main(string[] args)
        {
            string runPath = AppDomain.CurrentDomain.BaseDirectory;

            MapGenerateConfig mapGenConfig = new MapGenerateConfig();
            mapGenConfig.Width = 120;
            mapGenConfig.Height = 80;
            mapGenConfig.DivideLevel = 6;
            mapGenConfig.DivideRatio_Min = 0.15f;
            mapGenConfig.DivideRatio_Max = 0.45f;
            mapGenConfig.RoomRect_FillRatio_Min = 0.30f;
            mapGenConfig.RoomRect_FillRatio_Max = 0.80f;

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
                File.WriteAllText(@filename, randomMap.ToString());
            }

            sw.Stop();

            Console.WriteLine("TotalElapsed={0}, GenOnlyElapsed={1}", sw.Elapsed, sw_genOnly.Elapsed);
            //Console.ReadKey();
        }
    }
}