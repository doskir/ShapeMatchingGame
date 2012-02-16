using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShapeMatchingGame.Helpers;

namespace ShapeMatchingGame.MoveFinder
{
    class SimpleMoveFinder : IMoveFinder
    {
        public Move GetBestMove(Shape[,] shapes, int movesToLookAhead)
        {
            List<Move> validMoves = Helpers.GridHelpers.GetValidMoves(shapes);
            foreach(Move move in validMoves)
            {
                //apply the swap to the grid
                Shape[,] newGrid = Helpers.GridHelpers.DoMove(GridHelpers.Clone(shapes), move);
                //do the whole matching stuff on it
                int score;
                newGrid = Helpers.GridHelpers.HandleMatches(newGrid, out score);
                move.PredictedScore = score;
            }
            Move bestMove = validMoves.OrderBy(mv => mv.PredictedScore).LastOrDefault();
            return bestMove;
        }
    }
}
