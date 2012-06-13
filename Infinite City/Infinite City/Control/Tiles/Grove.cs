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
    ///   You may return up to three of your tokens from your tiles to your hand.
    /// </summary>
    internal class Grove : Tile
    {
        private readonly Selection[] _stateSequence = {
                                                          Selection.TileFromBoard, Selection.TileFromBoard, Selection.TileFromBoard,
                                                          Selection.None,
                                                      };

        private int _activeStateIndex = -1;

        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Grove"; }
        }

        public override string Rules
        {
            get { return "You may return up to three of your tokens from your tiles to your hand."; }
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
            _activeStateIndex = -1;
        }

        /// <summary>
        ///   Reverts the tile's active state.
        /// </summary>
        internal override Selection Revert()
        {
            _activeStateIndex = _activeStateIndex == 0 ? _activeStateIndex : _activeStateIndex = 1;
            return _stateSequence[_activeStateIndex];
        }

        internal override Selection Skip()
        {
            return Selection.None;
        }

        internal override Selection Transition(object input)
        {
            //if input is cancel jump to completed
            //else 
            //do remove and increment
            if (input == null && _activeStateIndex == -1)
                return _stateSequence[++_activeStateIndex];

            if (input != null)
            {
                var tile = input as Tile;

                if (tile == null || !tile.HasToken(Controller.DefaultInstance.ActivePlayer))
                    throw new ArgumentException("Choose a tile that you own");

                tile.RemoveToken(Controller.DefaultInstance.ActivePlayer);
                _activeStateIndex++;

                // if all tiles have been removed, stop
                if (Controller.DefaultInstance.Board.Tiles.Where(t => t.Tokens.ContainsKey(Owner)).Count() == 0)
                    _activeStateIndex = 3;

                return _stateSequence[_activeStateIndex];
            }
            throw new UnknownTransitionException("Grove");
        }

        internal override Exception TryValidateInput(object input)
        {
            var tile = input as Tile;
            if (tile == null || !tile.HasToken(Owner))
                return new ArgumentException("Choose a tile that you own");
            if (tile.Flags == Flags.Holy)
                return new ArgumentException("Cannot swap ownership with a holy site.");
            return null;
        }
    }
}