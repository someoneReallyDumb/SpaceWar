using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation;
namespace monogame.Classes
{
    public class MainMenu : Menu
    {
        public event Action OnPlayingStarted;
        public event Action OnLoadGame;
        public MainMenu(int widthScreen, int heightScreen) : base(widthScreen, heightScreen) 
        { 
            _buttonList.Add(new Label(new Vector2(0, 0), "Play", Color.White));
            _buttonList.Add(new Label(new Vector2(0, 40), "LoadGame", Color.White));
            _buttonList.Add(new Label(new Vector2(0, 40), "Exit", Color.White));
        }
        public override void PressEnter()
        {
            if (_selected == 0)
            {
                if (OnPlayingStarted != null)
                {
                    OnPlayingStarted();
                }
            }
            else if (_selected == 1)
            {
                if (OnLoadGame != null)
                    OnLoadGame();
            }
            else if (_selected == 2)
            {
                Game1.gameMode = GameMode.Exit;
            }
        }
    }
}
