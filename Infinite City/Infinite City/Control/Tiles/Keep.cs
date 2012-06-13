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
    ///   Take control of all adjacent tiles, ignoring all protections.
    /// </summary>
    internal class Keep : Tile
    {
        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Keep"; }
        }

        public override string Rules
        {
            get { return "Take control of all adjacent tiles, ignoring all protections."; }
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
            {
                tile.RemoveAllTokens();
                tile.AddToken(Owner);
                tile.Owner = Owner;

                // Every tile flipped needs to get processed
                // add it to the controller's processing queue
                if (tile.IsFlipped)
                    continue;

                tile.IsFlipped = true;
                Controller.DefaultInstance.TileQueue.Add(tile);
            }
            return Selection.None;
        }

        internal override Exception TryValidateInput(object input)
        {
            return null;
        }
    }
}