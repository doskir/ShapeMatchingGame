using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeMatchingGame
{
    class Shape
    {
        public ShapeColor Color;
        public ShapeType Type;
        public Shape(ShapeColor color,ShapeType type)
        {
            Color = color;
            Type = type;
        }

        public static readonly Shape Empty = new Shape(ShapeColor.None, ShapeType.None);
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
        None,Normal,Blast,Cross,Star
    }
}
