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
        private ShapeSlot _currentlyHighlightedShapeSlot;
        public int Score;
        public int Turn = 1;
        public Rectangle Rectangle;
        public GridViewDrawable(Point position,int rows,int columns,int slotWidth,int slotHeight)
        {
            _gridModel = new GridModel(rows, columns);
            ShapeSlots = new ShapeSlot[rows, columns];
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    ShapeSlots[row, column] =
                        new ShapeSlot(new Rectangle(column * slotWidth + Rectangle.X, row * slotHeight + Rectangle.Y,
                                                    slotWidth, slotHeight));
                }
            }
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
            _gridModel.FillGrid();
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
            _gridModel.DropShapes();
        }
        public ShapeView[,] ShapeSlotsToArray()
        {
            return _gridModel.Shapes;
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
            return _gridModel;
            GridModel gridModel = new GridModel(ShapeSlotsToArray());
            return gridModel;
        }
        public void HandleMatches()
        {
            int gainedScore;
            _gridModel.HandleMatches(out gainedScore);
            Score += gainedScore;
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
    }
}
