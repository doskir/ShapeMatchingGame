﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ShapeMatchingGame.MoveFinder
{
    class SimpleMoveFinder : IMoveFinder
    {

        public Move GetBestMove(GridModel gridModel, int movesToLookAhead)
        {
            if (movesToLookAhead < 1)
                return new Move(new Position(-1, -1), new Position(-1, -1));
            List<Move> validMoves = Helpers.GetValidMoves(gridModel);
            foreach (Move move in validMoves)
            {
                GridModel tempGridModel = gridModel.DeepCopy();
                tempGridModel.DoMove(move);
                //do the whole matching stuff on it
                move.PredictedScore = tempGridModel.Score;
            }
            Move bestMove = validMoves.OrderBy(mv => mv.PredictedScore).LastOrDefault();
            return bestMove;
        }
    }
}
