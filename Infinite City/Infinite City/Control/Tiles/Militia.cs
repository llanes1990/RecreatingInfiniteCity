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
    ///   Take control of an adjacent tile. Return any tokens on it to their owners.
    /// </summary>
    internal class Militia : Tile
    {
        private static readonly Selection[] States = new[] {Selection.TileFromBoard, Selection.None};

        private int _state;

        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Militia"; }
        }

        public override string Rules
        {
            get { return "Take control of an adjacent tile. Return any tokens on it to their owners."; }
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
            throw new InvalidOperationException("Cannot skip this action");
        }

        internal override Selection Transition(object input)
        {
            Exception ex = TryValidateInput(input);
            if (ex != null)
                throw ex;

            switch (_state)
            {
                case 0:
                    // in this case, there are no valid tiles
                    if (!Space.AdjacentTiles.Any(t => t.Flags != Flags.Holy))
                        return Selection.None;
                    return States[_state++];
                case 1:
                    var tile = (Tile)input;
                    tile.RemoveAllTokens();
                    tile.AddToken(Owner);
                    tile.Owner = Owner;
                    if (!tile.IsFlipped)
                    {
                        tile.IsFlipped = true;
                        return Controller.DefaultInstance.Activate(tile);
                    }
                    return States[_state++];
            }
            throw new UnknownTransitionException();
        }

        internal override Exception TryValidateInput(object input)
        {
            switch (_state)
            {
                case 1:
                    var tile = (Tile)input;
                    if (tile.Flags == Flags.Holy && tile.IsFlipped)
                        return new ArgumentException("Cannot swap ownership with a holy site.");

                    return tile.Space.AdjacentSpaces.Contains(Space)
                               ? null
                               : new ArgumentException("Tile must be neighboring the Speaking Stone");
            }
            return null;
        }
    }
}