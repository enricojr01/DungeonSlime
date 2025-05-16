using System;
using Gum.DataTypes;
using Gum.DataTypes.Variables;
using Gum.Graphics.Animation;
using Gum.Managers;
using Microsoft.Xna.Framework.Input;
using MonoGameGum.Forms;
using MonoGameGum.Forms.Controls;
using MonoGameGum.GueDeriving;
using MonoGameLibrary.Graphics;

namespace DungeonSlime.UI
{
    internal class AnimatedButton : Button
    {
        public AnimatedButton(TextureAtlas atlas) 
        {
            // Creates a top-level container to hold all visual elements.
            // Width is relative tochildren + extra padding, while height
            // is fixed.
            ContainerRuntime topLevelContainer = new ContainerRuntime();
            topLevelContainer.Height = 14f;
            topLevelContainer.HeightUnits = DimensionUnitType.Absolute;
            topLevelContainer.Width = 21f;
            topLevelContainer.WidthUnits = DimensionUnitType.RelativeToChildren;

            // Create the nine-slice background that will display button
            // graphics. A nine-slice allows the button to stretch while preserving
            // corner appearance.
            NineSliceRuntime nineSliceInstance = new NineSliceRuntime();
            nineSliceInstance.Height = 0f;
            nineSliceInstance.Texture = atlas.Texture;
            nineSliceInstance.TextureAddress = TextureAddress.Custom;
            nineSliceInstance.Dock(Gum.Wireframe.Dock.Fill);
            topLevelContainer.Children.Add(nineSliceInstance);

            // Create the text element that displays the button's label
            TextRuntime textInstance = new TextRuntime();
            textInstance.Name = "TextInstance";
            textInstance.Text = "START";
            textInstance.Blue = 130;
            textInstance.Green = 86;
            textInstance.Red = 70;
            textInstance.UseCustomFont = true;
            textInstance.CustomFontFile = "fonts/04b_30.fnt";
            textInstance.FontScale = 0.25f;
            textInstance.Anchor(Gum.Wireframe.Anchor.Center);
            textInstance.Width = 0;
            textInstance.WidthUnits = DimensionUnitType.RelativeToChildren;
            topLevelContainer.Children.Add(textInstance);

            // Create animation chain for the unfocused state using all the frames
            // from the unfocused-button animation (there's only one)
            TextureRegion unfocusedTextureRegion = atlas.GetRegion("unfocused-button");
            AnimationChain unfocusedAnimation = new AnimationChain();
            unfocusedAnimation.Name = nameof(unfocusedAnimation);
            AnimationFrame unfocusedFrame = new AnimationFrame
            {
                TopCoordinate = unfocusedTextureRegion.TopTextureCoordinate,
                BottomCoordinate = unfocusedTextureRegion.BottomTextureCoordinate,
                LeftCoordinate = unfocusedTextureRegion.LeftTextureCoordinate,
                RightCoordinate = unfocusedTextureRegion.RightTextureCoordinate,
                FrameLength = 0.3f,
                Texture = unfocusedTextureRegion.Texture,
            };
            unfocusedAnimation.Add(unfocusedFrame);

            // Create an animation chain for the focused state using all frames
            // from the focused-button-animation atlas animation.
            Animation focusedAtlasAnimation = atlas.GetAnimation("focused-button-animation");
            AnimationChain focusedAnimation = new AnimationChain();
            focusedAnimation.Name = nameof(focusedAnimation);
            foreach ( TextureRegion region in focusedAtlasAnimation.Frames)
            {
                AnimationFrame frame = new AnimationFrame
                {
                    TopCoordinate = region.TopTextureCoordinate,
                    BottomCoordinate = region.BottomTextureCoordinate,
                    LeftCoordinate = region.LeftTextureCoordinate,
                    RightCoordinate = region.RightTextureCoordinate,
                    FrameLength = (float)focusedAtlasAnimation.Delay.TotalSeconds,
                    Texture = region.Texture
                };

                focusedAnimation.Add(frame);
            }

            // assign both animation chains to the nine-slice
            nineSliceInstance.AnimationChains = new AnimationChainList
            {
                unfocusedAnimation,
                focusedAnimation,
            };

            // need a state category for the button states
            StateSaveCategory category = new StateSaveCategory();
            category.Name = Button.ButtonCategoryName;
            topLevelContainer.AddCategory(category);

            // Create the enabled (default/unfocused) state;
            StateSave enabledState = new StateSave();
            enabledState.Name = FrameworkElement.EnabledStateName;
            enabledState.Apply = () =>
            {
                nineSliceInstance.CurrentChainName = unfocusedAnimation.Name;
            };
            category.States.Add(enabledState);

            // Create focused state
            StateSave focusedState = new StateSave();
            focusedState.Name = FrameworkElement.FocusedStateName;
            focusedState.Apply = () =>
            {
                // When focused, use the focused animation and enable animation playback
                nineSliceInstance.CurrentChainName = focusedAnimation.Name;
                nineSliceInstance.Animate = true;
            };
            category.States.Add(focusedState);

            // Create the highlighted + focused state by cloning the focused
            // state since they appear the same.
            StateSave highlightedFocused = focusedState.Clone();
            highlightedFocused.Name = FrameworkElement.HighlightedFocusedStateName;
            category.States.Add(highlightedFocused);

            // Create the highlighted state (for mouse hover)
            // by cloning the enabled state since they appear the same;
            StateSave highlighted = enabledState.Clone();
            highlighted.Name = FrameworkElement.HighlightedStateName;
            category.States.Add(highlighted);

            // Add event handles for the keyboard input.
            this.KeyDown += this.HandleKeyDown;

            topLevelContainer.RollOn += this.HandleRollOn;

            this.Visual = topLevelContainer;
        }

        /// <summary>
        /// Handles keyboard input for navigation between buttons using left/right keys.
        /// </summary>
        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Keys.Left)
            {
                this.HandleTab(TabDirection.Up, loop: true);
            }
            if (e.Key == Keys.Right)
            {
                this.HandleTab(TabDirection.Down, loop: true);
            }
        }
        
        /// <summary>
        /// Automatically focuses the button when the mouse hovers over it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleRollOn(object sender, EventArgs e)
        {
            this.IsFocused = true;
        }
    }
}
