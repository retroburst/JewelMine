using JewelMine.Engine;
using JewelMine.Engine.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JewelMine.View.Forms
{
    /// <summary>
    /// Represents game information to display on view.
    /// </summary>
    public class GameInformationView
    {
        private Brush informationOverlayBrushPartiallyTransparent = null;
        private Brush informationOverlayBrushWhite = null;
        private Rectangle scoreRectangle = Rectangle.Empty;
        private Rectangle levelRectangle = Rectangle.Empty;
        private Rectangle gameStateTextRectangle = Rectangle.Empty;
        private Rectangle gameStateSubTextRectangle = Rectangle.Empty;
        private Font informationFont = null;
        private Font gameStateTextFont = null;
        private Font gameStateSubTextFont = null;
        private GameLogic gameLogic = null;
        private long previousScore = 0;
        private int previousLevel = GameConstants.GAME_DEFAULT_LEVEL;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameInformationView" /> class.
        /// </summary>
        /// <param name="gameEngine">The game engine.</param>
        public GameInformationView(GameLogic logic)
        {
            gameLogic = logic;
            InitialiseDrawingObjects();
        }

        /// <summary>
        /// Initialises the drawing objects.
        /// </summary>
        private void InitialiseDrawingObjects()
        {
            informationOverlayBrushPartiallyTransparent = new SolidBrush(Color.FromArgb(110, Color.White));
            informationOverlayBrushWhite = new SolidBrush(Color.White);
            informationFont = new Font(SystemInformation.MenuFont.FontFamily, 12.5f);
            gameStateTextFont = new Font(SystemInformation.MenuFont.FontFamily, 36.0f, FontStyle.Bold);
            gameStateSubTextFont = new Font(SystemInformation.MenuFont.FontFamily, 12.5f, FontStyle.Regular);
        }

        /// <summary>
        /// Invalidates this instance.
        /// </summary>
        public void Invalidate(Action<Rectangle> invalidate)
        {
            if (scoreRectangle != Rectangle.Empty) invalidate(scoreRectangle);
            if (levelRectangle != Rectangle.Empty) invalidate(levelRectangle);
            if (gameStateTextRectangle != Rectangle.Empty) invalidate(gameStateTextRectangle);
            if (gameStateSubTextRectangle != Rectangle.Empty) invalidate(gameStateSubTextRectangle);
        }

        /// <summary>
        /// Draws the game information.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        public void DrawGameInformation(Graphics graphics, int clientWidth, int clientHeight)
        {
            Brush levelBrush = previousLevel != gameLogic.State.Level ? informationOverlayBrushWhite : informationOverlayBrushPartiallyTransparent;
            Brush scoreBrush = previousScore != gameLogic.State.Score ? informationOverlayBrushWhite : informationOverlayBrushPartiallyTransparent;

            string score = string.Format(ViewConstants.SCORE_PATTERN, gameLogic.State.Score.ToString("000000"));
            string level = string.Format(ViewConstants.LEVEL_PATTERN, gameLogic.State.Level.ToString("00"));
            SizeF scoreSize = graphics.MeasureString(score, informationFont);
            SizeF levelSize = graphics.MeasureString(level, informationFont);
            int levelXPosition = (clientWidth - (int)levelSize.Width - 5);
            scoreRectangle = new Rectangle(5, 5, (int)scoreSize.Width, (int)scoreSize.Height);
            levelRectangle = new Rectangle(levelXPosition, 5, (int)levelSize.Width, (int)levelSize.Height);

            if (gameLogic.State.PlayState != GamePlayState.Playing)
            {
                string stateText = string.Empty;
                string stateSubText = string.Empty;
                switch (gameLogic.State.PlayState)
                {
                    case GamePlayState.NotStarted:
                        stateText = ViewConstants.GAME_START_TEXT;
                        stateSubText = ViewConstants.GAME_START_SUBTEXT;
                        break;
                    case GamePlayState.GameOver:
                        stateText = ViewConstants.GAME_OVER_TEXT;
                        stateSubText = ViewConstants.GAME_OVER_SUBTEXT;
                        break;
                    case GamePlayState.GameWon:
                        stateText = ViewConstants.GAME_WON_TEXT;
                        stateSubText = ViewConstants.GAME_WON_SUBTEXT;
                        break;
                    case GamePlayState.Paused:
                        stateText = ViewConstants.GAME_PAUSED_TEXT;
                        stateSubText = ViewConstants.GAME_PAUSED_SUBTEXT;
                        break;
                }
                SizeF stateTextSize = graphics.MeasureString(stateText, gameStateTextFont);
                SizeF stateSubTextSize = graphics.MeasureString(stateSubText, gameStateSubTextFont);
                Coordinates stateTextMiddle = CalculateMiddleCoordinatesForText(stateTextSize, clientWidth, clientHeight);
                Coordinates stateSubTextMiddle = CalculateMiddleCoordinatesForText(stateSubTextSize, clientWidth, clientHeight);

                gameStateTextRectangle = new Rectangle(stateTextMiddle.X, stateTextMiddle.Y, (int)stateTextSize.Width, (int)stateTextSize.Height);
                gameStateSubTextRectangle = new Rectangle(stateSubTextMiddle.X, stateSubTextMiddle.Y + (int)stateTextSize.Height, (int)stateSubTextSize.Width, (int)stateSubTextSize.Height);

                //TODO: this background work for info text
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(150, Color.Black)), gameStateTextRectangle);

                graphics.DrawString(stateText, gameStateTextFont, informationOverlayBrushWhite, new PointF(gameStateTextRectangle.X, gameStateTextRectangle.Y));
                graphics.DrawString(stateSubText, gameStateSubTextFont, informationOverlayBrushWhite, new PointF(gameStateSubTextRectangle.X, gameStateSubTextRectangle.Y));
            }
            else
            {
                gameStateTextRectangle = Rectangle.Empty;
                gameStateSubTextRectangle = Rectangle.Empty;
            }

            graphics.DrawString(score, informationFont, scoreBrush, new PointF(5, 5));
            graphics.DrawString(level, informationFont, levelBrush, new PointF(levelXPosition, 5));

            previousLevel = gameLogic.State.Level;
            previousScore = gameLogic.State.Score;
        }

        /// <summary>
        /// Calculates the middle coordinates for text.
        /// </summary>
        /// <param name="textSize">Size of the text.</param>
        /// <param name="clientWidth">Width of the client.</param>
        /// <param name="clientHeight">Height of the client.</param>
        /// <returns></returns>
        private Coordinates CalculateMiddleCoordinatesForText(SizeF textSize, int clientWidth, int clientHeight)
        {
            int halfWidth = (int)textSize.Width / 2;
            int halfHeight = (int)textSize.Height / 2;
            int x = (clientWidth / 2) - halfWidth;
            int y = (clientHeight / 2) - halfHeight;
            return (new Coordinates(x, y));
        }

    }
}
