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
        private IGameStateProvider gameStateProvider = null;
        private Dictionary<JewelType, Bitmap> jewelImageResourceDictionary = null;
        private Dictionary<JewelType, Bitmap> jewelResizedImageResourceDictionary = null;
        private Bitmap[] backgroundImageArray = null;
        private Random rand = new Random();
        private Rectangle[,] cells = null;
        private int cellHeight = 0;
        private int cellWidth = 0;
        private Pen deltaBorderPen = null;
        private Brush collisionOverlayBrush = null;
        private TextureBrush backgroundBrush = null;
        private Rectangle deltaBorder = Rectangle.Empty;
        private GameInformationView gameInformationView = null;
        private List<string> messages = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameView" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public GameView(IGameStateProvider provider)
        {
            InitializeComponent();
            messages = new List<string>();
            // save our game state provider into a variable
            gameStateProvider = provider;
            // init game information
            gameInformationView = new GameInformationView(provider);
            // set paint styles
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);
            cells = new Rectangle[provider.State.Mine.Columns, provider.State.Mine.Depth];
            backgroundImageArray = ViewHelpers.GenerateBackgroundImageArray();
            InitialiseDrawingObjects();
            // calculate cell dimensions
            CalculateGridCellDimensions();
            CalculateGridCells(cells);
            // generate the resized versions of jewels
            jewelImageResourceDictionary = ViewHelpers.GenerateJewelImageResourceDictionary();
            // generate and store resized images for this form difficultySize - we do 
            // this once here instead of resizing every time we
            // draw a delta - which is a very expensive operation
            jewelResizedImageResourceDictionary = ViewHelpers.GenerateResizedJewelImageResourceDictionary(jewelImageResourceDictionary, cellWidth, cellHeight);
        }

        /// <summary>
        /// Re initialise the view based on new game state.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public void ReInitialise(IGameStateProvider provider)
        {
            gameInformationView = new GameInformationView(provider);
            cells = new Rectangle[provider.State.Mine.Columns, provider.State.Mine.Depth];
            // calculate cell dimensions
            CalculateGridCellDimensions();
            CalculateGridCells(cells);
            // generate and store resized images for this form difficultySize - we do 
            // this once here instead of resizing every time we
            // draw a delta - which is a very expensive operation
            jewelResizedImageResourceDictionary = ViewHelpers.GenerateResizedJewelImageResourceDictionary(jewelImageResourceDictionary, cellWidth, cellHeight);
        }

        /// <summary>
        /// Toggles the debug information.
        /// </summary>
        public void ToggleDebugInfo()
        {
            gameInformationView.ToggleDebugInfo();
        }

        /// <summary>
        /// Adds a game information message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void AddGameInformationMessage(string message)
        {
            if(!string.IsNullOrEmpty(message)) messages.Add(message);
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
        /// Updates the view.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        public void UpdateView(GameLogicUpdate logicUpdate)
        {
            Invalidate();
            messages.AddRange(logicUpdate.Messages);
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
        /// Draws the game gameStateProvider text.
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
                GameAudioSystem.Instance.BackgroundMusicMuted,
                GameAudioSystem.Instance.SoundEffectsMuted,
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
            foreach (MarkedCollisionGroup mcg in gameStateProvider.State.Mine.MarkedCollisions)
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
            JewelGroup delta = gameStateProvider.State.Mine.Delta;
            if (delta != null && delta.HasWholeGroupEnteredBounds)
            {
                Rectangle topLeft = cells[delta.Top.Coordinates.X, delta.Top.Coordinates.Y];
                deltaBorder = new Rectangle();
                deltaBorder.X = topLeft.X - 2;
                deltaBorder.Y = topLeft.Y - 2;
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
            for (int i = 0; i < gameStateProvider.State.Mine.Columns; i++)
            {
                for (int j = 0; j < gameStateProvider.State.Mine.Depth; j++)
                {
                    Jewel jewel = (Jewel)gameStateProvider.State.Mine.Grid[i, j];
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
        /// Updates the layout.
        /// </summary>
        public void UpdateLayout()
        {
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
        /// Centera the view window to the screen.
        /// </summary>
        public void CenterViewWindow()
        {
            CenterToScreen();
        }

    }
}
