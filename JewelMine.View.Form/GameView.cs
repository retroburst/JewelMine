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

        /// <summary>
        /// Initializes a new instance of the <see cref="GameView" /> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public GameView(GameLogic engine)
        {
            InitializeComponent();
            preferredWindowSize = new Size(ViewConstants.WINDOW_PREFERRED_WIDTH, ViewConstants.WINDOW_PREFERRED_HEIGHT);
            // save our game engine into a variable
            gameLogic = engine;
            // init game information
            gameInformationView = new GameInformationView(gameLogic);
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
            // generate and store resized images for this form scoreSize - we do 
            // this once here instead of resizing every time we
            // draw a delta - which is a very expensive operation
            jewelResizedImageResourceDictionary = ViewHelpers.GenerateResizedJewelImageResourceDictionary(jewelImageResourceDictionary, cellWidth, cellHeight);
            gameAudioSystem = GameAudioSystem.Instance;
            gameAudioSystem.PlayBackgroundMusicLoop();
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
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.A:
                    logicInput.DeltaMovement = MovementType.Left;
                    break;
                case Keys.Right:
                case Keys.D:
                    logicInput.DeltaMovement = MovementType.Right;
                    break;
                case Keys.Down:
                case Keys.S:
                    logicInput.DeltaMovement = MovementType.Down;
                    break;
                case Keys.Space:
                case Keys.C:
                    logicInput.DeltaSwapJewels = true;
                    break;
                case Keys.R:
                    if (e.Control) logicInput.RestartGame = true;
                    break;
                case Keys.P:
                    if (e.Control) logicInput.PauseGame = true;
                    break;
                case Keys.Q:
                    if (e.Control) this.Close();
                    break;
                case Keys.M:
                    if (e.Control) gameAudioSystem.ToggleBackgroundMusicLoop();
                    break;
                case Keys.N:
                    if (e.Control) gameAudioSystem.ToggleSoundEffects();
                    break;
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
            if(logger.IsDebugEnabled) logger.Debug("Starting game loop");
            timer.Start();
            while (!disposing)
            {
                startTime = timer.ElapsedMilliseconds;
                Application.DoEvents();
                GameLogicUpdate logicUpdate = gameLogic.PerformGameLogic(logicInput);
                Invalidate(logicUpdate);
                PlaySounds(logicUpdate);
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
            if(logger.IsDebugEnabled) logger.Debug("Exiting game loop");
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
        /// Invalidates the view based on the specified logic update.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        private void Invalidate(GameLogicUpdate logicUpdate)
        {
            if (logicUpdate != null)
            {
                if (logicUpdate.GameStarted)
                {
                    Invalidate();
                    return;
                }
                logicUpdate.JewelMovements.ForEach(jm => Invalidate(CalculateInvalidationRegion(jm.Jewel, jm.Original, jm.New)));
                logicUpdate.Collisions.ForEach(c => CalculateInvalidationRegions(c).ForEach(r => Invalidate(r)));
                logicUpdate.InvalidCollisions.ForEach(ic => CalculateInvalidationRegions(ic).ForEach(r => Invalidate(r)));
                logicUpdate.FinalisedCollisions.ForEach(fc => CalculateInvalidationRegions(fc).ForEach(r => Invalidate(r)));
            }
            if (deltaBorder != Rectangle.Empty)
            {
                Invalidate(new Rectangle(deltaBorder.X - 2, deltaBorder.Y - 2, deltaBorder.Width + 4, deltaBorder.Height + 4));
                deltaBorder = Rectangle.Empty;
            }
            gameInformationView.Invalidate(Invalidate);
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
        /// </summary>
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
            gameInformationView.DrawGameInformation(graphics, ClientSize.Width, ClientSize.Height);
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
                    foreach(CollisionGroupMember m in mcg.Members)
                    {
                        collisionOverlayRectangles.Add(cells[m.Coordinates.X, m.Coordinates.Y]);
                    }
                }
            }
            if(collisionOverlayRectangles.Count > 0) graphics.FillRectangles(collisionOverlayBrush, collisionOverlayRectangles.ToArray());
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
                        //graphics.DrawRectangle(new Pen(Color.Red), new Rectangle(cell.X + jewelBitmapOffset, cell.Y + jewelBitmapOffset, cell.Width - jewelBitmapOffset, cell.Height - jewelBitmapOffset));
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
            // calculate new cell dimensions
            CalculateGridCellDimensions();
            CalculateGridCells(cells);
            // calculate new image sizes for this form scoreSize - we do 
            // this once here instead of resizing every time we
            // draw a delta - which is a very expensive operation
            jewelResizedImageResourceDictionary = ViewHelpers.GenerateResizedJewelImageResourceDictionary(jewelImageResourceDictionary, cellWidth, cellHeight);
            // invalidate the whole view so it is
            // all re-painted
            Invalidate(ClientRectangle);
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
            // draw the grid onto the control
            for (int i = 0; i <= cells.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= cells.GetUpperBound(1); j++)
                {
                    cells[i, j] = new Rectangle(i * cellWidth + widthOffset, j * cellHeight + heightOffset, cellWidth, cellHeight);
                }
            }
        }

        /// <summary>
        /// Calculates the invalidation region.
        /// </summary>
        /// <param name="cg">The cg.</param>
        /// <returns></returns>
        private List<Rectangle> CalculateInvalidationRegions(CollisionGroup cg)
        {
            List<Rectangle> regions = new List<Rectangle>();
            foreach (CollisionGroupMember collision in cg.Members)
            {
                Rectangle cell = cells[collision.Coordinates.X, collision.Coordinates.Y];
                Rectangle region = new Rectangle(cell.X - 2, cell.Y - 2, cellWidth + 4, cellHeight + 4);
                regions.Add(region);
            }
            return (regions);
        }

        /// <summary>
        /// Calculates the invalidation region.
        /// </summary>
        /// <param name="jewel">The jewel.</param>
        /// <param name="coordinates">The coordinates.</param>
        /// <returns></returns>
        public Rectangle CalculateInvalidationRegion(Jewel jewelModel, Coordinates coordinates)
        {
            Rectangle region = new Rectangle();
            region.X = cells[coordinates.X, coordinates.Y].X;
            region.Y = cells[coordinates.X, coordinates.Y].Y;
            region.Width = cellWidth;
            region.Height = cellHeight;
            return (region);
        }

        /// <summary>
        /// Calculates the invalidation region.
        /// </summary>
        /// <param name="jewel">The jewel.</param>
        /// <param name="originalCoordinates">The original coordinates.</param>
        /// <param name="newCoordinates">The new coordinates.</param>
        /// <returns></returns>
        public Rectangle CalculateInvalidationRegion(Jewel jewel, Coordinates originalCoordinates, Coordinates newCoordinates)
        {
            if (!gameLogic.State.Mine.CoordinatesInBounds(originalCoordinates)) return CalculateInvalidationRegion(jewel, newCoordinates);
            Rectangle region = new Rectangle();
            int minX = Math.Min(originalCoordinates.X, newCoordinates.X);
            int minY = Math.Min(originalCoordinates.Y, newCoordinates.Y);
            Rectangle targetCell = cells[minX, minY];
            // add an little extra to the margin for the delta border
            region.X = targetCell.X - 2;
            region.Y = targetCell.Y - 2;
            int heightMargin = cellHeight + 4;
            int widthMargin = cellWidth + 4;
            // movement down
            if (newCoordinates.Y > originalCoordinates.Y)
            {
                int difference = newCoordinates.Y - originalCoordinates.Y;
                region.Height = heightMargin * (difference + 1);
                region.Width = widthMargin;
            }
            // movement right or left
            else
            {
                int difference = Math.Max(originalCoordinates.X, newCoordinates.X) - Math.Min(originalCoordinates.X, newCoordinates.X);
                region.Height = heightMargin;
                region.Width = widthMargin * (difference + 1);
            }
            return (region);
        }

        /// <summary>
        /// Saves the window state.
        /// </summary>
        private void SaveWindowState()
        {
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
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Restores the state of the window.
        /// </summary>
        private void RestoreWindowState()
        {
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
            Screen screen = Screen.FromControl(this);
            if (screen.WorkingArea.Height < preferredWindowSize.Height)
            {
                // shrink for but try and maintain aspect ratio
                int heightDifference = preferredWindowSize.Height - screen.WorkingArea.Height;
                int customHeight = preferredWindowSize.Height - heightDifference;
                int customWidth = preferredWindowSize.Width - heightDifference;
                if(screen.WorkingArea.Width < customWidth)
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

    }
}
