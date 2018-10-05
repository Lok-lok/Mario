namespace MarioPirates
{
    internal class MarioStateStarBigLeftIdle : MarioStateStarBig
    {
        public MarioStateStarBigLeftIdle(Mario mario) : base(mario)
        {
            mario.Sprite = SpriteFactory.Instance.CreateSprite("star_big_idle_left");
        }

        public override void Left()
        {
            mario.State = new MarioStateStarBigLeftRun(mario);
        }

        public override void Right()
        {
            mario.State = new MarioStateStarBigRightRun(mario);
        }

        public override void Jump()
        {
            mario.State = new MarioStateStarBigLeftJump(mario);
        }

        public override void Crouch()
        {
            mario.State = new MarioStateStarBigLeftCrouch(mario);
        }
    }
}
