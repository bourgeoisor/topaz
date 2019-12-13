using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topaz.Engine.Interface
{
    class Button : Widget
    {
        public Action OnClicked { get; private set; }

        public bool IsPressed { get; private set; }
        public bool IsHovered { get; private set; }

        public Button(Widget parent) : base()
        {
            Parent = parent;
            Width = 50;
            Height = 11;
            Skin = Engine.Content.Instance.GetTexture("Interface/button");

            IsHovered = false;
            IsPressed = false;
        }

        public void SetOnClicked(Action callback)
        {
            OnClicked = callback;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsDisplaying) return;

            if (MouseIsIntersecting())
            {
                if (Engine.Input.LeftButtonPressed())
                {
                    IsPressed = true;
                } 
                else
                {
                    if (IsPressed)
                    {
                        OnClicked?.Invoke();
                    }

                    IsPressed = false;
                }
            }

            IsHovered = MouseIsIntersecting();
        }

        public void Draw(GameTime gameTime)
        {
            if (!IsDisplaying) return;

            int sprite = 0;
            if (IsHovered && !IsPressed) sprite = 1;
            if (IsHovered && IsPressed) sprite = 2;
            Rectangle source = new Rectangle(0, 11 * sprite, 50, 11);

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
