using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Exceptions;

namespace InfiniteCity.Model.Tiles
{
    public class Graveyard : Tile
    {
        public override string Building
        {
            get { return "Graveyard"; }
        }

        public override string Rules
        {
            get { return "Place an adjacent tile into your hand. Return any tokens on it to their owners."; }
        }

        internal override State Transition(Controller.Controller control, params object[] input)
        {
            if (control.State == State.ResolveTilePlacement && input == null)
            {
                //choose tile to pick up
                return State.ChooseTileFromBoard;
            }

            var tile = input[0] as Tile;
            if (tile != null && IsPlaced)
                //check the space is adjacent
                if (tile.Space.AdjacentSpaces.Contains(Space))
                {
                    tile.RemoveAllTokens();
                    PlacedBy.AddToHand(tile);
                    return State.NextTurn;
                }
                else //throw an exception to inform the user of bad placement
                    throw new ArgumentException("Graveyard: Tile must be next to the Graveyard");
            throw new UnknownTransitionException("Graveyard");
        }
    }
}