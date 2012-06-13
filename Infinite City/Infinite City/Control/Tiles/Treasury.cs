using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Exceptions;
using InfiniteCity.Model.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteCity.Control.Tiles
{
    /// <summary>
    ///   Swap this tile's token with a token on a tile you do not control.
    /// </summary>
    internal class Treasury : Tile
    {
        private static readonly Selection[] States = new[] {Selection.TileFromBoard, Selection.None};

        private int _state;

        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Treasury"; }
        }

        public override string Rules
        {
            get { return "Swap this tile's token with a token on a tile you do not control."; }
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
            switch (_state)
            {
                case 0:
                    if (
                        Controller.DefaultInstance.Board.Tiles.Where(t => t.Owner != Owner && t.TokenCount() == 1 && t.Flags != Flags.Holy).
                            Count() == 0)
                        return Selection.None;
                    return States[_state++];
                case 1:

                    Exception ex = TryValidateInput(input);
                    if (ex != null)
                        throw ex;

                    var tile = (Tile)input;
                    Player player = Owner;
                    Owner = tile.Owner;
                    tile.Owner = player;
                    IDictionary<IPlayer, int> tokens = Tokens;
                    Tokens = tile.Tokens;
                    tile.Tokens = tokens;
                    return States[_state++];
            }
            throw new UnknownTransitionException();
        }

        internal override Exception TryValidateInput(object input)
        {
            var tile = (Tile)input;
            if (tile.TokenCount() != 1)
                return new InvalidOperationException("Cannot swap ownership with a tile occupied by more than one player.");

            if (tile.Tokens.ContainsKey(Owner))
                return new InvalidOperationException("Cannot swap ownership with a tile you control.");

            if (tile.Flags == Flags.Holy)
                return new InvalidOperationException("Cannot swap ownership with a holy site.");
            return null;
        }
    }
}