using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeMatchingGame.Shape
{
    public class ShapeView : IShapeView
    {
        protected readonly ShapeModel ShapeModel;

        public ShapeColor ShapeColor
        {
            get { return ShapeModel.ShapeColor; }
        }
        public ShapeType ShapeType
        {
            get { return ShapeModel.ShapeType; }
        }
        public bool IsEmpty
        {
            get { return ShapeModel.IsEmpty || Destroyed; }
        }
        public bool RecentlySwapped
        {
            get { return ShapeModel.RecentlySwapped; }
            set { ShapeModel.RecentlySwapped = value; }
        }
        public bool RecentlyDropped
        {
            get { return ShapeModel.RecentlyDropped; }
            set { ShapeModel.RecentlySwapped = value; }
        }


        public bool RecentlyCreated { get; set; }

        public bool Destroyed { get; set; }

        public ShapeView(ShapeColor color,ShapeType type)
        {
            ShapeModel = new ShapeModel(color, type);
            RecentlyCreated = true;
        }
    }
}
