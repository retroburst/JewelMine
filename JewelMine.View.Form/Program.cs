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
            //DONE: user input -> keyboard buffer and logic for movement 
            //TODO: collisions -> detecting collisions in logic and sending in update to view
            //TODO: add some initial jewels to start level randomly 
            //      (need smarts to make sure no initial collisions and walls do not impair gameplay totally)
            //      (with walls, only allow up to 50% of width and 20% of height)
            //      (with jewels need to check surrounding similar to collision checks)
            //TODO: level win and transition to new level including increasing difficulty - increases speed and adds walls
            //TODO: game over
            //TODO: add music and sound for swaps and placement, supposed example below for NAudio
            /*
             * reader = new Mp3FileReader("test.mp3");
                var waveOut = new WaveOut(); // or WaveOutEvent()
                waveOut.Init(reader); 
                waveOut.Play();
            */


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // TODO inject the logic into the view
            using (GameView view = new GameView(new GameLogic()))
            {
                view.Show();
                view.GameLoop();
            }
        }
    }
}
