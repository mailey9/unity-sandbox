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
            var mapGenConfig = new MapGenerateConfig();

            Stopwatch sw_genOnly = new Stopwatch();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            int runCount = 100000;
            for (int i = 0; i < runCount; ++i)
            {
                sw_genOnly.Start();

                Map randomMap = new Map( MapGenerator.Generate(mapGenConfig) );

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