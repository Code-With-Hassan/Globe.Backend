using System;

namespace Globe.Shared.Exceptions
{
   public class UnauthorisedException : Exception
    {
        public UnauthorisedException(string message) : base(message) { }
    }
}