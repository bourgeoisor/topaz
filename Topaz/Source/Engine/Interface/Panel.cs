using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Topaz.Engine.Interface
{
    abstract class Panel : Widget
    {
        public Panel() : base()
        {
            _display = false;
        }
    }
}
