using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeMatchingGame.Grid
{
    internal interface IGridModel
    {
        bool DoMove(Move move);
        GridModel CloneRawGrid();
        int Turn { get; }
        int Score { get; }
        int Rows { get; }
        int Columns { get; }
    }
}
