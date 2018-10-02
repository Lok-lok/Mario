namespace MarioPirates
{
    internal class MarioStateSmallRightIdle : MarioStateSmall
    {
        public MarioStateSmallRightIdle(Mario mario) : base(mario)
        {
            mario.Sprite = SpriteFactory.CreateSpriteMario("small_idle_right");
        }

        public override void Left()
        {
            mario.State = new MarioStateSmallLeftRun(mario);
        }

        public override void Right()
        {
            mario.State = new MarioStateSmallRightRun(mario);
        }

        public override void Jump()
        {
            mario.State = new MarioStateSmallRightJump(mario);
        }

        public override void Crouch()
        {
            mario.State = new MarioStateSmallRightCrouch(mario);
        }

        public override void Big()
        {
            mario.State = new MarioStateBigRightIdle(mario);
        }

        public override void Fire()
        {
            mario.State = new MarioStateFireRightIdle(mario);
        }

        public override void Star()
        {
            mario.State = new MarioStateStarSmallRightIdle(mario);
        }
    }
}
