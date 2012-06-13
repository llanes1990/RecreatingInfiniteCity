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
    ///   Move a tile to a new location.
    /// </summary>
    internal class ReflectingPond : Tile
    {
        private readonly Selection[] _stateSequence = {Selection.TileFromBoard, Selection.PlayableSpace, Selection.None};
        private int _activeStateIndex = -1;

        private Tile _tile;

        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Reflecting Pond"; }
        }

        public override string Rules
        {
            get { return "Move a tile to a new location."; }
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
                switch (_activeStateIndex)
                {
                    case 0:
                    {
                        var tile = input as Tile;
                        if (tile == null)
                            throw new ArgumentException();

                        _tile = tile;
                        return _stateSequence[++_activeStateIndex];
                    }
                    case 1:
                    {
                        var space = input as Space;
                        if (space == null)
                            throw new ArgumentException();

                        _tile.Space.Tile = null;
                        _tile.Space = space;
                        space.Tile = _tile;

                        if (space.AdjacentTiles.Any(t => t.Building == "Sanctum"))
                            _tile.Flags = Flags.Holy;

                        return _stateSequence[++_activeStateIndex];
                    }
                }
            throw new UnknownTransitionException("Reflecting Pond");
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