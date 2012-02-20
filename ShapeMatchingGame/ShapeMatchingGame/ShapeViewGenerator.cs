using System;
using ShapeMatchingGame.Shape;

namespace ShapeMatchingGame
{
    class ShapeViewGenerator<TShapeViewType> where TShapeViewType : IShapeView
    {
        private readonly Random _rand;
        public ShapeViewGenerator(int seed = 0)
        {
            if (seed == 0)
                _rand = new Random();
            else
                _rand = new Random(seed);
        }
        public IShapeView GetNextShapeView(ShapeType type)
        {
            //TODO: Find out how many colors exist instead of having a magic number
            ShapeColor color = (ShapeColor) _rand.Next(7);
            Type specificType = typeof(TShapeViewType);
            IShapeView shapeView = Activator.CreateInstance(specificType, color, type) as IShapeView;
            return shapeView;
        }

        public IShapeView GetEmptyShapeView()
        {
            Type specificType = typeof (TShapeViewType);
            IShapeView shapeView = Activator.CreateInstance(specificType, ShapeColor.None, ShapeType.None) as IShapeView;
            return shapeView;
        }
    }
}
