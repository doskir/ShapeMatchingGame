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
        public RandomShapeGenerator()
        {
            rand = new Random();
        }

        public ShapeViewDrawable GetNextShapeViewDrawable(ShapeType type)
        {
            ShapeColor color = (ShapeColor) rand.Next(7);
            Shape.ShapeViewDrawable shapeViewDrawable = new Shape.ShapeViewDrawable(color, type);
            return shapeViewDrawable;
        }
        public ShapeView GetNextShapeView(ShapeType type)
        {
            ShapeColor color = (ShapeColor) rand.Next(7);
            ShapeView shapeView = new ShapeView(color, type);
            return shapeView;
        }
        public ShapeModel GetNextShape(ShapeType type)
        {
            ShapeColor color = (ShapeColor)rand.Next(7);
            Shape.ShapeModel shapeModel = new Shape.ShapeModel(color, type);
            return shapeModel;
        }

    }
}
