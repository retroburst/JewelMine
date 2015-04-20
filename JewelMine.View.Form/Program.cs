using JewelMine.Engine;
using JewelMine.Engine.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JewelMine.View.Forms
{
    /// <summary>
    /// Entry point for game execution.
    ///// </summary>
    public static class Program
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Program));
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //TODO: look in to moving main loop to logic layer and sending key presses to it (via event)
            //TODO: 
            log4net.Config.XmlConfigurator.Configure();
            try
            {
                if (logger.IsDebugEnabled) logger.Debug("Starting application.");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                GameLogicUserSettings settings = new GameLogicUserSettings();
                BuildGameLogicUserSettings(settings);
                using (GameView view = new GameView(new GameLogic(settings)))
                {
                    view.Show();
                    view.GameLoop();
                }
                if (logger.IsDebugEnabled) logger.Debug("Exiting application.");
            }
            catch (Exception ex)
            {
                if (logger.IsFatalEnabled) logger.Fatal("Fatal exception encountered.", ex);
            }
        }

        /// <summary>
        /// Builds the game logic user settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        private static void BuildGameLogicUserSettings(GameLogicUserSettings settings)
        {
            settings.UserPreferredDifficulty = Properties.Settings.Default.UserPreferenceDifficulty;
        }
    }
}
