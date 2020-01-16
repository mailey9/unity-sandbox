using System;
using System.Diagnostics;
using System.IO;

//TODO(용택): NOTE들 정리해서 문서로 빼고, 클린업 시작
//TODO(용택): (Project)
//              1. MapGen 은 이쯤에서 WrapUp
//              2. Block-Tiling 샘플 올리기
//              3. 맵 위에 NavMesh 띄우기
//              ~~ 까지를 단시간 목표로. 이후 다시 돌아와 고친다.
//                  (1) Wall-Corner 에 닿을 때 처리
//                  (2) + Corridor 폭이 좁을 때 Wall-Corner 케이스 처리
//                  (3) 말이 안되는 방(낮은 채움밀도), 길에 대한 분기예외처리
//                  (4) 다른 MapGen Impl.

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

            int runCount = 1;
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