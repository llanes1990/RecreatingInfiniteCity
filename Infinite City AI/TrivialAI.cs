using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Model;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Interfaces;

namespace InfiniteCity.AI
{
    public class TrivialAI : AIPlayer
    {
        public override object GetInput()
        {
            switch (GameState.Selection)
            {
                case Selection.Player:
                case Selection.Opponent:
                    return GameState.Players.Where(GameState.Validate).Random();
                case Selection.TileFromHand:
                    return Hand.Where(GameState.Validate).Random();
                case Selection.TileFromBoard:
                    return GameState.Board.Tiles.Where(GameState.Validate).Random();
                case Selection.TileFromCustom:
                    return GameState.TileSet.Where(GameState.Validate).Random();
                case Selection.Space:
                    return GameState.Board.Edges.Where(GameState.Validate).Random();
                case Selection.Token:
                    return
                        GameState.Board.Tiles.SelectMany(tile => tile.Tokens.Select(kvp => new Tuple<ITile, IPlayer>(tile, kvp.Key))).Where(
                            GameState.Validate).Random();
            }
            return null;
        }
    }
}