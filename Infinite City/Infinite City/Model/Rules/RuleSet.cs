using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using InfiniteCity.Model.Enums;

namespace InfiniteCity.Model.Rules
{
    internal sealed class RuleSet<T> where T: RuleArguments
    {
        private readonly ISet<Rule<T>> _rules;

        public RuleSet(RuleTypes type) : this(type.ToString()) {}

        public RuleSet(string name)
        {
            IEnumerable<Rule<T>> rules =
                Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(Rule<T>))).Select(
                    t => Activator.CreateInstance(t) as Rule<T>).Where(r => string.Equals(name, r.Set));
            _rules = new HashSet<Rule<T>>(rules);
        }

        public IEnumerable<Rule<T>> Rules
        {
            get { return _rules; }
        }

        public bool CheckAction(T arguments, double authority = 0)
        {
            Rule<T> rule;
            return CheckAction(arguments, authority, out rule);
        }

        public bool CheckAction(T arguments, double authority, out Rule<T> rule)
        {
            rule =
                _rules.Where(r => r.Authority>authority && r.Check(arguments).HasValue).OrderByDescending(r => r.Authority).FirstOrDefault();
            if (rule == null)
                return true;
            // ReSharper disable PossibleInvalidOperationException
            return (bool)rule.Check(arguments);
            // ReSharper restore PossibleInvalidOperationException
        }
    }
}