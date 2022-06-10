using System.Collections.Generic;

namespace ZestGames
{
    public static class CharacterTracker
    {
        private static List<Ai> aisInScene;
        public static List<Ai> AIsInScene => aisInScene == null ? aisInScene = new List<Ai>() : aisInScene;

        public static void AddAi(Ai ai)
        {
            if (!AIsInScene.Contains(ai))
                AIsInScene.Add(ai);
        }

        public static void RemoveAi(Ai ai)
        {
            if (AIsInScene.Contains(ai))
                AIsInScene.Remove(ai);
        }
    }
}
