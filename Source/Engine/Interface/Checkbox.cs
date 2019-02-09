using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Topaz.Engine.Interface
{
    class Checkbox : Widget
    {
        public Action<bool> OnStateChanged { get; private set; }
        public Action OnChecked { get; private set; }
        public Action OnUnchecked { get; private set; }

        public bool IsChecked { get; set; }
        public bool IsHovered { get; private set; }

        public Checkbox(Widget parent) : base()
        {
            Parent = parent;
            Width = 9;
            Height = 9;
            Skin = Engine.Content.Instance.GetTexture("Interface/checkbox");

            IsHovered = false;
            IsChecked = false;
        }

        public void SetOnStateChanged(Action<bool> callback)
        {
            OnStateChanged = callback;
        }

        public void SetOnChecked(Action callback)
        {
            OnChecked = callback;
        }

        public void SetOnUnchecked(Action callback)
        {
            OnUnchecked = callback;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsDisplayed) return;

            if (MouseIsIntersecting())
            {
                if (Engine.Input.Instance.LeftButtonPressed())
                {
                    IsChecked = !IsChecked;

                    if (IsChecked) OnChecked?.Invoke();
                    else OnUnchecked?.Invoke();

                    OnStateChanged?.Invoke(IsChecked);
                }
            }

            IsHovered = MouseIsIntersecting();
        }

        public void Draw(GameTime gameTime)
        {
            if (!IsDisplayed) return;

            int sprite = 0;
            if (IsHovered && !IsChecked) sprite = 1;
            if (!IsHovered && IsChecked) sprite = 2;
            if (IsHovered && IsChecked) sprite = 3;
            Rectangle source = new Rectangle(9*sprite, 0, 9, 9);

            Engine.Content.Instance.SpriteBatch.Draw(
                Skin,
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
