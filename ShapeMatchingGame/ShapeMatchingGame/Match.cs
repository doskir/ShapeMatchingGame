using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShapeMatchingGame
{
    class Match
    {
        public List<Position> InvolvedPositions;
        public Position Center;
        public Creates Creates;
        public bool IsValid;
        public Match(List<Position> positions,Position center,Creates creates)
        {
            InvolvedPositions = positions;
            Center = center;
            if (!InvolvedPositions.Contains(center))
                InvolvedPositions.Add(center);
            Creates = creates;
            IsValid = true;
        }

        public static Match Empty = new Match(new List<Position>(), new Position(-1, -1), Creates.Nothing)
                                        {IsValid = false};
    }
    enum Creates
    {
        Nothing,Blast,Cross,Star
    }
}
