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
        protected Anchor _parentAnchor;
        protected Anchor _alignmentAnchor;
        protected Texture2D _skin;
        protected bool _display;
        
        public Vector2 RelativePosition { get => _relativePosition; set => _relativePosition = value; }
        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }
        public Anchor AlignmentAnchor { get => _alignmentAnchor; set => _alignmentAnchor = value; }
        public bool Display { get => _display; set => _display = value; }

        public Widget()
        {
            _relativePosition = new Vector2(0, 0);
            _parentAnchor = Anchor.TopLeft;
            _alignmentAnchor = Anchor.TopLeft;
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

            float relativePositionX = _relativePosition.X * Engine.Content.DEFAULT_SCALE;
            float relativePositionY = _relativePosition.Y * Engine.Content.DEFAULT_SCALE;

            float width = _width * Engine.Content.DEFAULT_SCALE;
            float height = _height * Engine.Content.DEFAULT_SCALE;

            float parentWidth = viewport.Width;
            float parentHeight = viewport.Height;

            if (_parent != null)
            {
                Vector2 absolutePosition = _parent.AbsolutePosition();
                x = absolutePosition.X;
                y = absolutePosition.Y;

                parentWidth = _parent.Width * Engine.Content.DEFAULT_SCALE;
                parentHeight = _parent.Height * Engine.Content.DEFAULT_SCALE;
            }

            switch(_parentAnchor)
            {
                case Anchor.TopLeft:
                    x += relativePositionX;
                    y += relativePositionY;
                    break;
                case Anchor.TopRight:
                    x += parentWidth - relativePositionX - width;
                    y += relativePositionY;
                    break;
                case Anchor.BottomLeft:
                    x += relativePositionX;
                    y += parentHeight - relativePositionY - height;
                    break;
                case Anchor.BottomRight:
                    x += parentWidth - relativePositionX - width;
                    y += parentHeight - relativePositionY - height;
                    break;
                case Anchor.Center:
                    x += (parentWidth - width) / 2 - relativePositionX;
                    y += (parentHeight - height) / 2 - relativePositionY;
                    break;
            }

            return new Vector2(x, y);
        }

        public Vector2 OriginPoint()
        {
            switch (_alignmentAnchor)
            {
                case Anchor.TopLeft:
                    return new Vector2(0, 0);
                case Anchor.TopRight:
                    return new Vector2(_width, 0);
                case Anchor.BottomLeft:
                    return new Vector2(0, _height);
                case Anchor.BottomRight:
                    return new Vector2(_width, _height);
                case Anchor.Center:
                default:
                    return new Vector2(_width/2, _height/2);
            }
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
