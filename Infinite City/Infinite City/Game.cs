using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfiniteCity.Control;
using InfiniteCity.Control.AI;
using InfiniteCity.Control.Tiles;
using InfiniteCity.Model;
using InfiniteCity.Model.Enums;
using InfiniteCity.Model.Exceptions;
using InfiniteCity.Model.Interfaces;
using InfiniteCity.View;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace InfiniteCity
{
    /// <summary>
    ///   This is the main type for your game
    /// </summary>
    internal sealed class Game : Microsoft.Xna.Framework.Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _handSpriteBatch;
        private SpriteBatch _boardSpriteBatch;
        private SpriteBatch _scoreSpriteBatch;
        private SpriteBatch _endGameSpriteBatch;
        private SpriteBatch _menuSpriteBatch;

        private SpriteBatch _tooltipSpriteBatch;
        private int _tooltipOffset;

        private Controller _controller;
        private SpriteFont _seguiUI;

        /// <summary>
        ///   View classes should only access controller function defined in ISelectionManager
        /// </summary>
        private ISelectionManager _selectionManager;

        private KeyboardState _oldKeyboardState;

        private HandViewLayer _handViewLayer;
        private HandViewLayer _customViewLayer;
        private BoardViewLayer _boardViewLayer;
        private MenuViewLayer _menuViewLayer;

        private Camera _camera;
        private Texture2D _line;
        private Texture2D _white;
        private Texture2D _gray;

        private Tile[] _currentHand;
        private Tile[] _playerSelection;
        private Point _selectionPoint;

        private Boolean _moveCamera;

        private Color _oldColor;
        private Boolean _displayHelp;

        private Boolean _endGame;

        private Menu _menu;

        private Screen _currentScreen;
        private List<Tuple<string, int>> _aiParams;

        /// <summary>
        ///   Creates the game
        /// </summary>
        public Game()
        {
            _graphics = new GraphicsDeviceManager(this) {PreferredBackBufferWidth = 1280, PreferredBackBufferHeight = 720};

            //Use 720p resolution for now.
            //May change later

            Content.RootDirectory = "Content";
        }

        /// <summary>
        ///   Creates the players' source of inputs
        /// </summary>
        /// <param name = "inputParams">A list of Tuples to create the player source of inputs</param>
        public static Player[] InitPlayerInputs(List<Tuple<string, int>> inputParams)
        {
            var players = new List<Player>();
            int playerIndex = 0;
            for (int inputParamsIndex = 0; inputParamsIndex<inputParams.Count(); inputParamsIndex++)
                if (inputParams[inputParamsIndex].Item1 != "Disabled")
                {
                    players.Add(new Player(inputParams[inputParamsIndex].Item1+(inputParamsIndex+1),
                                           Controller.PlayerColors[inputParamsIndex],
                                           (ColorName)inputParamsIndex));
                    if (inputParams[inputParamsIndex].Item1 != "Human")
                    {
                        int index = inputParamsIndex;
                        Func<int, AI> inputSource =
                            (from ai in AI.Opponents where ai.Key == inputParams[index].Item1 select ai.Value).ElementAt(0);
                        players[playerIndex].Input = inputSource(inputParams[inputParamsIndex].Item2);
                    }
                    players[playerIndex].Name = inputParams[inputParamsIndex].Item1+" "+(inputParamsIndex+1);
                    playerIndex++;
                }

            return players.ToArray();
        }

        /// <summary>
        ///   This is called when the game should draw itself.
        /// </summary>
        /// <param name = "gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            switch (_currentScreen)
            {
                case Screen.Menu:
                    _menuSpriteBatch.Begin();
                    _menuViewLayer.Draw(_menuSpriteBatch);
                    _menuSpriteBatch.End();
                    break;
                default:
                    DrawGameView(gameTime);
                    break;
            }
            base.Draw(gameTime);
        }

        /// <summary>
        ///   Allows the game to perform any initialization it needs to before starting to run.
        ///   This is where it can query for any required services and load any non-graphic
        ///   related content.  Calling base.Initialize will enumerate through any components
        ///   and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _currentScreen = Screen.Menu;

            IsMouseVisible = true;
            _camera = new Camera(_graphics);

            // Why is the board starting at (1600,1100)? No one knows....
            _camera.PointTo(new Vector2(1600, 1100));

            _controller = Controller.DefaultInstance;

            _selectionManager = _controller;

            _oldKeyboardState = Keyboard.GetState();

            _boardViewLayer = new BoardViewLayer(_graphics, _controller);

            _handViewLayer = new HandViewLayer(_graphics, _controller) {IsCustom = false};

            _customViewLayer = new HandViewLayer(_graphics, _controller) {IsCustom = true};

            _menu = new Menu();

            // a blank tile for each player, colors get added later
            _playerSelection = new Tile[_controller.Players.Count()];
            for (int i = 0; i<_controller.Players.Count(); i++)
                _playerSelection[i] = new Blank {Owner = _controller.Players.ElementAt(i), Space = new Space()};
            _selectionPoint = new Point(0, 0);

            _moveCamera = true;

            _oldColor = Color.Wheat;

            _displayHelp = false;

            _endGame = false;

            _aiParams = null;

            base.Initialize();
        }

        /// <summary>
        ///   LoadContent will be called once per game and is the place to load
        ///   all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _seguiUI = Content.Load<SpriteFont>("Fonts\\SeguiUI");
            _menuViewLayer = new MenuViewLayer(_menu, _seguiUI);

            // textures for tiles
            Crossroads.TileTexture = Content.Load<Texture2D>("Tiles\\PlazaTile");
            Grove.TileTexture = Content.Load<Texture2D>("Tiles\\Complete\\Woodlands");
            Quarry.TileTexture = Content.Load<Texture2D>("Tiles\\Complete\\Tile_Quarry");
            Graveyard.TileTexture = Content.Load<Texture2D>("Tiles\\MausoleumTile");
            Market.TileTexture = Content.Load<Texture2D>("Tiles\\ShoppingComplexTile");
            Village.TileTexture = Content.Load<Texture2D>("Tiles\\HousingTile");
            Cathedral.TileTexture = Content.Load<Texture2D>("Tiles\\LibraryTile");
            Plantation.TileTexture = Content.Load<Texture2D>("Tiles\\FactoryTile");
            Plague.TileTexture = Content.Load<Texture2D>("Tiles\\SecurityGuardpostTile");
            Militia.TileTexture = Content.Load<Texture2D>("Tiles\\EmbassyTile");
            Amphitheater.TileTexture = Content.Load<Texture2D>("Tiles\\PoliceHQTile");
            ReflectingPond.TileTexture = Content.Load<Texture2D>("Tiles\\PostOfficeTile");
            AbandonedMine.TileTexture = Content.Load<Texture2D>("Tiles\\TransitStationTile");
            Wasteland.TileTexture = Content.Load<Texture2D>("Tiles\\Complete\\Tile_Wasteland");
            Spring.TileTexture = Content.Load<Texture2D>("Tiles\\PortTile");
            ThievesGuild.TileTexture = Content.Load<Texture2D>("Tiles\\TempleTile");
            Keep.TileTexture = Content.Load<Texture2D>("Tiles\\Complete\\Tile_Keep");
            Sanctum.TileTexture = Content.Load<Texture2D>("Tiles\\PowerStationTile");
            Treasury.TileTexture = Content.Load<Texture2D>("Tiles\\Complete\\Tile_Treasury");
            Tavern.TileTexture = Content.Load<Texture2D>("Tiles\\StadiumTile");

            Blank.TileTexture = Content.Load<Texture2D>("Tiles\\BaseTile");

            // load the arrows for the hand view layer
            _handViewLayer.LeftArrow = Content.Load<Texture2D>("LeftArrow");
            _handViewLayer.RightArrow = Content.Load<Texture2D>("RightArrow");
            _handViewLayer.Glow = Content.Load<Texture2D>("Tiles\\BaseTileGlow");
            _customViewLayer.Glow = Content.Load<Texture2D>("Tiles\\BaseTileGlow");
            _customViewLayer.LeftArrow = Content.Load<Texture2D>("LeftArrow");
            _customViewLayer.RightArrow = Content.Load<Texture2D>("RightArrow");

            // texture for the selector
            _boardViewLayer.Blank = Content.Load<Texture2D>("Tiles\\BaseTileGlow");
            _boardViewLayer.Holy = Content.Load<Texture2D>("Tiles\\holy");

            // texture for playable spaces
            _boardViewLayer.Possible = Content.Load<Texture2D>("Tiles\\good");

            // textures for ownership, pass them to the boardViewLayer
            _boardViewLayer.RedOwner = Content.Load<Texture2D>("Tiles\\Ownership\\BaseTerritoryColorRed");
            _boardViewLayer.BlueOwner = Content.Load<Texture2D>("Tiles\\Ownership\\BaseTerritoryColorBlue");
            _boardViewLayer.YellowOwner = Content.Load<Texture2D>("Tiles\\Ownership\\BaseTerritoryColorYellow");
            _boardViewLayer.GreenOwner = Content.Load<Texture2D>("Tiles\\Ownership\\BaseTerritoryColorGreen");
            _boardViewLayer.TanOwner = Content.Load<Texture2D>("Tiles\\Ownership\\BaseTerritoryColorTan");
            _boardViewLayer.SilverOwner = Content.Load<Texture2D>("Tiles\\Ownership\\BaseTerritoryColorSilver");
            _boardViewLayer.FlippedTile = Content.Load<Texture2D>("Tiles\\FlippedTile");

            // textures for tile worth in board display
            _boardViewLayer.Worth1 = Content.Load<Texture2D>("Tiles\\Worth1");
            _boardViewLayer.Worth2 = Content.Load<Texture2D>("Tiles\\Worth2");
            _boardViewLayer.Worth3 = Content.Load<Texture2D>("Tiles\\Worth3");
            _boardViewLayer.Worth4 = Content.Load<Texture2D>("Tiles\\Worth4");
            _boardViewLayer.Worth5 = Content.Load<Texture2D>("Tiles\\Worth5");

            // textures for tile worth in hand display
            _handViewLayer.Worth1 = _boardViewLayer.Worth1;
            _handViewLayer.Worth2 = _boardViewLayer.Worth2;
            _handViewLayer.Worth3 = _boardViewLayer.Worth3;
            _handViewLayer.Worth4 = _boardViewLayer.Worth4;
            _handViewLayer.Worth5 = _boardViewLayer.Worth5;

            _customViewLayer.Worth1 = _boardViewLayer.Worth1;
            _customViewLayer.Worth2 = _boardViewLayer.Worth2;
            _customViewLayer.Worth3 = _boardViewLayer.Worth3;
            _customViewLayer.Worth4 = _boardViewLayer.Worth4;
            _customViewLayer.Worth5 = _boardViewLayer.Worth5;

            _customViewLayer.Cross = Content.Load<Texture2D>("Tiles\\cross");

            // Create a new SpriteBatch, which can be used to draw textures.
            _handSpriteBatch = new SpriteBatch(GraphicsDevice);
            _boardSpriteBatch = new SpriteBatch(GraphicsDevice);
            _scoreSpriteBatch = new SpriteBatch(GraphicsDevice);
            _tooltipSpriteBatch = new SpriteBatch(GraphicsDevice);
            _endGameSpriteBatch = new SpriteBatch(GraphicsDevice);
            _menuSpriteBatch = new SpriteBatch(GraphicsDevice);

            //for drawing boarder between hand and board
            _line = Content.Load<Texture2D>("black");
            _white = Content.Load<Texture2D>("white");
            _gray = Content.Load<Texture2D>("gray");
            Content.Load<Texture2D>("board");

            MediaPlayer.Play(Content.Load<Song>("477450_Our-Ancestors-Menu-")); // http://www.newgrounds.com/audio/listen/477450
            MediaPlayer.IsRepeating = true;
            MediaPlayer.IsMuted = true;
        }

        /// <summary>
        ///   UnloadContent will be called once per game and is the place to unload
        ///   all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        ///   Allows the game to run logic such as updating the world,
        ///   checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name = "gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState newKeyboardState = Keyboard.GetState();
            switch (_currentScreen)
            {
                case Screen.Menu:
                    UpdateMenuScreen(newKeyboardState);
                    break;
                default:
                    UpdateGameScreen(newKeyboardState);
                    break;
            }
            _oldKeyboardState = newKeyboardState;
            base.Update(gameTime);
        }

        private void CheckEsc(KeyboardState newKeyboardState)
        {
            if (newKeyboardState.IsKeyDown(Keys.Escape))
                Exit();
        }

        private void CheckMute(KeyboardState newKeyboardState)
        {
            Keys[] oldStateKeys = _oldKeyboardState.GetPressedKeys();

            // Toggle for media player
            if (newKeyboardState.IsKeyDown(Keys.M) && !oldStateKeys.Contains(Keys.M))
                MediaPlayer.IsMuted = !MediaPlayer.IsMuted;
        }

        private void DrawBorder()
        {
            var outerTileDrawRectangle = new Rectangle(10,
                                                       _graphics.PreferredBackBufferHeight*4/5-10,
                                                       _graphics.PreferredBackBufferWidth-20,
                                                       _graphics.PreferredBackBufferHeight-5-_graphics.PreferredBackBufferHeight*4/5);

            var innerTileDrawRectangle = new Rectangle(20,
                                                       _graphics.PreferredBackBufferHeight*4/5,
                                                       _graphics.PreferredBackBufferWidth-40,
                                                       _graphics.PreferredBackBufferHeight-25-_graphics.PreferredBackBufferHeight*4/5);

            _handSpriteBatch.Draw(_line, outerTileDrawRectangle, Color.White);
            _handSpriteBatch.Draw(_white, innerTileDrawRectangle, Color.White);
        }

        private void DrawGameView(GameTime gameTime)
        {
            if (_controller.ActivePlayer.Input == null)
                _oldColor = _controller.ActivePlayer.Color;
            GraphicsDevice.Clear(_oldColor);

            // draw the tiles on the board
            _boardSpriteBatch.Begin(SpriteSortMode.Deferred,
                                    BlendState.AlphaBlend,
                                    SamplerState.LinearClamp,
                                    DepthStencilState.None,
                                    RasterizerState.CullCounterClockwise,
                                    null,
                                    _camera.GetTransformation());

            _boardViewLayer.Draw(_boardSpriteBatch, _selectionPoint);

            _boardSpriteBatch.End();

            if (_controller.ActivePlayer.Input == null)
                DrawHumanView();
            if (_endGame)
            {
                _endGameSpriteBatch.Begin(SpriteSortMode.Deferred,
                                          BlendState.AlphaBlend,
                                          SamplerState.LinearClamp,
                                          DepthStencilState.None,
                                          RasterizerState.CullCounterClockwise,
                                          null,
                                          Matrix.Identity);
                _endGameSpriteBatch.DrawString(_seguiUI,
                                               _selectionManager.Message,
                                               new Vector2(
                                                   _graphics.PreferredBackBufferWidth/2f-
                                                   _seguiUI.MeasureString(_selectionManager.Message).X/2,
                                                   0),
                                               Color.Black);
                _endGameSpriteBatch.End();
            }

            int offset = 2;

            // Draw the scoreboard and scores
            _scoreSpriteBatch.Begin(SpriteSortMode.Deferred,
                                    BlendState.AlphaBlend,
                                    SamplerState.LinearClamp,
                                    DepthStencilState.None,
                                    RasterizerState.CullCounterClockwise,
                                    null,
                                    Matrix.Identity);

            _scoreSpriteBatch.Draw(_white,
                                   new Rectangle(_graphics.PreferredBackBufferWidth*13/16,
                                                 0,
                                                 _graphics.PreferredBackBufferWidth*3/16,
                                                 18*(_controller.Players.ToArray().Length+3)),
                                   Color.Black);

            _scoreSpriteBatch.DrawString(_seguiUI, "SCOREBOARD", new Vector2(_graphics.PreferredBackBufferWidth*13/16+5, 0), Color.Gold);
            _scoreSpriteBatch.DrawString(_seguiUI,
                                         string.Format("{0,8}:{1,4}:{2,4}", "Name", "Pts", "Tkns"),
                                         new Vector2(_graphics.PreferredBackBufferWidth*13/16+5, 15),
                                         Color.White);
            int winning = _controller.Score.Max(kvp => kvp.Value);
            foreach (Player p in _controller.Players)
            {
                _scoreSpriteBatch.DrawString(_seguiUI,
                                             string.Format("{0,8}:{1,4}:{3,4}{2}",
                                                           p.Name,
                                                           _controller.Score[p],
                                                           _controller.Score[p] == winning && winning != 0 ? "!" : string.Empty,
                                                           p.Tokens),
                                             new Vector2(_graphics.PreferredBackBufferWidth*13/16+5, 15*offset),
                                             p.Color);
                offset++;
            }

            _scoreSpriteBatch.DrawString(_seguiUI,
                                         string.Format("Sanctums:{0,4}", _controller.Board.Tiles.Where(t => t.Building == "Sanctum").Count()),
                                         new Vector2(_graphics.PreferredBackBufferWidth*13/16+5, 15*offset),
                                         Color.Gold);

            _scoreSpriteBatch.End();

            //Display tooltips for tiles
            if (!_displayHelp)
                return;
            _tooltipSpriteBatch.Begin();

            _tooltipOffset -= (int)(gameTime.ElapsedGameTime.Milliseconds/10f);
            _tooltipSpriteBatch.Draw(_gray,
                                     new Rectangle(0, _graphics.PreferredBackBufferHeight*4/5-50, _graphics.PreferredBackBufferWidth, 40),
                                     new Color(128F, 128F, 128F, 128F));

            int tooltipYPosition = _graphics.PreferredBackBufferHeight*4/5-40;

            if (!_moveCamera)
            {
                Tile selectedTile = _controller.Board[_selectionPoint].Tile;
                if (selectedTile != null)
                    // don't display tool tips for unflipped tiles, that's cheating
                    if (!selectedTile.IsFlipped)
                    {
                        int nextMarqueeTextPosition = 0;

                        _tooltipOffset %= (int)_seguiUI.MeasureString("Tile is flipped over"+" - ").X;
                        while (nextMarqueeTextPosition+_tooltipOffset<_graphics.PreferredBackBufferWidth)
                        {
                            _tooltipSpriteBatch.DrawString(_seguiUI,
                                                           "Tile is flipped over"+" - ",
                                                           new Vector2(nextMarqueeTextPosition+_tooltipOffset, tooltipYPosition),
                                                           Color.White);
                            nextMarqueeTextPosition += (int)_seguiUI.MeasureString("Tile is flipped over"+" - ").X;
                        }
                    }
                    else
                    {
                        int nextMarqueeTextPosition = 0;

                        _tooltipOffset %= (int)_seguiUI.MeasureString(selectedTile.Building+" : "+selectedTile.Rules+" - ").X;
                        while (nextMarqueeTextPosition+_tooltipOffset<_graphics.PreferredBackBufferWidth)
                        {
                            _tooltipSpriteBatch.DrawString(_seguiUI,
                                                           selectedTile.Building+" : "+selectedTile.Rules+" - ",
                                                           new Vector2(nextMarqueeTextPosition+_tooltipOffset, tooltipYPosition),
                                                           Color.White);
                            nextMarqueeTextPosition += (int)_seguiUI.MeasureString(selectedTile.Building+" : "+selectedTile.Rules+" - ").X;
                        }
                    }
            }
            else
                // if there isn't a custom selection, display tool tips for selected hand tile
                if (_controller.Selection != Selection.TileFromCustom)
                {
                    if (_handViewLayer.SelectedTileIndex>=0 && _handViewLayer.SelectedTileIndex<_currentHand.Count())
                    {
                        Tile selectedTile = _currentHand[_handViewLayer.SelectedTileIndex];

                        if (selectedTile != null)
                        {
                            int nextMarqueeTextPosition = 0;
                            _tooltipOffset %= (int)_seguiUI.MeasureString(selectedTile.Building+":"+selectedTile.Rules+" - ").X;
                            while (nextMarqueeTextPosition+_tooltipOffset<_graphics.PreferredBackBufferWidth)
                            {
                                _tooltipSpriteBatch.DrawString(_seguiUI,
                                                               selectedTile.Building+":"+selectedTile.Rules+" - ",
                                                               new Vector2(nextMarqueeTextPosition+_tooltipOffset, tooltipYPosition),
                                                               Color.White);
                                nextMarqueeTextPosition += (int)_seguiUI.MeasureString(selectedTile.Building+":"+selectedTile.Rules+" - ").X;
                            }
                        }
                    }
                }
                else // Display tool tip for selected custom tile
                    if (_customViewLayer.SelectedTileIndex>=0 && _customViewLayer.SelectedTileIndex<_controller.TileSet.Count())
                    {
                        Tile selectedTile = _controller.TileSet[_customViewLayer.SelectedTileIndex];

                        if (selectedTile != null)
                        {
                            int nextMarqueeTextPosition = 0;
                            _tooltipOffset %= (int)_seguiUI.MeasureString(selectedTile.Building+":"+selectedTile.Rules+" - ").X;
                            while (nextMarqueeTextPosition+_tooltipOffset<_graphics.PreferredBackBufferWidth)
                            {
                                _tooltipSpriteBatch.DrawString(_seguiUI,
                                                               selectedTile.Building+":"+selectedTile.Rules+" - ",
                                                               new Vector2(nextMarqueeTextPosition+_tooltipOffset, tooltipYPosition),
                                                               Color.White);
                                nextMarqueeTextPosition += (int)_seguiUI.MeasureString(selectedTile.Building+":"+selectedTile.Rules+" - ").X;
                            }
                        }
                    }

            _tooltipSpriteBatch.End();
        }

        private void DrawHumanView()
        {
            if (_moveCamera)
            {
                var colorOverlay = new SpriteBatch(GraphicsDevice);

                colorOverlay.Begin(SpriteSortMode.Deferred,
                                   BlendState.AlphaBlend,
                                   SamplerState.LinearClamp,
                                   DepthStencilState.None,
                                   RasterizerState.CullCounterClockwise,
                                   null,
                                   Matrix.Identity);

                colorOverlay.Draw(_gray,
                                  new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                                  new Color(128F, 128F, 128F, 128F));

                colorOverlay.End();
            }

            // draw the tiles in the hand
            _handSpriteBatch.Begin(SpriteSortMode.Deferred,
                                   BlendState.AlphaBlend,
                                   SamplerState.LinearClamp,
                                   DepthStencilState.None,
                                   RasterizerState.CullCounterClockwise,
                                   null,
                                   Matrix.Identity);
            DrawBorder();

            _handViewLayer.Draw(_handSpriteBatch,
                                _controller.ActivePlayer.Hand,
                                _handViewLayer.SelectedTileIndex,
                                _graphics.PreferredBackBufferHeight*4/5+10);

            // if we are displaying custom tiles, display them
            if (_controller.Selection == Selection.TileFromCustom)
                _customViewLayer.Draw(_handSpriteBatch,
                                      _controller.TileSet,
                                      _customViewLayer.SelectedTileIndex,
                                      _graphics.PreferredBackBufferHeight*1/2);

            // if we are looking for a player, pass in the blank tiles for color display
            if (_controller.Selection == Selection.Opponent || _controller.Selection == Selection.Player)
                _customViewLayer.Draw(_handSpriteBatch,
                                      _playerSelection,
                                      _customViewLayer.SelectedTileIndex,
                                      _graphics.PreferredBackBufferHeight*1/2);

            _handSpriteBatch.DrawString(_seguiUI,
                                        _selectionManager.Message,
                                        new Vector2(
                                            _graphics.PreferredBackBufferWidth/2f-_seguiUI.MeasureString(_selectionManager.Message).X/2, 0),
                                        Color.Black);

            _handSpriteBatch.Draw(_white,
                                  new Rectangle(_graphics.PreferredBackBufferWidth*7/8,
                                                0,
                                                _graphics.PreferredBackBufferWidth/8,
                                                18*(_controller.Players.ToArray().Length+1)),
                                  Color.Black);

            _handSpriteBatch.End();

            if (!_moveCamera)
            {
                var colorOverlay = new SpriteBatch(GraphicsDevice);

                colorOverlay.Begin(SpriteSortMode.Deferred,
                                   BlendState.AlphaBlend,
                                   SamplerState.LinearClamp,
                                   DepthStencilState.None,
                                   RasterizerState.CullCounterClockwise,
                                   null,
                                   Matrix.Identity);

                colorOverlay.Draw(_gray,
                                  new Rectangle(20,
                                                _graphics.PreferredBackBufferHeight*4/5,
                                                _graphics.PreferredBackBufferWidth-40,
                                                _graphics.PreferredBackBufferHeight-25-_graphics.PreferredBackBufferHeight*4/5),
                                  new Color(128F, 128F, 128F, 128F));
                colorOverlay.End();
            }
        }

        /// <summary>
        ///   Initialize a new game if new controllers/models/views are necessary. This is usually done 
        ///   when Options changes some settings and various objects need to be reinitialized.
        /// </summary>
        /// <param name = "players">Players playing the game</param>
        private void InitializeNewGame(Player[] players)
        {
            IsMouseVisible = true;
            _camera = new Camera(_graphics);

            // Why is the board starting at (1600,1100)? No one knows....
            _camera.PointTo(new Vector2(1600, 1100));

            _controller = Controller.CustomInstance(players);

            _selectionManager = _controller;

            _oldKeyboardState = Keyboard.GetState();

            _boardViewLayer = new BoardViewLayer(_graphics, _controller);

            _handViewLayer = new HandViewLayer(_graphics, _controller) {IsCustom = false};

            _customViewLayer = new HandViewLayer(_graphics, _controller) {IsCustom = true};

            // a blank tile for each player, colors get added later
            _playerSelection = new Tile[_controller.Players.Count()];
            for (int i = 0; i<_controller.Players.Count(); i++)
                _playerSelection[i] = new Blank {Owner = _controller.Players.ElementAt(i), Space = new Space()};
            _selectionPoint = new Point(0, 0);

            _moveCamera = true;

            _oldColor = Color.Wheat;

            _displayHelp = false;

            _endGame = false;

            base.Initialize();
        }

        private void UpdateCamera(KeyboardState newKeyboardState)
        {
            // check if we need to move the camera
            if (newKeyboardState.IsKeyDown(Keys.Up))
                _camera.Position = _camera.Position-new Vector2(0, 3/_camera.Zoom);

            if (newKeyboardState.IsKeyDown(Keys.Left))
                _camera.Position = _camera.Position-new Vector2(3/_camera.Zoom, 0);

            if (newKeyboardState.IsKeyDown(Keys.Right))
                _camera.Position = _camera.Position-new Vector2(-3/_camera.Zoom, 0);

            if (newKeyboardState.IsKeyDown(Keys.Down))
                _camera.Position = _camera.Position-new Vector2(0, -3/_camera.Zoom);

            // Perform zoom on held buttons
            if (newKeyboardState.IsKeyDown(Keys.Q))
                _camera.ZoomOut();
            if (newKeyboardState.IsKeyDown(Keys.E))
                _camera.ZoomIn();
        }

        private void UpdateGameScreen(KeyboardState newKeyboardState)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            _currentHand = _controller.ActivePlayer.Hand.ToArray();

            UpdateCamera(newKeyboardState);

            CheckEsc(newKeyboardState);

            CheckMute(newKeyboardState);
            if (!_endGame)
            {
                // if the player is siting at the keyboard
                if (_controller.ActivePlayer.Input == null)
                    // try to process user input
                    try
                    {
                        UpdateInput(newKeyboardState);
                    }
                    catch (EndGameException e)
                    {
                        _endGame = true;
                        _controller.Message = e.Message;
                    }
                    catch (Exception e)
                    {
                        // if the user did something bad, catch the exception and display the message
                        _controller.Message = e.Message;
                    }

                else if (!_controller.ActivePlayer.Input.IsThinking)
                    if (_controller.ActivePlayer.Input.IsComplete)
                    {
                        object input = _controller.ActivePlayer.Input.Acknowledge();
                        try
                        {
                            _selectionManager.Submit(input);
                        }
                        catch (EndGameException e)
                        {
                            _endGame = true;
                            _controller.Message = e.Message;
                        }
                    }
                    else
                        _controller.ActivePlayer.Input.GetInputAsync();
            }
            else if (newKeyboardState.IsKeyDown(Keys.Space) && !_oldKeyboardState.IsKeyDown(Keys.Space))
                _controller.TogglePlayers();
        }

        /// <summary>
        ///   Here we check the keyboard for user input.
        ///   If we have any, we pass the right input
        ///   to the selectionManager.
        /// </summary>
        private void UpdateInput(KeyboardState newKeyboardState)
        {
            // Get the old key state to make sure buttom is pressed and not held
            Keys[] oldStateKeys = _oldKeyboardState.GetPressedKeys();

            if (newKeyboardState.IsKeyDown(Keys.I) && !oldStateKeys.Contains(Keys.I))
            {
                Tile temp = _controller.Board[_selectionPoint].Tile;
                if (temp != null)
                    _controller.Message = temp.Rules;
            }

            if (newKeyboardState.IsKeyDown(Keys.H) && !oldStateKeys.Contains(Keys.H))
                _displayHelp = !_displayHelp;

            if (newKeyboardState.IsKeyDown(Keys.D) && !oldStateKeys.Contains(Keys.D)) {}

            // testing the tile skip() functionality
            if (newKeyboardState.IsKeyDown(Keys.C) && !oldStateKeys.Contains(Keys.C))
                _selectionManager.Skip();

            // If we are not moving the camera, we are moving the selector
            if (!_moveCamera)
            {
                if (newKeyboardState.IsKeyDown(Keys.W) && !oldStateKeys.Contains(Keys.W))
                    _selectionPoint.Y--;

                if (newKeyboardState.IsKeyDown(Keys.A) && !oldStateKeys.Contains(Keys.A))
                    _selectionPoint.X++;

                if (newKeyboardState.IsKeyDown(Keys.D) && !oldStateKeys.Contains(Keys.D))
                    _selectionPoint.X--;

                if (newKeyboardState.IsKeyDown(Keys.S) && !oldStateKeys.Contains(Keys.S))
                    _selectionPoint.Y++;
            }
            else
            {
                if (newKeyboardState.IsKeyDown(Keys.A) && !oldStateKeys.Contains(Keys.A))
                    if (_controller.Selection != Selection.TileFromCustom && _controller.Selection != Selection.Opponent &&
                        _controller.Selection != Selection.Player)

                        _handViewLayer.SelectedTileIndex--;
                    else
                        _customViewLayer.SelectedTileIndex--;

                if (newKeyboardState.IsKeyDown(Keys.D) && !oldStateKeys.Contains(Keys.D))
                    if (_controller.Selection != Selection.TileFromCustom && _controller.Selection != Selection.Opponent &&
                        _controller.Selection != Selection.Player)

                        _handViewLayer.SelectedTileIndex++;
                    else
                        _customViewLayer.SelectedTileIndex++;
            }

            if (newKeyboardState.IsKeyDown(Keys.Enter) && !oldStateKeys.Contains(Keys.Enter))
                switch (_controller.Selection)
                {
                    case Selection.TileFromHand:
                        _selectionManager.Submit(_currentHand[_handViewLayer.SelectedTileIndex]);
                        break;
                    case Selection.PlayableSpace:
                        _selectionManager.Submit(_controller.Board[_selectionPoint]);
                        break;
                    case Selection.TileFromCustom:
                        _selectionManager.Submit(_controller.TileSet[_customViewLayer.SelectedTileIndex]);
                        break;
                    case Selection.TileFromBoard:
                        _selectionManager.Submit(_controller.Board[_selectionPoint].Tile);
                        break;
                    case Selection.Opponent:
                    case Selection.Player:
                        _selectionManager.Submit(_customViewLayer.ValidPlayers[_customViewLayer.SelectedTileIndex].Owner);
                        break;
                }
            if (newKeyboardState.IsKeyDown(Keys.Space) && !oldStateKeys.Contains(Keys.Space))
                _moveCamera = !_moveCamera;
        }

        private void UpdateMenuScreen(KeyboardState newKeyboardState)
        {
            MenuItem selectedMenuItem = _menu.MenuItems[_menu.SelectedMenuItemIndex];
            if ((newKeyboardState.IsKeyDown(Keys.Enter) && _oldKeyboardState.IsKeyUp(Keys.Enter)) ||
                (newKeyboardState.IsKeyDown(Keys.Space) && _oldKeyboardState.IsKeyUp(Keys.Space)))
                switch (_menu.CurrentSubMenu)
                {
                    case SubMenu.Main:
                        if (_menu.SelectedMenuItemIndex == 0)
                            _currentScreen = Screen.Game;
                        switch (_menu.SelectedMenuItemIndex)
                        {
                            case 1:
                                _menu.TransitionToOptions(_aiParams);
                                break;
                            case 2:
                                Exit();
                                break;
                        }
                        break;
                    case SubMenu.Options:
                        if (_menu.SelectedMenuItemIndex == _menu.MenuItems.Count-1)
                        {
                            if (_aiParams == null)
                                _aiParams = new List<Tuple<string, int>>();
                            _aiParams.Clear();

                            for (int currentPlayer = 0; currentPlayer<6; currentPlayer++)
                                _aiParams.Add(new Tuple<string, int>(MenuItem.AIs[_menu.MenuItems[currentPlayer*2].SelectedAIIndex].Item1,
                                                                     _menu.MenuItems[(currentPlayer*2)+1].SliderValue));
                            _menu.TransitionToMainMenu();

                            InitializeNewGame(InitPlayerInputs(_aiParams));
                        }
                        break;
                }
            else if ((newKeyboardState.IsKeyDown(Keys.Left) && _oldKeyboardState.IsKeyUp(Keys.Left)))
                switch (selectedMenuItem.MenuItemType)
                {
                    case MenuItemType.Bool:
                        selectedMenuItem.BoolValue = !selectedMenuItem.BoolValue;
                        break;
                    case MenuItemType.Slider:
                        selectedMenuItem.SliderDecrement();
                        break;
                    case MenuItemType.AISelector:
                        selectedMenuItem.SelectedAIIndex--;
                        selectedMenuItem.ToggleLevelSelector();
                        break;
                    default:
                        break;
                }
            else if ((newKeyboardState.IsKeyDown(Keys.Right) && _oldKeyboardState.IsKeyUp(Keys.Right)))
                switch (selectedMenuItem.MenuItemType)
                {
                    case MenuItemType.Bool:
                        selectedMenuItem.BoolValue = !selectedMenuItem.BoolValue;
                        break;
                    case MenuItemType.Slider:
                        selectedMenuItem.SliderIncrement();
                        break;
                    case MenuItemType.AISelector:
                        selectedMenuItem.SelectedAIIndex++;
                        selectedMenuItem.ToggleLevelSelector();
                        break;
                    default:
                        break;
                }

            if (newKeyboardState.IsKeyDown(Keys.Up) && _oldKeyboardState.IsKeyUp(Keys.Up))
                do
                {
                    _menu.SelectedMenuItemIndex--;
                } while (!_menu.MenuItems[_menu.SelectedMenuItemIndex].Enabled);
            if (newKeyboardState.IsKeyDown(Keys.Down) && _oldKeyboardState.IsKeyUp(Keys.Down))
                do
                {
                    _menu.SelectedMenuItemIndex++;
                } while (!_menu.MenuItems[_menu.SelectedMenuItemIndex].Enabled);
        }
    }
}