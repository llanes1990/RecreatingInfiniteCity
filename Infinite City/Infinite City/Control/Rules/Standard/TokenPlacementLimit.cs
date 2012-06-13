using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Exceptions;
using InfiniteCity.Model.Rules;
using InfiniteCity.Model.Rules.Arguments;

namespace InfiniteCity.Control.Rules.Standard
{
    internal class TokenPlacementLimit : Rule<TokenMovementArguments>
    {
        public override double Authority
        {
            get { return 100.0; }
        }

        public override string Set
        {
            get { return RuleTypes.TilePlacement.ToString(); }
        }

        public override bool? Check(TokenMovementArguments arguments)
        {
            if (arguments.Actor.Tokens<=0)
                return false;
            return null;
        }

        public override UnsuccessfulActionException GetException()
        {
            const string message = "This player has no remaining tokens.";
            return new UnsuccessfulActionException(message);
        }
    }
}