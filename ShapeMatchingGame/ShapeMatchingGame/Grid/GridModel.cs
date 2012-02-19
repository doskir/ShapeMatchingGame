using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShapeMatchingGame.Shape;

namespace ShapeMatchingGame.Grid
{
    class GridModel : IGridModel
    {
        //make private some day
        public readonly ShapeView[,] Shapes;
        private readonly RandomShapeGenerator _randomShapeGenerator = new RandomShapeGenerator();

        public int Turn { get; private set; }

        public int Score { get; private set; }

        public int Rows
        {
            get { return Shapes.GetLength(0); }
        }
        public int Columns
        {
            get { return Shapes.GetLength(1); }
        }

        private bool HasEmptyFields
        {
            get
            {
                for (int row = 0; row < Rows; row++)
                {
                    for (int column = 0; column < Columns; column++)
                    {
                        if (Shapes[row, column].IsEmpty)
                            return true;
                    }
                }
                return false;
            }
        }

        private GridModel(ShapeView[,] shapes)
        {
            Shapes = shapes;
        }
        public GridModel(int rows, int columns, int seed = 0)
        {
            if (seed != 0)
                _randomShapeGenerator = new RandomShapeGenerator(seed);

            Shapes = new ShapeView[rows,columns];
            for (int row = 0; row < rows; row++)
                for (int column = 0; column < columns; column++)
                    Shapes[row, column] = ShapeView.Empty;
            int addedScore;
            FinishTurn(out addedScore);
            Score += addedScore;
        }


        public bool DoMove(Move move)
        {
            if (!IsValidMove(move.From, move.To))
                return false;
            foreach (ShapeView shapeView in Shapes)
            {
                shapeView.RecentlySwapped = false;
                shapeView.RecentlyDropped = false;
            }
            Swap(move.From, move.To);
            Shapes[move.From.Row, move.From.Column].RecentlySwapped = true;
            Shapes[move.To.Row, move.To.Column].RecentlySwapped = true;
            int addedScore;
            FinishTurn(out addedScore);
            Score += addedScore;
            return true;
        }
        public bool IsValidMove(Position from, Position to)
        {
            bool isValid;
            lock (Shapes)
            {
                if (!IsPossibleMove(from, to))
                    return false;
                if (Shapes[from.Row, from.Column].IsEmpty || Shapes[to.Row, to.Column].IsEmpty)
                    return false;
                Swap(from, to);
                isValid = GetMatchAtPosition(to).IsValid
                          || GetMatchAtPosition(from).IsValid;
                //reverse the move
                Swap(from, to);
            }
            return isValid;
        }
        public GridModel CloneRawGrid()
        {
            GridModel gridModel = new GridModel(DeepCopyShapes());
            return gridModel;
        }
        private bool IsPossibleMove(Position from, Position to)
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
        private void FinishTurn(out int score)
        {
            do
            {
                HandleMatches(out score);
                DropShapes();
                FillGrid();
            } while (HasEmptyFields);
            Turn++;
        }
        private void Swap(Position from, Position to)
        {
            ShapeView temp = Shapes[from.Row, from.Column];
            Shapes[from.Row, from.Column] = Shapes[to.Row, to.Column];
            Shapes[to.Row, to.Column] = temp;
        }
        private Match GetMatchAtPosition(Position position)
        {
            int rows = Shapes.GetLength(0);
            int columns = Shapes.GetLength(1);
            ShapeColor myColor = Shapes[position.Row, position.Column].ShapeColor;
            if (myColor == ShapeColor.None)
                return Match.Empty;
            List<Position> matchingShapesLeft = new List<Position>();
            for (int column = position.Column - 1; column >= 0; column--)
            {
                if (Shapes[position.Row, column].ShapeColor == myColor)
                    matchingShapesLeft.Add(new Position(position.Row, column));
                else
                    break;
            }
            List<Position> matchingShapesRight = new List<Position>();
            for (int column = position.Column + 1; column < columns; column++)
            {
                if (Shapes[position.Row, column].ShapeColor == myColor)
                    matchingShapesRight.Add(new Position(position.Row, column));
                else
                    break;
            }

            List<Position> matchingShapesAbove = new List<Position>();
            for (int row = position.Row - 1; row >= 0; row--)
            {
                if (Shapes[row, position.Column].ShapeColor == myColor)
                    matchingShapesAbove.Add(new Position(row, position.Column));
                else
                    break;
            }
            List<Position> matchingShapesBelow = new List<Position>();
            for (int row = position.Row + 1; row < rows; row++)
            {
                if (Shapes[row, position.Column].ShapeColor == myColor)
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
        private void FillGrid()
        {
            for (int column = 0; column < Columns; column++)
            {
                for (int row = Rows - 1; row >= 0; row--)
                {
                    if (Shapes[0, column].IsEmpty)
                    {
                        Shapes[0, column] = _randomShapeGenerator.GetNextShapeView(ShapeType.Normal);
                    }
                }
            }
        }
        private void HandleMatches(out int score)
        {
            score = 0;
            bool foundMatch;
            do
            {
                foundMatch = false;
                IEnumerable<Match> matches = GetMatches();
                foreach (Match match in matches)
                {
                    if (match.IsValid)
                    {
                        foundMatch = true;
                        ShapeColor color = ShapeColor.None;
                        foreach (Position position in match.InvolvedPositions)
                        {
                            color = Shapes[position.Row, position.Column].ShapeColor;
                            DestroyShape(position, ref score);
                        }
                        if (match.Creates != Creates.Nothing)
                        {
                            //get the best spot to create the new shape
                            Position bestPosition = new Position(-1, -1);
                            foreach (Position position in match.InvolvedPositions)
                            {
                                if (Shapes[position.Row, position.Column].RecentlySwapped)
                                {
                                    bestPosition = position;
                                    break;
                                }
                                if (Shapes[position.Row, position.Column].RecentlyDropped)
                                    bestPosition = position;
                            }
                            if (bestPosition.Row == -1 && bestPosition.Column == -1)
                                bestPosition = match.Center;
                            if (match.Creates == Creates.Blast)
                                Shapes[bestPosition.Row, bestPosition.Column] = new ShapeView(color, ShapeType.Blast);

                            else if (match.Creates == Creates.Cross)
                                Shapes[bestPosition.Row, bestPosition.Column] = new ShapeView(color, ShapeType.Cross);
                        }

                    }
                }
                DropShapes();
            } while (foundMatch);
        }
        private void DropShapes()
        {
            int rows = Shapes.GetLength(0);
            int columns = Shapes.GetLength(1);
            for (int column = 0; column < columns; column++)
            {
                bool shapeDropped;
                do
                {
                    shapeDropped = false;
                    for (int row = rows - 2; row >= 0; row--)
                    {
                        if (Shapes[row, column].IsEmpty)
                            continue;
                        if (Shapes[row + 1, column].IsEmpty)
                        {
                            Shapes[row + 1, column] = Shapes[row, column];
                            Shapes[row, column] = ShapeView.Empty;
                            shapeDropped = true;
                        }
                    }
                } while (shapeDropped);
            }
        }
        private void DestroyShape(Position position, ref int score)
        {
            int rows = Shapes.GetLength(0);
            int columns = Shapes.GetLength(1);
            ShapeType type = Shapes[position.Row, position.Column].ShapeType;
            //to prevent infinite loops
            Shapes[position.Row, position.Column] = ShapeView.Empty;
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
                    if (position.Column < columns - 1)
                        positionsToClear.Add(new Position(position.Row + 1, position.Column + 1));
                }
                if (position.Column > 0)
                    positionsToClear.Add(new Position(position.Row, position.Column - 1));
                if (position.Column < columns - 1)
                    positionsToClear.Add(new Position(position.Row, position.Column + 1));

                foreach (Position blastPosition in positionsToClear)
                {
                    DestroyShape(blastPosition, ref score);
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
                    DestroyShape(crossPosition, ref score);
            }
            if (type != ShapeType.None)
            {
                Shapes[position.Row, position.Column] = ShapeView.Empty;
                score += 100;
            }
        }
        private IEnumerable<Match> GetMatches()
        {
            int rows = Shapes.GetLength(0);
            int columns = Shapes.GetLength(1);
            List<Match> matches = new List<Match>();
            for (int originRow = 0; originRow < rows; originRow++)
            {
                for (int originColumn = 0; originColumn < columns; originColumn++)
                {
                    Match match = GetMatchAtPosition(new Position(originRow, originColumn));
                    if (match.IsValid)
                    {
                        bool duplicateMatch = matches.Any(existingMatch => existingMatch.InvolvedPositions.Contains(match.Center));
                        //prevent duplicate match adding
                        if (!duplicateMatch)
                            matches.Add(match);
                    }
                }
            }
            return matches;
        }
        private ShapeView[,] DeepCopyShapes()
        {
            ShapeView[,] cloned = new ShapeView[Shapes.GetLength(0), Shapes.GetLength(1)];
            for (int row = 0; row < cloned.GetLength(0); row++)
            {
                for (int column = 0; column < cloned.GetLength(1); column++)
                {
                    //shapeviews do not allow changing of the color or type of a shape
                    cloned[row, column] = new ShapeView(Shapes[row, column].ShapeColor, Shapes[row, column].ShapeType);
                }
            }
            return cloned;
        }
    }
}
