using System;
using System.Diagnostics;
using System.IO;

//TODO(용택): NOTE들 정리해서 문서로 빼고, 클린업 시작

namespace minorlife
{   
    class Program
    {
        static void Main(string[] args)
        {
            string runPath = AppDomain.CurrentDomain.BaseDirectory;

            Stopwatch sw_genOnly = new Stopwatch();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int runCount = 1;
            for (int i = 0; i < runCount; ++i)
            {
                sw_genOnly.Start();

                var mapGenConfig = new MapGenerateConfig();

                Map randomMap = new Map(mapGenConfig.width, mapGenConfig.height);
                randomMap.Apply( MapGenerator.Generate(mapGenConfig) );
                randomMap.DEBUG_Apply( MapGenerator.DEBUG_Corridors );
                
                sw_genOnly.Stop();

                string filename = runPath + "RandGenMap_" + mapGenConfig.width + "x" + mapGenConfig.height + "_Level" + mapGenConfig.divideTreeLevel + "_" + i + ".txt";
                File.WriteAllText(@filename, mapGenConfig.ToString() + "\n" + randomMap.ToString());
            }

            sw.Stop();

            Console.WriteLine("TotalElapsed={0}, GenOnlyElapsed={1}", sw.Elapsed, sw_genOnly.Elapsed);
            //Console.ReadKey();
        }
    }
}