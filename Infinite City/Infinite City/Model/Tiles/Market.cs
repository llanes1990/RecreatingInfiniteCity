using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Exceptions;

namespace InfiniteCity.Model.Tiles
{
    public class Market : Tile
    {
        public override string Building
        {
            get { return "Market"; }
        }

        public override string Rules
        {
            get { return "Swap hands with another player."; }
        }

        internal override State Transition(Controller.Controller control, params object[] input)
        {
            if (control.State == State.ResolveTilePlacement && input == null)
            {
                return State.ChooseOpponent;
            }

            if (!IsPlaced)
                throw new UnknownTransitionException("Market");

            var player = input[0] as Player;
            if (player == null || player == PlacedBy)
                throw new ArgumentException("Market: Invalid player swap");

            Player.SwapHands(PlacedBy, player);
            return State.NextTurn;
        }
    }
}