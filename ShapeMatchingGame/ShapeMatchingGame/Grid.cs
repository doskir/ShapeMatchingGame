﻿using System;
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
        public void FillGrid()
        {
            for (int column = 0; column < _columns; column++)
            {
                ShapeSlot shapeSlot = ShapeSlots[0, column];
                if (shapeSlot.IsEmpty)
                {
                    Rectangle targetRectangle = new Rectangle(shapeSlot.Rectangle.X, -50, 50, 50);
                    bool willIntersect = false;
                    for (int row = 0; row < _rows; row++)
                    {
                        if (targetRectangle.Intersects(ShapeSlots[row, column].Shape.Rectangle))
                            willIntersect = true;
                    }
                    if (willIntersect)
                    {
                        break;
                    }
                    shapeSlot.Shape = _randomShapeGenerator.GetNextShape(ShapeType.Normal);
                    shapeSlot.Shape.Rectangle.X = shapeSlot.Rectangle.X;
                    shapeSlot.Shape.DropTo(shapeSlot.Rectangle);
                    shapeSlot.RecentlyDropped = true;
                    DropShapes();
                }
            }
        }
        public override void Update()
        {
            foreach (ShapeSlot shapeSlot in ShapeSlots)
                shapeSlot.Update();
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
                            shapeSlotBelow.RecentlyDropped = true;
                            currentShapeSlot.Shape = Shape.Empty;
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
                        ShapeSlots[position.Row, position.Column].Shape = Shape.Empty;
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
                            if (ShapeSlots[position.Row, position.Column].RecentlyDropped)
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


        public bool CanBeSwapped(ShapeSlot shapeSlot,ShapeSlot otherShapeSlot)
        {
            Position shapeSlotPosition = GetShapeSlotPosition(shapeSlot);
            Position otherShapeSlotPosition = GetShapeSlotPosition(otherShapeSlot);
            //check if the shapeslot is left of the other shapeslot
            if (shapeSlotPosition.Column + 1 == otherShapeSlotPosition.Column && shapeSlotPosition.Row == otherShapeSlotPosition.Row)
                return true;
            //check if the shapeslot is right of the other shapeslot
            if (shapeSlotPosition.Column - 1 == otherShapeSlotPosition.Column && shapeSlotPosition.Row == otherShapeSlotPosition.Row)
                return true;
            //check if the shapeslot is above the other shapeslot
            if (shapeSlotPosition.Row + 1 == otherShapeSlotPosition.Row && shapeSlotPosition.Column == otherShapeSlotPosition.Column)
                return true;
            //check if the shapeslot is below the other shapeslot
            if (shapeSlotPosition.Row - 1 == otherShapeSlotPosition.Row && shapeSlotPosition.Column == otherShapeSlotPosition.Column)
                return true;


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
                        if(CanBeSwapped(_currentlyHighlightedShapeSlot,slot))
                        {
                            //swap the slots
                            Swap(_currentlyHighlightedShapeSlot, slot);
                            //check if the move was valid
                            Match match = GetMatchAtPosition(ShapeSlotsToArray(), GetShapeSlotPosition(slot));
                            if (!match.IsValid)
                            {
                                match = GetMatchAtPosition(ShapeSlotsToArray(),
                                                           GetShapeSlotPosition(_currentlyHighlightedShapeSlot));
                                //not a valid move, roll back
                                if (!match.IsValid)
                                    Swap(_currentlyHighlightedShapeSlot, slot);
                            }
                            foreach (ShapeSlot shapeSlot in ShapeSlots)
                            {
                                shapeSlot.RecentlyDropped = false;
                                shapeSlot.RecentlySwappedTo = false;
                            }
                            //is a valid move
                            slot.RecentlySwappedTo = true;

                            _currentlyHighlightedShapeSlot.IsHighlighted = false;
                            _currentlyHighlightedShapeSlot = null;

                        }
                        return;
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
