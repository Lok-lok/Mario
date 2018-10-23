﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using static System.IO.File;

namespace MarioPirates
{
    internal class Mario : GameObjectRigidBody
    {
        private const int JumpHoldCountLimit = 20;

        public readonly Dictionary<string, Sprite> Sprites;

        public readonly MarioState State;

        private Action unsubscribe;

        private int JumpHoldCount;

        public Mario(int dstX, int dstY) : base(dstX, dstY, 0, 0)
        {
            Sprites = new Dictionary<string, Sprite>();
            new JavaScriptSerializer().Deserialize<List<string>>(ReadAllText("Content\\MarioSpritesList.json"))
                 .ForEach(s => Sprites.Add(s, SpriteFactory.Ins.CreateSprite(s)));

            RigidBody.CoR = 0.5f;

            State = new MarioState(this);

            SubscribeInputMoving();
            SubscribeInputTransition();
            SubscribeInputExtended();

            JumpHoldCount = 0;
        }

        private void SubscribeInputMoving()
        {
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyUpHold, (s, e) =>
            {
                if (!State.IsDead && JumpHoldCount < JumpHoldCountLimit)
                {
                    RigidBody.ApplyForce(new Vector2(0, -3000 + JumpHoldCount * 50));
                    JumpHoldCount += 1;
                }
            });
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyUpDown, (s, e) =>
            {
                if (!State.IsDead && JumpHoldCount < JumpHoldCountLimit)
                {
                    RigidBody.ApplyForce(new Vector2(0, -3000 + JumpHoldCount * 50));
                    JumpHoldCount += 1;
                }
            });
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyDownHold, (s, e) =>
            {
                if (!State.IsDead)
                {
                    State.Crouch();
                }
            });
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyLeftHold, (s, e) =>
            {
                if (!State.IsDead && !State.IsCrouch)
                {
                    RigidBody.ApplyForce(new Vector2(-2000 * State.VelocityMultipler, 0));
                }
            });
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyRightHold, (s, e) =>
            {
                if (!State.IsDead && !State.IsCrouch)
                {
                    RigidBody.ApplyForce(new Vector2(2000 * State.VelocityMultipler, 0));
                }
            });

            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyRightHold, (s, e) => State.Right());
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyLeftHold, (s, e) => State.Left());

            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyRightHold, (s, e) =>
            {
                if (RigidBody.Velocity.X < 0)
                {
                    State.Brake();
                }
                else
                {
                    State.Coast();
                }
            });
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyLeftHold, (s, e) =>
            {
                if (RigidBody.Velocity.X > 0)
                {
                    State.Brake();
                }
                else
                {
                    State.Coast();
                }
            });

            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyUpUp, (s, e) => JumpHoldCount = JumpHoldCountLimit);

            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyUpDown, (s, e) =>
            {
                if (RigidBody.Grounded)
                {
                    JumpHoldCount = 0;
                }
            });
        }

        private void SubscribeInputTransition()
        {
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyDown, (s, e) =>
            {
                switch ((e as KeyDownEventArgs).key)
                {
                    case Keys.Y:
                        State.Small();
                        break;
                    case Keys.U:
                        State.Big();
                        break;
                    case Keys.I:
                        State.Fire();
                        break;
                    case Keys.O:
                        State.Dead();
                        break;
                    case Keys.P:
                        State.Invincible();
                        break;
                }
            });
        }

        private void SubscribeInputExtended()
        {
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyXDown, (s, e) => State.Accelerated());
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyXUp, (s, e) => State.CancelAccelerated());
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyXDown, (s, e) =>
            {
                if (State.IsFire)
                {
                    var fireball = new Fireball((int)Location.X + (State.IsLeft ? -16 : 16 + Size.X), (int)Location.Y + 16);
                    fireball.RigidBody.Velocity = new Vector2(State.IsLeft ? -200f : 200f, 0f);
                    EventManager.Ins.RaiseEvent(EventEnum.GameObjectCreate, this, new GameObjectCreateEventArgs(fireball));
                    EventManager.Ins.RaiseEvent(EventEnum.GameObjectDestroy, this, new GameObjectDestroyEventArgs(fireball), 3000f);
                }
            });
        }

        public override void Update(float dt)
        {
            if (RigidBody.Velocity.Y != 0f)
                State.Jump();
            else if (RigidBody.Velocity.X != 0f)
                State.Run();
            else
                State.Idle();

            Camera.Ins.LookAt(Location);

            base.Update(dt);
        }

        public override void PostCollide(GameObjectRigidBody other, CollisionSide side)
        {
            // Response to collision with items.
            if (other is Coin)
            {
                // Score up
            }
            else if (other is Flower)
            {
                State.Fire();
            }
            else if (other is GreenMushroom)
            {
                // Life up
            }
            else if (other is PipeTop && side == CollisionSide.Bottom)
            {
                // Get in the pipe
            }
            else if (other is RedMushroom)
            {
                State.Big();
            }
            else if (other is Star)
            {
                State.Invincible();
                EventManager.Ins.RaiseEvent(EventEnum.Action, this, new ActionEventArgs(() => State.CancelInvincible()), 3000f);
            }

            // Response to collsion with enemies
            if (other is Goomba || other is Koopa)
            {
                if (side != CollisionSide.Bottom && !State.IsInvincible)
                {
                    if (State.IsSmall)
                    {
                        RigidBody.Velocity = new Vector2(0f, -250f);
                        State.Dead();
                        unsubscribe();
                        unsubscribe = null;
                        EventManager.Ins.RaiseEvent(EventEnum.GameObjectDestroy, this, new GameObjectDestroyEventArgs(this), 3000f);
                        EventManager.Ins.RaiseEvent(EventEnum.KeyDown, this, new KeyDownEventArgs(Keys.R), 4000f);
                    }
                    else
                    {
                        State.Small();
                    }
                }
                else
                {
                    RigidBody.Velocity = new Vector2(0f, -200f);
                }
            }

            base.PostCollide(other, side);
        }
    }
}
