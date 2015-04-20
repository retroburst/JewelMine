using JewelMine.Engine;
using JewelMine.Engine.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
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
        public static void Main()
        {
            //TODO: add more messages from game logic (e.g. difficulty change and last level)
            //TODO: look at using the timespan model to replace other tick counts if relevant
            log4net.Config.XmlConfigurator.Configure();
            try
            {
                if (logger.IsDebugEnabled) logger.Debug("Starting application.");
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                GameController controller = new GameController();
                controller.RunGame();
                if (logger.IsDebugEnabled) logger.Debug("Exiting application.");
                Application.Exit();
            }
            catch (Exception ex)
            {
                if (logger.IsFatalEnabled) logger.Fatal("Fatal exception encountered.", ex);
            }
        }

    }
}
