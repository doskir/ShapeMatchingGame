using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShapeMatchingGame.Shape;

namespace ShapeMatchingGame
{
    internal interface IShapeSlot
    {
        void Update();
        bool RecentlyDestroyed { get; set; }
        bool RecentlySwapped { get; set; }
        bool IsHighlighted { get; set; }
        bool IsEmpty { get; }
        ShapeColor ShapeColor { get; }
        ShapeType ShapeType { get; }
        void AssignShape(IShapeView shapeView);
    }
}
