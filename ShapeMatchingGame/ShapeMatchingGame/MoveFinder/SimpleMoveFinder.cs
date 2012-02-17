using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ShapeMatchingGame.Helpers;

namespace ShapeMatchingGame.MoveFinder
{
    class SimpleMoveFinder : IMoveFinder
    {
        public Move GetBestMove(Shape.ShapeViewDrawable[,] shapesViewDrawable, int movesToLookAhead)
        {
            int rows = shapesViewDrawable.GetLength(0);
            int columns = shapesViewDrawable.GetLength(1);
            List<Move> validMoves = Helpers.GridHelpers.GetValidMoves(shapesViewDrawable);
            foreach(Move move in validMoves)
            {
                //apply the swap to the grid
                Shape.ShapeViewDrawable[,] newGrid = Helpers.GridHelpers.DoMove(shapesViewDrawable, move);
                //do the whole matching stuff on it
                int score;
                newGrid = Helpers.GridHelpers.HandleMatches(newGrid, out score);
                move.PredictedScore = score;
            }
            Move bestMove = validMoves.OrderBy(mv => mv.PredictedScore).LastOrDefault();
            return bestMove;
        }

        public Move GetBestMove(GridModel gridModel, int movesToLookAhead)
        {
            throw new NotImplementedException();
        }
    }
}
