using JewelMine.View.Forms.Audio;
using NAudio;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JewelMine.View.Forms
{
    /// <summary>
    /// Encapsulates the audio system for this game,
    /// wraps NAudio classes and manages resource streams.
    /// </summary>
    public class GameAudioSystem : IDisposable
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
            if (File.Exists(ViewConstants.SOUND_COLLISION_FILENAME)) collisionSound = new CachedSound(ViewConstants.SOUND_COLLISION_FILENAME);
            if (File.Exists(ViewConstants.SOUND_SWAP_FILENAME)) swapSound = new CachedSound(ViewConstants.SOUND_SWAP_FILENAME);
            if (File.Exists(ViewConstants.SOUND_STATIONARY_FILENAME)) stationarySound = new CachedSound(ViewConstants.SOUND_STATIONARY_FILENAME);
            if (File.Exists(ViewConstants.SOUND_LEVELUP_FILENAME)) levelUpSound = new CachedSound(ViewConstants.SOUND_LEVELUP_FILENAME);
            if (File.Exists(ViewConstants.BACKGROUND_MUSIC_FILENAME)) backgroundMusic = new LoopStream(new WaveFileReader(ViewConstants.BACKGROUND_MUSIC_FILENAME));
            audioPlayer = AudioPlaybackEngine.Instance;
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
        /// <exception cref="System.NotImplementedException"></exception>
        public void Dispose()
        {
            if (audioPlayer != null) audioPlayer.Dispose();
        }
    }
}
