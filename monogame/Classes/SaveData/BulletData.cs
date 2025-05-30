using System;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogame.Classes.SaveData;

namespace monogame.Classes.SaveData
{
    public class BulletData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsAlive { get; set; }
        public List<BulletData> Bullets { get; set; }
    }
}
