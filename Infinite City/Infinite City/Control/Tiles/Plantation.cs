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
    ///   After filling your hand this turn, draw and keep five more tiles.
    /// </summary>
    internal class Plantation : Tile
    {
        private const int DrawSize = 5;

        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Plantation"; }
        }

        public override string Rules
        {
            get { return "After filling your hand this turn, draw and keep five more tiles."; }
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

        internal override Selection Transition(object input)
        {
            for (int i = 0; i<DrawSize; i++)
                Owner.AddToHand(Controller.DefaultInstance.Deck.Pop());

            return Selection.None;
        }

        internal override Exception TryValidateInput(object input)
        {
            return null;
        }
    }
}