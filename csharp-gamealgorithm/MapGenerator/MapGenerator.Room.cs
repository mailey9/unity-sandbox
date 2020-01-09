using System;
using System.Collections.Generic;

namespace minorlife
{
    public static partial class MapGenerator
    {
        private static Rect GenerateRandomSizeRectFrom(Rect original, float minRatio, float maxRatio)
        {
            //NOTE(용택): 원본 Rect 로 부터 min ~ max % 사이 비율의 Rect 를 만든다.
            float randomRatio = Rand.Range(minRatio, maxRatio);

            int newWidth = (int)(original.width * randomRatio);
            int newHeight = (int)(original.height * randomRatio);

            //NOTE(용택): width 가 10->8 이 되었다면, 새로운 x는 +2 만큼 될 수 있다.
            int possibleMaxX = original.x + (original.width - newWidth);
            int possibleMaxY = original.y + (original.height - newHeight);

            Rect returnRect = new Rect();
            returnRect.x      = Rand.Range(original.x, possibleMaxX + 1);
            returnRect.y      = Rand.Range(original.y, possibleMaxY + 1);
            returnRect.width  = newWidth;
            returnRect.height = newHeight;
            return returnRect;
        }

        private static List<Room> CreateRooms(List<Rect> leafNodes, int rectGenCount, float minSizeRatio, float maxSizeRatio, int discardWidth, int discardHeight)
        {
            List<Room> rooms = new List<Room>(leafNodes.Count);
            for (int i = 0; i < leafNodes.Count; ++i)
            {
                //TODO(용택): (CreateRooms) 시도하지 않아도 되는 크기면 버린다. 로직 체크.
                if (leafNodes[i].width < discardWidth || leafNodes[i].height < discardHeight)
                    continue;
                if (leafNodes[i].width * maxSizeRatio < discardWidth ||
                    leafNodes[i].height * maxSizeRatio < discardHeight)
                {
                    var maxPossibleWidth = leafNodes[i].width * maxSizeRatio;
                    var maxPossibleHeight = leafNodes[i].height * maxSizeRatio;
                    Console.WriteLine("should be discarded, maxW:{0}, maxH:{1}", maxPossibleWidth, maxPossibleHeight);
                    continue;
                }

                rooms.Add(new Room(rectGenCount));

                int loopCount = 0;
                while (loopCount < rectGenCount)
                {
                    Rect gen = GenerateRandomSizeRectFrom(leafNodes[i], minSizeRatio, maxSizeRatio);

                    if (gen.width < discardWidth || gen.height < discardHeight)
                        continue;

                    if (loopCount > 0)
                    {
                        if (rooms[rooms.Count-1].HasIntersection(gen) == false)
                            continue;
                    }

                    rooms[rooms.Count-1].Append(gen);
                    loopCount += 1;
                }
            }
            return rooms;
        }

        private static List<Room> MergeAdjacentRooms(List<Room> rooms)
        {
            for (int a = 0; a < rooms.Count; ++a)
            {
                for (int b = 0; b < rooms.Count; ++b)
                {
                    if (a == b)
                        continue;

                    if (Room.CanMerge(rooms[a], rooms[b]) == true)
                    {
                        System.Diagnostics.Debug.Assert(a < b, "?! possible ?!");
                        //mergingParis.Add(new RoomPair(rooms[a], rooms[b]));
                        
                        rooms[a].Append(rooms[b]);
                        rooms.RemoveAt(b);

                        a = -1;
                        break;
                    }
                }
            }

            return rooms;
        }

        //TODO(용택): 최소크기를 넘기더라도, 비율이 나쁜 Room 을 버리는 것을 고려
        //private static List<Room> DiscardUglyRooms(List<Room> rooms);
    }
}