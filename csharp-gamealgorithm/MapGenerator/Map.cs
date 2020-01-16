using System.Collections.Generic;
using System.Text;

namespace minorlife
{
    public class Map
    {
        //TODO(용택): (eTile) BitMask 또는 Weight 용 값 지정을 고려한다.
        public enum eTile
        {
            DEBUG           = -1,

            Empty           = 0,
            Room            = 1,
            RoomWall        = 2,
            RoomDoor        = 3,
            Corridor        = 4,
            CorridorWall    = 5,
        }

        public struct GeneratedMap
        {
            public eTile[,]         tileMap;
            public List<Room>      rooms;
            public List<Corridor>  corridors;
            public GraphMatrix     completeMatrix;
            public GraphMatrix     pathMatrix;
        }

        public int Width  { get{ return MapData.tileMap.GetLength(0); } }
        public int Height { get{ return MapData.tileMap.GetLength(1); } }

        //TODO(용택): (Map) tilemap array 이걸 층(Floor)별로 다시 2d array 를 보관하는 걸 생각해본다. arr[level][y][x];
        public GeneratedMap MapData { get; private set; }

        public Map(GeneratedMap generatedMap)
        {
            MapData = generatedMap;
            //bool rv = Setup(generatedMap); //검증?
        }

        public override string ToString()
        {
            int capa = MapData.tileMap.Length * 2 + MapData.tileMap.Length;
            StringBuilder stringBuilder = new StringBuilder(capa);

            for (int r = 0; r < Height; ++r)
            {
                for (int c = 0; c < Width; ++c)
                {
                    switch (MapData.tileMap[r,c])
                    {
                        case eTile.DEBUG:       stringBuilder.Append( "DB" ); break;
                        case eTile.Empty:       stringBuilder.Append( ". " ); break;
                        case eTile.Room:        stringBuilder.Append( "  " ); break;
                        case eTile.RoomWall:    stringBuilder.Append( "XX" ); break;
                        case eTile.RoomDoor:    stringBuilder.Append( "d " ); break;
                        case eTile.Corridor:    stringBuilder.Append( "_ " ); break;
                        case eTile.CorridorWall:stringBuilder.Append( "% " ); break;
                    }
                }
                stringBuilder.Append("\n");
            }
            return stringBuilder.ToString();
        }
    }
}