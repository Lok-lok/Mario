﻿using Microsoft.Xna.Framework;

namespace MarioPirates
{
    internal class Star : GameObjectRigidBody
    {
        private const int starWidth = 14, starHeight = 16;
        public Star(int dstX, int dstY)
        {
            location.X = dstX;
            location.Y = dstY;
            size = new Point(starWidth * 2, starHeight * 2);
            Sprite = SpriteFactory.Instance.CreateSprite("stars");
            RigidBody.Mass = 0.05f;
        }
    }
}
