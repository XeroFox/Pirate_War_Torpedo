using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
    /// Interaction logic for Scoreboard.xaml
    /// </summary>
    public partial class Scoreboard : Window
    {
        public Scoreboard()
        {
            
            InitializeComponent();
            Bg.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_underwater_bg.png", UriKind.Absolute))
            };
            ScoreData scoreData = new ScoreData();
            scoreData.loadJsonFile();
            //foreach(var sD in scoreData.gameDatas)
            //{
            //    listViewer.Items.Add(toList(sD));
            //}

            for(int i = scoreData.gameDatas.Count-1; i>=0; i--)
            {
                listViewer.Items.Add(toList(scoreData.gameDatas[i]));
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            this.Close();
            mw.Show();
        }

        private string toList(FinalData sd)
        {
            int multiply = 10;
            string name="";
            string name1= "";
            string name3 = "";
            if (sd.P1_NAME.Length < multiply)
            {
                int multi = multiply - sd.P1_NAME.Length;
                 name1= sd.P1_NAME+new string(' ', multi)+"\t\t";
            }
            else
            {
                name1 = sd.P1_NAME.Substring(0, 10)+"\t";
            }
            if (sd.P2_NAME.Length < multiply)
            {
                int multi = multiply - sd.P2_NAME.Length;
                name +=sd.P2_NAME+ new string(' ', multi)+"\t\t";
            }
            else
            {
                 name = sd.P2_NAME.Substring(0, 10)+"\t";
            }

            if (sd.WON.Length < 17)
            {
                int multi = multiply - sd.WON.Length;
                name3 += sd.WON + new string(' ', multi);
            }
            else
            {
                name3 = sd.WON.Substring(0, 17) + "\t";
            }

            return "" + name1 + "" + sd.P1_HIT + "\\" + sd.P1_MISS + "\t\t" + name + "" + sd.P2_HIT + "\\" + sd.P2_MISS + "\t\t " + sd.TURN + "\t " + name3;
        }
    }
}
