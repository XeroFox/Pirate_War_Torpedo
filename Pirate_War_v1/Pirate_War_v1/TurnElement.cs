using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pirate_War_v1
{
    internal class TurnElement
    {
        public string Name { get; set; }
        public int Round { get; set; }
        public Coordinates coordinates { get; set; }

        public TurnElement(string name, int round, Coordinates coord)
        {
            Name = name;
            Round = round;
            coordinates = coord;
        }

        public override string? ToString()
        {
            char[] characters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
            return Round + ": " + Name + " - " + characters[coordinates.Y-1] + coordinates.X;
        }
    }
}
