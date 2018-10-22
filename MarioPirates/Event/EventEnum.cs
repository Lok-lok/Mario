﻿namespace MarioPirates
{
    internal enum EventEnum : int
    {
        Action,

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

        // Generic keyboard input
        KeyDown,
        KeyUp,
        KeyHold,

        // 
        Collide,
        GameObjectCreate,
        GameObjectDestroy,
    }
}
