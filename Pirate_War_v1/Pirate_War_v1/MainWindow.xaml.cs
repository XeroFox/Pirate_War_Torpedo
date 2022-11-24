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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pirate_War_v1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Bg.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_underwater_bg.png", UriKind.Absolute))
            };
        }
        private void PlayClicked(object sender, RoutedEventArgs e)
        {
            game_select objgame_select = new game_select();
            this.Close();
            objgame_select.Show();
        }
        private void ScoreboardClicked(object sender, RoutedEventArgs e)
        {

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

    }
}
