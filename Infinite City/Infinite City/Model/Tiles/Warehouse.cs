using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Exceptions;

namespace InfiniteCity.Model.Tiles
{
    public class Warehouse : Tile
    {
        private const int DrawSize = 5;

        public override string Building
        {
            get { return "Warehouse"; }
        }

        public override string Rules
        {
            get { return "After filling your hand this turn, draw and keep five more tiles."; }
        }

        internal override State Transition(Controller.Controller control, params object[] input)
        {
            if (!IsPlaced)
                throw new UnknownTransitionException("Warehouse");

            for (int i = 0; i<DrawSize; i++)
                PlacedBy.AddToHand(control.Deck.Pop());

            return State.NextTurn;
        }
    }
}