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
        private Rectangle debugRectangle = Rectangle.Empty;
        private Rectangle debugInvalidationRetangle = Rectangle.Empty;
        private Font informationFont = null;
        private Font gameStateTextFont = null;
        private Font gameStateSubTextFont = null;
        private GameLogic gameLogic = null;
        private long previousScore = 0;
        private int previousLevel = GameConstants.GAME_DEFAULT_LEVEL;
        private bool previousMusicMutedState = false;
        private bool previousSoundMutedState = false;
        private bool showDebugInfo = false;
        private List<ToggleInformation> toggleInformationList = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameInformationView" /> class.
        /// </summary>
        /// <param name="gameLogic">The game engine.</param>
        public GameInformationView(GameLogic logic)
        {
            gameLogic = logic;
            toggleInformationList = new List<ToggleInformation>();
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
            informationShadowBrushBlack = new SolidBrush(Color.FromArgb(130, Color.Black));
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
            if (debugRectangle != Rectangle.Empty) invalidate(debugRectangle);
            // need to invalidate the debug messages rect as it's been hidden
            if (debugInvalidationRetangle != Rectangle.Empty)
            {
                invalidate(debugInvalidationRetangle);
                debugInvalidationRetangle = Rectangle.Empty;
            }
            InvalidateToggles(invalidate);
        }

        /// <summary>
        /// Invalidates the toggles.
        /// </summary>
        /// <param name="invalidate">The invalidate.</param>
        private void InvalidateToggles(Action<Rectangle> invalidate)
        {
            // invalidate any rectangles - add some padding for invalidation rectangles
            if (toggleInformationList.Count > 0)
            {
                int maxWidth = toggleInformationList.OrderByDescending(x => x.Rectangle.Width).First().Rectangle.Width;
                toggleInformationList.ForEach(x =>
                    invalidate(new Rectangle(x.Rectangle.Location, new Size(maxWidth, x.Rectangle.Height))));
            }
        }

        /// <summary>
        /// Draws the game information.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="clientWidth">Width of the client.</param>
        /// <param name="clientHeight">Height of the client.</param>
        /// <param name="windowWidth">Width of the window.</param>
        /// <param name="windowHeight">Height of the window.</param>
        /// <param name="backgroundMusicMuted">if set to <c>true</c> [background music on].</param>
        /// <param name="soundEffectsMuted">if set to <c>true</c> [sound fx on].</param>
        public void DrawGameInformation(Graphics graphics,
            int clientWidth, int clientHeight,
            int windowWidth, int windowHeight,
            bool backgroundMusicMuted, bool soundEffectsMuted)
        {
            DrawGameState(graphics, clientWidth, clientHeight);
            DrawLevel(graphics, clientWidth);
            DrawScore(graphics, clientWidth);
            DrawToggles(graphics, clientWidth, backgroundMusicMuted, soundEffectsMuted);
            if (showDebugInfo) DrawDebugInfo(graphics, clientWidth, clientHeight, windowWidth, windowHeight, backgroundMusicMuted, soundEffectsMuted);
        }

        /// <summary>
        /// Draws the toggles.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="clientWidth">Width of the client.</param>
        /// <param name="backgroundMusicMuted">if set to <c>true</c> [background music muted].</param>
        /// <param name="soundEffectsMuted">if set to <c>true</c> [sound effects muted].</param>
        private void DrawToggles(Graphics graphics, int clientWidth, bool backgroundMusicMuted, bool soundEffectsMuted)
        {
            bool backgroundMusicMutedStateChanged = (previousMusicMutedState != backgroundMusicMuted);
            bool soundEffectsMutedStateChanged = (previousSoundMutedState != soundEffectsMuted);
            // if there is a change again search in the list for existing and update the visible count
            ProcessBackgroundMusicToggle(backgroundMusicMuted, backgroundMusicMutedStateChanged);
            ProcessSoundEffectsToggle(soundEffectsMuted, soundEffectsMutedStateChanged);
            // remove any toggles that now expired
            toggleInformationList.RemoveAll(x => x.ToggleVisibleTickCount >= ViewConstants.TOGGLE_DISPLAY_TICK_COUNT);
            // draw toggles in order - updating the rectangles as we go and visible counts
            int yPosition = ViewConstants.TOGGLE_Y_OFFSET;
            foreach (ToggleInformation toggleInfo in toggleInformationList)
            {
                SizeF toggleSize = graphics.MeasureString(toggleInfo.ToggleText, informationFont);
                toggleInfo.Rectangle = new Rectangle(ViewConstants.DEFAULT_X_OFFSET, yPosition, (int)toggleSize.Width, (int)toggleSize.Height);
                graphics.FillRectangle(informationShadowBrushBlack, toggleInfo.Rectangle);
                graphics.DrawString(toggleInfo.ToggleText, informationFont, informationOverlayBrushPartiallyTransparent, new PointF(ViewConstants.DEFAULT_X_OFFSET, yPosition));
                toggleInfo.ToggleVisibleTickCount++;
                yPosition += (int)toggleSize.Height;
            }
            // save the current state of toggles
            previousMusicMutedState = backgroundMusicMuted;
            previousSoundMutedState = soundEffectsMuted;
        }

        /// <summary>
        /// Processes the sound effects toggle.
        /// </summary>
        /// <param name="soundEffectsMuted">if set to <c>true</c> [sound effects muted].</param>
        /// <param name="soundEffectsMutedStateChanged">if set to <c>true</c> [sound effects muted state changed].</param>
        private void ProcessSoundEffectsToggle(bool soundEffectsMuted, bool soundEffectsMutedStateChanged)
        {
            if (soundEffectsMutedStateChanged)
            {
                ToggleInformation toggleInformation = toggleInformationList.FirstOrDefault(x => x.Type == ToggleType.SoundEffects);
                if (toggleInformation != null)
                {
                    toggleInformation.ToggleVisibleTickCount = 0;
                    toggleInformation.ToggleText = string.Format(ViewConstants.TOGGLE_SOUND_PATTERN, ViewHelpers.EncodeBooleanForDisplay(!soundEffectsMuted));
                }
                else
                {
                    ToggleInformation newToggleInformation = new ToggleInformation()
                    {
                        Type = ToggleType.SoundEffects,
                        Rectangle = Rectangle.Empty,
                        ToggleText = string.Format(ViewConstants.TOGGLE_SOUND_PATTERN, ViewHelpers.EncodeBooleanForDisplay(!soundEffectsMuted)),
                        ToggleVisibleTickCount = 0
                    };
                    toggleInformationList.Add(newToggleInformation);
                }
            }
        }

        /// <summary>
        /// Processes the background music toggle.
        /// </summary>
        /// <param name="backgroundMusicMuted">if set to <c>true</c> [background music muted].</param>
        /// <param name="backgroundMusicMutedStateChanged">if set to <c>true</c> [background music muted state changed].</param>
        private void ProcessBackgroundMusicToggle(bool backgroundMusicMuted, bool backgroundMusicMutedStateChanged)
        {
            if (backgroundMusicMutedStateChanged)
            {
                ToggleInformation toggleInformation = toggleInformationList.FirstOrDefault(x => x.Type == ToggleType.BackgroundMusic);
                if (toggleInformation != null)
                {
                    toggleInformation.ToggleVisibleTickCount = 0;
                    toggleInformation.ToggleText = string.Format(ViewConstants.TOGGLE_MUSIC_PATTERN, ViewHelpers.EncodeBooleanForDisplay(!backgroundMusicMuted));
                }
                else
                {
                    ToggleInformation newToggleInformation = new ToggleInformation()
                    {
                        Type = ToggleType.BackgroundMusic,
                        Rectangle = Rectangle.Empty,
                        ToggleText = string.Format(ViewConstants.TOGGLE_MUSIC_PATTERN, ViewHelpers.EncodeBooleanForDisplay(!backgroundMusicMuted)),
                        ToggleVisibleTickCount = 0
                    };
                    toggleInformationList.Add(newToggleInformation);
                }
            }
        }

        /// <summary>
        /// Draws the debug information.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="clientWidth">Width of the client.</param>
        /// <param name="clientHeight">Height of the client.</param>
        /// <param name="windowWidth">Width of the window.</param>
        /// <param name="windowHeight">Height of the window.</param>
        /// <param name="backgroundMusicMuted">if set to <c>true</c> [background music on].</param>
        /// <param name="soundEffectsMuted">if set to <c>true</c> [sound fx on].</param>
        private void DrawDebugInfo(Graphics graphics, int clientWidth, int clientHeight,
            int windowWidth, int windowHeight,
            bool backgroundMusicMuted, bool soundEffectsMuted)
        {
            List<string> debugMessages = new List<string>();
            debugRectangle = new Rectangle();
            debugRectangle.X = clientWidth - ViewConstants.DEBUG_RECTANGLE_WIDTH;
            debugRectangle.Y = ViewConstants.DEBUG_RECTANGLE_HEIGHT_OFFSET;
            int yPosition = debugRectangle.Y + ViewConstants.DEBUG_RECTANGLE_PADDING;
            debugRectangle.Width = ViewConstants.DEBUG_RECTANGLE_WIDTH;
            debugRectangle.Height = clientHeight - ViewConstants.DEBUG_RECTANGLE_HEIGHT_OFFSET;
            graphics.FillRectangle(informationShadowBrushBlack, debugRectangle);
            BuildDebugMessages(debugMessages, clientWidth, clientHeight, windowWidth, windowHeight, backgroundMusicMuted, soundEffectsMuted);
            foreach (string message in debugMessages)
            {
                SizeF size = graphics.MeasureString(message, informationFont);
                graphics.DrawString(message, informationFont, informationOverlayBrushPartiallyTransparent, debugRectangle.X + ViewConstants.DEBUG_RECTANGLE_PADDING, yPosition);
                yPosition += (int)size.Height;
            }
        }

        /// <summary>
        /// Builds the debug messages.
        /// </summary>
        /// <param name="debugMessages">The debug messges.</param>
        /// <param name="clientWidth">Width of the client.</param>
        /// <param name="clientHeight">Height of the client.</param>
        /// <param name="windowWidth">Width of the window.</param>
        /// <param name="windowHeight">Height of the window.</param>
        /// <param name="backgroundMusicMuted">if set to <c>true</c> [background music on].</param>
        /// <param name="soundEffectsMuted">if set to <c>true</c> [sound fx on].</param>
        /// <exception cref="NotImplementedException"></exception>
        private void BuildDebugMessages(List<string> debugMessages,
            int clientWidth, int clientHeight,
            int windowWidth, int windowHeight,
            bool backgroundMusicMuted, bool soundEffectsMuted)
        {
            JewelGroup delta = gameLogic.State.Mine.Delta;
            string deltaJewels = "N/A";
            string deltaPosition = "N/A";
            if (delta != null) deltaPosition = string.Format("x={0}, y={1},{2},{3}", delta.Bottom.Coordinates.X, delta.Top.Coordinates.Y, delta.Middle.Coordinates.Y, delta.Bottom.Coordinates.Y);
            if (delta != null) deltaJewels = string.Format("{0}, {1}, {2}", GameHelpers.ShortenName(delta.Top.Jewel.JewelType.ToString()), GameHelpers.ShortenName(delta.Middle.Jewel.JewelType.ToString()), GameHelpers.ShortenName(delta.Bottom.Jewel.JewelType.ToString()));
            debugMessages.Add(string.Format("Window Size [{0}x{1}]", windowWidth, windowHeight));
            debugMessages.Add(string.Format("Client Size [{0}x{1}]", clientWidth, clientHeight));
            debugMessages.Add(string.Format("Mine Size [{0}x{1}]", gameLogic.State.Mine.Columns, gameLogic.State.Mine.Depth));
            debugMessages.Add(string.Format("Tick Milliseconds [{0}]", gameLogic.State.TickSpeedMilliseconds));
            debugMessages.Add(string.Format("Delta Statn. Tick [{0}]", gameLogic.State.DeltaStationaryTickCount));
            debugMessages.Add(string.Format("Finalise Col. Tick [{0}]", gameLogic.State.CollisionFinailseTickCount));
            debugMessages.Add(string.Format("State [{0}]", gameLogic.State.PlayState.ToString()));
            debugMessages.Add(string.Format("Delta [{0}]", gameLogic.State.Mine.Delta == null ? "None" : "Active"));
            debugMessages.Add(string.Format("Delta Position [{0}]", deltaPosition));
            debugMessages.Add(string.Format("Delta All In Bounds [{0}]", delta == null ? "N/A" : delta.HasWholeGroupEnteredBounds.ToString()));
            debugMessages.Add(string.Format("Delta Stat. Tick Count [{0}]", delta == null ? "N/A" : delta.StationaryTickCount.ToString()));
            debugMessages.Add(string.Format("Delta Jewels [{0}]", deltaJewels));
            debugMessages.Add(string.Format("Marked Collision Count [{0}]", gameLogic.State.Mine.MarkedCollisions.Count));
            debugMessages.Add(string.Format("Finalised Collision Count [{0}]", gameLogic.State.Mine.FinalisedCollisions.Count));
            debugMessages.Add(string.Format("Music [{0}]", backgroundMusicMuted ? "Muted" : "On"));
            debugMessages.Add(string.Format("Sound Effects [{0}]", soundEffectsMuted ? "Muted" : "On"));
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
            levelRectangle = new Rectangle(levelXPosition, ViewConstants.DEFAULT_Y_OFFSET, (int)levelSize.Width, (int)levelSize.Height);
            graphics.FillRectangle(informationShadowBrushBlack, levelRectangle);
            graphics.DrawString(level, informationFont, levelBrush, new PointF(levelXPosition, ViewConstants.DEFAULT_Y_OFFSET));
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
            string score = string.Format(ViewConstants.SCORE_PATTERN, gameLogic.State.Score.ToString(ViewConstants.SCORE_FORMAT_STRING));
            SizeF scoreSize = graphics.MeasureString(score, informationFont);
            scoreRectangle = new Rectangle(ViewConstants.DEFAULT_X_OFFSET, ViewConstants.DEFAULT_Y_OFFSET, (int)scoreSize.Width, (int)scoreSize.Height);
            graphics.FillRectangle(informationShadowBrushBlack, scoreRectangle);
            graphics.DrawString(score, informationFont, scoreBrush, new PointF(ViewConstants.DEFAULT_X_OFFSET, ViewConstants.DEFAULT_Y_OFFSET));
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

        /// <summary>
        /// Gets or sets the toggleInformation debug information.
        /// </summary>
        /// <value>
        /// The toggleInformation debug information.
        /// </value>
        public void ToggleDebugInfo()
        {
            showDebugInfo = !showDebugInfo;
            if (!showDebugInfo)
            {
                debugInvalidationRetangle = new Rectangle(debugRectangle.Location, debugRectangle.Size);
            }
        }

        /// <summary>
        /// Toggle types supported.
        /// </summary>
        private enum ToggleType
        {
            BackgroundMusic,
            SoundEffects
        }

        /// <summary>
        /// Manages information about an existing toggleInformation information display.
        /// </summary>
        private class ToggleInformation
        {
            public ToggleType Type { get; set; }
            public Rectangle Rectangle { get; set; }
            public string ToggleText { get; set; }
            public int ToggleVisibleTickCount { get; set; }
        }

    }
}
