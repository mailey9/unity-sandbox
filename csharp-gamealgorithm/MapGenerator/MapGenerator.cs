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
    public static partial class MapGenerator
    {
        static int DEBUG_generateRunCount = 0;
        static public Map.GeneratedMap Generate(MapGenerateConfig config)
        {
            Map.eTile[,] tilemap = CreateTileArray2D(config.width, config.height);

            List<Rect> bspTree = CreateBSPTree(config.width, config.height,
                                               config.divideTreeLevel,
                                               config.divideRatioMin, config.divideRatioMax);
            List<Rect> bspLeaves = GetLeafNodes(bspTree, config.divideTreeLevel);

            List<Room> rooms = CreateRooms(bspLeaves,
                                           config.rectFillCount, config.rectFillRatioMin, config.rectFillRatioMax,
                                           config.discardLessThanWidth, config.discardLessThanHeight);

            //NOTE(용택): 좌표상 겹치는 공간은 하나의 방으로 취급하도록 합친다.
                    int DEBUG_beforeMerged = rooms.Count;
            rooms = MergeAdjacentRooms(rooms);
                    int DEBUG_afterMerged = rooms.Count;
            if (DEBUG_beforeMerged != DEBUG_afterMerged)
            {
                Console.WriteLine("Room Merged: before(" + DEBUG_beforeMerged + "), after(" + DEBUG_afterMerged + ") <===" + DEBUG_generateRunCount);
            }

            //NOTE(용택): 후에 BinSearch 에 용이하도록 ID로 정렬해 둔다.
            //NOTE(용택): 사실 id 부여방식이 0부터 ++ 기 때문에 오름차순이 되긴해서 굳이 필요없긴 하다.
            rooms.Sort(Room.IdComparison);

            tilemap = ApplyRooms(tilemap, rooms);
            tilemap = ApplyRoomWalls(tilemap, rooms);

            
            {//teststart
                var corridors = CreateCorridors(tilemap, rooms, 5);
                foreach (var c in corridors)
                {
                    tilemap[c.DoorPointA.y, c.DoorPointA.x] = Map.eTile.RoomDoor;
                    tilemap[c.DoorPointB.y, c.DoorPointB.x] = Map.eTile.RoomDoor;
                    foreach (var c_way in c.GetCorridorPoints())
                    {
                        tilemap[c_way.y, c_way.x] = Map.eTile.Corridor;
                    }
                    foreach (var c_walls in c.GetWallPoints())
                    {
                        tilemap[c_walls.y, c_walls.x] = Map.eTile.CorridorWall;
                    }
                }
            }//testend

            //GraphMatrix completeGraphMatrix;
            //GraphMatrix manhattanMSTMatrix;
            //List<ManhattanEdge> mstEdges;
            //TODO(용택): 길을 몇 개 추가해서 생성한다.
            //CreateRoomGraphs(rooms, out completeGraphMatrix, out manhattanMSTMatrix, out mstEdges);

            //TODO(용택): SelectEntranceRoom();         // --multiple Entrance & multiple Exit 을 고려하자. 복층구조에 용이할 것 같다.
            //int entranceRoomId = SelectEntranceRoom(rooms);
            //TODO(용택): SelectExitRoom();             // --경우에 따라 Entrance 와 같을 수 있다. --multiple exit 을 고려하자.
            //int exitRoomId     = SelectExitRoom(rooms, entranceRoomId); //NOTE(용택): 입구에 대한 의존성이 있으니 직접넘긴다.

            //TODO(용택): 위를 정리해서 내보낸다.
            //Room 정보와 Corridor 정보를 이용해 Map을 만들고, 세팅한 후 리턴.
            DEBUG_generateRunCount += 1;

            Map.GeneratedMap generatedMap;
            generatedMap.tileMap = tilemap;
            generatedMap.rooms = rooms;
            generatedMap.corridors = null;// corridors;
            generatedMap.completeMatrix = null;// completeGraphMatrix;
            generatedMap.pathMatrix = null;// manhattanMSTMatrix;

            return generatedMap;
        }

        [Obsolete]
        private static void CreateRoomGraphs(List<Room> rooms, out GraphMatrix completeGraphMatrix, out GraphMatrix manhattanMSTMatrix, out List<ManhattanEdge> mstEdges)
        {
            completeGraphMatrix = new GraphMatrix();
            manhattanMSTMatrix  = new GraphMatrix();

            //NOTE(용택): 룩업테이블
            ulong[] lookupTable = new ulong[rooms.Count];
            completeGraphMatrix.lookupTable = new ulong[rooms.Count];
            manhattanMSTMatrix.lookupTable = new ulong[rooms.Count];

            for (int i = 0; i < lookupTable.Length; ++i)
            {
                lookupTable[i] = rooms[i].Id;
            }
            lookupTable.CopyTo(completeGraphMatrix.lookupTable, 0);
            lookupTable.CopyTo(manhattanMSTMatrix.lookupTable, 0);

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
                        int distance = Room.CalculateRoughManhattanDistance(rooms[r], rooms[c]);
                        manhattanEdges.Add( new ManhattanEdge(rooms[r].Id, rooms[c].Id, distance) );

                        completeGraphMatrix.graphMatrix[r, c] = distance;
                        manhattanMSTMatrix.graphMatrix[r, c]  = 0;
                    }
                }
            }

            manhattanEdges.Sort(ManhattanEdge.DistanceComparison);

            ulong[] unionFind = lookupTable;//NOTE(용택): 위에서 이미 각 Matrix 에 복사해두었으니 그대로 쓴다.
            int desiredEdgeCount = rooms.Count - 1;
            mstEdges = new List<ManhattanEdge>(desiredEdgeCount);

            int indexOfShortestEdges = 0;
            while (mstEdges.Count < desiredEdgeCount)
            {
                ManhattanEdge shortestEdge = manhattanEdges[indexOfShortestEdges];

                //NOTE(용택): (Kruskal) SortedSet<> 의 CompareTo() 구현이 생각보다 어려워서 관계된 커스텀 MultiSet 을 만드는 게 쉬울 것 같다.
                //NOTE(용택): (Kruskal) 작은 쪽에서 선형탐색을 하니 오래 걸리진 않겠지만.. 마음이 찜찜하다 ㅜ
                if ( mstEdges.Contains(shortestEdge) == false )
                {
                    int indexA = manhattanMSTMatrix.GetIndexOf(shortestEdge.NodeA);
                    int indexB = manhattanMSTMatrix.GetIndexOf(shortestEdge.NodeB);

                    //NOTE(용택): (Kruskal) Array UnionFind 를 이용한다.
                    if (unionFind[indexA] != unionFind[indexB])
                    {
                        mstEdges.Add(shortestEdge);

                        int indexOfA = Array.BinarySearch(manhattanMSTMatrix.lookupTable, shortestEdge.NodeA);
                        int indexOfB = Array.BinarySearch(manhattanMSTMatrix.lookupTable, shortestEdge.NodeB);
                        manhattanMSTMatrix.graphMatrix[indexOfA, indexOfB] = shortestEdge.ManhattanDistance;
                        manhattanMSTMatrix.graphMatrix[indexOfB, indexOfA] = shortestEdge.ManhattanDistance;

                        ulong unionFrom = unionFind[indexA];
                        for (int i = 0; i < unionFind.Length; ++i)
                        {
                            if (unionFind[i] == unionFrom)
                                unionFind[i] = unionFind[indexB];
                        }
                    }
                }

                indexOfShortestEdges += 1;
            }

            System.Diagnostics.Debug.Assert(manhattanMSTMatrix.ContainsIsolateNode() == false, "Contains Isolate Node");
        }

        static private Map.eTile[,] CreateTileArray2D(int width, int height)
        {
            Map.eTile[,] array2d = new Map.eTile[height, width];
            for (int r = 0; r < height; ++r)
            {
                for (int c = 0; c < width; ++c)
                {
                    array2d[r,c] = Map.eTile.Empty;
                }
            }
            return array2d;
        }

        static private Map.eTile[,] ApplyRooms(Map.eTile[,] tilemap, List<Room> rooms)
        {
            foreach (Room room in rooms)
            {
                for (int i = 0; i < room.RectCount; ++i)
                {
                    Rect rc = room.GetRect(i);
                    for (int y = rc.yMin; y <= rc.yMax; ++y)
                    {
                        for (int x = rc.xMin; x <= rc.xMax; ++x)
                            tilemap[y, x] = Map.eTile.Room;
                    }
                }
            }
            return tilemap;
        }
        static private Map.eTile[,] ApplyRoomWalls(Map.eTile[,] tilemap, List<Room> rooms)
        {
            foreach (Room room in rooms)
            {
                List<Point> roomHullPoints = room.GetHullPoints(tilemap);
                foreach (Point pt in roomHullPoints)
                    tilemap[pt.y, pt.x] = Map.eTile.RoomWall;
            }
            return tilemap;
        }
    }
}