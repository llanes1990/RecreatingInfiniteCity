using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model;

namespace InfiniteCity.Control.Decks
{
    internal sealed class FiniteDeck : Deck, IEnumerable<Tile>
    {
        private static readonly Random Random = new Random();
        private readonly LinkedList<Tile> _tiles;

        public FiniteDeck()
        {
            _tiles = new LinkedList<Tile>(YieldDeck());
            Shuffle();
        }

        public FiniteDeck(int seed)
        {
            _tiles = new LinkedList<Tile>(YieldDeck());
            Shuffle(seed);
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
                throw new InvalidOperationException("The deck is empty");
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
            Shuffle(_tiles, new Random());
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
            foreach (KeyValuePair<Type, int> kvp in TileWeights)
                for (int i = 0; i<kvp.Value; i++)
                {
                    var tile = (Tile)Activator.CreateInstance(kvp.Key);
                    tile.Worth = Math.Min(Random.Next(0, 5)*Random.Next(0, 5)/kvp.Value, 5);
                    yield return tile;
                }
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