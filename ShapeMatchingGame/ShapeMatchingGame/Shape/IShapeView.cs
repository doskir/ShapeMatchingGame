using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeMatchingGame.Shape
{
    internal interface IShapeView
    {
        ShapeColor ShapeColor { get; }
        ShapeType ShapeType { get; }
        bool IsEmpty { get; }
        bool RecentlySwapped { get; set; }
        bool RecentlyDropped { get; set; }
        bool RecentlyCreated { get; set; }
        bool Destroyed { get; set; }
        void Destroy();
    }
}
