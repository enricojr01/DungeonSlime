using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoGameLibrary.Input
{
    public class MouseInfo
    {
        public MouseState PreviousState { get; private set; }
        public MouseState CurrentState { get; private set; }

        public Point Position
        {
            get => CurrentState.Position;
            set => SetPosition(value.X, value.Y);
        }

        public int X
        {
            get => CurrentState.X;
            set => SetPosition(value, CurrentState.Y);
        }

        public int Y
        {
            get => CurrentState.Y;
            set => SetPosition(CurrentState.X, value);
        }

        public Point PositionDelta => CurrentState.Position - PreviousState.Position;
        public int XDelta => CurrentState.X - PreviousState.X;
        public int YDelta => CurrentState.Y - PreviousState.Y;
        public bool WasMoved => PositionDelta != Point.Zero;

        public int ScrollWheel => CurrentState.ScrollWheelValue;
        public int ScrollWheelDelta => CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue;

        public MouseInfo()
        {
            this.PreviousState = new MouseState();
            this.CurrentState = Mouse.GetState();
        }

        public void Update()
        {
            this.PreviousState = this.CurrentState;
            this.CurrentState = Mouse.GetState();
        }

        public bool IsButtonDown(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return CurrentState.LeftButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return CurrentState.MiddleButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return CurrentState.RightButton == ButtonState.Pressed;
                case MouseButton.XButton1:
                    return CurrentState.XButton1 == ButtonState.Pressed;
                case MouseButton.XButton2:
                    return CurrentState.XButton2 == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        public bool IsButtonUp(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return CurrentState.LeftButton == ButtonState.Released;
                case MouseButton.Middle:
                    return CurrentState.MiddleButton == ButtonState.Released;
                case MouseButton.Right:
                    return CurrentState.RightButton == ButtonState.Released;
                case MouseButton.XButton1:
                    return CurrentState.XButton1 == ButtonState.Released;
                case MouseButton.XButton2:
                    return CurrentState.XButton2 == ButtonState.Released;
                default:
                    return false;
            }
        }
        public bool WasButtonJustPressed(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return CurrentState.LeftButton == ButtonState.Released && PreviousState.LeftButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return CurrentState.MiddleButton == ButtonState.Released && PreviousState.LeftButton == ButtonState.Pressed;
                case MouseButton.Right:
                    return CurrentState.RightButton == ButtonState.Released && PreviousState.LeftButton == ButtonState.Pressed;
                case MouseButton.XButton1:
                    return CurrentState.XButton1 == ButtonState.Released && PreviousState.LeftButton == ButtonState.Pressed;
                case MouseButton.XButton2:
                    return CurrentState.XButton2 == ButtonState.Released && PreviousState.LeftButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        public void SetPosition(int x, int y)
        {
            Mouse.SetPosition(x, y);
            CurrentState = new MouseState(
                x,
                y,
                CurrentState.ScrollWheelValue,
                CurrentState.LeftButton,
                CurrentState.MiddleButton,
                CurrentState.RightButton,
                CurrentState.XButton1,
                CurrentState.XButton2
            );

        }
    }
}
