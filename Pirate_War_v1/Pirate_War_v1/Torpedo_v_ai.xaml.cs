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
using System.Windows.Media.Media3D;
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

        List<Rectangle> placingShipRect = new List<Rectangle>();

        List<ImageBrush> cursorSprites = new List<ImageBrush>();

        List<ImageBrush> rotateSprites = new List<ImageBrush>();

        List<ImageBrush> shipSprites = new List<ImageBrush>();

        int selectionMode = 0;
        int selectedShip = 0;
        int rotationMode = 0;

        bool placeable = false;

        int[] selectedGridPlayerIndex = {0,0};
        int[] selectedGridAiIndex = {0,0};

        List<Rectangle> playerShips = new List<Rectangle>();

        public Torpedo_v_ai()
        {
            InitializeComponent();
            player_zone = Torpedo_v_ai_matrix_movement.clearMatrixElements();
            ai_zone = Torpedo_v_ai_matrix_movement.clearMatrixElements();

            for (int i = 0; i < 4; i++)
            {
                placingShipRect.Add(new Rectangle
                {
                    Width = 50,
                    Height = 50,
                    Opacity = 0.3,
                    Visibility = Visibility.Hidden,
                    Fill = Brushes.Green
                });
                canvas.Children.Add(placingShipRect[i]);
            }
            canvas.Children.Add(selectedRectangle);

            loadSpriteDatas();

            setCustomPointerSprite(0);

            String[] ai_name_first = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\ai_names\\first.txt");
            String[] ai_name_mid = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\ai_names\\mid.txt");
            String[] ai_name_last = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\ai_names\\last.txt");
            Random random = new Random();
            ai_name.Text = ai_name_first[random.Next(0,ai_name_first.Length)] + " " + ai_name_mid[random.Next(0, ai_name_mid.Length)] + " " + ai_name_last[random.Next(0, ai_name_last.Length)];

        }

        void setCustomPointerSprite(int subimage)
        {
            if(subimage == 0)
            {
                customPointer.Fill = cursorSprites[0];
            }else if(subimage == 1)
            {
                customPointer.Fill = cursorSprites[1];
            }
            
        }


        void locatePressableItems(double pX, double pY)
        {
            if(pX > 60 && pX < 145)
            {
                if((pY > 60 && pY < 240) || (pY > 280 && pY < 415) || (pY > 480 && pY < 572))
                {
                    setCustomPointerSprite(1);
                }
            }
            if(selectionMode == 1 && pX > 615 && pX < 670 && pY > 330 && pY < 380)
            {
                setCustomPointerSprite(1);
                rotationButton.Fill = rotateSprites[1];
            }
            else
            {
                rotationButton.Fill = rotateSprites[0];
            }
        }

        void OnMouseMoveHandler(object sender, MouseEventArgs e)
        {
            setCustomPointerSprite(0);
            Point p = e.GetPosition(canvas);
            double pX = p.X-15;
            double pY = p.Y-10;
            Canvas.SetTop(customPointer, pY);
            Canvas.SetLeft(customPointer, pX);

            curr_turn.Text = p.X + " : " + p.Y;

            Cursor = Cursors.None;
            int[] matrix1 = { Convert.ToInt32(Math.Floor((p.X - 172) / 50)), Convert.ToInt32(Math.Floor((p.Y - 160) / 50)) };
            matrix1[0] = (matrix1[0] < 0 ? -1 : matrix1[0] > 8 ? 8 : matrix1[0]);
            matrix1[1] = (matrix1[1] < 0 ? -1 : matrix1[1] > 8 ? 8 : matrix1[1]);
            int[] matrix2 = { Convert.ToInt32(Math.Floor((p.X - 708) / 50)), Convert.ToInt32(Math.Floor((p.Y - 160) / 50)) };
            matrix2[0] = (matrix2[0] < 0 ? -1 : matrix2[0] > 8 ? 8 : matrix2[0]);
            matrix2[1] = (matrix2[1] < 0 ? -1 : matrix2[1] > 8 ? 8 : matrix2[1]);
            selectedGridPlayerIndex[0] = matrix1[0]+1;
            selectedGridPlayerIndex[1] = matrix1[1]+1;
            selectedGridAiIndex[0] = matrix2[0]+1;
            selectedGridAiIndex[1] = matrix2[1]+1;
            p1_name.Text = selectedGridPlayerIndex[0] + " : " + selectedGridPlayerIndex[1];
            ai_name.Text = selectedGridAiIndex[0] + " : " + selectedGridAiIndex[1];

            locatePressableItems(p.X, p.Y);

            drawSelectedMatrixIndex(matrix1, matrix2);


        }

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(canvas);
            if (p.X > 60 && p.X < 145)
            {
                if (p.Y > 60 && p.Y < 240 )
                {
                    var bc = new BrushConverter();
                    p1_gunboat.Foreground = bc.ConvertFrom("#3a76a9") as Brush;
                    p1_brig.Foreground = bc.ConvertFrom("#3a76a9") as Brush;
                    p1_frig.Foreground = Brushes.Green;
                    selectedShip = 4;
                    selectionMode = 1;
                }
                else if (p.Y > 280 && p.Y < 415)
                {
                    var bc = new BrushConverter();
                    p1_gunboat.Foreground = bc.ConvertFrom("#3a76a9") as Brush;
                    p1_frig.Foreground = bc.ConvertFrom("#3a76a9") as Brush;
                    p1_brig.Foreground = Brushes.Green;
                    selectedShip = 3;
                    selectionMode = 1;
                }
                else if (p.Y > 480 && p.Y < 572)
                {
                    var bc = new BrushConverter();
                    p1_brig.Foreground = bc.ConvertFrom("#3a76a9") as Brush;
                    p1_frig.Foreground = bc.ConvertFrom("#3a76a9") as Brush;
                    p1_gunboat.Foreground = Brushes.Green;
                    selectedShip = 2;
                    selectionMode = 1;
                }
                int[] tmp = { -1, -1 };
                drawSelectedMatrixIndex(tmp, tmp);
            }
            if(selectionMode == 1)
            {
                if (selectionMode == 1 && p.X > 615 && p.X < 670 && p.Y > 330 && p.Y < 380)
                {
                    rotationMode = (rotationMode == 0) ? 1 : 0;
                }
                rotationShipView.Fill = (rotationMode == 0 ? rotateSprites[2] : rotateSprites[3]);
            }
            if (placeable)
            {
                placeShip(rotationMode, selectedShip, selectedGridPlayerIndex[0] - 1, selectedGridPlayerIndex[1] - 1);
            }
        }

        void placeShip(int rotation, int shipType, int XX, int YY)
        {
            Rectangle tmpRect = new Rectangle
            {
                Width = (shipType == 2 ? 100 : shipType == 3 ? 150 : 215),
                Height = (shipType == 2 ? 30 : shipType == 3 ? 75 : 100),
                Opacity = 1,
                RenderTransform = (rotation == 1 ? new RotateTransform(-90) : new RotateTransform(0)),
                Visibility = Visibility.Visible,
                Fill = (shipType == 2 ? shipSprites[0] : shipType == 3 ? shipSprites[2] : shipSprites[4])
            };
            playerShips.Add(tmpRect);
            canvas.Children.Add(playerShips[playerShips.Count()-1]);
            Canvas.SetTop(playerShips[playerShips.Count() - 1], 160+YY*50 - (rotation == 0 ? (shipType == 2 ? -10 : shipType == 3 ? 13 : 25) : -playerShips[playerShips.Count() - 1].Width));
            Canvas.SetLeft(playerShips[playerShips.Count() - 1], 172+XX*50 - (rotation == 0 ? (shipType == 4 ? 15 : 0) : (shipType == 2 ? -10 : shipType == 3 ? 13 : 25)));
            player_zone = Torpedo_v_ai_matrix_movement.updateMatrix(selectedGridPlayerIndex[0], selectedGridPlayerIndex[1], shipType, player_zone, rotation);
            placeable = false;
            int[] tmp = { -1, -1 };
            drawSelectedMatrixIndex(tmp, tmp);
        }

        private void canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectionMode != 0)
            {
                selectionMode = 0;
                selectedShip = 0;
                var bc = new BrushConverter();
                p1_gunboat.Foreground = bc.ConvertFrom("#3a76a9") as Brush;
                p1_brig.Foreground = bc.ConvertFrom("#3a76a9") as Brush;
                p1_frig.Foreground = bc.ConvertFrom("#3a76a9") as Brush;
                int[] tmp = { -1, -1 };
                drawSelectedMatrixIndex(tmp, tmp);
            }
        }



        void loadSpriteDatas()
        {
            //Rotation Sprites
            rotateSprites.Add(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_rotate_btn.png", UriKind.Absolute))
            });
            rotateSprites.Add(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_rotate_btn_hover.png", UriKind.Absolute))
            });
            rotateSprites.Add(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_ship_rotate_0.png", UriKind.Absolute))
            });
            rotateSprites.Add(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_ship_rotate_1.png", UriKind.Absolute))
            });


            //Cursor Sprites
            cursorSprites.Add(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_hand.png", UriKind.Absolute))
            });
            cursorSprites.Add(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_stroked_hand.png", UriKind.Absolute))
            });

            //Ship Sprites
            shipSprites.Add(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_ship_gunboat.png", UriKind.Absolute))
            });
            shipSprites.Add(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_ship_gunboat_damaged.png", UriKind.Absolute))
            });
            shipSprites.Add(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_ship_brig.png", UriKind.Absolute))
            });
            shipSprites.Add(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_ship_brig_destroyed.png", UriKind.Absolute))
            });
            shipSprites.Add(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_ship_frigate.png", UriKind.Absolute))
            });
            shipSprites.Add(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_ship_frigate_damaged.png", UriKind.Absolute))
            });
        }

        void drawSelectedMatrixIndex(int[] matrix1, int[] matrix2)
        {
            if (selectionMode == 0)
            {
                rotationButton.Visibility = Visibility.Hidden;
                rotationShipView.Visibility = Visibility.Hidden;
                foreach (Rectangle x in placingShipRect)
                {
                    x.Visibility = Visibility.Hidden;
                }
                if (player_zone[matrix1[0] + 1, matrix1[1] + 1] == 0)
                {
                    selectedRectangle.Visibility = Visibility.Visible;
                    Canvas.SetLeft(selectedRectangle, 172 + matrix1[0] * 50);
                    Canvas.SetTop(selectedRectangle, 160 + matrix1[1] * 50);
                    setCustomPointerSprite(1);

                }
                else if (ai_zone[matrix2[0] + 1, matrix2[1] + 1] == 0)
                {
                    selectedRectangle.Visibility = Visibility.Visible;
                    Canvas.SetLeft(selectedRectangle, 708 + matrix2[0] * 50);
                    Canvas.SetTop(selectedRectangle, 160 + matrix2[1] * 50);
                    setCustomPointerSprite(1);
                }
                else
                {
                    selectedRectangle.Visibility = Visibility.Hidden;
                }
            }
            else
            {

                placeable = false;
                selectedRectangle.Visibility = Visibility.Hidden;
                rotationButton.Visibility = Visibility.Visible;
                rotationShipView.Visibility = Visibility.Visible;

                if (player_zone[matrix1[0] + 1, matrix1[1] + 1] == 0)
                {
                    placeable = true;
                    placingShipRect[0].Visibility = Visibility.Visible;
                    Canvas.SetLeft(placingShipRect[0], 172 + matrix1[0] * 50);
                    Canvas.SetTop(placingShipRect[0], 160 + matrix1[1] * 50);
                    if (selectedShip == 2)
                    {
                        if (rotationMode == 0)
                        {
                            if (matrix1[0] < 7)
                            {
                                placingShipRect[1].Visibility = Visibility.Visible;
                                Canvas.SetLeft(placingShipRect[1], 172 + (matrix1[0] + 1) * 50);
                                Canvas.SetTop(placingShipRect[1], 160 + matrix1[1] * 50);
                                placeable = Torpedo_v_ai_matrix_movement.is_ship_placeable(selectedGridPlayerIndex[0], selectedGridPlayerIndex[1],2,player_zone,0);
                            }
                            else
                            {
                                placeable = false;
                                placingShipRect[0].Fill = Brushes.Red;
                                placingShipRect[1].Visibility = Visibility.Hidden;
                            }
                        }
                        else
                        {
                            if (matrix1[1] < 7)
                            {
                                placingShipRect[1].Visibility = Visibility.Visible;
                                Canvas.SetLeft(placingShipRect[1], 172 + matrix1[0] * 50);
                                Canvas.SetTop(placingShipRect[1], 160 + (matrix1[1] + 1) * 50);
                                placeable = Torpedo_v_ai_matrix_movement.is_ship_placeable(selectedGridPlayerIndex[0], selectedGridPlayerIndex[1], 2, player_zone, 1);
                            }
                            else
                            {
                                placeable = false;
                                placingShipRect[0].Fill = Brushes.Red;
                                placingShipRect[1].Visibility = Visibility.Hidden;
                            }
                        }

                    }
                    else if (selectedShip == 3)
                    {
                        if (rotationMode == 0)
                        {
                            if (matrix1[0] < 6)
                            {
                                placingShipRect[1].Visibility = Visibility.Visible;
                                placingShipRect[2].Visibility = Visibility.Visible;
                                Canvas.SetLeft(placingShipRect[1], 172 + (matrix1[0] + 1) * 50);
                                Canvas.SetTop(placingShipRect[1], 160 + matrix1[1] * 50);
                                Canvas.SetLeft(placingShipRect[2], 172 + (matrix1[0] + 2) * 50);
                                Canvas.SetTop(placingShipRect[2], 160 + matrix1[1] * 50);
                                placeable = Torpedo_v_ai_matrix_movement.is_ship_placeable(selectedGridPlayerIndex[0], selectedGridPlayerIndex[1], 3, player_zone, 0);
                            }
                            else if(matrix1[0] < 7)
                            {
                                placeable = false;
                                placingShipRect[1].Visibility = Visibility.Visible;
                                placingShipRect[2].Visibility = Visibility.Hidden;
                                Canvas.SetLeft(placingShipRect[1], 172 + (matrix1[0]+1) * 50);
                                Canvas.SetTop(placingShipRect[1], 160 + matrix1[1] * 50);
                            }
                            else
                            {
                                placeable = false;
                                placingShipRect[0].Fill = Brushes.Red;
                                placingShipRect[1].Visibility = Visibility.Hidden;
                                placingShipRect[2].Visibility = Visibility.Hidden;
                            }
                        }
                        else
                        {
                            if (matrix1[1] < 6)
                            {
                                placingShipRect[1].Visibility = Visibility.Visible;
                                placingShipRect[2].Visibility = Visibility.Visible;
                                Canvas.SetLeft(placingShipRect[1], 172 + matrix1[0] * 50);
                                Canvas.SetTop(placingShipRect[1], 160 + (matrix1[1] + 1) * 50);
                                Canvas.SetLeft(placingShipRect[2], 172 + matrix1[0] * 50);
                                Canvas.SetTop(placingShipRect[2], 160 + (matrix1[1] + 2) * 50);
                                placeable = Torpedo_v_ai_matrix_movement.is_ship_placeable(selectedGridPlayerIndex[0], selectedGridPlayerIndex[1], 3, player_zone, 1);
                            }
                            else if (matrix1[1] < 7)
                            {
                                placeable = false;
                                placingShipRect[1].Visibility = Visibility.Visible;
                                placingShipRect[2].Visibility = Visibility.Hidden;
                                Canvas.SetLeft(placingShipRect[1], 172 + matrix1[0] * 50);
                                Canvas.SetTop(placingShipRect[1], 160 + (matrix1[1]+1) * 50);
                            }
                            else
                            {
                                placeable = false;
                                placingShipRect[0].Fill = Brushes.Red;
                                placingShipRect[1].Visibility = Visibility.Hidden;
                                placingShipRect[2].Visibility = Visibility.Hidden;
                            }
                        }

                    }
                    else if (selectedShip == 4)
                    {
                        if (rotationMode == 0)
                        {
                            if (matrix1[0] < 5)
                            {
                                placingShipRect[1].Visibility = Visibility.Visible;
                                placingShipRect[2].Visibility = Visibility.Visible;
                                placingShipRect[3].Visibility = Visibility.Visible;
                                Canvas.SetLeft(placingShipRect[1], 172 + (matrix1[0] + 1) * 50);
                                Canvas.SetTop(placingShipRect[1], 160 + matrix1[1] * 50);
                                Canvas.SetLeft(placingShipRect[2], 172 + (matrix1[0] + 2) * 50);
                                Canvas.SetTop(placingShipRect[2], 160 + matrix1[1] * 50);
                                Canvas.SetLeft(placingShipRect[3], 172 + (matrix1[0] + 3) * 50);
                                Canvas.SetTop(placingShipRect[3], 160 + matrix1[1] * 50);
                                placeable = Torpedo_v_ai_matrix_movement.is_ship_placeable(selectedGridPlayerIndex[0], selectedGridPlayerIndex[1], 4, player_zone, 0);
                            }
                            else if (matrix1[0] < 6)
                            {
                                placeable = false;
                                placingShipRect[1].Visibility = Visibility.Visible;
                                placingShipRect[2].Visibility = Visibility.Visible;
                                placingShipRect[3].Visibility = Visibility.Hidden;
                                Canvas.SetLeft(placingShipRect[1], 172 + (matrix1[0] + 1) * 50);
                                Canvas.SetTop(placingShipRect[1], 160 + matrix1[1] * 50);
                                Canvas.SetLeft(placingShipRect[2], 172 + (matrix1[0] + 2) * 50);
                                Canvas.SetTop(placingShipRect[2], 160 + matrix1[1] * 50);
                            }
                            else if (matrix1[0] < 7)
                            {
                                placeable = false;
                                placingShipRect[1].Visibility = Visibility.Visible;
                                placingShipRect[2].Visibility = Visibility.Hidden;
                                placingShipRect[3].Visibility = Visibility.Hidden;
                                Canvas.SetLeft(placingShipRect[1], 172 + (matrix1[0] + 1) * 50);
                                Canvas.SetTop(placingShipRect[1], 160 + matrix1[1] * 50);
                            }
                            else
                            {
                                placeable = false;
                                placingShipRect[1].Visibility = Visibility.Hidden;
                                placingShipRect[2].Visibility = Visibility.Hidden;
                                placingShipRect[3].Visibility = Visibility.Hidden;
                            }
                        }
                        else
                        {
                            if (matrix1[1] < 5)
                            {
                                placingShipRect[1].Visibility = Visibility.Visible;
                                placingShipRect[2].Visibility = Visibility.Visible;
                                placingShipRect[3].Visibility = Visibility.Visible;
                                Canvas.SetLeft(placingShipRect[1], 172 + matrix1[0] * 50);
                                Canvas.SetTop(placingShipRect[1], 160 + (matrix1[1] + 1) * 50);
                                Canvas.SetLeft(placingShipRect[2], 172 + matrix1[0] * 50);
                                Canvas.SetTop(placingShipRect[2], 160 + (matrix1[1] + 2) * 50);
                                Canvas.SetLeft(placingShipRect[3], 172 + matrix1[0] * 50);
                                Canvas.SetTop(placingShipRect[3], 160 + (matrix1[1] + 3) * 50);
                                placeable = Torpedo_v_ai_matrix_movement.is_ship_placeable(selectedGridPlayerIndex[0], selectedGridPlayerIndex[1], 4, player_zone, 1);
                            }
                            else if (matrix1[1] < 6)
                            {
                                placeable = false;
                                placingShipRect[1].Visibility = Visibility.Visible;
                                placingShipRect[2].Visibility = Visibility.Visible;
                                placingShipRect[3].Visibility = Visibility.Hidden;
                                Canvas.SetLeft(placingShipRect[1], 172 + matrix1[0] * 50);
                                Canvas.SetTop(placingShipRect[1], 160 + (matrix1[1] + 1) * 50);
                                Canvas.SetLeft(placingShipRect[2], 172 + matrix1[0] * 50);
                                Canvas.SetTop(placingShipRect[2], 160 + (matrix1[1] + 2) * 50);
                            }
                            else if (matrix1[1] < 7)
                            {
                                placeable = false;
                                placingShipRect[1].Visibility = Visibility.Visible;
                                placingShipRect[2].Visibility = Visibility.Hidden;
                                placingShipRect[3].Visibility = Visibility.Hidden;
                                Canvas.SetLeft(placingShipRect[1], 172 + matrix1[0] * 50);
                                Canvas.SetTop(placingShipRect[1], 160 + (matrix1[1] + 1) * 50);
                            }
                            else
                            {
                                placeable = false;
                                placingShipRect[1].Visibility = Visibility.Hidden;
                                placingShipRect[2].Visibility = Visibility.Hidden;
                                placingShipRect[3].Visibility = Visibility.Hidden;
                            }
                        }

                    }
                    setCustomPointerSprite(1);
                }
                else
                {
                    placingShipRect[0].Visibility = Visibility.Hidden;
                    placingShipRect[1].Visibility = Visibility.Hidden;
                    placingShipRect[2].Visibility = Visibility.Hidden;
                    placingShipRect[3].Visibility = Visibility.Hidden;
                }

                if (placeable)
                {
                    placingShipRect[0].Fill = Brushes.Green;
                    placingShipRect[1].Fill = Brushes.Green;
                    placingShipRect[2].Fill = Brushes.Green;
                    placingShipRect[3].Fill = Brushes.Green;
                }
                else
                {
                    placingShipRect[0].Fill = Brushes.Red;
                    placingShipRect[1].Fill = Brushes.Red;
                    placingShipRect[2].Fill = Brushes.Red;
                    placingShipRect[3].Fill = Brushes.Red;
                }
            }
        }
    }
}
