using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Control;
using InfiniteCity.Control.AI;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Interfaces;
using Microsoft.Xna.Framework;

namespace InfiniteCity.Model
{
    internal class Player : IPlayer
    {
        private LinkedList<Tile> _hand = new LinkedList<Tile>();
        private IAsyncInput _input;

        internal Player(string name, Color color, ColorName colorName)
        {
            Name = name;
            Color = color;
            ColorName = colorName;
            Tokens = 0;
        }

        /// <summary>
        ///   Gets the displayed color of the player.
        /// </summary>
        public Color Color { get; private set; }

        public ColorName ColorName { get; set; }

        /// <summary>
        ///   Gets the cards in the players hand.
        /// </summary>
        public IEnumerable<Tile> Hand
        {
            get { return _hand; }
        }

        /// <summary>
        ///   Gets the size of this players hand.
        /// </summary>
        public int HandSize
        {
            get { return _hand.Count; }
        }

        /// <summary>
        ///   Gets the player's input controller.
        /// </summary>
        public IAsyncInput Input
        {
            get { return _input; }
            internal set
            {
                _input = value;
                if (_input is AI)
                    ((AI)_input).Player = this;
            }
        }

        /// <summary>
        ///   Gets the displayed name of the player.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        ///   Gets the tiles on the board that this player owns.
        /// </summary>
        public IEnumerable<Tile> Tiles
        {
            get { return Controller.DefaultInstance.Board.Tiles.Where(tile => tile.Tokens.ContainsKey(this) && tile.Tokens[this]>0); }
        }

        /// <summary>
        ///   Gets or sets the number of tokens this player has remaining.
        /// </summary>
        public int Tokens { get; set; }

        /// <summary>
        ///   Gets the tiles on the board that this player owns.
        /// </summary>
        IEnumerable<ITile> IPlayer.Tiles
        {
            get { return Tiles; }
        }

        /// <summary>
        ///   Swaps the hands of two players.
        /// </summary>
        public static void SwapHands(Player playerA, Player playerB)
        {
            LinkedList<Tile> buffer = playerA._hand;
            playerA._hand = playerB._hand;
            playerB._hand = buffer;
        }

        /// <summary>
        ///   Adds a tile to a players hand.
        /// </summary>
        public void AddToHand(Tile tile)
        {
            _hand.AddLast(tile);
        }

        /// <summary>
        ///   Gets the player's current score, excluding any bonus points.
        /// </summary>
        public int CalculateScore()
        {
            int score = 0;
            var tiles = new HashSet<Tile>(Tiles);
            var sets = new LinkedList<LinkedList<Tile>>();

            score += tiles.Sum(tile => tile.Worth);
            while (tiles.Count>0)
            {
                Tile tile = tiles.First();

                var set = new LinkedList<Tile>();
                var queue = new HashSet<Tile>();

                while (tile != null)
                {
                    set.AddFirst(tile);
                    tiles.Remove(tile);

                    foreach (Tile adj in
                        tile.Space.AdjacentTiles.Where(
                            adj => tiles.Contains(adj) && !queue.Contains(adj) && adj.Tokens.ContainsKey(this) && adj.Tokens[this]>0))
                        queue.Add(adj);

                    tile = queue.FirstOrDefault();
                    queue.Remove(tile);
                }

                sets.AddFirst(set);
            }

            score += sets.Where(set => set.Count>=Controller.DefaultInstance.SmallestScoringGroup).Sum(set => set.Count);
            return score;
        }

        /// <summary>
        ///   Removes a tile from a players hand.
        /// </summary>
        public void RemoveFromHand(Tile tile)
        {
            _hand.Remove(tile);
        }
    }
}