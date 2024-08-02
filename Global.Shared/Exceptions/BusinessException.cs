using System;

namespace Globe.Shared.Exceptions
{
    /// <summary>
    /// BussinessException: this type of exceptions are logged as warning
    /// </summary>
    public class BusinessException : Exception
    {
       
        public BusinessException(string message) : base(message) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message">Main message. Shown to user</param>
        /// <param name="secondaryMessage">Secondary message. Used for logging.</param>
        public BusinessException(string message, string secondaryMessage) : base(message) {
            this.SecondaryMessage = SecondaryMessage;
        }

        /// <summary>
        /// Secondary message. Used for logging.
        /// </summary>
        public string SecondaryMessage { get; set; }
    }
}
