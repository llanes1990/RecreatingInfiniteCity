using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Exceptions;

namespace InfiniteCity.Model.Tiles
{
    /// GuildHall - ConstructionSite
    /// <summary>
    ///   Place another tile
    /// </summary>
    public class GuildHall : Tile
    {
        private int _transitionCount;

        public override string Building
        {
            get { return "GuildHall"; }
        }

        public override string Rules
        {
            get { return string.Empty; }
        }

        internal override State Transition(Controller.Controller control, params object[] input)
        {
            _transitionCount++;
            if (control.State == State.ResolveTilePlacement && input == null && _transitionCount == 0)
            {
                //allow the player to choose a new tile
                return State.ChooseTileFromHand;
            }

            //after user input play the selected tile and return the rest
            if (input != null) {
                var tile = input[0] as Tile;
                if (tile != null && _transitionCount == 1)
                {
                    //sets the state of the controller such that it'll play the new tile
                    control.ActiveTile = tile;
                    return State.PlaceTile;
                }
            }

            throw new UnknownTransitionException();
        }
    }
}