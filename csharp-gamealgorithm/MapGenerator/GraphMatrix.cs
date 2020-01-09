using System;

namespace minorlife
{
    public class GraphMatrix
    {
        public int LengthRow { get { return graphMatrix.GetLength(0); } }
        public int LengthColumn { get { return graphMatrix.GetLength(0); } }

        public UInt64[]  lookupTable = null;
        public int[,] graphMatrix = null;

        public bool GetShortestNode(UInt64 nodeFrom, out UInt64? nodeTo, out int? cost)
        {
            int indexFrom = GetIndexOf(nodeFrom);
            int indexTo = -1;

            int lowestCost = int.MaxValue;

            for (int col = 0; col < graphMatrix.GetLength(1); ++col)
            {
                int comparingCost = graphMatrix[indexFrom, col];
                if (comparingCost != 0)//NOTE(용택): 0은 경로없음.
                {
                    if (comparingCost < lowestCost)
                    {
                        lowestCost = comparingCost;
                        indexTo = col;
                    }
                }
            }

            if (lowestCost != int.MaxValue && indexTo != -1)
            {
                cost = lowestCost;
                nodeTo = GetNodeIdOf(indexTo);
                return true;
            }

            cost = null;
            nodeTo = null;
            return false;
        }
        public bool HasPath(UInt64 nodeA, UInt64 nodeB)
        {
            return GetCost(nodeA, nodeB) == 0 ? false : true;
        }
        public int GetCost(UInt64 nodeA, UInt64 nodeB)
        {
            int indexA = GetIndexOf(nodeA);
            int indexB = GetIndexOf(nodeB);

            System.Diagnostics.Debug.Assert(graphMatrix[indexA, indexB] == graphMatrix[indexB, indexA], "graphMatrix is not Bidirectional, Intended?");
            return graphMatrix[indexA, indexB];
        }
        public bool ContainsIsolateNode()
        {
            for (int row = 0; row < graphMatrix.GetLength(0); ++row)
            {
                int sum = 0;
                for (int col = 0; col < graphMatrix.GetLength(1); ++col)
                    sum += graphMatrix[row, col];

                if (sum == 0)
                    return true;
            }

            return false;
        }
        public int GetIndexOf(UInt64 id)
        {
            return Array.BinarySearch(lookupTable, id);
        }
        public UInt64 GetNodeIdOf(int index)
        {
            return lookupTable[index];
        }
    
        public override string ToString()
        {
            int capa = LengthRow * LengthColumn;
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(capa);
            return stringBuilder.ToString();
        }
    }
}
