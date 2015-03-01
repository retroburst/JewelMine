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
        private Dictionary<JewelType, Bitmap> imageResourceDictionary = Helpers.GenerateImageResourceDictionary();
        private Rectangle[,] cells = null;
        private int cellHeight = 0;
        private int cellWidth = 0;
        private int bitmapOffset = 2;
        private long startTime = 0;

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
            gameEngine.StartGame();
        }

        private void gameEngine_GameStateChanged(object sender, GameStateModel e)
        {
            // TODO need to know what has changed so can calculate invalidate region
            // and re-draw just that.
            Invalidate(ClientRectangle);
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
            timer.Start();
            while (gameEngine.GameStateModel.GamePlayState == GamePlayState.Playing)
            {
                startTime = timer.ElapsedMilliseconds;
                // TODO get the movement information to send to view for invalidation regions
                gameEngine.PerformGameLogic();
                Invalidate();
                Application.DoEvents();
                while (timer.ElapsedMilliseconds - startTime < gameEngine.GameStateModel.GameTickSpeedMilliseconds) { }
            }
            timer.Stop();
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
            DrawBackground(graphics, Helpers.GetBackgroundImage());
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
            //    Bitmap target = imageResourceDictionary[type];
            //    graphics.DrawImage(Helpers.ResizeImage(target, cell.Width - bitmapOffset, cell.Height - bitmapOffset, true, true), new Rectangle(cell.X + bitmapOffset, cell.Y + bitmapOffset, cell.Width - bitmapOffset, cell.Height - bitmapOffset), new Rectangle(0, 0, cell.Width, cell.Height), GraphicsUnit.Pixel);

            //}

            for(int i = 0; i < gameEngine.GameStateModel.MineModel.Columns; i++)
            {
                for(int j = 0; j < gameEngine.GameStateModel.MineModel.Depth; j++)
                {
                    JewelModel jewel = (JewelModel)gameEngine.GameStateModel.MineModel.Mine[i, j];
                    if (jewel != null)
                    {
                        Rectangle cell = cells[i, j];
                        Bitmap target = imageResourceDictionary[jewel.JewelType];
                        graphics.DrawImage(Helpers.ResizeImage(target, cell.Width - bitmapOffset, cell.Height - bitmapOffset, true, true), new Rectangle(cell.X + bitmapOffset, cell.Y + bitmapOffset, cell.Width - bitmapOffset, cell.Height - bitmapOffset), new Rectangle(0, 0, cell.Width, cell.Height), GraphicsUnit.Pixel);
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
            // invalidate the whole view so it is
            // all re-painted
            Invalidate(ClientRectangle);
        }       

        /// <summary>
        /// Draws the background.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="bg">The bg.</param>
        private void DrawBackground(Graphics g, Image bg)
        {
           if (bg == null) return;
           using (TextureBrush brush = new TextureBrush(bg, WrapMode.Tile))
           {
               g.FillRectangle(brush, 0, 0, ClientRectangle.Width, ClientRectangle.Height);
           }
        }

        /// <summary>
        /// Draws the grid.
        /// </summary>
        /// <param name="g">The g.</param>
        /// <param name="s">The s.</param>
        private void DrawGrid(Graphics g, Rectangle[,] s)
        {
            Pen pen = null;
            if (pen == null)
            {
                pen = new Pen(Color.SlateGray);
                pen.Alignment = PenAlignment.Center;
                pen.DashStyle = DashStyle.Dot;
                pen.DashCap = DashCap.Round;
                pen.EndCap = LineCap.DiamondAnchor;
                pen.LineJoin = LineJoin.Round;
                pen.StartCap = LineCap.DiamondAnchor;
            }

            cellWidth = ClientRectangle.Width / s.GetLength(0);
            cellHeight = ClientRectangle.Height / s.GetLength(1);
            int leftOverWidth = ClientRectangle.Width - (cellWidth * s.GetLength(0));
            int leftOverHeight = ClientRectangle.Height - (cellHeight * s.GetLength(1));

            int heightOffset = leftOverHeight / 2;
            int widthOffset = leftOverWidth / 2;

            for (int i = 0; i <= s.GetUpperBound(0); i++)
            {
                for (int j = 0; j <= s.GetUpperBound(1); j++)
                {
                    s[i, j] = new Rectangle(i * cellWidth + widthOffset, j * cellHeight + heightOffset, cellWidth, cellHeight);
                    g.DrawRectangle(pen, s[i, j]);
                }
            }
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
