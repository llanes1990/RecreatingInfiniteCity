using System;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Enums;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace InfiniteCity.Model.Interfaces
{
    public interface IPlayer
    {
        /// <summary>
        ///   Gets the displayed color of the player.
        /// </summary>
        Color Color { get; }

        /// <summary>
        ///   Gets the name of the displayed color of the player.
        /// </summary>
        ColorName ColorName { get; }

        /// <summary>
        ///   Gets the size of this players hand.
        /// </summary>
        int HandSize { get; }

        /// <summary>
        ///   Gets the player's input controller.
        /// </summary>
        IAsyncInput Input { get; }

        /// <summary>
        ///   Gets the displayed name of the player.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///   Gets the tiles on the board that this player owns.
        /// </summary>
        IEnumerable<ITile> Tiles { get; }

        /// <summary>
        ///   Gets the number of tokens this player has remaining.
        /// </summary>
        int Tokens { get; }

        /// <summary>
        ///   Gets the player's current score, excluding any bonus points.
        /// </summary>
        int CalculateScore();
    }
}