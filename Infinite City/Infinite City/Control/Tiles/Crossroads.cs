using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model;
using InfiniteCity.Model.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteCity.Control.Tiles
{
    /// <summary>
    ///   All tiles must be placed adjacent to a Crossroads tiles as long as any Crossroads tile in play has less than four adjacent tiles.
    /// </summary>
    internal class Crossroads : Tile
    {
        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Crossroads"; }
        }

        public override string Rules
        {
            get
            {
                return
                    "All tiles must be placed adjacent to a Crossroads tiles as long as any Crossroads tile in play has less than four adjacent tiles.";
            }
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
            return Selection.None;
        }

        /// <summary>
        ///   Skips the current selection.
        /// </summary>
        internal override Selection Skip()
        {
            return Selection.None;
        }

        internal override Exception TryValidateInput(object input)
        {
            return null;
        }
    }
}