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
            //TODO: stop changing background on level change but highlight level change on view
            //TODO: re-position text score left hand and level right hand
            //TODO: add debug diagnostics bound to a key
            //TODO: massive refactor
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
