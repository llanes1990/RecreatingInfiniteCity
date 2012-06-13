using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Control;
using InfiniteCity.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace InfiniteCity.Tests.Model
{
    [TestClass]
    public class BoardTest
    {
        private static readonly int[,] Offsets = new[,] {{0, 1}, {1, 1}, {1, 0}, {0, -1}, {-1, -1}, {-1, 0}};

        public TestContext TestContext { get; set; }

        public static IEnumerable<Point> Adjacencies(Point point)
        {
            for (int i = 0; i<Offsets.GetLength(0); i++)
                yield return new Point(point.X+Offsets[i, 0], point.Y+Offsets[i, 1]);
        }

        public static IEnumerable<Point> Adjacencies(int x, int y)
        {
            for (int i = 0; i<Offsets.GetLength(0); i++)
                yield return new Point(x+Offsets[i, 0], y+Offsets[i, 1]);
        }

        /// <summary>
        /// Tests that a Board has been initialized properly has no tiles
        /// </summary>
        [TestMethod]
        public void BoardConstructorTest()
        {
            var board = new Board();
            Assert.AreEqual(0, board.Edges.Count());
            Assert.AreEqual(0, board.Tiles.Count());
        }

        /// <summary>
        /// Tests that an arbitrary Space is properly added to a Board and contains the right references
        /// </summary>
        [TestMethod]
        public void BoardFarFarAwayTest()
        {
            var board = new Board();
            var point = new Point(48135, -2967);
            Space space = board[point];

            Assert.AreSame(board, space.Board);
            Assert.AreEqual(point, space.Location);
        }

        /// <summary>
        /// Tests that a space at the origin is properly added to a Board and contains the right references
        /// </summary>
        [TestMethod]
        public void BoardOriginTest()
        {
            var board = new Board();
            var point = new Point(0, 0);
            Space space = board[point];

            Assert.AreSame(board, space.Board);
            Assert.AreEqual(point, space.Location);
        }

        /// <summary>
        /// Tests that adjacent Tiles are added to a Board and contain the correct adjacent pointers
        /// </summary>
        [TestMethod]
        public void EdgeAdjacentTest()
        {
            var board = new Board();

            var points = new[] {new Point(0, 0), new Point(1, 1)};
            new MockTile {Space = board[points[0]]};
            new MockTile {Space = board[points[1]]};
            Point[] actual = board.Edges.Select(space => space.Location).ToArray();

            Point[] expected = Adjacencies(points[0]).Union(Adjacencies(points[1])).Except(points).ToArray();
            CollectionAssert.AreEquivalent(expected, actual);
        }

        /// <summary>
        /// Tests that placed tiles are not adjacent
        /// </summary>
        [TestMethod]
        public void EdgeDisjointTest()
        {
            var board = new Board();

            var points = new[] {new Point(0, 0), new Point(48135, -2967)};
            new MockTile {Space = board[points[0]]};
            new MockTile {Space = board[points[1]]};
            Point[] actual = board.Edges.Select(space => space.Location).ToArray();

            Point[] expected = Adjacencies(points[0]).Union(Adjacencies(points[1])).Except(points).ToArray();
            CollectionAssert.AreEquivalent(expected, actual);
        }

        /// <summary>
        /// Tests that a shared edge is only returned once
        /// </summary>
        [TestMethod]
        public void EdgeOverlappingTest()
        {
            var board = new Board();

            var points = new[] {new Point(0, 0), new Point(2, 2)};
            new MockTile {Space = board[points[0]]};
            new MockTile {Space = board[points[1]]};
            Point[] actual = board.Edges.Select(space => space.Location).ToArray();

            Point[] expected = Adjacencies(points[0]).Union(Adjacencies(points[1])).Except(points).ToArray();
            CollectionAssert.AreEquivalent(expected, actual);
        }

        /// <summary>
        /// Tests that edges are returned correctly after initial tile placement
        /// </summary>
        [TestMethod]
        public void EdgeSimpleTest()
        {
            var board = new Board();

            new MockTile {Space = board[0, 0]};
            Point[] actual = board.Edges.Select(space => space.Location).ToArray();

            CollectionAssert.AreEquivalent(Adjacencies(0, 0).ToArray(), actual);
        }

        /// <summary>
        /// Tests that tile and space references are updated correctly
        /// </summary>
        [TestMethod]
        public void TilePlacementTest()
        {
            var board = new Board();

            Space space = board[0, 0];
            var tile = new MockTile {Space = space};

            Assert.AreSame(space, tile.Space);
            Assert.AreSame(tile, space.Tile);
        }
    }
}