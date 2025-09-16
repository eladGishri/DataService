using System;

namespace Models.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when an update operation fails.
    /// </summary>
    public class UpdateOperationFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateOperationFailedException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UpdateOperationFailedException(string message) : base(message)
        {
        }
    }
}