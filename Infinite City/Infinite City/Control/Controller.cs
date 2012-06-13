using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using InfiniteCity.Control.AI;
using InfiniteCity.Control.Decks;
using InfiniteCity.Model;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Exceptions;
using InfiniteCity.Model.Interfaces;
using InfiniteCity.Model.Rules;
using InfiniteCity.Model.Rules.Arguments;
using Microsoft.Xna.Framework;

namespace InfiniteCity.Control
{
    internal sealed class Controller : ISelectionManager, IController, IGameState
    {
        public static readonly Color[] PlayerColors = new[] {Color.Red, Color.Yellow, Color.Tan, Color.Blue, Color.LimeGreen, Color.Silver};

        private static readonly Dictionary<Selection, Type> SelectionTypes = new Dictionary<Selection, Type>
            {
                {Selection.Opponent, typeof(Player)}, {Selection.Player, typeof(Player)}, {Selection.PlayableSpace, typeof(Space)},
                {Selection.TileFromBoard, typeof(Tile)}, {Selection.TileFromCustom, typeof(Tile)}, {Selection.TileFromHand, typeof(Tile)},
            };

        private readonly Player[] _players;

        private static Player[] _defaultPlayers = new[]
            {
                new Player("HAL 9000", Color.Red, ColorName.Red) {Input = new Hal2010(3)},
                new Player("Deepthgt", Color.Yellow, ColorName.Yellow) {Input = new Hal2010(5)},
                new Player("SAL 9000", Color.Tan, ColorName.Tan) {Input = new Hal2010(4)},
                new Player("Skynet", Color.Blue, ColorName.Blue) {Input = new Hal2010(9)},
                new Player("GLaDOS", Color.LimeGreen, ColorName.Green) {Input = new Hal2010(7)},
                new Player("Wheatley", Color.Silver, ColorName.Silver) {Input = new RandomAI()},
            };

        private static Controller _instance;
        private bool _lastRound;
        private Player _lastPlayer;

        /// <summary>
        ///   Constructor for the controller
        /// </summary>
        /// <param name = "deck"></param>
        /// <param name = "players"></param>
        private Controller(Deck deck, Player[] players)
        {
            Deck = deck;
            Board = new Board();
            _players = players;

            Score = new Dictionary<IPlayer, int>();

            TilePlacementRules = new RuleSet<TileMovementArguments>(RuleTypes.TilePlacement);
            TileRemovalRules = new RuleSet<TileMovementArguments>(RuleTypes.TileRemoval);
            TokenPlacementRules = new RuleSet<TokenMovementArguments>(RuleTypes.TokenPlacement);
            TokenRemovalRules = new RuleSet<TokenMovementArguments>(RuleTypes.TokenRemoval);
            Selection = Selection.TileFromHand;

            Board[0, 0].Tile = Deck.Pop();
            Board[1, 0].Tile = Deck.Pop();
            Board[0, -1].Tile = Deck.Pop();

            // Each player starts with 5 tiles
            foreach (Player player in players)
            {
                Score[player] = 0;
                for (int i = 0; i<5; i++)
                    player.AddToHand(Deck.Pop());
            }
            ActivePlayerIndex = 0;
            ActivePlayer = players[ActivePlayerIndex];
            Message = "Select a tile from your hand";
            Selection = Selection.TileFromHand;

            TileQueue = new List<Tile>();

            MaxTokens = 45;
            MaxSanctums = 10;
            SmallestScoringGroup = 3;
        }

        /// <summary>
        ///   Gets the singleton controller.
        /// </summary>
        public static Controller DefaultInstance
        {
            get { return _instance ?? (_instance = new Controller(new InfiniteDeck(), _defaultPlayers)); }
        }

        /// <summary>
        ///   Gets the active player.
        /// </summary>
        public Player ActivePlayer { get; set; }

        /// <sumary>
        ///   Gets the index of the active player
        /// </sumary>
        public int ActivePlayerIndex { get; set; }

        /// <summary>
        ///   Gets the active tile.
        /// </summary>
        public Tile ActiveTile { get; private set; }

        /// <summary>
        ///   Gets the board object.
        /// </summary>
        public Board Board { get; private set; }

        /// <summary>
        ///   Gets the deck object.
        /// </summary>
        public Deck Deck { get; private set; }

        /// <summary>
        ///   Gets the number of sanctum tiles that must be on the board to immediately end the game.
        /// </summary>
        public int MaxSanctums { get; set; }

        /// <summary>
        ///   Gets the number of tokens one player must have on the board to initiate the end of the game.
        /// </summary>
        public int MaxTokens { get; set; }

        /// <summary>
        ///   Gets or sets the selection prompt.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///   Gets or sets the custom set of players
        /// </summary>
        public IList<Player> PlayerSet { get; set; }

        /// <summary>
        ///   Gets the players in the game.
        /// </summary>
        public IEnumerable<Player> Players
        {
            get { return _players; }
        }

        /// <summary>
        ///   A dictionary mapping players to their scores at the beginning of the current turn.
        /// </summary>
        public IDictionary<IPlayer, int> Score { get; private set; }

        /// <summary>
        ///   The tile selected in the last user selection.
        /// </summary>
        public Tile SelectedTile { get; private set; }

        /// <summary>
        ///   Gets the required selection type.
        /// </summary>
        /// <exception cref = "InvalidOperationException">Throws if State does not equal Selection.</exception>
        public Selection Selection { get; private set; }

        /// <summary>
        ///   Gets the number of connected tiles required for scoring.
        /// </summary>
        public int SmallestScoringGroup { get; private set; }

        /// <summary>
        ///   Gets the game state of the game.
        /// </summary>
        public State State { get; private set; }

        public RuleSet<TileMovementArguments> TilePlacementRules { get; private set; }

        /// <summary>
        ///   The processing queue for tiles to be processed
        /// </summary>
        public List<Tile> TileQueue { get; set; }

        public RuleSet<TileMovementArguments> TileRemovalRules { get; private set; }

        /// <summary>
        ///   Gets or sets the custom set of tiles.
        /// </summary>
        /// <exception cref = "InvalidOperationException">Thrown if Selection does not equal Selection.Set.</exception>
        public IList<Tile> TileSet { get; set; }

        public RuleSet<TokenMovementArguments> TokenPlacementRules { get; private set; }

        public RuleSet<TokenMovementArguments> TokenRemovalRules { get; private set; }

        /// <summary>
        ///   The player whose turn it is.
        /// </summary>
        IPlayer IGameState.ActivePlayer
        {
            get { return ActivePlayer; }
        }

        /// <summary>
        ///   The current tile being processed. This is the tile that was just played and is still performing its action.
        ///   Controller will continually call transition on the ActiveTile until it transition returns State.NextTurn.
        /// </summary>
        ITile IGameState.ActiveTile
        {
            get { return ActiveTile; }
        }

        /// <summary>
        ///   Gets the board object.
        /// </summary>
        IBoard IGameState.Board
        {
            get { return Board; }
        }

        /// <summary>
        ///   Gets the deck object.
        /// </summary>
        IDeck IGameState.Deck
        {
            get { return Deck; }
        }

        /// <summary>
        ///   Gets the players in the game.
        /// </summary>
        IEnumerable<IPlayer> IGameState.Players
        {
            get { return Players; }
        }

        /// <summary>
        ///   The tile selected in the last user selection.
        /// </summary>
        ITile IGameState.SelectedTile
        {
            get { return SelectedTile; }
        }

        /// <summary>
        ///   Gets the custom set of tiles.
        /// </summary>
        /// <exception cref = "InvalidOperationException">Thrown if Selection does not equal Selection.Set.</exception>
        IEnumerable<ITile> IGameState.TileSet
        {
            get { return TileSet; }
        }

        /// <summary>
        ///   Gets the custom set of tiles.
        /// </summary>
        /// <exception cref = "InvalidOperationException">Thrown if Selection does not equal Selection.Set.</exception>
        IEnumerable<ITile> ISelectionManager.TileSet
        {
            get { return TileSet; }
        }

        /// <summary>
        ///   Gets the singleton controller using provided players
        /// </summary>
        public static Controller CustomInstance(Player[] players)
        {
            _instance = new Controller(new InfiniteDeck(), players);
            return _instance;
        }

        /// <summary>
        ///   Initializes a new game with the default deck and player list.
        /// </summary>
        public static void Initialize()
        {
            _instance = null;
        }

        /// <summary>
        ///   Initializes a new game with the given deck and default player list.
        /// </summary>
        public static void Initialize(Deck deck)
        {
            Initialize(deck, _defaultPlayers);
        }

        /// <summary>
        ///   Initializes a new game with the given deck and player list.
        /// </summary>
        public static void Initialize(Deck deck, params Player[] players)
        {
            _instance = new Controller(deck, players);
        }

        /// <summary>
        ///   Activates a tile placed on the board.
        /// </summary>
        /// <exception cref = "ArgumentException">Thrown if the tile is not on the board.</exception>
        public Selection Activate(Tile tile)
        {
            ActiveTile = tile;
            return ActiveTile.Transition(null);
        }

        /// <summary>
        ///   Tests if a tile can successfully be placed.
        /// </summary>
        public Exception CanPlaceTile(ITile tile, ISpace space, double authority = 0)
        {
            if (tile != null && tile.IsPlaced)
                return new InvalidOperationException("Cannot place an already placed tile.");
            if (space.Tile != null)
                return new InvalidOperationException("Cannot place a tile in an already occupied space.");
            Rule<TileMovementArguments> rule;
            var args = new TileMovementArguments {Space = space, Tile = tile};
            //ternary is a pain in the ass to debug!!!
            //return !TilePlacementRules.CheckAction(args, authority, out rule) ? rule.GetException(args) : null;
            return !TilePlacementRules.CheckAction(args, authority, out rule) ? rule.GetException() : null;
        }

        /// <summary>
        ///   Tests if a token can successfully be placed.
        /// </summary>
        public Exception CanPlaceToken(ITile tile, IPlayer player, double authority = 0)
        {
            if (!tile.IsPlaced)
                return new InvalidOperationException("Cannot modify tokens on an unplaced tile.");
            Rule<TokenMovementArguments> rule;
            var args = new TokenMovementArguments {Actor = player, Tile = tile};
            return !TokenPlacementRules.CheckAction(args, authority, out rule) ? rule.GetException() : null;
        }

        /// <summary>
        ///   Tests if a tile can successfully be removed.
        /// </summary>
        public Exception CanRemoveTile(ITile tile, double authority = 0)
        {
            if (!tile.IsPlaced)
                return new InvalidOperationException("Cannot remove an unplaced tile.");
            Rule<TileMovementArguments> rule;
            var args = new TileMovementArguments {Space = tile.Space, Tile = tile};
            return !TileRemovalRules.CheckAction(args, authority, out rule) ? rule.GetException() : null;
        }

        /// <summary>
        ///   Tests if a token can successfully be removed.
        /// </summary>
        public Exception CanRemoveToken(ITile tile, IPlayer player, double authority = 0)
        {
            if (!tile.IsPlaced)
                return new InvalidOperationException("Cannot modify tokens on an unplaced tile.");
            Rule<TokenMovementArguments> rule;
            var args = new TokenMovementArguments {Actor = player, Tile = tile};
            return !TokenRemovalRules.CheckAction(args, authority, out rule) ? rule.GetException() : null;
        }

        /// <summary>
        ///   Enumerates a list of possible spaces where the specific tile can be placed.
        /// </summary>
        public IEnumerable<Space> GetPlacableSpaces(Tile tile = null)
        {
            return Board.Edges.Where(space => (CanPlaceTile(tile, space) == null));
        }

        /// <summary>
        ///   Creates the players' source of inputs
        /// </summary>
        /// <param name = "inputParams">A list > to create the player source of inputs</param>
        public void InitPlayerInputs(List<Tuple<string, int>> inputParams)
        {
            _defaultPlayers = new Player[6];
            for (int playerIndex = 0; playerIndex<_defaultPlayers.Count(); playerIndex++)
            {
                if (inputParams[playerIndex].Item1 != "Human")
                {
                    int index = playerIndex;
                    Func<int, AI.AI> inputSource =
                        (from ai in AI.AI.Opponents where ai.Key == inputParams[index].Item1 select ai.Value).ElementAt(0);
                    _defaultPlayers[playerIndex].Input = inputSource(inputParams[playerIndex].Item2);
                }
                _defaultPlayers[playerIndex].Name = inputParams[playerIndex].Item1+(playerIndex+1);
            }
        }

        /// <summary>
        ///   Places a tile on a space but does not activates it.
        /// </summary>
        public void PlaceTile(Tile tile, Space space, double authority = 0)
        {
            Exception ex = CanPlaceTile(tile, space, authority);
            if (ex != null)
                throw ex;

            Debug.Assert(tile.Space == null && space.Tile == null);
            tile.Space = space;
            tile.Owner = ActivePlayer;
            tile.IsFlipped = true;
            tile.AddToken(tile.Owner);
            if (tile.Space.AdjacentTiles.Any(tile2 => (tile2.Building == "Sanctum" && tile2.IsFlipped)))
                tile.Flags = Flags.Holy;
            ActivePlayer.RemoveFromHand(tile);
        }

        /// <summary>
        ///   Removes a tile from a space on the board.
        /// </summary>
        public void RemoveTile(Tile tile, double authority = 0)
        {
            Exception ex = CanRemoveTile(tile, authority);
            if (ex != null)
                throw ex;

            tile.IsFlipped = false;
            tile.Space = null;
            tile.Owner = null;
            tile.RemoveAllTokens();
            tile.Reset();
        }

        public void Skip()
        {
            Selection = ActiveTile.Skip();
            ProcessQueue();
            UpdateMessage();
        }

        public void TogglePlayers()
        {
            ActivePlayerIndex = ++ActivePlayerIndex%_players.Length;
            ActivePlayer = _players[ActivePlayerIndex];
            UpdateScores();
        }

        /// <summary>
        ///   Tests whether or not a given input is valid.
        /// </summary>
        /// <param name = "input">The object to test for validity.</param>
        /// <returns>True if passing the given input to Submit will succeed.</returns>
        public bool Validate(object input)
        {
            return TryValidateInput(input) == null;
        }

        private void CheckEndGame()
        {
            if (ActivePlayer.Tokens>=MaxTokens && !_lastRound)
            {
                _lastRound = true;
                _lastPlayer = ActivePlayer;
            }
            else if (_lastRound)
                if (ActivePlayer == _lastPlayer)
                    throw new EndGameException("Game Over");

            if (Board.Tiles.Where(t => t.Building == "Sanctum").Count() == MaxSanctums)
                throw new EndGameException("Game Over");
        }

        private void NextPlayer()
        {
            CheckEndGame();
            ActivePlayerIndex = ++ActivePlayerIndex%_players.Length;
            ActivePlayer = _players[ActivePlayerIndex];
            CheckEndGame();

            State = State.NextTurn;
            Selection = Selection.TileFromHand;
        }

        /// <summary>
        ///   Gets a players score at the end of the last turn.
        /// </summary>
        int IGameState.PlayerScore(IPlayer player)
        {
            return Score[player];
        }

        private void ProcessQueue()
        {
            while (Selection == Selection.None)
            {
                if (TileQueue.Count() != 0)
                {
                    ActiveTile = TileQueue.First();
                    TileQueue = TileQueue.Where(t => t != ActiveTile).ToList();
                    Selection = ActiveTile.Transition(null);
                }
                else
                {
                    // draw up to max hand
                    while (ActivePlayer.HandSize<5)
                        ActivePlayer.AddToHand(Deck.Pop());

                    UpdateScores();
                    NextPlayer();
                    State = State.NextTurn;
                    Selection = Selection.TileFromHand;
                    if (Board.Tiles.Where(t => t.Building == "Sanctum").Count() == 5)
                    {
                        throw new EndGameException("End game");   
                    }
                }
            }
        }

        void ISelectionManager.Skip()
        {
            Selection = ActiveTile.Skip();
            ProcessQueue();
            UpdateMessage();
        }

        /// <summary>
        ///   This function is called by game whenever the input asked from the user is resolved.
        ///   It acts according to the controllers current state. Most commonly, this function checks 
        ///   the input for errors and then forwards the input to the active tile.
        /// </summary>
        /// <exception cref = "InvalidSelectionException">
        ///   The providing input was not valid and the user should be asked again
        /// </exception>
        void ISelectionManager.Submit(object input)
        {
            if (input == null)
                Selection = ActiveTile.Skip();
            else
            {
                Exception ex = TryValidateInput(input);
                if (ex != null)
                    throw ex;

                switch (State)
                {
                    case State.NextTurn:
                        switch (Selection)
                        {
                            case Selection.TileFromHand:
                                SelectedTile = (Tile)input;
                                if (SelectedTile.IsPlaced)
                                    break;
                                Selection = Selection.PlayableSpace;
                                break;
                            case Selection.PlayableSpace:
                                PlaceTile(SelectedTile, input as Space);
                                ActiveTile = SelectedTile;
                                Selection = ActiveTile.Transition(null);
                                State = State.ResolveTileActions;
                                break;
                        }
                        break;
                    case State.ResolveTileActions:
                        Selection = ActiveTile.Transition(input);
                        break;
                }
            }

            // None means the tile is done
            // since we have a queue now, if the queue isn't empty
            // pop the first and process it.  If it doesn't require
            // user input, pop the next one and process it.
            // Repeat until we need the user, or the queue is empty
            ProcessQueue();

            // Update the message to reflect needed action
            UpdateMessage();
        }

        private Exception TryValidateInput(object input)
        {
            if (input == null ||
                !(input.GetType().IsSubclassOf(SelectionTypes[Selection]) || input.GetType().Equals(SelectionTypes[Selection])))
                return new InvalidSelectionException("Invalid selection type.");

            // ReSharper disable AssignNullToNotNullAttribute
            // ReSharper disable PossibleNullReferenceException
            switch (Selection)
            {
                case Selection.TileFromBoard:
                    if (!Board.Tiles.Contains(input as Tile))
                        return new InvalidSelectionException("You must select a tile on the board.");
                    break;
                case Selection.TileFromHand:
                    if (!ActivePlayer.Hand.Contains(input as Tile))
                        return new InvalidSelectionException("You must select a tile in your hand.");

                    var tile = input as Tile;
                    if (tile.Building == "Treasury")
                        if (Board.Tiles.Where(t => t.TokenCount() != 0 && !t.Tokens.ContainsKey(ActivePlayer)).Count() == 0)
                            return new InvalidSelectionException("There is no tile to switch ownership with");

                    break;
                case Selection.TileFromCustom:
                    if (!TileSet.Contains(input as Tile))
                        return new InvalidSelectionException("You must select a tile in your hand.");
                    break;
                case Selection.PlayableSpace:
                    if (!ReferenceEquals(Board[(input as Space).Location], (input as Space)))
                        return new InvalidSelectionException("Bad space, what the hell man!");
                    Exception exception = CanPlaceTile(null, input as Space);
                    if (exception != null)
                        return exception;
                    break;
                case Selection.Player:
                    if (!_players.Contains(input as Player))
                        return new InvalidSelectionException("You must select a player.");
                    break;
                case Selection.Opponent:
                    if (!_players.Contains(input as Player) || input as Player == ActivePlayer)
                        return new InvalidSelectionException("You must select a opponent.");
                    break;
            }
            // ReSharper restore PossibleNullReferenceException
            // ReSharper restore AssignNullToNotNullAttribute
            switch (State)
            {
                case State.ResolveTileActions:
                    return ActiveTile.TryValidateInput(input);
                default:
                    return null;
            }
        }

        private void UpdateMessage()
        {
            switch (Selection)
            {
                case Selection.TileFromBoard:
                    Message = "Select a tile on the board";
                    break;
                case Selection.TileFromCustom:
                    Message = "Select a tile on the selection";
                    break;
                case Selection.Player:
                    Message = "Select a player, or yourself";
                    break;
                case Selection.Opponent:
                    Message = "Select a player, not yourself";
                    break;
                case Selection.PlayableSpace:
                    Message = "Select a space from the board";
                    break;
                case Selection.TileFromHand:
                    Message = "Select a tile from your hand";
                    break;
                case Selection.None:
                    Message = "You broke something";
                    break;
            }
        }

        /// <summary>
        ///   Updates the players'
        /// </summary>
        private void UpdateScores()
        {
            Score.Clear();
            foreach (Player player in _players)
                Score[player] = player.CalculateScore();
        }
    }
}