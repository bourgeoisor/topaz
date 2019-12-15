using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Topaz.Engine.Interface
{
    class Label : Widget
    {
        public string Text { get; private set; }

        public Label(Widget parent) : base()
        {
            Parent = parent;
        }

        public void Update()
        {
        }

        public void Draw()
        {
            Engine.Content.Instance.SpriteBatch.DrawString(
                Content.Instance.Font,
                Text,
                AbsolutePosition(),
                new Color(135, 90, 12),
                0,
                OriginPoint(),
                Engine.Content.DEFAULT_FONT_SCALE,
                SpriteEffects.None,
                0f
            );
        }

        public void SetText(string text)
        {
            Vector2 measure = Content.Instance.Font.MeasureString(text);
            Width = (int)measure.X;
            Height = (int)measure.Y;
            Text = text;
        }
    }
}
