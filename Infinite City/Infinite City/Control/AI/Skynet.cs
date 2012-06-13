using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using InfiniteCity.Control.Tiles;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Interfaces;

namespace InfiniteCity.Control.AI
{
    internal sealed class Skynet : Hal2010
    {
        private new readonly object _lock = new object();
        private readonly Queue<object> _queue = new Queue<object>();

        private readonly IDictionary<Tuple<ISpace, IPlayer>, int> _spaceWorths = new Dictionary<Tuple<ISpace, IPlayer>, int>();

        public Skynet() : base(9) {}

        protected override object GetInput()
        {
            _spaceWorths.Clear();
            if (_queue.Count == 0)
                switch (GameState.Selection)
                {
                    case Selection.TileFromHand:
                        EnqueuePlan(DevisePlans(Hand).Random(), _queue);
                        break;
                    case Selection.TileFromCustom:
                        EnqueuePlan(DevisePlans(GameState.TileSet).Random(), _queue);
                        break;
                }
            if (_queue.Count != 0)
            {
                if (GameState.Validate(_queue.Peek()))
                    return _queue.Dequeue();
                Debug.WriteLine("{0} ({1}) tried to make an invalid selection: {2}", Player.Name, GetType().Name, _queue.Peek());
                _queue.Clear();
            }
            // ReSharper disable PossibleNullReferenceException
            Debug.WriteLine("{0} ({1}) has become stumped; reverting to {2}.", Player.Name, GetType().Name, GetType().BaseType.Name);
            // ReSharper restore PossibleNullReferenceException
            return base.GetInput();
        }

        private static void EnqueuePlan(Plan plan, Queue<object> queue)
        {
            queue.Enqueue(plan.Tile);
            queue.Enqueue(plan.Space);
            if (plan.Arguments != null)
                foreach (object item in plan.Arguments)
                    queue.Enqueue(item);
        }

        private static IEnumerable<ISpace> GetAvailableSpaces()
        {
            IEnumerable<ISpace> spaces = GameState.Board.Edges;
            if (GameState.Board.Find<Crossroads>().Any(t => t.IsFlipped && t.Space.AdjacentTiles.Count()<4))
                spaces = spaces.Where(s => s.AdjacentTiles.Any(t => t is Crossroads));
            return spaces;
        }

        private int CalculateAdvantage(Plan plan)
        {
            IDictionary<IPlayer, int> scores = GameState.Players.ToDictionary(p => p, p => GameState.PlayerScore(p)+plan.Worth(p));
            if (scores[Player]>scores.Where(kvp => kvp.Key != Player).Max(kvp => kvp.Value))
                return scores.Where(kvp => kvp.Key != Player).Sum(kvp => scores[Player]-kvp.Value);
            return scores.Where(kvp => kvp.Key != Player && kvp.Value>scores[Player]).Sum(kvp => scores[Player]-kvp.Value);
        }

        private int ComparePlans(Plan planA, Plan planB)
        {
            int comparison = Comparer<int>.Default.Compare(CalculateAdvantage(planA), CalculateAdvantage(planB));
            return comparison != 0 ? comparison : Comparer<double>.Default.Compare(planA.Value, planB.Value);
        }

        private IEnumerable<Plan> DevisePlans(IEnumerable<ITile> tiles)
        {
            return DevisePlans(tiles, ComparePlans);
        }

        private IEnumerable<Plan> DevisePlans(IEnumerable<ITile> tiles, Func<Plan, Plan, int> comparer)
        {
            return tiles.SelectMany(tile => DevisePlans(tile, GetAvailableSpaces(), comparer)).TakeBest(comparer);
        }

        private IEnumerable<Plan> DevisePlans(ITile tile, IEnumerable<ISpace> spaces, Func<Plan, Plan, int> comparer)
        {
            return ((IEnumerable<Plan>)DeviseTilePlans((dynamic)tile, spaces)).TakeBest(comparer);
        }

        private IEnumerable<Plan> DeviseTilePlans(ITile tile, IEnumerable<ISpace> spaces)
        {
            foreach (ISpace space in spaces)
            {
                var plan = new Plan(tile, space) {Value = space.AdjacentTiles.Count(t => t.HasToken(Player))/6d};
                plan.Worths[Player] = GetSpaceWorth(space)+tile.Worth;
                yield return plan;
            }
        }

        private IEnumerable<Plan> DeviseTilePlans(Amphitheater tile, IEnumerable<ISpace> spaces)
        {
            return from space in spaces let target = space select new Plan(tile, space)
                {
                    Worths = GameState.Players.ToDictionary(p => p, p => GetSpaceWorth(target, p)+tile.Worth),
                    Value = space.AdjacentTiles.Count(t => t.HasToken(Player))/6d,
                };
        }

        private IEnumerable<Plan> DeviseTilePlans(Graveyard tile, IEnumerable<ISpace> spaces)
        {
            foreach (ISpace space in spaces)
            {
                double value = space.AdjacentTiles.Count(t => t.HasToken(Player));
                foreach (ITile adjacentTile in space.AdjacentTiles)
                {
                    ITile target = adjacentTile;
                    var plan = new Plan(tile, space)
                        {Worths = GameState.Players.ToDictionary(p => p, p => -GetTileWorth(target, p)), Value = value,};
                    plan.Worths[Player] += GetSpaceWorth(space)+tile.Worth;
                    plan.Arguments.Add(target);
                    yield return plan;
                }
            }
        }

        private IEnumerable<Plan> DeviseTilePlans(Market tile, IEnumerable<ISpace> spaces)
        {
            foreach (ISpace space in spaces)
                foreach (IPlayer opponent in GameState.Players.Where(p => p != Player))
                {
                    var plan = new Plan(tile, space) {Value = opponent.HandSize-Player.HandSize};
                    plan.Worths[Player] = GetSpaceWorth(space)+tile.Worth;
                    plan.Arguments.Add(opponent);
                    yield return plan;
                }
        }

        private IEnumerable<Plan> DeviseTilePlans(Militia tile, IEnumerable<ISpace> spaces)
        {
            foreach (ISpace space in spaces)
                foreach (ITile adjacentTile in space.AdjacentTiles.Where(t => !t.HasToken(Player)))
                {
                    ITile target = adjacentTile;
                    var plan = new Plan(tile, space) {Worths = GameState.Players.ToDictionary(p => p, p => -GetTileWorth(target, p)),};
                    plan.Worths[Player] = GetSpaceWorth(space)+tile.Worth+GetTileWorth(target);
                    plan.Arguments.Add(target);
                    yield return plan;
                }
        }

        private IEnumerable<Plan> DeviseTilePlans(Plague tile, IEnumerable<ISpace> spaces)
        {
            foreach (ITile otherTile in GameState.Board.Tiles)
            {
                ITile target = otherTile;
                var worths = GameState.Players.ToDictionary(p => p, p => -GetTileWorth(target, p));
                foreach (ISpace space in spaces)
                {
                    var plan = new Plan(tile, space) {Worths = worths, Value = space.AdjacentTiles.Count(t => t.HasToken(Player)),};
                    plan.Worths[Player] += GetSpaceWorth(space)+tile.Worth;
                    plan.Arguments.Add(target);
                    yield return plan;
                }
            }
        }

        private IEnumerable<Plan> DeviseTilePlans(ReflectingPond tile, IEnumerable<ISpace> spaces)
        {
            foreach (ITile otherTile in GameState.Board.Tiles)
            {
                ITile targetTile = otherTile;
                var worths = GameState.Players.ToDictionary(p => p, p => -GetTileWorth(targetTile, p));
                foreach (ISpace otherSpace in spaces)
                {
                    ISpace targetSpace = otherSpace;
                    worths = worths.ToDictionary(kvp => kvp.Key,
                                                 kvp => kvp.Value+(targetTile.HasToken(kvp.Key) ? GetSpaceWorth(targetSpace, kvp.Key) : 0));
                    foreach (ISpace space in spaces.Where(s => s != targetSpace))
                    {
                        var plan = new Plan(tile, space) {Worths = worths, Value = space.AdjacentTiles.Count(t => t.HasToken(Player)),};
                        plan.Worths[Player] += GetSpaceWorth(space)+tile.Worth;
                        plan.Arguments.Add(targetTile);
                        plan.Arguments.Add(targetSpace);
                        yield return plan;
                    }
                }
            }
        }

        private int GetSpaceWorth(ISpace space)
        {
            return GetSpaceWorth(space, Player);
        }

        private int GetSpaceWorth(ISpace space, IPlayer player)
        {
            int worth;
            lock (_lock)
                if (_spaceWorths.TryGetValue(new Tuple<ISpace, IPlayer>(space, player), out worth))
                    return worth;

            var visited = new HashSet<ITile>();
            if (space.Tile != null)
                visited.Add(space.Tile);

            var groups = new List<List<ITile>>();
            foreach (ITile tile in space.AdjacentTiles.Where(t => t.HasToken(player) && !visited.Contains(t)))
            {
                var group = new List<ITile>();
                var queue = new Queue<ITile>(new[] {tile});
                while (queue.Count>0)
                {
                    ITile query = queue.Dequeue();
                    visited.Add(query);
                    group.Add(query);
                    foreach (ITile adj in query.Space.AdjacentTiles.Where(t => t.HasToken(player) && !visited.Contains(t)))
                        queue.Enqueue(adj);
                }
                groups.Add(group);
            }

            int[] sizes = groups.Select(g => g.Count).ToArray();
            if (sizes.Length == 0 || sizes.Sum()<GameState.SmallestScoringGroup-1)
                worth = 0;
            else
                worth = 1+sizes.Where(i => i<GameState.SmallestScoringGroup).Sum();

            lock (_lock)
                _spaceWorths[new Tuple<ISpace, IPlayer>(space, player)] = worth;
            return worth;
        }

        private int GetTileWorth(ITile tile)
        {
            return GetTileWorth(tile, Player);
        }

        private int GetTileWorth(ITile tile, IPlayer player)
        {
            return tile.HasToken(player) ? GetSpaceWorth(tile.Space, Player)+tile.Worth : 0;
        }

        private sealed class Plan
        {
            public Plan(ITile tile, ISpace space)
            {
                Tile = tile;
                Space = space;
                Arguments = new List<object>();
                Worths = new Dictionary<IPlayer, int>();
            }

            /// <summary>
            ///   Gets the arguments to resolve tile placements.
            /// </summary>
            public IList<object> Arguments { get; private set; }

            /// <summary>
            ///   Gets the space to play the tile.
            /// </summary>
            public ISpace Space { get; private set; }

            /// <summary>
            ///   Gets the tile to choose for placement.
            /// </summary>
            public ITile Tile { get; private set; }

            /// <summary>
            ///   Gets the heuristical board state value for the player after this play.
            /// </summary>
            public double Value { get; set; }

            /// <summary>
            ///   Gets the expected score change for the given player after this play.
            /// </summary>
            public IDictionary<IPlayer, int> Worths { get; set; }

            /// <summary>
            ///   Gets the expected score change for the given player after this play.
            /// </summary>
            /// <param name = "player">The player who's score to check.</param>
            /// <returns>The estimated change in the player's score.</returns>
            public int Worth(IPlayer player)
            {
                int worth;
                return Worths == null ? 0 : Worths.TryGetValue(player, out worth) ? worth : 0;
            }
        }
    }
}