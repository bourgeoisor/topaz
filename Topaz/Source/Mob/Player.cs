using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topaz.Mob
{
    class Player : Mob
    {
        public Player() : base()
        {
            Sprite = Engine.Content.Instance.GetTexture("Temp/lucas");
        }
    }
}
