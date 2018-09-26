namespace MarioPirates
{

    public class MarioStateFireLeftJump : MarioStateFire
    {
        public MarioStateFireLeftJump(Mario mario) : base(mario)
        {
            mario.sprite = SpriteFactory.Instance.CreateSprite("mario_fire_jump_left");
        }

        public override void Jump()
        {
        }

        public override void Crouch()
        {
            mario.State = new MarioStateFireLeftIdle(mario);
        }

        public override void Small()
        {
            mario.State = new MarioStateSmallLeftJump(mario);
        }

        public override void Big()
        {
            mario.State = new MarioStateBigLeftJump(mario);
        }

        public override void Fire()
        {
        }
    }

}
