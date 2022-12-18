using System;
using System.Collections.Generic;
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
    /// Interaction logic for HowToPlay.xaml
    /// </summary>
    public partial class HowToPlay : Window
    {
        public HowToPlay()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow();
            this.Close();
            mw.Show();
        }

        private void AI_Click(object sender, RoutedEventArgs e)
        {
            string desc = "If you want to play in Player vs AI game mode, in that case, after pressing \r\nthe Play button, select Against AI and enter your name. \r\nAfter pressing the Start button, the game field appears.\r\nPlace your ships. Once you're done with that, the game starts.\r\nThe game will randomly select who takes the opening shot. \r\nYou can only shoot in the opponent's half of the field. \r\nThe game continuously indicates which player is next. \r\nWhoever finds a ship is next again.\r\nIf you want to show or hide the AI's ships (which is not fair),\r\nyou can do it with CTRL+P. \r\nThe game ends when someone sinks all of the opponent's ships. \r\nYou can use the Play Again button to repeat the game, while exiting can be done \r\nwith the Exit button.\r\n";
            text.Content= desc;
        }

        private void pvp_Click(object sender, RoutedEventArgs e)
        {
            string desc = "If you want to play in Player vs Player game mode, in that case, after pressing \r\nthe Play button, select Against human and enter the names.\r\nAfter pressing the Start button, the first player starts placing his ships, \r\nwhile the second player must turn away from the display. \r\nThen the second player places their ships. Once the ships have been placed \r\non the board, the game can begin.\r\nYou shoot at each other's board one after the other, the game continuously \r\nindicates which player is next.\r\nWhoever finds a ship is next again.\r\nIf you want to see where your ships are, you can do it with the eye icon \r\nabove the board, and then you can also hide it. \r\nYou can only do this it’s your turn. \r\nIf the game asks your friends to turn away, a notification will be sent, \r\nand the game will continue only after pressing the OK button.\r\nThe game ends when someone sinks all of the opponent's ships.\r\nYou can use the Play Again button to repeat the game, \r\nwhile exiting can be done with the Exit button.\r\n";
            text.Content = desc;
        }
    }
}
