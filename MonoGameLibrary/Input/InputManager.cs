using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Input
{
    public class InputManager
    {
        public KeyboardInfo Keyboard { get; private set; }
        public MouseInfo Mouse { get; private set; }
        public GamePadInfo[] GamePads { get; private set; }
        
        public InputManager()
        {
            this.Keyboard = new KeyboardInfo();
            this.Mouse = new MouseInfo();
            this.GamePads = new GamePadInfo[4];
            for (int i = 0; i < 4; i++)
            {
                GamePads[i] = new GamePadInfo((PlayerIndex)i);
            }
        }

        public void Update(GameTime gameTime)
        {
            this.Keyboard.Update();
            this.Mouse.Update();

            for (int i = 0; i < 4; i++)
            {
                GamePads[i].Update(gameTime);
            }
        }
    }
}
