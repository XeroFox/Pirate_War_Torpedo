using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pirate_War_v1
{
    internal class FinalData
    {
        public string P1_NAME { get; set; }
        public string P2_NAME { get; set; }
        public int P1_HIT { get; set; }
        public int P2_HIT { get; set; }
        public int P1_MISS { get; set; }
        public int P2_MISS { get; set; }
        public int TURN { get; set; }
        public string WON { get; set; }

        public FinalData(string p1_NAME, string p2_NAME, int p1_HIT, int p2_HIT, int p1_MISS, int p2_MISS, int tURN, string wON)
        {
            P1_NAME = p1_NAME;
            P2_NAME = p2_NAME;
            P1_HIT = p1_HIT;
            P2_HIT = p2_HIT;
            P1_MISS = p1_MISS;
            P2_MISS = p2_MISS;
            TURN = tURN;
            WON = wON;
        }
    }
}
