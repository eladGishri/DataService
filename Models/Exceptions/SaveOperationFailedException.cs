using System;

namespace Models.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a save operation fails.
    /// </summary>
    public class SaveOperationFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveOperationFailedException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SaveOperationFailedException(string message) : base(message)
        {
        }
    }
}