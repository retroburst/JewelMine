using JewelMine.Engine;
using JewelMine.Engine.Models;
using JewelMine.View.Forms.Audio;
using log4net;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JewelMine.View.Forms
{
    /// <summary>
    /// Windows forms based view
    /// for game.
    /// </summary>
    public partial class GameView : Form
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(GameView));
        private Size preferredWindowSize = Size.Empty;
        private bool disposing = false;
        private GameTimer timer = null;
        private GameLogic gameLogic = null;
        private Dictionary<JewelType, Bitmap> jewelImageResourceDictionary = null;
        private Dictionary<JewelType, Bitmap> jewelResizedImageResourceDictionary = null;
        private Bitmap[] backgroundImageArray = null;
        private Random rand = new Random();
        private Rectangle[,] cells = null;
        private int cellHeight = 0;
        private int cellWidth = 0;
        private long startTime = 0;
        private Pen deltaBorderPen = null;
        private Brush collisionOverlayBrush = null;
        private TextureBrush backgroundBrush = null;
        private Rectangle deltaBorder = Rectangle.Empty;
        private GameAudioSystem gameAudioSystem = null;
        private GameLogicInput logicInput = null;
        private GameInformationView gameInformationView = null;
        private Dictionary<Keys, Action> keyBindingDictionary = null;
        private List<string> messages = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameView" /> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public GameView(GameLogic engine)
        {
            InitializeComponent();
            messages = new List<string>();
            preferredWindowSize = new Size(ViewConstants.WINDOW_PREFERRED_WIDTH, ViewConstants.WINDOW_PREFERRED_HEIGHT);
            // save our game engine into a variable
            gameLogic = engine;
            // init game information
            gameInformationView = new GameInformationView(gameLogic);
            // init key bindings
            keyBindingDictionary = new Dictionary<Keys, Action>();
            InitialiseKeyBindings();
            // init game audio system
            gameAudioSystem = GameAudioSystem.Instance;
            // set paint styles
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);
            timer = new GameTimer();
            cells = new Rectangle[engine.State.Mine.Columns, engine.State.Mine.Depth];
            backgroundImageArray = ViewHelpers.GenerateBackgroundImageArray();
            // hook into interesting form events
            Load += LoadHandler;
            FormClosed += FormClosedHandler;
            FormClosing += FormClosingHandler;
            Layout += LayoutHandler;
            KeyDown += InputHandler;
            InitialiseDrawingObjects();
            logicInput = new GameLogicInput();
            // calculate cell dimensions
            CalculateGridCellDimensions();
            CalculateGridCells(cells);
            // generate the resized versions of jewels
            jewelImageResourceDictionary = ViewHelpers.GenerateJewelImageResourceDictionary();
            // generate and store resized images for this form difficultySize - we do 
            // this once here instead of resizing every time we
            // draw a delta - which is a very expensive operation
            jewelResizedImageResourceDictionary = ViewHelpers.GenerateResizedJewelImageResourceDictionary(jewelImageResourceDictionary, cellWidth, cellHeight);
            // restore user prefs
            RestoreUserPreferences();
            // start the background music loop
            gameAudioSystem.PlayBackgroundMusicLoop();
        }

        /// <summary>
        /// Initialises the key bindings.
        /// </summary>
        private void InitialiseKeyBindings()
        {
            ViewHelpers.PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingMoveLeft, Keys.Left, () => logicInput.DeltaMovement = MovementType.Left, keyBindingDictionary);
            ViewHelpers.PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingMoveRight, Keys.Right, () => logicInput.DeltaMovement = MovementType.Right, keyBindingDictionary);
            ViewHelpers.PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingMoveDown, Keys.Down, () => logicInput.DeltaMovement = MovementType.Down, keyBindingDictionary);
            ViewHelpers.PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingPauseGame, Keys.Control | Keys.P, () => logicInput.PauseGame = true, keyBindingDictionary);
            ViewHelpers.PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingRestartGame, Keys.Control | Keys.R, () => logicInput.RestartGame = true, keyBindingDictionary);
            ViewHelpers.PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingToggleDebugInfo, Keys.Control | Keys.D, () => gameInformationView.ToggleDebugInfo(), keyBindingDictionary);
            ViewHelpers.PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingQuitGame, Keys.Control | Keys.Q, () => this.Close(), keyBindingDictionary);
            ViewHelpers.PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingSwapDeltaJewels, Keys.Space, () => logicInput.DeltaSwapJewels = true, keyBindingDictionary);
            ViewHelpers.PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingToggleMusic, Keys.Control | Keys.M, () => ToggleBackgroundMusicLoop(), keyBindingDictionary);
            ViewHelpers.PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingToggleSoundEffects, Keys.Control | Keys.S, () => ToggleSoundEffects(), keyBindingDictionary);
            ViewHelpers.PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingDifficultyChange, Keys.Control | Keys.U, () => logicInput.ChangeDifficulty = true, keyBindingDictionary);
            ViewHelpers.PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingSaveGame, Keys.Control | Keys.Shift | Keys.S, () => logicInput.SaveGame = true, keyBindingDictionary);
            ViewHelpers.PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingLoadGame, Keys.Control | Keys.Shift | Keys.L, () => logicInput.LoadGame = true, keyBindingDictionary);
        }

        /// <summary>
        /// Handles the logicInput event of the GameView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        public void InputHandler(object sender, KeyEventArgs e)
        {
            if (gameLogic.State.PlayState == GamePlayState.Paused || gameLogic.State.PlayState == GamePlayState.NotStarted)
            {
                logicInput.GameStarted = true;
                return;
            }
            ProcessInput(e);
        }

        /// <summary>
        /// Processes the input.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void ProcessInput(KeyEventArgs e)
        {
            if (keyBindingDictionary.ContainsKey(e.KeyData))
            {
                Action bindingAction = keyBindingDictionary[e.KeyData];
                if (bindingAction != null) bindingAction();
            }
        }

        /// <summary>
        /// Calculates the grid cell dimensions.
        /// </summary>
        private void CalculateGridCellDimensions()
        {
            cellWidth = ClientRectangle.Width / cells.GetLength(0);
            cellHeight = ClientRectangle.Height / cells.GetLength(1);
        }

        /// <summary>
        /// Initialises the drawing objects.
        /// </summary>
        private void InitialiseDrawingObjects()
        {
            deltaBorderPen = new Pen(Color.GhostWhite);
            deltaBorderPen.Alignment = PenAlignment.Center;
            deltaBorderPen.DashStyle = DashStyle.Dot;
            deltaBorderPen.DashCap = DashCap.Round;
            deltaBorderPen.EndCap = LineCap.DiamondAnchor;
            deltaBorderPen.LineJoin = LineJoin.Bevel;
            deltaBorderPen.StartCap = LineCap.DiamondAnchor;
            deltaBorderPen.Width = 1.00f;
            collisionOverlayBrush = new SolidBrush(Color.FromArgb(65, Color.AntiqueWhite));
            backgroundBrush = new TextureBrush(backgroundImageArray[ViewHelpers.GenerateRandomIndex(backgroundImageArray, rand)], WrapMode.Tile);
        }

        /// <summary>
        /// Loads the handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void LoadHandler(object sender, EventArgs e)
        {
            RestoreWindowState();
        }

        /// <summary>
        /// Forms the closing handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FormClosingEventArgs"/> instance containing the event data.</param>
        private void FormClosingHandler(object sender, FormClosingEventArgs e)
        {
            SaveWindowState();
            SaveUserPreferences();
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Handles the FormClosed event of the GameView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FormClosedEventArgs"/> instance containing the event data.</param>
        private void FormClosedHandler(object sender, FormClosedEventArgs e)
        {
            disposing = true;
            Application.Exit();
        }

        /// <summary>
        /// The main game loop.
        /// </summary>
        public void GameLoop()
        {
            if (logger.IsDebugEnabled) logger.Debug("Starting game loop.");
            timer.Start();
            while (!disposing)
            {
                startTime = timer.ElapsedMilliseconds;
                Application.DoEvents();
                GameLogicUpdate logicUpdate = gameLogic.PerformGameLogic(logicInput);
                Invalidate();
                PlaySounds(logicUpdate);
                messages.AddRange(logicUpdate.Messages);
                if ((timer.ElapsedMilliseconds - startTime) < gameLogic.State.TickSpeedMilliseconds)
                {
                    // sleep the thread for the remaining time in this tick - saves burning CPU cycles
                    int sleepTime = (int)(gameLogic.State.TickSpeedMilliseconds - (timer.ElapsedMilliseconds - startTime));
                    Thread.Sleep(sleepTime);
                }
                // reset user logicInput descriptors
                logicInput.Clear();
            }
            timer.Stop();
            if (logger.IsDebugEnabled) logger.Debug("Exiting game loop.");
        }

        /// <summary>
        /// Plays the sounds.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        private void PlaySounds(GameLogicUpdate logicUpdate)
        {
            if (logicUpdate.FinalisedCollisions.Count > 0) gameAudioSystem.PlayCollision();
            if (logicUpdate.DeltaJewelsSwapped) gameAudioSystem.PlaySwap();
            if (logicUpdate.DeltaStationary) gameAudioSystem.PlayStationary();
            if (logicUpdate.LevelIncremented) gameAudioSystem.PlayLevelUp();
        }

        /// <summary>
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs" /> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            Draw(e.Graphics);
        }

        /// <summary>
        /// Draws the specified client rectangle.
        /// </summary>B
        /// <param name="graphics">The graphics.</param>
        private void Draw(Graphics graphics)
        {
            graphics.SmoothingMode = SmoothingMode.HighQuality;
            DrawBackground(graphics);
            DrawJewels(graphics);
            DrawDeltaBorder(graphics);
            DrawCollisions(graphics);
            DrawGameStateText(graphics);
        }

        /// <summary>
        /// Draws the game gameLogic text.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        private void DrawGameStateText(Graphics graphics)
        {
            gameInformationView.DrawGameInformation(
                graphics,
                ClientSize.Width,
                ClientSize.Height,
                Width,
                Height,
                gameAudioSystem.BackgroundMusicMuted,
                gameAudioSystem.SoundEffectsMuted,
                messages);
            messages.Clear();
        }

        /// <summary>
        /// Draws the collision animation.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void DrawCollisions(Graphics graphics)
        {
            List<Rectangle> collisionOverlayRectangles = new List<Rectangle>();
            foreach (MarkedCollisionGroup mcg in gameLogic.State.Mine.MarkedCollisions)
            {
                if (mcg.CollisionTickCount % 2 != 0)
                {
                    foreach (CollisionGroupMember m in mcg.Members)
                    {
                        collisionOverlayRectangles.Add(cells[m.Coordinates.X, m.Coordinates.Y]);
                    }
                }
            }
            if (collisionOverlayRectangles.Count > 0) graphics.FillRectangles(collisionOverlayBrush, collisionOverlayRectangles.ToArray());
        }

        /// <summary>
        /// Draws the delta border.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void DrawDeltaBorder(Graphics graphics)
        {
            JewelGroup delta = gameLogic.State.Mine.Delta;
            if (delta != null && delta.HasWholeGroupEnteredBounds)
            {
                Rectangle topLeft = cells[delta.Top.Coordinates.X, delta.Top.Coordinates.Y];
                deltaBorder = new Rectangle();
                deltaBorder.X = topLeft.X - 1;
                deltaBorder.Y = topLeft.Y - 1;
                deltaBorder.Height = (cellHeight * 3) + 2;
                deltaBorder.Width = cellWidth + 2;
                graphics.DrawRectangle(deltaBorderPen, deltaBorder);
            }
        }

        /// <summary>
        /// Draws the jewels.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        private void DrawJewels(Graphics graphics)
        {
            for (int i = 0; i < gameLogic.State.Mine.Columns; i++)
            {
                for (int j = 0; j < gameLogic.State.Mine.Depth; j++)
                {
                    Jewel jewel = (Jewel)gameLogic.State.Mine.Grid[i, j];
                    if (jewel != null)
                    {
                        Rectangle cell = cells[i, j];
                        Bitmap target = jewelResizedImageResourceDictionary[jewel.JewelType];
                        graphics.DrawImage(target, new Rectangle(cell.X, cell.Y, cell.Width, cell.Height), new Rectangle(0, 0, cell.Width, cell.Height), GraphicsUnit.Pixel);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Layout event of the GameView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LayoutEventArgs"/> instance containing the event data.</param>
        private void LayoutHandler(object sender, LayoutEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                logicInput.PauseGame = true;
                return;
            }
            // calculate new cell dimensions
            CalculateGridCellDimensions();
            CalculateGridCells(cells);
            // calculate new image sizes for this form difficultySize - we do 
            // this once here instead of resizing every time we
            // draw a delta - which is a very expensive operation
            jewelResizedImageResourceDictionary = ViewHelpers.GenerateResizedJewelImageResourceDictionary(jewelImageResourceDictionary, cellWidth, cellHeight);
            // invalidate the whole view so it is
            // all re-painted
            Invalidate();
            if (logger.IsDebugEnabled) logger.DebugFormat("Window resized to {0}x{1}.", Width, Height);
        }

        /// <summary>
        /// Draws the background.
        /// </summary>
        /// <param name="g">The g.</param>
        private void DrawBackground(Graphics g)
        {
            g.FillRectangle(backgroundBrush, 0, 0, ClientRectangle.Width, ClientRectangle.Height);
        }

        /// <summary>
        /// Calculates the grid cells.
        /// </summary>
        /// <param name="cells">The cells.</param>
        private void CalculateGridCells(Rectangle[,] cells)
        {
            int leftOverWidth = ClientRectangle.Width - (cellWidth * cells.GetLength(0));
            int leftOverHeight = ClientRectangle.Height - (cellHeight * cells.GetLength(1));
            int heightOffset = leftOverHeight / 2;
            int widthOffset = leftOverWidth / 2;
            // calculate and store grid rectangles
            for (int i = 0; i <= cells.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= cells.GetUpperBound(1); j++)
                {
                    cells[i, j] = new Rectangle(i * cellWidth + widthOffset, j * cellHeight + heightOffset, cellWidth, cellHeight);
                }
            }
        }

        /// <summary>
        /// Saves the window state.
        /// </summary>
        private void SaveWindowState()
        {
            if (logger.IsDebugEnabled) logger.Debug("Saving the window state.");
            if (WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.WindowLocation = Location;
                Properties.Settings.Default.WindowSize = Size;
            }
            else
            {
                Properties.Settings.Default.WindowLocation = RestoreBounds.Location;
                Properties.Settings.Default.WindowSize = RestoreBounds.Size;
            }
            Properties.Settings.Default.WindowState = WindowState;
        }

        /// <summary>
        /// Restores the state of the window.
        /// </summary>
        private void RestoreWindowState()
        {
            if (logger.IsDebugEnabled) logger.Debug("Restoring window state.");
            if (Properties.Settings.Default.WindowSize.IsEmpty)
            {
                FitPreferredSizeToScreen();
                return; // state has never been saved
            }
            StartPosition = FormStartPosition.Manual;
            Location = Properties.Settings.Default.WindowLocation;
            Size = Properties.Settings.Default.WindowSize;
            WindowState = Properties.Settings.Default.WindowState == FormWindowState.Minimized
                ? FormWindowState.Normal : Properties.Settings.Default.WindowState;
        }

        /// <summary>
        /// Fits the preferred size to screen.
        /// If the screen is too small, shrinks the
        /// form but tries to maintain aspect ratio.
        /// </summary>
        private void FitPreferredSizeToScreen()
        {
            if (logger.IsDebugEnabled) logger.Debug("Fitting preferred size window to screen.");
            Screen screen = Screen.FromControl(this);
            if (screen.WorkingArea.Height < preferredWindowSize.Height)
            {
                // shrink for but try and maintain aspect ratio
                int heightDifference = preferredWindowSize.Height - screen.WorkingArea.Height;
                int customHeight = preferredWindowSize.Height - heightDifference;
                int customWidth = preferredWindowSize.Width - heightDifference;
                if (screen.WorkingArea.Width < customWidth)
                {
                    int widthDifference = customWidth - screen.WorkingArea.Width;
                    customWidth -= widthDifference;
                    customHeight -= widthDifference;
                }
                Size = new Size(customWidth, customHeight);
            }
            else
            {
                Size = preferredWindowSize;
            }
            CenterToScreen();
        }

        /// <summary>
        /// Saves the user preferences.
        /// </summary>
        private void SaveUserPreferences()
        {
            Properties.Settings.Default.UserPreferenceMusicMuted = gameAudioSystem.BackgroundMusicMuted;
            Properties.Settings.Default.UserPreferenceSoundEffectsMuted = gameAudioSystem.SoundEffectsMuted;
            Properties.Settings.Default.UserPreferenceDifficulty = gameLogic.State.Difficulty.DifficultyLevel;
        }

        /// <summary>
        /// Restores the user preferences.
        /// </summary>
        private void RestoreUserPreferences()
        {
            gameAudioSystem.BackgroundMusicMuted = Properties.Settings.Default.UserPreferenceMusicMuted;
            AddBackgroundMusicStateMessage();
            gameAudioSystem.SoundEffectsMuted = Properties.Settings.Default.UserPreferenceSoundEffectsMuted;
            AddSoundEffectsStateMessage();
        }

        /// <summary>
        /// Adds the background music state message.
        /// </summary>
        private void AddBackgroundMusicStateMessage()
        {
            string message = string.Format(ViewConstants.TOGGLE_MUSIC_PATTERN, ViewHelpers.EncodeBooleanForDisplay(!gameAudioSystem.BackgroundMusicMuted));
            messages.Add(message);
        }

        /// <summary>
        /// Toggles the background music loop.
        /// </summary>
        private void ToggleBackgroundMusicLoop()
        {
            gameAudioSystem.ToggleBackgroundMusicLoop();
            AddBackgroundMusicStateMessage();
        }

        /// <summary>
        /// Adds the sound effects state message.
        /// </summary>
        private void AddSoundEffectsStateMessage()
        {
            string message = string.Format(ViewConstants.TOGGLE_SOUND_PATTERN, ViewHelpers.EncodeBooleanForDisplay(!gameAudioSystem.SoundEffectsMuted));
            messages.Add(message);
        }

        /// <summary>
        /// Toggles the sound effects.
        /// </summary>
        private void ToggleSoundEffects()
        {
            gameAudioSystem.ToggleSoundEffects();
            AddSoundEffectsStateMessage();
        }



    }
}
