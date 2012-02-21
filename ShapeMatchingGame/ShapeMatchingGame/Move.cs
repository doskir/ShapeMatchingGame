using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeMatchingGame
{
    class Move
    {
        public readonly Position From;
        public readonly Position To;
        public int PredictedScore;
        public Move(Position from,Position to,int predictedScore = 0)
        {
            From = from;
            To = to;
            PredictedScore = predictedScore;
        }
    }
}
