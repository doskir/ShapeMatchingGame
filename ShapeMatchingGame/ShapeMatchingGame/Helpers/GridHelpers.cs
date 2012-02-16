using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeMatchingGame.Helpers
{
    class GridHelpers
    {
        public static List<Move> GetValidMoves(Shape[,] shapes)
        {
            List<Move> possibleMoves = GetPossibleMoves(Clone(shapes));
            List<Move> validMoves = new List<Move>();
            foreach (Move move in possibleMoves)
            {
                if (IsValidMove(Clone(shapes), move.From, move.To))
                {
                    validMoves.Add(move);
                }
            }
            return validMoves;
        }
        public static Shape[,] Clone(Shape[,] shapes)
        {
            Shape[,] cloned = new Shape[shapes.GetLength(0), shapes.GetLength(1)];
            for (int row = 0; row < cloned.GetLength(0); row++)
            {
                for (int column = 0; column < cloned.GetLength(1); column++)
                {
                    cloned[row, column] = (Shape)shapes[row, column].Clone();
                }
            }
            return cloned;
        }
        private static List<Move> GetPossibleMoves(Shape[,] shapes)
        {
            int rows = shapes.GetLength(0);
            int columns = shapes.GetLength(1);
            List<Move> moves = new List<Move>();
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    Position from = new Position(row, column);
                    if (column > 0)
                        moves.Add(new Move(from, new Position(from.Row, from.Column - 1)));
                    if (column < columns - 1)
                        moves.Add(new Move(from, new Position(from.Row, from.Column + 1)));
                    if (row > 0)
                        moves.Add(new Move(from, new Position(from.Row - 1, from.Column)));
                    if (row < rows - 1)
                        moves.Add(new Move(from, new Position(from.Row + 1, from.Column)));
                }
            }
            return moves;
        }
        private static bool IsPossibleMove(Position from, Position to)
        {
            //check if the shapeslot is left of the other shapeslot
            if (from.Column + 1 == to.Column && from.Row == to.Row)
                return true;
            //check if the shapeslot is right of the other shapeslot
            if (from.Column - 1 == to.Column && from.Row == to.Row)
                return true;
            //check if the shapeslot is above the other shapeslot
            if (from.Row + 1 == to.Row && from.Column == to.Column)
                return true;
            //check if the shapeslot is below the other shapeslot
            if (from.Row - 1 == to.Row && from.Column == to.Column)
                return true;
            return false;
        }
        public static bool IsValidMove(Shape[,] shapes,Position from, Position to)
        {
            if (!IsPossibleMove(from, to))
                return false;
            //swap the shapes
            Shape temp = shapes[from.Row, from.Column];
            shapes[from.Row, from.Column] = shapes[to.Row, to.Column];
            shapes[to.Row, to.Column] = temp;
            if (GetMatchAtPosition(shapes, to).IsValid || GetMatchAtPosition(shapes, from).IsValid)
            {
                return GetMatchAtPosition(shapes, to).IsValid || GetMatchAtPosition(shapes, from).IsValid;
            }
            return false;
        }
        public static Match GetMatchAtPosition(Shape[,] shapes, Position position)
        {
            int rows = shapes.GetLength(0);
            int columns = shapes.GetLength(1);
            ShapeColor myColor = shapes[position.Row, position.Column].Color;
            if (myColor == ShapeColor.None)
                return Match.Empty;
            List<Position> matchingShapesLeft = new List<Position>();
            for (int column = position.Column - 1; column >= 0; column--)
            {
                if (shapes[position.Row, column].Color == myColor)
                    matchingShapesLeft.Add(new Position(position.Row, column));
                else
                    break;
            }
            List<Position> matchingShapesRight = new List<Position>();
            for (int column = position.Column + 1; column < columns; column++)
            {
                if (shapes[position.Row, column].Color == myColor)
                    matchingShapesRight.Add(new Position(position.Row, column));
                else
                    break;
            }

            List<Position> matchingShapesAbove = new List<Position>();
            for (int row = position.Row - 1; row >= 0; row--)
            {
                if (shapes[row, position.Column].Color == myColor)
                    matchingShapesAbove.Add(new Position(row, position.Column));
                else
                    break;
            }
            List<Position> matchingShapesBelow = new List<Position>();
            for (int row = position.Row + 1; row < rows; row++)
            {
                if (shapes[row, position.Column].Color == myColor)
                    matchingShapesBelow.Add(new Position(row, position.Column));
                else
                    break;
            }
            List<Position> horizontalMatch = new List<Position>(matchingShapesLeft);
            horizontalMatch.AddRange(matchingShapesRight);
            List<Position> verticalMatch = new List<Position>(matchingShapesAbove);
            verticalMatch.AddRange(matchingShapesBelow);

            if (horizontalMatch.Count < 2)
                horizontalMatch.RemoveRange(0, horizontalMatch.Count);
            if (verticalMatch.Count < 2)
                verticalMatch.RemoveRange(0, verticalMatch.Count);

            if (horizontalMatch.Count == 0 && verticalMatch.Count == 0)
                return Match.Empty;

            Creates creates = Creates.Nothing;
            if (horizontalMatch.Count >= 4 || verticalMatch.Count >= 4)
            {
                creates = Creates.Star;
            }
            else if (horizontalMatch.Count >= 2 && verticalMatch.Count >= 2)
            {
                creates = Creates.Cross;
            }
            else if (horizontalMatch.Count >= 3 || verticalMatch.Count >= 3)
            {
                creates = Creates.Blast;
            }
            List<Position> involvedShapes = new List<Position>(horizontalMatch);
            involvedShapes.AddRange(verticalMatch);
            Match match = new Match(involvedShapes, position, creates);
            return match;
        }

        public static Shape[,] DoMove(Shape[,] shapes, Move validMove)
        {
            foreach(Shape shape in shapes)
            {
                shape.RecentlyDropped = false;
                shape.RecentlySwapped = false;
            }
            Shape temp = shapes[validMove.From.Row, validMove.From.Column];
            shapes[validMove.From.Row, validMove.From.Column] = shapes[validMove.To.Row, validMove.To.Column];
            shapes[validMove.To.Row, validMove.To.Column] = temp;
            shapes[validMove.From.Row, validMove.From.Column].RecentlySwapped = true;
            shapes[validMove.To.Row, validMove.To.Column].RecentlySwapped = true;
            return shapes;
        }
        public static List<Match> GetMatches(Shape[,] shapeArray)
        {
            int rows = shapeArray.GetLength(0);
            int columns = shapeArray.GetLength(1);
            List<Match> matches = new List<Match>();
            for (int originRow = 0; originRow < rows; originRow++)
            {
                for (int originColumn = 0; originColumn < columns; originColumn++)
                {
                    Match match = GetMatchAtPosition(shapeArray, new Position(originRow, originColumn));
                    if (match.IsValid)
                    {
                        bool duplicateMatch = false;
                        //prevent duplicate match adding
                        foreach (Match existingMatch in matches)
                        {
                            if (existingMatch.InvolvedPositions.Contains(match.Center))
                            {
                                duplicateMatch = true;
                                break;
                            }
                        }
                        if (!duplicateMatch)
                            matches.Add(match);
                    }
                }
            }
            return matches;
        }
        public static Shape[,] HandleMatches(Shape[,] shapes, out int score)
        {
            score = 0;
            bool foundMatch = false;
            List<Match> matches = GetMatches(shapes);
            foreach(Match match in matches)
            {
                if (match.IsValid)
                {
                    foundMatch = true;
                    ShapeColor color = ShapeColor.None;
                    foreach (Position position in match.InvolvedPositions)
                    {
                        color = shapes[position.Row, position.Column].Color;
                        shapes = DestroyShape(shapes, position, ref score);
                    }
                    if(match.Creates != Creates.Nothing)
                    {
                        //get the best spot to create the new shape
                        Position bestPosition = new Position(-1, -1);
                        foreach (Position position in match.InvolvedPositions)
                        {
                            if (shapes[position.Row, position.Column].RecentlySwapped)
                            {
                                bestPosition = position;
                                break;
                            }
                            if (shapes[position.Row, position.Column].RecentlyDropped)
                                bestPosition = position;
                        }
                        if (bestPosition.Row == -1 && bestPosition.Column == -1)
                            bestPosition = match.Center;
                        if(match.Creates == Creates.Blast)
                            shapes[bestPosition.Row, bestPosition.Column] = new Shape(color, ShapeType.Blast);
                        if (match.Creates == Creates.Cross)
                            shapes[bestPosition.Row, bestPosition.Column] = new Shape(color, ShapeType.Cross);
                    }
                }
            }
            return shapes;
        }
        public static Shape[,] DestroyShape(Shape[,] shapes,Position position,ref int score)
        {
            int rows = shapes.GetLength(0);
            int columns = shapes.GetLength(1);
            ShapeType type = shapes[position.Row, position.Column].Type;
            //to prevent infinite loops
            shapes[position.Row,position.Column].Type = ShapeType.None;
            if (type == ShapeType.Blast)
            {
                List<Position> positionsToClear = new List<Position>();
                if (position.Row > 0)
                {
                    positionsToClear.Add(new Position(position.Row - 1, position.Column));
                    if (position.Column > 0)
                        positionsToClear.Add(new Position(position.Row - 1, position.Column - 1));
                    if (position.Column < columns - 1)
                        positionsToClear.Add(new Position(position.Row - 1, position.Column + 1));
                }
                if (position.Row < rows - 1)
                {
                    positionsToClear.Add(new Position(position.Row + 1, position.Column));
                    if (position.Column > 0)
                        positionsToClear.Add(new Position(position.Row + 1, position.Column - 1));
                    if (position.Column < columns- 1)
                        positionsToClear.Add(new Position(position.Row + 1, position.Column + 1));
                }
                if (position.Column > 0)
                    positionsToClear.Add(new Position(position.Row, position.Column - 1));
                if (position.Column < columns - 1)
                    positionsToClear.Add(new Position(position.Row, position.Column + 1));

                foreach (Position blastPosition in positionsToClear)
                {
                    shapes = DestroyShape(shapes, blastPosition, ref score);
                }
            }
            if (type == ShapeType.Cross)
            {
                List<Position> positionsToClear = new List<Position>();
                for (int row = 0; row < rows; row++)
                {
                    if (row == position.Row)
                        continue;
                    positionsToClear.Add(new Position(row, position.Column));
                }
                for (int column = 0; column < rows; column++)
                {
                    if (column == position.Column)
                        continue;
                    positionsToClear.Add(new Position(position.Row, column));
                }
                foreach (Position crossPosition in positionsToClear)
                    shapes = DestroyShape(shapes,crossPosition,ref score)
                ;
            }
            if (type != ShapeType.None)
            {
                shapes[position.Row, position.Column] = Shape.Empty;
                score += 100;
            }
            return shapes;
        }
    }
}
