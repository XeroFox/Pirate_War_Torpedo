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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Pirate_War_v1.Torpedo_1v1;

namespace Pirate_War_v1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MediaPlayer menubgSound = new MediaPlayer();
        public bool muted = false;
        public static MainWindow instance;
        public MainWindow()
        {
            instance = this;
            InitializeComponent();
            Bg.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_underwater_bg.png", UriKind.Absolute))
            };
            if (muted)
            {
                Sound_button.Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\hang_off2.png", UriKind.Absolute))
                };
            }
            else
            {
                Sound_button.Fill = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\hang_on2.png", UriKind.Absolute))
                };
            }
            PlayBG();

        }
        private void PlayClicked(object sender, RoutedEventArgs e)
        {
            //PlayBG();
            game_select objgame_select = new game_select();
            this.Close();
            objgame_select.Show();
        }
        private void ScoreboardClicked(object sender, RoutedEventArgs e)
        {
            game_select objgame_select = new game_select();
            this.Close();
            objgame_select.Show();
        }
        private void HowToPlayClicked(object sender, RoutedEventArgs e)
        {

        }
        private void CreditsClicked(object sender, RoutedEventArgs e)
        {
            Credits objCredits = new Credits();
            objCredits.Show();

        }
        private void ExitClicked(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(0);
        }
        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(canvas);
            Debug.WriteLine(p.X + ":" + p.Y);

            if (p.X > 192 && p.X < 269 && p.Y > 308 && p.Y < 400)
            {
                if (muted) {
                    menubgSound.Volume= 0.8f;
                    Sound_button.Fill = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\hang_on2.png", UriKind.Absolute))
                    };
                    muted = !muted;
                }
                else {
                    menubgSound.Volume = 0.0f;
                    Sound_button.Fill = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\hang_off2.png", UriKind.Absolute))
                    };
                    muted = !muted;
                }
            }


        }


        private void PlayBG()
        {
            Uri uri = new Uri(Directory.GetCurrentDirectory() + @"\sounds\menu_bg_music.wav", UriKind.Absolute);
            menubgSound.Open(uri);
            menubgSound.MediaEnded += new EventHandler(Media_Ended);
            menubgSound.Play();
        }

        private static void Media_Ended(object? sender, EventArgs e)
        {
            Uri uri = new Uri(Directory.GetCurrentDirectory() + @"\sounds\menu_bg_music.wav", UriKind.Absolute);
            menubgSound.Open(uri);
            menubgSound.Play();
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
