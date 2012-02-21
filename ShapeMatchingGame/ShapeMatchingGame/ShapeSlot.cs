using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShapeMatchingGame.Shape;

namespace ShapeMatchingGame
{
    internal class ShapeSlot : IShapeSlot,IDrawableObject
    {
        public ShapeColor ShapeColor
        {
            get { return _shapeViewDrawable.ShapeColor; }
        }
        public ShapeType ShapeType
        {
            get { return _shapeViewDrawable.ShapeType; }
        }
        public bool RecentlyDestroyed { get;set; }
        public bool RecentlySwapped
        {
            get { return _shapeViewDrawable.RecentlySwapped; }
            set { _shapeViewDrawable.RecentlySwapped = value; }
        }
        public bool IsHighlighted { get; set; }
        public bool Moving
        {
            get
            {
                return _shapeViewDrawable.Rectangle.X != Rectangle.X || _shapeViewDrawable.Rectangle.Y != Rectangle.Y;
            }
        }
        public bool IsEmpty
        {
            get { return _shapeViewDrawable.IsEmpty; }
        }
        public Texture2D Texture { get; private set; }
        public Rectangle Rectangle
        {
            get { return _rectangle; }
            set { _rectangle = value; }
        }

        private ShapeViewDrawable _shapeViewDrawable;
        public ShapeViewDrawable AssignedShape
        {
            get { return _shapeViewDrawable; }
        }
        private Rectangle _rectangle;
        private Vector2 _moveSpeed = new Vector2(6.0f, 6.0f);


        public ShapeSlot(Rectangle rectangle)
        {
            _rectangle = rectangle;
            _shapeViewDrawable = new ShapeViewDrawable(ShapeColor.None, ShapeType.None);
            Texture= Globals.Content.Load<Texture2D>("shapeSlotFrame");
        }
        public void AssignShape(IShapeView shapeView)
        {
            if (shapeView is ShapeViewDrawable)
            {
               _shapeViewDrawable = (ShapeViewDrawable)shapeView;
            }
            else
            {
                _shapeViewDrawable = new ShapeViewDrawable(shapeView);
            }
            if(shapeView.RecentlyCreated)
            {
                _shapeViewDrawable.Rectangle = new Rectangle(Rectangle.X, _shapeViewDrawable.Rectangle.Y,
                                                             _shapeViewDrawable.Rectangle.Width,
                                                             _shapeViewDrawable.Rectangle.Height);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle renderingRectangle = new Rectangle(_rectangle.X, _rectangle.Y, _rectangle.Width,
                                                         _rectangle.Height);
            Color frameColor = Color.Black;
            if (IsHighlighted)
                frameColor = Color.OrangeRed;
            else if (RecentlyDestroyed)
                frameColor = Color.Yellow;
            spriteBatch.Draw(Texture, renderingRectangle, frameColor);
            _shapeViewDrawable.Draw(spriteBatch);
        }

        public void Update()
        {
            if (Moving)
            {
                //TODO: check if this sets the rectangle in the shapeview
                Rectangle newShapeViewRectangle = _shapeViewDrawable.Rectangle;
               if(_shapeViewDrawable.Rectangle.X > Rectangle.X)
               {
                   //move the rectangle to the left
                   newShapeViewRectangle.X -= (int)_moveSpeed.X;
                   //check if we moved it too far
                   if(newShapeViewRectangle.X < Rectangle.X)
                   {
                       //move it to the same position
                       newShapeViewRectangle.X = Rectangle.X;
                   }
               }
               else if (_shapeViewDrawable.Rectangle.X < Rectangle.X)
               {
                   //move the rectangle to the right
                   newShapeViewRectangle.X += (int) _moveSpeed.X;
                   //check if we moved it too far
                   if (newShapeViewRectangle.X > Rectangle.X)
                   {
                       //move it to the same position
                       newShapeViewRectangle.X = Rectangle.X;
                   }
               }
                if (_shapeViewDrawable.Rectangle.Y > Rectangle.Y)
                {
                    //move the rectangle to the left
                    newShapeViewRectangle.Y -= (int)_moveSpeed.Y;
                    //check if we moved it too far
                    if (newShapeViewRectangle.Y < Rectangle.Y)
                    {
                        //move it to the same position
                        newShapeViewRectangle.Y = Rectangle.Y;
                    }
                }
                else if (_shapeViewDrawable.Rectangle.Y < Rectangle.Y)
                {
                    //move the rectangle to the right
                    newShapeViewRectangle.Y += (int)_moveSpeed.Y;
                    //check if we moved it too far
                    if (newShapeViewRectangle.Y > Rectangle.Y)
                    {
                        //move it to the same position
                        newShapeViewRectangle.Y = Rectangle.Y;
                    }
                }
                _shapeViewDrawable.Rectangle = newShapeViewRectangle;
            }
        }
    }

}
    
