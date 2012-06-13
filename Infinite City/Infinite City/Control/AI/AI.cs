using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using InfiniteCity.Model;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Interfaces;

namespace InfiniteCity.Control.AI
{
    public abstract class AI : IAsyncInput
    {
        protected readonly object _lock = new object();

        private object _input;
        private bool _isThinking;
        private bool _isComplete;

        public static IGameState GameState
        {
            get { return Controller.DefaultInstance; }
        }

        public static IEnumerable<KeyValuePair<string, Func<int, AI>>> Opponents
        {
            get
            {
                yield return new KeyValuePair<string, Func<int, AI>>("Wheatley", l => new RandomAI());
                yield return new KeyValuePair<string, Func<int, AI>>("HAL", l => new Hal2010(l));
                yield return new KeyValuePair<string, Func<int, AI>>("Skynet", l => new Skynet());
            }
        }

        public bool IsComplete
        {
            get
            {
                lock (_lock)
                    return _isComplete;
            }
        }

        public bool IsThinking
        {
            get
            {
                lock (_lock)
                    return _isThinking;
            }
        }

        public IPlayer Player { get; set; }

        protected IEnumerable<ITile> Hand
        {
            get { return ((Player)Player).Hand; }
        }

        public object Acknowledge()
        {
            lock (_lock)
            {
                if (_isThinking || !_isComplete)
                    throw new InvalidOperationException("Cannot acknowledge before last operation is complete.");
                object input = _input;
                _input = null;
                _isComplete = false;
                return input;
            }
        }

        public void GetInputAsync()
        {
            lock (_lock)
            {
                if (_isThinking || _isComplete)
                    throw new InvalidOperationException("Cannot begin async call before last operation is complete or acknowledged.");
                var thread = new Thread(() =>
                    {
                        object input = GetInput();
                        lock (_lock)
                        {
                            _input = input;
                            _isComplete = true;
                            _isThinking = false;
                        }
                    });
                _isThinking = true;
                thread.Start();
            }
        }

        protected static IEnumerable<ISpace> GetValidPlayableSpaceInputs()
        {
            return GameState.Board.Edges.AsParallel().Where(GameState.Validate).ToArray();
        }

        protected static IEnumerable<IPlayer> GetValidPlayerInputs()
        {
            return GameState.Players.Where(GameState.Validate);
        }

        protected static IEnumerable<ITile> GetValidTileFromBoardInputs()
        {
            return GameState.Board.Tiles.AsParallel().Where(GameState.Validate).ToArray();
        }

        protected static IEnumerable<ITile> GetValidTileFromCustomInputs()
        {
            return GameState.TileSet.Where(GameState.Validate);
        }

        protected abstract object GetInput();

        protected IEnumerable<object> GetValidInputs()
        {
            switch (GameState.Selection)
            {
                case Selection.Player:
                    return GetValidPlayerInputs();
                case Selection.Opponent:
                    return GetValidOpponentInputs();
                case Selection.TileFromHand:
                    return GetValidTileFromHandInputs();
                case Selection.TileFromBoard:
                    return GetValidTileFromBoardInputs();
                case Selection.TileFromCustom:
                    return GetValidTileFromCustomInputs();
                case Selection.PlayableSpace:
                    return GetValidPlayableSpaceInputs();
                default:
                    throw new InvalidOperationException();
            }
        }

        protected IEnumerable<IPlayer> GetValidOpponentInputs()
        {
            return GameState.Players.Where(p => p != Player).Where(GameState.Validate);
        }

        protected IEnumerable<ITile> GetValidTileFromHandInputs()
        {
            return Hand.Where(GameState.Validate);
        }
    }
}