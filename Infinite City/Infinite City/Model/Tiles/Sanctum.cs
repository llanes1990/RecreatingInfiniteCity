using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Exceptions;
using InfiniteCity.Model.Interfaces;

namespace InfiniteCity.Model.Tiles
{
    /// Sanctum - Library
    /// <summary>
    ///   Look at the top five tiles in the deck. Immediately play one that is not a Library,
    ///   then place the rest at the bottom of the deck.
    /// </summary>
    public class Sanctum : Tile
    {
        private const int DrawSize = 5;

        private readonly LinkedList<Tile> _drawnCards = new LinkedList<Tile>();
        private IEnumerable<ITile> _savedHand;

        public override string Building
        {
            get { return "Sanctum"; }
        }

        public override string Rules
        {
            get
            {
                return
                    "Look at the top five tiles in the deck. Immediately play one that is not a Sanctum, then place the rest at the bottom of the deck.";
            }
        }

        internal override State Transition(Controller.Controller control, params object[] input)
        {
            if (control.State == State.ResolveTilePlacement && input == null)
            {
                _savedHand = control.ActiveHand;
                //draw 5 cards from the deck and set as activehand to wait for user input
                for (int i = 0; i<DrawSize; i++)
                    _drawnCards.AddLast(control.Deck.Pop());
                control.ActiveHand = _drawnCards;

                return State.ChooseTileFromHand;
            }

            //after user input play the selected tile and return the rest
            var tile = input[0] as Tile;
            if (tile == null || !IsPlaced)
                throw new UnknownTransitionException("Sanctum");

            //place all other tile back in the deck.
            _drawnCards.Remove(tile);
            foreach (Tile tileInHand in _drawnCards)
                control.Deck.Enqueue(tileInHand);

            //and remember to revert the active hand back
            control.ActiveHand = _savedHand;

            //sets the state of the controller such that it'll play the new tile
            control.ActiveTile = tile;
            return State.PlaceTile;
        }
    }
}