using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShapeMatchingGame.Shape;

namespace ShapeMatchingGame.Grid
{
    class GridModel<TShapeViewType> : IGridModel where TShapeViewType:IShapeView
    {
        //make private some day
        protected readonly IShapeView[,] Shapes;
        private readonly ShapeViewGenerator<TShapeViewType> _shapeViewGenerator = new ShapeViewGenerator<TShapeViewType>();

        #region Properties
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
        #endregion

        private GridModel(IShapeView[,] shapes)
        {
            Shapes = shapes;
        }
        public GridModel(int rows, int columns, int seed = 0)
        {
            if (seed != 0)
                _shapeViewGenerator = new ShapeViewGenerator<TShapeViewType>(seed);

            Shapes = new IShapeView[rows,columns];
            for (int row = 0; row < rows; row++)
                for (int column = 0; column < columns; column++)
                    Shapes[row, column] = _shapeViewGenerator.GetEmptyShapeView();
            int addedScore;
            FinishTurn(out addedScore);
            Score += addedScore;
        }

        #region Logic
        public bool DoMove(Move move)
        {
            if (!IsValidMove(move))
                return false;
            foreach (IShapeView shapeView in Shapes)
            {
                shapeView.RecentlySwapped = false;
                shapeView.RecentlyDropped = false;
            }
            Swap(move.From, move.To);
            int addedScore;
            FinishTurn(out addedScore);
            Score += addedScore;
            return true;
        }
        public bool IsValidMove(Move move)
        {
            bool isValid;
            lock (Shapes)
            {
                if (!IsPossibleMove(move))
                    return false;
                if (Shapes[move.From.Row, move.From.Column].IsEmpty || Shapes[move.To.Row, move.To.Column].IsEmpty)
                    return false;
                Swap(move.From, move.To);
                isValid = GetMatchAtPosition(move.To).IsValid
                          || GetMatchAtPosition(move.From).IsValid;
                //reverse the move
                Swap(move.From, move.To);
            }
            return isValid;
        }
        private bool IsPossibleMove(Move move)
        {
            //check if the shapeslot is left of the other shapeslot
            if (move.From.Column + 1 == move.To.Column && move.From.Row == move.To.Row)
                return true;
            //check if the shapeslot is right of the other shapeslot
            if (move.From.Column - 1 == move.To.Column && move.From.Row == move.To.Row)
                return true;
            //check if the shapeslot is above the other shapeslot
            if (move.From.Row + 1 == move.To.Row && move.From.Column == move.To.Column)
                return true;
            //check if the shapeslot is below the other shapeslot
            if (move.From.Row - 1 == move.To.Row && move.From.Column == move.To.Column)
                return true;
            return false;
        }
        private void FinishTurn(out int bonusScore)
        {
            bonusScore = 0;
            bool foundMatches;
            do
            {
                DropShapes();
                FillGrid();
                foundMatches = HandleMatches(ref bonusScore);
            } while (foundMatches || HasEmptyFields);
            Turn++;
        }
        private void Swap(Position from, Position to)
        {
            IShapeView temp = Shapes[from.Row, from.Column];
            Shapes[from.Row, from.Column] = Shapes[to.Row, to.Column];
            Shapes[to.Row, to.Column] = temp;
            Shapes[from.Row, from.Column].RecentlySwapped = true;
            Shapes[to.Row, to.Column].RecentlySwapped = true;
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
        private bool HandleMatches(ref int score)
        {
            bool foundMatch;
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
                            Shapes[bestPosition.Row, bestPosition.Column] = _shapeViewGenerator.GetShapeView(color,
                                                                                                             ShapeType
                                                                                                                 .
                                                                                                                 Blast);

                        else if (match.Creates == Creates.Cross)
                            Shapes[bestPosition.Row, bestPosition.Column] = _shapeViewGenerator.GetShapeView(color,
                                                                                                             ShapeType
                                                                                                                 .
                                                                                                                 Cross);
                    }

                }
            }
            return foundMatch;
        }
        private void FillGrid()
        {
            for (int column = 0; column < Columns; column++)
            {
                for (int row = Rows - 1; row >= 0; row--)
                {
                    if (Shapes[0, column].IsEmpty)
                    {
                        Shapes[0, column] = _shapeViewGenerator.GetNextShapeView(ShapeType.Normal);
                    }
                }
            }
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
                            Shapes[row + 1, column].RecentlyDropped = true;
                            Shapes[row, column] = _shapeViewGenerator.GetEmptyShapeView();
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
            Shapes[position.Row, position.Column].Destroy();
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
                for (int column = 0; column < columns; column++)
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
                Shapes[position.Row, position.Column].Destroy();
                score += 100;
            }
        }
        #endregion
        public GridModel<TShapeViewType> CloneRawGrid()
        {
            var gridModel = new GridModel<TShapeViewType>(DeepCopyShapes());
            return gridModel;
        }
        private IShapeView[,] DeepCopyShapes()
        {
            IShapeView[,] cloned = new IShapeView[Rows, Columns];
            for (int row = 0; row < cloned.GetLength(0); row++)
            {
                for (int column = 0; column < cloned.GetLength(1); column++)
                {
                    //shapeviews do not allow changing of the color or type of a shape
                    cloned[row, column] = _shapeViewGenerator.GetShapeView(Shapes[row, column].ShapeColor,
                                                                           Shapes[row, column].ShapeType);
                }
            }
            return cloned;
        }

    }
}
