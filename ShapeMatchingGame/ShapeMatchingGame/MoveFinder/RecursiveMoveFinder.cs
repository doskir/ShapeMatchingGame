﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ShapeMatchingGame.Grid;
using ShapeMatchingGame.Shape;

namespace ShapeMatchingGame.MoveFinder
{
    class RecursiveMoveFinder : IMoveFinder
    {
        public Move GetBestMove(GridModel<ShapeViewDrawable> gridModel, int movesToLookAhead)
        {
            if (movesToLookAhead < 1)
                throw new Exception();
            List<Move> validMoves = Helpers.GetValidMoves(gridModel);
            foreach (Move move in validMoves)
            {
                GridModel<ShapeViewDrawable> tempGridModel = gridModel.CloneRawGrid();
                tempGridModel.DoMove(move);
                //do the whole matching stuff on it
                move.PredictedScore = tempGridModel.Score;
                if (movesToLookAhead - 1 < 1)
                    continue;
                Move bestNextMove = GetBestMove(gridModel, movesToLookAhead - 1);
                if (bestNextMove == null)
                {
                    //no valid moves left
                    move.PredictedScore = Int32.MinValue;
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
