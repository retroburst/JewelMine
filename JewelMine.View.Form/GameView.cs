using JewelMine.Engine;
using JewelMine.Engine.Models;
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
        public Dictionary<JewelType, Bitmap> jewelImageResourceDictionary = null;
        private Dictionary<JewelType, Bitmap> jewelResizedImageResourceDictionary = null;
        private Bitmap[] backgroundImageArray = Helpers.GenerateBackgroundImageArray();
        private Dictionary<int, TextureBrush> levelBackgroundDictionary = new Dictionary<int, TextureBrush>();
        private Random rand = new Random();
        private Rectangle[,] cells = null;
        private int cellHeight = 0;
        private int cellWidth = 0;
        private int bitmapOffset = 2;
        private long startTime = 0;
        Pen pen = null;


        /// <summary>
        /// Initializes a new instance of the <see cref="GameView" /> class.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public GameView(GameLogic engine)
        {
            InitializeComponent();
            gameEngine = engine;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer, true);
            timer = new GameTimer();
            cells = new Rectangle[engine.GameStateModel.MineModel.Columns, engine.GameStateModel.MineModel.Depth];
            FormClosed += GameView_FormClosed;
            Layout += GameView_Layout;
            gameEngine.StartGame(); if (pen == null)
                // TODO: put in own init meth
                pen = new Pen(Color.SlateGray);
            pen.Alignment = PenAlignment.Center;
            pen.DashStyle = DashStyle.Dot;
            pen.DashCap = DashCap.Round;
            pen.EndCap = LineCap.DiamondAnchor;
            pen.LineJoin = LineJoin.Round;
            pen.StartCap = LineCap.DiamondAnchor;

            // calculate cell dimensions
            cellWidth = ClientRectangle.Width / cells.GetLength(0);
            cellHeight = ClientRectangle.Height / cells.GetLength(1);
            // generate the resized versions of jewels
            jewelImageResourceDictionary = Helpers.GenerateJewelImageResourceDictionary();
            jewelResizedImageResourceDictionary = Helpers.GenerateResizedJewelImageResourceDictionary(jewelImageResourceDictionary, cellWidth, cellHeight, bitmapOffset);
        }

        /// <summary>
        /// Handles the FormClosed event of the GameView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="FormClosedEventArgs"/> instance containing the event data.</param>
        private void GameView_FormClosed(object sender, FormClosedEventArgs e)
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
            while (gameEngine.GameStateModel.GamePlayState == GamePlayState.Playing)
            {
                //Console.WriteLine("Looping");
                startTime = timer.ElapsedMilliseconds;
                // TODO get the movement information to send to view for invalidation regions
                GameLogicUpdate logicUpdate = gameEngine.PerformGameLogic();
                Invalidate(logicUpdate);
                Application.DoEvents();
                while ((timer.ElapsedMilliseconds - startTime) < gameEngine.GameStateModel.GameTickSpeedMilliseconds)
                {
                    //Console.WriteLine("Waiting..");
                }
            }
            timer.Stop();
            Console.WriteLine("Exited game loop..");
        }

        /// <summary>
        /// Invalidates the view based on the specified logic update.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        private void Invalidate(GameLogicUpdate logicUpdate)
        {
            if (logicUpdate != null)
            {
                if (logicUpdate.JewelMovements.Count == 0 && logicUpdate.NewJewels.Count == 0)
                {
                    Invalidate();
                }
                else
                {
                    foreach (NewJewel nj in logicUpdate.NewJewels)
                    {
                        Invalidate(CalculateInvalidationRegion(nj.Jewel, nj.X, nj.Y));
                    }
                    foreach (JewelMovement jm in logicUpdate.JewelMovements)
                    {
                        Invalidate(CalculateInvalidationRegion(jm.Jewel, jm.OriginalX, jm.OriginalY, jm.NewX, jm.NewY));
                    }
                }
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
            //DrawObjects<Wall>(g, walls, squares);
            //DrawObjects<Block>(g, blocks, squares);
            //DrawObjects<IStructure>(g, structures, squares);
            //DrawControlStrings(g, cstrings);

            //Random r = new Random();
            //string[] jewelNames = Enum.GetNames(typeof(JewelType)).Where(x => x != JewelType.Unknown.ToString()).ToArray();
            //foreach (var cell in cells)
            //{
            //    int randomIndex = r.Next(0, jewelNames.Length);
            //    string jewelName = jewelNames[randomIndex];
            //    JewelType type = (JewelType)Enum.Parse(typeof(JewelType), jewelName);
            //    Bitmap target = jewelImageResourceDictionary[type];
            //    graphics.DrawImage(Helpers.ResizeImage(target, cell.Width - bitmapOffset, cell.Height - bitmapOffset, true, true), new Rectangle(cell.X + bitmapOffset, cell.Y + bitmapOffset, cell.Width - bitmapOffset, cell.Height - bitmapOffset), new Rectangle(0, 0, cell.Width, cell.Height), GraphicsUnit.Pixel);

            //}

            for (int i = 0; i < gameEngine.GameStateModel.MineModel.Columns; i++)
            {
                for (int j = 0; j < gameEngine.GameStateModel.MineModel.Depth; j++)
                {
                    JewelModel jewel = (JewelModel)gameEngine.GameStateModel.MineModel.Mine[i, j];
                    if (jewel != null)
                    {
                        Rectangle cell = cells[i, j];
                        Bitmap target = jewelResizedImageResourceDictionary[jewel.JewelType];
                        graphics.DrawImage(target, new Rectangle(cell.X + bitmapOffset, cell.Y + bitmapOffset, cell.Width - bitmapOffset, cell.Height - bitmapOffset), new Rectangle(0, 0, cell.Width, cell.Height), GraphicsUnit.Pixel);
                    }
                }
            }

        }

        /// <summary>
        /// Handles the Layout event of the GameView control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="LayoutEventArgs"/> instance containing the event data.</param>
        private void GameView_Layout(object sender, LayoutEventArgs e)
        {
            // calculate new cell dimensions
            cellWidth = ClientRectangle.Width / cells.GetLength(0);
            cellHeight = ClientRectangle.Height / cells.GetLength(1);
            // calculate new image sizes
            jewelResizedImageResourceDictionary = Helpers.GenerateResizedJewelImageResourceDictionary(jewelImageResourceDictionary, cellWidth, cellHeight, bitmapOffset);
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
            TextureBrush brush = null;
            if (levelBackgroundDictionary.ContainsKey(gameEngine.GameStateModel.GameLevel))
            {
                brush = levelBackgroundDictionary[gameEngine.GameStateModel.GameLevel];
            }
            else
            {
                int randomIndex = Helpers.GenerateRandomIndex(backgroundImageArray, rand);
                brush = new TextureBrush(backgroundImageArray[randomIndex], WrapMode.Tile);
                levelBackgroundDictionary.Add(gameEngine.GameStateModel.GameLevel, brush);
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


            cellWidth = ClientRectangle.Width / cells.GetLength(0);
            cellHeight = ClientRectangle.Height / cells.GetLength(1);
            int leftOverWidth = ClientRectangle.Width - (cellWidth * cells.GetLength(0));
            int leftOverHeight = ClientRectangle.Height - (cellHeight * cells.GetLength(1));

            int heightOffset = leftOverHeight / 2;
            int widthOffset = leftOverWidth / 2;

            for (int i = 0; i <= cells.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= cells.GetUpperBound(1); j++)
                {
                    cells[i, j] = new Rectangle(i * cellWidth + widthOffset, j * cellHeight + heightOffset, cellWidth, cellHeight);
                    g.DrawRectangle(pen, cells[i, j]);
                }
            }
        }

        /// <summary>
        /// Calculates the invalidation region.
        /// </summary>
        /// <param name="jewelModel">The jewelModel.</param>
        /// <returns></returns>
        public Rectangle CalculateInvalidationRegion(JewelModel jewelModel, int x, int y)
        {
            Rectangle region = new Rectangle();
            region = cells[x, y];
            region.Width = cellWidth;
            region.Height = cellHeight;
            return (region);
        }

        /// <summary>
        /// Calculates the invalidation region.
        /// </summary>
        /// <param name="jewelModel">The jewelModel.</param>
        /// <param name="originalX">The original x.</param>
        /// <param name="originalY">The original y.</param>
        /// <param name="newX">The new x.</param>
        /// <param name="newY">The new y.</param>
        /// <returns></returns>
        public Rectangle CalculateInvalidationRegion(JewelModel jewelModel, int originalX, int originalY, int newX, int newY)
        {
            Rectangle region = new Rectangle();
            int minX = Math.Min(originalX, newX);
            int minY = Math.Min(originalY, newY);
            Rectangle targetCell = cells[minX, minY];
            region.X = targetCell.X;
            region.Y = targetCell.Y;
            // movement down
            if (newY > originalY)
            {
                region.Height = cellHeight * 2;
                region.Width = cellWidth;
            }
            // movement right or left
            else
            {
                region.Height = cellHeight;
                region.Width = cellWidth * 2;
            }
            return (region);
        }

        /// <summary>
        /// Draws the objects.
        /// </summary>
        //private void DrawObjects<T>(Graphics graphics, T[,] objects, Rectangle[,] squares)
        //    where T : IDrawable
        //{
        //    for (int i = 0; i <= squares.GetUpperBound(0); i++)
        //    {
        //        for (int j = 0; j <= squares.GetUpperBound(1); j++)
        //        {
        //            T obj = objects[i, j];
        //            if (obj != null)
        //                obj.Draw(squares[i, j], this, graphics);
        //        }
        //    }
        //}

    }
}
