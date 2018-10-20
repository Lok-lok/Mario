﻿namespace MarioPirates.Event
{
    internal class GameObjectDestroyEventArgs : System.EventArgs
    {
        public readonly GameObject Object;

        public GameObjectDestroyEventArgs(GameObject o) => Object = o;
    }
}
