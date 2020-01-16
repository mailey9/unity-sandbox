using System;

namespace minorlife
{
    public class PathGraph
    {
        int[,] graphMatrix;
        ulong[] lookupTable;

        public PathGraph(int[,] graphMatrix, ulong[] lookupTable)
        {
            this.graphMatrix = graphMatrix;
            this.lookupTable = lookupTable;
            Validate();
        }
        public bool HasPath(ulong from, ulong to)
        {
            return GetCost(from, to) == 0 ? false : true;
        }
        public int GetCost(ulong from, ulong to)
        {
            int indexFrom = GetIndexOfNodeId(from);
            int indexTo = GetIndexOfNodeId(to);

            return graphMatrix[indexFrom, indexTo];
        }
        public bool Validate()
        {
            System.Diagnostics.Debug.Assert(graphMatrix != null);
            System.Diagnostics.Debug.Assert(lookupTable != null);
#if DEBUG
            ulong[] copiedTable = new ulong[lookupTable.Length];
            Array.Copy(lookupTable, copiedTable, lookupTable.Length);
            Array.Sort(copiedTable);
            for (int i = 0; i < copiedTable.Length; ++i)
            {
                System.Diagnostics.Debug.Assert(lookupTable[i] == copiedTable[i], "Given NodeID LookupTable must be PRE-SORTED.");
            }
#endif

            for (int row = 0; row < graphMatrix.GetLength(0); ++row)
            {
                int sumColumn = 0;

                for (int column = 0; column < graphMatrix.GetLength(1); ++column)
                    sumColumn += graphMatrix[row, column];

                if (sumColumn == 0)
                {
                    System.Diagnostics.Debug.Assert(sumColumn != 0, "Containing Isolated Node");
                    return false;
                }
            }

            return true;
        }
        private int GetIndexOfNodeId(ulong id)
        {
            return Array.BinarySearch(lookupTable, id);
        }
    }
}