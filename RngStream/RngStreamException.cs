using System;
using System.Collections.Generic;
using System.Text;

namespace RngStreams
{
    /// <summary>
    /// Exception class for exceptions raised by the RngStream random number generator
    /// </summary>
    public class RngStreamException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Message">The message associated with the exception</param>
        public RngStreamException(string Message)
            : base(Message)
        {
            // Does nothing here
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Message">The message associated with the exception</param>
        /// <param name="innerException">The inner exception assocviated with this exception</param>
        public RngStreamException(string Message, Exception innerException)
            : base(Message, innerException)
        {
            // Does nothing here
        }
    }
}
