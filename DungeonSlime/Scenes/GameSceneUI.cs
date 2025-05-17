using System;
using DungeonSlime.UI;
using Gum.DataTypes;
using Gum.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using MonoGameGum;
using MonoGameGum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DungeonSlime.Scenes
{
    public class GameSceneUI : ContainerRuntime
    {
        // The string format to use when updating the text for the score
        // display
        private static readonly string s_scoreFormat = "Score: {0:D6}";

        // The sound effect to play for auditory feedback fo the user interface;
        private SoundEffect _uiSoundEffect;

        // The pause panel.
        private Panel _pausePanel;

        // The resume button on the pause panel. Field is used to track reference so
        // focus can be set when the pause panel is shown.
        private AnimatedButton _resumeButton;

        // The retry button on the game over panel. this field is used to track
        // the reference so focus can be set when the game over panel is shown.
        private AnimatedButton _retryButton;

        // The game over panel;
        private Panel _gameOverPanel;

        // The text runtime used to display the players score on the game screen.
        private TextRuntime _scoreText;

        /// <summary>
        /// Event invoked when the Resume button on the Pause panel is clicked.
        /// </summary>
        public event EventHandler ResumeButtonClick;

        /// <summary>
        /// Event invoked when the Quit button on either the Pause panel or the
        /// Game Over panel is clicked.
        /// </summary>
        public event EventHandler QuitButtonClick;

        /// <summary>
        /// Event invoked when the Retry button on the Game Over panel is clicked.
        /// </summary>
        public event EventHandler RetryButtonClick;

        public GameSceneUI()
        {
            // We want the dock to fill the entire screen, so we set it here.
            this.Dock(Gum.Wireframe.Dock.Fill);
            // Then we attach it to the root element.
            this.AddToRoot();
            
            // Gets a reference to the content manager that was registered with the
            // GumService wehn it was originally initialized.
            ContentManager content = GumService.Default.ContentLoader.XnaContentManager;

            // Use the content manager to load the sound effect and atlas for the user
            // interface elements;
            this._uiSoundEffect = content.Load<SoundEffect>("audio/ui");
            TextureAtlas atlas = TextureAtlas.FromFile(content, "images/atlas-definition.xml");

            // Create the text that will display the players' score and add it
            // as a child to the container.
            this._scoreText = this.CreateScoreText();
            this.AddChild(this._scoreText);

            // Create the pause panel and add it as a child to the container.
            this._pausePanel = this.CreatePausePanel(atlas);
            this.AddChild(this._pausePanel.Visual);

            this._gameOverPanel = this.CreateGameOverPanel(atlas);
            this.AddChild(this._gameOverPanel.Visual);
        }

        private TextRuntime CreateScoreText()
        {
            TextRuntime text = new TextRuntime();
            text.Anchor(Gum.Wireframe.Anchor.TopLeft);
            text.WidthUnits = DimensionUnitType.RelativeToChildren;
            text.X = 20.0f;
            text.Y = 5.0f;
            text.UseCustomFont = true;
            text.CustomFontFile = @"fonts/04b_30.fnt";
            text.FontScale = 0.25f;
            text.Text = string.Format(s_scoreFormat, 0);

            return text;
        }

        private Panel CreatePausePanel(TextureAtlas atlas)
        {
            Panel panel = new Panel();
            panel.Anchor(Gum.Wireframe.Anchor.Center);
            panel.Visual.WidthUnits = DimensionUnitType.Absolute;
            panel.Visual.HeightUnits = DimensionUnitType.Absolute;
            panel.Visual.Width = 264.0f;
            panel.Visual.Height = 70.0f;
            panel.IsVisible = false;

            TextureRegion backgroundRegion = atlas.GetRegion("panel-background");

            NineSliceRuntime background = new NineSliceRuntime();
            background.Dock(Gum.Wireframe.Dock.Fill);
            background.Texture = backgroundRegion.Texture;
            background.TextureAddress = TextureAddress.Custom;
            background.TextureHeight = backgroundRegion.Height;
            background.TextureWidth = backgroundRegion.Width;
            background.TextureTop = backgroundRegion.SourceRectangle.Top;
            background.TextureLeft = backgroundRegion.SourceRectangle.Left;
            panel.AddChild(background);

            TextRuntime text = new TextRuntime();
            text.Text = "PAUSED";
            text.UseCustomFont = true;
            text.CustomFontFile = "fonts/04b_30.fnt";
            text.FontScale = 0.5f;
            text.X = 10.0f;
            text.Y = 10.0f;
            panel.AddChild(text);

            this._resumeButton = new AnimatedButton(atlas);
            this._resumeButton.Text = "RESUME";
            this._resumeButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
            this._resumeButton.Visual.X = 9.0f;
            this._resumeButton.Visual.Y = -9.0f;

            _resumeButton.Click += this.OnResumeButtonClicked;
            _resumeButton.GotFocus += this.OnElementGotFocus;

            panel.AddChild(_resumeButton);

            AnimatedButton quitButton = new AnimatedButton(atlas);
            quitButton.Text = "QUIT";
            quitButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
            quitButton.Visual.X = -9.0f;
            quitButton.Visual.Y = -9.0f;

            quitButton.Click += this.OnQuitButtonClicked;
            quitButton.GotFocus += this.OnElementGotFocus;

            panel.AddChild(quitButton);

            return panel;
        }

        private Panel CreateGameOverPanel(TextureAtlas atlas)
        {
            Panel panel = new Panel();
            panel.Anchor(Gum.Wireframe.Anchor.Center);
            panel.Visual.WidthUnits = DimensionUnitType.Absolute;
            panel.Visual.HeightUnits = DimensionUnitType.Absolute;
            panel.Visual.Width = 264.0f;
            panel.Visual.Height = 70.0f;
            panel.IsVisible = false;

            TextureRegion backgroundRegion = atlas.GetRegion("panel-background");

            NineSliceRuntime background = new NineSliceRuntime();
            background.Dock(Gum.Wireframe.Dock.Fill);
            background.Texture = backgroundRegion.Texture;
            background.TextureAddress = TextureAddress.Custom;
            background.TextureHeight = backgroundRegion.Height;
            background.TextureWidth = backgroundRegion.Width;
            background.TextureTop = backgroundRegion.SourceRectangle.Top;
            background.TextureLeft = backgroundRegion.SourceRectangle.Left;
            panel.AddChild(background);

            TextRuntime text = new TextRuntime();
            text.Text = "GAME OVER";
            text.WidthUnits = DimensionUnitType.RelativeToChildren;
            text.UseCustomFont = true;
            text.CustomFontFile = "fonts/04b_30.fnt";
            text.FontScale = 0.5f;
            text.X = 10.0f;
            text.Y = 10.0f;
            panel.AddChild(text);

            _retryButton = new AnimatedButton(atlas);
            _retryButton.Text = "RETRY";
            _retryButton.Anchor(Gum.Wireframe.Anchor.BottomLeft);
            _retryButton.Visual.X = 9.0f;
            _retryButton.Visual.Y = -9.0f;

            _retryButton.Click += this.OnRetryButtonClicked;
            _retryButton.GotFocus += this.OnElementGotFocus;

            panel.AddChild(_retryButton);

            AnimatedButton quitButton = new AnimatedButton(atlas);
            quitButton.Text = "QUIT";
            quitButton.Anchor(Gum.Wireframe.Anchor.BottomRight);
            quitButton.Visual.X = -9.0f;
            quitButton.Visual.Y = -9.0f;

            quitButton.Click += this.OnQuitButtonClicked;
            quitButton.GotFocus += this.OnElementGotFocus;

            panel.AddChild(quitButton);

            return panel;
        }

        private void OnResumeButtonClicked(object sender, EventArgs args)
        {
            // Button was clicked, play the UI sound effect for auditory feedback
            Core.Audio.PlaySoundEffect(this._uiSoundEffect);

            // Since the resume button was clicked, we hide the pause panel.
            this.HidePausePanel();
            
            // Then we send the ResumeButtonClick event.
            if (ResumeButtonClick != null)
            {
                ResumeButtonClick(sender, args);
            }
        }

        private void OnRetryButtonClicked(object sender, EventArgs args)
        {
            // Button was clicked, play the ui sound effect for auditory feedback
            Core.Audio.PlaySoundEffect(this._uiSoundEffect);
            
            // Hide the retry panel 
            this.HideGameOverPanel();
            
            // Send the RetryButtonClick.
            if (RetryButtonClick != null)
            {
                RetryButtonClick(sender, args);
            }
        }

        private void OnQuitButtonClicked(object sender, EventArgs args)
        {
            // Button was clicked, play the ui sound effect for auditory feedback.
            Core.Audio.PlaySoundEffect(this._uiSoundEffect);
            
            // Both panels have a quit button so they both
            // get hidden.
            this.HidePausePanel();
            this.HideGameOverPanel();
            
            // Invoke the QuitButtonClick event.
            if (QuitButtonClick != null)
            {
                QuitButtonClick(sender, args);
            }
        }

        private void OnElementGotFocus(object sender, EventArgs args)
        {
            // When a UI element receives focus, the UI sound effect
            // will play to enhance auditory feedback.
            Core.Audio.PlaySoundEffect(this._uiSoundEffect);
        }

        /// <summary>
        /// Updates the text on the score display
        /// </summary>
        /// <param name="score">The score to display.</param>
        public void UpdateScoreText(int score)
        {
            this._scoreText.Text = string.Format(s_scoreFormat, score);
        }

        /// <summary>
        /// Tells the game scene ui to show the pause panel.
        /// </summary>
        public void ShowPausePanel()
        {
            this._pausePanel.IsVisible = true;
            this._resumeButton.IsFocused = true;
            this._gameOverPanel.IsVisible = false;
        }

        /// <summary>
        /// Tells the game scene ui to hide the pause panel.
        /// </summary>
        public void HidePausePanel()
        {
            this._pausePanel.IsVisible = false;
        }

        /// <summary>
        /// Tells the game scene ui to show the game over panel.
        /// </summary>
        public void ShowGameOverPanel()
        {
            this._gameOverPanel.IsVisible = true;
            this._retryButton.IsFocused = true;
            this._pausePanel.IsVisible = false;
        }

        /// <summary>
        /// Tells the game scene ui to hide the game over panel.
        /// </summary>
        public void HideGameOverPanel()
        {
            this._gameOverPanel.IsVisible = false;
        }
       
        /// <summary>
        /// Updates the game scene ui.
        /// </summary>
        /// <param name="gameTime">A snapshot of timing values for the current update cycle.</param>
        public void Update(GameTime gameTime)
        {
            GumService.Default.Update(gameTime);
        }

        /// <summary>
        /// Draws the game scene UI.
        /// </summary>
        public void Draw()
        {
            GumService.Default.Draw();
        }
    }
}
