//NOTE(용택): System.Random Wrapper
//          시드값 조절로 랜덤통제, 디버깅용

namespace minorlife
{
    public static class Rand
    {
        const int RANDOM_SEED = 1988 - 4 - 15 + 42;
        static System.Random random = new System.Random(RANDOM_SEED);

        public static float Range(float inclusiveMin, float inclusiveMax)
        {
            return random.Next((int)(inclusiveMin * 100), (int)(inclusiveMax * 100)) / 100.0f;
        }

        public static int Range(int inclusiveMin, int exclusiveMax)
        {
            return random.Next(inclusiveMin, exclusiveMax);
        }
    }
}