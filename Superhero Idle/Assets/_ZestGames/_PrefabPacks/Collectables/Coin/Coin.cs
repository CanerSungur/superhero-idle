using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZestGames
{
    public class Coin : CollectableBase
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
