using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Controller.Rules;
using InfiniteCity.Controller.Rules.Arguments;
using InfiniteCity.Model.Exceptions;

namespace InfiniteCity.Model.Tiles.Rules
{
    public class CrossroadsTilePlacement : Rule<TileMovementArguments>
    {
        public override double Authority
        {
            get { return 10.0; }
        }

        public override string Set
        {
            get { return RuleTypes.TilePlacement.ToString(); }
        }

        public override bool? Check(TileMovementArguments arguments)
        {
            if (arguments.Space.AdjacentSpaces.Any(s => s.Tile != null && s.Tile is Crossroads))
                return null; // If the tile is being placed next to a crossroads, don't interfere.
            if (arguments.Game.Board.Find<Crossroads>().Any(tile => tile.Space.AdjacentTiles.Count()<4))
                return false; // If there is a crossroads without four adjacent tiles, return false.
            return null;
        }

        public override UnsuccessfulActionException GetException(TileMovementArguments arguments)
        {
            const string message = "Cannot place this tile here because there is a crossroads in play without the required adjacencies.";
            return new UnsuccessfulActionException(message);
        }
    }
}