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
    ///  This is a test class for SanctumTest and is intended
    ///  to contain all SanctumTest Unit Tests
    ///</summary>
    [TestClass]
    public class SanctumTest
    {
        [TestInitialize]
        public void TestInitialize()
        {
            Controller.Initialize(new MockDeck());
        }

        ///<summary>
        ///  A test for Transition
        ///</summary>
        [TestMethod]
        public void TransitionTest()
        {
            var sanctum = new Cathedral();
            Controller controller = Controller.DefaultInstance;

            Selection actual = sanctum.Transition(null);
            Assert.AreEqual(Selection.TileFromCustom, actual);

            Tile selection = controller.TileSet.First();
            actual = sanctum.Transition(selection);
            Assert.AreEqual(Selection.PlayableSpace, actual);

            Space space = controller.GetPlacableSpaces().First();
            sanctum.Transition(space);
        }
    }
}