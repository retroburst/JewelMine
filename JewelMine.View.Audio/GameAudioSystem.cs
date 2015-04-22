using JewelMine.Engine;
using JewelMine.Engine.Models;
using JewelMine.View.Audio.NAudio;
using NAudio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JewelMine.View.Audio
{
    /// <summary>
    /// Encapsulates the audio system for this game,
    /// wraps NAudio classes and manages resource streams.
    /// </summary>
    public class GameAudioSystem : IGameAudioSystem, IDisposable
    {
        private AudioPlaybackEngine audioPlayer = null;
        private CachedSound swapSound = null;
        private CachedSound collisionSound = null;
        private CachedSound stationarySound = null;
        private CachedSound levelUpSound = null;
        private LoopStream backgroundMusic = null;
        private bool backgroundMusicMuted = false;
        private bool soundEffectsMuted = false;
        private static GameAudioSystem instance = null;

        /// <summary>
        /// Prevents a default instance of the <see cref="GameAudioSystem"/> class from being created.
        /// </summary>
        private GameAudioSystem()
        {
            if (File.Exists(AudioConstants.SOUND_COLLISION_FILENAME)) collisionSound = new CachedSound(AudioConstants.SOUND_COLLISION_FILENAME);
            if (File.Exists(AudioConstants.SOUND_SWAP_FILENAME)) swapSound = new CachedSound(AudioConstants.SOUND_SWAP_FILENAME);
            if (File.Exists(AudioConstants.SOUND_STATIONARY_FILENAME)) stationarySound = new CachedSound(AudioConstants.SOUND_STATIONARY_FILENAME);
            if (File.Exists(AudioConstants.SOUND_LEVELUP_FILENAME)) levelUpSound = new CachedSound(AudioConstants.SOUND_LEVELUP_FILENAME);
            if (File.Exists(AudioConstants.BACKGROUND_MUSIC_FILENAME)) backgroundMusic = new LoopStream(new WaveFileReader(AudioConstants.BACKGROUND_MUSIC_FILENAME));
            audioPlayer = AudioPlaybackEngine.Instance;
        }

        /// <summary>
        /// Plays the sounds.
        /// </summary>
        /// <param name="logicUpdate">The logic update.</param>
        public void PlaySounds(GameLogicUpdate logicUpdate)
        {
            if (logicUpdate.FinalisedCollisions.Count > 0) PlayCollision();
            if (logicUpdate.DeltaJewelsSwapped) PlaySwap();
            if (logicUpdate.DeltaStationary) PlayStationary();
            if (logicUpdate.LevelIncremented) PlayLevelUp();
        }

        /// <summary>
        /// Plays the collision sound.
        /// </summary>
        public void PlayCollision()
        {
            if (collisionSound != null && !soundEffectsMuted) audioPlayer.PlaySound(collisionSound);
        }

        /// <summary>
        /// Plays the swap sound.
        /// </summary>
        public void PlaySwap()
        {
            if (swapSound != null && !soundEffectsMuted) audioPlayer.PlaySound(swapSound);
        }

        /// <summary>
        /// Plays the stationary sound.
        /// </summary>
        public void PlayStationary()
        {
            if (stationarySound != null && !soundEffectsMuted) audioPlayer.PlaySound(stationarySound);
        }

        /// <summary>
        /// Plays the level up.
        /// </summary>
        public void PlayLevelUp()
        {
            if (levelUpSound != null && !soundEffectsMuted) audioPlayer.PlaySound(levelUpSound);
        }

        /// <summary>
        /// Toggles the sound effects.
        /// </summary>
        public void ToggleSoundEffects()
        {
            soundEffectsMuted = !soundEffectsMuted;
        }

        /// <summary>
        /// Plays the background music loop.
        /// </summary>
        public void PlayBackgroundMusicLoop()
        {
            if (backgroundMusic != null && !backgroundMusicMuted) audioPlayer.PlaySound(backgroundMusic);
        }

        /// <summary>
        /// Plays the background music loop.
        /// </summary>
        public void ToggleBackgroundMusicLoop()
        {
            if (backgroundMusicMuted)
            {
                if (backgroundMusic != null)
                {
                    audioPlayer.PlaySound(backgroundMusic);
                    backgroundMusicMuted = false;
                }
            }
            else
            {
                if (backgroundMusic != null)
                {
                    audioPlayer.MuteSound(backgroundMusic);
                    backgroundMusicMuted = true;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether [background music muted].
        /// </summary>
        /// <value>
        /// <c>true</c> if [background music muted]; otherwise, <c>false</c>.
        /// </value>
        public bool BackgroundMusicMuted 
        { 
            get 
            { 
                return(backgroundMusicMuted); 
            }
            set
            {
                backgroundMusicMuted = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [sound effects muted].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [sound effects muted]; otherwise, <c>false</c>.
        /// </value>
        public bool SoundEffectsMuted
        {
            get
            {
                return (soundEffectsMuted);
            }
            set
            {
                soundEffectsMuted = value;
            }
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static GameAudioSystem Instance
        {
            get
            {
                if (instance == null) instance = new GameAudioSystem();
                return (instance);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (audioPlayer != null) audioPlayer.Dispose();
        }

        /// <summary>
        /// Adds the background music state message.
        /// </summary>
        /// <param name="addMessage">The add message.</param>
        public void AddBackgroundMusicStateMessage(Action<string> addMessage)
        {
            string message = string.Format(AudioConstants.GAME_MESSAGE_TOGGLE_MUSIC_PATTERN, GameHelpers.EncodeBooleanForDisplay(!BackgroundMusicMuted));
            addMessage(message);
        }

        /// <summary>
        /// Adds the sound effects state message.
        /// </summary>
        /// <param name="addMessage">The add message.</param>
        public void AddSoundEffectsStateMessage(Action<string> addMessage)
        {
            string message = string.Format(AudioConstants.GAME_TOGGLE_SOUND_PATTERN, GameHelpers.EncodeBooleanForDisplay(!SoundEffectsMuted));
            addMessage(message);
        }
    }
}
