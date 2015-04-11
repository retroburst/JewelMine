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
    /// </summary>
    public static class Program
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Program));
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
            try
            {
                //TODO: massive refactor (add guard statements?)
                //TODO: add a special jewel connects any group collisions in any shape of the jewels around it
                //TODO: add a view for the coming up delta
                //TODO: text for music and sound toggles
                if (logger.IsDebugEnabled) logger.Debug("Starting application.");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                using (GameView view = new GameView(new GameLogic()))
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
    }
}
