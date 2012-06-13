using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Control;
using InfiniteCity.Model;
using InfiniteCity.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace InfiniteCity.Tests.Control
{
    ///<summary>
    ///  This is a test class for ControllerTest and is intended
    ///  to contain all ControllerTest Unit Tests
    ///</summary>
    [TestClass]
    public class ControllerTest
    {
        private Controller _control;

        /// <summary>
        /// Tests that a controller's spaces are correctly initialized
        /// </summary>
        [TestMethod]
        public void ControllerEdgesInitTest()
        {
            IEnumerable<Space> adjacencies = _control.Board.Tiles.SelectMany(tile => tile.Space.AdjacentSpaces);

            foreach (Space edge in _control.Board.Edges)
            {
                Assert.IsTrue(adjacencies.Contains(edge));
                Assert.IsNull(edge.Tile);
            }
        }

        /// <summary>
        /// Adds a tile and tests that the correct spaces where a tile can be placed are returned
        /// </summary>
        [TestMethod]
        public void GetPlacableSpacesTest()
        {
            IEnumerable<Space> spaces = _control.GetPlacableSpaces();

            IEnumerable<Space> edges = _control.Board.Edges;
            Assert.IsTrue(spaces.All(space => edges.Contains(space)));

            foreach (Space space in spaces)
                Assert.IsNull(_control.CanPlaceTile(new MockTile(), space));
        }

        /// <summary>
        /// Tests that a tile cannot be placed on a preoccupied space
        /// </summary>
        [TestMethod]
        public void PlaceTileOccupiedTileTest()
        {
            try
            {
                Space space = _control.GetPlacableSpaces(new MockTile()).First();
                _control.PlaceTile(new MockTile(), space);
                _control.PlaceTile(new MockTile(), space);

                Assert.Fail("Expected an Exception");
            }
            catch (InvalidOperationException) {}
        }

        /// <summary>
        /// Tests that a tile is correctly placed on the board at the correct space
        /// </summary>
        [TestMethod]
        public void PlaceTileTest()
        {
            Tile tile = new MockTile();
            Space space = _control.Board.Edges.First();
            _control.PlaceTile(tile, space);
            ReferenceEquals(space.Tile, tile);
            ReferenceEquals(tile, space.Tile);
        }

        /// <summary>
        /// Tests that a tile is successfully removed from the board
        /// </summary>
        [TestMethod]
        public void RemoveTestTile()
        {
            Tile tile = new MockTile();

            Space space = _control.GetPlacableSpaces(tile).First();
            _control.PlaceTile(tile, space);
            _control.RemoveTile(tile);

            Assert.AreEqual(tile.Space, null);
            Assert.AreEqual(space.Tile, null);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Controller.Initialize(new MockDeck());
            _control = Controller.DefaultInstance;
        }
    }
}