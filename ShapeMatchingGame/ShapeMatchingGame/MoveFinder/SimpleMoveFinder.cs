using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ShapeMatchingGame.Grid;
using ShapeMatchingGame.Shape;

namespace ShapeMatchingGame.MoveFinder
{
    class SimpleMoveFinder : IMoveFinder
    {

        public Move GetBestMove(GridModel<IShapeView> gridModel, int movesToLookAhead)
        {
            if (movesToLookAhead < 1)
                return new Move(new Position(-1, -1), new Position(-1, -1));
            List<Move> validMoves = Helpers.GetValidMoves(gridModel);
            foreach (Move move in validMoves)
            {
                GridModel<IShapeView> tempGridModel = gridModel.CloneRawGrid();
                tempGridModel.DoMove(move);
                //do the whole matching stuff on it
                move.PredictedScore = tempGridModel.Score;
            }
            Move bestMove = validMoves.OrderBy(mv => mv.PredictedScore).LastOrDefault();
            return bestMove;
        }
    }
}
