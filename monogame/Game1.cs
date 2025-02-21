using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogame.Classes;
using System;
using System.Collections.Generic;

namespace monogame
{
    public class Game1 : Game
    {
        //const
        private const int COUNT_ASTEROIDS = 10;
        //tools
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        //поля
        private Player _player;
        private Space _space;
        private Asteroid _asteroid;
        private List<Asteroid> _asteroids;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _player = new Player();
            _space = new Space(); 
            _asteroids = new List<Asteroid>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _player.LoadContent(Content);
            _space.LoadContent(Content);
            //_bullet.LoadContent(Content);
            for (int i = 0; i < COUNT_ASTEROIDS; i++)
            {
                Asteroid asteroid = new Asteroid();
                asteroid.LoadContent(Content);
                Random random = new Random();
                int x = random.Next(0, _graphics.PreferredBackBufferWidth
                                       - asteroid.Width);
                int y = random.Next(0, _graphics.PreferredBackBufferHeight);
                asteroid.Position = new Vector2(x, -y);
                _asteroids.Add(asteroid);
            }
            //_asteroid.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == 
                ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            _player.Update(
                _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight, Content);
            _space.Update();
            AsteroidsUpdate();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            {
                _space.Draw(_spriteBatch);
                _player.Draw(_spriteBatch);
                foreach (Asteroid asteroid in _asteroids)
                {
                    asteroid.Draw(_spriteBatch);
                }
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        private void AsteroidsUpdate()
        {
            for (int i = 0; i < _asteroids.Count; i++)
            {
                Asteroid asteroid = _asteroids[i];
                asteroid.Update();
                if (asteroid.Position.Y > _graphics.PreferredBackBufferHeight)
                {
                    Random random = new Random();
                    int x = random.Next(0, _graphics.PreferredBackBufferWidth
                                       - asteroid.Width);
                    int y = random.Next(0, _graphics.PreferredBackBufferHeight);
                    asteroid.Position = new Vector2(x, -y);
                }
                if (asteroid.Collision.Intersects(_player.Collision))
                {
                    _asteroids.Remove(asteroid);
                    i--;
                }
            }
            if (_asteroids.Count < COUNT_ASTEROIDS)
            {
                LoadAst();
            }
        }
        private void LoadAst()
        {
            Asteroid asteroid = new Asteroid();
            asteroid.LoadContent(Content);
            Random random = new Random();
            int x = random.Next(0, _graphics.PreferredBackBufferWidth
                                   - asteroid.Width);
            int y = random.Next(0, _graphics.PreferredBackBufferHeight);
            asteroid.Position = new Vector2(x, -y);
            _asteroids.Add(asteroid);
        }
    }
}
        