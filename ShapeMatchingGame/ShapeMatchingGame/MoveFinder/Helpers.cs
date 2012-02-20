using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShapeMatchingGame.Grid;
using ShapeMatchingGame.Shape;

namespace ShapeMatchingGame.MoveFinder
{
    class Helpers
    {
        public static List<Move> GetValidMoves(GridModel<IShapeView> gridModel)
        {
            List<Move> possibleMoves = GetPossibleMoves(gridModel);
            List<Move> validMoves = new List<Move>();
            foreach (Move move in possibleMoves)
            {
                if (gridModel.IsValidMove(move.From, move.To))
                {
                    validMoves.Add(move);
                }
            }
            return validMoves;
        }
        private static List<Move> GetPossibleMoves(GridModel<IShapeView> gridModel)
        {
            int rows = gridModel.Rows;
            int columns = gridModel.Columns;
            List<Move> moves = new List<Move>();
            for (int row = 0; row < rows - 1; row++)
            {
                for (int column = 0; column < columns - 1; column++)
                {
                    Position from = new Position(row, column);
                    //only allow moving down and to the right, moving left or up are just mirror moves
                    moves.Add(new Move(from, new Position(from.Row, from.Column + 1)));
                    moves.Add(new Move(from, new Position(from.Row + 1, from.Column)));
                }
            }
            return moves;
        }
    }
}
