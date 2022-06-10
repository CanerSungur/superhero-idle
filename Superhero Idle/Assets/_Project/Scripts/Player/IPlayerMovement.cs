
namespace SuperheroIdle
{
    public interface IPlayerMovement
    {
        public void Init(Player player);
        public void Motor();
        public bool IsMoving { get; }
        public bool IsGrounded { get; }
    }
}
