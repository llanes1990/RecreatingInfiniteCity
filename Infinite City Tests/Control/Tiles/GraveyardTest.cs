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
    ///  This is a test class for GraveyardTest and is intended
    ///  to contain all GraveyardTest Unit Tests
    ///</summary>
    [TestClass]
    public class GraveyardTest
    {
        private Controller _control;

        [TestInitialize]
        public void TestInitialize()
        {
            Controller.Initialize(new MockDeck());
            _control = Controller.DefaultInstance;
        }

        ///<summary>
        ///  A test for Transition
        ///</summary>
        [TestMethod]
        public void TransitionTest()
        {
            var target = new Graveyard();

            Tile tile = new MockTile();

            _control.PlaceTile(tile, _control.GetPlacableSpaces().First());
            _control.PlaceTile(target, tile.Space.AdjacentSpaces.First());

            Selection actual = target.Transition(null);
            Assert.AreEqual(Selection.TileFromBoard, actual);

            actual = target.Transition(tile);
            Assert.AreEqual(Selection.None, actual);

            Assert.AreEqual(null, tile.Space);

            Assert.IsTrue(_control.ActivePlayer.Hand.Contains(tile));
        }
    }
}