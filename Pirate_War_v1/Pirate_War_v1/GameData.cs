using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pirate_War_v1
{
    public class GameData
    {
        public string P1_NAME { get; set; }
        public string P2_NAME { get; set; }
        public int P1_HIT { get; set; }
        public int P2_HIT { get; set; }
        public int P1_MISS { get; set; }
        public int P2_MISS { get; set; }
        public int TURN { get; set; }
        public List<TurnElement> Steps { get; set; }
        public string WON { get; set; }

        public static GameData instance;

        public GameData(string p1_NAME, string p2_NAME)
        {
            instance = this;
            P1_HIT = 0;
            P2_HIT = 0;
            TURN = 1;
            Steps = new List<TurnElement>();
            P1_MISS = 0;
            P2_MISS = 0;
            P1_NAME = p1_NAME;
            P2_NAME = p2_NAME;
            WON = "";
        }

        public void saveMove(string player, int turn, Coordinates cord)
        {
            TurnElement turnElement = new TurnElement(player, turn, cord);
            Steps.Add(turnElement);
            GameStepsWindow.instance.listViewer.Items.Add(turnElement.ToString());
        }

        public void refreshStepsWindow(int index)
        {
            GameStepsWindow.instance.listViewer.Items.Clear();
            List<TurnElement> Steps2 = new List<TurnElement>();
            for (int i = 0; i <= index; i++)
            {
               GameStepsWindow.instance.listViewer.Items.Add(Steps[i].ToString());
                Steps2.Add(Steps[i]);
            }
            if (game_select.instance.Mode)
            {
                Torpedo_1v1.instance.selectedTurnIndex = -1;
            }
            Steps = new List<TurnElement>(Steps2);

        }

        public void reset()
        {
            P1_HIT = 0;
            P2_HIT = 0;
            TURN = 1;
            P1_MISS = 0;
            P2_MISS = 0;
        }

        public override string? ToString()
        {
            return "P1_H: " + P1_HIT + "P2_H: " + P2_HIT + "TURN: " + TURN + "P1_M: " + P1_MISS + "P2_M: " + P2_MISS + "P1_NAME: " + P1_NAME + "P2_NAME: " + P2_NAME + "WINNER: " + WON;
        }
    }
}
