using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using InfiniteCity.Control.Decks;
using InfiniteCity.Model;
using InfiniteCity.Tests.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfiniteCity.Tests.Control.Decks
{
    [TestClass]
    public class FiniteDeckTest
    {
        //checking push and  pop function
        [TestMethod]
        public void PushPopTest()
        {
            var deck = new FiniteDeck(6);
            Tile test_tile = new MockTile();
            deck.Push(test_tile);
            Assert.AreEqual(deck.Pop(), test_tile);
        }

        //checking remaining tile function
        [TestMethod]
        public void RemainingTest()
        {
            var deck = new FiniteDeck(6);
            int initial = deck.ToArray().Length;
            deck.Pop();
            Assert.AreEqual(deck.Remaining, initial-1);
        }

        [TestMethod]
        public void ShuffleTest()
        {
            var deck = new FiniteDeck(6);
            Tile test_tile = new MockTile();
            deck.Enqueue(test_tile);
            deck.Shuffle(50);
            Tile[] array = deck.ToArray();

            var deck2 = new FiniteDeck(6);

            deck2.Enqueue(test_tile);
            deck2.Shuffle(50);
            Tile[] array2 = deck2.ToArray();

            //CollectionAssert.AreEqual(array, array2);

            int compare_value1 = -1;

            for (int i = 0; i<array.Length; i++)
                if (array[i] == test_tile)
                    compare_value1 = i;

            int compare_value2 = -1;
            for (int i = 0; i<array2.Length; i++)
                if (array2[i] == test_tile)
                    compare_value2 = i;

            Assert.AreEqual(compare_value1, compare_value2);
        }
    }
}