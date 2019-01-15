using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Topaz.Interface
{
    class DebugInfo
    {
        const int OFFSET_X = 20;
        const int OFFSET_Y = 20;

        Scene.WorldScene _world;
        Engine.Window _window;
        Engine.Content _content;
        Networking.Client _client;

        string _text;
        bool _displaying;

        public DebugInfo(Scene.WorldScene world)
        {
            _world = world;
            _window = Engine.Window.Instance;
            _content = Engine.Content.Instance;
            _client = Networking.Client.Instance;

            _text = "";
            _displaying = false;
        }

        public void Update(GameTime gameTime)
        {
            Rectangle viewport = _window.GetViewport().Bounds;
            MouseState mouse = Mouse.GetState();
            GamePadState gamePad = GamePad.GetState(PlayerIndex.One);

            double fps = Math.Round(1 / gameTime.ElapsedGameTime.TotalSeconds);

            _text = "Video\n";
            _text += " " + fps + "fps (" + viewport.Width + "x" + viewport.Height + ")\n";
            _text += "Input\n";
            _text += " Mouse Coords: " + mouse.Position.ToString() + "\n";
            _text += " Tile Coords: " + _world.GetMouseTileCoordinates() + "\n";
            _text += "Player\n";
            _text += " Coordinates: " + _client.Player.GetCoordinates() + "\n";
            _text += "Network\n";
            _text += " Status: " + _client.GetClientConnectionStatus() + " (" + _client.LastLatency + "ms)" + "\n";
            _text += " Last Msg: " + _client.LastNetMessage + "\n\n";
            _text += "Binding\n";
            _text += " F1 - Toggle Debug Panel\n";
            _text += " F2 - Toggle Collision Bounds\n";
            _text += " F11 - Connect to Server\n";
        }

        public void Draw(GameTime gameTime)
        {
            if (_displaying)
            {
                Vector2 origin = new Vector2(0, 0);
                Vector2 position = new Vector2(OFFSET_X, OFFSET_Y);

                _content.SpriteBatch.Begin();
                _content.DrawStringOutline(
                    _content.Font,
                    _text, position,
                    Color.Black,
                    Color.White,
                    0,
                    origin,
                    1.0f,
                    SpriteEffects.None,
                    0.5f
                );
                _content.SpriteBatch.End();
            }
        }

        public void ToggleDisplay()
        {
            _displaying = !_displaying;
        }
    }
}
