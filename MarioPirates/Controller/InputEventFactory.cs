﻿using Microsoft.Xna.Framework.Input;
using System;

namespace MarioPirates.Controller
{
    internal static class InputEventFactory
    {
        public static (EventEnum, EventArgs) CreateKeyEvent(InputState state, Keys key)
        {
            if (Enum.TryParse<EventEnum>("Key" + key.ToString() + state.ToString(), out var e))
            {
                return (e, EventArgs.Empty);
            }
            switch (state)
            {
                case InputState.Down:
                    return (EventEnum.KeyDown, new KeyDownEventArgs(key));
            }
            return (0, null);
        }

        public static (EventEnum, EventArgs) CreateButtonEvent(InputState state, Buttons button)
        {
            switch (state)
            {
                case InputState.Down:
                    switch (button)
                    {
                        case Buttons.LeftThumbstickDown:
                            return (EventEnum.KeyDownDown, EventArgs.Empty);
                        case Buttons.LeftThumbstickLeft:
                            return (EventEnum.KeyLeftDown, EventArgs.Empty);
                        case Buttons.LeftThumbstickRight:
                            return (EventEnum.KeyRightDown, EventArgs.Empty);
                        case Buttons.A:
                            return (EventEnum.KeyUpDown, EventArgs.Empty);
                        case Buttons.B:
                            return (EventEnum.KeyXDown, EventArgs.Empty);
                    }
                    break;
                case InputState.Hold:
                    switch (button)
                    {
                        case Buttons.LeftThumbstickDown:
                            return (EventEnum.KeyDownHold, EventArgs.Empty);
                        case Buttons.LeftThumbstickLeft:
                            return (EventEnum.KeyLeftHold, EventArgs.Empty);
                        case Buttons.LeftThumbstickRight:
                            return (EventEnum.KeyRightHold, EventArgs.Empty);
                        case Buttons.A:
                            return (EventEnum.KeyUpHold, EventArgs.Empty);
                        case Buttons.B:
                            return (EventEnum.KeyXHold, EventArgs.Empty);
                    }
                    break;
                case InputState.Up:
                    switch (button)
                    {
                        case Buttons.LeftThumbstickDown:
                            return (EventEnum.KeyDownUp, EventArgs.Empty);
                        case Buttons.LeftThumbstickLeft:
                            return (EventEnum.KeyLeftUp, EventArgs.Empty);
                        case Buttons.LeftThumbstickRight:
                            return (EventEnum.KeyRightUp, EventArgs.Empty);
                        case Buttons.A:
                            return (EventEnum.KeyUpUp, EventArgs.Empty);
                        case Buttons.B:
                            return (EventEnum.KeyXUp, EventArgs.Empty);
                    }
                    break;
            }
            return (0, null);
        }
    }
}
