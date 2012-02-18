using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShapeMatchingGame.Shape;

namespace ShapeMatchingGame.Grid
{
    class GridViewDrawable : IDrawableObject
    {
        private GridModel _gridModel;
        public ShapeSlot[,] ShapeSlots;
        private int _rows;
        private int _columns;
        private RandomShapeGenerator _randomShapeGenerator;
        private ShapeSlot _currentlyHighlightedShapeSlot;
        public int Score;
        public int Turn = 1;
        public Rectangle Rectangle;
        public Texture2D Texture;
        public GridViewDrawable(Point position,int rows,int columns,int slotWidth,int slotHeight):this(position,rows,columns,slotWidth,slotHeight,-1)
        {
        }
        public GridViewDrawable(Point position,int rows, int columns, int slotWidth, int slotHeight, int seed)
        {
            _gridModel = new GridModel(rows, columns);
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
                    if (shapeSlot.ShapeViewDrawable.Moving)
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
                                if (creationRectangle.Intersects(ShapeSlots[compRow, column].ShapeViewDrawable.Rectangle))
                                {
                                    willIntersect = true;
                                    break;
                                }
                            }
                        } while (willIntersect);
                        shapeSlot.ShapeViewDrawable = _randomShapeGenerator.GetNextShapeViewDrawable(ShapeType.Normal);
                        shapeSlot.ShapeViewDrawable.Rectangle = creationRectangle;
                        shapeSlot.ShapeViewDrawable.Rectangle.X = shapeSlot.Rectangle.X;
                        shapeSlot.ShapeViewDrawable.DropTo(shapeSlot.Rectangle);
                        DropShapes();
                    }
                }
            }
        }

        public void Update()
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
                            shapeSlotBelow.ShapeViewDrawable = currentShapeSlot.ShapeViewDrawable;
                            shapeSlotBelow.ShapeViewDrawable.DropTo(shapeSlotBelow.Rectangle);
                            currentShapeSlot.ClearSlot();
                            shapeDropped = true;
                        }
                    }
                } while (shapeDropped);
            } 
        }
        public Shape.ShapeView[,] ShapeSlotsToArray()
        {
            ShapeView[,] shapeViewDrawableArray = new ShapeView[_rows,_columns];
            for(int row = 0;row < _rows;row++)
            {
                for(int column = 0;column < _columns;column++)
                {
                    shapeViewDrawableArray[row, column] = ShapeSlots[row, column].ShapeViewDrawable;
                }
            }
            return shapeViewDrawableArray;
        }
        public GridModel ToGridModel()
        {
            GridModel gridModel = new GridModel(ShapeSlotsToArray());
            return gridModel;
        }
        public bool HandleMatches()
        {
            bool foundMatch = false;
            ShapeView[,] shapeViewDrawableArray = ShapeSlotsToArray();
            List<Match> matches = GetMatches();
            foreach(Match match in matches)
            {
                if (match.IsValid)
                {
                    foundMatch = true;
                    ShapeColor color = ShapeColor.None;
                    foreach (Position position in match.InvolvedPositions)
                    {
                        color = ShapeSlots[position.Row, position.Column].ShapeViewDrawable.ShapeColor;
                        DestroyShape(position);
                    }
                    if (match.Creates != Creates.Nothing)
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
                            throw new Exception("creating match found, no valid shapeslot found");
                        if (match.Creates == Creates.Blast)
                        {
                            mostImportantShapeSlot.ShapeViewDrawable = new Shape.ShapeViewDrawable(color, ShapeType.Blast);
                        }
                        if(match.Creates == Creates.Cross)
                        {
                            mostImportantShapeSlot.ShapeViewDrawable = new Shape.ShapeViewDrawable(color, ShapeType.Cross);
                        }
                        mostImportantShapeSlot.ShapeViewDrawable.Rectangle.X = mostImportantShapeSlot.Rectangle.X;
                        mostImportantShapeSlot.ShapeViewDrawable.Rectangle.Y = mostImportantShapeSlot.Rectangle.Y;
                    }
                }
            }
            return foundMatch;
        }
        List<Match> GetMatches()
        {
            return _gridModel.GetMatches();
        }
        Match GetMatchAtPosition(ShapeModel[,] shapeViewDrawableArray,Position position)
        {
            ShapeColor myColor = shapeViewDrawableArray[position.Row, position.Column].GetShapeColor();
            if(myColor == ShapeColor.None)
                return Match.Empty;
            List<Position> matchingShapesLeft = new List<Position>();
            for (int column = position.Column - 1; column >= 0; column--)
            {
                if (shapeViewDrawableArray[position.Row, column].GetShapeColor() == myColor)
                    matchingShapesLeft.Add(new Position(position.Row, column));
                else
                    break;
            }
            List<Position> matchingShapesRight = new List<Position>();
            for (int column = position.Column + 1; column < shapeViewDrawableArray.GetLength(1); column++)
            {
                if (shapeViewDrawableArray[position.Row, column].GetShapeColor() == myColor)
                    matchingShapesRight.Add(new Position(position.Row, column));
                else
                    break;
            }

            List<Position> matchingShapesAbove = new List<Position>();
            for (int row = position.Row - 1; row >= 0; row--)
            {
                if (shapeViewDrawableArray[row, position.Column].GetShapeColor() == myColor)
                    matchingShapesAbove.Add(new Position(row, position.Column));
                else
                    break;
            }
            List<Position> matchingShapesBelow = new List<Position>();
            for (int row = position.Row + 1; row < shapeViewDrawableArray.GetLength(0); row++)
            {
                if (shapeViewDrawableArray[row, position.Column].GetShapeColor() == myColor)
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
            return _gridModel.IsValidMove(from, to);
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

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(ShapeSlot shapeSlot in ShapeSlots)
            {
                shapeSlot.Draw(spriteBatch);
            }
        }
        public bool DoMove(Position from,Position to)
        {
            if (!MovesAllowed)
                return false;
            if (IsValidMove(from, to))
            {
                _gridModel.DoMove(new Move(from, to));
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
                        if (DoMove(from, to))
                        {
                            _currentlyHighlightedShapeSlot.IsHighlighted = false;
                            _currentlyHighlightedShapeSlot = null;
                        }
                    }
                }
            }
        }
        public void Swap(ShapeSlot shapeSlot,ShapeSlot otherShapeSlot)
        {
            Shape.ShapeViewDrawable temp = shapeSlot.ShapeViewDrawable;
            shapeSlot.ShapeViewDrawable = otherShapeSlot.ShapeViewDrawable;
            otherShapeSlot.ShapeViewDrawable = temp;
            shapeSlot.ShapeViewDrawable.DropTo(shapeSlot.Rectangle);
            otherShapeSlot.ShapeViewDrawable.DropTo(otherShapeSlot.Rectangle);
        }

        public void DebugFunctionAt(Point cursorPosition)
        {
            foreach (ShapeSlot slot in ShapeSlots)
            {
                if (slot.Rectangle.Contains(cursorPosition))
                {
                    slot.ClearSlot();
                }
            }
        }
        public void DestroyShape(Position position)
        {
            ShapeType type = ShapeSlots[position.Row, position.Column].ShapeViewDrawable.ShapeType;
            //to prevent infinite loops
            ShapeSlots[position.Row, position.Column].DestroyShape();
            if (type == ShapeType.Blast)
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
                if (position.Row < _rows - 1)
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

                foreach (Position blastPosition in positionsToClear)
                {
                    DestroyShape(blastPosition);
                }
            }
            if(type == ShapeType.Cross)
            {
                List<Position> positionsToClear = new List<Position>();
                for(int row = 0; row < _rows;row++)
                {
                    if (row == position.Row)
                        continue;
                    positionsToClear.Add(new Position(row, position.Column));
                }
                for(int column = 0;column < _columns;column++)
                {
                    if (column == position.Column)
                        continue;
                    positionsToClear.Add(new Position(position.Row, column));
                }
                foreach (Position crossPosition in positionsToClear)
                    DestroyShape(crossPosition);
            }
            if (type != ShapeType.None)
            {
                ShapeSlots[position.Row, position.Column].DestroyShape();
                Score += 100;
            }
        }
    }
}
