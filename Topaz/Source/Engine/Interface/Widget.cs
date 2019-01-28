using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Topaz.Engine.Interface
{
    abstract class Widget
    {
        protected Widget _parent;
        protected Vector2 _relativePosition;
        protected int _width;
        protected int _height;
        protected Anchor _anchor;
        protected Texture2D _skin;
        protected bool _display;

        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }
        public Vector2 RelativePosition { get => _relativePosition; set => _relativePosition = value; }
        public bool Display { get => _display; set => _display = value; }

        public Widget()
        {
            _relativePosition = new Vector2(0, 0);
            _anchor = Anchor.TopLeft;
            _display = true;
        }

        public bool MouseIsIntersecting()
        {
            MouseState mouse = Mouse.GetState();
            Vector2 absolutePosition = AbsolutePosition();
            Rectangle boundingBox = new Rectangle(
                (int)absolutePosition.X,
                (int)absolutePosition.Y,
                (int)Engine.Content.DEFAULT_SCALE * _width,
                (int)Engine.Content.DEFAULT_SCALE * _height
            );
            return boundingBox.Contains(mouse.X, mouse.Y);
        }

        public Vector2 AbsolutePosition()
        {
            Viewport viewport = Engine.Window.Instance.GetViewport();

            float x = 0;
            float y = 0;

            float parentWidth = viewport.Width;
            float parentHeight = viewport.Height;

            if (_parent != null)
            {
                Vector2 absolutePosition = _parent.AbsolutePosition();
                x = absolutePosition.X;
                y = absolutePosition.Y;

                parentWidth = _parent.Width;
                parentHeight = _parent.Height;
            }

            switch(_anchor)
            {
                case Anchor.TopLeft:
                    x += _relativePosition.X;
                    y += _relativePosition.Y;
                    break;
                case Anchor.TopRight:
                    x += parentWidth - _relativePosition.X - _width * Engine.Content.DEFAULT_SCALE;
                    y += _relativePosition.Y;
                    break;
                case Anchor.BottomLeft:
                    x += _relativePosition.X;
                    y += parentHeight - _relativePosition.Y - _height * Engine.Content.DEFAULT_SCALE;
                    break;
                case Anchor.BottomRight:
                    x += parentWidth - _relativePosition.X - _width * Engine.Content.DEFAULT_SCALE;
                    y += parentHeight - _relativePosition.Y - _height * Engine.Content.DEFAULT_SCALE;
                    break;
                case Anchor.Center:
                    x += (parentWidth - _width * Engine.Content.DEFAULT_SCALE) / 2 - _relativePosition.X;
                    y += (parentHeight - _height * Engine.Content.DEFAULT_SCALE) / 2 - _relativePosition.Y;
                    break;
            }

            return new Vector2(x, y);
        }

        public void ToggleDisplay()
        {
            _display = !_display;
        }
    }

    public enum Anchor
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Center
    }
}
