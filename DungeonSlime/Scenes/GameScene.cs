using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGameGum;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using MonoGameLibrary.Scenes;
using DungeonSlime.GameObjects;

namespace DungeonSlime.Scenes;

public class GameScene: Scene 
{ 
    private enum GameState
    {
        Playing,
        Paused,
        GameOver
    }

    // Reference to the slime;
    private Slime _slime;

    // Reference to the bat.
    private Bat _bat;

    // Defines the tilemap to draw.
    private Tilemap _tilemap;

    // Defines the bounds of the room.
    private Rectangle _roomBounds;

    // Plays when the slime eats a bat
    private SoundEffect _collectSoundEffect;

    // Tracks player score
    private int _score;

    private GameSceneUI _ui;
    private GameState _state;

    private Effect _grayscaleEffect;
    private float _saturation = 1.0f;
    private const float FADE_SPEED = 0.02f;

    public override void Initialize()
    {
        base.Initialize();

        // disable exit on escape, it will instead
        // be used to return to the title screen.
        Core.ExitOnEscape = false;

        this._roomBounds = Core.GraphicsDevice.PresentationParameters.Bounds;
        this._roomBounds.Inflate(-this._tilemap.TileWidth, -this._tilemap.TileHeight);

        this._slime.BodyCollision += OnSlimeBodyCollision;

        GumService.Default.Root.Children.Clear();

        this.InitializeUI();
        this.InitializeNewGame();
    }

    private void InitializeUI()
    {
        GumService.Default.Root.Children.Clear();
        this._ui = new GameSceneUI();
        this._ui.ResumeButtonClick += this.OnResumeButtonClicked;
        this._ui.RetryButtonClick += this.OnRetryButtonClicked;
        this._ui.QuitButtonClick += this.OnQuitButtonClicked;
    }

    public override void LoadContent()
    {
        // create the texture atlas from its XML configuration file.
        TextureAtlas atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");

        // Create the tilemap from the XML configuration file.
        this._tilemap = Tilemap.FromFile(Content, "images/tilemap-definition.xml");
        this._tilemap.Scale = new Vector2(4.0f, 4.0f);

        // Create the animated sprite for the slime from the atlas
        AnimatedSprite slimeAnimation = atlas.CreateAnimatedSprite("slime-animation");
        slimeAnimation.Scale = new Vector2(4.0f, 4.0f);

        // Create the slime
        this._slime = new Slime(slimeAnimation);

        // Create the animated sprite for the bat from the atlas
        AnimatedSprite batAnimation = atlas.CreateAnimatedSprite("bat-animation");
        batAnimation.Scale = new Vector2(4.0f, 4.0f);

        // Load the bounce sound effect for the bat.
        SoundEffect bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");

        // Create the bat.
        this._bat = new Bat(batAnimation, bounceSoundEffect);

        // Load the collect sound effect.
        this._collectSoundEffect = Content.Load<SoundEffect>("audio/collect");

        // Load the grayscale effect
        this._grayscaleEffect = Content.Load<Effect>("effects/grayscaleEffect");
    }

    public override void Update(GameTime gameTime)
    {
        this._ui.Update(gameTime);

        if (this._state != GameState.Playing)
        {
            // When paused or in the game over state, we gradually
            // decreatse the saturation to create the fading grayscale
            this._saturation = Math.Max(0.0f, this._saturation - FADE_SPEED);
            if (this._state == GameState.GameOver)
            {
                return;
            }
        }

        if (GameController.Pause())
        {
            TogglePause();
        }

        if (this._state == GameState.Paused)
        {
            return;
        }

        this._slime.Update(gameTime);
        this._bat.Update(gameTime);

        this.CollisionChecks();
    }

    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);

        if (this._state != GameState.Playing)
        {
            this._grayscaleEffect.Parameters["Saturation"].SetValue(this._saturation);
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: this._grayscaleEffect);
        } else
        {
            Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        }

        this._tilemap.Draw(Core.SpriteBatch);
        this._slime.Draw();
        this._bat.Draw();
        Core.SpriteBatch.End();

        this._ui.Draw();
    }

    private void CollisionChecks()
    {
        Circle slimeBounds = this._slime.GetBounds();
        Circle batBounds = this._bat.GetBounds();

        if (slimeBounds.Intersects(batBounds))
        {
            // Move the bat to a new position away from the slime.
            this.PositionBatAwayFromSlime();

            // Randomize the veloicty of the bat.
            this._bat.RandomizeVelocity();

            // tell the slime to grow.
            this._slime.Grow();

            // Update the score.
            this._score += 100;

            // Update the score display.
            this._ui.UpdateScoreText(_score);

            // play the collect sound effect.
            Core.Audio.PlaySoundEffect(this._collectSoundEffect);
        }

        if (slimeBounds.Top < this._roomBounds.Top || 
            slimeBounds.Bottom > this._roomBounds.Bottom ||
            slimeBounds.Left < this._roomBounds.Left ||
            slimeBounds.Right > this._roomBounds.Right)
        {
            this.GameOver();
            return;
        }

        if (batBounds.Top < this._roomBounds.Top)
        {
            this._bat.Bounce(Vector2.UnitY);
        }
        else if (batBounds.Bottom > this._roomBounds.Bottom)
        {
            this._bat.Bounce(-Vector2.UnitY);
        }

        if (batBounds.Left < this._roomBounds.Left)
        {
            this._bat.Bounce(Vector2.UnitX);
        }
        else if (batBounds.Right > this._roomBounds.Right)
        {
            this._bat.Bounce(-Vector2.UnitX);
        }
    }
    private void InitializeNewGame()
    {
        // the slime starts in the center of the arena
        Vector2 slimePos = new Vector2();
        slimePos.X = (this._tilemap.Columns / 2) * this._tilemap.TileWidth;
        slimePos.Y = (this._tilemap.Rows / 2) * this._tilemap.TileHeight;

        // slime init
        this._slime.Initialize(slimePos, this._tilemap.TileWidth);

        // init the bat
        this._bat.RandomizeVelocity();
        this.PositionBatAwayFromSlime();

        this._score = 0;
        this._state = GameState.Playing;
    }

    private void PositionBatAwayFromSlime()
    {
        // Calculate the position that is in the center of
        // the bounds of the room
        float roomCenterX = this._roomBounds.X + this._roomBounds.Width * 0.5f;
        float roomCenterY = this._roomBounds.Y + this._roomBounds.Height * 0.5f;
        Vector2 roomCenter = new Vector2(roomCenterX, roomCenterY);

        // Get the bounds of the slime and calculate the center
        // position
        Circle slimeBounds = this._slime.GetBounds();
        Vector2 slimeCenter = new Vector2(slimeBounds.X, slimeBounds.Y);

        // Calculate the distance vector from the center of
        // the room to the center of the slime.
        Vector2 centerToSlime = slimeCenter - roomCenter;

        // Get the bounds of the bat
        Circle batBounds = this._bat.GetBounds();

        // Calculate the amount of padding we add to the
        // new position to ensure that it isn't sticking
        // to walls.
        int padding = batBounds.Radius * 2;

        Vector2 newBatPosition = Vector2.Zero;
        if (Math.Abs(centerToSlime.X) > Math.Abs(centerToSlime.Y))
        {
            newBatPosition.Y = Random.Shared.Next(
                this._roomBounds.Top + padding,
                this._roomBounds.Bottom - padding
            );

            if (centerToSlime.X > 0)
            {
                // The slime is closer to the right side wall, so
                // place the bat on the left side wall
                newBatPosition.X = this._roomBounds.Left + padding;
            }
            else
            {
                // The slime is closer to the left side wall, so place
                // the bat on the right side wall.
                newBatPosition.X = this._roomBounds.Right + padding * 2;
            }
        }
        else
        {
            // The slime is closer to either the top or bottom wall, so the X
            // position will be a random position between the left and right
            // walls.
            newBatPosition.X = Random.Shared.Next(
                this._roomBounds.Left + padding,
                this._roomBounds.Right - padding
            );

            if (centerToSlime.Y > 0)
            {
                // If the slime is closer to the top wall, place
                // the bat on the bottom wall
                newBatPosition.Y = _roomBounds.Top + padding;
            } 
            else
            {
                // If the slime is closer to the bottom wall,
                // place the bat on the top wall.
                newBatPosition.Y = _roomBounds.Bottom - padding * 2;
            }
        }
        this._bat.Position = newBatPosition;
    }

    private void OnSlimeBodyCollision(object sender, EventArgs args)
    {
        this.GameOver();
    }

    private void TogglePause()
    {
        if (this._state == GameState.Paused)
        {
            // We're now unpausing the game, so hide the pause panel.
            this._ui.HidePausePanel();

            // And set the state back to playing
            this._state = GameState.Playing;
        }
        else
        {
            // We are pausing the game, so show the pause panel
            this._ui.ShowPausePanel();

            // And set the state to paused.
            this._state = GameState.Paused;

            this._saturation = 1.0f;
        }
    }

    private void GameOver()
    {
        this._ui.ShowGameOverPanel();
        this._state = GameState.GameOver;
        this._saturation = 1.0f;
    }

    private void OnResumeButtonClicked(object sender, EventArgs args)
    {
        this._state = GameState.Playing;
    }

    private void OnRetryButtonClicked(object sender, EventArgs args)
    {
        InitializeNewGame();
    }

    private void OnQuitButtonClicked(object sender, EventArgs args)
    {
        Core.ChangeScene(new TitleScene());
    }
}
