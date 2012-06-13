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
    ///  This is a test class for NeighborhoodTest and is intended
    ///  to contain all NeighborhoodTest Unit Tests
    ///</summary>
    [TestClass]
    public class NeighborhoodTest
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
            Tile otherTile = _control.Board.Tiles.First();

            var neighborhood = new Village();
            _control.PlaceTile(neighborhood, _control.GetPlacableSpaces().First());

            Selection actual = neighborhood.Transition(null);
            Assert.AreEqual(Selection.TileFromBoard, actual);

            actual = neighborhood.Transition(otherTile);
            Assert.AreEqual(Selection.None, actual);
        }
    }
}