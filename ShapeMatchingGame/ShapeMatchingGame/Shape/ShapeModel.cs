﻿namespace ShapeMatchingGame.Shape
{
    public class ShapeModel : IShapeModel
    {
        #region Properties
        public ShapeColor ShapeColor { get; private set; }
        public ShapeType ShapeType { get; private set; }
        public bool RecentlySwapped { get; set; }
        public bool RecentlyDropped { get; set; }
        
        public bool IsEmpty
        {
            get { return ShapeType == ShapeType.None && ShapeColor == ShapeColor.None; }
        }
        #endregion
        public ShapeModel(ShapeColor shapeColor,ShapeType shapeType)
        {
            ShapeColor = shapeColor;
            ShapeType = shapeType;
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
        None,
        Any //means it matches any color
    }

    public enum ShapeType
    {
        None, Normal, Blast, Cross, Star
    }
}
