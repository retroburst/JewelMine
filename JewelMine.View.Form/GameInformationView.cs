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
        private Brush informationShadowBrushBlack = null;
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
        /// <param name="gameLogic">The game engine.</param>
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
            informationShadowBrushBlack = new SolidBrush(Color.FromArgb(150, Color.Black));
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
            DrawGameState(graphics, clientWidth, clientHeight);
            DrawLevel(graphics, clientWidth);
            DrawScore(graphics, clientWidth);
        }

        /// <summary>
        /// Draws the level.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="clientWidth">Width of the client.</param>
        private void DrawLevel(Graphics graphics, int clientWidth)
        {
            Brush levelBrush = previousLevel != gameLogic.State.Level ? informationOverlayBrushWhite : informationOverlayBrushPartiallyTransparent;
            string level = string.Format(ViewConstants.LEVEL_PATTERN, gameLogic.State.Level.ToString("00"));
            SizeF levelSize = graphics.MeasureString(level, informationFont);
            int levelXPosition = (clientWidth - (int)levelSize.Width - 5);
            levelRectangle = new Rectangle(levelXPosition, 5, (int)levelSize.Width, (int)levelSize.Height);
            graphics.FillRectangle(informationShadowBrushBlack, levelRectangle);
            graphics.DrawString(level, informationFont, levelBrush, new PointF(levelXPosition, 5));
            previousLevel = gameLogic.State.Level;
        }

        /// <summary>
        /// Draws the score.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="clientWidth">Width of the client.</param>
        private void DrawScore(Graphics graphics, int clientWidth)
        {
            Brush scoreBrush = previousScore != gameLogic.State.Score ? informationOverlayBrushWhite : informationOverlayBrushPartiallyTransparent;
            string score = string.Format(ViewConstants.SCORE_PATTERN, gameLogic.State.Score.ToString("000000"));
            SizeF scoreSize = graphics.MeasureString(score, informationFont);
            scoreRectangle = new Rectangle(5, 5, (int)scoreSize.Width, (int)scoreSize.Height);
            graphics.FillRectangle(informationShadowBrushBlack, scoreRectangle);
            graphics.DrawString(score, informationFont, scoreBrush, new PointF(5, 5));
            previousScore = gameLogic.State.Score;
        }

        /// <summary>
        /// Draws the state of the game.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="clientWidth">Width of the client.</param>
        /// <param name="clientHeight">Height of the client.</param>
        private void DrawGameState(Graphics graphics, int clientWidth, int clientHeight)
        {
            if (gameLogic.State.PlayState != GamePlayState.Playing)
            {
                string stateText = string.Empty;
                string stateSubText = string.Empty;
                GetGameStateText(out stateText, out stateSubText);
                SizeF stateTextSize = graphics.MeasureString(stateText, gameStateTextFont);
                SizeF stateSubTextSize = graphics.MeasureString(stateSubText, gameStateSubTextFont);
                Coordinates stateTextMiddle = CalculateMiddleCoordinatesForText(stateTextSize, clientWidth, clientHeight);
                Coordinates stateSubTextMiddle = CalculateMiddleCoordinatesForText(stateSubTextSize, clientWidth, clientHeight);

                // give a little more room
                //stateTextSize.Width += 2;
                //stateSubTextSize.Width += 2;

                gameStateTextRectangle = new Rectangle(stateTextMiddle.X, stateTextMiddle.Y, (int)stateTextSize.Width, (int)stateTextSize.Height);
                gameStateSubTextRectangle = new Rectangle(stateSubTextMiddle.X, stateSubTextMiddle.Y + (int)stateTextSize.Height, (int)stateSubTextSize.Width, (int)stateSubTextSize.Height);

                graphics.FillRectangle(informationShadowBrushBlack, gameStateTextRectangle);
                graphics.FillRectangle(informationShadowBrushBlack, gameStateSubTextRectangle);

                graphics.DrawString(stateText, gameStateTextFont, informationOverlayBrushWhite, new PointF(gameStateTextRectangle.X, gameStateTextRectangle.Y));
                graphics.DrawString(stateSubText, gameStateSubTextFont, informationOverlayBrushWhite, new PointF(gameStateSubTextRectangle.X, gameStateSubTextRectangle.Y));
            }
            else
            {
                gameStateTextRectangle = Rectangle.Empty;
                gameStateSubTextRectangle = Rectangle.Empty;
            }
        }

        /// <summary>
        /// Gets the game state text.
        /// </summary>
        /// <param name="stateText">The state text.</param>
        /// <param name="stateSubText">The state sub text.</param>
        private void GetGameStateText(out string stateText, out string stateSubText)
        {
            stateText = string.Empty;
            stateSubText = string.Empty;
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
