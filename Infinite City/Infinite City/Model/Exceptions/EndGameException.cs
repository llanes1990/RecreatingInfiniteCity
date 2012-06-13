using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace InfiniteCity.Model.Exceptions
{
    internal class EndGameException : Exception
    {
        public EndGameException() {}
        public EndGameException(SerializationInfo info, StreamingContext context) : base(info, context) {}
        public EndGameException(string message) : base(message) {}
        public EndGameException(string message, Exception innerException) : base(message, innerException) {}
    }
}