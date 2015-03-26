using JewelMine.Engine;
using JewelMine.Engine.Models;
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
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //TODO: add some walls as the levels increase or remove walls from game
            //TODO: show game pause / game over / game won -- text when occurs
            //TODO: add debug diagnostics bound to a key
            //TODO: add logging
            //TODO: add sound for delta stationary event
            //TODO: massive refactor (add guard statements?)
            //TODO: add msbuild build script

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (GameView view = new GameView(new GameLogic()))
            {
                view.Show();
                view.GameLoop();
            }
        }
    }
}
