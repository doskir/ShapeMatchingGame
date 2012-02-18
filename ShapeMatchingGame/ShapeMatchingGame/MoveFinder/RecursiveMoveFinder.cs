using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ShapeMatchingGame.MoveFinder
{
    class RecursiveMoveFinder : IMoveFinder
    {
        public Move GetBestMove(GridModel gridModel, int movesToLookAhead)
        {
            if (movesToLookAhead < 1)
                throw new Exception();
            List<Move> validMoves = gridModel.GetValidMoves();
            foreach (Move move in validMoves)
            {
                GridModel tempGridModel = gridModel.DeepCopy();
                tempGridModel.DoMove(move);
                //do the whole matching stuff on it
                int score;
                tempGridModel.HandleMatches(out score);
                move.PredictedScore = score;
                if (movesToLookAhead - 1 < 1)
                    continue;
                Move bestNextMove = GetBestMove(gridModel, movesToLookAhead - 1);
                if (bestNextMove == null)
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
