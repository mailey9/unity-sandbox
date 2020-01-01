using System;
using System.Collections.Generic;

namespace minorlife
{
    public class MapGenerator
    {
        static int DEBUG_generateRunCount = 0;
        static public List<Room> Generate(MapGenerateConfig config)
        {
            List<Rect> rectsBinaryTree = CreateDividedRectBinaryTree(config.Height,
                                                                     config.Width,
                                                                     config.DivideLevel,
                                                                     config.DivideRatioMin,
                                                                     config.DivideRatioMax);

            List<Room> rooms = CreateRooms(rectsBinaryTree,
                                           config.DivideLevel,
                                           config.RectFillCount,
                                           config.RectFillRatioMin,
                                           config.RectFillRatioMax,
                                           config.DiscardLessThanWidth,
                                           config.DiscardLessThanHeight);

            rooms = MergeConsecutiveRooms(rooms);

            //int entranceRoomId = SelectEntranceRoom(rooms);
            //int exitRoomId     = SelectExitRoom(rooms, entranceRoomId); //NOTE(용택): 입구에 대한 의존성이 있으니 직접넘긴다.
            //List<int> discardingRoomIds = SelectDiscardingRooms(rooms, entranceRoomId, exitRoomId); //NOTE(용택): 입구, 출구에 대한 의존성이 있으니 직접넘긴다.
            
            //CreatePaths();

            //NOTE(용택): Thought, Consecutive 여부를 판단할 때, BT Sibling 이면 Consecutive 다. 그냥 거리로 하는 게 낫겠다.
            //           거리로 소팅한다. 대상과의 거리를 알 수 있다. 그래프를 어떻게 그릴 것인가?

            DEBUG_generateRunCount += 1;
            return rooms;
        }

        #region BSP Tree Functions
        static private void DivideVertically(Rect rect, float minRatio, float maxRatio, ref Rect divided_1, ref Rect divided_2)
        {
            //NOTE(용택): Input 1개 --> Output 2개 (ratio 로 Divide 된 두 개의 Rect)
            //            out 이나 ref Param 을 쓸 수 밖에 없을 것 같다. multiple return value 가 안되서..
            //            class 형 리턴을 생각해볼 수는 있다.
            //NOTE(용택): C# 버전에 따라 달라지는데, Unity는 최근까지 .NET Standard 2.0 이 Stable 버전이었다.
            //            따라서 기타 라이브러리 때문에, 가급적 구식버전을 쓰도록 한다.

            //TODO(용택): 이후 Unity 랜덤으로 대체.
            Random random = new Random();
            float randomRatio = random.Next( (int)(minRatio*100), (int)(maxRatio*100) ) / 100.0f;

            int randomHeight = (int)(rect.height * randomRatio);
            divided_1 = divided_2 = rect;//우선 value copy.

            divided_1.height = randomHeight;
            divided_2.height = rect.height - randomHeight;

            divided_1.row = rect.row;
            divided_2.row = rect.row + randomHeight;
        }
        static private void DivideHorizontally(Rect rect, float minRatio, float maxRatio, ref Rect divided_1, ref Rect divided_2)
        {
            //TODO(용택): 이후 Unity 랜덤으로 대체.
            Random random = new Random();
            float randomRatio = random.Next( (int)(minRatio*100), (int)(maxRatio*100) ) / 100.0f;

            int randomWidth = (int)(rect.width * randomRatio);
            divided_1 = divided_2 = rect;//우선 value copy.

            divided_1.width = randomWidth;
            divided_2.width = rect.width - randomWidth;

            divided_1.col = rect.col;
            divided_2.col = rect.col + randomWidth;
        }
        static private List<Rect> CreateDividedRectBinaryTree(int mapRow, int mapCol, int treeLevel, float minRatio, float maxRatio)
        {
            //NOTE(용택): ArrayReprsentation BinaryTree 를 이용한다. (단, 계산하고 버려지므로 성능상의 큰 이점은 없을 것이다.)
            int roomCapa = (int)MathF.Pow(2.0f, treeLevel);
            List<Rect> rectsBinaryTree = new List<Rect>( roomCapa );

            Rect dummy_0;
            dummy_0.row    = -1;
            dummy_0.col    = -1;
            dummy_0.width  = -1;
            dummy_0.height = -1;
            rectsBinaryTree.Add(dummy_0);

            Rect rect_root;
            rect_root.row   = 0;
            rect_root.col   = 0;
            rect_root.width = mapCol;
            rect_root.height= mapRow;
            rectsBinaryTree.Add(rect_root);//NOTE(용택): 계산 편의를 위해 0번은 더미, 1번은 루트 노드다.

            int currentLevel = 1;
            while (currentLevel < treeLevel)
            {
                int nodeIndexStart = (int)MathF.Pow(2.0f, currentLevel - 1);
                int nodeIndexEnd   = (int)MathF.Pow(2.0f, currentLevel);
                for (int nodeIndex = nodeIndexStart; nodeIndex < nodeIndexEnd; ++nodeIndex)
                {
                    Rect divided_1 = new Rect();
                    Rect divided_2 = new Rect();

                    //NOTE(용택): 랜덤으로 하는 것을 고려. 생성된 모양이 나빠질 수 있긴 하다.
                    int divideMode = currentLevel % 2;
                    if (divideMode == 0)
                    {
                        DivideVertically(rectsBinaryTree[nodeIndex], minRatio, maxRatio, ref divided_1, ref divided_2);
                    }
                    else
                    {
                        DivideHorizontally(rectsBinaryTree[nodeIndex], minRatio, maxRatio, ref divided_1, ref divided_2);
                    }

                    rectsBinaryTree.Add(divided_1);
                    rectsBinaryTree.Add(divided_2);
                }

                currentLevel += 1;
            }

            return rectsBinaryTree;
        }
        #endregion //BSP Tree Functions
        #region Room Creation Functions
        static private Rect CalculateFilledRect(Rect rect, float minRatio, float maxRatio)
        {
            Rect filled = rect;

            //TODO(용택): 이후 Unity 랜덤으로 대체.
            Random random = new Random();
            float randomRatio = random.Next( (int)(minRatio*100), (int)(maxRatio*100) ) / 100.0f;

            int randomWidth  = (int)(rect.width * randomRatio);
            int randomHeight = (int)(rect.height * randomRatio);

            //NOTE(용택): int * float 암시적 캐스팅으로 0 이 될 수 있다. 최소 1은 유지해주도록 한다.
            filled.width  = (randomWidth > 0) ? randomWidth : 1;
            filled.height = (randomHeight > 0) ? randomHeight : 1;

            int maxRow = rect.row + (rect.height - randomHeight);
            int maxCol = rect.col + (rect.width - randomWidth);

            //NOTE(용택): random.Next(int min-include,int max-exclude); 이다.
            filled.row = random.Next(rect.row, maxRow + 1);
            filled.col = random.Next(rect.col, maxCol + 1);

            return filled;
        }
        static private List<Room> CreateRooms(List<Rect> rectBinaryTree, int treeLevel, int fillCount, float minFillRatio, float maxFillRatio, int discardWidth, int discardHeight)
        {
            //NOTE(용택): 최대 레벨의 노드 외에는 실제 맵 적용에 필요없다.
            int leafNodeIndexStart = (int)MathF.Pow(2.0f, treeLevel - 1);
            int numberOfLeafNodes  = leafNodeIndexStart;

            List<Rect> leaves = rectBinaryTree.GetRange(leafNodeIndexStart, numberOfLeafNodes);
            List<Room>     rooms  = new List<Room>(leaves.Count);

            for (int i = 0; i < numberOfLeafNodes; ++i)
            {
                if (leaves[i].height * maxFillRatio < discardHeight ||
                    leaves[i].width * maxFillRatio < discardWidth)
                {
                    continue;
                }

                rooms.Add(new Room(fillCount));

                int fillLoopCount = 0;
                while (fillLoopCount < fillCount)
                {
                    Rect filled = CalculateFilledRect(leaves[i], minFillRatio, maxFillRatio);

                    if (filled.height < discardHeight || filled.width < discardWidth)
                    {
                        continue;
                    }

                    if (fillLoopCount > 0)
                    {
                        if (rooms[rooms.Count-1].HasIntersection(filled) == false)
                        {
                            continue;
                        }
                    }
                    rooms[rooms.Count-1].Append(filled);
                    fillLoopCount += 1;
                }
            }

            return rooms;
        }

        static private List<Room> MergeConsecutiveRooms(List<Room> rooms)
        {
            //TODO(용택): (MergeRooms)오버헤드가 큰 지 측정 필요.
            //TODO(용택): (MergeRooms)루프문 재점검 필요
            for (int a = 0; a < rooms.Count; ++a)
            {
                int b = 0;
                while (b < rooms.Count)
                {
                    if (rooms.Count == 1 || a < rooms.Count) break;
                    if (a == b || Room.canMerge(rooms[a], rooms[b]) == false)
                    {
                        b += 1;
                        continue;
                    }

                    rooms[a].Append(rooms[b]);
                    rooms.Remove(rooms[b]);
                }
            }
            return rooms;
        }
        static private List<int> GetDistanceSqSortedRoomIds(int targetRoomId, List<Room> rooms)
        {
            Room targetRoom = Room.FindRoom(targetRoomId, rooms);

            //TODO(용택): (DistanceSorted) 계산해서 Dict 에 넣은 뒤, 다시 List 로 가져오는데 .. 그대로 SortedDict 써도 사실 문제 없다..
            SortedDictionary<float, int> distanceIdPairs = Room.GetEuclidDistanceIdPairs(targetRoom, rooms);

            List<int> sortedIds = new List<int>(rooms.Count - 1);
            foreach(var pair in distanceIdPairs)
            {
                //Console.WriteLine(pair.Value + " : " + pair.Key);
                sortedIds.Add(pair.Value);
            }

            return sortedIds;
        }
        #endregion //Room Creation Functions
        
        //TODO(용택): SelectDiscardingRooms();      // --DiscardRooms() ? --Consecutive 노드를 하나씩은 보존하도록 한다. (되도록, 이미 Room 단계에서 버려질 수 있다.)
        //TODO(용택): SelectEntranceRoom();         // --multiple Entrance & multiple Exit 을 고려하자. 복층구조에 용이할 것 같다.
        //TODO(용택): SelectExitRoom();             // --경우에 따라 Entrance 와 같을 수 있다. --multiple exit 을 고려하자.

        //TODO(용택): CreatePath();
        //
        //static private void 
    }
}