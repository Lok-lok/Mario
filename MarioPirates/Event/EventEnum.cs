﻿namespace MarioPirates.Event
{
    internal enum EventEnum : int
    {
        // Common input
        KeyUpDown,
        KeyDownDown,
        KeyLeftDown,
        KeyRightDown,
        KeyUpUp,
        KeyDownUp,
        KeyLeftUp,
        KeyRightUp,
        KeyUpHold,
        KeyDownHold,
        KeyLeftHold,
        KeyRightHold,
        KeyWDown,
        KeySDown,
        KeyADown,
        KeyDDown,
        KeyWUp,
        KeySUp,
        KeyAUp,
        KeyDUp,
        KeyWHold,
        KeySHold,
        KeyAHold,
        KeyDHold,

        // Generic keyboard input
        KeyDown,
        KeyUp,
        KeyHold,

        // Generic gamepad input
        ButtonDown,
        ButtonUp,
        ButtonHold,

        // 
        Collide,
    }
}
