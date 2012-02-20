using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeMatchingGame.Shape
{
    class ShapeViewDrawable : IShapeView,IDrawableObject
    {
        private ShapeModel _shapeModel;
        public Texture2D Texture { get; private set; }
        private Texture2D _overlayTexture;
        private Color shapeDrawColor;
        public Rectangle Rectangle { get; set; }
        public ShapeColor ShapeColor
        {
            get { return _shapeModel.ShapeColor; }
        }
        public ShapeType ShapeType
        {
            get { return _shapeModel.ShapeType; }
        }
        public bool IsEmpty
        {
            get { return _shapeModel.IsEmpty; }
        }
        public bool RecentlySwapped
        {
            get { return _shapeModel.RecentlySwapped; }
            set { _shapeModel.RecentlySwapped = value; }
        }
        public bool RecentlyDropped
        {
            get { return _shapeModel.RecentlyDropped; }
            set { _shapeModel.RecentlySwapped = value; }
        }



        public ShapeViewDrawable(ShapeColor shapeColor, ShapeType shapeType, Rectangle creationRectangle)
        {
            _shapeModel = new ShapeModel(shapeColor, shapeType);
            Rectangle = creationRectangle;
            switch (ShapeColor)
            {
                case ShapeColor.Blue:
                    Texture = Globals.Content.Load<Texture2D>("triangle");
                    shapeDrawColor = Color.Blue;
                    break;
                case ShapeColor.Green:
                    Texture = Globals.Content.Load<Texture2D>("rectangle");
                    shapeDrawColor = Color.Green;
                    break;
                case ShapeColor.Orange:
                    Texture = Globals.Content.Load<Texture2D>("pentagon");
                    shapeDrawColor = Color.Orange;
                    break;
                case ShapeColor.Red:
                    Texture = Globals.Content.Load<Texture2D>("hexagon");
                    shapeDrawColor = Color.Red;
                    break;
                case ShapeColor.Violet:
                    Texture = Globals.Content.Load<Texture2D>("heptagon");
                    shapeDrawColor = Color.Violet;
                    break;
                case ShapeColor.White:
                    Texture = Globals.Content.Load<Texture2D>("rotatedRectangle");
                    shapeDrawColor = Color.White;
                    break;
                case ShapeColor.Yellow:
                    Texture = Globals.Content.Load<Texture2D>("circle");
                    shapeDrawColor = Color.Yellow;
                    break;
                default:
                    //load an invisible texture
                    Texture = Globals.Content.Load<Texture2D>("pixel");
                    shapeDrawColor = Color.Transparent;
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
            _shapeModel.RecentlyDropped = shapeView.RecentlyDropped;
            _shapeModel.RecentlySwapped = shapeView.RecentlyDropped;
        }

        public static readonly ShapeViewDrawable Empty = new ShapeViewDrawable(ShapeColor.None, ShapeType.None);

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rectangle, shapeDrawColor);
            Rectangle centeredRectangle = new Rectangle(Rectangle.X + Rectangle.Width/4,
                                                        Rectangle.Y + Rectangle.Height/4, Rectangle.Width/2,
                                                        Rectangle.Height/2);
            if (_overlayTexture != null)
                spriteBatch.Draw(_overlayTexture, centeredRectangle, Color.Black);
        }
    }
}
