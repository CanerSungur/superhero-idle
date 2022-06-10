
namespace ZestGames
{
    public class CollectableExample : CollectableBase
    {
        public override void Apply()
        {
            if (coll) coll.enabled = false;

            // Play Audio

            if (collectStyle == CollectStyle.OnSite)
            {
                // collect instantly
            }
            else if (collectStyle == CollectStyle.MoveToUi)
            {
                if (movement)
                {
                    movement.OnStartMovement?.Invoke();
                }
                else
                {
                    // collect instantly

                }
            }
        }
    }
}
