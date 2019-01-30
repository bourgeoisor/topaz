using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Topaz.Engine.Interface
{
    class Label : Widget
    {
        private string _text;

        public Label(Widget parent) : base()
        {
            _parent = parent;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime)
        {
            Engine.Content.Instance.SpriteBatch.DrawString(
                Content.Instance.Font,
                _text,
                AbsolutePosition(),
                new Color(135, 90, 12),
                0,
                OriginPoint(),
                Engine.Content.DEFAULT_SCALE,
                SpriteEffects.None,
                0f
            );
        }

        public void SetText(string text)
        {
            _text = text;

            Vector2 size = Content.Instance.Font.MeasureString(text);
            _width = (int)size.X;
            _height = (int)size.Y;
        }
    }
}
