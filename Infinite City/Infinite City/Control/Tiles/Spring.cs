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
    ///   Place a flipped over tile on a playable space.
    /// </summary>
    internal class Spring : Tile
    {
        private readonly Selection[] _stateSequence = {Selection.PlayableSpace, Selection.None};
        private int _activeStateIndex = -1;

        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Spring"; }
        }

        public override string Rules
        {
            get { return "Place a flipped over tile on a playable space."; }
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

            if (input != null)
            {
                var space = input as Space;
                if (space == null)
                    throw new ArgumentException();

                Tile tile = Controller.DefaultInstance.Deck.Pop();
                space.Tile = tile;
                tile.Space = space;
                tile.Tokens.Clear();
                tile.Owner = null;

                if (space.AdjacentTiles.Any(t => t.Building == "Sanctum"))
                    tile.Flags = Flags.Holy;

                return _stateSequence[++_activeStateIndex];
            }
            throw new UnknownTransitionException("Spring");
        }

        internal override Exception TryValidateInput(object input)
        {
            var space = (Space)input;
            return space.Tile != null ? new InvalidOperationException("Cannot place a tile on a space which already has a tile") : null;
        }
    }
}