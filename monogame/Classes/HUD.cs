using System;
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
    public class HUD : ISaveable
    {
        private HealthBar _healthBar;
        private Label _labelScore;
        public HUD() 
        {
            Vector2 position = new Vector2(20, 20);
            _healthBar = new HealthBar(new Vector2(20, 20), 10, 200, 15);
            _labelScore = new Label(new Vector2(
                position.X, position.Y + _healthBar.DestinationRectangle.Height + 20),
                "Score: 0", Color.White);
        }
        public void LoadContent(GraphicsDevice graphics, ContentManager content)
        {
            _healthBar.LoadContent(graphics);
            _labelScore.LoadContent(content);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            _healthBar.Draw(spriteBatch);
            _labelScore.Draw(spriteBatch);
        }
        public void OnPlayerTakeDamage(int health)
        {
            _healthBar.NumParts = health;
        }
        public void OnScoreUpdated(int score)
        {
            _labelScore.Text = $"Score: {score}";
        }
        public void OnPlayerHealed()
        {
            _healthBar.NumParts += 5;
            if (_healthBar.NumParts > 10)
            {
                _healthBar.NumParts = 10;
            }
        }
        public void Reset()
        {
            _healthBar.NumParts = 10;
            _labelScore.Text = "Score: 0";
        }

        public object SaveData()
        {
            HUDData hudData = new HUDData();
            hudData.ScoreText = _labelScore.Text;
            hudData.HealthNumParts = _healthBar.NumParts;
            return hudData;
        }

        public void LoadData(object data, ContentManager content)
        {
            if (!(data is HUDData))
            {
                return;
            }
            HUDData hudData = (HUDData)data;
            _healthBar.NumParts = hudData.HealthNumParts;
            _labelScore.Text = hudData.ScoreText;
        }
    }
}
