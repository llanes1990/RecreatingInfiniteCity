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
    ///   Place another tile on the board.
    /// </summary>
    internal class Quarry : Tile
    {
        private const double Authority = 0;
        private static readonly Selection[] States = new[] {Selection.TileFromHand, Selection.PlayableSpace};

        private int _state;
        private Tile _tile;

        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Quarry"; }
        }

        public override string Rules
        {
            get { return "Place another tile on the board."; }
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

        internal override Selection Revert()
        {
            if (_state>=0)
                _state--;
            return States[_state];
        }

        internal override Selection Skip()
        {
            return Selection.None;
        }

        internal override Selection Transition(object input)
        {
            Exception ex = TryValidateInput(input);
            if (ex != null)
                throw ex;

            switch (_state)
            {
                case 0:
                    return States[_state++];
                case 1:
                    _tile = input as Tile;
                    return States[_state++];
                case 2:
                    var space = input as Space;
                    Controller.DefaultInstance.PlaceTile(_tile, space, Authority);
                    return Controller.DefaultInstance.Activate(_tile);
            }
            throw new UnknownTransitionException();
        }

        internal override Exception TryValidateInput(object input)
        {
            switch (_state)
            {
                case 0:
                case 1:
                    return null;
                case 2:
                    return Controller.DefaultInstance.CanPlaceTile(_tile, input as Space, Authority);
            }
            throw new UnknownTransitionException();
        }
    }
}