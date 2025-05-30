using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogame.Classes.SaveData;

namespace monogame.Classes
{
    public class Player : ISaveable
    {
        private Vector2 _position;
        private Texture2D _texture;
        private float _speed = 7;
        private int _health = 10;
        private int _score = 0;
        public event Action<int> TakeDamage;
        public event Action<int> UpdateScore;
        private Rectangle _collision;
        private SoundEffect _soundEffect;
        private List<Bullet> _bullets = new List<Bullet>();
        private int _heightScreen;
        private int _timer = 0;
        private int _maxTime = 10;
        public Rectangle Collision
        {
            get { return _collision; }
        }
        public List <Bullet> Bullets
        {
            get { return _bullets; }
        }
        public int Health
        {
            get => _health;
        }
        public int Score
        {
            get => _score;
        }
        public Player(int heightScreen) 
        {
            _position = new Vector2(30,30);
            _texture = null;
            _collision = new Rectangle((int)_position.X, (int)_position.Y, 0, 0);
            _heightScreen = heightScreen;
        }
        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("player");
            _soundEffect = content.Load<SoundEffect>("laserfire");
        }
        public void Update(int widthScreen, int heightScreen, ContentManager content)
        {
            KeyboardState keyboard = Keyboard.GetState();
            #region Movement
            if (keyboard.IsKeyDown(Keys.S))
            {
                _position.Y += _speed;
            }
            if (keyboard.IsKeyDown(Keys.W))
            {
                _position.Y -= _speed;
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                _position.X -= _speed;
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                _position.X += _speed;
            }
            #endregion
            #region Bounds
            if (_position.X < 0)
            {
                _position.X = 0;
            }
            if (_position.Y < 0)
            {
                _position.Y = 0;
            }
            if (_position.X > widthScreen - _texture.Width)
            {
                _position.X = widthScreen - _texture.Width;
            }
            if (_position.Y > heightScreen - _texture.Height)
            {
                _position.Y = heightScreen - _texture.Height;
            }
            #endregion
            _collision = new Rectangle((int)_position.X + 20, (int)_position.Y + 20, 
                                       _texture.Width - 40, _texture.Height - 40);
            if (_timer<=_maxTime)
            {
                _timer++;
            }
            if (keyboard.IsKeyDown(Keys.Space) && _timer >= _maxTime)
            {
                Bullet bullet = new Bullet(new Vector2(0, -4),
                    "bullet", "laserFire", heightScreen);
                bullet.Position = new Vector2(_position.X + _texture.Width / 2 -
                    bullet.Width / 2, _position.Y + bullet.Height / 4);
                bullet.LoadContent(content);
                _bullets.Add(bullet);
                PlaySoundEffect(bullet);
                _timer = 0;
            }
            foreach (Bullet bullet in _bullets)
            {
                bullet.Update();
            }
            for (int i = 0; i < _bullets.Count; i++)
            {
                if (_bullets[i].IsAlive == false)
                {
                    _bullets.RemoveAt(i);
                    i--;
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, Color.White);
            foreach (Bullet bullet in _bullets)
            {
                bullet.Draw(spriteBatch);
            }
        }
        public void PlaySoundEffect(Bullet bullet)
        {
            if (_soundEffect == null)
            {
                return;
            }
            SoundEffectInstance instance = _soundEffect.CreateInstance();
            instance.Volume = 0.1f;
            instance.Play();
        }
        public void Damage()
        {
            _health--;
            if (TakeDamage != null)
                TakeDamage(_health);
        }
        public void Heal()
        {
            _health += 5;
            if (_health > 10)
            {
                _health = 10;
            }
        }
        public void AddScore()
        {
            _score++;
            if (UpdateScore != null) UpdateScore(_score); 
        }
        public void Reset()
        {
            _position = new Vector2(350, 400);
            _score = 0;
            _health = 10;
            _bullets.Clear();
        }

        public object SaveData()
        {
            List<BulletData> bullets = new List<BulletData>();
            foreach (Bullet bullet in _bullets)
            {
                bullets.Add((BulletData)bullet.SaveData());
            }
            PlayerData data = new PlayerData();
            data.X = (int)_position.X;
            data.Y = (int)_position.Y;
            data.Health = _health;
            data.Score = _score;
            data.Timer = _timer;
            data.Bullets = bullets;

            return data;
        }
        public void LoadData(object data, ContentManager content)
        {
            if (!(data is PlayerData))
            {
                return;
            }
            PlayerData playerData = (PlayerData)data;
            _position = new Vector2(playerData.X, playerData.Y);
            _health = playerData.Health;
            _score = playerData.Score;
            _timer = playerData.Timer;
            foreach (var bullet in playerData.Bullets)
            {
                Bullet bull = new Bullet(new Vector2(0, -4),
                    "bullet", "laserFire", _heightScreen);
                bull.LoadData(bullet, content);
                bull.LoadContent(content);
                _bullets.Add(bull);
            }
        }
    }
}
