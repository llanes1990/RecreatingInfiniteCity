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
    ///   Remove a tile from the board.
    /// </summary>
    internal class Wasteland : Tile
    {
        private readonly Selection[] _stateSequence = {Selection.TileFromBoard, Selection.None};
        private int _activeStateIndex = -1;

        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Wasteland"; }
        }

        public override string Rules
        {
            get { return "Remove a tile from the board."; }
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
                return _stateSequence[++_activeStateIndex];

            Exception ex = TryValidateInput(input);
            if (ex != null)
                throw ex;

            if (input != null)
            {
                var tile = input as Tile;
                if (tile == null)
                    throw new ArgumentException();

                Controller.DefaultInstance.RemoveTile(tile);
                tile.Space = null;
                return _stateSequence[++_activeStateIndex];
            }
            throw new UnknownTransitionException("Wasteland");
        }

        internal override Exception TryValidateInput(object input)
        {
            if (_activeStateIndex == 0)
            {
                var tile = (Tile)input;
                if (tile.Flags == Flags.Holy)
                    return new InvalidOperationException("You cannot move a holy site");
            }
            return null;
        }
    }
}