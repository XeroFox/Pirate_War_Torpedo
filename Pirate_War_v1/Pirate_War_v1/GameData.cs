using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pirate_War_v1
{
    internal class GameData
    {
        public int P1_HIT { get; set; }
        public int P2_HIT { get; set; }
        public int P1_MISS { get; set; }
        public int P2_MISS { get; set; }
        public int TURN { get; set; }
        public List<string> STEPS { get; set; }

        public GameData()
        {
            P1_HIT = 0;
            P2_HIT = 0;
            TURN = 1;
            STEPS = new List<string>();
            P1_MISS = 0;
            P2_MISS = 0;
        }

        public void saveMove(int XX, int YY, bool hit, string player)
        {
            string[] characters = new string[] { "A", "B", "C", "D", "E", "F", "G", "H" };
            string savingData = String.Format("{0} - {1}{2}:{3}", player, characters[XX], YY, (hit ? "X" : "O"));
            STEPS.Add(savingData);
        }

        public string toJSON()
        {

            string jsonString = jsonString = JsonSerializer.Serialize(this);

            return jsonString;
        }
    }
}
