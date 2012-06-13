using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Interfaces;

namespace InfiniteCity.Control.AI
{
    public abstract class RuleBasedAI : WeightedAI
    {
        protected readonly Lazy<IDictionary<Type, PlacedTileSelector>> _placedTileRules;
        protected readonly Lazy<IDictionary<Type, PlayerSelector>> _playerRules;
        protected readonly Lazy<IDictionary<Type, PlayableSpaceSelector>> _playableSpaceRules;
        protected readonly Lazy<IDictionary<Type, PlayableTileSelector>> _playableTileRules;

        protected RuleBasedAI(int level) : base(level)
        {
            _placedTileRules = new Lazy<IDictionary<Type, PlacedTileSelector>>(PlacedTileRuleDictionary);
            _playableSpaceRules = new Lazy<IDictionary<Type, PlayableSpaceSelector>>(PlayableSpaceRuleDictionary);
            _playableTileRules = new Lazy<IDictionary<Type, PlayableTileSelector>>(PlayableTileRuleDictionary);
            _playerRules = new Lazy<IDictionary<Type, PlayerSelector>>(PlayerRuleDictionary);
        }

        /// <summary>
        ///   Populates a dictionary of rules, switching on the type of GameState.ActiveTile.
        /// </summary>
        protected abstract IDictionary<Type, PlacedTileSelector> PlacedTileRuleDictionary();

        /// <summary>
        ///   Populates a dictionary of rules, switching on the type of GameState.SelectedTile.
        /// </summary>
        protected abstract IDictionary<Type, PlayableSpaceSelector> PlayableSpaceRuleDictionary();

        /// <summary>
        ///   Populates a dictionary of rules, switching on the type of the input tile.
        /// </summary>
        protected abstract IDictionary<Type, PlayableTileSelector> PlayableTileRuleDictionary();

        /// <summary>
        ///   Populates a dictionary of rules, switching on the type of GameState.ActiveTile.
        /// </summary>
        protected abstract IDictionary<Type, PlayerSelector> PlayerRuleDictionary();

        /// <summary>
        ///   Gets the base weight added to the default weight or non-null result of any PlacedTileSelector. Defaults to 0.
        /// </summary>
        protected virtual double? GetBasePlacedTileWeight(ITile input)
        {
            return 0;
        }

        /// <summary>
        ///   Gets the base weight added to the default weight or non-null result of any PlayableSpaceSelector. Defaults to 0.
        /// </summary>
        protected virtual double? GetBasePlayableSpaceWeight(ISpace input)
        {
            return 0;
        }

        /// <summary>
        ///   Gets the base weight added to the default weight or non-null result of any PlayableTileSelector. Defaults to 0.
        /// </summary>
        protected virtual double? GetBasePlayableTileWeight(ITile input)
        {
            return 0;
        }

        /// <summary>
        ///   Gets the base weight added to the default weight or non-null result of any PlayerSelector. Defaults to 0.
        /// </summary>
        protected virtual double? GetBasePlayerWeight(IPlayer input)
        {
            return 0;
        }

        /// <summary>
        ///   Gets the weight of a placed tile if no relevant PlacedTileRule exists.
        /// </summary>
        protected virtual double? GetDefaultPlacedTileWeight(ITile input)
        {
            return input.Worth*(input.TokenCount()-input.TokenCount(Player)*2);
        }

        /// <summary>
        ///   Gets the weight of a playable space if no relevant PlayableSpaceRule exists.
        /// </summary>
        protected virtual double? GetDefaultPlayableSpaceWeight(ISpace input)
        {
            return input.AdjacentTiles.Count(t => t.HasToken(Player));
        }

        /// <summary>
        ///   Gets the weight of a playable tile if no relevant PlayableTileRule exists.
        /// </summary>
        protected virtual double? GetDefaultPlayableTileWeight(ITile input)
        {
            return input.Worth;
        }

        /// <summary>
        ///   Gets the weight of a player if no relevant PlayerRule exists.
        /// </summary>
        protected virtual double? GetDefaultPlayerWeight(IPlayer input)
        {
            return input == Player ? 0 : GameState.PlayerScore(input);
        }

        protected override sealed IEnumerable<KeyValuePair<object, double>> GetWeightedInput()
        {
            switch (GameState.Selection)
            {
                case Selection.Player:
                    return GetValidPlayerInputs().Weight(WeightPlayer);
                case Selection.Opponent:
                    return GetValidOpponentInputs().Weight(WeightPlayer);
                case Selection.TileFromHand:
                    return GetValidTileFromHandInputs().Weight(WeightPlayableTile);
                case Selection.TileFromBoard:
                    return GetValidTileFromBoardInputs().Weight(WeightPlacedTile);
                case Selection.TileFromCustom:
                    return GetValidTileFromCustomInputs().Weight(WeightPlayableTile);
                case Selection.PlayableSpace:
                    return GetValidPlayableSpaceInputs().Weight(WeightPlayableSpace);
                default:
                    throw new InvalidOperationException();
            }
        }

        protected double? WeightPlacedTile(ITile input)
        {
            PlacedTileSelector selector;
            double? weight = GameState.ActiveTile != null &&
                             _placedTileRules.Value.TryGetValue(GameState.ActiveTile.GetType(), out selector)
                                 ? selector(input)
                                 : GetDefaultPlacedTileWeight(input);
            return weight.HasValue ? weight+GetBasePlacedTileWeight(input) : null;
        }

        protected double? WeightPlayableSpace(ISpace input)
        {
            PlayableSpaceSelector selector;
            double? weight = GameState.SelectedTile != null &&
                             _playableSpaceRules.Value.TryGetValue(GameState.SelectedTile.GetType(), out selector)
                                 ? selector(input)
                                 : GetDefaultPlayableSpaceWeight(input);
            return weight.HasValue ? weight+GetBasePlayableSpaceWeight(input) : null;
        }

        protected double? WeightPlayableTile(ITile input)
        {
            PlayableTileSelector selector;
            double? weight = _playableTileRules.Value.TryGetValue(input.GetType(), out selector)
                                 ? selector(input)
                                 : GetDefaultPlayableTileWeight(input);
            return weight.HasValue ? weight+GetBasePlayableTileWeight(input) : null;
        }

        protected double? WeightPlayer(IPlayer input)
        {
            PlayerSelector selector;
            double? weight = GameState.ActiveTile != null && _playerRules.Value.TryGetValue(GameState.ActiveTile.GetType(), out selector)
                                 ? selector(input)
                                 : GetDefaultPlayerWeight(input);
            return weight.HasValue ? weight+GetBasePlayerWeight(input) : null;
        }

        protected delegate double? PlacedTileSelector(ITile input);

        protected delegate double? PlayableSpaceSelector(ISpace input);

        protected delegate double? PlayableTileSelector(ITile input);

        protected delegate double? PlayerSelector(IPlayer input);
    }
}