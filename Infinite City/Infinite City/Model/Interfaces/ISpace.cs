using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace InfiniteCity.Model.Interfaces
{
    public interface ISpace
    {
        /// <summary>
        ///   Gets all the spaces adjacent to this one.
        /// </summary>
        IEnumerable<ISpace> AdjacentSpaces { get; }

        /// <summary>
        ///   Gets all the tiles adjacent to this space.
        /// </summary>
        IEnumerable<ITile> AdjacentTiles { get; }

        /// <summary>
        ///   Gets the board this space is on.
        /// </summary>
        IBoard Board { get; }

        /// <summary>
        ///   Gets the location of this space.
        /// </summary>
        Point Location { get; }

        /// <summary>
        ///   Gets the tile placed on this space.
        /// </summary>
        ITile Tile { get; }

        /// <summary>
        ///   Determines whether or not this space is adjacent to the given point.
        /// </summary>
        bool IsAdjacentTo(Point point);

        /// <summary>
        ///   Determines whether or not this space is adjacent to the given point.
        /// </summary>
        bool IsAdjacentTo(int x, int y);

        /// <summary>
        ///   Determines whether or not this space is adjacent to the given tile.
        /// </summary>
        bool IsAdjacentTo(ITile tile);

        /// <summary>
        ///   Determines whether or not this space is adjacent to the given space.
        /// </summary>
        bool IsAdjacentTo(ISpace space);

        /// <summary>
        ///   Determines whether or not this space is adjacent to the given type of tile.
        /// </summary>
        bool IsAdjacentTo<T>() where T: ITile;
    }
}