using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Control;
using InfiniteCity.Control.Tiles;
using InfiniteCity.Model;
using InfiniteCity.Model.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace InfiniteCity.Tests.Control.Tiles
{
    ///<summary>
    ///  This is a test class for WarehouseTest and is intended
    ///  to contain all WarehouseTest Unit Tests
    ///</summary>
    [TestClass]
    public class WarehouseTest
    {
        private Controller _control;

        [TestInitialize]
        public void TestInitialize()
        {
            Controller.Initialize();
            _control = Controller.DefaultInstance;
        }

        ///<summary>
        ///  A test for Transition
        ///</summary>
        [TestMethod]
        public void TransitionTest()
        {
            var target = new Plantation();

            _control.PlaceTile(target, _control.GetPlacableSpaces(target).First());

            Player player = _control.ActivePlayer;

            Selection actual = target.Transition(null);
            Assert.AreEqual(Selection.None, actual);

            Assert.IsTrue(player.HandSize>=10);
        }
    }
}