using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Control;
using InfiniteCity.Model.Interfaces;

namespace InfiniteCity.Model.Rules
{
    public class RuleArguments
    {
        public RuleArguments()
        {
            Actor = Controller.DefaultInstance.ActivePlayer;
        }

        public IPlayer Actor { get; set; }
    }
}