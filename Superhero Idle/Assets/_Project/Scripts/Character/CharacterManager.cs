using System.Collections.Generic;

namespace SuperheroIdle
{
    public static class CharacterManager
    {
        #region CIVILLIANS
        private static List<Civillian> _civilliansInScene;
        public static List<Civillian> CivilliansInScene => _civilliansInScene == null ? _civilliansInScene = new List<Civillian>() : _civilliansInScene;

        public static void AddCivillian(Civillian civillian)
        {
            if (!CivilliansInScene.Contains(civillian))
                CivilliansInScene.Add(civillian);
        }

        public static void RemoveCivillian(Civillian civillian)
        {
            if (CivilliansInScene.Contains(civillian))
                CivilliansInScene.Remove(civillian);
        }
        #endregion

        #region CRIMINALS
        private static List<Criminal> _criminalsInScene;
        public static List<Criminal> CriminalsInScene => _criminalsInScene == null ? _criminalsInScene = new List<Criminal>() : _criminalsInScene;

        public static void AddCriminal(Criminal criminal)
        {
            if (!CriminalsInScene.Contains(criminal))
                CriminalsInScene.Add(criminal);
        }

        public static void RemoveCriminal(Criminal criminal)
        {
            if (CriminalsInScene.Contains(criminal))
                CriminalsInScene.Remove(criminal);
        }
        #endregion
    }
}
