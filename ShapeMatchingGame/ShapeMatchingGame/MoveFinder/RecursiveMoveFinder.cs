using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ShapeMatchingGame.MoveFinder
{
    class RecursiveMoveFinder : IMoveFinder
    {
        public Move GetBestMove(Shape[,] shapes, int movesToLookAhead)
        {
            if (movesToLookAhead < 1)
                return new Move(new Position(-1, -1), new Position(-1, -1));
            int rows = shapes.GetLength(0);
            int columns = shapes.GetLength(1);
            List<Move> validMoves = Helpers.GridHelpers.GetValidMoves(shapes);
            foreach (Move move in validMoves)
            {
                //apply the swap to the grid
                Shape[,] newGrid = Helpers.GridHelpers.DoMove(shapes, move);
                //do the whole matching stuff on it
                int score;
                newGrid = Helpers.GridHelpers.HandleMatches(newGrid, out score);
                move.PredictedScore = score;
                Move bestNextMove = GetBestMove(newGrid, movesToLookAhead - 1);
                if(bestNextMove == null)
                {
                    //no valid moves left
                }
                else
                {
                    move.PredictedScore += bestNextMove.PredictedScore;
                }
            }
            Move bestMove = validMoves.OrderBy(mv => mv.PredictedScore).LastOrDefault();
            return bestMove;
        }
    }
}
