using System;
using Microsoft.Xna.Framework;
using MonoGameGum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary.Graphics;
using Gum.DataTypes;
using Gum.DataTypes.Variables;
using Gum.Managers;
using System.Security.AccessControl;

namespace DungeonSlime.UI
{
    internal class OptionsSlider : Slider
    {
        private TextRuntime _textInstance;
        private ColoredRectangleRuntime _fillRectangle;

        public string Text
        {
            get => this._textInstance.Text;
            set => this._textInstance.Text = value;
        }

        public OptionsSlider(TextureAtlas atlas)
        {
            // Create the top-level container for all visual elements
            ContainerRuntime topLevelContainer = new ContainerRuntime();
            topLevelContainer.Height = 55f;
            topLevelContainer.Width = 264f;
            TextureRegion backgroundRegion = atlas.GetRegion("panel-background");

            // Create the background panel that contains everything
            NineSliceRuntime background = new NineSliceRuntime();
            background.Texture = atlas.Texture;
            background.TextureAddress = TextureAddress.Custom;
            background.TextureHeight = backgroundRegion.Height;
            background.TextureLeft = backgroundRegion.SourceRectangle.Left;
            background.TextureTop = backgroundRegion.SourceRectangle.Top;
            background.TextureWidth = backgroundRegion.Width;
            background.Dock(Gum.Wireframe.Dock.Fill);
            topLevelContainer.AddChild(background);

            this._textInstance = new TextRuntime();
            this._textInstance.CustomFontFile = @"fonts/04b_30.fnt";
            this._textInstance.UseCustomFont = true;
            this._textInstance.FontScale = 0.5f;
            this._textInstance.Text = "Replace Me";
            this._textInstance.X = 10f;
            this._textInstance.Y = 10f;
            this._textInstance.WidthUnits = DimensionUnitType.RelativeToChildren;
            topLevelContainer.AddChild(this._textInstance);

            // Create container for slider track and decorative elements;
            ContainerRuntime innerContainer = new ContainerRuntime();
            innerContainer.Height = 13f;
            innerContainer.Width = 241f;
            innerContainer.X = 10f;
            innerContainer.Y = 33f;
            topLevelContainer.AddChild(innerContainer);

            // Create the "off" side of the slider (left end)
            TextureRegion offBackgroundRegion = atlas.GetRegion("slider-off-background");
            NineSliceRuntime offBackground = new NineSliceRuntime();
            offBackground.Dock(Gum.Wireframe.Dock.Left);
            offBackground.Texture = atlas.Texture;
            offBackground.TextureAddress = TextureAddress.Custom;
            offBackground.TextureHeight = offBackgroundRegion.Height;
            offBackground.TextureLeft = offBackgroundRegion.SourceRectangle.Left;
            offBackground.TextureTop = offBackgroundRegion.SourceRectangle.Top;
            offBackground.Width = 28f;
            offBackground.WidthUnits = DimensionUnitType.Absolute;
            offBackground.Dock(Gum.Wireframe.Dock.Left);
            innerContainer.AddChild(offBackground);

            // Create the "middle" side of the slider
            TextureRegion middleBackgroundRegion = atlas.GetRegion("slider-middle-background");
            NineSliceRuntime middleBackground = new NineSliceRuntime();
            middleBackground.Dock(Gum.Wireframe.Dock.FillVertically);
            middleBackground.Texture = middleBackgroundRegion.Texture;
            middleBackground.TextureAddress = TextureAddress.Custom;
            middleBackground.TextureHeight = middleBackgroundRegion.Height;
            middleBackground.TextureLeft = middleBackgroundRegion.SourceRectangle.Left;
            middleBackground.TextureTop = middleBackgroundRegion.SourceRectangle.Top;
            middleBackground.TextureWidth = middleBackgroundRegion.Width;
            middleBackground.Width = 179f;
            middleBackground.WidthUnits = DimensionUnitType.Absolute;
            middleBackground.Dock(Gum.Wireframe.Dock.Left);
            middleBackground.X = 27f;
            innerContainer.AddChild(middleBackground);

            // create the "max" portion of the slider;
            TextureRegion maxBackgroundRegion = atlas.GetRegion("slider-max-background");
            NineSliceRuntime maxBackground = new NineSliceRuntime();
            maxBackground.Texture = maxBackgroundRegion.Texture;
            maxBackground.TextureAddress = TextureAddress.Custom;
            maxBackground.TextureHeight = maxBackgroundRegion.Height;
            maxBackground.TextureLeft = maxBackgroundRegion.SourceRectangle.Left;
            maxBackground.TextureTop = maxBackgroundRegion.SourceRectangle.Top;
            maxBackground.TextureWidth = maxBackgroundRegion.Width;
            maxBackground.Width = 36f;

            maxBackground.WidthUnits = DimensionUnitType.Absolute;
            maxBackground.Dock(Gum.Wireframe.Dock.Right);
            innerContainer.AddChild(maxBackground);

            // create the interactive track that will respond to clicks
            ContainerRuntime trackInstance = new ContainerRuntime();
            trackInstance.Name = "TrackInstance";
            trackInstance.Dock(Gum.Wireframe.Dock.Fill);
            trackInstance.Height = -2f;
            trackInstance.Width = -2f;
            middleBackground.AddChild(trackInstance);

            // create the fill rectangle that visually displays the current value
            this._fillRectangle = new ColoredRectangleRuntime();
            this._fillRectangle.Dock(Gum.Wireframe.Dock.Left);
            this._fillRectangle.Width = 90f;
            this._fillRectangle.WidthUnits = DimensionUnitType.PercentageOfParent;
            trackInstance.AddChild(this._fillRectangle);

            // Add "off" text to the left end
            TextRuntime offText = new TextRuntime();
            offText.Red = 70;
            offText.Green = 86;
            offText.Blue = 130;
            offText.CustomFontFile = @"fonts/04b_30.fnt";
            offText.FontScale = 0.25f;
            offText.UseCustomFont = true;
            offText.Text = "OFF";
            offText.Anchor(Gum.Wireframe.Anchor.Center);
            offBackground.AddChild(offText);

            // add "max" text to the right end;
            TextRuntime maxText = new TextRuntime();
            maxText.Red = 70;
            maxText.Green = 86;
            maxText.Blue = 130;
            maxText.CustomFontFile = @"fonts/04b_30.fnt";
            maxText.FontScale = 0.25f;
            maxText.UseCustomFont = true;
            maxText.Text = "MAX";
            maxText.Anchor(Gum.Wireframe.Anchor.Center);
            maxBackground.AddChild(maxText);

            // define colors for focused and unfocused states
            Color focusedColor = Color.White;
            Color unfocusedColor = Color.Gray;

            // Create slider state category - Slider.SliderCategoryName is required.
            StateSaveCategory sliderCategory = new StateSaveCategory();
            sliderCategory.Name = Slider.SliderCategoryName;
            topLevelContainer.AddCategory(sliderCategory);

            // Create the enabled (default/unfocused) state
            StateSave enabled = new StateSave();
            enabled.Name = FrameworkElement.EnabledStateName;
            enabled.Apply = () =>
            {
                background.Color = unfocusedColor;
                this._textInstance.Color = unfocusedColor;
                offBackground.Color = unfocusedColor;
                middleBackground.Color = unfocusedColor;
                maxBackground.Color = unfocusedColor;
                this._fillRectangle.Color = unfocusedColor;
            };

            sliderCategory.States.Add(enabled);

            // Create the focused state;
            StateSave focused = new StateSave();
            focused.Name = FrameworkElement.FocusedStateName;
            focused.Apply = () =>
            {
                background.Color = focusedColor;
                this._textInstance.Color = focusedColor;
                offBackground.Color = focusedColor;
                middleBackground.Color = focusedColor;
                maxBackground.Color = focusedColor;
                this._fillRectangle.Color = focusedColor;
            };
            sliderCategory.States.Add(focused);

            // Create the highlighted + focused state by cloning the focused state;
            StateSave highlightedFocused = focused.Clone();
            highlightedFocused.Name = FrameworkElement.HighlightedFocusedStateName;
            sliderCategory.States.Add(highlightedFocused);

            // create the highlighted state by cloning the enabled state.
            StateSave highlighted = enabled.Clone();
            highlighted.Name = FrameworkElement.HighlightedStateName;
            sliderCategory.States.Add(highlighted);

            // Assign the configured container as this slider's visual;
            this.Visual = topLevelContainer;

            // Enable click-to-point functionality for the slider
            // This allows users to click anywhere ont he track to jump to that value;
            this.IsMoveToPointEnabled = true;

            // Add event handlers.
            this.Visual.RollOn += this.HandleRollOn;
            this.ValueChanged += this.HandleValueChanged;
            this.ValueChangedByUi += this.HandleValueChangedByUi;
        }

        /// <summary>
        /// Automatically focuses the slider when the user interacts with it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleValueChangedByUi(object sender, EventArgs e)
        {
            this.IsFocused = true;
        }

        /// <summary>
        /// Automatically focuses the slider when the mouse hovers over it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleRollOn(object sender, EventArgs e)
        {
            this.IsFocused = true;
        }
        
        /// <summary>
        /// Updates the fill rectangle width to visually represent the current value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleValueChanged(object sender, EventArgs e)
        {
            double ratio = (this.Value - this.Minimum) / (this.Maximum - this.Minimum);
            this._fillRectangle.Width = 100 * (float)ratio;
        }
    }
}
