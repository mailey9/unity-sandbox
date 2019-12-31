using System;
using System.Collections.Generic;

namespace minorlife
{
    public class MapGenerator
    {
        static public List<Room> Generate(MapGenerateConfig config)
        {
            List<RoomRect> roomRectsBinaryTree = CreateDividedRoomRectBinaryTree(config.Height,
                                                                                 config.Width,
                                                                                 config.DivideLevel,
                                                                                 config.DivideRatioMin,
                                                                                 config.DivideRatioMax);

            List<Room> rooms = CreateRooms(roomRectsBinaryTree,
                                           config.DivideLevel,
                                           config.RoomRectFillCount,
                                           config.RoomRectFillRatioMin,
                                           config.RoomRectFillRatioMax,
                                           config.DiscardLessThanWidth,
                                           config.DiscardLessThanHeight);

            //GetDistanceSortedRoomIds(3, rooms);
            MergeConnectiveRooms(rooms);

            //int entranceRoomId = SelectEntranceRoom(rooms);
            //int exitRoomId     = SelectExitRoom(rooms, entranceRoomId); //NOTE(용택): 입구에 대한 의존성이 있으니 직접넘긴다.
            //List<int> discardingRoomIds = SelectDiscardingRooms(rooms, entranceRoomId, exitRoomId); //NOTE(용택): 입구, 출구에 대한 의존성이 있으니 직접넘긴다.

            //NOTE(용택): Thought, 
            //          Consecutive 여부를 판단할 때, BT Sibling 이면 Consecutive 다.
            //          그냥 거리로 하는 게 낫겠다.

            return rooms;
        }

        #region BSP Tree Functions
        static private void DivideVertically(RoomRect roomRect, float minRatio, float maxRatio, ref RoomRect divided_1, ref RoomRect divided_2)
        {
            //NOTE(용택): Input 1개 --> Output 2개 (ratio 로 Divide 된 두 개의 RoomRect)
            //            out 이나 ref Param 을 쓸 수 밖에 없을 것 같다. multiple return value 가 안되서..
            //            class 형 리턴을 생각해볼 수는 있다.
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
        static private List<RoomRect> CreateDividedRoomRectBinaryTree(int mapRow, int mapCol, int treeLevel, float minRatio, float maxRatio)
        {
            //NOTE(용택): ArrayReprsentation BinaryTree 를 이용한다. (단, 계산하고 버려지므로 성능상의 큰 이점은 없을 것이다.)
            int roomCapa = (int)MathF.Pow(2.0f, treeLevel);
            List<RoomRect> roomRectsBinaryTree = new List<RoomRect>( roomCapa );

            RoomRect dummy_0;
            dummy_0.row    = -1;
            dummy_0.col    = -1;
            dummy_0.width  = -1;
            dummy_0.height = -1;
            roomRectsBinaryTree.Add(dummy_0);

            RoomRect roomRect_root;
            roomRect_root.row   = 0;
            roomRect_root.col   = 0;
            roomRect_root.width = mapCol;
            roomRect_root.height= mapRow;
            roomRectsBinaryTree.Add(roomRect_root);//NOTE(용택): 계산 편의를 위해 0번은 더미, 1번은 루트 노드다.

            int currentLevel = 1;
            while (currentLevel < treeLevel)
            {
                int nodeIndexStart = (int)MathF.Pow(2.0f, currentLevel - 1);
                int nodeIndexEnd   = (int)MathF.Pow(2.0f, currentLevel);
                for (int nodeIndex = nodeIndexStart; nodeIndex < nodeIndexEnd; ++nodeIndex)
                {
                    RoomRect divided_1 = new RoomRect();
                    RoomRect divided_2 = new RoomRect();

                    //NOTE(용택): 랜덤으로 하는 것을 고려. 생성된 모양이 나빠질 수 있긴 하다.
                    int divideMode = currentLevel % 2;
                    if (divideMode == 0)
                    {
                        DivideVertically(roomRectsBinaryTree[nodeIndex], minRatio, maxRatio, ref divided_1, ref divided_2);
                    }
                    else
                    {
                        DivideHorizontally(roomRectsBinaryTree[nodeIndex], minRatio, maxRatio, ref divided_1, ref divided_2);
                    }

                    roomRectsBinaryTree.Add(divided_1);
                    roomRectsBinaryTree.Add(divided_2);
                }

                currentLevel += 1;
            }

            return roomRectsBinaryTree;
        }
        #endregion //BSP Tree Functions
        #region Room Creation Functions
        static private RoomRect FillRoomRect(RoomRect roomRect, float minRatio, float maxRatio)
        {
            RoomRect filled = roomRect;

            //TODO(용택): 이후 Unity 랜덤으로 대체.
            Random random = new Random();
            float randomRatio = random.Next( (int)(minRatio*100), (int)(maxRatio*100) ) / 100.0f;

            int randomWidth  = (int)(roomRect.width * randomRatio);
            int randomHeight = (int)(roomRect.height * randomRatio);

            //NOTE(용택): int * float 암시적 캐스팅으로 0 이 될 수 있다. 최소 1은 유지해주도록 한다.
            filled.width  = (randomWidth > 0) ? randomWidth : 1;
            filled.height = (randomHeight > 0) ? randomHeight : 1;

            int maxRow = roomRect.row + (roomRect.height - randomHeight);
            int maxCol = roomRect.col + (roomRect.width - randomWidth);

            filled.row = random.Next(roomRect.row, maxRow);
            filled.col = random.Next(roomRect.col, maxCol);

            return filled;
        }
        static private List<Room> CreateRooms(List<RoomRect> roomRectBinaryTree, int treeLevel, int fillCount, float minFillRatio, float maxFillRatio, int discardWidth, int discardHeight)
        {
            //NOTE(용택): 최대 레벨의 노드 외에는 실제 맵 적용에 필요없다.
            int leafNodeIndexStart = (int)MathF.Pow(2.0f, treeLevel - 1);
            int numberOfLeafNodes  = leafNodeIndexStart;

            List<RoomRect> leaves = roomRectBinaryTree.GetRange(leafNodeIndexStart, numberOfLeafNodes);
            List<Room>     rooms  = new List<Room>(leaves.Count);

            for (int i = 0; i < numberOfLeafNodes; ++i)
            {
                if (leaves[i].row < discardHeight * maxFillRatio ||
                    leaves[i].col < discardWidth * maxFillRatio)
                {
                    continue;
                }

                rooms.Add(new Room(fillCount));

                int fillLoopCount = 0;
                while (fillLoopCount < fillCount)
                {
                    RoomRect filled = FillRoomRect(leaves[i], minFillRatio, maxFillRatio);

                    if (filled.row < discardHeight || filled.col < discardWidth)
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

                //TODO(용택): Connectivity 가 보장되지 않으면 Discard.
                //            이건 생각해본다. Rect-shaped Path 를 만들 수도 있다.
            }

            return rooms;
        }

        static private void MergeConnectiveRooms(List<Room> rooms)
        {
            //TODO(용택): 인접하고 통로가 이미 있는 방의 경우는 Merge
            
            //NOTE(용택): 최대 5개(타겟+4방향)이 머지가 가능할 수 있다. 최소는 0 (=없음)
            //          머지 가능 여부는 세로방향을 예로 row의 min/max 를 조사, 차이가 1이면 가능하다.
            //          이건 필터조건이고, 실계산은 위 조건을 만족시 col[] 을 조사해야한다.
            //          RowMajor Sort, ColumnMajor Sort 를 통해 어느정도 속도업을 노릴 수 있다. (n^2, n^4 를 피할 수 있다.)
            //          이미 필터조건에서 걸러지기 때문에 naive implementation 도 나쁘지는 않을 것 같다.
            //NOTE(용택): 그래프 연결 작업은 이 이후에 되어야 한다.

            Console.Write("\nRows: ");
            rooms.Sort(Room.RowMajorComparison);
            foreach(Room room in rooms)
            {
                Console.Write( room.MinRow + " ");
            }

            Console.Write("\nColumns: ");
            rooms.Sort(Room.ColumnMajorComparison);
            foreach(Room room in rooms)
            {
                Console.Write( room.MinCol + " ");
            }
        }
        static private List<int> GetDistanceSortedRoomIds(int targetRoomId, List<Room> rooms)
        {
            Room targetRoom = Room.FindRoom(targetRoomId, rooms);

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
        
        //TODO(용택): 세분화해서 나누지 않고, MapGenerator 클래스에서 다 만들어서 넘긴다.
        //           어차피 List<Room>, List<Path> 는 Map 이 Hold 하는게 맞다.
        //           덩치가 커지면 Partial 클래스를 고려한다.

        //TODO(용택): SelectDiscardingRooms();      // --DiscardRooms() ? --Consecutive 노드를 하나씩은 보존하도록 한다. (되도록, 이미 Room 단계에서 버려질 수 있다.)
        //TODO(용택): SelectEntranceRoom();         // --multiple Entrance & multiple Exit 을 고려하자. 복층구조에 용이할 것 같다.
        //TODO(용택): SelectExitRoom();             // --경우에 따라 Entrance 와 같을 수 있다. --multiple exit 을 고려하자.

        //TODO(용택): CreatePath();
        //
        //static private void 
    }
}