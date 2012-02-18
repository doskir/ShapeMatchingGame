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

        void Draw(SpriteBatch spriteBatch);
        void Update();
    }
}
