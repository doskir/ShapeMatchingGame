using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeMatchingGame
{
    class Grid : DrawableObject
    {
        public ShapeSlot[,] ShapeSlots;
        private int _rows;
        private int _columns;
        private RandomShapeGenerator _randomShapeGenerator;
        private ShapeSlot _currentlyHighlightedShapeSlot;
        public int Score;
        public Grid(Point position,int rows,int columns,int slotWidth,int slotHeight):this(position,rows,columns,slotWidth,slotHeight,-1)
        {
        }
        public Grid(Point position,int rows, int columns, int slotWidth, int slotHeight, int seed)
        {
            _rows = rows;
            _columns = columns;
            ShapeSlots = new ShapeSlot[rows,columns];
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    ShapeSlots[row, column] =
                        new ShapeSlot(new Rectangle(column*slotWidth + Rectangle.X, row*slotHeight + Rectangle.Y,
                                                    slotWidth, slotHeight));
                }
            }
            _randomShapeGenerator = new RandomShapeGenerator(seed);
            Rectangle = new Rectangle(position.X, position.Y, columns * slotWidth, rows * slotHeight);
            FillGrid();
        }
        public bool MovesAllowed
        {
            get
            {
                foreach (ShapeSlot shapeSlot in ShapeSlots)
                    if (shapeSlot.Shape.Moving)
                        return false;
                return true;
            }
        }
        public void FillGrid()
        {
            if (!MovesAllowed)
                return;
            for (int column = 0; column < _columns; column++)
            {
                for (int row = _rows - 1; row >= 0; row--)
                {
                    ShapeSlot shapeSlot = ShapeSlots[0, column];
                    if (shapeSlot.IsEmpty)
                    {
                        Rectangle creationRectangle = new Rectangle(shapeSlot.Rectangle.X, 0, 50, 50);
                        bool willIntersect;
                        do
                        {
                            creationRectangle.Y -= 50;
                            willIntersect = false;
                            for (int compRow = 0; compRow < _rows; compRow++)
                            {
                                if (creationRectangle.Intersects(ShapeSlots[compRow, column].Shape.Rectangle))
                                {
                                    willIntersect = true;
                                    break;
                                }
                            }
                        } while (willIntersect);
                        shapeSlot.Shape = _randomShapeGenerator.GetNextShape(ShapeType.Normal);
                        shapeSlot.Shape.Rectangle = creationRectangle;
                        shapeSlot.Shape.Rectangle.X = shapeSlot.Rectangle.X;
                        shapeSlot.Shape.DropTo(shapeSlot.Rectangle);
                        DropShapes();
                    }
                }
            }
        }

        public override void Update()
        {
            foreach (ShapeSlot shapeSlot in ShapeSlots)
                shapeSlot.Update();
            if (!MovesAllowed)
                return;
            HandleMatches();
            DropShapes();
            FillGrid();
        }

        public void DropShapes()
        {
            for (int column = 0; column < _columns; column++)
            {
                bool shapeDropped;
                do
                {
                    shapeDropped = false;
                    for (int row = _rows - 2; row >= 0; row--)
                    {
                        ShapeSlot currentShapeSlot = ShapeSlots[row, column];
                        if (currentShapeSlot.IsEmpty)
                        {
                            continue;
                        }
                        ShapeSlot shapeSlotBelow = ShapeSlots[row + 1, column];
                        if (shapeSlotBelow.IsEmpty)
                        {
                            //drop the current shape to the slow below
                            shapeSlotBelow.Shape = currentShapeSlot.Shape;
                            shapeSlotBelow.Shape.DropTo(shapeSlotBelow.Rectangle);
                            currentShapeSlot.ClearSlot();
                            shapeDropped = true;
                        }
                    }
                } while (shapeDropped);
            } 
        }
        public Shape[,] ShapeSlotsToArray()
        {
            Shape[,] shapeArray = new Shape[_rows,_columns];
            for(int row = 0;row < _rows;row++)
            {
                for(int column = 0;column < _columns;column++)
                {
                    shapeArray[row, column] = ShapeSlots[row, column].Shape;
                }
            }
            return shapeArray;
        }
        public bool HandleMatches()
        {
            bool foundMatch = false;
            Shape[,] shapeArray = ShapeSlotsToArray();
            List<Match> matches = GetMatches(shapeArray);
            foreach(Match match in matches)
            {
                if (match.IsValid)
                {
                    foundMatch = true;
                    ShapeColor color = ShapeColor.None;
                    foreach (Position position in match.InvolvedPositions)
                    {
                        color = ShapeSlots[position.Row, position.Column].Shape.Color;
                        ShapeType type = ShapeSlots[position.Row, position.Column].Shape.Type;
                        if(type == ShapeType.Blast)
                        {
                            List<Position> positionsToClear = new List<Position>();
                            if (position.Row > 0)
                            {
                                positionsToClear.Add(new Position(position.Row - 1, position.Column));
                                if (position.Column > 0)
                                    positionsToClear.Add(new Position(position.Row - 1, position.Column - 1));
                                if (position.Column < _columns - 1)
                                    positionsToClear.Add(new Position(position.Row - 1, position.Column + 1));
                            }
                            if(position.Row < _rows -1)
                            {
                                positionsToClear.Add(new Position(position.Row + 1, position.Column));
                                if (position.Column > 0)
                                    positionsToClear.Add(new Position(position.Row + 1, position.Column - 1));
                                if (position.Column < _columns - 1)
                                    positionsToClear.Add(new Position(position.Row + 1, position.Column + 1));
                            }
                            if (position.Column > 0)
                                positionsToClear.Add(new Position(position.Row, position.Column - 1));
                            if (position.Column < _columns - 1)
                                positionsToClear.Add(new Position(position.Row, position.Column + 1));

                            foreach(Position blastPosition in positionsToClear)
                            {
                                ShapeSlots[blastPosition.Row, blastPosition.Column].DestroyShape();
                                Score += 100;
                            }
                        }


                        ShapeSlots[position.Row, position.Column].DestroyShape();
                        Score += 100;
                    }

                    if (match.Creates == Creates.Blast)
                    {
                        ShapeSlot mostImportantShapeSlot = null;
                        foreach (Position position in match.InvolvedPositions)
                        {
                            if (ShapeSlots[position.Row, position.Column].RecentlySwappedTo)
                            {
                                mostImportantShapeSlot = ShapeSlots[position.Row, position.Column];
                                break;
                            }
                            if (ShapeSlots[position.Row, position.Column].RecentlyDestroyed)
                                mostImportantShapeSlot = ShapeSlots[position.Row, position.Column];
                        }
                        if (mostImportantShapeSlot == null)
                            throw new Exception("blast match found, no valid shapeslot found");
                        mostImportantShapeSlot.Shape = new Shape(color, ShapeType.Blast);
                        mostImportantShapeSlot.Shape.Rectangle.X = mostImportantShapeSlot.Rectangle.X;
                        mostImportantShapeSlot.Shape.Rectangle.Y = mostImportantShapeSlot.Rectangle.Y;

                        Score += 500;
                    }
                }
            }
            return foundMatch;

        }
        List<Match> GetMatches(Shape[,] shapeArray)
        {
            List<Match> matches = new List<Match>();
            for (int originRow = 0; originRow < _rows;originRow++)
            {
                for (int originColumn = 0; originColumn < _columns; originColumn++)
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
        Match GetMatchAtPosition(Shape[,] shapeArray,Position position)
        {
            ShapeColor myColor = shapeArray[position.Row, position.Column].Color;
            if(myColor == ShapeColor.None)
                return Match.Empty;
            List<Position> matchingShapesLeft = new List<Position>();
            for (int column = position.Column - 1; column >= 0; column--)
            {
                if (shapeArray[position.Row, column].Color == myColor)
                    matchingShapesLeft.Add(new Position(position.Row, column));
                else
                    break;
            }
            List<Position> matchingShapesRight = new List<Position>();
            for (int column = position.Column + 1; column < 8; column++)
            {
                if (shapeArray[position.Row, column].Color == myColor)
                    matchingShapesRight.Add(new Position(position.Row, column));
                else
                    break;
            }

            List<Position> matchingShapesAbove = new List<Position>();
            for (int row = position.Row - 1; row >= 0; row--)
            {
                if (shapeArray[row, position.Column].Color == myColor)
                    matchingShapesAbove.Add(new Position(row, position.Column));
                else
                    break;
            }
            List<Position> matchingShapesBelow = new List<Position>();
            for (int row = position.Row + 1; row < 8; row++)
            {
                if (shapeArray[row, position.Column].Color == myColor)
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
        public bool IsPossibleMove(Position from,Position to)
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
        public bool IsValidMove(Position from,Position to)
        {
            if (!IsPossibleMove(from, to))
                return false;
            Shape[,] shapes = ShapeSlotsToArray();
            //swap the shapes
            Shape temp = shapes[from.Row, from.Column];
            shapes[from.Row, from.Column] = shapes[to.Row, to.Column];
            shapes[to.Row, to.Column] = temp;
            if (GetMatchAtPosition(shapes, to).IsValid || GetMatchAtPosition(shapes, from).IsValid)
            {
                return true;
            }
            return false;
        }
        public Position GetShapeSlotPosition(ShapeSlot shapeSlot)
        {
            for (int row = 0; row < _rows; row++)
            {
                for (int column = 0; column < _columns; column++)
                {
                    if (ShapeSlots[row, column] == shapeSlot)
                    {
                        return new Position(row, column);
                    }
                }
            }
            throw new Exception("Shapeslot is not part of the grid. WTF!");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach(ShapeSlot shapeSlot in ShapeSlots)
            {
                shapeSlot.Draw(spriteBatch);
            }
        }
        public bool DoMove(Position from,Position to)
        {
            if(IsValidMove(from,to))
            {
                foreach(ShapeSlot shapeSlot in ShapeSlots)
                {
                    shapeSlot.RecentlyDestroyed = false;
                    shapeSlot.RecentlySwappedTo = false;
                }
                Swap(ShapeSlots[from.Row, from.Column], ShapeSlots[to.Row, to.Column]);
                ShapeSlots[from.Row, from.Column].RecentlySwappedTo = true;
                ShapeSlots[to.Row, to.Column].RecentlySwappedTo = true;
                return true;
            }
            return false;
        }
        public void Clicked(Point position)
        {
            foreach(ShapeSlot slot in ShapeSlots)
            {
                if(slot.Rectangle.Contains(position))
                {
                    if(slot == _currentlyHighlightedShapeSlot)
                    {
                        slot.IsHighlighted = false;
                        _currentlyHighlightedShapeSlot = null;
                    }
                    else if (_currentlyHighlightedShapeSlot == null)
                    {
                        slot.IsHighlighted = true;
                        _currentlyHighlightedShapeSlot = slot;
                        return;
                    }
                    else
                    {
                        Position from = GetShapeSlotPosition(_currentlyHighlightedShapeSlot);
                        Position to = GetShapeSlotPosition(slot);
                        DoMove(from, to);

                        _currentlyHighlightedShapeSlot.IsHighlighted = false;
                        _currentlyHighlightedShapeSlot = null;
                    }
                }
            }
        }
        public void Swap(ShapeSlot shapeSlot,ShapeSlot otherShapeSlot)
        {
            Shape temp = shapeSlot.Shape;
            shapeSlot.Shape = otherShapeSlot.Shape;
            otherShapeSlot.Shape = temp;
            shapeSlot.Shape.DropTo(shapeSlot.Rectangle);
            otherShapeSlot.Shape.DropTo(otherShapeSlot.Rectangle);
        }

        public void DeleteAt(Point cursorPosition)
        {
            foreach (ShapeSlot slot in ShapeSlots)
            {
                if (slot.Rectangle.Contains(cursorPosition))
                {
                    slot.Shape = Shape.Empty;
                }
            }
        }
    }
}
