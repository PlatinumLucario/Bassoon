using System;

namespace Bassoon
{
    /// <summary>
    /// Exceptions that come from using Bassoon
    /// </summary>
    public class BassoonException : Exception
    {
        /// <summary>
        /// Creates a new Bassoon error.
        /// </summary>
        public BassoonException() : base()
        {
        }

        /// <summary>
        /// Creates a new Bassoon error with a message attached.
        /// </summary>
        /// <param name="message">Message to send</param>
        public BassoonException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new Bassoon error with a message attached and an inner error.
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="inner">The exception that occured inside of this one</param>
        public BassoonException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
