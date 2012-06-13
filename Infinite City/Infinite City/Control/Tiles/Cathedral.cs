using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Exceptions;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteCity.Control.Tiles
{
    /// <summary>
    ///   Look at the top five tiles in the deck. Immediately play one that is not a Cathedral, then place the rest at the bottom of the deck.
    /// </summary>
    internal class Cathedral : Tile
    {
        private const double Authority = 0;
        private const int Drawsize = 5;
        private static readonly Selection[] States = new[] {Selection.TileFromCustom, Selection.PlayableSpace};
        private readonly LinkedList<Tile> _drawnCards = new LinkedList<Tile>();

        private int _state;
        private Tile _tile;

        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Cathedral"; }
        }

        public override string Rules
        {
            get
            {
                return
                    "Look at the top five tiles in the deck. Immediately play one that is not a Cathedral, then place the rest at the bottom of the deck.";
            }
        }

        /// <summary>
        ///   Gets the texture of this tile.
        /// </summary>
        public override Texture2D Texture
        {
            get { return TileTexture; }
        }

        internal override void Reset()
        {
            _state = 0;
        }

        /// <summary>
        ///   Reverts the tile's active state.
        /// </summary>
        internal override Selection Revert()
        {
            _state = _state == 0 ? _state : _state = 1;
            return States[_state];
        }

        /// <summary>
        ///   Skips the current selection.
        /// </summary>
        internal override Selection Skip()
        {
            throw new InvalidOperationException("Cannot skip this action");
        }

        internal override Selection Transition(object input)
        {
            Controller control = Controller.DefaultInstance;

            Exception ex = TryValidateInput(input);
            if (ex != null)
                throw ex;

            switch (_state)
            {
                case 0:
                    //draw 5 cards from the deck and set as activehand to wait for user input
                    for (int i = 0; i<Drawsize; i++)
                        _drawnCards.AddLast(control.Deck.Pop());
                    control.TileSet = _drawnCards.ToList();
                    return States[_state++];
                case 1:
                    _tile = (Tile)input;
                    if (_tile.Building == "Cathedral")
                        throw new InvalidOperationException("Cannot place Sanctum from within a Sanctum");
                    _drawnCards.Remove(_tile);
                    foreach (Tile tileInHand in _drawnCards)
                        control.Deck.Enqueue(tileInHand);

                    return States[_state++];
                case 2:
                    var space = input as Space;
                    control.PlaceTile(_tile, space, Authority);
                    return Controller.DefaultInstance.Activate(_tile);
            }
            throw new UnknownTransitionException();
        }

        internal override Exception TryValidateInput(object input)
        {
            switch (_state)
            {
                case 1:
                    var tile = input as Tile;
                    if (tile == null || tile.Building == "Cathedral")
                        return new InvalidOperationException("Cannot place Cathedral from within a Cathedral");
                    break;
            }
            return null;
        }
    }
}