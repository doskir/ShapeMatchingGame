using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShapeMatchingGame.Grid;
using ShapeMatchingGame.Shape;

namespace ShapeMatchingGame.MoveFinder
{
    interface IMoveFinder
    {
        Move GetBestMove(GridModel<IShapeView> gridModel, int movesToLookAhead);
    }
}
