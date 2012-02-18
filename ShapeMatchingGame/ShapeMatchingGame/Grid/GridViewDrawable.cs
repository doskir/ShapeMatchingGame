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
        public int Turn
        {
            get { return _gridModel.Turn; }
        }
        public Rectangle Rectangle;
        public GridViewDrawable(Point position,int rows,int columns,int slotWidth,int slotHeight)
        {
            _gridModel = new GridModel(rows, columns);
            _rows = _gridModel.Rows;
            _columns = _gridModel.Columns;
            ShapeSlots = new ShapeSlot[rows, columns];
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    ShapeSlots[row, column] = new ShapeSlot(new Rectangle(column * slotWidth + Rectangle.X, row * slotHeight + Rectangle.Y,
                                                    slotWidth, slotHeight));
                }
            }
            Rectangle = new Rectangle(position.X, position.Y, columns * slotWidth, rows * slotHeight);
            int addedScore;
            _gridModel.FinishTurn(out addedScore);
        }

        public bool MovesAllowed
        {
            get { return !_gridModel.HasEmptyFields; }
        }


        public void Update()
        {
            foreach (ShapeSlot shapeSlot in ShapeSlots)
                shapeSlot.Update();
        }

        public ShapeView[,] ShapeSlotsToArray()
        {
            return _gridModel.Shapes;
        }
        public GridModel ToGridModel()
        {
            return _gridModel;
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
            for (int row = 0; row < _rows; row++)
            {
                for (int column = 0; column < _columns; column++)
                {
                    ShapeSlots[row, column].Draw(spriteBatch);
                    ShapeViewDrawable shapeViewDrawable = new ShapeViewDrawable(_gridModel.Shapes[row, column]);
                    shapeViewDrawable.Rectangle = ShapeSlots[row, column].Rectangle;
                    shapeViewDrawable.Draw(spriteBatch);

                }
            }
        }

        public bool DoMove(Position from, Position to)
        {
            if (!MovesAllowed)
                return false;
            if (_gridModel.DoMove(new Move(from, to)))
            {
                int addedScore;
                _gridModel.FinishTurn(out addedScore);
                Score += addedScore;
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
