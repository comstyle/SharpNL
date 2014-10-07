using System;

namespace SharpNL.Utility {

    /// <summary>
    /// This exception indicates that a resource violates the expected data format.
    /// </summary>
    [Serializable]
    internal class InvalidFormatException : Exception {

        public InvalidFormatException() { }

        public InvalidFormatException(string message) : base(message) { }

        public InvalidFormatException(string message, Exception innerException) : base(message, innerException) { }

    }
}
