// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;

namespace libsndfileSharp
{
    public class SndFileException : Exception
    {
        /// <summary>
        /// Creates a new SndFile error.
        /// </summary>
        public SndFileException() : base()
        {
        }

        /// <summary>
        /// Creates a new SndFile error with a message attached.
        /// </summary>
        /// <param name="message">Message to send</param>
        public SndFileException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new SndFile error with a message attached and an inner error.
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="inner">The exception that occured inside of this one</param>
        public SndFileException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
