using System;
using DungeonSlime.UI;
using Gum.DataTypes;
using Gum.Wireframe;
using Gum.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using MonoGameGum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;

namespace DungeonSlime.Scenes
{
    public class GameScene : Scene
    {
        private AnimatedSprite _slime;
        private AnimatedSprite _bat;
        private Vector2 _slimePosition;
        private const float MOVEMENT_SPEED = 5.0f;
        private Vector2 _batPosition;
        private Vector2 _batVelocity;
        private Tilemap _tilemap;
        private Rectangle _roomBounds;
        private SoundEffect _bounceSoundEffect;
        private SoundEffect _collectSoundEffect;
        private SpriteFont _font;
        private int _score;
        private Vector2 _scoreTextPosition;
        private Vector2 _scoreTextOrigin;
        // ui stuff
        private Panel _pausePanel;
        private AnimatedButton _resumeButton;
        private SoundEffect _uiSoundEffect;
        private TextureAtlas _atlas;

        public override void LoadContent()
        {
            // Generate the texture atlas from its XML Configuration File
            this._atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");
            
            // Create the slime animated sprite from the atlas
            this._slime = this._atlas.CreateAnimatedSprite("slime-animation");
            this._slime.Scale = new Vector2(4.0f, 4.0f);

            // Create the bat animated sprite from the atlas
            this._bat = this._atlas.CreateAnimatedSprite("bat-animation");
            this._bat.Scale = new Vector2(4.0f, 4.0f);

            // Create the tilemap from its XML Configuration File
            this._tilemap = Tilemap.FromFile(Content, "images/tilemap-definition.xml");
            this._tilemap.Scale = new Vector2(4.0f, 4.0f);

            // Load sound effects
            this._bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");
            this._collectSoundEffect = Content.Load<SoundEffect>("audio/collect");
            this._uiSoundEffect = Core.Content.Load<SoundEffect>("audio/ui");
            
            // Load the font
            this._font = Core.Content.Load<SpriteFont>("fonts/04B_30");
        }

        private void InitializeUI()
        {
            GumService.Default.Root.Children.Clear();
            CreatePausePanel();
        }

        public override void Initialize()
        {
            base.Initialize();
        
            // On the game screen, the Esc key will switch back to the title screen.
            Core.ExitOnEscape = false;
            Rectangle screenBounds = Core.GraphicsDevice.PresentationParameters.Bounds;


            // This Rectangle is used for bounds checking
            this._roomBounds = new Rectangle(
                (int)this._tilemap.TileWidth,
                (int)this._tilemap.TileHeight,
                screenBounds.Width - (int)this._tilemap.TileWidth * 2,
                screenBounds.Height - (int)this._tilemap.TileHeight * 2
            );
            
            // The slime will start in the middle of the screen
            int centerRow = this._tilemap.Rows / 2;
            int centerColumn = this._tilemap.Columns / 2;

            this._slimePosition = new Vector2(centerColumn * this._tilemap.TileWidth, centerRow * this._tilemap.TileHeight);
            // The bat will start in the top left corner of the screen
            this._batPosition = new Vector2(this._roomBounds.Left, this._roomBounds.Top);

            this._scoreTextPosition = new Vector2(this._roomBounds.Left, this._tilemap.TileHeight * 0.5f);
            float scoreTextYOrigin = this._font.MeasureString("Score").Y * 0.5f;
            this._scoreTextOrigin = new Vector2(0, scoreTextYOrigin);

            AssignRandomBatVelocity();

            this.InitializeUI();
        }

        public override void Update(GameTime gameTime)
        {
            GumService.Default.Update(gameTime);
            if (_pausePanel.IsVisible)
            {
                return;
            }

            this._slime.Update(gameTime);
            this._bat.Update(gameTime);

            CheckKeyboardInput();
            //CheckGamepadInput();

            // I'm not sure what a "normal" is but it's required
            // for vector calculations
            Vector2 normal = Vector2.Zero;

            // Bounding box for the slime
            Circle slimeBounds = new Circle(
                (int)(this._slimePosition.X + (this._slime.Width * 0.5f)),
                (int)(this._slimePosition.Y + (this._slime.Height * 0.5f)),
                (int)(this._slime.Width * 0.5f)
            );

            // bounding box checks to make sure the slime doesn't
            // leave the screen.
            if (slimeBounds.Left < this._roomBounds.Left)
            {
                this._slimePosition.X = this._roomBounds.Left;
            }
            else if (slimeBounds.Right > this._roomBounds.Right)
            {
                this._slimePosition.X = this._roomBounds.Right - this._slime.Width;
            }
            if (slimeBounds.Top < this._roomBounds.Top)
            {
                this._slimePosition.Y = this._roomBounds.Top;
            }
            else if (slimeBounds.Bottom > this._roomBounds.Bottom)
            {
                this._slimePosition.Y = this._roomBounds.Bottom - this._slime.Height;
            }

            // bat posiiton calculator 
            Vector2 newBatPosition = this._batPosition + this._batVelocity;
            // Bounding box for the bat sprite.
            Circle batBounds = new Circle(
                (int)(newBatPosition.X + (this._bat.Width * 0.5f)),
                (int)(newBatPosition.Y + (this._bat.Height * 0.5f)),
                (int)(this._bat.Width * 0.5f)
            );

            // distance-based checks to make sure the bat doesn't leave the screen AND
            // bounces back towards the inside of the arena.
            if (batBounds.Left < this._roomBounds.Left)
            {
                normal.X = Vector2.UnitX.X;
                newBatPosition.X = this._roomBounds.Left;
            }
            else if (batBounds.Right > this._roomBounds.Right)
            {
                normal.X = -Vector2.UnitX.X;
                newBatPosition.X = this._roomBounds.Right - this._bat.Width;
            }

            if (batBounds.Top < this._roomBounds.Top)
            {
                normal.Y = Vector2.UnitY.Y;
                newBatPosition.Y = this._roomBounds.Top;
            }
            else if (batBounds.Bottom > this._roomBounds.Bottom)
            {
                normal.Y = -Vector2.UnitY.Y;
                newBatPosition.Y = this._roomBounds.Bottom - this._bat.Height;
            }

            // Again, not sure what "normal" here is but
            // this bit of code makes sure that the bat reflects around it
            // if it leaves the screen.
            if (normal != Vector2.Zero)
            {
                this._batVelocity = Vector2.Reflect(this._batVelocity, normal);
                Core.Audio.PlaySoundEffect(this._bounceSoundEffect);
            }

            this._batPosition = newBatPosition;

            if (slimeBounds.Intersects(batBounds))
            {
                int column = Random.Shared.Next(1, this._tilemap.Columns - 1);
                int row = Random.Shared.Next(1, this._tilemap.Rows - 1);
                this._batPosition = new Vector2(column * this._bat.Width, row * this._bat.Height);

                AssignRandomBatVelocity();

                Core.Audio.PlaySoundEffect(this._collectSoundEffect);

                this._score += 100;
            }

            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            Core.GraphicsDevice.Clear(Color.CornflowerBlue);
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

            this._tilemap.Draw(Core.SpriteBatch);
            this._slime.Draw(Core.SpriteBatch, this._slimePosition);
            this._bat.Draw(Core.SpriteBatch, this._batPosition);

            Core.SpriteBatch.DrawString(
                this._font,
                $"Score {this._score}",
                this._scoreTextPosition,
                Color.White,
                0.0f,
                this._scoreTextOrigin,
                1.0f,
                SpriteEffects.None,
                0.0f
            );

            Core.SpriteBatch.End();
            GumService.Default.Draw();
        }

        private void CreatePausePanel()
        {
            this._pausePanel = new Panel();
            this._pausePanel.Anchor(Anchor.Center);
            this._pausePanel.Visual.WidthUnits = DimensionUnitType.Absolute;
            this._pausePanel.Visual.HeightUnits = DimensionUnitType.Absolute;
            this._pausePanel.Visual.Height = 70;
            this._pausePanel.Visual.Width = 264;
            this._pausePanel.IsVisible = false;
            this._pausePanel.AddToRoot();

            TextureRegion backgroundRegion = this._atlas.GetRegion("panel-background");
            NineSliceRuntime background = new NineSliceRuntime();
            background.Dock(Dock.Fill);
            background.Texture = backgroundRegion.Texture;
            background.TextureAddress = TextureAddress.Custom;
            background.TextureHeight = backgroundRegion.Height;
            background.TextureLeft = backgroundRegion.SourceRectangle.Left;
            background.TextureTop = backgroundRegion.SourceRectangle.Top;
            background.TextureWidth = backgroundRegion.Width;
            this._pausePanel.AddChild(background);

            var textInstance = new TextRuntime();
            textInstance.Text = "PAUSED";
            textInstance.CustomFontFile = @"fonts/04b_30.fnt";
            textInstance.UseCustomFont = true;
            textInstance.FontScale = 0.5f;
            textInstance.X = 10f;
            textInstance.Y = 10f;
            this._pausePanel.AddChild(textInstance);

            this._resumeButton = new AnimatedButton(this._atlas);
            this._resumeButton.Text = "RESUME";
            this._resumeButton.Anchor(Anchor.BottomLeft);
            this._resumeButton.Visual.X = 9f;
            this._resumeButton.Visual.Y = -9f;
            this._resumeButton.Visual.Width = 80;
            this._resumeButton.Click += this.HandleResumeButtonClicked;
            this._pausePanel.AddChild(this._resumeButton);

            var quitButton = new AnimatedButton(this._atlas);
            quitButton.Text = "QUIT";
            quitButton.Anchor(Anchor.BottomRight);
            quitButton.Visual.X = -9f;
            quitButton.Visual.Y = -9f;
            quitButton.Width = 80;
            quitButton.Click += this.HandleQuitButtonClicked;
            this._pausePanel.AddChild(quitButton);
        }

        private void HandleResumeButtonClicked(object sender, EventArgs e)
        {
            Core.Audio.PlaySoundEffect(this._uiSoundEffect);
            this._pausePanel.IsVisible = false;
        }
        
        private void HandleQuitButtonClicked(object sender, EventArgs e)
        {
            Core.Audio.PlaySoundEffect(this._uiSoundEffect);
            Core.ChangeScene(new TitleScene());
        }

        private void PauseGame()
        {
            this._pausePanel.IsVisible = true;
            this._resumeButton.IsFocused = true;
        }

        private void AssignRandomBatVelocity()
        {
            float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);

            Vector2 direction = new Vector2(x, y);

            this._batVelocity = direction * MOVEMENT_SPEED;
        }

        private void CheckKeyboardInput()
        {
            KeyboardInfo keyboard = Core.Input.Keyboard;

            if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
            {
                //Core.ChangeScene(new TitleScene());
                this.PauseGame();
            }
            
            float speed = MOVEMENT_SPEED;
            if (keyboard.IsKeyDown(Keys.Space))
            {
                speed *= 1.5f;
            }

            if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
            {
                this._slimePosition.Y -= speed;
            }

            if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
            {
                this._slimePosition.Y += speed;
            }

            if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
            {
                this._slimePosition.X -= speed;
            }

            if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
            {
                this._slimePosition.X += speed;
            }

            if (keyboard.WasKeyJustPressed(Keys.M))
            {
                Core.Audio.ToggleMute();
            }

            if (keyboard.WasKeyJustPressed(Keys.OemPlus))
            {
                Core.Audio.SongVolume += 0.1f;
                Core.Audio.SoundEffectVolume += 0.1f;
            }

            if (keyboard.WasKeyJustPressed(Keys.OemMinus))
            {
                Core.Audio.SongVolume -= 0.1f;
                Core.Audio.SoundEffectVolume -= 0.1f;
            }
        }
    }
}
