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
    ///   Swap hands with another player.
    /// </summary>
    internal class Market : Tile
    {
        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Market"; }
        }

        public override string Rules
        {
            get { return "Swap hands with another player."; }
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
            return Selection.Opponent;
        }

        /// <summary>
        ///   Skips the current selection.
        /// </summary>
        internal override Selection Skip()
        {
            throw new InvalidOperationException("Cannot skip this action");
        }

        internal override Selection Transition(object input)
        {
            if (input == null)
                return Selection.Opponent;

            if (!IsPlaced)
                throw new UnknownTransitionException("Market");

            var player = input as Player;
            if (player == null || player == Owner)
                throw new ArgumentException("Market: Invalid player swap");

            Player.SwapHands(Owner, player);
            return Selection.None;
        }

        internal override Exception TryValidateInput(object input)
        {
            var player = input as Player;
            if (player == null || player == Owner)
                return new ArgumentException("Market: Invalid player swap");
            return null;
        }
    }
}