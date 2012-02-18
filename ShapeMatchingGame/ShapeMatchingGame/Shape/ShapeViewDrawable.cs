﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeMatchingGame.Shape
{
    class ShapeViewDrawable : ShapeView,IDrawableObject
    {
        public Rectangle Rectangle;
        public ShapeViewDrawable(ShapeColor color,ShapeType type):this(color,type,new Rectangle(0,-50,50,50))
        {
        }
        public ShapeViewDrawable(ShapeColor color,ShapeType type,Rectangle creationRectangle):base(color,type)
        {
            Rectangle = creationRectangle;
        }
        public ShapeViewDrawable(ShapeView shapeView):this(shapeView.ShapeColor,shapeView.ShapeType,new Rectangle(0,0,50,50))
        {
            
        }

        public static readonly ShapeViewDrawable Empty = new ShapeViewDrawable(ShapeColor.None, ShapeType.None);
        private Rectangle _targetRectangle = Rectangle.Empty;
        private Vector2 _moveSpeed;
        public bool Moving;
        public void DropTo(Rectangle rectangle)
        {
            _targetRectangle = rectangle;
            _moveSpeed.Y = 6.0f;
            if (_targetRectangle.Y < Rectangle.Y)
                _moveSpeed.Y *= -1;
            _moveSpeed.X = 6.0f;
            if (_targetRectangle.X < Rectangle.X)
                _moveSpeed.X *= -1;
            Moving = true;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            Texture2D shapeTexture;
            Color shapeDrawColor;
            switch (ShapeColor)
            {
                case ShapeColor.Blue:
                    shapeTexture = Globals.Content.Load<Texture2D>("triangle");
                    shapeDrawColor = Microsoft.Xna.Framework.Color.Blue;
                    break;
                case ShapeColor.Green:
                    shapeTexture = Globals.Content.Load<Texture2D>("rectangle");
                    shapeDrawColor = Microsoft.Xna.Framework.Color.Green;
                    break;
                case ShapeColor.Orange:
                    shapeTexture = Globals.Content.Load<Texture2D>("pentagon");
                    shapeDrawColor = Microsoft.Xna.Framework.Color.Orange;
                    break;
                case ShapeColor.Red:
                    shapeTexture = Globals.Content.Load<Texture2D>("hexagon");
                    shapeDrawColor = Microsoft.Xna.Framework.Color.Red;
                    break;
                case ShapeColor.Violet:
                    shapeTexture = Globals.Content.Load<Texture2D>("heptagon");
                    shapeDrawColor = Microsoft.Xna.Framework.Color.Violet;
                    break;
                case ShapeColor.White:
                    shapeTexture = Globals.Content.Load<Texture2D>("rotatedRectangle");
                    shapeDrawColor = Microsoft.Xna.Framework.Color.White;
                    break;
                case ShapeColor.Yellow:
                    shapeTexture = Globals.Content.Load<Texture2D>("circle");
                    shapeDrawColor = Microsoft.Xna.Framework.Color.Yellow;
                    break;
                default:
                    //load an invisible texture
                    shapeTexture = Globals.Content.Load<Texture2D>("pixel");
                    shapeDrawColor = Microsoft.Xna.Framework.Color.Transparent;
                    break;
            }
            spriteBatch.Draw(shapeTexture, Rectangle, shapeDrawColor);
            Texture2D overlay = null;
            Rectangle centeredRectangle = new Rectangle(Rectangle.X + Rectangle.Width/4,
                                                        Rectangle.Y + Rectangle.Height/4, Rectangle.Width/2,
                                                        Rectangle.Height/2);
            if (ShapeType == ShapeType.Blast)
                overlay = Globals.Content.Load<Texture2D>("bomb-icon");
            if (ShapeType == ShapeType.Cross)
                overlay = Globals.Content.Load<Texture2D>("cross");
            if (overlay != null)
                spriteBatch.Draw(overlay, centeredRectangle, Microsoft.Xna.Framework.Color.Black);
        }

        public void Update()
        {
            if(!_targetRectangle.IsEmpty)
            {
                Rectangle.X += (int)_moveSpeed.X;
                Rectangle.Y += (int)_moveSpeed.Y;
                if ((Rectangle.Y >= _targetRectangle.Y && _moveSpeed.Y >= 0)
                    || Rectangle.Y <= _targetRectangle.Y && _moveSpeed.Y < 0)
                {
                    Rectangle.Y = _targetRectangle.Y;
                    _moveSpeed.Y = 0;
                }
                if ((Rectangle.X >= _targetRectangle.X && _moveSpeed.X >= 0)
                    || Rectangle.X <= _targetRectangle.X && _moveSpeed.X < 0)
                {
                    _moveSpeed.X = 0;
                    Rectangle.X = _targetRectangle.X;
                }
                if (_moveSpeed.X == 0 && _moveSpeed.Y == 0)
                {
                    _targetRectangle = Rectangle.Empty;
                    Moving = false;
                }
            }
        }
    }
}
