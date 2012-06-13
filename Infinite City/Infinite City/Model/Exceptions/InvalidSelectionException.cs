using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace InfiniteCity.Model.Exceptions
{
    internal class InvalidSelectionException : Exception
    {
        public InvalidSelectionException() {}
        public InvalidSelectionException(SerializationInfo info, StreamingContext context) : base(info, context) {}
        public InvalidSelectionException(string message) : base(message) {}
        public InvalidSelectionException(string message, Exception innerException) : base(message, innerException) {}
    }
}