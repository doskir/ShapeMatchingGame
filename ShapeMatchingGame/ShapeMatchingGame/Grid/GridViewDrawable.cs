using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShapeMatchingGame.Shape;

namespace ShapeMatchingGame.Grid
{
    class GridViewDrawable : IDrawableObject
    {
        private GridModelProxy<ShapeViewDrawable> _gridModel;
        public ShapeSlot[,] ShapeSlots;
        private int _rows;
        private int _columns;
        private ShapeSlot _currentlyHighlightedShapeSlot;
        public int Score
        {
            get { return _gridModel.Score; }
        }
        public int Turn
        {
            get { return _gridModel.Turn; }
        }
        public Rectangle _rectangle;
        public Rectangle Rectangle
        {
            get { return _rectangle; }
            set { _rectangle = value; }
        }

        public GridViewDrawable(Point position,int rows,int columns,int slotWidth,int slotHeight)
        {
            _gridModel = new GridModelProxy<ShapeViewDrawable>(rows, columns);
            _rows = _gridModel.Rows;
            _columns = _gridModel.Columns;
            ShapeSlots = new ShapeSlot[rows, columns];
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    ShapeSlots[row, column] = new ShapeSlot(new Rectangle(column * slotWidth + _rectangle.X, row * slotHeight + _rectangle.Y,
                                                    slotWidth, slotHeight));
                }
            }
            _rectangle = new Rectangle(position.X, position.Y, columns * slotWidth, rows * slotHeight);
        }


        public void Update()
        {
            foreach (ShapeSlot shapeSlot in ShapeSlots)
                shapeSlot.Update();
        }

        public GridModel<IShapeView> ToGridModel()
        {
            return _gridModel.CloneRawGrid();
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

        public Texture2D Texture
        {
            get { throw new NotImplementedException(); }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            for (int row = 0; row < _rows; row++)
            {
                for (int column = 0; column < _columns; column++)
                {
                    ShapeSlots[row, column].Draw(spriteBatch);
                }
            }
        }

        public bool DoMove(Position from, Position to)
        {
            return _gridModel.DoMove(new Move(from, to));
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
    }
}
