using System.Linq;
using InfiniteCity.Control;
using InfiniteCity.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfiniteCity.Tests.Model
{
    [TestClass]
    public class PlayerTest
    {
        private Controller _control;

        /// <summary>
        /// Adds a single tile to the board and tests that the player's score was correctly updated
        /// </summary>
        [TestMethod]
        public void AddSingleTileScoreTest()
        {
            Tile tile = new MockTile();
            _control.PlaceTile(tile, _control.GetPlacableSpaces().First());

            Assert.AreEqual(tile.Worth, _control.ActivePlayer.CalculateScore());
        }

        /// <summary>
        /// Adds three tiles adjacent to each other and tests that the player's score was correctly updated
        /// </summary>
        [TestMethod]
        public void AddThreeAdjacentTileScoreTest()
        {
            Tile[] tiles = {new MockTile(), new MockTile(), new MockTile()};
            int worth = 0;

            _control.PlaceTile(tiles[0], _control.GetPlacableSpaces().First());
            worth += tiles[0].Worth;

            //gets the adjacent spaces to the first tile and intersects them to make sure those places are playable
            Space[] possibleSpaces = tiles[0].Space.AdjacentSpaces.Intersect(_control.GetPlacableSpaces()).ToArray();

            Assert.IsTrue(possibleSpaces.Length>=tiles.Length-1);

            for (int i = 1; i<tiles.Length; i++)
            {
                _control.PlaceTile(tiles[i], possibleSpaces[i]);
                worth += tiles[i].Worth;
            }

            Assert.AreEqual(worth+tiles.Length, _control.ActivePlayer.CalculateScore());
        }

        /// <summary>
        /// Tests that a player's score starts at 0
        ///</summary>
        [TestMethod]
        public void ScoreStartsAtZeroTest()
        {
            foreach (Player player in _control.Players)
                Assert.AreEqual(0, player.CalculateScore());
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Controller.Initialize(new MockDeck());
            _control = Controller.DefaultInstance;
        }
    }
}