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
    ///   Place an adjacent tile into your hand. Return any tokens on it to their owners.
    /// </summary>
    internal class Graveyard : Tile
    {
        private const double Authority = 0;
        private static readonly Selection[] States = new[] {Selection.TileFromBoard, Selection.None};

        private int _state;

        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Graveyard"; }
        }

        public override string Rules
        {
            get { return "Place an adjacent tile into your hand. Return any tokens on it to their owners."; }
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
            return Selection.None;
        }

        internal override Selection Transition(object input)
        {
            Exception ex = TryValidateInput(input);
            if (ex != null)
                throw ex;

            switch (_state)
            {
                case 0:
                    if (Space.AdjacentTiles.Where(t => t.Flags != Flags.Holy).Count() == 0)
                        return Skip();

                    return States[_state++];
                case 1:
                    var tile = (Tile)input;
                    Controller.DefaultInstance.RemoveTile(tile, Authority);
                    Controller.DefaultInstance.ActivePlayer.AddToHand(tile);
                    return States[_state++];
            }
            return Selection.None;
        }

        internal override Exception TryValidateInput(object input)
        {
            switch (_state)
            {
                case 1:
                    Exception ex;
                    if ((ex = Controller.DefaultInstance.CanRemoveTile(input as Tile, Authority)) != null)
                        return ex;
                    if (((Tile)input).Flags == Flags.Holy)
                        return new InvalidOperationException("Cannot reposess a holy site");
                    break;
            }
            return null;
        }
    }
}