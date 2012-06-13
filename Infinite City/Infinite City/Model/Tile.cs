using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using InfiniteCity.Control;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteCity.Model
{
    internal abstract class Tile : ITile
    {
        private IDictionary<IPlayer, int> _tokens = new Dictionary<IPlayer, int>();
        private Space _space;
        private bool _isFlipped;
        private Player _owner;

        /// <summary>
        ///   Gets the name of this tile's building.
        /// </summary>
        public abstract string Building { get; }

        /// <summary>
        ///   Gets the rules text of this tile.
        /// </summary>
        public abstract string Rules { get; }

        /// <summary>
        ///   Gets the texture of this tile.
        /// </summary>
        public abstract Texture2D Texture { get; }

        /// <summary>
        ///   Gets or sets the game flags set on this tile.
        /// </summary>
        public Flags Flags { get; set; }

        /// <summary>
        ///   Gets or sets whether or not this tile is flipped over or not. True means the tile is visible; false means it is upside-down.
        /// </summary>
        /// <exception cref = "System.InvalidOperationException">Thrown if the tile has not been placed.</exception>
        public bool IsFlipped
        {
            get
            {
                if (!IsPlaced)
                    throw new InvalidOperationException("Cannot call this method while the tile is not placed.");
                return _isFlipped;
            }
            set
            {
                if (!IsPlaced)
                    throw new InvalidOperationException("Cannot call this method while the tile is not placed.");
                _isFlipped = value;
            }
        }

        /// <summary>
        ///   Gets whether or not this tile is placed on the board.
        /// </summary>
        public bool IsPlaced
        {
            get { return _space != null; }
        }

        /// <summary>
        ///   Gets or sets the player who placed this tile onto the board.
        /// </summary>
        /// <exception cref = "System.InvalidOperationException">Thrown if the tile has not been placed.</exception>
        public Player Owner
        {
            get
            {
                if (!IsPlaced)
                    throw new InvalidOperationException("Cannot call this method while the tile is not placed.");
                return _owner;
            }
            set { _owner = value; }
        }

        /// <summary>
        ///   Gets or sets the space this tile is on.
        /// </summary>
        public Space Space
        {
            get { return _space; }
            set
            {
                //Debug.Assert(_space == null || ReferenceEquals(this, _space.Tile));
                if (ReferenceEquals(_space, value))
                    return;
                Space old = _space;
                _space = value;
                if (old != null)
                    old.Tile = null;
                if (_space != null)
                    _space.Tile = this;
                Debug.Assert(_space == null || ReferenceEquals(this, _space.Tile));
            }
        }

        /// <summary>
        ///   Gets the title text of this tile.
        /// </summary>
        public string Title
        {
            get { return Flags.HasFlag(Flags.Holy) ? string.Format("Holy {0}", Building) : Building; }
        }

        /// <summary>
        ///   Gets the dictionary containing the number of tokens on this tile.
        /// </summary>
        /// <exception cref = "System.InvalidOperationException">Thrown if the tile has not been placed.</exception>
        public IDictionary<IPlayer, int> Tokens
        {
            get
            {
                if (!IsPlaced)
                    throw new InvalidOperationException("Cannot call this method while the tile is not placed.");
                return _tokens;
            }
            set { _tokens = value; }
        }

        /// <summary>
        ///   Gets the point value of this tile.
        /// </summary>
        public int Worth { get; set; }

        /// <summary>
        ///   Gets the player who placed this tile onto the board.
        /// </summary>
        /// <exception cref = "System.InvalidOperationException">Thrown if the tile has not been placed.</exception>
        IPlayer ITile.PlacedBy
        {
            get { return Owner; }
        }

        /// <summary>
        ///   Gets the space this tile is on.
        /// </summary>
        ISpace ITile.Space
        {
            get { return Space; }
        }

        /// <summary>
        ///   Gets the players with tokens on this tile and the number of tokens they have.
        /// </summary>
        /// <exception cref = "System.InvalidOperationException" />
        IEnumerable<KeyValuePair<IPlayer, int>> ITile.Tokens
        {
            get
            {
                if (!IsPlaced)
                    throw new InvalidOperationException("Cannot call this method while the tile is not placed.");
                return _tokens.Where(kvp => kvp.Value>0);
            }
        }

        public void AddToken(IPlayer player, double authority = 0)
        {
            Exception ex = Controller.DefaultInstance.CanPlaceToken(this, player, authority);
            if (ex != null)
                throw ex;

            int value;
            if (!_tokens.TryGetValue(player, out value))
                value = 0;
            _tokens[player] = ++value;
            ((Player)player).Tokens++;
        }

        public bool HasToken(IPlayer player)
        {
            return TokenCount(player) != 0;
        }

        public void RemoveAllTokens()
        {
            foreach (KeyValuePair<IPlayer, int> pair in _tokens)
                ((Player)pair.Key).Tokens -= pair.Value;
            _tokens.Clear();
        }

        public void RemoveToken(IPlayer player, double authority = 0)
        {
            Exception ex = Controller.DefaultInstance.CanRemoveToken(this, player, authority);
            if (ex != null)
                throw ex;

            if (!_tokens.ContainsKey(player))
                throw new InvalidOperationException();
            _tokens[player]--;

            if (_tokens[player] == 0)
                _tokens.Remove(player);

            ((Player)player).Tokens--;
        }

        /// <summary>
        ///   Gets the number of tokens on this tile.
        /// </summary>
        /// <exception cref = "System.InvalidOperationException">Thrown if the tile has not been placed.</exception>
        public int TokenCount()
        {
            if (!IsPlaced)
                throw new InvalidOperationException("Cannot call this method while the tile is not placed.");
            return _tokens.Values.Sum();
        }

        /// <summary>
        ///   Gets the number of tokens a player has on this tile.
        /// </summary>
        /// <exception cref = "System.InvalidOperationException">Thrown if the tile has not been placed.</exception>
        public int TokenCount(IPlayer player)
        {
            if (!IsPlaced)
                throw new InvalidOperationException("Cannot call this method while the tile is not placed.");
            int tokens;
            return _tokens.TryGetValue(player, out tokens) ? tokens : 0;
        }

        /// <summary>
        ///   Resets the tile's active state.
        /// </summary>
        internal abstract void Reset();

        /// <summary>
        ///   Reverts the tile's active state.
        /// </summary>
        internal abstract Selection Revert();

        /// <summary>
        ///   Skips the current selection.
        /// </summary>
        internal abstract Selection Skip();

        internal abstract Exception TryValidateInput(object input);

        /// <summary>
        ///   Transitions this active tile from its current state to the next state.
        /// </summary>
        /// <param name = "input">Null, or the input the tile requested.</param>
        /// <returns>NextTurn or the request for the next input.</returns>
        internal virtual Selection Transition(object input)
        {
            return Selection.None;
        }
    }
}