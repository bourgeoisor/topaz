using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Topaz.Engine.Interface
{
    class Checkbox : Widget
    {
        private bool _hover;
        private bool _checked;

        private Action<bool> _onStateChanged;
        private Action _onChecked;
        private Action _onUnchecked;

        public bool Checked { get => _checked; set => _checked = value; }

        public Checkbox(Widget parent) : base()
        {
            _parent = parent;
            _width = 9;
            _height = 9;
            _skin = Engine.Content.Instance.GetTexture("Interface/checkbox");

            _hover = false;
            _checked = false;
        }

        public void SetOnStateChanged(Action<bool> callback)
        {
            _onStateChanged = callback;
        }

        public void SetOnChecked(Action callback)
        {
            _onChecked = callback;
        }

        public void SetOnUnchecked(Action callback)
        {
            _onUnchecked = callback;
        }

        public void Update(GameTime gameTime)
        {
            if (!_display) return;

            if (MouseIsIntersecting())
            {
                if (Engine.Input.Instance.LeftButtonPressed())
                {
                    _checked = !_checked;

                    if (_checked) _onChecked?.Invoke();
                    else _onUnchecked?.Invoke();

                    _onStateChanged?.Invoke(_checked);
                }
            }

            _hover = MouseIsIntersecting();
        }

        public void Draw(GameTime gameTime)
        {
            if (!_display) return;

            int sprite = 0;
            if (_hover && !_checked) sprite = 1;
            if (!_hover && _checked) sprite = 2;
            if (_hover && _checked) sprite = 3;
            Rectangle source = new Rectangle(9*sprite, 0, 9, 9);

            Engine.Content.Instance.SpriteBatch.Draw(
                _skin,
                AbsolutePosition(),
                source,
                Color.White,
                0f,
                OriginPoint(),
                Engine.Content.DEFAULT_SCALE,
                SpriteEffects.None,
                0f
            );
        }
    }
}
