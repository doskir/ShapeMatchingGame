using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeMatchingGame
{
    class RandomShapeGenerator
    {
        private Random rand;
        public RandomShapeGenerator(int seed)
        {
            if (seed == -1)
                rand = new Random();
            else
                rand = new Random(seed);
        }

        public Shape GetNextShape(ShapeType type)
        {
            Shape shape = new Shape(ShapeColor.None, type);
            shape.Color = (ShapeColor) rand.Next(7);
            return shape;
        }

    }
}
