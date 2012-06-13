using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Control.Tiles;
using InfiniteCity.Model.Interfaces;

namespace InfiniteCity.Model
{
    internal abstract class Deck : IDeck
    {
        protected static readonly Dictionary<Type, int> TileWeights = new Dictionary<Type, int>
                                                                          {
                                                                              {typeof(Crossroads), 6}, {typeof(Graveyard), 6},
                                                                              {typeof(Grove), 4}, {typeof(Quarry), 7}, {typeof(Market), 4},
                                                                              {typeof(Village), 10}, {typeof(Cathedral), 6},
                                                                              {typeof(Plantation), 5}, {typeof(Plague), 9},
                                                                              {typeof(Militia), 8}, {typeof(Amphitheater), 3},
                                                                              {typeof(ReflectingPond), 8}, {typeof(AbandonedMine), 8},
                                                                              {typeof(Wasteland), 3}, {typeof(Spring), 8},
                                                                              {typeof(ThievesGuild), 4}, {typeof(Keep), 1},
                                                                              {typeof(Sanctum), 5}, {typeof(Treasury), 6}, {typeof(Tavern), 5},
                                                                              //{typeof(Blank),20}
                                                                          };

        /// <summary>
        ///   Gets the number of cards remaining in the deck. This property is not supported by infinite decks.
        /// </summary>
        /// <exception cref = "System.NotSupportedException" />
        public abstract int Remaining { get; }

        /// <summary>
        ///   Gets each type of tile this deck contains.
        /// </summary>
        public virtual IEnumerable<Type> TileTypes
        {
            get { return TileWeights.Keys; }
        }

        /// <summary>
        ///   Places a tile on the bottom of the deck.
        /// </summary>
        public abstract void Enqueue(Tile tile);

        /// <summary>
        ///   Removes and returns the first tile off the top of the deck.
        /// </summary>
        public abstract Tile Pop();

        /// <summary>
        ///   Places a tile on the top of the deck.
        /// </summary>
        public abstract void Push(Tile tile);

        /// <summary>
        ///   Randomizes the deck.
        /// </summary>
        public abstract void Shuffle();
    }
}