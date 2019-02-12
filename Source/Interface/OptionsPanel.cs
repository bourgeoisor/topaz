using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Topaz.Interface
{
    class OptionsPanel : Engine.Interface.Panel
    {
        Engine.Interface.Checkbox _musicMuteCheckbox;
        Engine.Interface.Label _musicMuteLabel;

        Engine.Interface.Checkbox _soundsMuteCheckbox;
        Engine.Interface.Label _soundsMuteLabel;

        Engine.Interface.Checkbox _fullscreenCheckbox;
        Engine.Interface.Label _fullscreenLabel;

        public OptionsPanel() : base()
        {
            RelativePosition = new Vector2(0, 0);
            Width = 200;
            Height = 120;
            ParentAnchor = Anchor.Center;
            Skin = Engine.Content.Instance.GetTexture("Interface/options");

            {
                _musicMuteCheckbox = new Engine.Interface.Checkbox(this);
                _musicMuteCheckbox.IsChecked = Engine.Core.Instance.Settings.Audio.MusicMute;
                _musicMuteCheckbox.RelativePosition = new Vector2(4, 4);
                _musicMuteCheckbox.SetOnStateChanged(delegate (bool isChecked) {
                    Engine.Core.Instance.Settings.Audio.MusicMute = isChecked;
                    Engine.Content.Instance.SyncFromSettings();
                });

                _musicMuteLabel = new Engine.Interface.Label(this);
                _musicMuteLabel.RelativePosition = new Vector2(16, 4);
                _musicMuteLabel.SetText("Mute Music");
            }

            {
                _soundsMuteCheckbox = new Engine.Interface.Checkbox(this);
                _soundsMuteCheckbox.IsChecked = Engine.Core.Instance.Settings.Audio.SoundsMute;
                _soundsMuteCheckbox.RelativePosition = new Vector2(4, 16);
                _soundsMuteCheckbox.SetOnStateChanged(delegate (bool isChecked) {
                    Engine.Core.Instance.Settings.Audio.SoundsMute = isChecked;
                    Engine.Content.Instance.SyncFromSettings();
                });

                _soundsMuteLabel = new Engine.Interface.Label(this);
                _soundsMuteLabel.RelativePosition = new Vector2(16, 16);
                _soundsMuteLabel.SetText("Mute Sounds");
            }

            {
                _fullscreenCheckbox = new Engine.Interface.Checkbox(this);
                _fullscreenCheckbox.IsChecked = Engine.Core.Instance.Settings.Video.Fullscreen;
                _fullscreenCheckbox.RelativePosition = new Vector2(4, 28);
                _fullscreenCheckbox.SetOnStateChanged(delegate (bool isChecked) {
                    Engine.Core.Instance.ToggleFullscreen(isChecked);
                });

                _fullscreenLabel = new Engine.Interface.Label(this);
                _fullscreenLabel.RelativePosition = new Vector2(16, 28);
                _fullscreenLabel.SetText("Fullscreen");
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!IsDisplaying) return;
            if (!MouseIsIntersecting()) return;

            _musicMuteCheckbox.Update(gameTime);
            _soundsMuteCheckbox.Update(gameTime);
            _fullscreenCheckbox.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            if (!IsDisplaying) return;

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

            _musicMuteCheckbox.Draw(gameTime);
            _musicMuteLabel.Draw(gameTime);

            _soundsMuteCheckbox.Draw(gameTime);
            _soundsMuteLabel.Draw(gameTime);

            _fullscreenCheckbox.Draw(gameTime);
            _fullscreenLabel.Draw(gameTime);

            Engine.Content.Instance.SpriteBatch.End();
        }
    }
}
