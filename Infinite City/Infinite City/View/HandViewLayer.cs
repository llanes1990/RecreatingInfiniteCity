using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Control;
using InfiniteCity.Model;
using InfiniteCity.Model.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteCity.View
{
    /// <summary>
    ///   This class will handle the drawing of the layer that displays the players cards
    ///   on the screen
    /// </summary>
    internal class HandViewLayer
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly int _numTilesToDraw;
        private readonly int _lastSelectedTileIndex;
        private readonly Controller _controller;
        private Vector2 _tileDrawRectangleSize;
        private Vector2 _arrowDrawRectangleSize;
        private int _selectedTileIndex;
        private float _currentTileXOffset;

        /// <summary>
        ///   Initialize the HandViewLayer.
        /// </summary>
        public HandViewLayer(GraphicsDeviceManager graphics, Controller controller)
        {
            _graphics = graphics;

            //Initialize the number of cards to draw from the players hand at a given time.
            _numTilesToDraw = 5;

            //Scale the tile to a manageable size
            //Change from magic numbers to finalized values later
            _tileDrawRectangleSize = new Vector2(100, 100);

            // Scale arrow to be the right size
            _arrowDrawRectangleSize = new Vector2(100, 100);

            _lastSelectedTileIndex = 0;

            _controller = controller;

            _selectedTileIndex = _numTilesToDraw/2;
        }

        public Texture2D Cross { get; set; }
        public Texture2D Glow { get; set; }
        public bool IsCustom { get; set; }
        public Texture2D LeftArrow { get; set; }
        public Texture2D RightArrow { get; set; }

        public int SelectedTileIndex
        {
            get
            {
                if (_selectedTileIndex>_numTilesToDraw)
                    return _selectedTileIndex;
                if (_selectedTileIndex<=0)
                    return 0;
                return _selectedTileIndex;
            }
            set
            {
                int oldSelectedTileIndex = _selectedTileIndex;
                switch (_controller.Selection)
                {
                    case Selection.Player:
                    case Selection.Opponent:
                        if (value>=_controller.Players.Count())
                            _selectedTileIndex = _controller.Players.Count()-1;
                        else if (value<=0)
                            _selectedTileIndex = 0;
                        else
                            _selectedTileIndex = value;
                        break;
                    case Selection.TileFromCustom:
                        if (value>=_controller.TileSet.Count())
                            _selectedTileIndex = _controller.TileSet.Count()-1;
                        else if (value<=0)
                            _selectedTileIndex = 0;
                        else
                            _selectedTileIndex = value;
                        break;
                    default:
                        if (value>=_controller.ActivePlayer.Hand.Count())
                            _selectedTileIndex = _controller.ActivePlayer.Hand.Count()-1;
                        else if (value<=0)
                            _selectedTileIndex = 0;
                        else
                            _selectedTileIndex = value;
                        break;
                }
                _currentTileXOffset -= (_selectedTileIndex-oldSelectedTileIndex)*_tileDrawRectangleSize.X;
            }
        }

        public List<Tile> ValidPlayers { get; private set; }
        public Texture2D Worth1 { get; set; }
        public Texture2D Worth2 { get; set; }
        public Texture2D Worth3 { get; set; }
        public Texture2D Worth4 { get; set; }
        public Texture2D Worth5 { get; set; }

        /// <summary>
        ///   This will draw the player's hand to the given SpriteBatch
        ///   The selected tile will be drawn in the middle.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, List<Texture2D> tileTextures, int selectedTileIndex, int heightToDraw)
        {
            var middleCardPosition = new Vector2((_graphics.PreferredBackBufferWidth/2f)-(_tileDrawRectangleSize.X/2)-_currentTileXOffset,
                                                 heightToDraw);
            _currentTileXOffset = (int)_currentTileXOffset/1.3F;

            for (int currentTileIndex = Math.Max(0, selectedTileIndex-(_numTilesToDraw/2));
                 (currentTileIndex<=selectedTileIndex+Math.Ceiling(_numTilesToDraw/2f) && currentTileIndex<tileTextures.Count());
                 currentTileIndex++)
            {
                int indexOffsetFromSelectedTileIndex = currentTileIndex-selectedTileIndex;
                var currentTileDrawRectangle =
                    new Rectangle((int)(middleCardPosition.X+(_tileDrawRectangleSize.X*indexOffsetFromSelectedTileIndex)),
                                  (int)middleCardPosition.Y,
                                  (int)_tileDrawRectangleSize.X,
                                  (int)_tileDrawRectangleSize.Y);

                // "fun" section of code to prevent the coloring of the hand during player selection
                if (!IsCustom || (_controller.Selection != Selection.Player && _controller.Selection != Selection.Opponent))
                {
                    spriteBatch.Draw(tileTextures[currentTileIndex], currentTileDrawRectangle, null, Color.White);

                    if (currentTileIndex == SelectedTileIndex)
                        spriteBatch.Draw(Glow, currentTileDrawRectangle, null, Color.Gray);

                    // get the tile that we are currently looking at
                    Tile currTile = _controller.Selection == Selection.TileFromCustom
                                        ? _controller.TileSet[currentTileIndex]
                                        : _controller.ActivePlayer.Hand.ElementAt(currentTileIndex);

                    // if the tile has a worth, draw it
                    if (currTile.Worth != 0)
                        switch (currTile.Worth)
                        {
                            case 1:
                                spriteBatch.Draw(Worth1, currentTileDrawRectangle, Color.White);
                                break;
                            case 2:
                                spriteBatch.Draw(Worth2, currentTileDrawRectangle, Color.White);
                                break;
                            case 3:
                                spriteBatch.Draw(Worth3, currentTileDrawRectangle, Color.White);
                                break;
                            case 4:
                                spriteBatch.Draw(Worth4, currentTileDrawRectangle, Color.White);
                                break;
                            case 5:
                                spriteBatch.Draw(Worth5, currentTileDrawRectangle, Color.White);
                                break;
                        }
                }
                else
                {
                    spriteBatch.Draw(tileTextures[currentTileIndex],
                                     currentTileDrawRectangle,
                                     null,
                                     ValidPlayers[currentTileIndex].Owner.Color);
                    if (currentTileIndex == SelectedTileIndex)
                        spriteBatch.Draw(Glow, currentTileDrawRectangle, Color.Gray);

                    //if (_controller.ActivePlayer == _controller.Players.ElementAt(currentTileIndex))
                    //    spriteBatch.Draw(cross, currentTileDrawRectangle, null, Color.White);
                }
            }

            // Check if arrows should be displayed
            bool drawLeftArrow = false, drawRightArrow = false;
            int numTilesFromMiddle = _numTilesToDraw/2;
            if (SelectedTileIndex>numTilesFromMiddle)
                drawLeftArrow = true;

            int handSize = _controller.ActivePlayer.HandSize;
            if (SelectedTileIndex+numTilesFromMiddle+1<handSize)
                drawRightArrow = true;

            // If left arrow should be displayed
            if (drawLeftArrow)
            {
                var leftArrowRectangle = new Rectangle(
                    (int)(middleCardPosition.X-(_tileDrawRectangleSize.X*2)-10-_arrowDrawRectangleSize.X),
                    (int)middleCardPosition.Y,
                    (int)_arrowDrawRectangleSize.X,
                    (int)_tileDrawRectangleSize.Y);
                spriteBatch.Draw(LeftArrow, leftArrowRectangle, Color.White);
            }
            if (!drawRightArrow)
                return;
            var rightArrowRectange = new Rectangle((int)(middleCardPosition.X+(_tileDrawRectangleSize.X*3)+10),
                                                   (int)middleCardPosition.Y,
                                                   (int)_arrowDrawRectangleSize.X,
                                                   (int)_tileDrawRectangleSize.Y);
            spriteBatch.Draw(RightArrow, rightArrowRectange, Color.White);
        }

        public void Draw(SpriteBatch spriteBatch, IEnumerable<Tile> hand, int heightToDraw)
        {
            int selectedTileIndex = _lastSelectedTileIndex;

            if (_controller.Selection == Selection.Opponent && IsCustom)
                ValidPlayers = hand.Where(t => t.Owner != _controller.ActivePlayer).ToList();
            else
                ValidPlayers = hand.ToList();

            List<Texture2D> tileTextures = ValidPlayers.Select(t => t.Texture).ToList();
            Draw(spriteBatch, tileTextures, selectedTileIndex, heightToDraw);
        }

        public void Draw(SpriteBatch spriteBatch, IEnumerable<Tile> hand, int selectedTileIndex, int heightToDraw)
        {
            if (_controller.Selection == Selection.Opponent && IsCustom)
                ValidPlayers = hand.Where(t => t.Owner != _controller.ActivePlayer).ToList();
            else if (_controller.Selection == Selection.Player && IsCustom)
                ValidPlayers = hand.Where(t => _controller.PlayerSet.Contains(t.Owner)).ToList();
            else
                ValidPlayers = hand.ToList();

            List<Texture2D> tileTextures = ValidPlayers.Select(t => t.Texture).ToList();
            Draw(spriteBatch, tileTextures, selectedTileIndex, heightToDraw);
        }
    }
}