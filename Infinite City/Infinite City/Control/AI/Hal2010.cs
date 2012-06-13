using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Control.Tiles;
using InfiniteCity.Model.Interfaces;

namespace InfiniteCity.Control.AI
{
    public class Hal2010 : RuleBasedAI
    {
        public Hal2010(int level) : base(level) {}

        protected override double? GetBasePlayableSpaceWeight(ISpace input)
        {
            return input.AdjacentTiles.Any(t => t is Sanctum) ? 10 : 0;
        }

        protected override double? GetBasePlayableTileWeight(ITile input)
        {
            return input.Worth;
        }

        /// <summary>
        ///   Populates a dictionary of rules, switching on the type of GameState.ActiveTile.
        /// </summary>
        protected override IDictionary<Type, PlacedTileSelector> PlacedTileRuleDictionary()
        {
            // ReSharper disable UseObjectOrCollectionInitializer
            var dictionary = new Dictionary<Type, PlacedTileSelector>();
            // ReSharper restore UseObjectOrCollectionInitializer

            // Value more highly if it's not yours but valuable.
            dictionary.Add(typeof(Amphitheater), i => i.HasToken(Player) ? 0 : WeightPlayableTile(i));
            // Value more highly if it's not adjacent or valuable.
            dictionary.Add(typeof(Grove), i => 6-i.Space.AdjacentTiles.Count(t => t.HasToken(Player))*2-i.Worth);
            // Value more highly if it's not yours but valuable.
            dictionary.Add(typeof(Militia), i => i.HasToken(Player) ? 0 : WeightPlayableTile(i));
            // Value more highly if it's not yours but valuable.
            dictionary.Add(typeof(Plague), i => i.HasToken(Player) ? 0 : i.Worth*i.TokenCount());
            // Value more highly if it's not yours but valuable.
            dictionary.Add(typeof(Tavern), i => i.Worth*(i.TokenCount()-i.TokenCount(Player)));
            // Value more highly if it's valuable or near your tiles.
            dictionary.Add(typeof(Treasury), i => i.Space.AdjacentTiles.Count(t => t.HasToken(Player))+i.Worth*2);
            // Value more highly if it's near your tiles.
            dictionary.Add(typeof(Village), i => i.Space.AdjacentTiles.Count(t => t.HasToken(Player))*2);
            // Value more highly if it's not yours but valuable.
            dictionary.Add(typeof(Wasteland), i => i.HasToken(Player) ? 0 : i.Worth*i.TokenCount());
            return dictionary;
        }

        /// <summary>
        ///   Populates a dictionary of rules, switching on the type of GameState.SelectedTile.
        /// </summary>
        protected override IDictionary<Type, PlayableSpaceSelector> PlayableSpaceRuleDictionary()
        {
            // ReSharper disable UseObjectOrCollectionInitializer
            var dictionary = new Dictionary<Type, PlayableSpaceSelector>();
            // ReSharper restore UseObjectOrCollectionInitializer

            // Value less highly if surrounding spaces aren't owned by others.
            dictionary.Add(typeof(Amphitheater), i => i.AdjacentTiles.SumOrNull(t => t.TokenCount(Player)-t.TokenCount()));
            // Value more highly if surrounding spaces aren't owned.
            dictionary.Add(typeof(Crossroads), i => -i.AdjacentTiles.SumOrNull(t => t.TokenCount(Player)+t.TokenCount()));
            // Value more highly if a surrounding tiles is high-value.
            dictionary.Add(typeof(Graveyard), i => i.AdjacentTiles.Where(t => !t.HasToken(Player)).MaxOrNull(WeightPlacedTile));
            // Value more highly if surrounding unowned tiles are high-value.
            dictionary.Add(typeof(Keep), i => i.AdjacentTiles.Where(t => !t.HasToken(Player)).SumOrNull(t => t.Worth));
            // Value more highly if a surrounding tiles is high-value.
            dictionary.Add(typeof(Militia), i => i.AdjacentTiles.Where(t => !t.HasToken(Player)).MaxOrNull(WeightPlacedTile));
            // Value more highly if surrounding owned tiles are high-value.
            dictionary.Add(typeof(Sanctum), i => i.AdjacentTiles.Where(t => t.HasToken(Player)).SumOrNull(t => t.Worth));
            // Value more highly if surrounding unowned tiles are high-value.
            dictionary.Add(typeof(Tavern), i => i.AdjacentTiles.Where(t => !t.HasToken(Player)).SumOrNull(t => t.Worth));
            // Value more highly if surrounding spaces aren't owned.
            dictionary.Add(typeof(Treasury), i => -i.AdjacentTiles.SumOrNull(t => t.TokenCount(Player)+t.TokenCount()));
            return dictionary;
        }

        /// <summary>
        ///   Populates a dictionary of rules, switching on the type of the input tile.
        /// </summary>
        protected override IDictionary<Type, PlayableTileSelector> PlayableTileRuleDictionary()
        {
            // ReSharper disable UseObjectOrCollectionInitializer
            var dictionary = new Dictionary<Type, PlayableTileSelector>();
            // ReSharper restore UseObjectOrCollectionInitializer

            // Value highly if tiles remain.
            dictionary.Add(typeof(Cathedral), i => Math.Min(5, GameState.Deck.Remaining)*3);
            // Value highly if you're low on tokens.
            dictionary.Add(typeof(Grove), i => Player.Tokens>GameState.MaxTokens-5 ? 10 : 0);
            // Always value highly.
            dictionary.Add(typeof(Keep), i => 12);
            // Value highly if other players have bigger hands.
            dictionary.Add(typeof(Market), i => (GameState.Players.Where(p => p != Player).MaxOrNull(p => p.HandSize)-Hand.Count())*2);
            // Value highly if your hand isn't large.
            dictionary.Add(typeof(Plantation), i => Math.Min(10, Hand.Count()+5)/2d);
            // Value highly depending on other tiles in hand.
            dictionary.Add(typeof(Quarry), i => Hand.Where(t => !(t is Quarry)).MaxOrNull(WeightPlayableTile)+Hand.Count(t => t is Quarry));
            // Always value highly.
            dictionary.Add(typeof(Sanctum), i => 8);
            // Value highly if another player has a large hand.
            dictionary.Add(typeof(ThievesGuild), i => GameState.Players.Where(p => p != Player).MaxOrNull(p => p.HandSize));
            //Value highly if a high-worth tile is empty.
            dictionary.Add(typeof(Village),
                           i => GameState.Board.Tiles.Where(t => t.IsFlipped && t.TokenCount() == 0).MaxOrNull(t => t.Worth));
            return dictionary;
        }

        /// <summary>
        ///   Populates a dictionary of rules, switching on the type of GameState.ActiveTile.
        /// </summary>
        protected override IDictionary<Type, PlayerSelector> PlayerRuleDictionary()
        {
            // ReSharper disable UseObjectOrCollectionInitializer
            var dictionary = new Dictionary<Type, PlayerSelector>();
            // ReSharper restore UseObjectOrCollectionInitializer

            // Value players with the largest hand.
            dictionary.Add(typeof(Market), i => i.HandSize*i.HandSize);
            // Value players with the highest score.
            dictionary.Add(typeof(Tavern), i => i == Player ? 0 : GameState.PlayerScore(i));
            // Value players with the largest hand.
            dictionary.Add(typeof(ThievesGuild), i => i.HandSize*i.HandSize);
            return dictionary;
        }
    }
}