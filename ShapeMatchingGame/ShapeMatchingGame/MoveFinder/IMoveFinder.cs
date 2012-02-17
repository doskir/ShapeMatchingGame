using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeMatchingGame.MoveFinder
{
    interface IMoveFinder
    {
        Move GetBestMove(Shape.ShapeViewDrawable[,] shapesViewDrawable, int movesToLookAhead);
        Move GetBestMove(GridModel gridModel, int movesToLookAhead);
    }
}
