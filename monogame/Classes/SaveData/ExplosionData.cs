using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogame.Classes.SaveData
{
    public class ExplosionData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double Timer { get; set; }
        public int FrameNum { get; set; }
        public bool IsAlive { get; set; }
    }
}
