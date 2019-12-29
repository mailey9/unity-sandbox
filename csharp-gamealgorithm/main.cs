using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace csharp_gamealgorithm
{
    public struct JsonConfig
    {
        public int NUMBER_OF_GENERATE {get; set;}

        public int SIZE_ROW {get; set;}
        public int SIZE_COL {get; set;}

        public int DIVIDE_LEVEL {get; set;}
        public float DIVIDE_RATIO_MIN {get; set;}
        public float DIVIDE_RATIO_MAX {get; set;}

        public float ROOMRECT_FILL_RATIO_MIN {get; set;}
        public float ROOMRECT_FILL_RATIO_MAX {get; set;}
    }
    public struct RoomRect
    {
        public int row;
        public int col;
        public int width;
        public int height;

        public override string ToString()
        {
            return "r:" + row + ", c:" + col + ", w:" + width + ", h:" + height;
        }
    }

    public class Map
    {
        public struct Setup
        {
            public const int DEFAULT_SIZE_ROW = 80;
            public const int DEFAULT_SIZE_COL = 50;

            public const int MIN_SIZE_ROW = 3;
            public const int MIN_SIZE_COL = 5;

            public const int DEFAULT_DIVIDE_LEVEL = 4;
            public const float DEFAULT_DIVIDE_RATIO_MIN = 0.35f;
            public const float DEFAULT_DIVIDE_RATIO_MAX = 0.45f;//NOTE(용택): 1-x 니 반대쪽이 55%가 된다. 0.5f 이하면 된다.

            public const float DEFAULT_ROOMRECT_FILL_RATIO_MIN = 0.75f;
            public const float DEFAULT_ROOMRECT_FILL_RATIO_MAX = 0.95f;
        }

        List< List<int> >   _map;
        public override string ToString()
        {
            string ret = "";
            foreach(List<int> rows in _map)
            {
                foreach(int i in rows)
                {
                    if (i == 0)
                        ret += ". ";
                    else
                        ret += "# ";
                }
                ret += "\n";
            }
            return ret;
        }

        bool ValidateRoomRect(RoomRect roomRect)
        {
            if (roomRect.row < 0 || roomRect.col < 0 ||
                roomRect.row + roomRect.height > _map.Count ||
                roomRect.col + roomRect.width > _map[0].Count)
            {
                Console.WriteLine("out of map(r" + _map.Count + ",c" + _map[0].Count + ") , RoomRect:" + roomRect);
                return false;
            }

            if (roomRect.width <= 0 || roomRect.height <= 0)
            {
                Console.WriteLine("room size too small: " + roomRect);
                return false;
            }

            return true;
        }

        //TODO(용택): 나중에 팩토리 같은 걸로 옮긴다.
        static public Map Generate(int mapRow = Setup.DEFAULT_SIZE_ROW, int mapCol = Setup.DEFAULT_SIZE_COL,
                                   int divideLevel = Setup.DEFAULT_DIVIDE_LEVEL,
                                   float minDivideRatio = Setup.DEFAULT_DIVIDE_RATIO_MIN,
                                   float maxDivdedRatio = Setup.DEFAULT_DIVIDE_RATIO_MAX,
                                   float minFillRatio = Setup.DEFAULT_ROOMRECT_FILL_RATIO_MIN,
                                   float maxFillRatio = Setup.DEFAULT_ROOMRECT_FILL_RATIO_MAX)
        {
            Map map  = new Map();
            map._map = Create2dArray(mapRow,mapCol);

            List<RoomRect> roomRects = CreateDividedRooms(mapRow,
                                                          mapCol,
                                                          minDivideRatio,
                                                          maxDivdedRatio,
                                                          divideLevel);

            //NOTE(용택): 디버그용 Validation ..
            for (int roomIndex = 1; roomIndex < roomRects.Count; ++roomIndex)//NOTE(용택): 더미인 0번은 스킵
            {
                RoomRect original = roomRects[roomIndex];
                RoomRect filled   = FillRoom(roomRects[roomIndex], minFillRatio, maxFillRatio);
                map.ValidateRoomRect( original );
                map.ValidateRoomRect( filled );

                roomRects[roomIndex] = filled;
            }

            //NOTE(용택): 최대 레벨의 노드들만 채워주면 된다. (레벨 4 * 바이너리 노드 = 방 8개 = 8~15번 노드)
            int roomNodeIndexStart = (int)Math.Pow(2.0, divideLevel - 1);
            int roomNodeIndexEnd   = (int)Math.Pow(2.0, divideLevel);
            for (int roomNodeIndex = roomNodeIndexStart; roomNodeIndex < roomNodeIndexEnd; ++roomNodeIndex)
            {
                //TODO(용택): 디버그를 위해 위해서 Fill 해주었지만, 사실은 여기서 해줘야 한다.
                //roomRects[roomNodeIndex] = FillRoom(roomRects[roomNodeIndex],
                //                                    Setup.DEFAULT_ROOMRECT_FILL_RATIO_MIN,
                //                                    Setup.DEFAULT_ROOMRECT_FILL_RATIO_MAX);

                //NOTE(용택): 생성된 room-node 들을 실제 map 반영한다.
                //NOTE(용택): memcpy 같은거 없나..?
                RoomRect room = roomRects[roomNodeIndex];//TODO(용택): 임시변수 없애도록 할 것
                for (int r = room.row; r < room.row + room.height; ++r)
                {
                    for (int c = room.col; c < room.col + room.width; ++c)
                    {
                        map._map[r][c] = 1;
                    }
                }
            }

            return map;
        }

        static private List<List<int>> Create2dArray(int row, int col)
        {
            var array2d = new List< List<int> >(row);
            for(int r = 0; r < row; ++r)
            {
                array2d.Add( new List<int>(new int[col]) );
                for(int c = 0; c < array2d[r].Count; ++c)
                {
                    array2d[r][c] = 0;
                }
            }
            return array2d;
        }

        static private RoomRect FillRoom(RoomRect roomRect, float minRatio, float maxRatio)
        {
            RoomRect filled = roomRect;

            //TODO(용택): 이후 Unity 랜덤으로 대체.
            Random random = new Random();
            float randomRatio = random.Next( (int)(minRatio*100), (int)(maxRatio*100) ) / 100.0f;

            int randomWidth  = (int)(roomRect.width * randomRatio);
            int randomHeight = (int)(roomRect.height * randomRatio);

            //NOTE(용택): int * float 암시적 캐스팅으로, 최소 1은 유지해주도록 한다.
            filled.width  = (randomWidth > 0) ? randomWidth : 1;
            filled.height = (randomHeight > 0) ? randomHeight : 1;

//NOTE(용택): 비율을 채우는 알고리즘의 케이스스터디
//      row:22, height:5 일 때...        // 이 때, row 의 index 는 22,23,24,25,26 이 가능.
//      random 으로 뽑은값은 0.8 (80%)
//      height * 0.8 = 4                 // 22~25, 23~26 의 두 가지 케이스가 생긴다. (해당 중 채운 비율)
//                                      // 즉, 랜덤으로 뽑는 "새로운 row의 값" 은 22 또는 23 이 되어야한다.
//      originalHeight(5) - randomHeight(4) = 1(*)
//      originalRow(22) 에서 originalRow(22) + 1(*) ==> 22 ~ 23
//
//      케이스가 맞는지 확인해보자. row 22, height 5, randomRatio 40%( 5*40% = 2 ) ... 
//      가능한 인덱스 채움은 22~23, 23~24, 24~25, 25~26 의 네 가지 케이스가 있다. 랜덤으로 뽑은 값은 22~25 가 되어야한다.
//      originalHeight(5) - randomHeight(2) = 3
//      originalRow(22) 에서 originalRow(22) + 3 ==> 22 ~ 25   .. okay, correct !

            int maxRow = roomRect.row + (roomRect.height - randomHeight);
            int maxCol = roomRect.col + (roomRect.width - randomWidth);
            filled.row = random.Next(roomRect.row, maxRow);
            filled.col = random.Next(roomRect.col, maxCol);

            return filled;
        }
        static private List<RoomRect> CreateDividedRooms(int mapRow, int mapCol, float minRatio, float maxRatio, int treeLevel)
        {
            //NOTE(용택): ArrayReprsentation BinaryTree 를 이용한다. (단, 계산하고 버려지므로 성능상의 큰 이점은 없을 것이다.)
            int roomCapa = (int)Math.Pow(2.0, treeLevel);
            List<RoomRect> roomRects = new List<RoomRect>( roomCapa );

            RoomRect dummy_0;
            dummy_0.row    = -1;
            dummy_0.col    = -1;
            dummy_0.width  = -1;
            dummy_0.height = -1;
            roomRects.Add(dummy_0);

            RoomRect roomRect_root;
            roomRect_root.row   = 0;
            roomRect_root.col   = 0;
            roomRect_root.width = mapCol;
            roomRect_root.height= mapRow;
            roomRects.Add(roomRect_root);//NOTE(용택): 계산 편의를 위해 0번은 더미, 1번은 루트 노드다.

            //NOTE(용택):
            //  2중 루프를 이용해야한다.
            //  Level 에 대한 while loop
            //  Level 의 모든 index(=node) 에 대한 for 루프
            int currentLevel = 1;
            while (currentLevel < treeLevel)
            {
                int nodeIndexStart = (int)Math.Pow(2.0, currentLevel - 1);
                int nodeIndexEnd   = (int)Math.Pow(2.0, currentLevel);
                for (int nodeIndex = nodeIndexStart; nodeIndex < nodeIndexEnd; ++nodeIndex)
                {
                    RoomRect divided_1 = new RoomRect();
                    RoomRect divided_2 = new RoomRect();

                    //NOTE(용택): 랜덤으로 하는 것을 고려. 모양이 나빠질 수 있긴 하다.
                    int divideMode = currentLevel % 2;
                    if (divideMode == 0)
                    {
                        DivideVertically(roomRects[nodeIndex], minRatio, maxRatio, ref divided_1, ref divided_2);
                    }
                    else
                    {
                        DivideHorizontally(roomRects[nodeIndex], minRatio, maxRatio, ref divided_1, ref divided_2);
                    }

                    roomRects.Add(divided_1);
                    roomRects.Add(divided_2);
                }

                currentLevel += 1;
            }

            return roomRects;
        }

        static private void DivideVertically(RoomRect roomRect, float minRatio, float maxRatio, ref RoomRect divided_1, ref RoomRect divided_2)
        {
            //NOTE(용택): Input 1개 --> Output 2개 (ratio 로 Divide 된 두 개의 RoomRect)
            //            out 이나 ref Param 을 쓸 수 밖에 없을 것 같다. multiple return value 가 안되서..
            //NOTE(용택): C# 버전에 따라 달라지는데, Unity는 최근까지 .NET Standard 2.0 이 Stable 버전이었다.
            //            따라서 기타 라이브러리 때문에, 가급적 구식버전을 쓰도록 한다.

            //TODO(용택): 이후 Unity 랜덤으로 대체.
            Random random = new Random();
            float randomRatio = random.Next( (int)(minRatio*100), (int)(maxRatio*100) ) / 100.0f;

            int randomHeight = (int)(roomRect.height * randomRatio);
            divided_1 = divided_2 = roomRect;//우선 value copy.

            divided_1.height = randomHeight;
            divided_2.height = roomRect.height - randomHeight;

            divided_1.row = roomRect.row;
            divided_2.row = roomRect.row + randomHeight;
        }

        static private void DivideHorizontally(RoomRect roomRect, float minRatio, float maxRatio, ref RoomRect divided_1, ref RoomRect divided_2)
        {
            //TODO(용택): 이후 Unity 랜덤으로 대체.
            Random random = new Random();
            float randomRatio = random.Next( (int)(minRatio*100), (int)(maxRatio*100) ) / 100.0f;

            int randomWidth = (int)(roomRect.width * randomRatio);
            divided_1 = divided_2 = roomRect;//우선 value copy.

            divided_1.width = randomWidth;
            divided_2.width = roomRect.width - randomWidth;

            divided_1.col = roomRect.col;
            divided_2.col = roomRect.col + randomWidth;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string jsonString = File.ReadAllText("runConfig.json");
            JsonConfig config = JsonSerializer.Deserialize<JsonConfig>(jsonString);


            Stopwatch sw_genOnly = new Stopwatch();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (int i = 0; i < config.NUMBER_OF_GENERATE; ++i)
            {
                sw_genOnly.Start();

                Map randomMap = Map.Generate(config.SIZE_ROW,
                                             config.SIZE_COL,
                                             config.DIVIDE_LEVEL,
                                             config.DIVIDE_RATIO_MIN,
                                             config.DIVIDE_RATIO_MAX,
                                             config.ROOMRECT_FILL_RATIO_MIN,
                                             config.ROOMRECT_FILL_RATIO_MAX);

                sw_genOnly.Stop();

                string filename = "RandGenMap_" + config.SIZE_COL + "x" + config.SIZE_ROW + "_Level" + config.DIVIDE_LEVEL + "_" + i + ".txt";
                System.IO.File.WriteAllText(@filename, randomMap.ToString());
            }

            sw.Stop();

            Console.WriteLine("TotalElapsed={0}, GenOnlyElapsed={1}", sw.Elapsed, sw_genOnly.Elapsed);
            //Console.ReadKey();
        }
    }
}
