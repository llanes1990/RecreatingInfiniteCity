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
    ///   Look at an opponent's hand and play one of their tiles.
    /// </summary>
    internal class ThievesGuild : Tile
    {
        private const double Authority = 0;

        private static readonly Selection[] States = new[]
            {Selection.Opponent, Selection.TileFromCustom, Selection.PlayableSpace, Selection.None};

        private int _state = -1;
        private Tile _tile;
        private Player _player;

        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Thieves Guild"; }
        }

        public override string Rules
        {
            get { return "Look at an opponent's hand and play one of their tiles."; }
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
            _state = -1;
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

            if (input == null && _state == -1)
                return States[++_state];

            Exception ex = TryValidateInput(input);
            if (ex != null)
                throw ex;

            switch (_state)
            {
                case 0:
                    _player = (Player)input;
                    if (_player != null)
                        control.TileSet = _player.Hand.ToList();
                    return States[++_state];
                case 1:
                    _tile = input as Tile;
                    _player.RemoveFromHand(_tile);
                    _player.AddToHand(control.Deck.Pop());

                    return States[++_state];
                case 2:
                    var space = input as Space;
                    control.PlaceTile(_tile, space, Authority);
                    ++_state;
                    return Controller.DefaultInstance.Activate(_tile);
            }
            throw new UnknownTransitionException("Thieves Guild");
        }

        internal override Exception TryValidateInput(object input)
        {
            return null;
        }
    }
}