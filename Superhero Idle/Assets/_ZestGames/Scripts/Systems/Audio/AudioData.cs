using ZestCore.MonoBehaviours;

namespace ZestGames
{
    public class AudioData : Singleton<AudioData>
    {
        public Audio[] Audios;

        private void Awake() => this.Reload();
    }
}
