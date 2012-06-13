using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model;
using InfiniteCity.Model.Enums;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteCity.Control.Tiles
{
    /// <summary>
    ///   Prevents itself and adjacent tiles from being modified by other tiles, except the Keep.
    /// </summary>
    internal class Sanctum : Tile
    {
        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Sanctum"; }
        }

        public override string Rules
        {
            get { return "Prevents itself and adjacent tiles from being modified by other tiles, except the Keep."; }
        }

        /// <summary>
        ///   Gets the texture of this tile.
        /// </summary>
        public override Texture2D Texture
        {
            get { return TileTexture; }
        }

        internal override void Reset() {}

        internal override Selection Revert()
        {
            return Selection.None;
        }

        internal override Selection Skip()
        {
            return Selection.None;
        }

        internal override Selection Transition(object input)
        {
            Exception ex = TryValidateInput(input);
            if (ex != null)
                throw ex;

            foreach (Tile tile in Space.AdjacentTiles)
                tile.Flags = Flags.Holy;

            Flags = Flags.Holy;

            return Selection.None;
        }

        internal override Exception TryValidateInput(object input)
        {
            return null;
        }
    }
}