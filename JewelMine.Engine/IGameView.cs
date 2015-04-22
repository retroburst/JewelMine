using JewelMine.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine
{
    /// <summary>
    /// Represents a game view contract.
    /// </summary>
    public interface IGameView
    {
        /// <summary>
        /// Initialises the view.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="audioSystem">The audio system.</param>
        void InitialiseView(IGameStateProvider provider, IGameAudioSystem audioSystem);

        /// <summary>
        /// Re-initialises the view. Used when changing
        /// the mine structure (columns and depth).
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="audioSystem">The audio system.</param>
        void ReInitialiseView(IGameStateProvider provider, IGameAudioSystem audioSystem);

        /// <summary>
        /// Updates the view based on game logic.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        void UpdateView(GameLogicUpdate logicUpdate);

        /// <summary>
        /// Updates the view layout. Used when the view's
        /// window is resized by the user or programmatically.
        /// </summary>
        void UpdateViewLayout();

        /// <summary>
        /// Adds a game information message for display
        /// on the game view.
        /// </summary>
        /// <param name="message">The message.</param>
        void AddGameInformationMessage(string message);

        /// <summary>
        /// Toggles the debug information.
        /// </summary>
        void ToggleDebugInfo();
    }
}
