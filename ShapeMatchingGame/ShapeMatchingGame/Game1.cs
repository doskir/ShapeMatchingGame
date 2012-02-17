using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ShapeMatchingGame.MoveFinder;
using ShapeMatchingGame.Utility;

namespace ShapeMatchingGame
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            this.IsMouseVisible = true;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Globals.Content = Content;
            _spriteFont = Content.Load<SpriteFont>("SpriteFont1");
            _grid = new Grid(new Point(20, 20), 8, 8, 50, 50);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private MouseState _previousMouseState;
        private KeyboardState _previousKeyboardState;
        private bool playAlone = false;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            MouseState currentMouseState = Mouse.GetState();
            KeyboardState currentKeyboardState = Keyboard.GetState();
            if(currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                Point cursorPosition = new Point(currentMouseState.X, currentMouseState.Y);
                if(_grid.Rectangle.Contains(cursorPosition))
                {
                    _grid.Clicked(cursorPosition);
                }
            }
            if(currentMouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released)
            {
                Point cursorPosition = new Point(currentMouseState.X, currentMouseState.Y);
                if (_grid.Rectangle.Contains(cursorPosition))
                {
                    _grid.DebugFunctionAt(cursorPosition);
                }
            }

            if (currentKeyboardState.IsKeyDown(Keys.B) && _previousKeyboardState.IsKeyUp(Keys.B))
            {
                playAlone = !playAlone;
            }
            if(playAlone && _grid.MovesAllowed)
            {
                MoveFinder.IMoveFinder moveFinder = new RecursiveMoveFinder();
                Move bestMove = moveFinder.GetBestMove(_grid.ShapeSlotsToArray(), 2);
                if (bestMove == null)
                {
                    Debug.WriteLine("Game over on turn {0}. \n Score:{1}", _grid.Turn, _grid.Score);
                    _grid = new Grid(new Point(20, 20), 8, 8, 50, 50);
                }
                else
                {
                    int predictedScore = _grid.Score + bestMove.PredictedScore;
                    _grid.DoMove(bestMove.From, bestMove.To);
                    if (predictedScore < _grid.Score)
                    {
                        Debug.WriteLine("predicted score was too high");
                    }
                }
            }
            

            _previousKeyboardState = currentKeyboardState;
            _previousMouseState = currentMouseState;

            _grid.Update();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private Grid _grid;
        private SpriteFont _spriteFont;
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            _grid.Draw(spriteBatch);
            string scoreString = "Score: " + _grid.Score;
            float width = _spriteFont.MeasureString(scoreString).X;
            Vector2 textPosition = new Vector2(graphics.GraphicsDevice.Viewport.Width, 20);
            textPosition.X -= width;
            textPosition.X -= 20;
            spriteBatch.DrawString(_spriteFont, scoreString, textPosition, Color.Black);

            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
