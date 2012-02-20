namespace ShapeMatchingGame.Shape
{
    class ShapeModel : IShapeModel
    {
        public ShapeColor ShapeColor { get; private set; }
        public ShapeType ShapeType { get; private set; }
        public bool RecentlySwapped { get; set; }
        public bool RecentlyDropped { get; set; }
        public bool IsEmpty
        {
            get { return ShapeType == ShapeType.None && ShapeColor == ShapeColor.None; }
        }

        public ShapeModel(ShapeColor shapeColor,ShapeType shapeType)
        {
            ShapeColor = shapeColor;
            ShapeType = shapeType;
        }

        public static readonly ShapeModel Empty = new ShapeModel(ShapeColor.None, ShapeType.None);

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
