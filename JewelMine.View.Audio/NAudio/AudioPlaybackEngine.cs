using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace JewelMine.View.Audio.NAudio
{
    /// <summary>
    /// Wraps the NAudio sound output functionality,
    /// allows mixing of sounds and fire and forget
    /// playback.
    /// </summary>
    public class AudioPlaybackEngine : IDisposable
    {
        private readonly IWavePlayer outputDevice = null;
        private readonly MixingSampleProvider mixer = null;
        private Dictionary<LoopStream, WaveChannel32> loopChannels = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioPlaybackEngine"/> class.
        /// </summary>
        /// <param name="sampleRate">The sample rate.</param>
        /// <param name="channelCount">The channel count.</param>
        public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2)
        {
            loopChannels = new Dictionary<LoopStream, WaveChannel32>();
            outputDevice = new WaveOutEvent();
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
            mixer.ReadFully = true;
            outputDevice.Init(mixer);
            outputDevice.Play();
        }

        /// <summary>
        /// Plays the sound.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void PlaySound(string fileName)
        {
            var input = new AudioFileReader(fileName);
            AddMixerInput(new AutoDisposeFileReader(input));
        }

        /// <summary>
        /// Plays the sound.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void PlaySound(LoopStream stream)
        {
            if(loopChannels.ContainsKey(stream))
            {
                loopChannels[stream].Volume = 1.0f;
            }
            else
            {
                loopChannels.Add(stream, new WaveChannel32(stream, 1.0f, 0.0f));
                mixer.AddMixerInput(loopChannels[stream]);
            }
        }

        /// <summary>
        /// Mutes the sound.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void MuteSound(LoopStream stream)
        {
            if (loopChannels.ContainsKey(stream))
            {
                loopChannels[stream].Volume = 0.0f;
            }
        }

        /// <summary>
        /// Converts to right channel count.
        /// </summary>
        /// <param name="logicInput">The logicInput.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException">Not yet implemented this channel count conversion</exception>
        private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
        {
            if (input.WaveFormat.Channels == mixer.WaveFormat.Channels)
            {
                return input;
            }
            if (input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2)
            {
                return new MonoToStereoSampleProvider(input);
            }
            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }

        /// <summary>
        /// Plays the sound.
        /// </summary>
        /// <param name="sound">The sound.</param>
        public void PlaySound(CachedSound sound)
        {
            AddMixerInput(new CachedSoundSampleProvider(sound));
        }

        /// <summary>
        /// Adds the mixer logicInput.
        /// </summary>
        /// <param name="logicInput">The logicInput.</param>
        private void AddMixerInput(ISampleProvider input)
        {
            mixer.AddMixerInput(ConvertToRightChannelCount(input));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            outputDevice.Dispose();
        }

        /// <summary>
        /// The instance.
        /// </summary>
        public static readonly AudioPlaybackEngine Instance = new AudioPlaybackEngine(44100, 2);
    }
}
