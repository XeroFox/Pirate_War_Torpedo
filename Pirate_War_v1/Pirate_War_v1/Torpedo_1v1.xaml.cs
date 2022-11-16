using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// Interaction logic for Torpedo_1v1.xaml
    /// </summary>
    public partial class Torpedo_1v1 : Window
    {
        Brush customColor;
        Random r = new Random();
        String parent = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName).FullName;
        public Torpedo_1v1()
        {
            InitializeComponent();
            
        }
        private void myCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Rectangle)
            {
                Rectangle activeRectangle = (Rectangle)e.OriginalSource;

                PlayCanvas.Children.Remove(activeRectangle);
            }
            else
            {
                customColor = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 255)));

                Rectangle newRectangle = new Rectangle
                {
                    Width = 100,
                    Height = 50,
                    Fill = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(@"C:\Users\Kiss Szabolcs\Documents\GitHub\Pirate_War_Torpedo\Pirate_War_v1\Pirate_War_v1\sources\spr_ship_brig.png", UriKind.Absolute))
                        //ImageSource = new BitmapImage(new Uri(parent+@"\sources\spr_ship_brig.png", UriKind.Absolute))
                    },
                };

                Canvas.SetLeft(newRectangle, Mouse.GetPosition(PlayCanvas).X);
                Canvas.SetTop(newRectangle, Mouse.GetPosition(PlayCanvas).Y);

                PlayCanvas.Children.Add(newRectangle);
            }
        }

    }
}

