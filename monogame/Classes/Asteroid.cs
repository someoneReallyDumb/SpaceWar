﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogame.Classes.SaveData;
namespace monogame.Classes
{
    public class Asteroid : ISaveable
    {
        private Vector2 _position;
        private Texture2D _texture;
        private Rectangle _collision;
        public Rectangle Collision
        {
            get { return _collision; }
        }
        public bool IsAlive { get; set; }
        public int Width
        {
            get { return _texture.Width; }
        }
        public int Height
        {
            get { return _texture.Width; }
        }
        public Vector2 Position
        {
            set { _position = value; }
            get { return _position; }
        }
        public Asteroid() : this(Vector2.Zero)
        {
           
        }
        public Asteroid(Vector2 position)
        {
            _texture = null;
            _position = position;
            IsAlive = true;
            _collision = new Rectangle((int)_position.X, (int)_position.Y, 0, 0);
        }
        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("asteroid");
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, Color.White);
        }
        public void Update()
        {
            _position.Y += 2;
            _collision = new Rectangle((int)_position.X, (int)_position.Y,
                                       _texture.Width, _texture.Height);
        }
        public void LoadData(object data, ContentManager content)
        {
            if (!(data is AsteroidData))
            {
                return;
            }
            AsteroidData asteroidData = (AsteroidData)data;
            _position = new Vector2(asteroidData.X, asteroidData.Y);
            IsAlive = asteroidData.isAlive;
            LoadContent(content);
        }

        public object SaveData()
        {
            AsteroidData asteroidData = new AsteroidData();
            asteroidData.X = (int)_position.X;
            asteroidData.Y = (int)_position.Y;
            asteroidData.isAlive = IsAlive;
            return asteroidData;
        }
    }
}
