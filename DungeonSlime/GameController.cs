using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Input;

namespace DungeonSlime
{
    public static class GameController
    {
        private static KeyboardInfo s_keyboard => Core.Input.Keyboard;
        private static GamePadInfo s_gamePad => Core.Input.GamePads[(int)PlayerIndex.One];

        /// <summary>
        /// returns true if the player has triggered the "move up" action
        /// </summary>
        /// <returns>true / false, if the player is pushing Up / W / DpadUp / LeftThumbstickUp </returns>
        public static bool MoveUp()
        {
            return s_keyboard.WasKeyJustPressed(Keys.Up) ||
                s_keyboard.WasKeyJustPressed(Keys.W) ||
                s_gamePad.WasButtonJustPressed(Buttons.DPadUp) ||
                s_gamePad.WasButtonJustPressed(Buttons.LeftThumbstickUp);
        }

        /// <summary>
        /// returns true if the player has triggered the "move down" action
        /// </summary>
        /// <returns>true / false, if the player is pushing Down / S / DpadDown / LeftThumbstickDown </returns>
        public static bool MoveDown()
        {
            return s_keyboard.WasKeyJustPressed(Keys.Down) ||
                s_keyboard.WasKeyJustPressed(Keys.S) ||
                s_gamePad.WasButtonJustPressed(Buttons.DPadDown) ||
                s_gamePad.WasButtonJustPressed(Buttons.LeftThumbstickDown);
        }

        /// <summary>
        /// returns true if the player has triggered the "move down" action
        /// </summary>
        /// <returns>true / false, if the player is pushing Left / A / DpadLeft / LeftThumbstickLeft </returns>
        public static bool MoveLeft()
        {
            return s_keyboard.WasKeyJustPressed(Keys.Left) ||
                s_keyboard.WasKeyJustPressed(Keys.A) ||
                s_gamePad.WasButtonJustPressed(Buttons.DPadLeft) ||
                s_gamePad.WasButtonJustPressed(Buttons.LeftThumbstickLeft);
        }

        /// <summary>
        /// returns true if the player has triggered the "move down" action
        /// </summary>
        /// <returns>true / false, if the player is pushing Right / D / DpadRight / LeftThumbstickRight </returns>
        public static bool MoveRight()
        {
            return s_keyboard.WasKeyJustPressed(Keys.Right) ||
                s_keyboard.WasKeyJustPressed(Keys.D) ||
                s_gamePad.WasButtonJustPressed(Buttons.DPadRight) ||
                s_gamePad.WasButtonJustPressed(Buttons.LeftThumbstickRight);
        }
        
        /// <summary>
        /// Returns true if the player has triggered the "pause" action.
        /// </summary>
        /// <returns></returns>
        public static bool Pause()
        {
            return s_keyboard.WasKeyJustPressed(Keys.Escape) ||
                s_gamePad.WasButtonJustPressed(Buttons.Start);
        }

        /// <summary>
        /// Returns true if the player has triggered the "pause" action.
        /// </summary>
        /// <returns></returns>
        public static bool Action()
        {
            return s_keyboard.WasKeyJustPressed(Keys.Enter) ||
                s_gamePad.WasButtonJustPressed(Buttons.A);
        }
    }
}
