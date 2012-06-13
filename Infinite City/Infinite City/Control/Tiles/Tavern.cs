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
    ///   Move any or all tokens from adjacent tiles to this tile.
    /// </summary>
    internal class Tavern : Tile
    {
        private readonly Selection[] _stateSequence = {Selection.TileFromBoard, Selection.Player, Selection.None,};

        private int _activeStateIndex = -1;

        private Tile _currentTile;

        /// <summary>
        ///   Gets or sets the texture of this type of tile.
        /// </summary>
        public static Texture2D TileTexture { get; set; }

        public override string Building
        {
            get { return "Tavern"; }
        }

        public override string Rules
        {
            get { return "Move any or all tokens from adjacent tiles to this tile."; }
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
            _activeStateIndex = -1;
        }

        /// <summary>
        ///   Reverts the tile's active state.
        /// </summary>
        internal override Selection Revert()
        {
            _activeStateIndex = _activeStateIndex == 0 ? _activeStateIndex : _activeStateIndex = 1;
            return _stateSequence[_activeStateIndex];
        }

        internal override Selection Skip()
        {
            return Selection.None;
        }

        internal override Selection Transition(object input)
        {
            if (input == null && _activeStateIndex == -1)
                if (Space.AdjacentTiles.Where(t => t.TokenCount() != 0 && t.Flags != Flags.Holy).Count() == 0)
                    return Selection.None;
                else
                    return _stateSequence[++_activeStateIndex];

            if (input != null && _activeStateIndex == 0)
            {
                var tile = input as Tile;

                if (tile == null || tile.TokenCount() == 0)
                    throw new ArgumentException("Choose a tile that has tokens on it");

                // Multiple players have tokens on this tile, ask which one
                if (tile.TokenCount()>1 && tile.Tokens.Count()>1)
                {
                    _currentTile = tile;
                    Controller.DefaultInstance.PlayerSet =
                        Controller.DefaultInstance.Players.Where(p => tile.Tokens.ContainsKey(p)).ToList();
                    return _stateSequence[++_activeStateIndex];
                }
                var tempPlayer = (Player)tile.Tokens.First().Key;
                tile.RemoveToken(tempPlayer);
                AddToken(tempPlayer);
                if (Space.AdjacentTiles.Where(t => t.TokenCount() != 0 && t.Flags != Flags.Holy).Count() == 0)
                    return Selection.None;
                return _stateSequence[_activeStateIndex];
            }
            if (input != null && _activeStateIndex == 1)
            {
                var player = input as Player;
                _currentTile.RemoveToken(player);
                AddToken(player);
                Controller.DefaultInstance.PlayerSet = null;
                if (Space.AdjacentTiles.Where(t => t.TokenCount() != 0 && t.Flags != Flags.Holy).Count() == 0)
                    return Selection.None;
                return _stateSequence[--_activeStateIndex];
            }
            throw new UnknownTransitionException("Grove");
        }

        internal override Exception TryValidateInput(object input)
        {
            if (_activeStateIndex == 0)
            {
                var tile = (Tile)input;
                if (!Space.AdjacentTiles.Contains(tile))
                    return new ArgumentException("Choose a tile that is adjacent to the Tavern");
                if (tile.Flags == Flags.Holy)
                    return new ArgumentException("Cannot swap ownership with a holy site.");
                if (tile.TokenCount() == 0)
                    return new ArgumentException("Choose a tile that has tokens on it");
            }
            else
            {
                var player = (Player)input;
                if (!_currentTile.Tokens.ContainsKey(player))
                    return new ArgumentException("Choose a player that has a token on this tile");
            }

            return null;
        }
    }
}