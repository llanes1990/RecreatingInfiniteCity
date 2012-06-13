using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using InfiniteCity.Model.Interfaces;

namespace InfiniteCity.Control.AI
{
    public abstract class WeightedAI : AI, ILeveledAI
    {
        private static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>(() => new Random());

        private int _level;

        protected WeightedAI(int level)
        {
            Level = level;
        }

        public int Level
        {
            get
            {
                lock (_lock)
                    return _level;
            }
            set
            {
                if (value<1 || value>9)
                    throw new ArgumentOutOfRangeException("value", "Level must be between 1 and 9, inclusive.");
                lock (_lock)
                    _level = value;
            }
        }

        protected abstract IEnumerable<KeyValuePair<object, double>> GetWeightedInput();

        protected virtual double SkewWeight(double weight)
        {
            return Math.Pow(Math.Abs(weight), Level/3d+1)*Math.Sign(weight);
        }

        protected override object GetInput()
        {
            KeyValuePair<object, double>[] inputs = GetWeightedInput().ToArray();
            if (inputs.Length == 0)
                return null;
            if (inputs.All(kvp => kvp.Value == 0))
                return inputs.Random().Key;

            inputs = inputs.Select(kvp => new KeyValuePair<object, double>(kvp.Key, SkewWeight(kvp.Value))).ToArray();
            double min = inputs.Min(kvp => kvp.Value);
            if (min<0)
                inputs = inputs.Select(kvp => new KeyValuePair<object, double>(kvp.Key, kvp.Value-min+1)).ToArray();

            double index = Random.Value.NextDouble()*inputs.Sum(kvp => kvp.Value);
            foreach (KeyValuePair<object, double> kvp in inputs)
            {
                if (index<kvp.Value)
                    return kvp.Key;
                index -= kvp.Value;
            }
            throw new InvalidOperationException();
        }
    }
}