using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary.Audio;
using MonoGameLibrary.Input;
using MonoGameLibrary.Scenes;

namespace MonoGameLibrary
{
    public class Core : Game
    {
        internal static Core s_instance;

        /// <summary>
        /// Gets a reference to the audio control system.
        /// </summary>
        public static AudioController Audio { get; private set; }

        /// <summary>
        /// Gets the content manager, which is used to load assets
        /// </summary>
        public static new ContentManager Content { get; private set; }

        /// <summary>
        /// Gets a reference to the Core instance.
        /// </summary>
        public static Core Instance => s_instance;

        /// <summary>
        /// Gets the graphics device used to create graphical resources and perform rendering.
        /// </summary>
        public static new GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// Gets the graphics device manager, for the control over the presentation of graphics.
        /// </summary>
        public static GraphicsDeviceManager Graphics { get; private set; }

        /// <summary>
        /// Gets a reference to the input management system
        /// </summary>
        public static InputManager Input { get; private set; }

        /// <summary>
        /// Gets the SpriteBatch, which is used for all 2D Rendering
        /// </summary>
        public static SpriteBatch SpriteBatch { get; private set; }
        private static Scene s_activeScene;
        private static Scene s_nextScene;

        /// <summary>
        /// Gets / Sets the value that indicates the game will exit if when the esc key is pressed.
        /// </summary>
        public static bool ExitOnEscape { get; set; }

        public Core(string title, int width, int height, bool fullScreen)
        {
            if (s_instance != null)
            {
                throw new InvalidOperationException($"Only a single Core instance can be created!");
            }

            // The tutorial says this is to ensure "global member access"
            // but i'm not entirely certain what they mean by that, yet.
            s_instance = this;

            Graphics = new GraphicsDeviceManager(this);
            Graphics.PreferredBackBufferWidth = width;
            Graphics.PreferredBackBufferHeight = height;
            Graphics.IsFullScreen = fullScreen;
            Graphics.ApplyChanges();

            Window.Title = title;
            // NOTE: The core's content manager is a reference to the base
            //       content manager.
            // NOTE: Why the explicit assignment though? For conveninece?
            //       its not as if base.Content is inaccessible to derived classes
            Content = base.Content;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            GraphicsDevice = base.GraphicsDevice;
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Input = new InputManager();
            Audio = new AudioController();
        }

        protected override void UnloadContent()
        {
            Audio.Dispose();
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            // Update the input manager.
            Input.Update(gameTime);

            // Update the audio controller.
            Audio.Update();

            if (ExitOnEscape && Input.Keyboard.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // if nextScene is set, transition to that scene
            // otherwise update the active scene;
            if (s_nextScene != null)
            {
                TransitionScene();
            }

            if (s_activeScene != null)
            {
                s_activeScene.Update(gameTime);
            }

            base.Update(gameTime); 
        }

        protected override void Draw(GameTime gameTime)
        {
            if (s_activeScene != null)
            {
                s_activeScene.Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        public static void ChangeScene(Scene next)
        {
            if (s_activeScene != next)
            {
                s_nextScene = next;
            }
        }
        
        private static void TransitionScene()
        {
            if (s_activeScene != null)
            {
                s_activeScene.Dispose();
            }

            GC.Collect();
            s_activeScene = s_nextScene;
            s_nextScene = null;

            if (s_activeScene != null)
            {
                s_activeScene.Initialize();
            }
        }
    }
}
