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
        public Move GetBestMove(Shape[,] shapes, int movesToLookAhead)
        {
            int rows = shapes.GetLength(0);
            int columns = shapes.GetLength(1);
            List<Move> validMoves = Helpers.GridHelpers.GetValidMoves(shapes);
            foreach(Move move in validMoves)
            {
                //apply the swap to the grid
                Shape[,] newGrid = Helpers.GridHelpers.DoMove(shapes, move);
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
