﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace monogame.Classes
{
    public class Player
    {
        private Vector2 _position;
        private Texture2D _texture;
        private float _speed = 7;
        private Rectangle _collision;
        private List<Bullet> _bullets = new List<Bullet>();
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
        public Player() 
        {
            _position = new Vector2(30,30);
            _texture = null;
            _collision = new Rectangle((int)_position.X, (int)_position.Y, 0, 0);
        }
        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("player");
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
                Bullet bullet = new Bullet();
                bullet.Position = new Vector2(_position.X + _texture.Width / 2 -
                    bullet.Width / 2, _position.Y + bullet.Height / 4);
                bullet.LoadContent(content);
                _bullets.Add(bullet);
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
    }
}
