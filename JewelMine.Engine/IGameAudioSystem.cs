using JewelMine.Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.Engine
{
    /// <summary>
    /// Represents the contract for the
    /// game audio system component.
    /// </summary>
    public interface IGameAudioSystem : IDisposable
    {
        /// <summary>
        /// Plays sounds based on game logic.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        void PlaySounds(GameLogicUpdate logicUpdate);

        /// <summary>
        /// Plays the collision.
        /// </summary>
        void PlayCollision();

        /// <summary>
        /// Plays the swap.
        /// </summary>
        void PlaySwap();

        /// <summary>
        /// Plays the stationary.
        /// </summary>
        void PlayStationary();

        /// <summary>
        /// Plays the level up.
        /// </summary>
        void PlayLevelUp();

        /// <summary>
        /// Toggles the sound effects.
        /// </summary>
        void ToggleSoundEffects();

        /// <summary>
        /// Plays the background music loop.
        /// </summary>
        void PlayBackgroundMusicLoop();

        /// <summary>
        /// Toggles the background music loop.
        /// </summary>
        void ToggleBackgroundMusicLoop();

        /// <summary>
        /// Gets a value indicating whether [background music muted].
        /// </summary>
        /// <value>
        /// <c>true</c> if [background music muted]; otherwise, <c>false</c>.
        /// </value>
        bool BackgroundMusicMuted { get; set; }

        /// <summary>
        /// Gets a value indicating whether [sound effects muted].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [sound effects muted]; otherwise, <c>false</c>.
        /// </value>
        bool SoundEffectsMuted { get; set; }

        /// <summary>
        /// Adds the background music state message.
        /// </summary>
        /// <param name="addMessage">The add message.</param>
        void AddBackgroundMusicStateMessage(Action<string> addMessage);

        /// <summary>
        /// Adds the sound effects state message.
        /// </summary>
        /// <param name="addMessage">The add message.</param>
        void AddSoundEffectsStateMessage(Action<string> addMessage);
    }
}
