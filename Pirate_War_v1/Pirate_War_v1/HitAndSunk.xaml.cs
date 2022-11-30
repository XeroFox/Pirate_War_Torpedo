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
    /// Interaction logic for HitAndSunk.xaml
    /// </summary>
    public partial class HitAndSunk : Window
    {
        public HitAndSunk()
        {
            InitializeComponent();
            Bg.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_underwater_bg.png", UriKind.Absolute))
            };
        }
        private void OkButtonPressed(object sender, RoutedEventArgs e)
        {
            this.Close();
            P_name.Content = "";
            InfoText.Content = "Hit and Sunk";

        }
    }
}
