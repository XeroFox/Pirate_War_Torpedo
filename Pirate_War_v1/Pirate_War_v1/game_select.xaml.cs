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

    public partial class game_select : Window
    {
        string player1 = "";
        string player2 = "";
        string playeraiellen = "";
        string empty = "";
        int szamlaloai = 0;
        int szamlalo1 = 0;
        int szamlalo2 = 0;
        public static game_select instance;
        public game_select()
        {
            InitializeComponent();
            instance = this;
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
            MainWindow.menubgSound.Pause();
            MediaPlayer bgSound = new MediaPlayer();
            Uri uri = new Uri(System.IO.Directory.GetCurrentDirectory() + @"\sounds\game_bg_music.wav", UriKind.Absolute);
            bgSound.Open(uri);

            //bgSound.Play();
            //bgSound.Volume = 1.0f;

            if (Player1Box.Visibility == Visibility.Visible && Player2Box.Visibility == Visibility.Visible)
            {
                char[] one = Player1Box.Text.ToCharArray();
                char[] two = Player2Box.Text.ToCharArray();
                if (Player1Box.Text.Length == 0 || Player2Box.Text.Length == 0)
                {
                    MessageBox.Show("PLayer name can not be empty!");
                    if (Player1Box.Text.Length == 0)
                        szamlalo1++;
                    if (Player2Box.Text.Length == 0)
                        szamlalo2++;
                }
                for (int i = 0; i < Player1Box.Text.Length; i++)
                {
                    if (!Char.IsLetterOrDigit(one[i]))
                    {
                        MessageBox.Show("PLayer name can not contain special character!");
                        szamlalo1++;
                        break;
                    }

                }
                for (int i = 0; i < Player2Box.Text.Length; i++)
                {
                    if (!Char.IsLetterOrDigit(two[i]))
                    {
                        MessageBox.Show("PLayer name can not contain special character!");
                        szamlalo2++;
                        break;
                    }
                }
                if (szamlalo1 == 0 && szamlalo2 == 0)
                {
                    Torpedo_1v1 objTorpedo_1v1 = new Torpedo_1v1();
                    this.Close();
                    objTorpedo_1v1.Show();
                }
            }
            if (PlayerAiEllen.Visibility == Visibility.Visible)
            {
                char[] three = PlayerAiEllen.Text.ToCharArray();
                if (PlayerAiEllen.Text.Length == 0)
                {
                    MessageBox.Show("PLayer name can not be empty!");
                    szamlaloai++;
                }
                for (int i = 0; i < PlayerAiEllen.Text.Length; i++)
                {
                    if (!Char.IsLetterOrDigit(three[i]))
                    {
                        MessageBox.Show("PLayer name can not contain special character!");
                        szamlaloai++;
                        break;
                    }
                }
                if (szamlaloai == 0)
                {
//                    Torpedo_v_ai objTorpedo = new Torpedo_v_ai();
//                    this.Close();
//                    objTorpedo.Show();
                    Torpedo_v_ai TorpAi = new Torpedo_v_ai();
                    TorpAi.Show();
                    this.Close();

                }
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
            szamlalo1 = 0;
        }

        private void PlayerAiEllen_TextChanged(object sender, TextChangedEventArgs e)
        {
            playeraiellen = PlayerAiEllen.Text;
            szamlaloai = 0;
        }

        private void Player2_TextChanged(object sender, TextChangedEventArgs e)
        {
            player2 = Player2Box.Text;
            szamlalo2 = 0;

        }
        public static MediaPlayer mediaPlayer = new MediaPlayer();

        public static void playMusic()
        {
            MainWindow.menubgSound.Stop();
            Uri uri = new Uri(System.IO.Directory.GetCurrentDirectory() + @"\sounds\game_bg_music.wav", UriKind.Absolute);
            mediaPlayer.Open(uri);
            //mediaPlayer.SetValue = 1000;
            mediaPlayer.Play();
        }
        //MediaPlayer bgSound = new MediaPlayer();
        //Uri uri = new Uri(System.IO.Directory.GetCurrentDirectory() + @"\sounds\game_bg_music.wav", UriKind.Absolute);
        //bgSound.Open(uri);

        //bgSound.Play();
        //bgSound.Volume = 0.1f;
    }
}
