using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Interfaces;

namespace InfiniteCity.Model.Rules.Arguments
{
    public class TokenMovementArguments : RuleArguments
    {
        public IPlayer Owner { get; set; }
        public ITile Tile { get; set; }
    }
}