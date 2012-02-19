using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShapeMatchingGame.Grid;

namespace ShapeMatchingGame.MoveFinder
{
    interface IMoveFinder
    {
        Move GetBestMove(GridModel gridModel, int movesToLookAhead);
    }
}
