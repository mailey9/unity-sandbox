using System.Text;

namespace minorlife
{
    public class MapGenerateConfig
    {
        public int width = 200;
        public int height = 200;

        public int divideTreeLevel = 5;
        public float divideRatioMin = 0.35f;
        public float divideRatioMax = 0.50f;

        public int rectFillCount = 3;
        public float rectFillRatioMin = 0.35f;
        public float rectFillRatioMax = 0.80f;

        public int discardLessThanWidth = 5;
        public int discardLessThanHeight = 5;
        //TODO(용택): 가로-세로 비율이 편향된 모양을 버린다.
        //discardLessThanWidthHeightRatio = 0.40f;

        public int corridorBreadth = 3;

        public override string ToString()
        {
            int capa = 20 * 11 + 11;
            StringBuilder stringBuilder = new StringBuilder(capa);

            stringBuilder.Append("Width: " + width + "\n");
            stringBuilder.Append("Height: " + height + "\n");
            stringBuilder.Append("Divide Level: " + divideTreeLevel + "\n");
            stringBuilder.Append("Divide Ratio Min: " + divideRatioMin + "\n");
            stringBuilder.Append("Divide Ratio Max: " + divideRatioMax + "\n");
            stringBuilder.Append("Rect Fill Count: " + rectFillCount + "\n");
            stringBuilder.Append("Rect Fill Ratio Min: " + rectFillRatioMin + "\n");
            stringBuilder.Append("Rect Fill Ratio Max: " + rectFillRatioMax + "\n");
            stringBuilder.Append("Discard Less Than Width: " + discardLessThanWidth + "\n");
            stringBuilder.Append("Discard Less Than Height: " + discardLessThanHeight + "\n");
            stringBuilder.Append("Corridor Breadth: " + corridorBreadth + "\n");

            return stringBuilder.ToString();
        }
    }
}