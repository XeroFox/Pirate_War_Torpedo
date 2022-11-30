using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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
    /// Interaction logic for EndOfMatch.xaml
    /// </summary>
    public partial class EndOfMatch : Window
    {
        public String WINNER;
        public static int P1Wins;
        public static int P2Wins;
        public EndOfMatch()
        {
            InitializeComponent();
            Bg.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_underwater_bg.png", UriKind.Absolute))
            };
            Ship.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_ship_frigate_damaged_rotated.png", UriKind.Absolute))
            };
        }

        private void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            if (Label2.Content.Equals("Second player name"))
            {
                Torpedo_1v1 Playagainwindow = new Torpedo_1v1();
                this.Close();
                Playagainwindow.Show();
            }else if(Label2.Content.Equals("Ai name"))
            {
                Torpedo_v_ai Playagainwindow = new Torpedo_v_ai();
                this.Close();
                Playagainwindow.Show();
            }
            
            if (WINNER == "1")
            {
                P1Wins = P1Wins + 1;
            }
            else
            {
                P2Wins = P2Wins + 1;
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            this.Close();
            main.Show();
            
        }

    }
}
