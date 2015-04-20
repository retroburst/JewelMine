using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace JewelMine.View.Forms.Audio
{
    /// <summary>
    /// File reader for NAudio that auto diposes
    /// after read is complete.
    /// </summary>
    public class AutoDisposeFileReader : ISampleProvider
    {
        private readonly AudioFileReader reader;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoDisposeFileReader"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public AutoDisposeFileReader(AudioFileReader reader)
        {
            this.reader = reader;
            this.WaveFormat = reader.WaveFormat;
        }

        /// <summary>
        /// Fill the specified buffer with 32 bit floating point samples
        /// </summary>
        /// <param name="buffer">The buffer to fill with samples.</param>
        /// <param name="offset">Offset into buffer</param>
        /// <param name="count">The number of samples to read</param>
        /// <returns>
        /// the number of samples written to the buffer.
        /// </returns>
        public int Read(float[] buffer, int offset, int count)
        {
            if (isDisposed)
                return 0;
            int read = reader.Read(buffer, offset, count);
            if (read == 0)
            {
                reader.Dispose();
                isDisposed = true;
            }
            return read;
        }

        /// <summary>
        /// Gets the WaveFormat of this Sample Provider.
        /// </summary>
        /// <value>
        /// The wave format.
        /// </value>
        public WaveFormat WaveFormat { get; private set; }
    }
}

