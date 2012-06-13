using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model;

namespace InfiniteCity.Tests.Model
{
    internal sealed class MockDeck : Deck, IEnumerable<Tile>
    {
        private readonly int _seed;
        private readonly LinkedList<Tile> _tiles;

        public MockDeck() : this(0x53454544) {}

        public MockDeck(int seed) : this(seed, YieldDeck()) {}

        public MockDeck(IEnumerable<Tile> tiles) : this(0x53454544, tiles) {}

        public MockDeck(int seed, IEnumerable<Tile> tiles)
        {
            _seed = seed;
            _tiles = new LinkedList<Tile>(tiles);
        }

        /// <summary>
        ///   Gets the number of cards remaining in the deck. This property is not supported by infinite decks.
        /// </summary>
        /// <exception cref = "System.NotSupportedException" />
        public override int Remaining
        {
            get { return _tiles.Count; }
        }

        /// <summary>
        ///   Places a tile on the bottom of the deck.
        /// </summary>
        public override void Enqueue(Tile tile)
        {
            _tiles.AddLast(tile);
        }

        /// <summary>
        ///   Removes and returns the first tile off the top of the deck.
        /// </summary>
        public override Tile Pop()
        {
            if (_tiles.Count == 0)
                throw new InvalidOperationException();
            LinkedListNode<Tile> node = _tiles.First;
            _tiles.RemoveFirst();
            return node.Value;
        }

        /// <summary>
        ///   Places a tile on the top of the deck.
        /// </summary>
        public override void Push(Tile tile)
        {
            _tiles.AddFirst(tile);
        }

        /// <summary>
        ///   Randomizes the deck.
        /// </summary>
        public override void Shuffle()
        {
            Shuffle(_seed);
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///   A <see cref = "T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<Tile> GetEnumerator()
        {
            return _tiles.GetEnumerator();
        }

        /// <summary>
        ///   Randomizes the deck.
        /// </summary>
        public void Shuffle(int seed)
        {
            Shuffle(_tiles, new Random(seed));
        }

        private static void Shuffle<T>(LinkedList<T> list, Random random)
        {
            T[] array = list.ToArray();
            list.Clear();

            for (int i = array.Length; i>0; i--)
            {
                int item = random.Next(0, i);
                list.AddFirst(array[item]);
                Array.Copy(array, item+1, array, item, array.Length-(item+1));
            }
        }

        private static IEnumerable<Tile> YieldDeck()
        {
            for (int i = 0; i<120; i++)
                yield return new MockTile();
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
            return _tiles.GetEnumerator();
        }
    }
}