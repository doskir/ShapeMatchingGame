using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShapeMatchingGame.Shape;

namespace ShapeMatchingGame
{
    internal class ShapeSlot : IDrawableObject
    {
        private Shape.ShapeViewDrawable _shapeViewDrawable;

        public Shape.ShapeViewDrawable ShapeViewDrawable
        {
            get { return _shapeViewDrawable; }
            set
            {
                _shapeViewDrawable = value;
                _shapeViewDrawable.DropTo(Rectangle);
            }
        }
        public Rectangle Rectangle;
        public Texture2D Texture;
        public bool RecentlyDestroyed;
        public bool RecentlySwappedTo;
        public bool IsHighlighted;

        public ShapeSlot(Rectangle rectangle)
        {
            Rectangle = rectangle;
            ShapeViewDrawable = new Shape.ShapeViewDrawable(ShapeColor.None, ShapeType.None);
            Texture = Globals.Content.Load<Texture2D>("shapeSlotFrame");
        }

        public bool IsEmpty
        {
            get { return ShapeViewDrawable.IsEmpty; }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle renderingRectangle = new Rectangle(Rectangle.X, Rectangle.Y, Rectangle.Width,
                                                         Rectangle.Height);
            Color frameColor = Color.Black;
            if (IsHighlighted)
                frameColor = Color.OrangeRed;
            else if (RecentlyDestroyed)
                frameColor = Color.Yellow;
            spriteBatch.Draw(Texture, renderingRectangle, frameColor);
            if (ShapeViewDrawable.Rectangle.X == 0 && ShapeViewDrawable.Rectangle.Y == 0)
            {
                ShapeViewDrawable.Rectangle.X = Rectangle.X;
                ShapeViewDrawable.Rectangle.Y = Rectangle.Y;
            }
            ShapeViewDrawable.Draw(spriteBatch);
        }

        public void Update()
        {
            ShapeViewDrawable.Update();
        }

        public void ClearSlot()
        {
            ShapeViewDrawable = ShapeViewDrawable.Empty;
        }

        public void DestroyShape()
        {
            ClearSlot();
            RecentlyDestroyed = true;
        }
    }
}
    
