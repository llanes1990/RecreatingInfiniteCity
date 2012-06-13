namespace InfiniteCity.Model
{
    public enum State
    {
        /// <summary>
        /// Signals the end of the current players turn
        /// </summary>
        NextTurn,
        /// <summary>
        /// Wait for selection of ANY player in the game
        /// </summary>
        ChoosePlayer,
        /// <summary>
        /// Wait for selection of a player who is not the current ActivePlayer 
        /// </summary>
        ChooseOpponent,
        /// <summary>
        /// Wait for the selection of tokens on the board. This likely involves selecting a 
        /// tile and then the tokens, but that's the interfaces problem. Only a single token at a time.
        /// </summary>
        ChooseToken,
        /// <summary>
        /// Wait for the player to select a tile from the ActiveHand
        /// </summary>
        ChooseTileFromHand,
        /// <summary>
        /// Wait for the player to select a tile from the Board
        /// </summary>
        ChooseTileFromBoard,
        /// <summary>
        /// Wait for the player to select an empty space on the board in order to place a tile there
        /// </summary>
        ChooseSpace,
        /// <summary>
        /// PlaceTile signals the controller to take the ActiveTile and place it on the board.
        /// </summary>
        PlaceTile,
        /// <summary>
        /// The controller exists in this state when it is verifying that a tile can indeed be placed in the provided space
        /// </summary>
        ResolveTilePlacement,
    }
}
