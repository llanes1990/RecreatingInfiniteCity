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
    ///   Place one of your tokens on an empty tile.
    /// </summary>
    internal class Village : Tile
    {
        private readonly Selection[] _stateSequence = {Selection.TileFromBoard, Selection.None};
        private int _activeStateIndex = -1;

        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Village"; }
        }

        public override string Rules
        {
            get { return "Place one of your tokens on an empty tile."; }
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
                IEnumerable<Tile> tiles = Controller.DefaultInstance.Board.Tiles.Where(t => t.TokenCount() == 0);
                if (tiles.Count() == 0)
                    ++_activeStateIndex;
                return _stateSequence[_activeStateIndex];
            }

            if (input != null)
            {
                var tile = input as Tile;
                if (tile == null)
                    throw new ArgumentException();

                if (tile.TokenCount() == 0)
                {
                    tile.AddToken(Owner);
                    tile.Owner = Owner;
                    ++_activeStateIndex;
                }
                else
                    throw new InvalidOperationException("Cannot inhabit a tile which is already owned");

                if (!tile.IsFlipped)
                {
                    tile.IsFlipped = true;
                    return Controller.DefaultInstance.Activate(tile);
                }
                return _stateSequence[_activeStateIndex];
            }
            throw new UnknownTransitionException("Village");
        }

        internal override Exception TryValidateInput(object input)
        {
            var tile = (Tile)input;
            if (tile.TokenCount() != 0)
                return new InvalidOperationException("Cannot inhabit a tile which is already owned");
            return null;
        }
    }
}