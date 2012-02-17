using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShapeMatchingGame.Shape;

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

        public Shape.ShapeViewDrawable GetNextShape(ShapeType type)
        {
            Shape.ShapeViewDrawable shapeViewDrawable = new Shape.ShapeViewDrawable(ShapeColor.None, type);
            shapeViewDrawable.Color = (ShapeColor) rand.Next(7);
            return shapeViewDrawable;
        }

    }
}
