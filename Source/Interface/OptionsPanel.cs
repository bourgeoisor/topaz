using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Topaz.Interface
{
    class OptionsPanel : Engine.Interface.Panel
    {
        Engine.Interface.Checkbox _muteCheckbox;
        Engine.Interface.Label _muteLabel;

        Engine.Interface.Checkbox _fullscreenCheckbox;
        Engine.Interface.Label _fullscreenLabel;

        public OptionsPanel() : base()
        {
            RelativePosition = new Vector2(0, 0);
            Width = 200;
            Height = 120;
            ParentAnchor = Anchor.Center;
            Skin = Engine.Content.Instance.GetTexture("Interface/options");
            
            _muteCheckbox = new Engine.Interface.Checkbox(this);
            _muteCheckbox.IsChecked = Engine.Window.Instance.Settings.Audio.Mute;
            _muteCheckbox.RelativePosition = new Vector2(4, 4);
            _muteCheckbox.SetOnStateChanged(delegate (bool isChecked) {
                Engine.Window.Instance.Settings.Audio.Mute = isChecked;
                Engine.Content.Instance.SyncSettings();
            });

            _muteLabel = new Engine.Interface.Label(this);
            _muteLabel.RelativePosition = new Vector2(16, 4);
            _muteLabel.SetText("Mute Audio");

            _fullscreenCheckbox = new Engine.Interface.Checkbox(this);
            _fullscreenCheckbox.IsChecked = Engine.Window.Instance.Settings.Video.Fullscreen;
            _fullscreenCheckbox.RelativePosition = new Vector2(4, 16);
            _fullscreenCheckbox.SetOnStateChanged(delegate (bool isChecked) {
                Engine.Window.Instance.ToggleFullscreen(isChecked);
            });

            _fullscreenLabel = new Engine.Interface.Label(this);
            _fullscreenLabel.RelativePosition = new Vector2(16, 16);
            _fullscreenLabel.SetText("Fullscreen");
        }

        public void Update(GameTime gameTime)
        {
            if (!IsDisplayed) return;
            if (!MouseIsIntersecting()) return;

            _muteCheckbox.Update(gameTime);
            _fullscreenCheckbox.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            if (!IsDisplayed) return;

            Engine.Content.Instance.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, null);

            Engine.Content.Instance.SpriteBatch.Draw(
                Skin,
                AbsolutePosition(),
                null,
                Color.White,
                0f,
                OriginPoint(),
                Engine.Content.DEFAULT_SCALE,
                SpriteEffects.None,
                0f
            );

            _muteCheckbox.Draw(gameTime);
            _muteLabel.Draw(gameTime);

            _fullscreenCheckbox.Draw(gameTime);
            _fullscreenLabel.Draw(gameTime);

            Engine.Content.Instance.SpriteBatch.End();
        }
    }
}
