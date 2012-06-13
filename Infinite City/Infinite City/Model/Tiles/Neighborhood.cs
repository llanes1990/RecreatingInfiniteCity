using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Exceptions;

namespace InfiniteCity.Model.Tiles
{
    public class Neighborhood : Tile
    {
        private readonly State[] _stateSequence = {State.ChooseTileFromBoard, State.NextTurn};
        private int _activeStateIndex = -1;

        public override string Building
        {
            get { return "Neighborhood"; }
        }

        public override string Rules
        {
            get { return "Place one of your tokens on an empty tile"; }
        }

        internal override State Transition(Controller.Controller control, params object[] input)
        {
            if (control.State == State.ResolveTilePlacement && input == null && _activeStateIndex == -1)
            {
                return _stateSequence[++_activeStateIndex];
            }

            IEnumerable<Tile> tiles = control.Board.Tiles.Where(t => t.TokenCount == 0);
            if (tiles.Count() == 0)
            {
                control.DrawHand(PlacedBy);
                return _stateSequence[++_activeStateIndex];
            }

            if (input != null) {
                var tile = input[0] as Tile;
                if (tile == null)
                    throw new ArgumentException();

                if (tile.TokenCount == 0)
                {
                    tile.AddToken(PlacedBy);
                    ++_activeStateIndex;
                    control.DrawHand(PlacedBy);
                }
            return _stateSequence[_activeStateIndex];
            }
            throw new UnknownTransitionException("Neighborhood");
        }
    }
}