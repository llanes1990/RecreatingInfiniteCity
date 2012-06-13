using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfiniteCity.Model.Enums
{
    /// <summary>
    ///   Switches where the the selection is coming from.
    /// </summary>
    public enum Selection
    {
        /// <summary>
        ///   No selection is required and the controller should advance the game state.
        /// </summary>
        None = 0,
        /// <summary>
        ///   Requires the selection of a player, including the active player.
        /// </summary>
        /// <seealso cref = "Model.Player" />
        Player,
        /// <summary>
        ///   Requires the selection of a player, not including the active player.
        /// </summary>
        /// <seealso cref = "Model.Player" />
        Opponent,
        /// <summary>
        ///   Requires the selection of a tile from the active player's hand.
        /// </summary>
        /// <seealso cref = "Model.Tile" />
        TileFromHand,
        /// <summary>
        ///   Requires the selection of a tile from the board.
        /// </summary>
        /// <seealso cref = "Model.Tile" />
        TileFromBoard,
        /// <summary>
        ///   Requires the selection of a tile from the contoller's selection set.
        /// </summary>
        /// <seealso cref = "Model.Tile" />
        TileFromCustom,
        /// <summary>
        ///   Requires the selection of a space from the board without a tile on it.
        /// </summary>
        /// <seealso cref = "Model.Space" />
        PlayableSpace,
    }
}