using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Numerics;
using System.Security.Policy;
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
            SCORING,
            INFO1,
            INFO2,
            INFOINGAME
        }
        public enum Turn
        {
            P1,
            P2
        }

        public GameTable p1Table = new GameTable(game_select.instance.Player1Box.Text);
        public GameTable p2Table = new GameTable(game_select.instance.Player2Box.Text);
        public static Torpedo_1v1 instance;


        public GameData gameData = new GameData(game_select.instance.Player1Box.Text, game_select.instance.Player2Box.Text);

        List<ImageBrush> cursorSprites = new List<ImageBrush>();
        List<ImageBrush> rotateSprites = new List<ImageBrush>();
        List<ImageBrush> eyeSprites = new List<ImageBrush>();
        List<ImageBrush> SoundSprites = new List<ImageBrush>();
        public bool muted = false;
        public List<ImageBrush> shipSprites = new List<ImageBrush>();
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

        States STATE = States.INFO1;



        int rotation = 0;
        int selectedShipType = 4;
        bool played = false;

        string[] columnName = new string[] { "A", "B", "C", "D", "E", "F", "G", "H" };
        BrushConverter bc = new BrushConverter();

        public int[] maxPlaceableShips = { 4, 2, 1 };
        public int[] currP1Ships = { 0, 0, 0 };
        public int[] currP2Ships = { 0, 0, 0 };

        bool placeable = false;
        bool p1shipVisible = false;
        bool p2shipVisible = false;

        public Turn game_curr_turn = Turn.P1;
        public int currTurn = 1;
        GameStepsWindow gameStepsWindow = new GameStepsWindow();
        public Turn firstTurn = Turn.P1;


        public List<Rectangle> placed_canvas_rectangles = new List<Rectangle>();
        public List<TurnElement> newSteps = new List<TurnElement>();
        public bool is_turn_changed = false;

        public bool Mode = true;

        public int selectedTurnIndex = -1;


        public Torpedo_1v1()
        {
            instance = this;
            InitializeComponent();
            loadSpriteDatas();
            playMusic();
            //mediaPlayer.Volume= 1;
            game_bg.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\csharp_torpedo.png", UriKind.Absolute))
            };
            Bg.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\spr_underwater_bg.png", UriKind.Absolute))
            };
            refreshScores();


            //MediaPlayer bgSound = new MediaPlayer();
            //Uri uri = new Uri(System.IO.Directory.GetCurrentDirectory() + @"\sounds\game_bg_music.wav", UriKind.Absolute);
            //bgSound.Open(uri);
            //bgSound.Play();




            canvas.Children.Add(selectedRectangle);
            p1_name.Text = p1Table.Name;
            p2_name.Text = p2Table.Name;
            P_name.Content = p2Table.Name;



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
            P1_eyeButton.Fill = eyeSprites[1];
            P2_eyeButton.Fill = eyeSprites[1];
            Sound_Button.Fill = SoundSprites[0];
            p1_frig.Foreground = Brushes.Green;
            setCustomPointerSprite(0);

        }
        // Grid kiarjzolása
        public void drawSelectedGrid()
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
                P1_eyeButton.Visibility = Visibility.Visible;
                P1_eyeButton.Visibility = Visibility.Visible;
                Sound_Button.Visibility = Visibility.Visible;


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
            else if (STATE == States.INFO1)
            {
                InfoText.Content = "Turn around while " + p1Table.Name + Environment.NewLine + " places own ships!";
                P_name.Content = p2Table.Name;
                P_name.Content = p1Table.Name;
                P_name.Visibility = Visibility.Visible;
                InfoText.Visibility = Visibility.Visible;
                OkButton.Visibility = Visibility.Visible;
                Bg.Visibility = Visibility.Visible;
                refreshScores();
            }
            else if (STATE == States.INFO2)
            {
                InfoText.Content = "Turn around while " + p2Table.Name + Environment.NewLine + " places own ships!";
                P_name.Content = p1Table.Name;
                P_name.Visibility = Visibility.Visible;
                InfoText.Visibility = Visibility.Visible;
                OkButton.Visibility = Visibility.Visible;
                Bg.Visibility = Visibility.Visible;
            }
            else if (STATE == States.INFOINGAME)
            {
                if (game_curr_turn == Turn.P1)
                {
                    InfoText.Content = "Turn around while " + p1Table.Name + Environment.NewLine + " check own ships!";
                    P_name.Content = p2Table.Name;
                    P_name.Visibility = Visibility.Visible;
                    InfoText.Visibility = Visibility.Visible;
                    OkButton.Visibility = Visibility.Visible;
                    Bg.Visibility = Visibility.Visible;
                }
                else
                {
                    InfoText.Content = "Turn around while " + p2Table.Name + Environment.NewLine + " check own ships!";
                    P_name.Content = p1Table.Name;
                    P_name.Visibility = Visibility.Visible;
                    InfoText.Visibility = Visibility.Visible;
                    OkButton.Visibility = Visibility.Visible;
                    Bg.Visibility = Visibility.Visible;
                }
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
            
        }
        // ------------------------- GOMB NYOMÁS ESEMÉNY -------------------
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.P)
            {
                if (game_curr_turn == Turn.P2)
                {
                    if (!p2shipVisible) { STATE = States.INFOINGAME; }
                    else ShowAndHideShips(p2Table);
                    p2shipVisible = false;
                }
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.Q)
            {
                if (game_curr_turn == Turn.P1)
                {
                    if (!p1shipVisible) STATE = States.INFOINGAME;
                    else ShowAndHideShips(p1Table);
                    p1shipVisible = false;
                }

            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.N)
            {
                game_select.mediaPlayer.Volume -= 0.1;
            }
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.H)
            {
                game_select.mediaPlayer.Volume += 0.1;

            }


        }

        private void ShowAndHideShips(GameTable gametable)
        {

            if (gametable == p1Table)
            {
                P1_eyeButton.Fill = (P1_eyeButton.Fill == eyeSprites[0] ? eyeSprites[1] : eyeSprites[0]);
            }
            else
            {
                P2_eyeButton.Fill = (P2_eyeButton.Fill == eyeSprites[0] ? eyeSprites[1] : eyeSprites[0]);
            }
            foreach (Ships ship in gametable.ships)
            {
                if (!ship.Destroyed)
                {
                    ship.shipBody.Visibility = (ship.shipBody.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible);
                }
            }
        }
        //------------------------------------- BAL KLIKK ESEMÉNYEK ---------------------------------

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (STATE == States.PLAYING)
            {
                Point p = e.GetPosition(canvas);
                Debug.WriteLine(p.X + ":" + p.Y);
                if (game_curr_turn == Turn.P1)
                {

                    if (p.X > 162 && p.X < 203 && p.Y > 118 && p.Y < 150)
                    {
                        if (!p1shipVisible)
                        {
                            STATE = States.INFOINGAME;
                            //P1_eyeButton.Fill = eyeSprites[1];
                        }
                        else
                        {
                            ShowAndHideShips(p1Table);
                            //P1_eyeButton.Fill = eyeSprites[0];
                        }
                        p1shipVisible = false;
                    }
                }
                else
                {
                    if (p.X > 703 && p.X < 744 && p.Y > 116 && p.Y < 156)
                    {
                        if (!p2shipVisible)
                        {
                            STATE = States.INFOINGAME;
                            //P2_eyeButton.Fill = eyeSprites[1];
                        }
                        else
                        {
                            ShowAndHideShips(p2Table);
                            //P2_eyeButton.Fill = eyeSprites[0];
                        }
                        p2shipVisible = false;
                    }
                }
                if (p.X > 612 && p.X < 664 && p.Y > 502 && p.Y < 545)
                {
                    if (muted)
                    {
                        Sound_Button.Fill = SoundSprites[0];

                        soundSetVolume(0.7);

                    }
                    else
                    {
                        Sound_Button.Fill = SoundSprites[1];

                        soundSetVolume(0.0);
                    }
                    muted = !muted;
                }


                rotationShipView.Fill = (rotation == 0 ? rotateSprites[2] : rotateSprites[3]);
                rotationShipView.Fill = (rotation == 0 ? rotateSprites[2] : rotateSprites[3]);
                //P1_eyeButton.Fill = (p1shipVisible ? eyeSprites[1] : eyeSprites[0]);
                //P2_eyeButton.Fill = (p2shipVisible ? eyeSprites[1] : eyeSprites[0]);
                if (game_curr_turn == Turn.P1 && mouseSide == 1 && mouseX > 0 && mouseX < 9 && mouseY > 0 && mouseY < 9)
                {
                    if (p2Table.getCoordinate(mouseY, mouseX).Value == 0 || p2Table.getCoordinate(mouseY, mouseX).Value >= 2 && p2Table.getCoordinate(mouseY, mouseX).Value <= 4)
                    {
                        if (is_turn_changed && selectedTurnIndex != -1)
                        {

                            is_turn_changed = false;
                            gameData.refreshStepsWindow(selectedTurnIndex);
                            //gameData.Steps = newSteps;
                        }

                        bool isScored = p2Table.isScored(mouseY, mouseX);
                        bool shot_result = p2Table.makeAShot(mouseY, mouseX);
                        createMarker(mouseX, mouseY, isScored, MARGINLEFT2);
                        gameData.saveMove("P1", currTurn, p1Table.getCoordinate(mouseY, mouseX));
                        Debug.WriteLine(gameData.Steps[gameData.Steps.Count - 1].ToString());
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
                            if (firstTurn == game_curr_turn) currTurn++;
                        }
                        //gameData.saveMove(mouseX - 1, mouseY, isScored, p1Table.Name);
                        drawSelectedGrid();
                        refreshScores();
                    }
                }
                if (game_curr_turn == Turn.P2 && mouseSide == 0 && mouseX > 0 && mouseX < 9 && mouseY > 0 && mouseY < 9)
                {
                    if (p1Table.getCoordinate(mouseY, mouseX).Value == 0 || p1Table.getCoordinate(mouseY, mouseX).Value >= 2 && p1Table.getCoordinate(mouseY, mouseX).Value <= 4)
                    {
                        if (is_turn_changed && selectedTurnIndex != -1)
                        {
                            is_turn_changed = false;
                            gameData.refreshStepsWindow(selectedTurnIndex);
                            //gameData.Steps = newSteps;
                        }
                        bool isScored = p1Table.isScored(mouseY, mouseX);
                        bool shot_result = p1Table.makeAShot(mouseY, mouseX);
                        createMarker(mouseX, mouseY, isScored, MARGINLEFT1);
                        gameData.saveMove("P2", currTurn, p2Table.getCoordinate(mouseY, mouseX));
                        Debug.WriteLine(gameData.Steps[gameData.Steps.Count - 1].ToString());
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
                            if (firstTurn == game_curr_turn) currTurn++;
                        }
                        //gameData.saveMove(mouseX - 1, mouseY, isScored, p1Table.Name);
                        drawSelectedGrid();
                        refreshScores();
                    }
                }
            }
            else if (STATE == States.PREP1)
            {
                Point p = e.GetPosition(canvas);
                LeftButtonDownPreps(p1Table, p, p1_gunboat, p1_brig, p1_frig, currP1Ships, MARGINLEFT1);
                if (p.X > 612 && p.X < 664 && p.Y > 502 && p.Y < 545)
                {
                    if (muted)
                    {
                        Sound_Button.Fill = SoundSprites[0];

                        soundSetVolume(0.7);

                    }
                    else
                    {
                        Sound_Button.Fill = SoundSprites[1];

                        soundSetVolume(0.0);
                    }
                    muted = !muted;
                }
            }
            else if (STATE == States.PREP2)
            {
                Point p = e.GetPosition(canvas);
                LeftButtonDownPreps(p2Table, p, p2_gunboat, p2_brig, p2_frig, currP2Ships, MARGINLEFT2);
                if (p.X > 612 && p.X < 664 && p.Y > 502 && p.Y < 545)
                {
                    if (muted)
                    {
                        Sound_Button.Fill = SoundSprites[0];
                        soundSetVolume(1.0);

                    }
                    else
                    {
                        Sound_Button.Fill = SoundSprites[1];
                        soundSetVolume(0.0);
                    }
                    muted = !muted;
                }
            }
            else if (STATE == States.INFO1)
            {
                Point p = e.GetPosition(canvas);
                if (p.X > 484 && p.X < 831 && p.Y > 360 && p.Y < 460)
                {
                    P_name.Visibility = Visibility.Hidden;
                    InfoText.Visibility = Visibility.Hidden;
                    OkButton.Visibility = Visibility.Hidden;
                    Bg.Visibility = Visibility.Hidden;
                    STATE = States.PREP1;
                }
            }
            else if (STATE == States.INFO2)
            {
                Point p = e.GetPosition(canvas);
                if (p.X > 484 && p.X < 831 && p.Y > 360 && p.Y < 460)
                {
                    P_name.Visibility = Visibility.Hidden;
                    InfoText.Visibility = Visibility.Hidden;
                    OkButton.Visibility = Visibility.Hidden;
                    Bg.Visibility = Visibility.Hidden;
                    STATE = States.PREP2;
                }
            }
            else if (STATE == States.INFOINGAME)
            {
                if (game_curr_turn == Turn.P1)
                {
                    Point p = e.GetPosition(canvas);
                    if (p.X > 484 && p.X < 831 && p.Y > 360 && p.Y < 460)
                    {
                        P_name.Visibility = Visibility.Hidden;
                        InfoText.Visibility = Visibility.Hidden;
                        OkButton.Visibility = Visibility.Hidden;
                        Bg.Visibility = Visibility.Hidden;
                        STATE = States.PLAYING;
                        ShowAndHideShips(p1Table);
                    }
                    p1shipVisible = true;
                }
                else
                {
                    Point p = e.GetPosition(canvas);
                    if (p.X > 484 && p.X < 831 && p.Y > 360 && p.Y < 460)
                    {
                        P_name.Visibility = Visibility.Hidden;
                        InfoText.Visibility = Visibility.Hidden;
                        OkButton.Visibility = Visibility.Hidden;
                        Bg.Visibility = Visibility.Hidden;
                        STATE = States.PLAYING;
                        ShowAndHideShips(p2Table);
                    }
                    p2shipVisible = true;
                }
            }
        }

        private void soundSetVolume(double v)
        {
            mediaPlayer.Volume= v;
        }

        public void makeP1Shots(int mouseY, int mouseX)
        {
            if (p2Table.getCoordinate(mouseY, mouseX).Value == 0 || p2Table.getCoordinate(mouseY, mouseX).Value >= 2 && p2Table.getCoordinate(mouseY, mouseX).Value <= 4)
            {
                bool isScored = p2Table.isScored(mouseY, mouseX);
                bool shot_result = p2Table.makeAShot(mouseY, mouseX);
                createMarker(mouseX, mouseY, isScored, MARGINLEFT2);
                //gameData.saveMove("P1", currTurn, p1Table.getCoordinate(mouseY, mouseX));
                //Debug.WriteLine(gameData.Steps[gameData.Steps.Count - 1].ToString());
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
                    if (firstTurn == game_curr_turn) currTurn++;
                }
                //gameData.saveMove(mouseX - 1, mouseY, isScored, p1Table.Name);
                drawSelectedGrid();
                refreshScores();
            }
        }

        public void MakeP2Shots(int mouseY, int mouseX)
        {
            if (p1Table.getCoordinate(mouseY, mouseX).Value == 0 || p1Table.getCoordinate(mouseY, mouseX).Value >= 2 && p1Table.getCoordinate(mouseY, mouseX).Value <= 4)
            {
                bool isScored = p1Table.isScored(mouseY, mouseX);
                bool shot_result = p1Table.makeAShot(mouseY, mouseX);
                createMarker(mouseX, mouseY, isScored, MARGINLEFT1);
                //gameData.saveMove("P2", currTurn, p2Table.getCoordinate(mouseY, mouseX));
                //Debug.WriteLine(gameData.Steps[gameData.Steps.Count - 1].ToString());
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
                    if (firstTurn == game_curr_turn) currTurn++;
                }
                //gameData.saveMove(mouseX - 1, mouseY, isScored, p1Table.Name);
                drawSelectedGrid();
                refreshScores();
            }
        }

        void LeftButtonDownPreps(GameTable pTable, Point p, TextBlock p_gunboat, TextBlock p_brig, TextBlock p_frig, int[] currPShips, int MARGINLEFT)
        {
            if (placeable)
            {
                pTable.placeShip(mouseY, mouseX, rotation, selectedShipType);
                pTable.getShipByCoordinate(mouseY, mouseX).shipBody.Fill = shipSprites[pTable.getShipByCoordinate(mouseY, mouseX).SpriteIndex];
                canvas.Children.Add(pTable.getShipByCoordinate(mouseY, mouseX).shipBody);
                placed_canvas_rectangles.Add(pTable.getShipByCoordinate(mouseY, mouseX).shipBody);
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
                        STATE = States.INFO2;
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
                    else
                    {
                        STATE = States.PLAYING;
                        gameStepsWindow.Show();
                        gameStepsWindow.Left = this.Left + this.Width;
                        gameStepsWindow.Top = this.Top + this.Height / 6;
                        game_turn.Visibility = Visibility.Visible;
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

        void drawTableShips(GameTable gT, int PlayerIndex)
        {
            foreach (Ships element in gT.ships)
            {
                element.shipBody.Fill = shipSprites[element.SpriteIndex];
                element.shipBody.Visibility = Visibility.Hidden;

                canvas.Children.Add(element.shipBody);
                placed_canvas_rectangles.Add(element.shipBody);
                Canvas.SetTop(element.shipBody, MARGINTOP + (element.StartingCoordinates.X - 1) * CELLSIZE - (element.Rotation == 0 ? (element.Type == 2 ? -10 : element.Type == 3 ? 13 : 25) : -element.shipBody.Width));
                Canvas.SetLeft(element.shipBody, (PlayerIndex == 0 ? MARGINLEFT1 : MARGINLEFT2) + (element.StartingCoordinates.Y - 1) * CELLSIZE - (element.Rotation == 0 ? (element.Type == 4 ? 15 : 0) : (element.Type == 2 ? -10 : element.Type == 3 ? 13 : 25)));

            }
        }

        void resetToDefault()
        {
            foreach (Rectangle rect in placed_canvas_rectangles)
            {
                canvas.Children.Remove(rect);
            }
        }

        public void backToSelectedTurn(int ind)
        {
            resetToDefault();
            gameData.reset();
            p1Table.setTableToDefault();
            p2Table.setTableToDefault();
            drawTableShips(p2Table, 1);
            drawTableShips(p1Table, 0);
            TurnElement te = null;

            //ITT KELLENE LŐNI VALAHOGY MERT NEM MENT
            game_curr_turn = Turn.P1;
            currTurn = (te == null ? 1 : te.Round);


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

            // Eyes Sprites
            eyeSprites.Add(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\szem_nyitott.png", UriKind.Absolute))
            });
            eyeSprites.Add(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\szem_lehuzva.png", UriKind.Absolute))
            });

            //Sound Sprites
            SoundSprites.Add(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\hang_on.png", UriKind.Absolute))
            }); SoundSprites.Add(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\hang_off.png", UriKind.Absolute))
            });


        }
        public void createMarker(int XX, int YY, bool scored, int MARGINLEFT)
        {
            Rectangle markerRect = new Rectangle
            {
                Width = CELLSIZE,
                Height = CELLSIZE,
                Opacity = 0.6,
                Fill = (scored ? markerSprites[0] : markerSprites[1])
            };
            canvas.Children.Add(markerRect);
            placed_canvas_rectangles.Add(markerRect);
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
        public void refreshScores()
        {
            p1_Scores.Text = "Score: " + EndOfMatch.P1Wins;
            p2_Scores.Text = "Score: " + EndOfMatch.P2Wins;
            //Debug.Write(EndOfMatch.P1Wins.ToString() + " : " + EndOfMatch.P2Wins.ToString());
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
            game_turn.Text = currTurn.ToString();
            gameData.TURN = currTurn;

            WinCheck(played);
        }
        void WinCheck(bool played)
        {
            if (played)
            {
                if (p1_brig.Text == "0/2" && p1_frig.Text == "0/1" && p1_gunboat.Text == "0/4")
                {
                    EndOfMatch eom = new EndOfMatch();
                    eom.P1Miss.Content = "Miss: " + p1_miss.Text;
                    eom.P1Hit.Content = "Hit: " + p1_hit.Text;
                    eom.P2Miss.Content = "Miss: " + p2_miss.Text;
                    eom.P2Hit.Content = "Hit: " + p2_hit.Text;
                    eom.winnerText.Content = p2Table.Name + " WINS!!";
                    gameData.WON = p2Table.Name;
                    eom.WINNER = "2";
                    eom.Player1Box.Text = p1Table.Name;
                    eom.Player2Box.Foreground = Brushes.Green;
                    eom.Player2Box.Text = p2Table.Name;
                    eom.Player1Box.Foreground = Brushes.Red;
                    eom.Show();
                    FinalData finalData = new FinalData(gameData.P1_NAME, gameData.P2_NAME, gameData.P1_HIT, gameData.P2_HIT, gameData.P1_MISS, gameData.P2_MISS, gameData.TURN, gameData.WON);
                    ScoreData scoreData = new ScoreData();
                    scoreData.loadJsonFile();
                    scoreData.gameDatas.Add(finalData);
                    scoreData.writeJsonFile();
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
                    gameData.WON = p1Table.Name;
                    eom.WINNER = "1";
                    eom.Player1Box.Text = p1Table.Name;
                    eom.Player1Box.Foreground = Brushes.Green;
                    eom.Player2Box.Text = p2Table.Name;
                    eom.Player2Box.Foreground = Brushes.Red;
                    eom.Show();
                    FinalData finalData = new FinalData(gameData.P1_NAME, gameData.P2_NAME, gameData.P1_HIT, gameData.P2_HIT, gameData.P1_MISS, gameData.P2_MISS, gameData.TURN, gameData.WON);
                    ScoreData scoreData = new ScoreData();
                    scoreData.loadJsonFile();
                    scoreData.gameDatas.Add(finalData);
                    scoreData.writeJsonFile();
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
                if (pX > 595 && pX < 300 && pY > 366 && pY < 460)
                {
                    setCustomPointerSprite(1);
                }
            }
            if (STATE == States.PLAYING)
            {
                if (game_curr_turn == Turn.P1)
                {
                    if (pX > 162 && pX < 203 && pY > 118 && pY < 150)
                    {
                        setCustomPointerSprite(1);
                        //P1_eyeButton.Fill = eyeSprites[1];
                    }
                }
                else
                {
                    if (pX > 703 && pX < 744 && pY > 116 && pY < 156)
                    {
                        setCustomPointerSprite(1);
                        //P1_eyeButton.Fill = eyeSprites[1];
                    }
                }
            }
        }
        /*private void PopupOkButtonPressed(object sender, RoutedEventArgs e)
        {
            P_name.Visibility = Visibility.Hidden;
            InfoText.Visibility = Visibility.Hidden;
            OkButton.Visibility = Visibility.Hidden;
            Bg.Visibility = Visibility.Hidden;

        }*/

        private void infoCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = e.GetPosition(infoCanvas);
            Debug.WriteLine("Poz: " + p.X + " : " + p.Y);
            locatePressableElements(p.X, p.Y);

        }
        private void Window_LocationChanged(object sender, EventArgs e)
        {
            gameStepsWindow.Left = this.Left + this.Width;
            gameStepsWindow.Top = this.Top + this.Height / 6;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            gameStepsWindow.Close();
        }
        public static MediaPlayer mediaPlayer = new MediaPlayer();

        public static void playMusic()
        {
            Uri uri = new Uri(Directory.GetCurrentDirectory() + @"\sounds\game_bg_music.wav", UriKind.Absolute);
            mediaPlayer.Open(uri);
            mediaPlayer.MediaEnded += new EventHandler(Media_Ended);
            mediaPlayer.Play();
        }

        private static void Media_Ended(object? sender, EventArgs e)
        {
            Uri uri = new Uri(Directory.GetCurrentDirectory() + @"\sounds\game_bg_music.wav", UriKind.Absolute);
            mediaPlayer.Open(uri);
            mediaPlayer.Play();
        }
    }
}

