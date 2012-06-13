using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model;

namespace InfiniteCity.Control.Decks
{
    internal sealed class InfiniteDeck : Deck, IEnumerable<Tile>
    {
        private readonly Random _random = new Random();

        private readonly Type[] _tiles;

        public InfiniteDeck()
        {
            _tiles = WeightedTileArray();
        }

        /// <summary>
        ///   Gets the number of cards remaining in the deck. This property is not supported by infinite decks.
        /// </summary>
        /// <exception cref = "System.NotSupportedException" />
        public override int Remaining
        {
            get { return int.MaxValue; }
        }

        /// <summary>
        ///   Places a tile on the bottom of the deck.
        /// </summary>
        public override void Enqueue(Tile tile) {}

        /// <summary>
        ///   Removes and returns the first tile off the top of the deck.
        /// </summary>
        public override Tile Pop()
        {
            Type type = _tiles.Random();
            var tile = (Tile)Activator.CreateInstance(type);
            tile.Worth = Math.Min(_random.Next(0, 5)*_random.Next(0, 5)/TileWeights[type], 5);
            return tile;
        }

        /// <summary>
        ///   Places a tile on the top of the deck.
        /// </summary>
        public override void Push(Tile tile) {}

        /// <summary>
        ///   Randomizes the deck.
        /// </summary>
        public override void Shuffle() {}

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///   A <see cref = "T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<Tile> GetEnumerator()
        {
            throw new NotSupportedException();
        }

        private static Type[] WeightedTileArray()
        {
            return TileWeights.SelectMany(kvp => Enumerable.Repeat(kvp.Key, kvp.Value)).ToArray();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///   An <see cref = "T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotSupportedException();
        }
    }
}