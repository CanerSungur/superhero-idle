namespace ZestGames
{
    public interface IAiMovement
    {
        public void Init(Ai ai);
        public void Motor();
        public bool IsMoving { get; }
        public bool IsGrounded { get; }

    }
}
