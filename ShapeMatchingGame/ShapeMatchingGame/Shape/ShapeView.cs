using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeMatchingGame.Shape
{
    public class ShapeView
    {
        private ShapeModel _shapeModel;
        public ShapeColor ShapeColor
        {
           get { return _shapeModel.GetShapeColor(); }
        }
        public ShapeType ShapeType
        {
            get { return _shapeModel.GetShapeType(); }
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

        public ShapeView(ShapeColor color,ShapeType type)
        {
            _shapeModel = new ShapeModel(color, type);
        }

        public static ShapeView Empty = new ShapeView(ShapeColor.None, ShapeType.None);
    }
}
