using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeMatchingGame
{
    internal abstract class DrawableObject
    {
        public Rectangle Rectangle;
        public Texture2D Texture;
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
