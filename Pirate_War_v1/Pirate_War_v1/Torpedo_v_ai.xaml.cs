using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Threading;
using static Pirate_War_v1.Torpedo_1v1;
using static System.Formats.Asn1.AsnWriter;

namespace Pirate_War_v1
{
    /// <summary>
    /// Interaction logic for Torpedo_v_ai.xaml
    /// </summary>
    public partial class Torpedo_v_ai : Window
    {
        enum States
        {
            PREP,
            PLAYING,
            GAMEEND
        }
        enum Turn
        {
            PLAYER,
            AI
        }
        public static Torpedo_v_ai instance;
        GameTable playerTable = new GameTable(game_select.instance.PlayerAiEllen.Text);
        GameTable aiTable = new GameTable("AI");

        GameData gameData = new GameData(game_select.instance.PlayerAiEllen.Text, "AI");

        List<ImageBrush> cursorSprites = new List<ImageBrush>();
        List<ImageBrush> rotateSprites = new List<ImageBrush>();
        List<ImageBrush> shipSprites = new List<ImageBrush>();
        List<ImageBrush> markerSprites = new List<ImageBrush>();
        List<ImageBrush> SoundSprites = new List<ImageBrush>();

        List<Rectangle> placingShipRect = new List<Rectangle>();

        int MARGINLEFT1 = 172;
        int MARGINLEFT2 = 708;
        int MARGINTOP = 160;
        int CELLSIZE = 50;

        int mouseX = 0;
        int mouseY = 0;
        int mouseSide = 0;
        public bool muted = false;

        Rectangle selectedRectangle = new Rectangle
        {
            Width = 50,
            Height = 50,
            Opacity = 0.3,
            Fill = new SolidColorBrush(Color.FromRgb(0, 0, 255))
        };

        States STATE = States.PREP;

        int rotation = 0;
        int selectedShipType = 4;

        string[] columnName = new string[] { "A", "B", "C", "D", "E", "F", "G", "H" };
        BrushConverter bc = new BrushConverter();

        public int[] maxPlaceableShips = { 4, 2, 1 };
        public int[] currPlayerShips = { 0, 0, 0 };
        public int[] currAiShips = { 4, 2, 1 };

        bool placeable = false;

        Turn game_curr_turn = Turn.PLAYER;

        List<Coordinates> aiPossibleShots = new List<Coordinates>();
        List<Coordinates> aiNextTip = new List<Coordinates>();
        List<Coordinates> aiAllShots = new List<Coordinates>();
        Coordinates aiFirstHit;
        Coordinates aiLastHit;


        private readonly DispatcherTimer _timer = new DispatcherTimer(DispatcherPriority.Send);
        private const double RefreshTimeSec = 1;

        int currTurn = 1;
        GameStepsWindow gameStepsWindow = new GameStepsWindow();

        public int selectedTurnIndex = 0;

        Turn firstTurn = Turn.PLAYER;

        public List<Rectangle> placed_canvas_rectangles = new List<Rectangle>();
        public List<TurnElement> newSteps = new List<TurnElement>();
        public bool is_turn_changed = false;

        // MAIN
        public Torpedo_v_ai()
        {
            InitializeComponent();
            instance = this;
            loadSpriteDatas();
            playMusic();
            ai_game_bg.Fill = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\csharp_torpedo.png", UriKind.Absolute))
            };

            canvas.Children.Add(selectedRectangle);
            aiTable.generateRandomShips();
            p1_name.Text = playerTable.Name;
            ai_name.Text = aiTable.Name;
            refreshScores();
            //MediaPlayer bgSound = new MediaPlayer();
            //Uri uri = new Uri(System.IO.Directory.GetCurrentDirectory() + @"\sounds\game_bg_music.wav", UriKind.Absolute);
            //bgSound.Open(uri);
            //bgSound.Play();
            curr_turn.Text = playerTable.Name + " Placing Ships";

            _timer.Interval = TimeSpan.FromSeconds(RefreshTimeSec);
            _timer.Tick += TimerTick;


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
            Sound_Button.Fill = SoundSprites[0];
            p1_frig.Foreground = Brushes.Green;
            setCustomPointerSprite(0);

            setAiRandomName();

            drawTableShips(aiTable, 1);
            drawSelectedGrid();
        }

        // XAML EVENT START
        void OnMouseMoveHandler(object sender, MouseEventArgs e)
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

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (STATE == States.PLAYING)
            {
                if (game_curr_turn == Turn.PLAYER && mouseSide == 1 && mouseX > 0 && mouseX < 9 && mouseY > 0 && mouseY < 9)
                {
                    if (aiTable.getCoordinate(mouseY, mouseX).Value == 0 || aiTable.getCoordinate(mouseY, mouseX).Value >= 2 && aiTable.getCoordinate(mouseY, mouseX).Value <= 4)
                    {
                        if (is_turn_changed)
                        {
                            is_turn_changed = false;
                            //gameData.Steps = newSteps;
                            gameData.refreshStepsWindow(selectedTurnIndex);
                        }
                        bool isScored = aiTable.isScored(mouseY, mouseX);
                        bool shot_result = aiTable.makeAShot(mouseY, mouseX);
                        createMarker(mouseX, mouseY, isScored, MARGINLEFT2);
                        gameData.saveMove("P1", currTurn, aiTable.getCoordinate(mouseY, mouseX));
                        Debug.WriteLine(gameData.Steps[gameData.Steps.Count - 1].ToString());
                        if (isScored)
                        {
                            gameData.P1_HIT++;
                            if (shot_result)
                            {
                                aiTable.getShipByCoordinate(mouseY, mouseX).shipBody.Visibility = Visibility.Visible;
                                aiTable.getShipByCoordinate(mouseY, mouseX).shipBody.Fill = shipSprites[aiTable.getShipByCoordinate(mouseY, mouseX).SpriteIndex];
                                currAiShips[aiTable.getShipByCoordinate(mouseY, mouseX).Type - 2]--;
                                ai_frig.Text = currAiShips[2] + "/" + maxPlaceableShips[2];
                                ai_brig.Text = currAiShips[1] + "/" + maxPlaceableShips[1];
                                ai_gunboat.Text = currAiShips[0] + "/" + maxPlaceableShips[0];
                            }
                        }
                        else
                        {
                            gameData.P1_MISS++;
                            game_curr_turn = Turn.AI;
                            if (firstTurn == game_curr_turn) currTurn++;
                            _timer.Stop();
                            _timer.Start();
                        }
                        drawSelectedGrid();
                        refreshScores();
                        checkIfGameEnded();
                    }

                }
                Point p = e.GetPosition(canvas);
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
            else if (STATE == States.PREP)
            {
                Point p = e.GetPosition(canvas);
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
                if (placeable)
                {
                    playerTable.placeShip(mouseY, mouseX, rotation, selectedShipType);
                    playerTable.getShipByCoordinate(mouseY, mouseX).shipBody.Fill = shipSprites[playerTable.getShipByCoordinate(mouseY, mouseX).SpriteIndex];
                    canvas.Children.Add(playerTable.getShipByCoordinate(mouseY, mouseX).shipBody);
                    placed_canvas_rectangles.Add(playerTable.getShipByCoordinate(mouseY, mouseX).shipBody);
                    Canvas.SetTop(playerTable.getShipByCoordinate(mouseY, mouseX).shipBody, MARGINTOP + (mouseY - 1) * CELLSIZE - (playerTable.getShipByCoordinate(mouseY, mouseX).Rotation == 0 ? (playerTable.getShipByCoordinate(mouseY, mouseX).Type == 2 ? -10 : playerTable.getShipByCoordinate(mouseY, mouseX).Type == 3 ? 13 : 25) : -playerTable.getShipByCoordinate(mouseY, mouseX).shipBody.Width));
                    Canvas.SetLeft(playerTable.getShipByCoordinate(mouseY, mouseX).shipBody, MARGINLEFT1 + (mouseX - 1) * CELLSIZE - (playerTable.getShipByCoordinate(mouseY, mouseX).Rotation == 0 ? (playerTable.getShipByCoordinate(mouseY, mouseX).Type == 4 ? 15 : 0) : (playerTable.getShipByCoordinate(mouseY, mouseX).Type == 2 ? -10 : playerTable.getShipByCoordinate(mouseY, mouseX).Type == 3 ? 13 : 25)));
                    currPlayerShips[selectedShipType - 2]++;

                    p1_gunboat.Text = currPlayerShips[0] + "/" + maxPlaceableShips[0];
                    p1_brig.Text = currPlayerShips[1] + "/" + maxPlaceableShips[1];
                    p1_frig.Text = currPlayerShips[2] + "/" + maxPlaceableShips[2];

                    if (currPlayerShips[selectedShipType - 2] >= maxPlaceableShips[selectedShipType - 2])
                    {
                        selectedShipType--;
                        p1_gunboat.Foreground = bc.ConvertFrom("#3a76a9") as Brush;
                        p1_brig.Foreground = bc.ConvertFrom("#3a76a9") as Brush;
                        p1_frig.Foreground = bc.ConvertFrom("#3a76a9") as Brush;
                        if (selectedShipType == 3)
                        {
                            p1_brig.Foreground = Brushes.Green;
                        }
                        else if (selectedShipType == 2)
                        {
                            p1_gunboat.Foreground = Brushes.Green;
                        }
                    }
                    if (currPlayerShips.SequenceEqual(maxPlaceableShips))
                    {
                        STATE = States.PLAYING;
                        Random r = new Random();
                        Turn[] randTurn = { Turn.PLAYER, Turn.AI };
                        game_curr_turn = randTurn[r.Next(randTurn.Count())];
                        firstTurn = game_curr_turn;
                        playerTable.removeNines();
                        refreshScores();
                        aiPossibleShots = playerTable.aiGeneratePossibleMoves();
                        gameStepsWindow.Show();
                        gameStepsWindow.Left = this.Left + this.Width;
                        gameStepsWindow.Top = this.Top + this.Height / 6;

                        game_turn.Visibility = Visibility.Visible;

                        if (game_curr_turn.Equals(Turn.AI))
                        {
                            _timer.Stop();
                            _timer.Start();
                        }
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
        }
        private void soundSetVolume(double v)
        {
            mediaPlayer.Volume = v;
        }


        public void MakeP1Shots(int mouseY, int mouseX)
        {
            if (aiTable.getCoordinate(mouseY, mouseX).Value == 0 || aiTable.getCoordinate(mouseY, mouseX).Value >= 2 && aiTable.getCoordinate(mouseY, mouseX).Value <= 4)
            {

                bool isScored = aiTable.isScored(mouseY, mouseX);
                bool shot_result = aiTable.makeAShot(mouseY, mouseX);
                createMarker(mouseX, mouseY, isScored, MARGINLEFT2);
                //gameData.saveMove("P1", currTurn, aiTable.getCoordinate(mouseY, mouseX));
                Debug.WriteLine(gameData.Steps[gameData.Steps.Count - 1].ToString());
                if (isScored)
                {
                    gameData.P1_HIT++;
                    if (shot_result)
                    {
                        aiTable.getShipByCoordinate(mouseY, mouseX).shipBody.Visibility = Visibility.Visible;
                        aiTable.getShipByCoordinate(mouseY, mouseX).shipBody.Fill = shipSprites[aiTable.getShipByCoordinate(mouseY, mouseX).SpriteIndex];
                        currAiShips[aiTable.getShipByCoordinate(mouseY, mouseX).Type - 2]--;
                        ai_frig.Text = currAiShips[2] + "/" + maxPlaceableShips[2];
                        ai_brig.Text = currAiShips[1] + "/" + maxPlaceableShips[1];
                        ai_gunboat.Text = currAiShips[0] + "/" + maxPlaceableShips[0];
                    }
                }
                else
                {
                    gameData.P1_MISS++;
                    game_curr_turn = Turn.AI;
                    if (firstTurn == game_curr_turn) currTurn++;
                    //_timer.Stop();
                    //_timer.Start();
                }
                drawSelectedGrid();
                refreshScores();
                checkIfGameEnded();
            }
        }

        public void MakeAIShots(int mouseY, int mouseX)
        {
            if (playerTable.getCoordinate(mouseY, mouseX).Value == 0 || playerTable.getCoordinate(mouseY, mouseX).Value >= 2 && playerTable.getCoordinate(mouseY, mouseX).Value <= 4)
            {

                bool isScored = playerTable.isScored(mouseY, mouseX);
                bool shot_result = playerTable.makeAShot(mouseY, mouseX);
                createMarker(mouseX, mouseY, isScored, MARGINLEFT1);
                //gameData.saveMove("P1", currTurn, aiTable.getCoordinate(mouseY, mouseX));
                Debug.WriteLine(gameData.Steps[gameData.Steps.Count - 1].ToString());
                if (isScored)
                {
                    gameData.P2_HIT++;
                    if (shot_result)
                    {
                        playerTable.getShipByCoordinate(mouseY, mouseX).shipBody.Visibility = Visibility.Visible;
                        playerTable.getShipByCoordinate(mouseY, mouseX).shipBody.Fill = shipSprites[playerTable.getShipByCoordinate(mouseY, mouseX).SpriteIndex];
                        currPlayerShips[playerTable.getShipByCoordinate(mouseY, mouseX).Type - 2]--;
                        p1_frig.Text = currAiShips[2] + "/" + maxPlaceableShips[2];
                        p1_brig.Text = currAiShips[1] + "/" + maxPlaceableShips[1];
                        p1_gunboat.Text = currAiShips[0] + "/" + maxPlaceableShips[0];
                    }
                }
                else
                {
                    gameData.P2_MISS++;
                    game_curr_turn = Turn.PLAYER;
                    if (firstTurn == game_curr_turn) currTurn++;
                    //_timer.Stop();
                    //_timer.Start();
                }
                drawSelectedGrid();
                refreshScores();
                checkIfGameEnded();
            }
        }


        private void canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.P)
            {
                foreach (Ships ship in aiTable.ships)
                {
                    if (!ship.Destroyed)
                    {
                        ship.shipBody.Visibility = (ship.shipBody.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible);
                    }
                }
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.B)
            {
                //resetToDefault();
                newSteps = new List<TurnElement>();
                backToSelectedTurn(selectedTurnIndex);
                is_turn_changed = true;
            }

        }
        // XAML EVENT END

        void setAiRandomName()
        {
            String[] ai_name_first = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\ai_names\\first.txt");
            String[] ai_name_mid = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\ai_names\\mid.txt");
            String[] ai_name_last = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\ai_names\\last.txt");
            Random random = new Random();
            aiTable.Name = ai_name_first[random.Next(0, ai_name_first.Length)] + " " + ai_name_mid[random.Next(0, ai_name_mid.Length)] + " " + ai_name_last[random.Next(0, ai_name_last.Length)];
            ai_name.Text = aiTable.Name;
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
            placed_canvas_rectangles.Add(markerRect);
            Canvas.SetTop(markerRect, MARGINTOP + (YY - 1) * CELLSIZE);
            Canvas.SetLeft(markerRect, MARGINLEFT + (XX - 1) * CELLSIZE);
        }

        void locatePressableElements(double pX, double pY)
        {
            if (STATE == States.PREP)
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

        public void refreshScores()
        {
            if (game_curr_turn == Turn.PLAYER)
            {
                curr_turn.Text = playerTable.Name + "'s Turn";
            }
            else if (game_curr_turn == Turn.AI)
            {
                curr_turn.Text = aiTable.Name + "'s Turn";
            }

            p1_hit.Text = gameData.P1_HIT.ToString();
            p1_miss.Text = gameData.P1_MISS.ToString();
            ai_hit.Text = gameData.P2_HIT.ToString();
            ai_miss.Text = gameData.P2_MISS.ToString();
            game_turn.Text = currTurn.ToString();
            gameData.TURN = currTurn;
        }

        void drawSelectedGrid()
        {
            if (STATE == States.PLAYING)
            {
                placingShipRect[0].Visibility = Visibility.Hidden;
                placingShipRect[1].Visibility = Visibility.Hidden;
                placingShipRect[2].Visibility = Visibility.Hidden;
                placingShipRect[3].Visibility = Visibility.Hidden;

                rotationButton.Visibility = Visibility.Hidden;
                rotationShipView.Visibility = Visibility.Hidden;

                if (mouseX > 0 && mouseX < 9 && mouseY > 0 && mouseY < 9)
                {
                    selectedRectangle.Visibility = Visibility.Visible;
                    selected_index_name.Visibility = Visibility.Visible;
                    Canvas.SetTop(selectedRectangle, MARGINTOP + (mouseY - 1) * CELLSIZE);
                    Canvas.SetLeft(selectedRectangle, (mouseSide == 0 ? MARGINLEFT1 : MARGINLEFT2) + (mouseX - 1) * CELLSIZE);

                    if (mouseSide == 1)
                    {
                        if (aiTable.getCoordinate(mouseY, mouseX).Value > 4 || aiTable.getCoordinate(mouseY, mouseX).Value == 1)
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
            else if (STATE == States.PREP)
            {
                selectedRectangle.Visibility = Visibility.Hidden;
                selected_index_name.Visibility = Visibility.Hidden;

                rotationButton.Visibility = Visibility.Visible;
                rotationShipView.Visibility = Visibility.Visible;

                if (selectedShipType >= 2 && mouseSide == 0 & mouseX > 0 && mouseX < 9 && mouseY > 0 && mouseY < 9)
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

                    placeable = playerTable.isShipPlaceable(mouseY, mouseX, rotation, selectedShipType);
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
                    Canvas.SetLeft(placingShipRect[0], MARGINLEFT1 + (mouseX - 1) * CELLSIZE);

                    Canvas.SetTop(placingShipRect[1], MARGINTOP + (mouseY - (rotation == 0 ? 1 : 0)) * CELLSIZE);
                    Canvas.SetLeft(placingShipRect[1], MARGINLEFT1 + (mouseX - (rotation == 1 ? 1 : 0)) * CELLSIZE);

                    Canvas.SetTop(placingShipRect[2], MARGINTOP + (mouseY - (rotation == 0 ? 1 : -1)) * CELLSIZE);
                    Canvas.SetLeft(placingShipRect[2], MARGINLEFT1 + (mouseX - (rotation == 1 ? 1 : -1)) * CELLSIZE);

                    Canvas.SetTop(placingShipRect[3], MARGINTOP + (mouseY - (rotation == 0 ? 1 : -2)) * CELLSIZE);
                    Canvas.SetLeft(placingShipRect[3], MARGINLEFT1 + (mouseX - (rotation == 1 ? 1 : -2)) * CELLSIZE);
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
        }

        private void TimerTick(object sender, EventArgs e)
        {
            if (game_curr_turn == Turn.AI)
            {
                aiMakeAMove();
            }
        }

        private void checkIfGameEnded()
        {
            int whoWins = 0;
            if (currAiShips[0] == 0 && currAiShips[1] == 0 && currAiShips[2] == 0)
            {
                whoWins = 0;
            }
            else if (currPlayerShips[0] == 0 && currPlayerShips[1] == 0 && currPlayerShips[2] == 0)
            {
                whoWins = 1;
            }

            if ((currAiShips[0] == 0 && currAiShips[1] == 0 && currAiShips[2] == 0)
                || (currPlayerShips[0] == 0 && currPlayerShips[1] == 0 && currPlayerShips[2] == 0))
            {
                STATE = States.GAMEEND;
                gameData.WON = (whoWins == 0 ? playerTable.Name : aiTable.Name);
                EndOfMatch eom = new EndOfMatch();
                eom.P1Miss.Content = "Miss: " + p1_miss.Text;
                eom.P1Hit.Content = "Hit: " + p1_hit.Text;
                eom.P2Miss.Content = "Miss: " + ai_miss.Text;
                eom.P2Hit.Content = "Hit: " + ai_hit.Text;
                eom.winnerText.Content = (whoWins == 0 ? playerTable.Name : aiTable.Name) + " WINS!!";
                eom.winnerText.FontSize = (whoWins == 0 ? 90 : 64);
                eom.Label2.Content = "Ai name";
                eom.WINNER = "2";
                eom.Player1Box.Text = playerTable.Name;
                eom.Player2Box.Foreground = (whoWins == 0 ? Brushes.Red : Brushes.Green);
                eom.Player2Box.Text = aiTable.Name;
                eom.Player2Box.FontSize = 30;
                eom.Player1Box.Foreground = (whoWins == 0 ? Brushes.Green : Brushes.Red);
                eom.Show();

                FinalData finalData = new FinalData(gameData.P1_NAME, gameData.P2_NAME, gameData.P1_HIT, gameData.P2_HIT, gameData.P1_MISS, gameData.P2_MISS, gameData.TURN, gameData.WON);
                ScoreData scoreData = new ScoreData();
                scoreData.loadJsonFile();
                scoreData.gameDatas.Add(finalData);
                scoreData.writeJsonFile();

                this.Close();
            }
        }


        // AI
        void aiMakeAMove()
        {
            if (STATE == States.PLAYING)
            {
                aiPossibleShots = playerTable.aiGeneratePossibleMoves();
                aiAllShots = playerTable.aiAllDeadZones();

                refreshScores();


                //System.Threading.Thread.Sleep(1000);
                Random random = new Random();

                if (aiNextTip.Count() <= 0)
                {
                    int targetIndex = random.Next(0, aiPossibleShots.Count());
                    Coordinates targetCoord = aiPossibleShots[targetIndex];
                    aiPossibleShots.RemoveAt(targetIndex);
                    gameData.saveMove("Ai", currTurn, targetCoord);
                    Debug.WriteLine(gameData.Steps[gameData.Steps.Count - 1].ToString());


                    if (targetCoord.Value == 0)
                    {
                        aiCreateMarker(targetCoord.Y, targetCoord.X, false);
                        playerTable.makeAShot(targetCoord.X, targetCoord.Y);
                        game_curr_turn = Turn.PLAYER;
                        if (firstTurn == game_curr_turn) currTurn++;
                        gameData.P2_MISS++;
                    }
                    else
                    {
                        bool is_destroyed = playerTable.makeAShot(targetCoord.X, targetCoord.Y);
                        aiCreateMarker(targetCoord.Y, targetCoord.X, true);
                        aiNextTip = playerTable.aiGetNextTips(targetCoord.X, targetCoord.Y, aiPossibleShots);
                        aiFirstHit = targetCoord;
                        gameData.P2_HIT++;
                        if (is_destroyed)
                        {
                            playerTable.getShipByCoordinate(targetCoord.X, targetCoord.Y).shipBody.Visibility = Visibility.Visible;
                            playerTable.getShipByCoordinate(targetCoord.X, targetCoord.Y).shipBody.Fill = shipSprites[playerTable.getShipByCoordinate(targetCoord.X, targetCoord.Y).SpriteIndex];
                            currPlayerShips[playerTable.getShipByCoordinate(targetCoord.X, targetCoord.Y).Type - 2]--;
                            p1_frig.Text = currPlayerShips[2] + "/" + maxPlaceableShips[2];
                            p1_brig.Text = currPlayerShips[1] + "/" + maxPlaceableShips[1];
                            p1_gunboat.Text = currPlayerShips[0] + "/" + maxPlaceableShips[0];
                        }
                    }
                }
                else
                {
                    int targetIndex = random.Next(0, aiNextTip.Count());
                    Coordinates targetCoord = aiNextTip[targetIndex];
                    aiNextTip.RemoveAt(targetIndex);
                    gameData.saveMove("Ai", currTurn, targetCoord);
                    Debug.WriteLine(gameData.Steps[gameData.Steps.Count - 1].ToString());


                    if (targetCoord.Value == 0)
                    {
                        aiCreateMarker(targetCoord.Y, targetCoord.X, false);
                        playerTable.makeAShot(targetCoord.X, targetCoord.Y);
                        game_curr_turn = Turn.PLAYER;
                        if (firstTurn == game_curr_turn) currTurn++;
                        gameData.P2_MISS++;
                    }
                    else
                    {

                        bool is_destroyed = playerTable.makeAShot(targetCoord.X, targetCoord.Y);
                        aiCreateMarker(targetCoord.Y, targetCoord.X, true);
                        gameData.P2_HIT++;
                        if (is_destroyed)
                        {
                            playerTable.aiReplaceDestroyedShipSides();
                            foreach (Coordinates coordinates in aiNextTip)
                            {
                                if (coordinates.Value == 0)
                                {
                                    playerTable.getCoordinate(coordinates.X, coordinates.Y).Value = 9;
                                }
                            }
                            aiNextTip.Clear();

                            playerTable.getShipByCoordinate(targetCoord.X, targetCoord.Y).shipBody.Visibility = Visibility.Visible;
                            playerTable.getShipByCoordinate(targetCoord.X, targetCoord.Y).shipBody.Fill = shipSprites[playerTable.getShipByCoordinate(targetCoord.X, targetCoord.Y).SpriteIndex];
                            currPlayerShips[playerTable.getShipByCoordinate(targetCoord.X, targetCoord.Y).Type - 2]--;
                            p1_frig.Text = currPlayerShips[2] + "/" + maxPlaceableShips[2];
                            p1_brig.Text = currPlayerShips[1] + "/" + maxPlaceableShips[1];
                            p1_gunboat.Text = currPlayerShips[0] + "/" + maxPlaceableShips[0];

                        }
                        else
                        {
                            aiLastHit = targetCoord;
                            aiNextTip = playerTable.aiGetNextShipShot(aiFirstHit, aiLastHit, aiAllShots);
                        }
                    }
                }

                if (game_curr_turn == Turn.AI)
                {
                    _timer.Stop();
                    _timer.Start();
                    //aiMakeAMove();
                }

                refreshScores();
                checkIfGameEnded();
            }

        }

        void aiCreateMarker(int XX, int YY, bool scored)
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
            Canvas.SetLeft(markerRect, MARGINLEFT1 + (XX - 1) * CELLSIZE);
        }


        //Visszajátszás
        void drawTableShips(GameTable gT, int PlayerIndex)
        {
            foreach (Ships element in gT.ships)
            {
                element.shipBody.Fill = shipSprites[element.SpriteIndex];
                element.shipBody.Visibility = (PlayerIndex == 0 ? Visibility.Visible : Visibility.Hidden);

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
            aiTable.setTableToDefault();
            playerTable.setTableToDefault();
            drawTableShips(aiTable, 1);
            drawTableShips(playerTable, 0);
            TurnElement te = null;
            //ITT KELLENE LŐNI VALAHOGY MERT NEM MENT
            game_curr_turn = Turn.PLAYER;
            currTurn = (te == null ? 1 : te.Round);


        }



        void loadSpriteDatas()
        {
            Cursor = Cursors.None;

            //Sound Sprites
            SoundSprites.Add(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\hang_on.png", UriKind.Absolute))
            }); SoundSprites.Add(new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(Directory.GetCurrentDirectory() + "\\sources\\hang_off.png", UriKind.Absolute))
            });

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
