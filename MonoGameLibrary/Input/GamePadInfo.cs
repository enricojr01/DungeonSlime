using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGameLibrary.Input
{
    public class GamePadInfo
    {
        private TimeSpan _vibrationTimeRemaining = TimeSpan.Zero;
        public PlayerIndex PlayerIndex { get; }

        public GamePadState PreviousState { get; private set; }

        public GamePadState CurrentState { get; private set; }

        public bool IsConnected => CurrentState.IsConnected;
        public Vector2 LeftThumbStick => CurrentState.ThumbSticks.Left;
        public Vector2 RightThumbStick => CurrentState.ThumbSticks.Right;
        public float LeftTrigger => CurrentState.Triggers.Left;
        public float RightTrigger => CurrentState.Triggers.Right;

        public GamePadInfo(PlayerIndex playerIndex)
        {
            this.PlayerIndex = playerIndex;
            this.PreviousState = new GamePadState();
            this.CurrentState = GamePad.GetState(playerIndex);
        }

        public void Update(GameTime gameTime)
        {
            this.PreviousState = this.CurrentState;
            this.CurrentState = GamePad.GetState(PlayerIndex);

            if (_vibrationTimeRemaining > TimeSpan.Zero)
            {
                this._vibrationTimeRemaining = - gameTime.ElapsedGameTime;

                if (this._vibrationTimeRemaining <= TimeSpan.Zero)
                {
                    StopVibration();
                }
            }
        }

        public bool IsButtonDown(Buttons button)
        {
            return CurrentState.IsButtonDown(button);
        }

        public bool IsButtonUp(Buttons button)
        {
            return CurrentState.IsButtonUp(button);
        }

        public bool WasButtonJustPressed(Buttons button)
        {
            return CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);
        }

        public bool WasButtonJustReleased(Buttons button)
        {
            return CurrentState.IsButtonUp(button) && PreviousState.IsButtonDown(button);
        }

        public void SetVibration(float strength, TimeSpan time)
        {
            this._vibrationTimeRemaining = time;
            GamePad.SetVibration(PlayerIndex, strength, strength);
        }

        public void StopVibration()
        {
            GamePad.SetVibration(PlayerIndex, 0.0f, 0.0f);
        }
    }
}
