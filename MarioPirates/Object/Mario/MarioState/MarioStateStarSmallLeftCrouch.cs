namespace MarioPirates
{
    internal class MarioStateStarSmallLeftCrouch : MarioStateStarSmall
    {
        public MarioStateStarSmallLeftCrouch(Mario mario) : base(mario)
        {
            mario.Sprite = SpriteFactory.Instance.CreateSpriteMario("star_small_crouch_right");
        }

        public override void Left()
        {
            mario.State = new MarioStateStarSmallLeftCrouchRun(mario);
        }

        public override void Right()
        {
            mario.State = new MarioStateStarSmallRightCrouchRun(mario);
        }

        public override void Jump()
        {
            mario.State = new MarioStateStarSmallLeftIdle(mario);
        }

        public override void Crouch()
        {
        }

        public override void Big()
        {
            mario.State = new MarioStateStarBigLeftCrouch(mario);
        }

        public override void Fire()
        {
            mario.State = new MarioStateStarBigLeftCrouch(mario);
        }
    }
}
