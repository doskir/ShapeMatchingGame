using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeMatchingGame
{
    class ShapeSlot : DrawableObject
    {
        private Shape _shape;
        public Shape Shape
        {
            get { return _shape; }
            set {
                _shape = value;
                _shape.DropTo(Rectangle);
            }
        }
        public bool RecentlyDropped;
        public bool RecentlySwappedTo;
        public bool IsHighlighted;
        public ShapeSlot(Rectangle rectangle)
        {
            Rectangle = rectangle;
            Shape = new Shape(ShapeColor.None, ShapeType.None);
            Texture = Globals.Content.Load<Texture2D>("shapeSlotFrame");
        }
        public bool IsEmpty
        {
            get { return Shape.Color == ShapeColor.None || Shape.Type == ShapeType.None; }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle renderingRectangle = new Rectangle(Rectangle.X, Rectangle.Y, Rectangle.Width,
                                             Rectangle.Height);
            Color frameColor = Color.Black;
            if(IsHighlighted)
                frameColor = Color.OrangeRed;
            else if(RecentlyDropped)
                frameColor = Color.Yellow;
            spriteBatch.Draw(Texture, renderingRectangle, frameColor);
            if(Shape.Rectangle.X == 0 && Shape.Rectangle.Y == 0)
            {
                Shape.Rectangle.X = Rectangle.X;
                Shape.Rectangle.Y = Rectangle.Y;
            }
            Shape.Draw(spriteBatch);
        }

        public override void Update()
        {
            Shape.Update();
        }
    }
}
