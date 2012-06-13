using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Control;
using InfiniteCity.Model;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InfiniteCity.View
{
    internal class BoardViewLayer
    {
        private readonly Controller _control;
        private readonly GraphicsDeviceManager _graphics;
        private Board _gameBoard;
        private IEnumerable<Tile> _tilesOnBoard;
        private Vector2 _tileDrawRectangleSize;

        public BoardViewLayer(GraphicsDeviceManager graphics, Controller control)
        {
            _graphics = graphics;
            _control = control;
        }

        public Texture2D Blank { get; set; }
        public Texture2D BlueOwner { get; set; }
        public Texture2D FlippedTile { get; set; }
        public Texture2D GreenOwner { get; set; }
        public Texture2D Holy { get; set; }
        public Texture2D Possible { get; set; }
        public Texture2D RedOwner { get; set; }
        public Texture2D SilverOwner { get; set; }
        public Texture2D TanOwner { get; set; }
        public Texture2D Worth1 { get; set; }
        public Texture2D Worth2 { get; set; }
        public Texture2D Worth3 { get; set; }
        public Texture2D Worth4 { get; set; }
        public Texture2D Worth5 { get; set; }
        public Texture2D YellowOwner { get; set; }

        public void Draw(SpriteBatch spriteBatch, Point currentPoint)
        {
            _gameBoard = _control.Board;
            _tilesOnBoard = _gameBoard.Tiles;
            foreach (Tile currTile in _tilesOnBoard)
            {
                // If there are now owners, draw the tile plain
                if (currTile.TokenCount() == 0)
                    spriteBatch.Draw(currTile.IsFlipped ? currTile.Texture : FlippedTile, GetSpacePos(currTile.Space), Color.White);
                else
                {
                    // first draw down the plain version
                    spriteBatch.Draw(currTile.Texture, GetSpacePos(currTile.Space), Color.White);
                    IPlayer[] players = currTile.Tokens.Keys.ToArray();

                    // If the active player owns this tile, make the entire tile that color
                    if (players.Contains(_control.ActivePlayer) && currTile.Tokens[_control.ActivePlayer] != 0)
                        spriteBatch.Draw(currTile.Texture, GetSpacePos(currTile.Space), _control.ActivePlayer.Color);
                    foreach (IPlayer player in players)
                        // display the ownship strips on the sides of the tile
                        switch (player.ColorName)
                        {
                            case ColorName.Red:
                                spriteBatch.Draw(RedOwner, GetSpacePos(currTile.Space), Color.White);
                                break;
                            case ColorName.Blue:
                                spriteBatch.Draw(BlueOwner, GetSpacePos(currTile.Space), Color.White);
                                break;
                            case ColorName.Yellow:
                                spriteBatch.Draw(YellowOwner, GetSpacePos(currTile.Space), Color.White);
                                break;
                            case ColorName.Green:
                                spriteBatch.Draw(GreenOwner, GetSpacePos(currTile.Space), Color.White);
                                break;
                            case ColorName.Tan:
                                spriteBatch.Draw(TanOwner, GetSpacePos(currTile.Space), Color.White);
                                break;
                            case ColorName.Silver:
                                spriteBatch.Draw(SilverOwner, GetSpacePos(currTile.Space), Color.White);
                                break;
                        }
                }
                if (currTile.Flags == Flags.Holy)
                    spriteBatch.Draw(Holy, GetSpacePos(currTile.Space), Color.White);
                if (currTile.Worth != 0 && currTile.IsFlipped)
                    switch (currTile.Worth)
                    {
                        case 1:
                            spriteBatch.Draw(Worth1, GetSpacePos(currTile.Space), Color.White);
                            break;
                        case 2:
                            spriteBatch.Draw(Worth2, GetSpacePos(currTile.Space), Color.White);
                            break;
                        case 3:
                            spriteBatch.Draw(Worth3, GetSpacePos(currTile.Space), Color.White);
                            break;
                        case 4:
                            spriteBatch.Draw(Worth4, GetSpacePos(currTile.Space), Color.White);
                            break;
                        case 5:
                            spriteBatch.Draw(Worth5, GetSpacePos(currTile.Space), Color.White);
                            break;
                    }
            }
            Space temp = _gameBoard[currentPoint];
            spriteBatch.Draw(Blank, GetSpacePos(temp), Color.SlateGray);

            Space[] goodSpaces = _control.GetPlacableSpaces().ToArray();
            foreach (Space currSpace in goodSpaces)
                spriteBatch.Draw(Possible, GetSpacePos(currSpace), Color.White);
        }

        public Rectangle GetSpacePos(Space currSpace)
        {
            const float tileHeight = 50;
            const float tileWidth = 60;
            float xOffset = (_graphics.PreferredBackBufferWidth/4f);
            float yOffset = (_graphics.PreferredBackBufferHeight/4f);
            _tileDrawRectangleSize = new Vector2(tileHeight, tileWidth);
            Vector2 cardPosition;

            if (currSpace.Location.Y == 0)
                cardPosition = new Vector2(xOffset+(tileHeight*(-1*currSpace.Location.X)), (yOffset+(tileHeight*currSpace.Location.Y)));
            else
                cardPosition =
                    new Vector2(
                        xOffset+(_tileDrawRectangleSize.X*(currSpace.Location.X*-1))+_tileDrawRectangleSize.X/2*currSpace.Location.Y,
                        (yOffset+(_tileDrawRectangleSize.Y*currSpace.Location.Y)-(_tileDrawRectangleSize.Y*currSpace.Location.Y)/4));

            var currentTileDrawRectangle = new Rectangle((int)(cardPosition.X),
                                                         (int)cardPosition.Y,
                                                         (int)_tileDrawRectangleSize.X+1,
                                                         (int)_tileDrawRectangleSize.Y+1);

            return currentTileDrawRectangle;
        }
    }
}