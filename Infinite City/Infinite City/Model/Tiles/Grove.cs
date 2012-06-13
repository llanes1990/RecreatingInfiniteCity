using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Exceptions;

namespace InfiniteCity.Model.Tiles
{
    /// Grove - Aboretum
    /// <summary>
    ///   You may return up to three of your tokens to from your tiles to your hand.
    /// </summary>
    public class Grove : Tile
    {
        private readonly State[] _stateSequence = {
                                                      State.ChooseTileFromBoard, State.ChooseTileFromBoard, State.ChooseTileFromBoard,
                                                      State.NextTurn
                                                  };

        private int _activeStateIndex = -1;

        public override string Building
        {
            get { return "Grove"; }
        }

        public override string Rules
        {
            get { return "You may return up to three of your tokens from your tiles to your hand."; }
        }

        internal override State Transition(Controller.Controller control, params object[] input)
        {
            //if input is cancel jump to completed
            //else 
            //do remove and increment
            if (control.State == State.ResolveTilePlacement && input == null && _activeStateIndex == -1)
            {
                return _stateSequence[++_activeStateIndex];
            }

            if (input != null) {
                var tile = input[0] as Tile;
                if (tile == null)
                    throw new ArgumentException();

                tile.RemoveToken(tile.PlacedBy);
            _activeStateIndex++;
            return _stateSequence[_activeStateIndex];
            }
            throw new UnknownTransitionException("Grove");
        }
    }
}