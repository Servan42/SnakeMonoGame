using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

// https://docs.monogame.net/articles/getting_started/5_adding_basic_code.html
namespace PocMonoGame
{
    public class Game1 : Game
    {
        SnakeEngine snakeBoard;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D levTexture;
        private Texture2D zerglingTexture;
        private Texture2D rectangleTexture;

        private SpriteFont scoreFont;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1000;
            _graphics.PreferredBackBufferHeight = 1000;
            this.snakeBoard = new SnakeEngine(_graphics.PreferredBackBufferWidth / 10, _graphics.PreferredBackBufferHeight / 10);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1d / 144d);
        }

        protected override void Initialize()
        {
            // Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // use this.Content to load your game content here
            levTexture = Content.Load<Texture2D>("lev");
            zerglingTexture = Content.Load<Texture2D>("zergling");
            scoreFont = Content.Load<SpriteFont>("score");

            rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            rectangleTexture.SetData([Color.LightGray]);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Add your update logic here
            //var kstate = Keyboard.GetState();
            snakeBoard.Tick();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Add your drawing code here
            _spriteBatch.Begin();

            foreach (var tail in snakeBoard.GetTail())
            {
                _spriteBatch.Draw(rectangleTexture, new Rectangle(tail.x * 10, tail.y * 10, 10, 10), new Color(255, 243, 169));
            }

            _spriteBatch.Draw(
                zerglingTexture,
                snakeBoard.GetFoodPos().ToVector().Scale(10),
                null,
                Color.White,
                0f,
                new Vector2(zerglingTexture.Width / 2, zerglingTexture.Height / 2),
                new Vector2(0.25f, 0.25f),
                SpriteEffects.None,
                0f
            );

            _spriteBatch.Draw(
                levTexture,
                snakeBoard.GetSnakeHeadPos().ToVector().Scale(10),
                null,
                Color.White,
                0f,
                new Vector2(levTexture.Width / 2, levTexture.Height / 2),
                new Vector2(0.5f, 0.5f),
                SpriteEffects.None,
                0f
            );

            _spriteBatch.DrawString(scoreFont, $"Fps: {Convert.ToInt32(1 / gameTime.ElapsedGameTime.TotalSeconds)}", new Vector2(0, 0), Color.LightGray);
            _spriteBatch.DrawString(scoreFont, snakeBoard.GetScoreString(), new Vector2(0, 20), Color.LightGray);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
