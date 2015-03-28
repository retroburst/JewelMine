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
            //TODO: add debug diagnostics bound to a key
            //TODO: massive refactor (add guard statements?)
            //TODO: add msbuild build script
            //TODO: bug allows delta to sit against other jewels indefinately when input control is sideways
            log4net.Config.XmlConfigurator.Configure();
            if (logger.IsDebugEnabled) logger.Debug("Starting application");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (GameView view = new GameView(new GameLogic()))
            {
                view.Show();
                view.GameLoop();
            }
            if (logger.IsDebugEnabled) logger.Debug("Exiting application");
        }
    }
}
