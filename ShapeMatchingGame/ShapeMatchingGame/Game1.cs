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
using ShapeMatchingGame.Grid;
using ShapeMatchingGame.MoveFinder;
using ShapeMatchingGame.Shape;
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
            graphics.PreferredBackBufferHeight = 850;
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
            Globals.Content = Content;
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _spriteFont = Content.Load<SpriteFont>("SpriteFont1");
            _gridViewDrawable = new GridViewDrawable(new Point(20, 20), 8, 8, 50, 50);

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
        private TimeSpan _lastMoveTotalGameTime = TimeSpan.Zero;
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
                if(_gridViewDrawable.Rectangle.Contains(cursorPosition))
                {
                    _gridViewDrawable.Click(cursorPosition);
                }
            }
            if(currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Pressed)
            {
                Point cursorPosition = new Point(currentMouseState.X, currentMouseState.Y);
                if(_gridViewDrawable.Rectangle.Contains(cursorPosition))
                {
                    _gridViewDrawable.DragTo(cursorPosition);
                }
            }
            if(currentMouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released)
            {
                Point cursorPosition = new Point(currentMouseState.X, currentMouseState.Y);
                if (_gridViewDrawable.Rectangle.Contains(cursorPosition))
                {
                    _gridViewDrawable.Update();
                }
            }

            if (currentKeyboardState.IsKeyDown(Keys.B) && _previousKeyboardState.IsKeyUp(Keys.B))
            {
                playAlone = !playAlone;
            }
            if (playAlone)
            {
                MoveFinder.IMoveFinder moveFinder = new RecursiveMoveFinder();
                Move bestMove = moveFinder.GetBestMove(_gridViewDrawable.ToGridModel(), 2);
                if (bestMove == null)
                {
                    Debug.WriteLine("Game over on turn {0}. \n Score:{1}", _gridViewDrawable.Turn,
                                    _gridViewDrawable.Score);
                    _gridViewDrawable = new GridViewDrawable(new Point(20, 20), 8, 8, 50, 50);
                }
                else
                {
                    int predictedScore = _gridViewDrawable.Score + bestMove.PredictedScore;
                    _gridViewDrawable.DoMove(bestMove);
                    if (predictedScore > _gridViewDrawable.Score)
                    {
                        //Debug.WriteLine("predicted score was too high");
                    }
                }
                _lastMoveTotalGameTime = gameTime.TotalGameTime;
            }


            _previousKeyboardState = currentKeyboardState;
            _previousMouseState = currentMouseState;

            _gridViewDrawable.Update();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        private GridViewDrawable _gridViewDrawable;
        private SpriteFont _spriteFont;
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            _gridViewDrawable.Draw(spriteBatch);
            string scoreString = "Score: " + _gridViewDrawable.Score;
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
