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
    ///  This is a test class for GuildHallTest and is intended
    ///  to contain all GuildHallTest Unit Tests
    ///</summary>
    [TestClass]
    public class GuildHallTest
    {
        private Controller _control;

        [TestInitialize]
        public void TestInitialize()
        {
            Controller.Initialize(new MockDeck());
            _control = Controller.DefaultInstance;
        }

        [TestMethod]
        public void TestNewTileIsPlaced()
        {
            var target = new Quarry();
            Tile tile = new MockTile();
            Space space = _control.GetPlacableSpaces(tile).First();

            target.Transition(null);
            target.Transition(tile);
            target.Transition(space);

            Assert.AreEqual(tile, space.Tile);
        }

        [TestMethod]
        public void TestTileIsRemoved()
        {
            var target = new Quarry();

            Selection actual = target.Transition(null);
            Assert.AreEqual(Selection.TileFromHand, actual);

            Player player = _control.ActivePlayer;
            Tile tile = player.Hand.First();
            actual = target.Transition(tile);
            Assert.AreEqual(Selection.PlayableSpace, actual);

            target.Transition(_control.GetPlacableSpaces(tile).First());
            Assert.AreEqual(tile, _control.ActiveTile);

            Assert.IsFalse(player.Hand.Contains(tile));
        }

        ///<summary>
        ///  A test for Transition
        ///</summary>
        [TestMethod]
        public void TransitionTest()
        {
            var target = new Quarry();
            Tile tile = new MockTile();

            Selection actual = target.Transition(null);
            Assert.AreEqual(Selection.TileFromHand, actual);

            actual = target.Transition(tile);
            Assert.AreEqual(Selection.PlayableSpace, actual);

            target.Transition(_control.GetPlacableSpaces(tile).First());
            Assert.AreEqual(tile, _control.ActiveTile);
        }
    }
}