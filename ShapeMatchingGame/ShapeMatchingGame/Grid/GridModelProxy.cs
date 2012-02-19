using System.Collections.Generic;
using ShapeMatchingGame.Shape;

namespace ShapeMatchingGame.Grid
{
    internal class GridModelProxy : IGridModel
    {
        private readonly GridModel _gridModel;

        public bool DoMove(Move move)
        {
            return _gridModel.DoMove(move);
        }

        public GridModel CloneRawGrid()
        {
            return _gridModel.CloneRawGrid();
        }

        public int Turn
        {
            get { return _gridModel.Turn; }
        }

        public int Score
        {
            get { return _gridModel.Score; }
        }

        public int Rows
        {
            get { return _gridModel.Rows; }
        }

        public int Columns
        {
            get { return _gridModel.Columns; }
        }
        public GridModelProxy(int rows,int columns,int seed = 0)
        {
            _gridModel = new GridModel(rows, columns, seed);
        }
    }
}
