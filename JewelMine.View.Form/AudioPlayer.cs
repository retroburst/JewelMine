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

        public void Play(Stream resourceStream)
        {
            waveOutDevice = new WaveOutEvent();
            mp3FileReader = new Mp3FileReader(resourceStream);
            waveOutDevice.Init(mp3FileReader);
            waveOutDevice.Volume = 0.25f;
            
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
