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
        public bool CountingScore;
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
            for(int column = 0;column < _columns;column++)
            {
                bool shapeCreated;
                do
                {
                    shapeCreated = false;
                    ShapeSlot shapeSlot = ShapeSlots[0, column];
                    if (shapeSlot.IsEmpty)
                    {
                        shapeSlot.Shape = _randomShapeGenerator.GetNextShape(ShapeType.Normal);
                        shapeCreated = true;
                        DropShapes();
                    }
                } while (shapeCreated);
            }
        }
        public void Update()
        {
            do
            {
                DropShapes();
                FillGrid();
            } while (CheckForMatches());
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
        public bool CheckForMatches()
        {
            Shape[,] shapeArray = ShapeSlotsToArray();
            for (int row = 0; row < _rows; row++)
            {
                for (int column = 0; column < _columns; column++)
                {
                    Position position = new Position(row, column);
                    List<Position> matchedShapes = MatchedShapesAt(shapeArray, position);
                    if (matchedShapes.Count > 0)
                    {
                        matchedShapes.Add(position);
                        foreach (Position matchedPosition in matchedShapes)
                        {
                            ShapeSlots[matchedPosition.Row, matchedPosition.Column].Shape = Shape.Empty;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        List<Position> MatchedShapesAt(Shape[,] shapeArray, Position origin)
        {
            ShapeColor myColor = shapeArray[origin.Row, origin.Column].Color;
            List<Position> matchingShapesLeft = new List<Position>();
            for (int column = origin.Column - 1; column >= 0; column--)
            {
                if (shapeArray[origin.Row, column].Color == myColor)
                    matchingShapesLeft.Add(new Position(origin.Row, column));
                else
                    break;
            }
            List<Position> matchingShapesRight = new List<Position>();
            for (int column = origin.Column + 1; column < 8; column++)
            {
                if (shapeArray[origin.Row, column].Color == myColor)
                    matchingShapesRight.Add(new Position(origin.Row, column));
                else
                    break;
            }

            List<Position> matchingShapesAbove = new List<Position>();
            for (int row = origin.Row - 1; row >= 0; row--)
            {
                if (shapeArray[row, origin.Column].Color == myColor)
                    matchingShapesAbove.Add(new Position(row, origin.Column));
                else
                    break;
            }
            List<Position> matchingShapesBelow = new List<Position>();
            for (int row = origin.Row + 1; row < 8; row++)
            {
                if (shapeArray[row, origin.Column].Color == myColor)
                    matchingShapesBelow.Add(new Position(row, origin.Column));
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

            if (horizontalMatch.Count >= 3)
            {
                //blast shape
            }
            else if (horizontalMatch.Count >= 2 && verticalMatch.Count >= 2)
            {
                //Cross Shape will be created
            }

            List<Position> involvedShapes = new List<Position>(horizontalMatch);
            involvedShapes.AddRange(verticalMatch);
            return involvedShapes;
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
                    slot.Shape = Shape.Empty;
                    return;
                }
            }
            
        }
    }
}
