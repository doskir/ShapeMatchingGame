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
        #region Properties
        private ShapeSlot _currentlyHighlightedShapeSlot;

        private Rectangle _rectangle;
        public Rectangle Rectangle
        {
            get { return _rectangle; }
            set { _rectangle = value; }
        }
        public Texture2D Texture
        {
            //TODO: use it for a nice border frame for the whole grid
            get { throw new NotImplementedException(); }
        }
        #endregion

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
        public new bool DoMove(Move move)
        {
            if(base.DoMove(move))
            {
                //after doing a move the shape will still be associated with the wrong slot...
                SwapAssignedShapes(ShapeSlots[move.From.Row, move.From.Column], ShapeSlots[move.To.Row, move.To.Column]);
                //the shapeslots still have their shapes assigned, we can use this to find out which shapes have been destroyed
                //and mark their slots accordingly                                 
                foreach(ShapeSlot shapeSlot in ShapeSlots)
                {
                    shapeSlot.RecentlyDestroyed = shapeSlot.AssignedShape.Destroyed;
                }
                AssignShapes();
                return true;
            }
            return false;
        }
        public void Click(Point point)
        {
            foreach (ShapeSlot slot in ShapeSlots)
            {
                if (slot.Rectangle.Contains(point))
                {
                    if (slot == _currentlyHighlightedShapeSlot)
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
                        if (DoMove(new Move(from, to)))
                        {
                            _currentlyHighlightedShapeSlot.IsHighlighted = false;
                            _currentlyHighlightedShapeSlot = null;
                        }
                    }
                }
            }
        }
        public void DragTo(Point point)
        {
            //cant drag anywhere if we don't have a highlighted slot
            if (_currentlyHighlightedShapeSlot == null || _currentlyHighlightedShapeSlot.IsEmpty)
                return;
            if (_currentlyHighlightedShapeSlot.Rectangle.Contains(point))
            {
                //didn't actually drag anywhere
                return;
            }
            foreach (ShapeSlot slot in ShapeSlots)
            {
                if (slot.Rectangle.Contains(point))
                {
                    Position from = GetShapeSlotPosition(_currentlyHighlightedShapeSlot);
                    Position to = GetShapeSlotPosition(slot);
                    if (DoMove(new Move(from, to)))
                    {
                        _currentlyHighlightedShapeSlot.IsHighlighted = false;
                        _currentlyHighlightedShapeSlot = null;
                    }
                }
            }

        }
        private void SwapAssignedShapes(ShapeSlot shapeSlot,ShapeSlot other)
        {
            IShapeView temp = shapeSlot.AssignedShape;
            shapeSlot.AssignShape(other.AssignedShape);
            other.AssignShape(temp);
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
        private Position GetShapeSlotPosition(ShapeSlot shapeSlot)
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


    }
}
