using JewelMine.Engine;
using JewelMine.Engine.Models;
using JewelMine.View.Forms.Audio;
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
        private GameTimer timer = null;
        private GameLogic gameEngine = null;
        private MovementType? inputMovement = null;
        private bool inputSwapDeltaJewels = false;
        private Dictionary<JewelType, Bitmap> jewelImageResourceDictionary = null;
        private Dictionary<JewelType, Bitmap> jewelResizedImageResourceDictionary = null;
        private Bitmap[] backgroundImageArray = null;
        private Dictionary<int, TextureBrush> levelBackgroundDictionary = new Dictionary<int, TextureBrush>();
        private Random rand = new Random();
        private Rectangle[,] cells = null;
        private int cellHeight = 0;
        private int cellWidth = 0;
        private int jewelBitmapOffset = 2;
        private long startTime = 0;
        private Pen gridPen = null;
        private Pen deltaBorderPen = null;
        private Brush collisionOverlayBrush = null;
        private Brush informationOverlayBrush = null;
        private Rectangle deltaBorder = Rectangle.Empty;
        private GameAudioSystem gameAudioSystem = null;
        private Font scoreFont = null;
        private Rectangle scoreRectangle = Rectangle.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameView" /> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public GameView(GameLogic engine)
        {
            InitializeComponent();
            // save our game engine into a variable
            gameEngine = engine;
            // set paint styles
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);
            timer = new GameTimer();
            cells = new Rectangle[engine.GameStateModel.Mine.Columns, engine.GameStateModel.Mine.Depth];
            // hook into interesting form events
            FormClosed += FormClosedHandler;
            Layout += LayoutHandler;
            KeyDown += InputHandler;
            InitialiseDrawingObjects();
            backgroundImageArray = ViewHelpers.GenerateBackgroundImageArray();
            // calculate cell dimensions
            CalculateGridCellDimensions();
            // generate the resized versions of jewels
            jewelImageResourceDictionary = ViewHelpers.GenerateJewelImageResourceDictionary();
            // generate and store resized images for this form size - we do 
            // this once here instead of resizing every time we
            // draw a delta - which is a very expensive operation
            jewelResizedImageResourceDictionary = ViewHelpers.GenerateResizedJewelImageResourceDictionary(jewelImageResourceDictionary, cellWidth, cellHeight, jewelBitmapOffset);
            gameAudioSystem = GameAudioSystem.Instance;
            // signal game start
            gameEngine.StartGame();
            gameAudioSystem.PlayBackgroundMusicLoop();
        }

        /// <summary>
        /// Handles the input event of the GameView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        public void InputHandler(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.A:
                    inputMovement = MovementType.Left;
                    break;
                case Keys.Right:
                case Keys.D:
                    inputMovement = MovementType.Right;
                    break;
                case Keys.Down:
                case Keys.S:
                    inputMovement = MovementType.Down;
                    break;
                case Keys.Space:
                case Keys.C:
                    inputSwapDeltaJewels = true;
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
            gridPen = new Pen(Color.SlateGray);
            gridPen.Alignment = PenAlignment.Center;
            gridPen.DashStyle = DashStyle.Dot;
            gridPen.DashCap = DashCap.Round;
            gridPen.EndCap = LineCap.DiamondAnchor;
            gridPen.LineJoin = LineJoin.Round;
            gridPen.StartCap = LineCap.DiamondAnchor;

            deltaBorderPen = new Pen(Color.GhostWhite);
            deltaBorderPen.Alignment = PenAlignment.Center;
            deltaBorderPen.DashStyle = DashStyle.Dot;
            deltaBorderPen.DashCap = DashCap.Round;
            deltaBorderPen.EndCap = LineCap.DiamondAnchor;
            deltaBorderPen.LineJoin = LineJoin.Bevel;
            deltaBorderPen.StartCap = LineCap.DiamondAnchor;
            deltaBorderPen.Width = 1.00f;

            collisionOverlayBrush = new SolidBrush(Color.FromArgb(65, Color.AntiqueWhite));

            informationOverlayBrush = new SolidBrush(Color.FromArgb(80, Color.White));
            scoreFont = new Font(SystemInformation.MenuFont.FontFamily, 12.5f);
        }

        /// <summary>
        /// Handles the FormClosed event of the GameView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FormClosedEventArgs"/> instance containing the event data.</param>
        private void FormClosedHandler(object sender, FormClosedEventArgs e)
        {
            gameEngine.StopGame();
            Application.Exit();
        }

        /// <summary>
        /// The main game loop.
        /// </summary>
        public void GameLoop()
        {
            Console.WriteLine("Starting game loop..");
            timer.Start();
            while (gameEngine.GameStateModel.PlayState == GamePlayState.Playing)
            {
                //Console.WriteLine("Looping");
                startTime = timer.ElapsedMilliseconds;
                Application.DoEvents();
                GameLogicUpdate logicUpdate = gameEngine.PerformGameLogic(new GameLogicInput() { DeltaMovement = inputMovement, DeltaSwapJewels = inputSwapDeltaJewels });
                Invalidate(logicUpdate);
                PlaySounds(logicUpdate);
                while ((timer.ElapsedMilliseconds - startTime) < gameEngine.GameStateModel.TickSpeedMilliseconds)
                {
                    //Console.WriteLine("Waiting..");
                }
                // reset input descriptors
                inputMovement = null;
                inputSwapDeltaJewels = false;
            }
            timer.Stop();
            Console.WriteLine("Exited game loop..");
        }

        /// <summary>
        /// Plays the sounds.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        private void PlaySounds(GameLogicUpdate logicUpdate)
        {
            if(logicUpdate.FinalisedCollisions.Count > 0)
            {
                gameAudioSystem.PlayCollision();
            }
            if(logicUpdate.DeltaJewelsSwapped)
            {
                gameAudioSystem.PlaySwap();
            }
        }

        /// <summary>
        /// Invalidates the view based on the specified logic update.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        private void Invalidate(GameLogicUpdate logicUpdate)
        {
            if (logicUpdate != null)
            {
                if (logicUpdate.LevelIncremented)
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
            if(scoreRectangle != Rectangle.Empty)
            {
                Invalidate(scoreRectangle);
            }
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
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            DrawBackground(graphics);
            DrawGrid(graphics, cells);
            DrawJewels(graphics);
            DrawDeltaBorder(graphics);
            DrawCollisions(graphics);
            DrawGameStateText(graphics);
            //DrawObjects<Wall>(g, walls, squares);
            //DrawObjects<Block>(g, blocks, squares);
            //DrawObjects<IStructure>(g, structures, squares);
            //DrawControlStrings(g, cstrings);
        }

        /// <summary>
        /// Draws the game state text.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        private void DrawGameStateText(Graphics graphics)
        {
            string score = string.Format("Level//{0} Score//{1} LevelUp//{2} Speed//{3}", gameEngine.GameStateModel.Level, gameEngine.GameStateModel.Score, gameEngine.GameStateModel.Level * GameConstants.GAME_LEVEL_INCREMENT_SCORE_THRESHOLD, gameEngine.GameStateModel.TickSpeedMilliseconds);
            SizeF size = graphics.MeasureString(score, scoreFont);
            scoreRectangle = new Rectangle(5, 5, (int)size.Width, (int)size.Height);
            graphics.DrawString(score, scoreFont, informationOverlayBrush, new PointF(5, 5));
        }

        /// <summary>
        /// Draws the collision animation.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void DrawCollisions(Graphics graphics)
        {
            GameState state = gameEngine.GameStateModel;
            foreach (MarkedCollisionGroup mcg in state.Mine.MarkedCollisions)
            {
                if (mcg.CollisionTickCount % 2 != 0)
                {
                    List<Rectangle> regions = CalculateInvalidationRegions(mcg);
                    graphics.FillRectangles(collisionOverlayBrush, regions.ToArray());
                }
            }
        }

        /// <summary>
        /// Draws the delta border.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void DrawDeltaBorder(Graphics graphics)
        {
            JewelGroup delta = gameEngine.GameStateModel.Mine.Delta;
            if (delta != null && delta.HasWholeGroupEnteredBounds)
            {
                Rectangle topLeft = cells[delta.Top.Coordinates.X, delta.Top.Coordinates.Y];
                deltaBorder = new Rectangle();
                deltaBorder.X = topLeft.X + 2;
                deltaBorder.Y = topLeft.Y + 2;
                deltaBorder.Height = (cellHeight * 3) - 4;
                deltaBorder.Width = cellWidth - 4;

                graphics.DrawRectangle(deltaBorderPen, deltaBorder);
            }
        }

        /// <summary>
        /// Draws the jewels.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        private void DrawJewels(Graphics graphics)
        {
            for (int i = 0; i < gameEngine.GameStateModel.Mine.Columns; i++)
            {
                for (int j = 0; j < gameEngine.GameStateModel.Mine.Depth; j++)
                {
                    Jewel jewel = (Jewel)gameEngine.GameStateModel.Mine.Grid[i, j];
                    if (jewel != null)
                    {
                        Rectangle cell = cells[i, j];
                        Bitmap target = jewelResizedImageResourceDictionary[jewel.JewelType];
                        graphics.DrawImage(target, new Rectangle(cell.X + jewelBitmapOffset, cell.Y + jewelBitmapOffset, cell.Width - jewelBitmapOffset, cell.Height - jewelBitmapOffset), new Rectangle(0, 0, cell.Width, cell.Height), GraphicsUnit.Pixel);
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
            // calculate new image sizes for this form size - we do 
            // this once here instead of resizing every time we
            // draw a delta - which is a very expensive operation
            jewelResizedImageResourceDictionary = ViewHelpers.GenerateResizedJewelImageResourceDictionary(jewelImageResourceDictionary, cellWidth, cellHeight, jewelBitmapOffset);
            // invalidate the whole view so it is
            // all re-painted
            Invalidate(ClientRectangle);

            this.Text = string.Format("Jewel Mine [{0}x{1}]", this.Width.ToString(), this.Height.ToString());
        }

        /// <summary>
        /// Draws the background.
        /// </summary>
        /// <param name="g">The g.</param>
        private void DrawBackground(Graphics g)
        {
            // assigns a random background to each level
            TextureBrush brush = null;
            if (levelBackgroundDictionary.ContainsKey(gameEngine.GameStateModel.Level))
            {
                brush = levelBackgroundDictionary[gameEngine.GameStateModel.Level];
            }
            else
            {
                int randomIndex = ViewHelpers.GenerateRandomIndex(backgroundImageArray, rand);
                brush = new TextureBrush(backgroundImageArray[randomIndex], WrapMode.Tile);
                levelBackgroundDictionary.Add(gameEngine.GameStateModel.Level, brush);
            }
            g.FillRectangle(brush, 0, 0, ClientRectangle.Width, ClientRectangle.Height);
        }

        /// <summary>
        /// Draws the grid.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="cells">The cells.</param>
        private void DrawGrid(Graphics g, Rectangle[,] cells)
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
                    g.DrawRectangle(gridPen, cells[i, j]);
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
                Rectangle region;
                region = cells[collision.Coordinates.X, collision.Coordinates.Y];
                region.Width = cellWidth;
                region.Height = cellHeight;
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
            region = cells[coordinates.X, coordinates.Y];
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
            if (!gameEngine.GameStateModel.Mine.CoordinatesInBounds(originalCoordinates)) return CalculateInvalidationRegion(jewel, newCoordinates);
            Rectangle region = new Rectangle();
            int minX = Math.Min(originalCoordinates.X, newCoordinates.X);
            int minY = Math.Min(originalCoordinates.Y, newCoordinates.Y);
            Rectangle targetCell = cells[minX, minY];
            region.X = targetCell.X;
            region.Y = targetCell.Y;
            // movement down
            if (newCoordinates.Y > originalCoordinates.Y)
            {
                int difference = newCoordinates.Y - originalCoordinates.Y;
                region.Height = cellHeight * (difference + 1);
                region.Width = cellWidth;
            }
            // movement right or left
            else
            {
                int difference = Math.Max(originalCoordinates.X, newCoordinates.X) - Math.Min(originalCoordinates.X, newCoordinates.X);
                region.Height = cellHeight;
                region.Width = cellWidth * (difference + 1);
            }
            return (region);
        }

        /// <summary>
        /// Draws the objects.
        /// </summary>
        //private void DrawObjects<T>(Graphics graphics, T[,] objects, Rectangle[,] squares)
        //    where T : IDrawable
        //{
        //    for (int coordinates = 0; coordinates <= squares.GetUpperBound(0); coordinates++)
        //    {
        //        for (int y = 0; y <= squares.GetUpperBound(1); y++)
        //        {
        //            T obj = objects[coordinates, y];
        //            if (obj != null)
        //                obj.Draw(squares[coordinates, y], this, graphics);
        //        }
        //    }
        //}

    }
}
