//NOTE(용택): Room = Node(Vertex) , Corridor = Edge 로 보는 그래프다.
//          (1) 각 노드 N 개가 N-1 개의 엣지를 갖는 완전 그래프라고 가정하자. (각 중점에 대해 거리를 구할 수 있으므로)
//          (2) 이제 MST를 구할 수 있다.
//              -- 여기서 노드는 좌표쌍으로 표현가능하므로 Euclidean Minimum Spanning Tree (EMST) 이다.
//                즉, 문제는 "EMST 를 어떻게 빠르게 구할 것인가?" 이다.
//              -- 관련된 논문들도 많이있고, 병렬화에 대한 전략도 정말 많이 연구된 분야 중 하나.
//          (3) Kruskal, Prim, Boruvka 에 더해서, Delaunay Triangluation (들로네 삼각분할) 법이 있다.
//              -- 이 중 들로네가 EMST 를 찾는데 가장 빠르다. 위 (1) 에서의 과정이 필요 없기 때문
//              -- 모두 Greedy Approach 이기 때문에 모양이 조금씩 다를 수 있다. (깥은 비용이 발생하는 엣지에 대해)
//                유클리디언 거리를 이용하면 그렇지만, 맨하튼 거리를 이용하면 Kruskal 과 Prim 의 결과가 다를 수 있음.
//          --> 우선 Kruskal 로 간다.
//                  Prim vs Kruskal 은 엣지수가 많은 Dense Graph (이 경우에는 Complete Graph 이니) Prim 쪽이 선호되고, 빠르다.
//                  (나중에 트윅에 이용 되거나, 비용을 들고있음으로 생기는 장점이 있다. -- 숏컷을 만드는 컨텐츠라던가.)
//                  따라서 <distance, vertex> set 에서부터 시작한다.

using System;
using System.Collections.Generic;

namespace minorlife
{
    public class MapGenerator
    {
        static int DEBUG_generateRunCount = 0;
        static public List<Room> Generate(MapGenerateConfig config)
        {
            //NOTE(용택): Width * Height 로 사각형을 만들고, BSP로 사각형을 분할한다.
            //TODO(용택): 여기서 Map 을 반환할 것이므로, 함수를 좀 더 잘게 나눈다., 스택 용량은 생각하지 않는다.
            List<Rect> rectsBinaryTree = CreateDividedRectBinaryTree(config.Height,
                                                                     config.Width,
                                                                     config.DivideLevel,
                                                                     config.DivideRatioMin,
                                                                     config.DivideRatioMax);
            //NOTE(용택): 분할된 공간의 leaf 노드들만 남기고 버린다.
            //TODO(용택): 함수 분리, DiscardLeafNodes();

            //NOTE(용택): 각 공간을 일정횟수로 랜덤하게 채워넣는다. 과정 중 일정 너비/높이 이하는 버린다.
            List<Room> rooms = CreateRooms(rectsBinaryTree,
                                           config.DivideLevel,
                                           config.RectFillCount,
                                           config.RectFillRatioMin,
                                           config.RectFillRatioMax,
                                           config.DiscardLessThanWidth,
                                           config.DiscardLessThanHeight);

            //NOTE(용택): 좌표상 겹치는 공간은 하나의 방으로 취급하도록 합친다.
            rooms = MergeConsecutiveRooms(rooms);

            //TODO(용택): Width/Height 비율이 나쁠 때 버리는 것을 고려.
            //NOTE(용택): 비율이 나쁠 때 해당 공간을 버린다.
            //rooms = DiscardUglyRooms(rooms);

            //NOTE(용택): 후에 BinSearch 에 용이하도록 ID로 정렬해 둔다.
            //NOTE(용택): 사실 id 부여방식이 0부터 ++ 기 때문에 오름차순이 되긴해서 굳이 필요없긴 하다.
            rooms.Sort(Room.IdComparison);

            GraphMatrix completeGraphMatrix;
            GraphMatrix manhattanMSTMatrix;
            List<ManhattanEdge> createdEdges;
            CreateRoomGraphs(rooms, out completeGraphMatrix, out manhattanMSTMatrix, out createdEdges);

            {
                Console.WriteLine("---- Complete Matrix ----");
                Console.Write("Lookup Index: ");
                for (int i = 0; i < completeGraphMatrix.lookupTable.Length; ++i)
                    Console.Write("{0}", i.ToString().PadLeft(3));
                Console.WriteLine("");

                for (int r = 0; r < completeGraphMatrix.LengthRow; ++r)
                {
                    for (int c = 0; c < completeGraphMatrix.LengthColumn; ++c)
                        Console.Write("{0}", completeGraphMatrix.graphMatrix[r, c].ToString().PadLeft(6) );
                    Console.Write("\n");
                }
                Console.WriteLine("---- MST Matrix ----");
                for (int r = 0; r < manhattanMSTMatrix.LengthRow; ++r)
                {
                    for (int c = 0; c < manhattanMSTMatrix.LengthColumn; ++c)
                        Console.Write("{0}", manhattanMSTMatrix.graphMatrix[r, c].ToString().PadLeft(6) );
                    Console.Write("\n");
                }
                Console.WriteLine("---- ---- ---- ----");
            }

            //TODO(용택): SelectEntranceRoom();         // --multiple Entrance & multiple Exit 을 고려하자. 복층구조에 용이할 것 같다.
            //int entranceRoomId = SelectEntranceRoom(rooms);
            //TODO(용택): SelectExitRoom();             // --경우에 따라 Entrance 와 같을 수 있다. --multiple exit 을 고려하자.
            //int exitRoomId     = SelectExitRoom(rooms, entranceRoomId); //NOTE(용택): 입구에 대한 의존성이 있으니 직접넘긴다.

            //TODO(용택): 통로를 만든다. Room1, Room2 에서 Edge 로 표현될 좌표를 선택하고, Manhattan 어프로치로 경로를 찾는다., Rectangle-Shape 가 될 것이다.
            //TODO(용택): 이미 있는 Corridor 와 좌표가 겹치는 경우에 대해 고려한다.
            //통로를 만든다.
            //List<Corridor> corridors = CreateCorridors(edgeMatrix);

            //TODO(용택): 위를 정리해서 내보낸다.
            //Room 정보와 Corridor 정보를 이용해 Map을 만들고, 세팅한 후 리턴.

            //NOTE(용택): Thought, Consecutive 여부를 판단할 때, BT Sibling 이면 Consecutive 다. 그냥 거리로 하는 게 낫겠다.
            //           거리로 소팅한다. 대상과의 거리를 알 수 있다. 그래프를 어떻게 그릴 것인가?

            DEBUG_generateRunCount += 1;
            return rooms;
        }

        private static void CreateRoomGraphs(List<Room> rooms, out GraphMatrix completeGraphMatrix, out GraphMatrix manhattanMSTMatrix, out List<ManhattanEdge> createdEdges)
        {
            completeGraphMatrix = new GraphMatrix();
            manhattanMSTMatrix  = new GraphMatrix();

            //NOTE(용택): 룩업테이블
            int[] lookupTable = new int[rooms.Count];
            for (int i = 0; i < lookupTable.Length; ++i)
            {
                lookupTable[i] = rooms[i].Id;
            }
            completeGraphMatrix.lookupTable = lookupTable;
            manhattanMSTMatrix.lookupTable = lookupTable;

            //NOTE(용택): 그래프생성 및 초기화
            completeGraphMatrix.graphMatrix = new int[rooms.Count, rooms.Count];// 비용을 전부 들고 있는 완전그래프
            manhattanMSTMatrix.graphMatrix  = new int[rooms.Count, rooms.Count];// 낮은 비용만 순서쌍으로 연결해서 표현할 Sparse 그래프

            List<ManhattanEdge> manhattanEdges = new List<ManhattanEdge>();// 거리로 정렬할 노드쌍 { node_a, node_b, distance(=weight) }
            for (int r = 0; r < rooms.Count; ++r)
            {
                for (int c = 0; c < rooms.Count; ++c)
                {
                    if (r == c)
                    {
                        completeGraphMatrix.graphMatrix[r, c] = 0;
                        manhattanMSTMatrix.graphMatrix[r, c]  = 0;
                    }
                    else
                    {
                        int distance = rooms[r].CalculateManhattanDistance(rooms[c]);
                        manhattanEdges.Add( new ManhattanEdge(rooms[r].Id, rooms[c].Id, distance) );

                        completeGraphMatrix.graphMatrix[r, c] = distance;
                        manhattanMSTMatrix.graphMatrix[r, c]  = 0;
                    }
                }
            }

            manhattanEdges.Sort(ManhattanEdge.DistanceComparison);

            int desiredEdgeCount = rooms.Count - 1;
            createdEdges = new List<ManhattanEdge>(desiredEdgeCount);
            //var manhattanEdges_enumerator = manhattanEdges.GetEnumerator();
            //manhattanEdges_enumerator.MoveNext();//NOTE(용택): 첫 번째는 더미라서, Next 를 한번은 때려줘야 한다.
            int index = 0;
            while (createdEdges.Count < desiredEdgeCount)
            {
                //ManhattanEdge shortestEdge = manhattanEdges_enumerator.Current;
                ManhattanEdge shortestEdge = manhattanEdges[index];

                //TODO(용택): (Kruskal) SortedSet<> 의 CompareTo() 구현이 생각보다 어려워서 관계된 커스텀 MultiSet 을 만드는 게 쉬울 것 같다.
                //TODO(용택): (Kruskal) 작은 쪽에서 선형탐색을 하니 오래 걸리진 않겠지만.. 마음이 찜찜하다 ㅜ
                if ( createdEdges.Contains(shortestEdge) == false )
                { 
                    createdEdges.Add(shortestEdge);
                    
                    int indexOfA = Array.BinarySearch(manhattanMSTMatrix.lookupTable, shortestEdge.NodeA);
                    int indexOfB = Array.BinarySearch(manhattanMSTMatrix.lookupTable, shortestEdge.NodeB);
                    manhattanMSTMatrix.graphMatrix[indexOfA, indexOfB] = shortestEdge.ManhattanDistance;
                    manhattanMSTMatrix.graphMatrix[indexOfB, indexOfA] = shortestEdge.ManhattanDistance;
                }

                //manhattanEdges_enumerator.MoveNext();
                index += 1;
            }

            foreach (ManhattanEdge i in createdEdges)
            {
                Console.WriteLine( i.ToString() );
            }
        }

        private static List<Corridor> CreateCorridors(int[,] edgeMatrix)
        {
            //MST를 만든다?
            
            //SortedSet<VertexPairByDisance>

            throw new NotImplementedException();
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

        //TODO(용택): (DistanceSorted) Obsolote 시킨다. 맨하탄으로 계산하고, 노드 weight 계산 시 반환한다.
        [ObsoleteAttribute()]
        static private List<int> GetDistanceSqSortedRoomIds(int targetRoomId, List<Room> rooms)
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
    }
}