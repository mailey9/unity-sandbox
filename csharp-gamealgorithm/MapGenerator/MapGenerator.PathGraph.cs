using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace minorlife
{
    public static partial class MapGenerator
    {
        private class Path
        {
            public ulong A { get; private set; }
            public ulong B { get; private set; }
        }
        private static List<Corridor> CreateCorridors(Map.eTile[,] roomAppliedTilemap, List<Room> rooms, int samplingRange)
        {
            //NOTE(용택): 중복제거 Rect 쌍을 얻는다. --> 거리순으로 정렬한다. (오름차순) --> 복제맵에 반영하며 샘플링한다.
            Map.eTile[,] copiedTilemap = roomAppliedTilemap.Clone() as Map.eTile[,];

            HashSet<PathCandidate> pathCandidatesSet = new HashSet<PathCandidate>();
            foreach(Room a in rooms)
            {
                foreach(Room b in rooms)
                {
                    if (a == b) continue;

                    //NOTE(용택): 내부적으로 a.Rects 와 b.Rects 를 비교하기 때문에 O(n^4)다.
                    var nonDirectivePath = new PathCandidate(a, b);
                    pathCandidatesSet.Add(nonDirectivePath);
                }
            }

            //  정렬
            List<PathCandidate> sortedPathCandidates = new List<PathCandidate>(pathCandidatesSet.Count);
            foreach (PathCandidate path in pathCandidatesSet)
                sortedPathCandidates.Add(path);

            sortedPathCandidates.Sort(PathCandidate.ManhattanDistanceComparison);

            //  샘플링 시뮬레이션
            List<Corridor> corridors = new List<Corridor>(sortedPathCandidates.Count);

            foreach (PathCandidate candidate in sortedPathCandidates)
            {
                Corridor sampledCorridor = SamplePathCandidate(copiedTilemap, candidate, samplingRange);

                if (sampledCorridor != null)
                {
                    corridors.Add(sampledCorridor);

                    //  맵에 반영
                    copiedTilemap[sampledCorridor.DoorPointA.y, sampledCorridor.DoorPointA.x] = Map.eTile.RoomDoor;
                    copiedTilemap[sampledCorridor.DoorPointB.y, sampledCorridor.DoorPointB.x] = Map.eTile.RoomDoor;

                    foreach (Point pt in sampledCorridor.GetCorridorPoints())
                        copiedTilemap[pt.y, pt.x] = Map.eTile.Corridor;
                    foreach (Point pt in sampledCorridor.GetWallPoints())
                        copiedTilemap[pt.y, pt.x] = Map.eTile.CorridorWall;
                }
            }

            corridors.TrimExcess();
            return corridors;
        }

        private static Corridor SamplePathCandidate(Map.eTile[,] copiedMap, PathCandidate candidate, int samplingRange)
        {
            System.Diagnostics.Debug.Assert(samplingRange >= 5, "SamplingRange must be at least 5. (+2 for corridor walls)");

            Point doorPointA = new Point(-1, -1);
            Point doorPointB = new Point(-1, -1);
            List<Point> wallPoints = new List<Point>(candidate.ManhattanDistance * 2);
            List<Point> corridorPoints = new List<Point>(candidate.ManhattanDistance * samplingRange);

            int maxHalfRange = samplingRange / 2;
            int minHalfRange = -1 * (samplingRange - maxHalfRange - 1);

            Point samplingPoint = candidate.A;
            
            Point[] samplingBrush_Xpoints = new Point[samplingRange];
            Point[] samplingBrush_Ypoints = new Point[samplingRange];

            void UpdateBrushPoints()
            {
                int b = 0;
                for (int i = minHalfRange; i <= maxHalfRange; ++i)
                {
                    samplingBrush_Xpoints[b] = new Point(samplingPoint.x + i, samplingPoint.y);
                    samplingBrush_Ypoints[b] = new Point(samplingPoint.x, samplingPoint.y + i);
                    b += 1;
                }
            }

            //시작
            Point[] samplingVectors = candidate.GetSamplingVectorPoints();
            for (int i = 0; i < samplingVectors.Length; ++i)
            {
                samplingPoint += samplingVectors[i];
                UpdateBrushPoints();

                //NOTE(용택): 복제된 맵 copiedMap 에 대고 샘플을 시작한다.
                //          1 Room 인 경우: 샘플의 시작지점은 방 안에서 시작한다. 따라서 Room 이면 무시한다.
                //          2 RoomWall 인 경우: A to B 이므로, A 이면 경로시작점 B 이면 경로끝지점이 된다. 양 문의 후보다.
                //          3 Empty 인 경우: 경로중간점에 추가된다.
                //          4 Corridor 인 경우: 통로끼리 경로가 겹치지 않는다고 "우선 가정" 한다. --TODO(용택): 해당 통로가 B로 연결가능할 때 Okay 되는 분기처리가 필요.
                //          5 연결성테스트: 이전좌표와 연속성이 보장되어야 한다.

                switch(copiedMap[samplingPoint.y, samplingPoint.x])
                {
                    case Map.eTile.Room:
                        break;
                    case Map.eTile.RoomWall:
                        {
                            if (candidate.RoomB.Contains(samplingPoint) == true)
                            {
                                //TODO(용택): (Sample Path) 코너엣지에서 "문" 한 칸 밀어내기 분기처리 필요
                                doorPointB = new Point(samplingPoint.x, samplingPoint.y);//경로끝점
                                return new Corridor(candidate.RoomA, candidate.RoomB, doorPointA, doorPointB, corridorPoints, wallPoints);
                            }
                            else if (candidate.RoomA.Contains(samplingPoint) == true)
                            {
                                doorPointA = new Point(samplingPoint.x, samplingPoint.y);//경로시작점
                            }
                            else
                            {
                                return null;
                            }
                        }
                        break;
                    case Map.eTile.Corridor:
                        //TODO(용택): (Sample Path) 해당 통로가 B 로 연결가능할 때 Okay 되는 분기처리가 필요.
                        return null;
                    case Map.eTile.Empty:
                        {
                            //TODO(용택): (Sample Path) dirVector, CorridorWall, Corridor 분기처리문 재고려
                            if (samplingVectors[i].x == 1 || samplingVectors[i].x == -1)
                            {
                                for (int x = 0; x < samplingBrush_Xpoints.Length; ++x)
                                {

                                    if (copiedMap[samplingBrush_Ypoints[x].y, samplingBrush_Ypoints[x].x] != Map.eTile.Empty)
                                    {
                                        return null;
                                    }
                                    else
                                    {
                                        if (x == 0 || x == samplingRange - 1)//== samplingBrush_Xpoints.Length - 1
                                            wallPoints.Add(samplingBrush_Ypoints[x]);
                                        else
                                            corridorPoints.Add(samplingBrush_Ypoints[x]);
                                    }
                                }
                            }
                            if (samplingVectors[i].y == 1 || samplingVectors[i].y == -1)
                            {
                                for (int y = 0; y < samplingBrush_Ypoints.Length; ++y)
                                {

                                    if (copiedMap[samplingBrush_Xpoints[y].y, samplingBrush_Xpoints[y].x] != Map.eTile.Empty)
                                    {
                                        return null;
                                    }
                                    else
                                    {
                                        if (y == 0 || y == samplingRange - 1)//== samplingBrush_Xpoints.Length - 1
                                            wallPoints.Add(samplingBrush_Xpoints[y]);
                                        else
                                            corridorPoints.Add(samplingBrush_Xpoints[y]);
                                    }
                                }
                            }
                        }
                        break;
                }
            }//for(;;) samplingPoints

            System.Diagnostics.Debug.Assert(false, "LogicError, the implementation is must not reached THIS line.");
            return null;
        }
    }
}