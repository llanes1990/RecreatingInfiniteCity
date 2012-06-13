using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Enums;

namespace InfiniteCity.Model.Interfaces
{
    internal interface ISelectionManager
    {
        /// <summary>
        ///   Gets the active player.
        /// </summary>
        Player ActivePlayer { get; }

        /// <summary>
        ///   Gets the selection prompt.
        /// </summary>
        string Message { get; }

        /// <summary>
        ///   The tile selected in the last user selection.
        /// </summary>
        Tile SelectedTile { get; }

        /// <summary>
        ///   Gets the required selection type.
        /// </summary>
        Selection Selection { get; }

        /// <summary>
        ///   Gets the custom set of tiles.
        /// </summary>
        /// <exception cref = "InvalidOperationException">Thrown if Selection does not equal Selection.Set.</exception>
        IEnumerable<ITile> TileSet { get; }

        /// <summary>
        ///   Skips the selected tile's input phase
        /// </summary>
        void Skip();

        /// <summary>
        ///   Submits the selected input.
        /// </summary>
        /// <param name = "input">The object required for selection.</param>
        /// <exception cref = "ArgumentException">Thrown if the input is invalid.</exception>
        void Submit(object input);

        /// <summary>
        ///   Tests whether or not a given input is valid.
        /// </summary>
        /// <param name = "input">The object to test for validity.</param>
        /// <returns>True if passing the given input to Submit will succeed.</returns>
        bool Validate(object input);
    }
}