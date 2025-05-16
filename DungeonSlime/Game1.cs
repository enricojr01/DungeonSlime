using Microsoft.Xna.Framework.Media;
using MonoGameLibrary;
using DungeonSlime.Scenes;
using MonoGameGum;
using MonoGameGum.Forms.Controls;

namespace DungeonSlime
{
    public partial class Game1 : Core 
    {
        private Song _themeSong;
        public Game1() : base("Dungeon Slime", 1280, 720, false)
        {
        }

        private void InitializeGum()
        {
            // regular init call to start the service
            GumService.Default.Initialize(this);

            // pass the default content manager into Gum
            GumService.Default.ContentLoader.XnaContentManager = Core.Content;

            // register the default keyboard for UI control
            FrameworkElement.KeyboardsForUiControl.Add(GumService.Default.Keyboard);

            // register gamepads for ui control
            FrameworkElement.GamePadsForUiControl.AddRange(GumService.Default.Gamepads);

            // Customize the tab reverse UI to trigger when the keyboard up arrow is pushed.
            FrameworkElement.TabReverseKeyCombos.Add(new KeyCombo() { PushedKey = Microsoft.Xna.Framework.Input.Keys.Up });

            // Make the down key trigger UI navigation too
            FrameworkElement.TabKeyCombos.Add(new KeyCombo() { PushedKey = Microsoft.Xna.Framework.Input.Keys.Down });
            
            // I don't get why we're doing this? To quote the tutorial, "the UI assets were created at 1/4th the size
            // to reduce the size of the texture atlas" so we're setting the canvas size to 1/4 the game's resolution
            // and then using GumService.Default.Renderer.Camera.Zoom to scale everything up to render at full res
            // is it really just space saving?
            GumService.Default.CanvasWidth = GraphicsDevice.PresentationParameters.BackBufferWidth / 4.0f;
            GumService.Default.CanvasHeight = GraphicsDevice.PresentationParameters.BackBufferHeight / 4.0f;
            GumService.Default.Renderer.Camera.Zoom = 4.0f;
        }

        protected override void Initialize()
        {
            base.Initialize();

            this.InitializeGum();

            Audio.PlaySong(this._themeSong);
            ChangeScene(new TitleScene());
        }

        protected override void LoadContent()
        {
            this._themeSong = Content.Load<Song>("audio/theme");
        }
    }
}
