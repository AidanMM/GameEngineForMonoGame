using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using MonoGameEngine.Engine_Components;
using MonoGameEngine.Game_Objects;

namespace MonoGameEngine.Levels
{
    class Level1 : BaseState
    {

        public Level1()
        {
            AddObjectToHandler("Player", new Player());
        }
    }
}
