using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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
            _grid = new Grid(new Point(20,20),8, 8, 50, 50);
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

        private MouseState _previousState;
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
            MouseState currentState = Mouse.GetState();
            if(currentState.LeftButton == ButtonState.Pressed && _previousState.LeftButton == ButtonState.Released)
            {
                Point cursorPosition = new Point(currentState.X, currentState.Y);
                if(_grid.Rectangle.Contains(cursorPosition))
                {
                    _grid.Clicked(cursorPosition);
                }
            }
            if(currentState.RightButton == ButtonState.Pressed && _previousState.RightButton == ButtonState.Released)
            {
                Point cursorPosition = new Point(currentState.X, currentState.Y);
                if (_grid.Rectangle.Contains(cursorPosition))
                {
                    _grid.DebugFunctionAt(cursorPosition);
                }
            }
            _previousState = currentState;

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
