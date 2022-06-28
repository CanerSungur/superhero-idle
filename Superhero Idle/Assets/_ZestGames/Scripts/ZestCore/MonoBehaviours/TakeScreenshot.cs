using UnityEngine;
using ZestCore.Utility;

namespace SuperheroIdle
{
    public class TakeScreenshot : MonoBehaviour
    {
        private void Update()
        {
            Screenshot.TakeAScreenshot();
        }
    }
}
