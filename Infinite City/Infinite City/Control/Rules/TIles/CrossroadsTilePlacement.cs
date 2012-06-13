using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Control.Tiles;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Exceptions;
using InfiniteCity.Model.Rules;
using InfiniteCity.Model.Rules.Arguments;

namespace InfiniteCity.Control.Rules.Tiles
{
    internal class CrossroadsTilePlacement : Rule<TileMovementArguments>
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
            if (arguments.Space.AdjacentSpaces.Any(s => s.Tile != null && s.Tile is Crossroads && s.Tile.IsFlipped))
                return null; // If the tile is being placed next to a crossroads, don't interfere.
            if (Controller.DefaultInstance.Board.Find<Crossroads>().Any(tile => tile.IsFlipped && tile.Space.AdjacentTiles.Count()<4))
                return false; // If there is a crossroads without four adjacent tiles, return false.
            return null;
        }

        public override UnsuccessfulActionException GetException()
        {
            const string message = "Cannot place this tile here because there is a crossroads in play without the required adjacencies.";
            return new UnsuccessfulActionException(message);
        }
    }
}