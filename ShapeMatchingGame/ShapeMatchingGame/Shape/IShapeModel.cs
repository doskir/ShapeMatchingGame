using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeMatchingGame.Shape
{
    interface IShapeModel
    {
        ShapeColor ShapeColor { get; }
        ShapeType ShapeType { get; }
        bool IsEmpty { get; }
        bool RecentlySwapped { get; }
        bool RecentlyDropped { get; }
    }
}
