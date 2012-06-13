using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Enums;

namespace InfiniteCity.Model.Interfaces
{
    internal interface IController
    {
        /// <summary>
        ///   Gets the active player.
        /// </summary>
        Player ActivePlayer { get; }

        /// <summary>
        ///   Gets the active tile.
        /// </summary>
        Tile ActiveTile { get; }

        /// <summary>
        ///   Gets the board object.
        /// </summary>
        Board Board { get; }

        /// <summary>
        ///   Gets the deck object.
        /// </summary>
        Deck Deck { get; }

        /// <summary>
        ///   Gets the number of sanctum tiles that must be on the board to immediately end the game.
        /// </summary>
        int MaxSanctums { get; }

        /// <summary>
        ///   Gets the number of tokens one player must have on the board to initiate the end of the game.
        /// </summary>
        int MaxTokens { get; }

        /// <summary>
        ///   Gets or sets the selection prompt.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        ///   Gets the players in the game.
        /// </summary>
        IEnumerable<Player> Players { get; }

        /// <summary>
        ///   A dictionary mapping players to their scores at the beginning of the current turn.
        /// </summary>
        IDictionary<IPlayer, int> Score { get; }

        /// <summary>
        ///   Gets the number of connected tiles required for scoring.
        /// </summary>
        int SmallestScoringGroup { get; }

        /// <summary>
        ///   Gets or sets the custom set of tiles.
        /// </summary>
        /// <exception cref = "InvalidOperationException">Thrown if Selection does not equal Selection.Set.</exception>
        IList<Tile> TileSet { get; set; }

        /// <summary>
        ///   Activates a tile placed on the board.
        /// </summary>
        /// <exception cref = "ArgumentException">Thrown if the tile is not on the board.</exception>
        Selection Activate(Tile tile);

        /// <summary>
        ///   Places a tile on a space but does not activates it.
        /// </summary>
        void PlaceTile(Tile tile, Space space, double authority = 0);

        /// <summary>
        ///   Removes a tile from a space.
        /// </summary>
        void RemoveTile(Tile tile, double authority = 0);
    }
}