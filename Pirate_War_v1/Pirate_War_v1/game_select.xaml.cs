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
                char[] one = Player1Box.Text.ToCharArray();
                char[] two = Player2Box.Text.ToCharArray();
                if (Player1Box.Text.Length == 0 || Player2Box.Text.Length == 0)
                {
                    MessageBox.Show("PLayer name can not be empty!");
                }
                else if (true)
                {
                    for (int i = 0; i < Player1Box.Text.Length; i++)
                    {
                        if (!Char.IsLetterOrDigit(one[i]))
                        {
                            MessageBox.Show("PLayer name can not contain special character!");
                            break;
                        }

                    }
                    for (int i = 0; i < Player2Box.Text.Length; i++)
                    {
                        if (!Char.IsLetterOrDigit(two[i]))
                        {
                            MessageBox.Show("PLayer name can not contain special character!");
                            break;
                        }
                        if (Player1Box.Text.Length - 1 == i)
                            break;
                    }
                }//else gamebe lép
            }
            if (PlayerAiEllen.Visibility == Visibility.Visible)
            {
                char[] three = PlayerAiEllen.Text.ToCharArray();
                if (PlayerAiEllen.Text.Length == 0)
                {
                    MessageBox.Show("PLayer name can not be empty!");
                }
                else if (true)
                {
                    for (int i = 0; i < PlayerAiEllen.Text.Length; i++)
                    {
                        if (!Char.IsLetterOrDigit(three[i]))
                        {
                            MessageBox.Show("PLayer name can not contain special character!");
                            break;
                        }
                        if (Player1Box.Text.Length - 1 == i)
                            break;
                    }
                }//else gamebe lép
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
    