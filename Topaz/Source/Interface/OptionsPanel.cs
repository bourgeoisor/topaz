using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Topaz.Interface
{
    class OptionsPanel : Engine.Interface.Panel
    {
        Engine.Interface.Checkbox _checkboxMute;

        public OptionsPanel() : base()
        {
            _relativePosition = new Vector2(0, 0);
            _width = 200;
            _height = 120;
            _anchor = Engine.Interface.Anchor.Center;
            _skin = Engine.Content.Instance.GetTexture("Interface/options");
            
            _checkboxMute = new Engine.Interface.Checkbox(this);
            _checkboxMute.RelativePosition = new Vector2(10, 10);
            _checkboxMute.SetOnChecked(delegate () { Console.WriteLine("checked!"); });
            _checkboxMute.SetOnUnchecked(delegate () { Console.WriteLine("unchecked!"); });
        }

        public void Update(GameTime gameTime)
        {
            if (!_display) return;
            if (!MouseIsIntersecting()) return;

            _checkboxMute.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            if (!_display) return;

            Engine.Content.Instance.SpriteBatch.Draw(
                _skin,
                AbsolutePosition(),
                null,
                Color.White,
                0f,
                new Vector2(0, 0),
                Engine.Content.DEFAULT_SCALE,
                SpriteEffects.None,
                0f
            );

            _checkboxMute.Draw(gameTime);
        }
    }
}
