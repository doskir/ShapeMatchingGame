namespace ShapeMatchingGame.Shape
{
    class ShapeModel
    {
        readonly ShapeColor _color;
        public ShapeColor GetShapeColor()
        {
            return _color;
        }

        readonly ShapeType _type;
        public ShapeType GetShapeType()
        {
            return _type;
        }
        public ShapeModel(ShapeColor color,ShapeType type)
        {
            _color = color;
            _type = type;
        }
        public bool IsEmpty
        {
            get { return _type == ShapeType.None && _color == ShapeColor.None; }
        }
        public static readonly ShapeModel Empty = new ShapeModel(ShapeColor.None, ShapeType.None);
        public bool RecentlySwapped;
        public bool RecentlyDropped;
        public ShapeModel DeepCopy()
        {
            ShapeModel newShape = new ShapeModel(_color, _type);
            return newShape;
        }
    }

    public enum ShapeColor
    {
        Blue = 0,
        Green = 1,
        Orange = 2,
        Red = 3,
        Violet = 4,
        White = 5,
        Yellow = 6,
        None

    }

    public enum ShapeType
    {
        None, Normal, Blast, Cross, Star
    }
}
