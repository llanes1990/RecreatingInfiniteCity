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
using Microsoft.Xna.Framework;

namespace InfiniteCity.Tests.Control.Tiles
{
    ///<summary>
    ///  This is a test class for MarketTest and is intended
    ///  to contain all MarketTest Unit Tests
    ///</summary>
    [TestClass]
    public class MarketTest
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
            var market = new Market();
            _control.PlaceTile(market, _control.GetPlacableSpaces().First());

            Selection actual = market.Transition(null);
            Assert.AreEqual(Selection.Opponent, actual);

            var player = new Player("Player 2", Color.Blue, ColorName.Blue);
            actual = market.Transition(player);
            Assert.AreEqual(Selection.None, actual);
        }
    }
}