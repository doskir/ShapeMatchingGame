using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeMatchingGame.Shape
{
    class ShapeViewDrawable : ShapeView,IDrawableObject
    {
        public Texture2D Texture { get; private set; }
        private readonly Texture2D _overlayTexture;
        private readonly Color _shapeDrawColor;
        public Rectangle Rectangle { get; set; }

        public ShapeViewDrawable(ShapeColor shapeColor, ShapeType shapeType, Rectangle creationRectangle):base(shapeColor,shapeType)
        {
            Rectangle = creationRectangle;
            switch (ShapeColor)
            {
                case ShapeColor.Blue:
                    Texture = Globals.Content.Load<Texture2D>("triangle");
                    _shapeDrawColor = Color.Blue;
                    break;
                case ShapeColor.Green:
                    Texture = Globals.Content.Load<Texture2D>("rectangle");
                    _shapeDrawColor = Color.Green;
                    break;
                case ShapeColor.Orange:
                    Texture = Globals.Content.Load<Texture2D>("pentagon");
                    _shapeDrawColor = Color.Orange;
                    break;
                case ShapeColor.Red:
                    Texture = Globals.Content.Load<Texture2D>("hexagon");
                    _shapeDrawColor = Color.Red;
                    break;
                case ShapeColor.Violet:
                    Texture = Globals.Content.Load<Texture2D>("heptagon");
                    _shapeDrawColor = Color.Violet;
                    break;
                case ShapeColor.White:
                    Texture = Globals.Content.Load<Texture2D>("rotatedRectangle");
                    _shapeDrawColor = Color.White;
                    break;
                case ShapeColor.Yellow:
                    Texture = Globals.Content.Load<Texture2D>("circle");
                    _shapeDrawColor = Color.Yellow;
                    break;
                default:
                    //load an invisible texture
                    Texture = Globals.Content.Load<Texture2D>("pixel");
                    _shapeDrawColor = Color.Transparent;
                    break;
            }
            if (ShapeType == ShapeType.Blast)
                _overlayTexture = Globals.Content.Load<Texture2D>("bomb-icon");
            if (ShapeType == ShapeType.Cross)
                _overlayTexture = Globals.Content.Load<Texture2D>("cross");
        }
        public ShapeViewDrawable(ShapeColor shapeColor,ShapeType shapeType):this(shapeColor,shapeType,new Rectangle(0,-50,50,50))
        {
        }
        public ShapeViewDrawable(IShapeView shapeView)
            : this(shapeView.ShapeColor, shapeView.ShapeType)
        {
            ShapeModel.RecentlyDropped = shapeView.RecentlyDropped;
            ShapeModel.RecentlySwapped = shapeView.RecentlyDropped;
            if (!shapeView.RecentlyCreated)
                RecentlyCreated = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rectangle, _shapeDrawColor);
            Rectangle centeredRectangle = new Rectangle(Rectangle.X + Rectangle.Width/4,
                                                        Rectangle.Y + Rectangle.Height/4, Rectangle.Width/2,
                                                        Rectangle.Height/2);
            if (_overlayTexture != null)
                spriteBatch.Draw(_overlayTexture, centeredRectangle, Color.Black);
        }
    }
}
