using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model;
using InfiniteCity.Model.Enums;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteCity.Tests.Model
{
    internal class MockTile : Tile
    {
        private readonly string _building;
        private readonly string _rules;

        public MockTile() : this("Façade", string.Empty) {}

        public MockTile(string building, string rules)
        {
            _building = building;
            _rules = rules;
        }

        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        /// <summary>
        ///   Gets the name of this tile's building.
        /// </summary>
        public override string Building
        {
            get { return _building; }
        }

        /// <summary>
        ///   Gets the rules text of this tile.
        /// </summary>
        public override string Rules
        {
            get { return _rules; }
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