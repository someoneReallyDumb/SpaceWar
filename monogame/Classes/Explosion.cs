﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using monogame.Classes.SaveData;

namespace monogame.Classes
{
    public class Explosion : ISaveable
    {
        private Texture2D _texture;
        private Vector2 _position;  //1983x717
        private double _time = 0.0d;
        private double _duration = 30.0d;
        private int _frameNumber = 0;
        private int _widthFrame = 117;
        private int _heightFrame = 117;
        private Rectangle _sourceRectangle;
        private SoundEffect _soundEffect;
        public bool IsAlive { get; set; } = true;
        public int Width
        {
            get { return _widthFrame; }
        }
        public int Height
        {
            get { return _heightFrame; }
        }
        public Vector2 Position
        {
            set { _position = value; }
        }
        public Explosion(Vector2 position)
        {
            _texture = null;
            _position = position;
            _sourceRectangle = new Rectangle(_frameNumber * _widthFrame, 0,
                _widthFrame, _heightFrame);
        }
        public void Update(GameTime gameTime)
        {
            _time += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_time > _duration)
            {
                _frameNumber++;
                _time = 0;
            }
            if (_frameNumber == 17)
            {
                IsAlive = false;
            }
            _sourceRectangle = new Rectangle(_frameNumber * _widthFrame, 0,
                _widthFrame, _heightFrame);
            Debug.WriteLine("Time: " + gameTime.ElapsedGameTime.TotalMilliseconds);
        }
        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("explosion3");
            _soundEffect = content.Load<SoundEffect>("explosion");
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, _sourceRectangle, Color.White);
        }
        public void PlaySoundEffect()
        {
            SoundEffectInstance instance = _soundEffect.CreateInstance();
            instance.Volume = 0.01f;
            instance.Play();
        }

        public object SaveData()
        {
            ExplosionData explosionData = new ExplosionData();
            explosionData.X = (int)_position.X;
            explosionData.Y = (int)_position.Y;
            explosionData.Timer = _time; //
            explosionData.FrameNum = (int)_frameNumber;
            explosionData.IsAlive = IsAlive;
            return explosionData;
        }

        public void LoadData(object data, ContentManager content)
        {
            if (!(data is ExplosionData))
            {
                return;
            }
            ExplosionData explosionData = (ExplosionData)data;
            _position = new Vector2(explosionData.X, explosionData.Y); 
            _time = explosionData.Timer;
            _frameNumber = explosionData.FrameNum;
            IsAlive = explosionData.IsAlive;
            LoadContent(content);
        }
    }
}
