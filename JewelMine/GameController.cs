using JewelMine.Engine;
using JewelMine.View.Forms;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JewelMine
{
    /// <summary>
    /// Game controller manages the major components
    /// of the game and runs the game loop.
    /// </summary>
    public class GameController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(GameController));
        private GameTimer timer = null;
        private GameLogic gameLogic = null;
        private GameView view = null;
        private GameLogicInput logicInput = null;
        private GameAudioSystem gameAudioSystem = null;
        private Dictionary<Keys, Action> keyBindingDictionary = null;
        private Size preferredWindowSize = Size.Empty;
        private bool exitingGame = false;
        private long gameStartTime = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameController"/> class.
        /// </summary>
        public GameController()
        {
            gameAudioSystem = GameAudioSystem.Instance;
            timer = new GameTimer();
            logicInput = new GameLogicInput();

            keyBindingDictionary = new Dictionary<Keys, Action>();
            InitialiseKeyBindings();
            
            preferredWindowSize = new Size(ViewConstants.WINDOW_PREFERRED_WIDTH, ViewConstants.WINDOW_PREFERRED_HEIGHT);
            
            GameLogicUserSettings settings = new GameLogicUserSettings();
            BuildGameLogicUserSettings(settings);
            gameLogic = new GameLogic(settings);
        }

        /// <summary>
        /// Runs the game.
        /// </summary>
        public void RunGame()
        {
            // create a new view and guarantee it 
            // will be disposed with 'using' statement
            using (view = new GameView(gameLogic))
            {
                // hook up to view events
                view.FormClosing += HandleViewClosing;
                view.KeyDown += HandleViewKeyDown;
                view.Load += HandleViewLoad;
                view.Layout += HandleViewLayout;
                // restore user preferences
                RestoreUserPreferences();
                // show the form
                view.Show();
                // proceed into the main game loop
                GameLoop();
            }
            // clean up audio system
            gameAudioSystem.Dispose();
        }

        /// <summary>
        /// Handles the view closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FormClosingEventArgs"/> instance containing the event data.</param>
        private void HandleViewClosing(object sender, FormClosingEventArgs e)
        {
            SaveViewWindowState();
            SaveUserPreferences();
            Properties.Settings.Default.Save();
            exitingGame = true;
        }

        /// <summary>
        /// Handles the view layout.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="LayoutEventArgs"/> instance containing the event data.</param>
        private void HandleViewLayout(object sender, LayoutEventArgs e)
        {
            if (view.WindowState == FormWindowState.Minimized)
            {
                logicInput.PauseGame = true;
                return;
            }
            view.UpdateLayout();
            if (logger.IsDebugEnabled) logger.DebugFormat("View window resized to {0}x{1}.", view.Width, view.Height);
        }

        /// <summary>
        /// Handles the view load.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void HandleViewLoad(object sender, EventArgs e)
        {
            RestoreViewWindowState();
        }

        /// <summary>
        /// Handles the view key down.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void HandleViewKeyDown(object sender, KeyEventArgs e)
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
        /// Initialises the key bindings.
        /// </summary>
        private void InitialiseKeyBindings()
        {
           PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingMoveLeft, Keys.Left, () => logicInput.DeltaMovement = MovementType.Left, keyBindingDictionary);
           PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingMoveRight, Keys.Right, () => logicInput.DeltaMovement = MovementType.Right, keyBindingDictionary);
           PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingMoveDown, Keys.Down, () => logicInput.DeltaMovement = MovementType.Down, keyBindingDictionary);
           PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingPauseGame, Keys.Control | Keys.P, () => logicInput.PauseGame = true, keyBindingDictionary);
           PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingRestartGame, Keys.Control | Keys.R, () => logicInput.RestartGame = true, keyBindingDictionary);
           PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingToggleDebugInfo, Keys.Control | Keys.D, () => view.ToggleDebugInfo(), keyBindingDictionary);
           PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingQuitGame, Keys.Control | Keys.Q, () => view.Close(), keyBindingDictionary);
           PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingSwapDeltaJewels, Keys.Space, () => logicInput.DeltaSwapJewels = true, keyBindingDictionary);
           PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingToggleMusic, Keys.Control | Keys.M, () => ToggleBackgroundMusicLoop(), keyBindingDictionary);
           PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingToggleSoundEffects, Keys.Control | Keys.S, () => ToggleSoundEffects(), keyBindingDictionary);
           PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingDifficultyChange, Keys.Control | Keys.U, () => logicInput.ChangeDifficulty = true, keyBindingDictionary);
           PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingSaveGame, Keys.Control | Keys.Shift | Keys.S, () => logicInput.SaveGame = true, keyBindingDictionary);
           PerformSafeKeyBinding(Properties.Settings.Default.KeyBindingLoadGame, Keys.Control | Keys.Shift | Keys.L, () => logicInput.LoadGame = true, keyBindingDictionary);
        }

        /// <summary>
        /// Performs a safe key binding.
        /// </summary>
        /// <param name="settingsKeyData">The settings key data.</param>
        /// <param name="defaultKeyData">The default key data.</param>
        /// <param name="bindingAction">The binding action.</param>
        /// <param name="keyBindings">The key bindings.</param>
        public static void PerformSafeKeyBinding(Keys settingsKeyData, Keys defaultKeyData, Action bindingAction, Dictionary<Keys, Action> keyBindings)
        {
            if (settingsKeyData == Keys.None)
            {
                keyBindings.Add(defaultKeyData, bindingAction);
            }
            else
            {
                keyBindings.Add(settingsKeyData, bindingAction);
            }
        }

        /// <summary>
        /// Games the loop.
        /// </summary>
        private void GameLoop()
        {
            if (logger.IsDebugEnabled) logger.Debug("Starting game loop.");
            timer.Start();
            gameAudioSystem.PlayBackgroundMusicLoop();
            while (!exitingGame)
            {
                gameStartTime = timer.ElapsedMilliseconds;
                Application.DoEvents();
                GameLogicUpdate logicUpdate = gameLogic.PerformGameLogic(logicInput);
                view.UpdateView(logicUpdate);
                gameAudioSystem.PlaySounds(logicUpdate);
                if ((timer.ElapsedMilliseconds - gameStartTime) < gameLogic.State.TickSpeedMilliseconds)
                {
                    // sleep the thread for the remaining time in this tick - saves burning CPU cycles
                    int sleepTime = (int)(gameLogic.State.TickSpeedMilliseconds - (timer.ElapsedMilliseconds - gameStartTime));
                    Thread.Sleep(sleepTime);
                }
                // reset user logicInput descriptors
                logicInput.Clear();
            }
            timer.Stop();
            if (logger.IsDebugEnabled) logger.Debug("Exiting game loop.");
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
        /// Builds the game logic user settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        private void BuildGameLogicUserSettings(GameLogicUserSettings settings)
        {
            settings.UserPreferredDifficulty = Properties.Settings.Default.UserPreferenceDifficulty;
        }

        /// <summary>
        /// Adds the background music state message.
        /// </summary>
        private void AddBackgroundMusicStateMessage()
        {
            string message = string.Format(ViewConstants.TOGGLE_MUSIC_PATTERN, ViewHelpers.EncodeBooleanForDisplay(!gameAudioSystem.BackgroundMusicMuted));
            view.AddGameInformationMessage(message);
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
            view.AddGameInformationMessage(message);
        }

        /// <summary>
        /// Toggles the sound effects.
        /// </summary>
        private void ToggleSoundEffects()
        {
            gameAudioSystem.ToggleSoundEffects();
            AddSoundEffectsStateMessage();
        }

        /// <summary>
        /// Restores the state of the view window.
        /// </summary>
        private void RestoreViewWindowState()
        {
            if (logger.IsDebugEnabled) logger.Debug("Restoring view window state.");
            if (Properties.Settings.Default.WindowSize.IsEmpty)
            {
                FitViewPreferredSizeToScreen();
                return; // state has never been saved
            }
            view.StartPosition = FormStartPosition.Manual;
            view.Location = Properties.Settings.Default.WindowLocation;
            view.Size = Properties.Settings.Default.WindowSize;
            view.WindowState = Properties.Settings.Default.WindowState == FormWindowState.Minimized ? FormWindowState.Normal : Properties.Settings.Default.WindowState;
        }

        /// <summary>
        /// Fits the preferred view size to screen.
        /// If the screen is too small, shrinks the
        /// form but tries to maintain aspect ratio.
        /// </summary>
        private void FitViewPreferredSizeToScreen()
        {
            if (logger.IsDebugEnabled) logger.Debug("Fitting preferred view size window to screen.");
            Screen screen = Screen.FromControl(view);
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
                view.Size = new Size(customWidth, customHeight);
            }
            else
            {
                view.Size = preferredWindowSize;
            }
            view.CenterViewWindow();
        }

        /// <summary>
        /// Saves the state of the view window.
        /// </summary>
        private void SaveViewWindowState()
        {
            if (logger.IsDebugEnabled) logger.Debug("Saving the view window state.");
            if (view.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.WindowLocation = view.Location;
                Properties.Settings.Default.WindowSize = view.Size;
            }
            else
            {
                Properties.Settings.Default.WindowLocation = view.RestoreBounds.Location;
                Properties.Settings.Default.WindowSize = view.RestoreBounds.Size;
            }
            Properties.Settings.Default.WindowState = view.WindowState;
        }

    }
}
