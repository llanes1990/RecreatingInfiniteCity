using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace InfiniteCity.Model.Exceptions
{
    public class UnsuccessfulActionException : Exception
    {
        public UnsuccessfulActionException() {}
        public UnsuccessfulActionException(SerializationInfo info, StreamingContext context) : base(info, context) {}
        public UnsuccessfulActionException(string message) : base(message) {}
        public UnsuccessfulActionException(string message, Exception innerException) : base(message, innerException) {}
    }
}