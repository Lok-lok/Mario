﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using static System.IO.File;

namespace MarioPirates
{
    internal class Mario : GameObjectRigidBody, IDisposable
    {
        public readonly Dictionary<string, Sprite> Sprites;

        public readonly MarioState State;

        private Action unsubscribe;

        private int JumpHoldCount;

        private int TransitionToBigCount;

        private int TransitionToSmallCount;

        private Vector2 StoredToBigLocation;
        private Vector2 StoredToSmallLocation;

        public Mario(int dstX, int dstY) : base(dstX, dstY, 0, 0)
        {
            Sprites = new Dictionary<string, Sprite>();
            new JavaScriptSerializer().Deserialize<List<string>>(ReadAllText("Content\\MarioSpritesList.json"))
                 .ForEach(s => Sprites.Add(s, SpriteFactory.Ins.CreateSprite(s)));

            RigidBody.CoR = Constants.MARIO_CO_R; //0.5f

            State = new MarioState(this);

            SubscribeInput();

            JumpHoldCount = 0;
            TransitionToBigCount = Constants.MARIO_TRANSITION_COUNT_MAX; //30
            TransitionToSmallCount = Constants.MARIO_TRANSITION_COUNT_MAX; //30
        }

        public void SubscribeInput()
        {
            SubscribeInputMoving();
            SubscribeInputTransition();
            SubscribeInputX();
        }

        private void UnsubscribeInput()
        {
            unsubscribe();
            unsubscribe = null;
        }

        private void SubscribeInputMoving()
        {
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyUpHold, (s, e) =>
            {
                if (JumpHoldCount < Constants.MARIO_JUMP_HOLD_COUNT_LIMIT) // 30
                {
                    RigidBody.ApplyForce(new Vector2(0, Constants.MARIO_Y_FORCE + JumpHoldCount * Constants.MARIO_JUMP_HOLD_COUNT_MULTIPLIER)); // -5000, 240
                    JumpHoldCount += 1;
                }
            });
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyUpDown, (s, e) =>
            {
                if (RigidBody.Grounded)
                {
                    RigidBody.ApplyForce(new Vector2(0, Constants.MARIO_Y_FORCE + JumpHoldCount * Constants.MARIO_JUMP_HOLD_COUNT_MULTIPLIER)); //-5000, 240
                    JumpHoldCount = 1;
                }
            });
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyDownHold, (s, e) =>
            {
                State.Crouch();
            });
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyLeftHold, (s, e) =>
            {
                if (!State.IsCrouch)
                {
                    RigidBody.ApplyForce(new Vector2(-Constants.MARIO_X_FORCE * State.VelocityMultipler, 0)); //-2000
                }
            });
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyRightHold, (s, e) =>
            {
                if (!State.IsCrouch)
                {
                    RigidBody.ApplyForce(new Vector2(Constants.MARIO_X_FORCE * State.VelocityMultipler, 0)); // 2000
                }
            });

            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyRightHold, (s, e) => State.Right());
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyLeftHold, (s, e) => State.Left());

            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyRightDown, (s, e) =>
            {
                if (RigidBody.Velocity.X < -0)
                {
                    State.Brake();
                }
                else
                {
                    State.Coast();
                }
            });
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyLeftDown, (s, e) =>
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

            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyUpUp, (s, e) => JumpHoldCount = Constants.MARIO_JUMP_HOLD_COUNT_LIMIT);
        }

        private void SubscribeInputTransition()
        {
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyDown, (s, e) =>
            {
                switch ((e as KeyDownEventArgs).key)
                {
                    case Keys.Y:
                        if (!State.IsSmall)
                            TransitionToSmallCount = 0;
                        State.Small();
                        break;
                    case Keys.U:
                        if (State.IsSmall)
                            TransitionToBigCount = 0;
                        State.Big();
                        break;
                    case Keys.I:
                        if (State.IsSmall)
                            TransitionToBigCount = 0;
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

        private void SubscribeInputX()
        {
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyXDown, (s, e) => State.Accelerated());
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyXUp, (s, e) => State.CancelAccelerated());
            unsubscribe += EventManager.Ins.Subscribe(EventEnum.KeyXDown, (s, e) =>
            {
                if (State.IsFire)
                {
                    var fireball = new Fireball((int)Location.X + (State.IsLeft ? -Constants.FIREBALL_WIDTH : Constants.FIREBALL_WIDTH + Size.X), (int)Location.Y + Constants.FIREBALL_HEIGHT); //16, 16, 16
                    fireball.RigidBody.Velocity = new Vector2(State.IsLeft ? -Constants.FIREBALL_COLLISION_VELOCITY : Constants.FIREBALL_COLLISION_VELOCITY, 0f); // -200, 200
                    EventManager.Ins.RaiseEvent(EventEnum.GameObjectCreate, this, new GameObjectCreateEventArgs(fireball));
                    EventManager.Ins.RaiseEvent(EventEnum.GameObjectDestroy, this, new GameObjectDestroyEventArgs(fireball), Constants.DESTROY_FIREBALL_DELAY); //3000
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
            if (RigidBody.Velocity.X < Constants.MARIO_TRANSITION_COUNT_MAX && RigidBody.Velocity.X > -Constants.MARIO_TRANSITION_COUNT_MAX) // 30, -30
                State.Coast();

            if (TransitionToBigCount < Constants.MARIO_TRANSITION_COUNT_MAX)
            {
                if (TransitionToBigCount % Constants.MARIO_TRANSITION_COUNT == 0) //5
                {
                    StoredToBigLocation = new Vector2(Location.X, Location.Y + Size.Y);
                    var targetHeight = (int)(MarioStateBig.marioHeight / Constants.MARIO_TRANSITION_ZOOM[TransitionToBigCount / Constants.MARIO_TRANSITION_COUNT]); //5
                    Location = new Vector2(StoredToBigLocation.X, StoredToBigLocation.Y - targetHeight);
                    Size = new Point(MarioStateBig.marioWidth, targetHeight);
                }
                TransitionToBigCount++;
            }

            if (TransitionToSmallCount < Constants.MARIO_TRANSITION_COUNT_MAX)
            {
                if (TransitionToSmallCount % Constants.MARIO_TRANSITION_COUNT == 0) //5
                {
                    StoredToSmallLocation = new Vector2(Location.X, Location.Y + Size.Y);
                    var targetHeight = (int)(MarioStateSmall.marioHeight * Constants.MARIO_TRANSITION_ZOOM[TransitionToSmallCount / Constants.MARIO_TRANSITION_COUNT]); //5
                    Location = new Vector2(StoredToSmallLocation.X, StoredToSmallLocation.Y - targetHeight);
                    Size = new Point(MarioStateSmall.marioWidth, targetHeight);
                }

                TransitionToSmallCount++;
            }

            Camera.Ins.LookAt(Location);

            base.Update(dt);
        }

        public override void PostCollide(GameObjectRigidBody other, CollisionSide side)
        {
            base.PostCollide(other, side);
            // Response to collision with items.
            if (other is Coin)
            {
                // Score up
            }
            else if (other is Flower)
            {
                if (State.IsSmall)
                {
                    TransitionToBigCount = 0;
                    State.Transiting();
                    EventManager.Ins.RaiseEvent(EventEnum.Action, this, new ActionEventArgs(() => State.CancelTransiting()), Constants.DESTROY_FLOWER_DELAY); //1000
                }
                State.Fire();
            }
            else if (other is GreenMushroom)
            {
                // Life up
            }
            else if (other is PipeTop pipe && pipe.ToLevel != null && side is CollisionSide.Bottom && State.Action.State == MarioStateEnum.Crouch)
            {
                UnsubscribeInput();
                Location = new Vector2(pipe.Location.X + Constants.MARIO_LOCATION_IN_PIPE, Location.Y); // 32/4
                RigidBody.Motion = MotionEnum.Keyframe;
                RigidBody.Velocity = new Vector2(0f, Constants.MARIO_PIPE_COLLISION_VELOCITY); //50
                EventManager.Ins.RaiseEvent(EventEnum.Action, this, new ActionEventArgs(() => Scene.Ins.Active(pipe.ToLevel)), Constants.MARIO_COLLISION_EVENT_DELAY); //1000
            }
            else if (other is RedMushroom)
            {
                if (State.IsSmall)
                {
                    TransitionToBigCount = 0;
                    State.Transiting();
                    EventManager.Ins.RaiseEvent(EventEnum.Action, this, new ActionEventArgs(() => State.CancelTransiting()), Constants.MARIO_COLLISION_EVENT_DELAY); //1000
                }
                State.Big();
            }
            else if (other is Star)
            {
                State.Invincible();
                EventManager.Ins.RaiseEvent(EventEnum.Action, this, new ActionEventArgs(() => State.CancelInvincible()), Constants.MARIO_STAR_COLLISION_EVENT_DELAY); // 3000
            }

            // Response to collsion with enemies
            if (other is Goomba || other is Koopa)
            {
                if (side != CollisionSide.Bottom && !State.IsInvincible && !State.IsTransiting)
                {
                    if (State.IsSmall)
                    {
                        RigidBody.Velocity = new Vector2(0f, Constants.SMALL_MARIO_ENEMY_COLLISION_VELOCITY); //-250
                        State.Dead();
                    }
                    else
                    {
                        TransitionToSmallCount = 0;
                        State.Transiting();
                        EventManager.Ins.RaiseEvent(EventEnum.Action, this, new ActionEventArgs(() => State.CancelTransiting()), Constants.MARIO_COLLISION_EVENT_DELAY); //1000
                        State.Small();
                    }
                }
                else
                {
                    RigidBody.Velocity = new Vector2(0f, Constants.MARIO_ENEMY_COLLISION_VELOCITY); //-200
                }
            }
        }

        public void Dispose()
        {
            UnsubscribeInput();
            EventManager.Ins.RaiseEvent(EventEnum.KeyDown, this, new KeyDownEventArgs(Keys.R), Constants.MARIO_DISPOSE_EVENT_DELAY); //4000
        }
    }
}
