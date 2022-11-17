        using System;
        using System.Collections.Generic;
        using System.Diagnostics;
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
                enum States
                {
                    PREP1,
                    PREP2,
                    PLAYING,
                    SCORING
                }
                enum Turn
                {
                    P1,
                    P2
                }

                GameTable p1Table = new GameTable("Player1");
                GameTable p2Table = new GameTable("Player2");


                GameData gameData = new GameData();

                List<ImageBrush> cursorSprites = new List<ImageBrush>();
                List<ImageBrush> rotateSprites = new List<ImageBrush>();
                List<ImageBrush> shipSprites = new List<ImageBrush>();
                List<ImageBrush> markerSprites = new List<ImageBrush>();

                List<Rectangle> placingShipRect = new List<Rectangle>();

                int MARGINLEFT1 = 172;
                int MARGINLEFT2 = 708;
                int MARGINTOP = 160;
                int CELLSIZE = 50;

                int mouseX = 0;
                int mouseY = 0;
                int mouseSide = 0;

                Rectangle selectedRectangle = new Rectangle
                {
                    Width = 50,
                    Height = 50,
                    Opacity = 0.3,
                    Fill = new SolidColorBrush(Color.FromRgb(0, 0, 255))
                };

                States STATE = States.PREP1;

                int rotation = 0;
                int selectedShipType = 4;
                bool played = false;

                string[] columnName = new string[] { "A", "B", "C", "D", "E", "F", "G", "H" };
                BrushConverter bc = new BrushConverter();

                int[] maxPlaceableShips = { 4, 2, 1 };
                int[] currP1Ships = { 0, 0, 0 };
                int[] currP2Ships = { 0, 0, 0 };

                bool placeable = false;

                Turn game_curr_turn = Turn.P1;


                public Torpedo_1v1()
                {
                    InitializeComponent();
                    loadSpriteDatas();
                    game_bg.Fill = new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\csharp_torpedo.png", UriKind.Absolute))
                    };
                    refreshScores();
                    canvas.Children.Add(selectedRectangle);
                    p1_name.Text = p1Table.Name;
                    p2_name.Text = p2Table.Name;
                    
                    curr_turn.Text = p1Table.Name + " Placing Ships";

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
                    rotationShipView.Fill = rotateSprites[2];
                    rotationButton.Fill = rotateSprites[0];
                    p1_frig.Foreground = Brushes.Green;
                    setCustomPointerSprite(0);

                }
                // Grid kiarjzolása
                void drawSelectedGrid()
                {
                    if (STATE == States.PLAYING)
                    {
                        played = true;
                        placingShipRect[0].Visibility = Visibility.Hidden;
                        placingShipRect[1].Visibility = Visibility.Hidden;
                        placingShipRect[2].Visibility = Visibility.Hidden;
                        placingShipRect[3].Visibility = Visibility.Hidden;
                        rotationButton.Visibility = Visibility.Hidden;
                        rotationShipView.Visibility = Visibility.Hidden;


                        if (game_curr_turn == Turn.P1)
                        {
                            DrawPlaying(p2Table);
                        }
                        else
                        {
                            DrawPlaying(p1Table);
                        }
                    }


                    else if (STATE == States.PREP1)
                    {
                        DrawPreps(0, p1Table, MARGINLEFT1);
                    }


                    else if (STATE == States.PREP2)
                    {
                        DrawPreps(1, p2Table, MARGINLEFT2);
                    }
                }

                void DrawPlaying(GameTable gametable)
                {
                    if (mouseX > 0 && mouseX < 9 && mouseY > 0 && mouseY < 9)
                    {
                        selectedRectangle.Visibility = Visibility.Visible;
                        selected_index_name.Visibility = Visibility.Visible;
                        Canvas.SetTop(selectedRectangle, MARGINTOP + (mouseY - 1) * CELLSIZE);
                        Canvas.SetLeft(selectedRectangle, (mouseSide == 0 ? MARGINLEFT1 : MARGINLEFT2) + (mouseX - 1) * CELLSIZE);

                        if (mouseSide == 1 && gametable == p2Table)
                        {
                            if (gametable.getCoordinate(mouseY, mouseX).Value > 4 || gametable.getCoordinate(mouseY, mouseX).Value == 1)
                            {
                                setCustomPointerSprite(0);
                                selected_index_name.Foreground = Brushes.Red;
                                selectedRectangle.Fill = Brushes.Red;
                            }
                            else
                            {
                                setCustomPointerSprite(1);
                                selected_index_name.Foreground = Brushes.Lime;
                                selectedRectangle.Fill = bc.ConvertFrom("#3a76a9") as Brush;
                            }
                        }
                        else if (mouseSide == 0 && gametable == p1Table)
                        {
                            if (gametable.getCoordinate(mouseY, mouseX).Value > 4 || gametable.getCoordinate(mouseY, mouseX).Value == 1)
                            {
                                setCustomPointerSprite(0);
                                selected_index_name.Foreground = Brushes.Red;
                                selectedRectangle.Fill = Brushes.Red;
                            }
                            else
                            {
                                setCustomPointerSprite(1);
                                selected_index_name.Foreground = Brushes.Lime;
                                selectedRectangle.Fill = bc.ConvertFrom("#3a76a9") as Brush;
                            }
                        }
                        else
                        {
                            setCustomPointerSprite(1);
                            selected_index_name.Foreground = Brushes.Lime;
                            selectedRectangle.Fill = bc.ConvertFrom("#3a76a9") as Brush;
                        }

                        selected_index_name.Text = columnName[mouseX - 1] + (mouseY);


                        Canvas.SetTop(selected_index_name, MARGINTOP + (mouseY - 1) * CELLSIZE);
                        Canvas.SetLeft(selected_index_name, (mouseSide == 0 ? MARGINLEFT1 : MARGINLEFT2) + (mouseX - 1) * CELLSIZE);
                    }

                    else
                    {
                        selectedRectangle.Visibility = Visibility.Hidden;
                        selected_index_name.Visibility = Visibility.Hidden;
                        setCustomPointerSprite(0);
                    }
                }

                void DrawPreps(int mouseSides, GameTable gametable, int MARGINLEFTSIDE)
                {
                    selectedRectangle.Visibility = Visibility.Hidden;
                    selected_index_name.Visibility = Visibility.Hidden;

                    rotationButton.Visibility = Visibility.Visible;
                    rotationShipView.Visibility = Visibility.Visible;

                    if (selectedShipType >= 2 && mouseSide == mouseSides && mouseX > 0 && mouseX < 9 && mouseY > 0 && mouseY < 9)
                    {
                        placingShipRect[0].Visibility = Visibility.Visible;

                        if (rotation == 0)
                        {
                            placingShipRect[1].Visibility = (mouseX > 7 || selectedShipType < 2 ? Visibility.Hidden : Visibility.Visible);
                            placingShipRect[2].Visibility = (mouseX > 6 || selectedShipType < 3 ? Visibility.Hidden : Visibility.Visible);
                            placingShipRect[3].Visibility = (mouseX > 5 || selectedShipType < 4 ? Visibility.Hidden : Visibility.Visible);
                        }
                        else
                        {
                            placingShipRect[1].Visibility = (mouseY > 7 || selectedShipType < 2 ? Visibility.Hidden : Visibility.Visible);
                            placingShipRect[2].Visibility = (mouseY > 6 || selectedShipType < 3 ? Visibility.Hidden : Visibility.Visible);
                            placingShipRect[3].Visibility = (mouseY > 5 || selectedShipType < 4 ? Visibility.Hidden : Visibility.Visible);
                        }
                        placeable = gametable.isShipPlaceable(mouseY, mouseX, rotation, selectedShipType);
                        if (placeable)
                        {
                            setCustomPointerSprite(1);
                            placingShipRect[0].Fill = Brushes.Lime;
                            placingShipRect[1].Fill = Brushes.Lime;
                            placingShipRect[2].Fill = Brushes.Lime;
                            placingShipRect[3].Fill = Brushes.Lime;
                        }
                        else
                        {
                            setCustomPointerSprite(0);
                            placingShipRect[0].Fill = Brushes.Red;
                            placingShipRect[1].Fill = Brushes.Red;
                            placingShipRect[2].Fill = Brushes.Red;
                            placingShipRect[3].Fill = Brushes.Red;
                        }


                        Canvas.SetTop(placingShipRect[0], MARGINTOP + (mouseY - 1) * CELLSIZE);
                        Canvas.SetLeft(placingShipRect[0], MARGINLEFTSIDE + (mouseX - 1) * CELLSIZE);

                        Canvas.SetTop(placingShipRect[1], MARGINTOP + (mouseY - (rotation == 0 ? 1 : 0)) * CELLSIZE);
                        Canvas.SetLeft(placingShipRect[1], MARGINLEFTSIDE + (mouseX - (rotation == 1 ? 1 : 0)) * CELLSIZE);

                        Canvas.SetTop(placingShipRect[2], MARGINTOP + (mouseY - (rotation == 0 ? 1 : -1)) * CELLSIZE);
                        Canvas.SetLeft(placingShipRect[2], MARGINLEFTSIDE + (mouseX - (rotation == 1 ? 1 : -1)) * CELLSIZE);

                        Canvas.SetTop(placingShipRect[3], MARGINTOP + (mouseY - (rotation == 0 ? 1 : -2)) * CELLSIZE);
                        Canvas.SetLeft(placingShipRect[3], MARGINLEFTSIDE + (mouseX - (rotation == 1 ? 1 : -2)) * CELLSIZE);
                    }
                    else
                    {
                        setCustomPointerSprite(0);
                        placingShipRect[0].Visibility = Visibility.Hidden;
                        placingShipRect[1].Visibility = Visibility.Hidden;
                        placingShipRect[2].Visibility = Visibility.Hidden;
                        placingShipRect[3].Visibility = Visibility.Hidden;
                        placeable = false;
                    }
                }

            


            // ------------------- JOBB KLIKK ESEMÉNY ----------------------
                private void canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
                {
                    Debug.WriteLine(gameData.toJSON());
                }
            // ------------------------- GOMB NYOMÁS ESEMÉNY -------------------
                private void Window_KeyDown(object sender, KeyEventArgs e)
                {

                    if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.P)
                    {
                        foreach (Ships ship in p2Table.ships)
                        {
                            if (!ship.Destroyed)
                            {
                                ship.shipBody.Visibility = (ship.shipBody.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible);
                            }
                        }
                    }

                    if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.Q)
                    {
                        foreach (Ships ship in p1Table.ships)
                        {
                            if (!ship.Destroyed)
                            {
                                ship.shipBody.Visibility = (ship.shipBody.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible);
                            }
                        }
                    }

                }
            //------------------------------------- BAL KLIKK ESEMÉNYEK ---------------------------------

                private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
                {
                    if (STATE == States.PLAYING)
                    {
                        if (game_curr_turn == Turn.P1 && mouseSide == 1 && mouseX > 0 && mouseX < 9 && mouseY > 0 && mouseY < 9)
                        {
                            if (p2Table.getCoordinate(mouseY, mouseX).Value == 0 || p2Table.getCoordinate(mouseY, mouseX).Value >= 2 && p2Table.getCoordinate(mouseY, mouseX).Value <= 4)
                            {
                                bool isScored = p2Table.isScored(mouseY, mouseX);
                                bool shot_result = p2Table.makeAShot(mouseY, mouseX);
                                createMarker(mouseX, mouseY, isScored, MARGINLEFT2);
                                if (isScored)
                                {
                                    gameData.P1_HIT++;
                                    if (shot_result)
                                    {
                                        p2Table.getShipByCoordinate(mouseY, mouseX).shipBody.Visibility = Visibility.Visible;
                                        p2Table.getShipByCoordinate(mouseY, mouseX).shipBody.Fill = shipSprites[p2Table.getShipByCoordinate(mouseY, mouseX).SpriteIndex];
                                        currP2Ships[p2Table.getShipByCoordinate(mouseY, mouseX).Type - 2]--;
                                        p2_frig.Text = currP2Ships[2] + "/" + maxPlaceableShips[2];
                                        p2_brig.Text = currP2Ships[1] + "/" + maxPlaceableShips[1];
                                        p2_gunboat.Text = currP2Ships[0] + "/" + maxPlaceableShips[0];
                                    }
                                }
                                else
                                {
                                    gameData.P1_MISS++;
                                    game_curr_turn = Turn.P2;
                                }
                                gameData.saveMove(mouseX - 1, mouseY, isScored, p1Table.Name);
                                drawSelectedGrid();
                                refreshScores();
                            }

                        }

                        if (game_curr_turn == Turn.P2 && mouseSide == 0 && mouseX > 0 && mouseX < 9 && mouseY > 0 && mouseY < 9)
                        {
                            if (p1Table.getCoordinate(mouseY, mouseX).Value == 0 || p1Table.getCoordinate(mouseY, mouseX).Value >= 2 && p1Table.getCoordinate(mouseY, mouseX).Value <= 4)
                            {
                                bool isScored = p1Table.isScored(mouseY, mouseX);
                                bool shot_result = p1Table.makeAShot(mouseY, mouseX);
                                createMarker(mouseX, mouseY, isScored, MARGINLEFT1);
                                if (isScored)
                                {
                                    gameData.P2_HIT++;
                                    if (shot_result)
                                    {
                                        p1Table.getShipByCoordinate(mouseY, mouseX).shipBody.Visibility = Visibility.Visible;
                                        p1Table.getShipByCoordinate(mouseY, mouseX).shipBody.Fill = shipSprites[p1Table.getShipByCoordinate(mouseY, mouseX).SpriteIndex];
                                        currP1Ships[p1Table.getShipByCoordinate(mouseY, mouseX).Type - 2]--;
                                        p1_frig.Text = currP1Ships[2] + "/" + maxPlaceableShips[2];
                                        p1_brig.Text = currP1Ships[1] + "/" + maxPlaceableShips[1];
                                        p1_gunboat.Text = currP1Ships[0] + "/" + maxPlaceableShips[0];
                                    }
                                }
                                else
                                {
                                    gameData.P2_MISS++;
                                    game_curr_turn = Turn.P1;
                                }
                                gameData.saveMove(mouseX - 1, mouseY, isScored, p1Table.Name);
                                drawSelectedGrid();
                                refreshScores();
                            }
                        }
                    }
                    else if (STATE == States.PREP1)
                    {
                            Point p = e.GetPosition(canvas);
                            LeftButtonDownPreps(p1Table, p, p1_gunboat, p1_brig, p1_frig, currP1Ships, MARGINLEFT1);
                    }
                    else if (STATE == States.PREP2)
                    {
                            Point p = e.GetPosition(canvas);
                            LeftButtonDownPreps(p2Table, p, p2_gunboat, p2_brig, p2_frig, currP2Ships, MARGINLEFT2);
                    }
                }

                void LeftButtonDownPreps (GameTable pTable, Point p, TextBlock p_gunboat, TextBlock p_brig, TextBlock p_frig, int[] currPShips, int MARGINLEFT)
                {
                    if (placeable)
                    {
                        pTable.placeShip(mouseY, mouseX, rotation, selectedShipType);
                        pTable.getShipByCoordinate(mouseY, mouseX).shipBody.Fill = shipSprites[pTable.getShipByCoordinate(mouseY, mouseX).SpriteIndex];
                        canvas.Children.Add(pTable.getShipByCoordinate(mouseY, mouseX).shipBody);
                        Canvas.SetTop(pTable.getShipByCoordinate(mouseY, mouseX).shipBody, MARGINTOP + (mouseY - 1) * CELLSIZE - (pTable.getShipByCoordinate(mouseY, mouseX).Rotation == 0 ? (pTable.getShipByCoordinate(mouseY, mouseX).Type == 2 ? -10 : pTable.getShipByCoordinate(mouseY, mouseX).Type == 3 ? 13 : 25) : -pTable.getShipByCoordinate(mouseY, mouseX).shipBody.Width));
                        Canvas.SetLeft(pTable.getShipByCoordinate(mouseY, mouseX).shipBody, MARGINLEFT + (mouseX - 1) * CELLSIZE - (pTable.getShipByCoordinate(mouseY, mouseX).Rotation == 0 ? (pTable.getShipByCoordinate(mouseY, mouseX).Type == 4 ? 15 : 0) : (pTable.getShipByCoordinate(mouseY, mouseX).Type == 2 ? -10 : pTable.getShipByCoordinate(mouseY, mouseX).Type == 3 ? 13 : 25)));
                        currPShips[selectedShipType - 2]++;

                        p_gunboat.Text = currPShips[0] + "/" + maxPlaceableShips[0];
                        p_brig.Text = currPShips[1] + "/" + maxPlaceableShips[1];
                        p_frig.Text = currPShips[2] + "/" + maxPlaceableShips[2];

                        if (currPShips[selectedShipType - 2] >= maxPlaceableShips[selectedShipType - 2])
                        {
                            selectedShipType--;
                            p_gunboat.Foreground = bc.ConvertFrom("#3a76a9") as Brush;
                            p_brig.Foreground = bc.ConvertFrom("#3a76a9") as Brush;
                            p_frig.Foreground = bc.ConvertFrom("#3a76a9") as Brush;
                            if (selectedShipType == 3)
                            {
                                p_brig.Foreground = Brushes.Green;
                            }
                            else if (selectedShipType == 2)
                            {
                                p_gunboat.Foreground = Brushes.Green;
                            }
                        }
                        if (currPShips.SequenceEqual(maxPlaceableShips))
                        {
                            if (STATE == States.PREP1)
                            {
                                STATE = States.PREP2;
                                game_curr_turn = Turn.P2;
                                p2_frig.Foreground = Brushes.Green;
                                selectedShipType = 4;
                                p1Table.removeNines();
                                refreshScores();

                                foreach (Ships ship in p1Table.ships)
                                {
                                    if (!ship.Destroyed)
                                    {
                                        ship.shipBody.Visibility = Visibility.Hidden;
                                    }
                                }
                            }
                            else {
                                STATE = States.PLAYING;
                                game_curr_turn = (game_curr_turn == Turn.P1 ? Turn.P2 : Turn.P1);
                                pTable.removeNines();
                                refreshScores();

                                    foreach (Ships ship in pTable.ships)
                                    {
                                        if (!ship.Destroyed)
                                        {
                                            ship.shipBody.Visibility = Visibility.Hidden;
                                        }
                                    }                            
                            }
                            curr_turn.Text = (pTable == p1Table ? p2Table.Name + " Placing Ships" : p1Table.Name + "'s Turn ");
                        }
                        placeable = false;
                        drawSelectedGrid();
                    }
                    if (p.X > 615 && p.X < 670 && p.Y > 330 && p.Y < 380)
                    {
                        rotation = (rotation == 0) ? 1 : 0;
                    }
                    rotationShipView.Fill = (rotation == 0 ? rotateSprites[2] : rotateSprites[3]);
                }


                void loadSpriteDatas()
                {
                    Cursor = Cursors.None;

                    // Rotation Sprites
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


                    // Cursor Sprites
                    cursorSprites.Add(new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_hand.png", UriKind.Absolute))
                    });
                    cursorSprites.Add(new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_stroked_hand.png", UriKind.Absolute))
                    });

                    // Ship Sprites
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

                    // Shot Marker Sprites
                    markerSprites.Add(new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_flatblack_bone_femur.png", UriKind.Absolute))
                    });
                    markerSprites.Add(new ImageBrush
                    {
                        ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_circle.png", UriKind.Absolute))
                    });
                }
                void createMarker(int XX, int YY, bool scored, int MARGINLEFT)
                {
                    Rectangle markerRect = new Rectangle
                    {
                        Width = CELLSIZE,
                        Height = CELLSIZE,
                        Opacity = 0.6,
                        Fill = (scored ? markerSprites[0] : markerSprites[1])
                    };
                    canvas.Children.Add(markerRect);
                    Canvas.SetTop(markerRect, MARGINTOP + (YY - 1) * CELLSIZE);
                    Canvas.SetLeft(markerRect, MARGINLEFT + (XX - 1) * CELLSIZE);
             
                }

                void setCustomPointerSprite(int subimage)
                {
                    if (subimage == 0)
                    {
                        customPointer.Fill = cursorSprites[0];
                    }
                    else if (subimage == 1)
                    {
                        customPointer.Fill = cursorSprites[1];
                    }

                }
                void refreshScores()
                {
                p1_Scores.Text = "Score: " + EndOfMatch.P1Wins;
                p2_Scores.Text = "Score: " + EndOfMatch.P2Wins;
                Debug.Write(EndOfMatch.P1Wins.ToString() + " : " + EndOfMatch.P2Wins.ToString());
                    if (game_curr_turn == Turn.P1)
                    {
                        curr_turn.Text = p1Table.Name + "'s Turn";
                    }
                    else if (game_curr_turn == Turn.P2)
                    {
                        curr_turn.Text = p2Table.Name + "'s Turn";
                    }
                    p1_hit.Text = gameData.P1_HIT.ToString();
                    p1_miss.Text = gameData.P1_MISS.ToString();
                    p2_hit.Text = gameData.P2_HIT.ToString();
                    p2_miss.Text = gameData.P2_MISS.ToString();

                    WinCheck(played);
                }
                void WinCheck(bool played)
                {
                    if (played)
                    {
                        if (p1_brig.Text == "0/2" && p1_frig.Text == "0/1" && p1_gunboat.Text == "0/4")
                        {
                            EndOfMatch eom = new EndOfMatch();
                            eom.P1Miss.Content = "Miss: "+p1_miss.Text;
                            eom.P1Hit.Content = "Hit: "+p1_hit.Text;
                            eom.P2Miss.Content = "Miss: "+p2_miss.Text;
                            eom.P2Hit.Content = "Hit: "+ p2_hit.Text;
                            eom.winnerText.Content = p2Table.Name + " WINS!!";
                            eom.WINNER = "2";
                            eom.Player1Box.Text = p1Table.Name;
                            eom.Player2Box.Foreground = Brushes.Green;
                            eom.Player2Box.Text = p2Table.Name;
                            eom.Player1Box.Foreground = Brushes.Red;
                            eom.Show();
                            this.Close();
                        }
                        if (p2_brig.Text == "0/2" && p2_frig.Text == "0/1" && p2_gunboat.Text == "0/4")
                        {
                            EndOfMatch eom = new EndOfMatch();
                            eom.P1Miss.Content = "Miss: " + p1_miss.Text;
                            eom.P1Hit.Content = "Hit: " + p1_hit.Text;
                            eom.P2Miss.Content = "Miss: " + p2_miss.Text;
                            eom.P2Hit.Content = "Hit: " + p2_hit.Text;
                            eom.winnerText.Content = p1Table.Name + " WINS!!";
                            eom.WINNER = "1";
                            eom.Player1Box.Text = p1Table.Name;
                            eom.Player1Box.Foreground = Brushes.Green;
                            eom.Player2Box.Text = p2Table.Name;
                            eom.Player2Box.Foreground = Brushes.Red;
                            eom.Show();
                            this.Close();
                        }
                    }
                }
                void canvas_MouseMove(object sender, MouseEventArgs e)
                {
                    Point p = e.GetPosition(canvas);
                    double pX = p.X - 15;
                    double pY = p.Y - 10;
                    Canvas.SetTop(customPointer, pY);
                    Canvas.SetLeft(customPointer, pX);

                    if (p.X >= MARGINLEFT2 - CELLSIZE)
                    {
                        mouseX = Convert.ToInt32(Math.Floor((p.X - MARGINLEFT2) / CELLSIZE)) + 1;
                        mouseSide = 1;
                    }
                    else
                    {
                        mouseX = Convert.ToInt32(Math.Floor((p.X - MARGINLEFT1) / CELLSIZE)) + 1;
                        mouseSide = 0;
                    }
                    mouseY = Convert.ToInt32(Math.Floor((p.Y - MARGINTOP) / CELLSIZE)) + 1;
                    if (mouseX < 0) mouseX = -1;
                    if (mouseY < 0) mouseY = -1;
                    if (mouseX > 8) mouseX = 9;
                    if (mouseY > 8) mouseY = 9;

                    drawSelectedGrid();
                    locatePressableElements(p.X, p.Y);
                    Debug.WriteLine(mouseX + " " + mouseY);
                }
                void locatePressableElements(double pX, double pY)
                {
                    if (STATE == States.PREP1 || STATE == States.PREP2)
                    {
                        if (pX > 615 && pX < 670 && pY > 330 && pY < 380)
                        {
                            setCustomPointerSprite(1);
                            rotationButton.Fill = rotateSprites[1];
                        }
                        else
                        {
                            rotationButton.Fill = rotateSprites[0];
                        }
                    }
                }
            }
        }

