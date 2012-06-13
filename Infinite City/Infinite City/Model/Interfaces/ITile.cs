using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Enums;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteCity.Model.Interfaces
{
    public interface ITile
    {
        /// <summary>
        ///   Gets the name of this tile's building.
        /// </summary>
        string Building { get; }

        /// <summary>
        ///   Gets the game flags set on this tile.
        /// </summary>
        Flags Flags { get; }

        /// <summary>
        ///   Gets whether or not this tile is flipped over or not.
        /// </summary>
        /// <exception cref = "System.InvalidOperationException" />
        bool IsFlipped { get; }

        /// <summary>
        ///   Gets whether or not this tile is placed on the board.
        /// </summary>
        bool IsPlaced { get; }

        /// <summary>
        ///   Gets the player who placed this tile onto the board.
        /// </summary>
        /// <exception cref = "System.InvalidOperationException" />
        IPlayer PlacedBy { get; }

        /// <summary>
        ///   Gets the rules text of this tile.
        /// </summary>
        string Rules { get; }

        /// <summary>
        ///   Gets the space this tile is on.
        /// </summary>
        ISpace Space { get; }

        /// <summary>
        ///   Gets the texture of this tile.
        /// </summary>
        Texture2D Texture { get; }

        /// <summary>
        ///   Gets the title text of this tile.
        /// </summary>
        string Title { get; }

        /// <summary>
        ///   Gets the players with tokens on this tile and the number of tokens they have.
        /// </summary>
        /// <exception cref = "System.InvalidOperationException" />
        IEnumerable<KeyValuePair<IPlayer, int>> Tokens { get; }

        /// <summary>
        ///   Gets the point value of this tile.
        /// </summary>
        int Worth { get; }

        bool HasToken(IPlayer player);

        /// <summary>
        ///   Gets the number of tokens on this tile.
        /// </summary>
        /// <exception cref = "System.InvalidOperationException" />
        int TokenCount();

        /// <summary>
        ///   Gets the number of tokens a player has on this tile.
        /// </summary>
        /// <exception cref = "System.InvalidOperationException" />
        int TokenCount(IPlayer player);
    }
}