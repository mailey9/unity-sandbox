namespace minorlife
{
    public struct MapGenerateConfig
    {
        public int Width {get; set;}
        public int Height {get; set;}

        public int DivideLevel {get; set;}
        public float DivideRatio_Min {get; set;}
        public float DivideRatio_Max {get; set;}

        public float RoomRect_FillRatio_Min {get; set;}
        public float RoomRect_FillRatio_Max {get; set;}

        public static MapGenerateConfig DEFAULT
        {
            get
            {
                MapGenerateConfig defaultConfig = new MapGenerateConfig();
                defaultConfig.Width  = 80;
                defaultConfig.Height = 50;
                defaultConfig.DivideLevel = 4;
                defaultConfig.DivideRatio_Min = 0.25f;
                defaultConfig.DivideRatio_Max = 0.45f;
                defaultConfig.RoomRect_FillRatio_Min = 0.45f;
                defaultConfig.RoomRect_FillRatio_Max = 0.80f;

                return defaultConfig;
            }
        }
    }
}