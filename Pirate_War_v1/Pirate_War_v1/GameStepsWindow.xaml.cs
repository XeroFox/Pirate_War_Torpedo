using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Pirate_War_v1
{
    /// <summary>
    /// Interaction logic for GameStepsWindow.xaml
    /// </summary>
    public partial class GameStepsWindow : Window
    {
        public static GameStepsWindow instance;
        int MARGINLEFT1 = 172;
        int MARGINLEFT2 = 708;
        public List<TurnElement> turnElements = new List<TurnElement>();
        public GameStepsWindow()
        {
            InitializeComponent();
            instance = this;
            //listViewer.Items.Add("2: Ai - C2");
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            foreach (var step in listViewer.Items)
            {
                //Debug.WriteLine(step.ToString());
            }
            int index = 0;
            foreach (var step in listViewer.Items)
            {
                if (sender.ToString().Contains(step.ToString()))
                {
                    break;
                }
                index++;
            }

            if (game_select.instance.Mode)
            {
                Torpedo_1v1.instance.selectedTurnIndex = index;
                Torpedo_1v1.instance.backToSelectedTurn(Torpedo_1v1.instance.selectedTurnIndex);
                Torpedo_1v1.instance.is_turn_changed = true;
                refreshUi();
                turnElements.Clear();

                for (int i = 0; i < index; i++)
                {
                    if (GameData.instance.Steps[i].Name == "P1")
                    {
                        Torpedo_1v1.instance.makeP1Shots(GameData.instance.Steps[i].coordinates.X, GameData.instance.Steps[i].coordinates.Y);
                    }
                    else
                    {
                        Torpedo_1v1.instance.MakeP2Shots(GameData.instance.Steps[i].coordinates.X, GameData.instance.Steps[i].coordinates.Y);
                    }
                    turnElements.Add(GameData.instance.Steps[i]);
                }


            }
            else
            {
                if (listViewer.Items[index].ToString().Contains("P"))
                {
                    Torpedo_v_ai.instance.selectedTurnIndex = index;
                    Torpedo_v_ai.instance.backToSelectedTurn(Torpedo_v_ai.instance.selectedTurnIndex);

                    Torpedo_v_ai.instance.is_turn_changed = true;
                    refreshUi2();
                    turnElements.Clear();

                    //resetToDefault();
                    //Torpedo_v_ai.instance.newSteps = new List<TurnElement>();
                    Torpedo_v_ai.instance.backToSelectedTurn(Torpedo_v_ai.instance.selectedTurnIndex);
                    //Torpedo_v_ai.instance.is_turn_changed = true;

                    for (int i = 0; i < index; i++)
                    {
                        if (GameData.instance.Steps[i].Name == "P1")
                        {
                            Torpedo_v_ai.instance.MakeP1Shots(GameData.instance.Steps[i].coordinates.X, GameData.instance.Steps[i].coordinates.Y);
                        }
                        else
                        {
                            Torpedo_v_ai.instance.MakeAIShots(GameData.instance.Steps[i].coordinates.X, GameData.instance.Steps[i].coordinates.Y);
                        }

                    }
                }
            }
            Debug.WriteLine(index);
        }

        void refreshUi()
        {
            GameData.instance.reset();
            //GameData.instance.P1_HIT = 0;
            //GameData.instance.P1_MISS = 0;
            Torpedo_1v1.instance.refreshScores();

            Torpedo_1v1.instance.currP1Ships[2] = 1;
            Torpedo_1v1.instance.currP1Ships[1] = 2;
            Torpedo_1v1.instance.currP1Ships[0] = 4;
            //GameData.instance.P2_HIT = 0;
            //GameData.instance.P2_MISS = 0;
            Torpedo_1v1.instance.currP2Ships[2] = 1;
            Torpedo_1v1.instance.currP2Ships[1] = 2;
            Torpedo_1v1.instance.currP2Ships[0] = 4;

            Torpedo_1v1.instance.p2_frig.Text = Torpedo_1v1.instance.currP2Ships[2] + "/" + Torpedo_1v1.instance.maxPlaceableShips[2];
            Torpedo_1v1.instance.p2_brig.Text = Torpedo_1v1.instance.currP2Ships[1] + "/" + Torpedo_1v1.instance.maxPlaceableShips[1];
            Torpedo_1v1.instance.p2_gunboat.Text = Torpedo_1v1.instance.currP2Ships[0] + "/" + Torpedo_1v1.instance.maxPlaceableShips[0];

            Torpedo_1v1.instance.p1_frig.Text = Torpedo_1v1.instance.currP1Ships[2] + "/" + Torpedo_1v1.instance.maxPlaceableShips[2];
            Torpedo_1v1.instance.p1_brig.Text = Torpedo_1v1.instance.currP1Ships[1] + "/" + Torpedo_1v1.instance.maxPlaceableShips[1];
            Torpedo_1v1.instance.p1_gunboat.Text = Torpedo_1v1.instance.currP1Ships[0] + "/" + Torpedo_1v1.instance.maxPlaceableShips[0];
        }

        void refreshUi2()
        {
            GameData.instance.reset();
            //GameData.instance.P1_HIT = 0;
            //GameData.instance.P1_MISS = 0;
            Torpedo_v_ai.instance.refreshScores();

            Torpedo_v_ai.instance.currPlayerShips[2] = 1;
            Torpedo_v_ai.instance.currPlayerShips[1] = 2;
            Torpedo_v_ai.instance.currPlayerShips[0] = 4;
            //GameData.instance.P2_HIT = 0;
            //GameData.instance.P2_MISS = 0;
            Torpedo_v_ai.instance.currAiShips[2] = 1;
            Torpedo_v_ai.instance.currAiShips[1] = 2;
            Torpedo_v_ai.instance.currAiShips[0] = 4;


            Torpedo_v_ai.instance.ai_frig.Text = Torpedo_v_ai.instance.currAiShips[2] + "/" + Torpedo_v_ai.instance.maxPlaceableShips[2];
            Torpedo_v_ai.instance.ai_brig.Text = Torpedo_v_ai.instance.currAiShips[1] + "/" + Torpedo_v_ai.instance.maxPlaceableShips[1];
            Torpedo_v_ai.instance.ai_gunboat.Text = Torpedo_v_ai.instance.currAiShips[0] + "/" + Torpedo_v_ai.instance.maxPlaceableShips[0];

            Torpedo_v_ai.instance.p1_frig.Text = Torpedo_v_ai.instance.currPlayerShips[2] + "/" + Torpedo_v_ai.instance.maxPlaceableShips[2];
            Torpedo_v_ai.instance.p1_brig.Text = Torpedo_v_ai.instance.currPlayerShips[1] + "/" + Torpedo_v_ai.instance.maxPlaceableShips[1];
            Torpedo_v_ai.instance.p1_gunboat.Text = Torpedo_v_ai.instance.currPlayerShips[0] + "/" + Torpedo_v_ai.instance.maxPlaceableShips[0];
        }
    }
}
