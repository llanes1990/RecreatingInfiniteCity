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
    ///   Remove all tokens from a tile on the board.
    /// </summary>
    internal class Plague : Tile
    {
        private readonly Selection[] _stateSequence = {Selection.TileFromBoard, Selection.None};
        private int _activeStateIndex = -1;

        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Plague"; }
        }

        public override string Rules
        {
            get { return "Remove all tokens from a tile on the board."; }
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
            throw new InvalidOperationException("Cannot skip this action");
        }

        internal override Selection Transition(object input)
        {
            if (input == null && _activeStateIndex == -1)
            {
                ++_activeStateIndex;
                IEnumerable<Tile> tiles = Controller.DefaultInstance.Board.Tiles.Where(t => t.TokenCount() != 0);
                if (tiles.Count() == 0)
                    ++_activeStateIndex;
                return _stateSequence[_activeStateIndex];
            }

            Exception ex = TryValidateInput(input);
            if (ex != null)
                throw ex;

            if (input != null)
            {
                var tile = input as Tile;
                if (tile == null)
                    throw new ArgumentException();

                if (tile.TokenCount() != 0)
                {
                    tile.RemoveAllTokens();
                    tile.Owner = null;
                    return _stateSequence[++_activeStateIndex];
                }
                throw new InvalidOperationException("You cannot wipe out a tile with no owners");
            }
            throw new UnknownTransitionException("Plague");
        }

        internal override Exception TryValidateInput(object input)
        {
            var tile = (Tile)input;
            if (tile.TokenCount() == 0)
                return new InvalidOperationException("You cannot wipe out a tile with no owners");
            if (tile.Flags == Flags.Holy)
                return new InvalidOperationException("You cannot wipe out a holy site");
            return null;
        }
    }
}