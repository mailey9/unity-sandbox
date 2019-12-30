using System.Text;

namespace minorlife
{
    //TODO(용택): class 로 해서 Defualt 박아넣는게 맞는 것 같다..
    public struct MapGenerateConfig
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public int DivideLevel { get; set; }
        public float DivideRatioMin { get; set; }
        public float DivideRatioMax { get; set; }

        public int RoomRectFillCount { get; set; }
        public float RoomRectFillRatioMin { get; set; }
        public float RoomRectFillRatioMax { get; set; }

        public int DiscardLessThanWidth { get; set; }
        public int DiscardLessThanHeight { get; set; }

        public static MapGenerateConfig DEFAULT
        {
            get
            {
                MapGenerateConfig defaultConfig = new MapGenerateConfig();
                defaultConfig.Width  = 80;
                defaultConfig.Height = 50;
                defaultConfig.DivideLevel = 4;
                defaultConfig.DivideRatioMin = 0.25f;
                defaultConfig.DivideRatioMax = 0.45f;
                defaultConfig.RoomRectFillCount = 1;
                defaultConfig.RoomRectFillRatioMin = 0.45f;
                defaultConfig.RoomRectFillRatioMax = 0.80f;
                defaultConfig.DiscardLessThanWidth = 1;
                defaultConfig.DiscardLessThanHeight = 1;

                return defaultConfig;
            }
        }

        public override string ToString()
        {
            int capa = 20 * 10 + 10;
            StringBuilder stringBuilder = new StringBuilder(capa);

            stringBuilder.Append("Width: " + Width + "\n");
            stringBuilder.Append("Height: " + Height + "\n");
            stringBuilder.Append("Divide Level: " + DivideLevel + "\n");
            stringBuilder.Append("Divide Ratio Min: " + DivideRatioMin + "\n");
            stringBuilder.Append("Divide Ratio Max: " + DivideRatioMax + "\n");
            stringBuilder.Append("RoomRect Fill Count: " + RoomRectFillCount + "\n");
            stringBuilder.Append("RoomRect Fill Ratio Min: " + RoomRectFillRatioMin + "\n");
            stringBuilder.Append("RoomRect Fill Ratio Max: " + RoomRectFillRatioMax + "\n");
            stringBuilder.Append("Discard Less Than Width: " + DiscardLessThanWidth + "\n");
            stringBuilder.Append("Discard Less Than Height: " + DiscardLessThanHeight + "\n");

            return stringBuilder.ToString();
        }
    }
}