using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeMatchingGame.MoveFinder
{
    interface IMoveFinder
    {
        Move GetBestMove(Shape[,] shapes, int movesToLookAhead);
    }
}
