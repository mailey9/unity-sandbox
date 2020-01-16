namespace minorlife
{
    public enum eDirection
    {
        None            = 0,

        Up              = 1,
        Down            = 2,
        Left            = 4,
        Right           = 8,

        UpLeft          = 5,
        UpRight         = 9,
        DownLeft        = 6,
        DownRight       = 10,

        UpDown          = 3,
        LeftRight       = 12,

        UpDownLeft      = 7,
        UpDownRight     = 11,
        UpLeftRight     = 13,
        DownLeftRight   = 14,

        UpDownLeftRight = 15,//Up | Down | Left | Right     //okay!
    }
}