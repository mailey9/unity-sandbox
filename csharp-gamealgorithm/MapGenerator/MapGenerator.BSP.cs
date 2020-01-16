using System;
using System.Collections.Generic;

namespace minorlife
{
    public static partial class MapGenerator
    {
        private class RectPair
        {
            public Rect a;
            public Rect b;
            public RectPair(Rect a, Rect b) { this.a = a; this.b = b; }
        }

        private static RectPair DivideVertically(Rect rect, float minRatio, float maxRatio)
        {
            float randomRatio = Rand.Range(minRatio, maxRatio);
            int randomHeight = (int)(rect.height * randomRatio);

            Rect a = new Rect(rect.x, rect.y, rect.width, randomHeight);
            Rect b = new Rect(rect.x, rect.y + randomHeight, rect.width, rect.height - randomHeight);
            return new RectPair(a, b);
        }
        private static RectPair DivideHorizontally(Rect rect, float minRatio, float maxRatio)
        {
            float randomRatio = Rand.Range(minRatio, maxRatio);
            int randomWidth = (int)(rect.width * randomRatio);

            Rect a = new Rect(rect.x, rect.y, randomWidth, rect.height);
            Rect b = new Rect(rect.x + randomWidth, rect.y, rect.width - randomWidth, rect.height);
            return new RectPair(a, b);
        }

        private static List<Rect> CreateBSPTree(int width, int height, int treeLevel, float minRatio, float maxRatio)
        {
            //NOTE(용택): 배열표현 이진트리를 이용한다.
            int capacity = (int)MathF.Pow(2.0f, treeLevel);

            List<Rect> bspTree = new List<Rect>(capacity);
            bspTree.Add(new Rect(-1, -1, -1, -1));     //[0]은 더미노드 (이용X)
            bspTree.Add(new Rect(0, 0, width, height));//[1]은 루트노드 (Child 계산편의)

            int currentLevel = 1;
            while (currentLevel < treeLevel)
            {
                int startIndexOfLevel = (int)MathF.Pow(2.0f, currentLevel - 1);
                int endIndexOfLevel = (int)MathF.Pow(2.0f, currentLevel);

                int divideMode = currentLevel % 2;
                for (int i = startIndexOfLevel; i < endIndexOfLevel; ++i)
                {
                    RectPair divideResult = (divideMode == 0)
                        ? DivideVertically(bspTree[i], minRatio, maxRatio)
                        : DivideHorizontally(bspTree[i], minRatio, maxRatio);

                    bspTree.Add(divideResult.a);
                    bspTree.Add(divideResult.b);
                }

                currentLevel += 1;
            }

            return bspTree;
        }

        private static List<Rect> GetLeafNodes(List<Rect> bspTree, int treeLevel)
        {
            int startIndexOfLeafNodeStart = (int)MathF.Pow(2.0f, treeLevel - 1);
            int countOfLeafNodes = startIndexOfLeafNodeStart;//같다
            return bspTree.GetRange(startIndexOfLeafNodeStart, countOfLeafNodes);
        }
    }
}