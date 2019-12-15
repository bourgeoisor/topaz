using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Topaz.Engine.Interface
{
    abstract class Widget : IGameObject
    {
        public Engine.Core Core = Engine.Core.Instance;

        public Widget Parent { get; protected set; }
        public List<Widget> Children { get; private set; }
        public Texture2D Skin { get; protected set; }

        public Vector2 RelativePosition { get; set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public Anchor ParentAnchor { get; protected set; }
        public Anchor AlignmentAnchor { get; protected set; }

        public bool Visible { get; set; }

        public enum Anchor { TopLeft, TopRight, BottomLeft, BottomRight, Center }

        public Widget()
        {
            Children = new List<Widget>();
            RelativePosition = new Vector2(0, 0);
            ParentAnchor = Anchor.TopLeft;
            AlignmentAnchor = Anchor.TopLeft;
            Visible = true;
        }

        public virtual void Update()
        {
            foreach (Widget widget in Children)
            {
                widget.Update();
            }
        }

        public virtual void Draw()
        {
            foreach (Widget widget in Children)
            {
                widget.Draw();
            }
        }

        public bool MouseIsIntersecting()
        {
            MouseState mouse = Mouse.GetState();
            Vector2 absolutePosition = AbsolutePosition();
            Rectangle boundingBox = new Rectangle(
                (int)absolutePosition.X,
                (int)absolutePosition.Y,
                (int)Engine.Content.DEFAULT_SCALE * Width,
                (int)Engine.Content.DEFAULT_SCALE * Height
            );
            return boundingBox.Contains(mouse.X, mouse.Y);
        }

        public Vector2 AbsolutePosition()
        {
            Viewport viewport = Engine.Core.Instance.GetViewport();

            float x = 0;
            float y = 0;

            float relativePositionX = RelativePosition.X * Engine.Content.DEFAULT_SCALE;
            float relativePositionY = RelativePosition.Y * Engine.Content.DEFAULT_SCALE;

            float width = Width * Engine.Content.DEFAULT_SCALE;
            float height = Height * Engine.Content.DEFAULT_SCALE;

            float parentWidth = viewport.Width;
            float parentHeight = viewport.Height;

            if (Parent != null)
            {
                Vector2 absolutePosition = Parent.AbsolutePosition();
                x = absolutePosition.X;
                y = absolutePosition.Y;

                parentWidth = Parent.Width * Engine.Content.DEFAULT_SCALE;
                parentHeight = Parent.Height * Engine.Content.DEFAULT_SCALE;
            }

            switch(ParentAnchor)
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
            switch (AlignmentAnchor)
            {
                case Anchor.TopLeft:
                    return new Vector2(0, 0);
                case Anchor.TopRight:
                    return new Vector2(Width, 0);
                case Anchor.BottomLeft:
                    return new Vector2(0, Height);
                case Anchor.BottomRight:
                    return new Vector2(Width, Height);
                case Anchor.Center:
                default:
                    return new Vector2(Width / 2, Height / 2);
            }
        }

        public void ToggleDisplay()
        {
            Visible = !Visible;
        }
    }
}
