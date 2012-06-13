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
    ///   Swap the location of two tiles.
    /// </summary>
    internal class AbandonedMine : Tile
    {
        private readonly Selection[] _stateSequence = {Selection.TileFromBoard, Selection.TileFromBoard, Selection.None};
        private int _activeStateIndex = -1;

        private Tile _tile1;
        private Tile _tile2;

        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Abandoned Mine"; }
        }

        public override string Rules
        {
            get { return "Swap the location of two tiles."; }
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

            Exception e = TryValidateInput(input);
            if (e != null)
                throw e;

            if (input != null)
                switch (_activeStateIndex)
                {
                    case 0:
                    {
                        var tile = input as Tile;
                        if (tile == null)
                            throw new ArgumentException();

                        _tile1 = tile;
                        return _stateSequence[++_activeStateIndex];
                    }
                    case 1:
                    {
                        var tile = input as Tile;
                        if (tile == null)
                            throw new ArgumentException();

                        _tile2 = tile;

                        Space temp1 = _tile1.Space;
                        Space temp2 = _tile2.Space;
                        _tile1.Space = temp2;
                        _tile2.Space = temp1;
                        temp2.Tile = _tile1;
                        temp1.Tile = _tile2;
                        return _stateSequence[++_activeStateIndex];
                    }
                }
            throw new UnknownTransitionException("Abandoned Mine");
        }

        internal override Exception TryValidateInput(object input)
        {
            if (_activeStateIndex == 0 || _activeStateIndex == 1)
            {
                var tile = (Tile)input;
                if (tile.Flags == Flags.Holy)
                    return new InvalidOperationException("You cannot move a holy site");
            }
            return null;
        }
    }
}