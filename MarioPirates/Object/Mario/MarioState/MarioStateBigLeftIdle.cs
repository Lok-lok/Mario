namespace MarioPirates
{
    internal class MarioStateBigLeftIdle : MarioStateBig
    {
        public MarioStateBigLeftIdle(Mario mario) : base(mario)
        {
            mario.Sprite = SpriteFactory.CreateSprite("big_idle_left");
        }

        public override void Left()
        {
            mario.State = new MarioStateBigLeftRun(mario);
        }

        public override void Right()
        {
            mario.State = new MarioStateBigRightRun(mario);
        }

        public override void Jump()
        {
            mario.State = new MarioStateBigLeftJump(mario);
        }

        public override void Crouch()
        {
            mario.State = new MarioStateBigLeftCrouch(mario);
        }

        public override void Small()
        {
            mario.State = new MarioStateSmallLeftIdle(mario);
        }

        public override void Fire()
        {
            mario.State = new MarioStateFireLeftIdle(mario);
        }

        public override void Star()
        {
            mario.State = new MarioStateStarBigLeftIdle(mario);
        }
    }
}
