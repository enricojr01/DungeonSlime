using System;
using DungeonSlime.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using MonoGameGum;
using MonoGameGum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;

namespace DungeonSlime.Scenes
{
    public class TitleScene : Scene
    {
        private const string DUNGEON_TEXT = "Dungeon";
        private const string SLIME_TEXT = "Slime";
        private const string PRESS_ENTER_TEXT = "Press Enter to Start";

        private SpriteFont _font;
        private SpriteFont _font5x;
        private Vector2 _dungeonTextPos;
        private Vector2 _dungeonTextOrigin;
        private Vector2 _slimeTextPos;
        private Vector2 _slimeTextOrigin;
        private Vector2 _pressEnterPos;
        private Vector2 _pressEnterOrigin;
        private Texture2D _backgroundPattern;
        private Rectangle _backgroundDestination;
        private Vector2 _backgroundOffset;
        private float _scrollSpeed = 50.0f;

        // UI bullshit
        private SoundEffect _uiSoundEffect;
        private Panel _titleScreenButtonsPanel;
        private Panel _optionsPanel;
        private AnimatedButton _optionsButton;
        private AnimatedButton _optionsBackButton;

        private TextureAtlas _atlas;

        public override void Initialize()
        {
            base.Initialize();

            // While on the title screen, you are allowed to hit
            // Esc to close the game.
            Core.ExitOnEscape = true;
            
            // NOTE: remember that to avoid division operations
            // we multiply by 0.5f instead because it's faster.
            Vector2 size = this._font5x.MeasureString(DUNGEON_TEXT);
            this._dungeonTextPos = new Vector2(640, 100);
            this._dungeonTextOrigin = size * 0.5f;

            size = this._font5x.MeasureString(SLIME_TEXT);
            this._slimeTextPos = new Vector2(757, 207);
            this._slimeTextOrigin = size * 0.5f;

            size = this._font.MeasureString(PRESS_ENTER_TEXT);
            this._pressEnterPos = new Vector2(640, 620);
            this._pressEnterOrigin = size * 0.5f;

            this._backgroundOffset = Vector2.Zero;
            this._backgroundDestination = Core.GraphicsDevice.PresentationParameters.Bounds;

            this.InitializeUI();
        }

        private void InitializeUI()
        {
            GumService.Default.Root.Children.Clear();
            CreateTitlePanel();
            CreateOptionsPanel();
        }

        public override void LoadContent()
        {
            // this font is used for standard text
            this._font = Core.Content.Load<SpriteFont>("fonts/04B_30");

            // this font is used for the title text
            this._font5x = Content.Load<SpriteFont>("fonts/04B_30_5x");

            // this image is used for the background pattern on the title screen.
            this._backgroundPattern = Content.Load<Texture2D>("images/background-pattern");

            // this sound effect plays when the player interacts with UI elements.
            this._uiSoundEffect = Core.Content.Load<SoundEffect>("audio/ui");

            // Load the texture atlas from the xml configuration file.
            this._atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");
       }

        public override void Update(GameTime gameTime)
        {
            //if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Enter))
            //{
            //    Core.ChangeScene(new GameScene());
            //}

            float offset = this._scrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            this._backgroundOffset.X -= offset;
            this._backgroundOffset.Y -= offset;

            this._backgroundOffset.X %= this._backgroundPattern.Width;
            this._backgroundOffset.Y %= this._backgroundPattern.Height;

            GumService.Default.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Core.GraphicsDevice.Clear(new Color(32, 40, 78, 255));

            // Draw background step, using the PointWrap sampler state
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointWrap);
            Core.SpriteBatch.Draw(
                this._backgroundPattern,
                this._backgroundDestination,
                new Rectangle(this._backgroundOffset.ToPoint(), this._backgroundDestination.Size),
                Color.White * 0.5f
            );
            Core.SpriteBatch.End();
            if (this._titleScreenButtonsPanel.IsVisible)
            {
                Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
                
                Color dropShadowColor = new Color(19, 23, 46, 175);
                dropShadowColor = Color.Black * 0.5f;
                
                // Draw the dungeon text slightly offset from the original position and
                // with a transparent color to give it a drop shadow.
                Core.SpriteBatch.DrawString(
                    this._font5x,
                    DUNGEON_TEXT,
                    this._dungeonTextPos + new Vector2(10, 10),
                    dropShadowColor,
                    0.0f,
                    this._dungeonTextOrigin,
                    1.0f,
                    SpriteEffects.None,
                    1.0f
                );

                Core.SpriteBatch.DrawString(
                    this._font5x,
                    DUNGEON_TEXT,
                    this._dungeonTextPos,
                    Color.White,
                    0.0f,
                    this._dungeonTextOrigin,
                    1.0f,
                    SpriteEffects.None,
                    1.0f
                );

                Core.SpriteBatch.DrawString(
                    this._font5x,
                    SLIME_TEXT,
                    this._slimeTextPos + new Vector2(10, 10),
                    dropShadowColor,
                    0.0f,
                    this._slimeTextOrigin,
                    1.0f,
                    SpriteEffects.None,
                    1.0f
                );

                Core.SpriteBatch.DrawString(
                    this._font5x,
                    SLIME_TEXT,
                    this._slimeTextPos,
                    Color.White,
                    0.0f,
                    this._slimeTextOrigin,
                    1.0f,
                    SpriteEffects.None,
                    1.0f
                );

                //Core.SpriteBatch.DrawString(
                //    this._font,
                //    PRESS_ENTER_TEXT,
                //    this._pressEnterPos,
                //    dropShadowColor,
                //    0.0f,
                //    this._pressEnterOrigin,
                //    1.0f,
                //    SpriteEffects.None,
                //    1.0f
                //);

                Core.SpriteBatch.End();
            }

            GumService.Default.Draw();
        }

        private void CreateTitlePanel()
        {
            this._titleScreenButtonsPanel = new Panel();
            this._titleScreenButtonsPanel.Dock(Gum.Wireframe.Dock.Fill);
            this._titleScreenButtonsPanel.AddToRoot();

            var startButton = new AnimatedButton(this._atlas);
            startButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
            startButton.Visual.X = 50;
            startButton.Visual.Y = -12;
            startButton.Text = "Start";
            startButton.Click += this.HandleStartClicked;
            this._titleScreenButtonsPanel.AddChild(startButton);

            this._optionsButton = new AnimatedButton(this._atlas);
            this._optionsButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
            this._optionsButton.Visual.X = -50;
            this._optionsButton.Visual.Y = -12;
            this._optionsButton.Text = "Options";
            this._optionsButton.Click += this.HandleOptionsClicked;
            this._titleScreenButtonsPanel.AddChild(_optionsButton);

            startButton.IsFocused = true;
        }

        private void CreateOptionsPanel()
        {
            this._optionsPanel = new Panel();
            this._optionsPanel.Dock(Gum.Wireframe.Dock.Fill);
            this._optionsPanel.IsVisible = false;
            this._optionsPanel.AddToRoot();

            var optionsText = new TextRuntime();
            optionsText.X = 10;
            optionsText.Y = 10;
            optionsText.Text = "OPTIONS";
            optionsText.UseCustomFont = true;
            optionsText.FontScale = 0.5f;
            optionsText.CustomFontFile = @"font/04b_30.fnt";
            this._optionsPanel.AddChild(optionsText);

            var musicSlider = new OptionsSlider(this._atlas);
            musicSlider.Name = "MusicSlider";
            musicSlider.Text = "Music";
            musicSlider.Anchor(Gum.Wireframe.Anchor.Top);
            musicSlider.Visual.Y = 30f;
            musicSlider.Minimum = 0;
            musicSlider.Maximum = 1;
            musicSlider.Value = Core.Audio.SongVolume;
            musicSlider.SmallChange = 0.1;
            musicSlider.LargeChange = 0.2;
            musicSlider.ValueChanged += this.HandleMusicSliderValueChanged;
            musicSlider.ValueChangeCompleted += this.HandleMusicSliderValueChangeCompleted;
            this._optionsPanel.AddChild(musicSlider);

            var sfxSlider = new OptionsSlider(this._atlas);
            sfxSlider.Name = "SfxSlider";
            sfxSlider.Text = "SFX";
            sfxSlider.Anchor(Gum.Wireframe.Anchor.Top);
            sfxSlider.Visual.Y = 93;
            sfxSlider.Minimum = 0;
            sfxSlider.Maximum = 1;
            sfxSlider.Value = Core.Audio.SoundEffectVolume;
            sfxSlider.SmallChange = 0.1;
            sfxSlider.LargeChange = 0.2;
            sfxSlider.ValueChanged += this.HandleSfxSliderChanged;
            sfxSlider.ValueChangeCompleted += this.HandleSfxSliderChangeCompleted;
            this._optionsPanel.AddChild(sfxSlider);

            this._optionsBackButton = new AnimatedButton(this._atlas);
            this._optionsBackButton.Text = "BACK";
            this._optionsBackButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
            this._optionsBackButton.X = -28f;
            this._optionsBackButton.Y = -10f;
            this._optionsBackButton.Click += this.HandleOptionsButtonBack;
            this._optionsPanel.AddChild(this._optionsBackButton);
        }

        private void HandleStartClicked(object sender, EventArgs e)
        {
            Core.Audio.PlaySoundEffect(this._uiSoundEffect);
            Core.ChangeScene(new GameScene());
        }

        private void HandleOptionsClicked(object sender, EventArgs e)
        {
            Core.Audio.PlaySoundEffect(this._uiSoundEffect);
            this._titleScreenButtonsPanel.IsVisible = false;
            this._optionsPanel.IsVisible = true;
            this._optionsBackButton.IsFocused = true;
        }

        private void HandleSfxSliderChanged(object sender, EventArgs args)
        {
            var slider = (Slider)sender;
            Core.Audio.SoundEffectVolume = (float)slider.Value;
        }

        private void HandleSfxSliderChangeCompleted(object sender, EventArgs e)
        {
            Core.Audio.PlaySoundEffect(this._uiSoundEffect);
        }

        private void HandleMusicSliderValueChanged(object sender, EventArgs args)
        {
            var slider = (Slider)sender;
            Core.Audio.SongVolume = (float)slider.Value;
        }

        private void HandleMusicSliderValueChangeCompleted(object sender, EventArgs args)
        {
            Core.Audio.PlaySoundEffect(this._uiSoundEffect);
        }
        
        private void HandleOptionsButtonBack(object sender, EventArgs e)
        {
            Core.Audio.PlaySoundEffect(this._uiSoundEffect);
            this._titleScreenButtonsPanel.IsVisible = true;
            this._optionsPanel.IsVisible = false;
            this._optionsButton.IsFocused = true;
        }
    }
}
