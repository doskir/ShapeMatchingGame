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
        public Shape Shape;
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
            Texture2D shapeTexture;
            Color shapeDrawColor;
            switch (Shape.Color)
            {
                case ShapeColor.Blue:
                    shapeTexture = Globals.Content.Load<Texture2D>("triangle");
                    shapeDrawColor = Color.Blue;
                    break;
                case ShapeColor.Green:
                    shapeTexture = Globals.Content.Load<Texture2D>("rectangle");
                    shapeDrawColor = Color.Green;
                    break;
                case ShapeColor.Orange:
                    shapeTexture = Globals.Content.Load<Texture2D>("pentagon");
                    shapeDrawColor = Color.Orange;
                    break;
                case ShapeColor.Red:
                    shapeTexture = Globals.Content.Load<Texture2D>("hexagon");
                    shapeDrawColor = Color.Red;
                    break;
                case ShapeColor.Violet:
                    shapeTexture = Globals.Content.Load<Texture2D>("heptagon");
                    shapeDrawColor = Color.Violet;
                    break;
                case ShapeColor.White:
                    shapeTexture = Globals.Content.Load<Texture2D>("rotatedRectangle");
                    shapeDrawColor = Color.White;
                    break;
                case ShapeColor.Yellow:
                    shapeTexture = Globals.Content.Load<Texture2D>("circle");
                    shapeDrawColor = Color.Yellow;
                    break;
                default:
                    //load an invisible texture
                    shapeTexture = Globals.Content.Load<Texture2D>("pixel");
                    shapeDrawColor = Color.Transparent;
                    break;
            }
            spriteBatch.Draw(shapeTexture, Rectangle, shapeDrawColor);
            if(Shape.Type == ShapeType.Blast)
            {
                Texture2D overlay = Globals.Content.Load<Texture2D>("bomb-icon");
                Rectangle centeredRectangle = new Rectangle(Rectangle.X + Rectangle.Width/4,
                                                            Rectangle.Y + Rectangle.Height/4, Rectangle.Width/2,
                                                            Rectangle.Height/2);

                spriteBatch.Draw(overlay, centeredRectangle, Color.Black);
            }
        }
    }
}
