using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Exceptions;

namespace InfiniteCity.Model.Rules
{
    public abstract class Rule<T> where T: RuleArguments
    {
        public abstract double Authority { get; }
        public abstract string Set { get; }
        public abstract bool? Check(T arguments);
        public abstract UnsuccessfulActionException GetException();
    }
}