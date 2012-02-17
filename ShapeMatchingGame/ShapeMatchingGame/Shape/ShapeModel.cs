namespace ShapeMatchingGame.Shape
{
    class ShapeModel
    {
        public ShapeColor Color;
        public ShapeType Type;
        public ShapeModel(ShapeColor color,ShapeType type)
        {
            Color = color;
            Type = type;
        }
        public bool IsEmpty
        {
            get { return Type == ShapeType.None && Color == ShapeColor.None; }
        }
        public static readonly ShapeModel Empty = new ShapeModel(ShapeColor.None, ShapeType.None);
        public bool RecentlySwapped;
        public bool RecentlyDropped;
        public ShapeModel DeepCopy()
        {
            ShapeModel newShape = new ShapeModel(Color, Type);
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
