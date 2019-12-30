using System;
using System.Collections.Generic;

namespace minorlife
{
    public class MapGenerator
    {
        static public List<Room> Generate(MapGenerateConfig config)
        {
            List<RoomRect> roomRectsBinaryTree = CreateDividedRooms(config.Height,
                                                                    config.Width,
                                                                    config.DivideRatioMin,
                                                                    config.DivideRatioMax,
                                                                    config.DivideLevel);

            //NOTE(용택): 최대 레벨의 노드 외에는 실제 맵 적용에 필요없다.
            int leafNodeIndexStart = (int)Math.Pow(2.0, config.DivideLevel - 1);
            int numberOfLeafNodes  = leafNodeIndexStart;

            List<RoomRect> leaves = roomRectsBinaryTree.GetRange(leafNodeIndexStart, numberOfLeafNodes);
            List<Room> rooms = new List<Room>(leaves.Count);
            for (int i = 0; i < numberOfLeafNodes; ++i)
            {
                //TODO(용택): leaf 노드의 크기가 일정 이하면 Discard
                if (leaves[i].row < config.DiscardLessThanHeight * config.RoomRectFillRatioMax ||
                    leaves[i].col < config.DiscardLessThanWidth * config.RoomRectFillRatioMax)
                {
                    continue;
                }

                rooms.Add(new Room(config.RoomRectFillCount));

                int fillCount = 0;
                while (fillCount < config.RoomRectFillCount)
                {
                    RoomRect filled = FillRoom(leaves[i], config.RoomRectFillRatioMin, config.RoomRectFillRatioMax);

                    if (filled.row < config.DiscardLessThanHeight || filled.col < config.DiscardLessThanWidth)
                    {
                        continue;
                    }

                    if (fillCount > 0)//rooms[i].RectCount > 0
                    {
                        if (rooms[rooms.Count-1].HasIntersection(filled) == false)
                        {
                            continue;
                        }
                    }
                    rooms[rooms.Count-1].Append(filled);
                    fillCount += 1;
                }

                //TODO(용택): Connectivity 가 보장되지 않으면 Discard.
            }

            return rooms;
        }

        static private RoomRect FillRoom(RoomRect roomRect, float minRatio, float maxRatio)
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

        static private List<RoomRect> CreateDividedRooms(int mapRow, int mapCol, float minRatio, float maxRatio, int treeLevel)
        {
            //NOTE(용택): ArrayReprsentation BinaryTree 를 이용한다. (단, 계산하고 버려지므로 성능상의 큰 이점은 없을 것이다.)
            int roomCapa = (int)Math.Pow(2.0, treeLevel);
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
                int nodeIndexStart = (int)Math.Pow(2.0, currentLevel - 1);
                int nodeIndexEnd   = (int)Math.Pow(2.0, currentLevel);
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
        
    }
}