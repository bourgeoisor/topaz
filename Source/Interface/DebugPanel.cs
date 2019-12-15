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

        public override void Update()
        {
            Rectangle viewport = Core.GetViewport().Bounds;
            MouseState mouse = Mouse.GetState();
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);

            _text = "FPS: " + Core.Time.FramesPerSecond + "\n";
            _text += "Display: " + viewport.Width + "x" + viewport.Height + "\n";
            _text += "Memory: " + Math.Round((GC.GetTotalMemory(false) / 1024f) / 1024f, 2) + "MiB\n";
            _text += "Mouse Coords: " + mouse.Position.ToString() + "\n";
            _text += "Coordinates: " + Networking.Client.Instance.Player.Coordinates + "\n";
            _text += "\n";
            _text += "Status: " + Networking.Client.Instance.GetClientConnectionStatus() + " (" + Networking.Client.Instance.LastLatency + "ms)" + "\n";
            _text += "Last Msg: " + Networking.Client.Instance.LastNetMessage;
        }

        public override void Draw()
        {
            if (!Visible) return;

            Engine.Content.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, null);

            Engine.Content.Instance.DrawStringOutline(
                Engine.Content.Instance.Font,
                _text,
                AbsolutePosition(),
                Color.Black,
                Color.White,
                OriginPoint(),
                1f,
                SpriteEffects.None,
                0.5f
            );

            Engine.Content.Instance.SpriteBatch.End();
        }
    }
}
