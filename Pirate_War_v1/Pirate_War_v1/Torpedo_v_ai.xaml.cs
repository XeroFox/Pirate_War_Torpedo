using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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
using Path = System.IO.Path;

namespace Pirate_War_v1
{
    /// <summary>
    /// Interaction logic for Torpedo_v_ai.xaml
    /// </summary>
    public partial class Torpedo_v_ai : Window
    {

        int[,] player_zone = new int[10,10];
        int[,] ai_zone = new int[10, 10];

        Rectangle selectedRectangle = new Rectangle
        {
            Width = 50,
            Height = 50,
            Opacity = 0.3,
            Fill = new SolidColorBrush(Color.FromRgb(0,0,255))
        };

        public Torpedo_v_ai()
        {
            InitializeComponent();
            for(int i = 0; i < 10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    if(i == 0 || i == 9 || j == 0 || j == 9)
                    {
                        player_zone[i, j] = -1;
                        ai_zone[i, j] = -1;
                    }
                    else
                    {
                        player_zone[i, j] = 0;
                        ai_zone[i, j] = 0;
                    }
                }
            }
            canvas.Children.Add(selectedRectangle);
            customPointer.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_hand.png", UriKind.Absolute))
            };
            String[] ai_name_first = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\ai_names\\first.txt");
            String[] ai_name_mid = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\ai_names\\mid.txt");
            String[] ai_name_last = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\ai_names\\last.txt");
            Random random = new Random();
            ai_name.Text = ai_name_first[random.Next(0,ai_name_first.Length)] + " " + ai_name_mid[random.Next(0, ai_name_mid.Length)] + " " + ai_name_last[random.Next(0, ai_name_last.Length)];

        }

        void OnMouseMoveHandler(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(canvas);
            double pX = p.X-15;
            double pY = p.Y-10;
            Canvas.SetTop(customPointer, pY);
            Canvas.SetLeft(customPointer, pX);
            Cursor = Cursors.None;
            int[] matrix1 = { Convert.ToInt32(Math.Floor((pX - 158) / 50)), Convert.ToInt32(Math.Floor((pY - 154) / 50)) };
            matrix1[0] = (matrix1[0] < 0 ? -1 : matrix1[0] > 8 ? 8 : matrix1[0]);
            matrix1[1] = (matrix1[1] < 0 ? -1 : matrix1[1] > 8 ? 8 : matrix1[1]);
            int[] matrix2 = { Convert.ToInt32(Math.Floor((pX - 694) / 50)), Convert.ToInt32(Math.Floor((pY - 154) / 50)) };
            matrix2[0] = (matrix2[0] < 0 ? -1 : matrix2[0] > 8 ? 8 : matrix2[0]);
            matrix2[1] = (matrix2[1] < 0 ? -1 : matrix2[1] > 8 ? 8 : matrix2[1]);
            p1_name.Text = matrix1[0] + " : " + matrix1[1];
            ai_name.Text = matrix2[0] + " : " + matrix2[1];

            if (player_zone[matrix1[0]+1, matrix1[1]+1] == 0)
            {
                selectedRectangle.Visibility = Visibility.Visible;
                Canvas.SetLeft(selectedRectangle, 172 + matrix1[0] * 50);
                Canvas.SetTop(selectedRectangle, 160 + matrix1[1] * 50);

            }
            else if(ai_zone[matrix2[0]+1, matrix2[1]+1] == 0)
            {
                selectedRectangle.Visibility = Visibility.Visible;
                Canvas.SetLeft(selectedRectangle, 708 + matrix2[0] * 50);
                Canvas.SetTop(selectedRectangle, 160 + matrix2[1] * 50);
            }
            else
            {
                selectedRectangle.Visibility = Visibility.Hidden;
            }
        }
    }
}
