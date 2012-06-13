using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace InfiniteCity.Model.Exceptions
{
    internal class UnknownTransitionException : Exception
    {
        public UnknownTransitionException() {}
        public UnknownTransitionException(SerializationInfo info, StreamingContext context) : base(info, context) {}
        public UnknownTransitionException(string message) : base(message) {}
        public UnknownTransitionException(string message, Exception innerException) : base(message, innerException) {}
    }
}