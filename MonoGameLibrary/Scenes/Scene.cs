using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameLibrary.Scenes
{
    public abstract class Scene : IDisposable
    {
        /// <summary>
        /// Gets the ContentManager used for loading scene-specific assets.
        /// </summary>
        protected ContentManager Content { get; }

        /// <summary>
        /// Gets a value that indicates if the scene has been disposed of.
        /// </summary>
        public bool IsDisposed { get; private set; }

        public Scene()
        {
            this.Content = new ContentManager(Core.Content.ServiceProvider);
            this.Content.RootDirectory = Core.Content.RootDirectory;
        }

        /// <summary>
        /// Initializes the scene.
        /// </summary>
        /// <remarks>
        /// When overriding this in a derived class, ensure that base.Initialize()
        /// is still called, as this is when LoadContent() is called, and make
        /// sure it's at the END of the function body not the beginning.
        /// </remarks>
        public virtual void Initialize()
        {
            this.LoadContent();
        }
        
        /// <summary>
        /// Loads scene-specific content. Override this in derived classes.
        /// </summary>
        public virtual void LoadContent() { }

        /// <summary>
        /// Unloads scene-specific content.
        /// </summary>
        public virtual void UnloadContent() { }

        /// <summary>
        /// Updates the scene.
        /// </summary>
        /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
        public virtual void Update(GameTime gameTime) { }

        /// <summary>
        /// Draws the scene
        /// </summary>
        /// <param name="gameTime">A snapshot of the timing values for the current frame.</param>
        public virtual void Draw(GameTime gameTime) { }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.UnloadContent();
                this.Content.Dispose();
            }
        }

        ~Scene() => Dispose(false);
    }
}
