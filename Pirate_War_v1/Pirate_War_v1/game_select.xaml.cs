using System;
using System.Collections.Generic;
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
    /// Interaction logic for game_select.xaml
    /// </summary>
    public partial class game_select : Window
    {
        String player1 = "";
        String player2 = "";
        String playeraiellen = "";
        String empty = "";
        Char[] specialchar = {'@', '#', '$', '%', '!', '~' };
        public game_select()
        {
            InitializeComponent();
            Bg.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_underwater_bg.png", UriKind.Absolute))
            };

            Label1.Visibility = Visibility.Hidden;
            Label2.Visibility = Visibility.Hidden;
            LabelAiEllen.Visibility = Visibility.Hidden;
            Player1Box.Visibility = Visibility.Hidden;
            Player2Box.Visibility = Visibility.Hidden;
            PlayerAiEllen.Visibility = Visibility.Hidden;
        }

        private void Vissza_Click(object sender, RoutedEventArgs e)
        {
            MainWindow objMainWindow = new MainWindow();
            this.Close();
            objMainWindow.Show();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
           if (Player1Box.Visibility == Visibility.Visible && Player2Box.Visibility == Visibility.Visible)
            {
                if (Player1Box.Text.Length == 0 || Player2Box.Text.Length == 0)
                {
                    MessageBox.Show("PLayer name can not be empty!");
                }else for(int i = 0; i < specialchar.Length; i++)
                    {
                        if(Player1Box.Text.Contains(specialchar[i]) || Player2Box.Text.Contains(specialchar[i]))
                        {
                            MessageBox.Show("PLayer name can not contain special character!");
                            break;
                        }
                    } //megcsinalni az elset hogy megnyitsa a game-t
            }
            if (PlayerAiEllen.Visibility == Visibility.Visible)
            {
                if (PlayerAiEllen.Text.Length == 0)
                {
                    MessageBox.Show("PLayer name can not be empty!");
                }
                else for (int i = 0; i < specialchar.Length; i++)
                    {
                        if (PlayerAiEllen.Text.Contains(specialchar[i]))
                        {
                            MessageBox.Show("PLayer name can not contain special character!");
                            break;
                        }
                    } //megcsinalni az elset hogy megnyitsa a game-t
            }

        }

        private void Ember_ellen_Click(object sender, RoutedEventArgs e)
        {
            Label1.Visibility = Visibility.Visible;
            Label2.Visibility = Visibility.Visible;
            Player1Box.Visibility = Visibility.Visible;
            Player2Box.Visibility = Visibility.Visible;
            PlayerAiEllen.Text = empty;
            playeraiellen = empty;

            if (PlayerAiEllen.IsVisible && LabelAiEllen.IsVisible) 
            {
                LabelAiEllen.Visibility = Visibility.Hidden;
                PlayerAiEllen.Visibility = Visibility.Hidden;
                
            }
        }

        private void AI_ellen_Click(object sender, RoutedEventArgs e)
        {
            PlayerAiEllen.Visibility = Visibility.Visible;
            LabelAiEllen.Visibility = Visibility.Visible;
            Player1Box.Text = empty;
            Player2Box.Text = empty;
            player1 = empty;
            player2 = empty;
            if (Label1.IsVisible && Player1Box.IsVisible)
            {
                Label1.Visibility = Visibility.Hidden;
                Label2.Visibility = Visibility.Hidden;
                Player1Box.Visibility = Visibility.Hidden;
                Player2Box.Visibility = Visibility.Hidden;
            }
        }

        private void Player1_TextChanged(object sender, TextChangedEventArgs e)
        {
            player1 = Player1Box.Text;
        }

        private void PlayerAiEllen_TextChanged(object sender, TextChangedEventArgs e)
        {
            playeraiellen = PlayerAiEllen.Text;
        }

        private void Player2_TextChanged(object sender, TextChangedEventArgs e)
        {
            player2 = Player2Box.Text;
        }
    }
}
