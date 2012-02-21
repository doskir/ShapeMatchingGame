using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShapeMatchingGame.Shape;

namespace ShapeMatchingGame.Grid
{
    class GridViewDrawable : GridModel<ShapeViewDrawable>, IDrawableObject
    {
        public readonly ShapeSlot[,] ShapeSlots;
        private ShapeSlot _currentlyHighlightedShapeSlot;
        private Rectangle _rectangle;
        public Rectangle Rectangle
        {
            get { return _rectangle; }
            set { _rectangle = value; }
        }

        public GridViewDrawable(Point position,int rows,int columns,int slotWidth,int slotHeight) : base(rows,columns)
        {
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
            AssignShapes();
        }


        public void Update()
        {
            foreach (ShapeSlot shapeSlot in ShapeSlots)
                shapeSlot.Update();
        }

        public GridModel<ShapeViewDrawable> ToGridModel()
        {
            return CloneRawGrid();
        }

        public Position GetShapeSlotPosition(ShapeSlot shapeSlot)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
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
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    ShapeSlots[row, column].Draw(spriteBatch);
                }
            }
        }

        public bool DoMove(Position from, Position to)
        {
            if(base.DoMove(new Move(from,to)))
            {
                //swap the slots
                IShapeView temp = ShapeSlots[from.Row, from.Column].AssignedShape;
                ShapeSlots[from.Row, from.Column].AssignShape(ShapeSlots[to.Row, to.Column].AssignedShape);
                ShapeSlots[to.Row, to.Column].AssignShape(temp);
                foreach(ShapeSlot shapeSlot in ShapeSlots)
                {
                    shapeSlot.RecentlyDestroyed = shapeSlot.AssignedShape.Destroyed;
                }
                AssignShapes();
                return true;
            }
            return false;
        }
        private void AssignShapes()
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    if (ShapeSlots[row, column].AssignedShape != Shapes[row, column])
                    {
                        ShapeSlots[row, column].AssignShape(Shapes[row, column]);
                    }
                }
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
