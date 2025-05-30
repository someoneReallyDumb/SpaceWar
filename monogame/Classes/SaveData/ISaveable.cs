﻿using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogame.Classes.SaveData
{
    public interface ISaveable
    {
        object SaveData();
        void LoadData(object data, ContentManager content);
    }
}
