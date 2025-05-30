using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogame.Classes;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;
using monogame.Classes.SaveData;
using System.Text.Json;
using System.IO;
using System.Text.Json.Nodes;

namespace monogame
{
    public class Game1 : Game
    {
        //const
        private const int COUNT_ASTEROIDS = 10;
        private const int COUNT_ENEMIES = 10;
        //tools
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        //поля
        private Player _player;
        private Space _space;
        public static GameMode gameMode = GameMode.Menu;
        private MainMenu _mainMenu;
        private PauseMenu _pauseMenu;
        private HUD _hud;
        private HealBoost _healBoost;
        private Asteroid _asteroid;
        private GameOver _gameOver;
        private List<Asteroid> _asteroids;
        private List <Explosion> _explosions;
        private List <Enemy> _enemies;
        private Song _gameSong;
        private Song _menuSong;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _player = new Player(_graphics.PreferredBackBufferHeight);
            _space = new Space(); 
            _asteroids = new List<Asteroid>();
            _explosions = new List<Explosion>();
            _enemies = new List<Enemy>();
            _mainMenu = new MainMenu(_graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight);
            _pauseMenu = new PauseMenu(_graphics.PreferredBackBufferWidth,
               _graphics.PreferredBackBufferHeight);
            _gameOver = new GameOver(_graphics.PreferredBackBufferWidth, 
                _graphics.PreferredBackBufferHeight);
            _hud = new HUD();
            _healBoost = new HealBoost(_graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight);
            _player.TakeDamage += _hud.OnPlayerTakeDamage;
            _player.UpdateScore += _hud.OnScoreUpdated;
            _mainMenu.OnPlayingStarted += OnPlayingStarted;
            _mainMenu.OnLoadGame += LoadGame;
            _pauseMenu.OnPlayingResume += OnPlayingResumed;
            _pauseMenu.OnSaveGame += SaveGame;

            base.Initialize();
        }
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            _player.LoadContent(Content);
            _space.LoadContent(Content);
            _hud.LoadContent(GraphicsDevice, Content);
            _healBoost.LoadContent(Content);
            _gameOver.LoadContent(Content);
            _mainMenu.LoadContent(Content);
            _pauseMenu.LoadContent(Content);
            _gameSong = Content.Load<Song>("gameMusic");
            _menuSong = Content.Load<Song>("menuMusic");
            MediaPlayer.Volume = 0.09f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_menuSong);
            //_bullet.LoadContent(Content);
            for (int i = 0; i < COUNT_ASTEROIDS; i++)
            {
                LoadAst();
            }
            for (int i = 0; i < COUNT_ENEMIES; i++)
            {
                LoadEnemies();
            }
            //_asteroid.LoadContent(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            switch (gameMode)
            {
                case GameMode.Menu:
                    _space.Speed = 1;
                    _space.Update();
                    _mainMenu.Update();
                    break;
                case GameMode.Pause:
                    _space.Speed = 1;
                    _space.Update();
                    _pauseMenu.Update();
                    break;
                case GameMode.Playing:
                    _space.Speed = 3;
                    _player.Update(
                    _graphics.PreferredBackBufferWidth,
                    _graphics.PreferredBackBufferHeight, Content);
                    _space.Update();
                    _healBoost.Update();
                    UpdateAsteroids();
                    UpdateEnemies();
                    CheckCollision();
                    UpdExplosions(gameTime);
                    if (_player.Health <= 0)
                    {
                        gameMode = GameMode.GameOver;
                        _gameOver.SetScore(_player.Score);
                        MediaPlayer.Play(_menuSong);
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        gameMode = GameMode.Pause;
                        MediaPlayer.Play(_menuSong);
                    }
                    break;
                case GameMode.GameOver:
                    _space.Speed = 1;
                    _space.Update();
                    _gameOver.Update();
                    break;
                case GameMode.Exit:
                    Exit();
                    break;
                default:
                    break;
            }
           
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            {
                switch (gameMode)
                {
                    case GameMode.Menu:
                        _space.Draw(_spriteBatch);
                        _mainMenu.Draw(_spriteBatch);
                        break;
                    case GameMode.Pause:
                        _space.Draw(_spriteBatch);
                        _pauseMenu.Draw(_spriteBatch);
                        break;
                    case GameMode.Playing:
                        _space.Draw(_spriteBatch);
                        _player.Draw(_spriteBatch);
                        _healBoost.Draw(_spriteBatch);
                        foreach (Asteroid asteroid in _asteroids)
                        {
                            asteroid.Draw(_spriteBatch);
                        }
                        foreach (Explosion explosion in _explosions)
                        {
                            explosion.Draw(_spriteBatch);
                        }
                        foreach (Enemy enemy in _enemies)
                        {
                            enemy.Draw(_spriteBatch);
                        }
                        _hud.Draw(_spriteBatch);
                        break;
                    case GameMode.GameOver:
                        _space.Draw(_spriteBatch);
                        _gameOver.Draw(_spriteBatch);
                        break;
                    case GameMode.Exit:
                        break;
                    default:
                        break;
                }
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        private void UpdateAsteroids()
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
                if (!asteroid.IsAlive)
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
        private void UpdateEnemies()
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                Enemy enemy = _enemies[i];
                enemy.Update(Content);
                if (!enemy.IsAlive)
                {
                    _enemies.Remove(enemy);
                    i--;
                }
            }
            if (_enemies.Count < COUNT_ENEMIES)
            {
                LoadEnemies();
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
        private void LoadEnemies()
        {
            Enemy enemy = new Enemy(_graphics.PreferredBackBufferHeight);
            enemy.LoadContent(Content);
            Random random = new Random();
            int x = random.Next(0, _graphics.PreferredBackBufferWidth
                                   - enemy.Width);
            int y = random.Next(0, _graphics.PreferredBackBufferHeight);
            enemy.Position = new Vector2(x, -y);
            _enemies.Add(enemy);
        }
        private void CheckCollision()
        {
            foreach (Asteroid asteroid in _asteroids)
            {
                if (asteroid.Collision.Intersects(_player.Collision))
                {
                    asteroid.IsAlive = false;
                    _player.Damage();
                    CreateExplosion(asteroid.Position, asteroid.Width, asteroid.Height);
                }
                foreach (Bullet bullet in _player.Bullets)
                {
                    if (asteroid.Collision.Intersects(bullet.Collision))
                    {
                        bullet.IsAlive = false;
                        asteroid.IsAlive = false;
                        CreateExplosion(asteroid.Position, asteroid.Width, asteroid.Height);
                        _player.AddScore();
                    }
                }
            }
            foreach (Enemy enemy in _enemies)
            {
                if (enemy.Collision.Intersects(_player.Collision))
                {
                    enemy.IsAlive = false;
                    _player.Damage();
                    CreateExplosion(enemy.Position, enemy.Width, enemy.Height);
                }
                foreach (Bullet enemyBullet in enemy.Bullets)
                {
                    if (enemyBullet.Collision.Intersects(_player.Collision))
                    {
                        enemyBullet.IsAlive = false;
                        _player.Damage();
                        CreateExplosion(enemyBullet.Position,
                            enemyBullet.Width, enemyBullet.Height);
                    }
                }
                foreach (Bullet bullet in _player.Bullets)
                {
                    if (enemy.Collision.Intersects(bullet.Collision))
                    {
                        enemy.IsAlive = false;
                        bullet.IsAlive = false;
                        CreateExplosion(enemy.Position, enemy.Width, enemy.Height);
                    }
                    foreach (Bullet enemyBullet in enemy.Bullets)
                    {
                        if (enemyBullet.Collision.Intersects(bullet.Collision))
                        {
                            enemyBullet.IsAlive = false;
                            bullet.IsAlive = false;
                        }
                    }
                }
            }
            if (_healBoost.Collision.Intersects(_player.Collision))
            {
                _healBoost.Reset();
                _player.Heal();
                _hud.OnPlayerHealed();
            }
        }
        private void CreateExplosion(Vector2 spawnPosition, int width, int height)
        {
            Explosion explosion = new Explosion(spawnPosition);
            Vector2 position = spawnPosition;
            position = new Vector2(
                position.X - explosion.Width / 2,
                position.Y - explosion.Height / 2);
            position = new Vector2(position.X + width / 2,
                position.Y + height / 2);
            explosion.Position = position;
            explosion.LoadContent(Content);
            _explosions.Add(explosion);
            explosion.PlaySoundEffect();
        }
        private void UpdExplosions(GameTime gameTime)
        {
            for (int i = 0; i < _explosions.Count; i++)
            {
                _explosions[i].Update(gameTime);
                if(!_explosions[i].IsAlive)
                {
                    _explosions.RemoveAt(i);
                    i--;
                }
            }
        }
        private void OnPlayingStarted()
        {
            gameMode = GameMode.Playing;
            MediaPlayer.Play(_gameSong);
            Reset();
        }
        private void OnPlayingResumed()
        {
            gameMode = GameMode.Playing;
            MediaPlayer.Play(_gameSong);
        }
        private void Reset()
        {
            _player.Reset();
            _hud.Reset();
            _healBoost.Reset();
            _explosions.Clear();
            _asteroids.Clear();

        }
        private void SaveGame()
        {
            PlayerData playerData = (PlayerData)_player.SaveData();
            object[] asteroidDatas = new object[_asteroids.Count];
            object[] explosionDatas = new object[_explosions.Count];
            for (int i = 0; i < _asteroids.Count; i++)
            {
                asteroidDatas[i] = _asteroids[i].SaveData();
            }
            for (int i = 0; i < _explosions.Count; i++)
            {
                explosionDatas[i] = _explosions[i].SaveData();
            }
            object[] datas =
            {
                _player.SaveData(),
                _hud.SaveData(),
                asteroidDatas,
                explosionDatas
            };
            string stringData = JsonSerializer.Serialize(datas);
            //StreamWriter writer = new StreamWriter("save.json");
            //writer.WriteLine(stringData);
            //writer.Close();
            File.WriteAllText("save.json", stringData);
            gameMode = GameMode.Menu;

        }
        private void LoadGame()
        {
            if (!File.Exists("save.json"))
            {
                return;
            }
            string jsonString = File.ReadAllText("save.json");
            object[] datas = JsonSerializer.Deserialize<object[]>(jsonString);
            if (datas == null)
            {
                return;
            }
            OnPlayingStarted();
            PlayerData playerData = ((JsonElement)datas[0]).Deserialize<PlayerData>();
            HUDData hudData = ((JsonElement)datas[1]).Deserialize<HUDData>();
            _player.LoadData(playerData, Content);
            _hud.LoadData(hudData, Content);
            object[] array = ((JsonElement)datas[2]).Deserialize<object[]>();
            object[] explArray = ((JsonElement)datas[3]).Deserialize<object[]>();
            for (int i = 0; i < array.Length; i++)
            {
                AsteroidData asteroidData = ((JsonElement)array[i]).Deserialize<AsteroidData>();
                Asteroid asteroid = new Asteroid();
                asteroid.LoadData(asteroidData, Content);
                _asteroids.Add(asteroid);
            }
            for (int i = 0; i < explArray.Length; i++)
            {
                ExplosionData explosionData = ((JsonElement)array[i]).Deserialize<ExplosionData>();
                Explosion explosion = new Explosion(Vector2.Zero);
                explosion.LoadData(explosionData, Content);
                _explosions.Add(explosion);
            }
        }
    }
}
        