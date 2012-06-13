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
    internal class TilePlacementAdjacency : Rule<TileMovementArguments>
    {
        public override double Authority
        {
            get { return 100.0; }
        }

        public override string Set
        {
            get { return RuleTypes.TilePlacement.ToString(); }
        }

        public override bool? Check(TileMovementArguments arguments)
        {
            if (arguments.Space.AdjacentSpaces.All(s => s.Tile == null))
                return false;
            return null;
        }

        public override UnsuccessfulActionException GetException()
        {
            const string message = "Cannot place this tile here because it is not adjacent to any other tiles.";
            return new UnsuccessfulActionException(message);
        }
    }
}