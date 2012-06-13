using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Enums;

namespace InfiniteCity.Model.Interfaces
{
    public interface IGameState
    {
        /// <summary>
        ///   Gets the active player.
        /// </summary>
        IPlayer ActivePlayer { get; }

        /// <summary>
        ///   Gets the active tile.
        /// </summary>
        ITile ActiveTile { get; }

        /// <summary>
        ///   Gets the board object.
        /// </summary>
        IBoard Board { get; }

        /// <summary>
        ///   Gets the deck object.
        /// </summary>
        IDeck Deck { get; }

        /// <summary>
        ///   Gets the number of sanctum tiles that must be on the board to immediately end the game.
        /// </summary>
        int MaxSanctums { get; }

        /// <summary>
        ///   Gets the number of tokens one player must have on the board to initiate the end of the game.
        /// </summary>
        int MaxTokens { get; }

        /// <summary>
        ///   Gets the selection prompt.
        /// </summary>
        string Message { get; }

        /// <summary>
        ///   Gets the players in the game.
        /// </summary>
        IEnumerable<IPlayer> Players { get; }

        /// <summary>
        ///   The tile selected in the last user selection.
        /// </summary>
        ITile SelectedTile { get; }

        /// <summary>
        ///   Gets the required selection type.
        /// </summary>
        /// <exception cref = "InvalidOperationException">Throws if State does not equal State.Selection.</exception>
        Selection Selection { get; }

        /// <summary>
        ///   Gets the number of connected tiles required for scoring.
        /// </summary>
        int SmallestScoringGroup { get; }

        /// <summary>
        ///   Gets the game state of the game.
        /// </summary>
        State State { get; }

        /// <summary>
        ///   Gets the custom set of tiles.
        /// </summary>
        /// <exception cref = "InvalidOperationException">Thrown if Selection does not equal Selection.Set.</exception>
        IEnumerable<ITile> TileSet { get; }

        /// <summary>
        ///   Tests if a tile can successfully be placed.
        /// </summary>
        Exception CanPlaceTile(ITile tile, ISpace space, double authority = 0);

        /// <summary>
        ///   Tests if a token can successfully be placed.
        /// </summary>
        Exception CanPlaceToken(ITile tile, IPlayer player, double authority = 0);

        /// <summary>
        ///   Tests if a tile can successfully be removed.
        /// </summary>
        Exception CanRemoveTile(ITile tile, double authority = 0);

        /// <summary>
        ///   Tests if a token can successfully be removed.
        /// </summary>
        Exception CanRemoveToken(ITile tile, IPlayer player, double authority = 0);

        /// <summary>
        ///   Gets a players score at the end of the last turn.
        /// </summary>
        int PlayerScore(IPlayer player);

        /// <summary>
        ///   Skips the current tile's processing phase
        /// </summary>
        void Skip();

        /// <summary>
        ///   Tests whether or not a given input is valid.
        /// </summary>
        /// <param name = "input">The object to test for validity.</param>
        /// <returns>True if passing the given input to Submit will succeed.</returns>
        bool Validate(object input);
    }
}