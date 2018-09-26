namespace MarioPirates
{

    public class MarioStateFireRightIdle : MarioStateFire
    {
        public MarioStateFireRightIdle(Mario mario) : base(mario)
        {
            mario.sprite = SpriteFactory.Instance.CreateSprite("mario_fire_idle_right");
        }

        public override void Left()
        {
            mario.State = new MarioStateFireLeftIdle(mario);
        }

        public override void Right()
        {
            mario.State = new MarioStateFireRightRun(mario);
        }

        public override void Jump()
        {
            mario.State = new MarioStateFireRightJump(mario);
        }

        public override void Crouch()
        {
            mario.State = new MarioStateFireRightCrouch(mario);
        }

        public override void Small()
        {
            mario.State = new MarioStateSmallRightIdle(mario);
        }

        public override void Big()
        {
            mario.State = new MarioStateBigRightIdle(mario);
        }

        public override void Fire()
        {
        }
    }

}
