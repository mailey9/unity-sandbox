using System.Collections.Generic;

namespace minorlife
{
    public struct GraphMatrix
    {
        public int LengthRow { get{return graphMatrix.GetLength(0);} }
        public int LengthColumn { get{return graphMatrix.GetLength(0);} }

        public int[]   lookupTable;
        public int[,]  graphMatrix;

        //TODO(용택): (GraphMatrix) 다음 인터페이스 구현

        //public bool IsConnected(int nodeA, int nodeB);
        //public int GetCost(int nodeA, int nodeB);

        //private int GetIndexOf(int id);
    }
}
