using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Topaz.Interface
{
    class DebugPanel : Engine.Interface.Panel
    {
        private Scene.WorldScene _world;
        private string _text;

        public DebugPanel(Scene.WorldScene world)
        {
            RelativePosition = new Vector2(5, 5);
            _world = world;
            _text = "";
        }

        public void Update(GameTime gameTime)
        {
            Rectangle viewport = Engine.Core.Instance.GetViewport().Bounds;
            MouseState mouse = Mouse.GetState();
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);

            double fps = Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds);

            _text = "Video\n";
            _text += " " + fps + "fps (" + viewport.Width + "x" + viewport.Height + ")\n";
            _text += "Input\n";
            _text += " Mouse Coords: " + mouse.Position.ToString() + "\n";
            _text += "Player\n";
            _text += " Coordinates: " + Networking.Client.Instance.Player.Coordinates + "\n";
            _text += "Network\n";
            _text += " Status: " + Networking.Client.Instance.GetClientConnectionStatus() + " (" + Networking.Client.Instance.LastLatency + "ms)" + "\n";
            _text += " Last Msg: " + Networking.Client.Instance.LastNetMessage;
        }

        public void Draw(GameTime gameTime)
        {
            if (!IsDisplaying) return;

            Engine.Content.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, null);

            Engine.Content.Instance.DrawStringOutline(
                Engine.Content.Instance.Font,
                _text,
                AbsolutePosition(),
                Color.Black,
                Color.White,
                0,
                OriginPoint(),
                Engine.Content.DEFAULT_SCALE,
                SpriteEffects.None,
                0.5f
            );

            Engine.Content.Instance.SpriteBatch.End();
        }
    }
}
