using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Control;
using InfiniteCity.Control.Tiles;
using InfiniteCity.Model;
using InfiniteCity.Model.Enums;
using InfiniteCity.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfiniteCity.Tests.Control.Tiles
{
    ///<summary>
    ///  This is a test class for GroveTest and is intended
    ///  to contain all GroveTest Unit Tests
    ///</summary>
    [TestClass]
    public class GroveTest
    {
        private Controller _control;

        [TestMethod]
        public void CanOnlyRemoveYourTokens()
        {
            //place the grove
            var grove = new Grove();
            _control.PlaceTile(grove, _control.GetPlacableSpaces(grove).First());

            //place a target tile to remove tokens from
            Tile tile = new MockTile();

            _control.PlaceTile(tile, _control.GetPlacableSpaces(tile).First());

            //start grove
            Selection actual = grove.Transition(null);
            Assert.AreEqual(Selection.TileFromBoard, actual);

            _control.ActivePlayer = _control.Players.Where(player => player != _control.ActivePlayer).First();
            try
            {
                grove.Transition(tile);
                Assert.Fail("Expected an Exception");
            }
            catch (ArgumentException) {}
        }

        [TestMethod]
        public void MultipleRemoveToken()
        {
            //place the grove
            var grove = new Grove();
            _control.PlaceTile(grove, _control.GetPlacableSpaces(grove).First());

            //place a target tile to remove tokens from
            Tile tile = new MockTile();

            _control.PlaceTile(tile, _control.GetPlacableSpaces(tile).First());

            //start grove
            grove.Transition(null);
            try
            {
                grove.Transition(tile);
                grove.Transition(tile);
                Assert.Fail("Expected an Exception");
            }
            catch (ArgumentException) {}
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Controller.Initialize(new MockDeck());
            _control = Controller.DefaultInstance;
        }

        [TestMethod]
        public void TileMustHaveToken()
        {
            //place the grove
            var grove = new Grove();
            _control.PlaceTile(grove, _control.GetPlacableSpaces(grove).First());

            //start grove
            Selection actual = grove.Transition(null);
            Assert.AreEqual(Selection.TileFromBoard, actual);

            _control.ActivePlayer = _control.Players.Where(player => player != _control.ActivePlayer).First();
            try
            {
                grove.Transition(_control.Board.Tiles.First());
                Assert.Fail("Expected an Exception");
            }
            catch (ArgumentException) {}
        }

        ///<summary>
        ///  A test for Transition
        ///</summary>
        [TestMethod]
        public void TransitionTest()
        {
            //place the grove
            var grove = new Grove();
            _control.PlaceTile(grove, _control.GetPlacableSpaces(grove).First());

            //place a target tile to remove tokens from
            Tile[] tiles = {new MockTile(), new MockTile(), new MockTile()};

            foreach (Tile tile in tiles)
                _control.PlaceTile(tile, _control.GetPlacableSpaces(tile).First());

            Selection actual = grove.Transition(null);
            foreach (Tile tile in tiles)
            {
                Assert.AreEqual(Selection.TileFromBoard, actual);
                actual = grove.Transition(tile);
            }
            Assert.AreEqual(Selection.None, actual);
        }
    }
}