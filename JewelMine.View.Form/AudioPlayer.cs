using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using System.IO;

namespace JewelMine.View.Forms
{
    public class AudioPlayer : IDisposable
    {
        private WaveOutEvent waveOutDevice = null;
        private Mp3FileReader mp3FileReader = null;

        public AudioPlayer()
        {
            
        }

        /// <summary>
        /// Plays the specified resource stream.
        /// </summary>
        /// <param name="resourceStream">The resource stream.</param>
        public void Play(Stream resourceStream)
        {
            Play(resourceStream, 1.0f);
        }

        /// <summary>
        /// Plays the specified resource stream.
        /// </summary>
        /// <param name="resourceStream">The resource stream.</param>
        /// <param name="volume">The volume.</param>
        public void Play(Stream resourceStream, float volume)
        {
            if (volume > 1.0f || volume < 0.0f) throw new ArgumentException("Argument must be between 0.0 and 1.0.", "volume");
            waveOutDevice = new WaveOutEvent();
            mp3FileReader = new Mp3FileReader(resourceStream);
            waveOutDevice.Init(mp3FileReader);
            waveOutDevice.Volume = volume;
            waveOutDevice.Play();
        }

        public void Stop()
        {

        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (waveOutDevice != null)
            {
                waveOutDevice.Stop();
            }
            if (mp3FileReader != null)
            {
                mp3FileReader.Dispose();
                mp3FileReader = null;
            }
            if (waveOutDevice != null)
            {
                waveOutDevice.Dispose();
                waveOutDevice = null;
            }
        }
    }
}
