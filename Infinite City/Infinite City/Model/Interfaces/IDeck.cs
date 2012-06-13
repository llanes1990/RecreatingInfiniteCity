using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteCity.Model.Interfaces
{
    public interface IDeck
    {
        /// <summary>
        ///   Gets the number of cards remaining in the deck. This property is not supported by infinite decks.
        /// </summary>
        /// <exception cref = "System.NotSupportedException" />
        int Remaining { get; }

        /// <summary>
        ///   Gets each type of tile this deck contains.
        /// </summary>
        IEnumerable<Type> TileTypes { get; }
    }
}