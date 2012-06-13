using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model;
using InfiniteCity.Model.Enums;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteCity.Control.Tiles
{
    internal class Blank : Tile
    {
        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Blank"; }
        }

        public override string Rules
        {
            get { return string.Empty; }
        }

        /// <summary>
        ///   Gets the texture of this tile.
        /// </summary>
        public override Texture2D Texture
        {
            get { return TileTexture; }
        }

        /// <summary>
        ///   Resets the tile's active state.
        /// </summary>
        internal override void Reset() {}

        /// <summary>
        ///   Reverts the tile's active state.
        /// </summary>
        internal override Selection Revert()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///   Skips the current selection.
        /// </summary>
        internal override Selection Skip()
        {
            throw new NotImplementedException();
        }

        internal override Selection Transition(object input)
        {
            return Selection.None;
        }

        internal override Exception TryValidateInput(object input)
        {
            return new NotImplementedException();
        }
    }
}