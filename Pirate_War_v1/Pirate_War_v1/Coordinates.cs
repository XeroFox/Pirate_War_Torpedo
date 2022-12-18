using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pirate_War_v1
{
    public class Coordinates
    {
        public int X { get; }
        public int Y { get; }
        public int Value { get; set; }

        public Coordinates(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
            Value = 0;
        }

        public override string ToString()
        {
            return "X: " + X + " - Y:" + Y + " = " + Value;
        }

        public bool compareCoord(Coordinates c1)
        {
            bool result = false;

            if(c1.X == this.X && c1.Y == this.Y)
            {
                return true;
            }

            return result;
        }

        public override int GetHashCode()
        {
            return $"{this.X}{this.Y}{this.Value}".GetHashCode();
        }
    }
}
