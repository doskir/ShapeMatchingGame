using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeMatchingGame
{
    interface IDrawableObject
    {
        Texture2D Texture { get; }
        Rectangle Rectangle { get; set; }
        void Draw(SpriteBatch spriteBatch);
    }
}
