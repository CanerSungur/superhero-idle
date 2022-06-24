using System.Collections.Generic;
using UnityEngine;

namespace SuperheroIdle
{
    public static class CharacterManager
    {
        public static Transform PlayerTransform { get; private set; }
        public static void SetPlayerTransform(Transform transform) => PlayerTransform = transform;

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

        #region ATMS
        private static List<ATM> _atmsInScene;
        public static List<ATM> AtmsInScene => _atmsInScene == null ? _atmsInScene = new List<ATM>() : _atmsInScene;

        public static void AddATM(ATM atm)
        {
            if (!AtmsInScene.Contains(atm))
                AtmsInScene.Add(atm);
        }

        public static void RemoveATM(ATM atm)
        {
            if (AtmsInScene.Contains(atm))
                AtmsInScene.Remove(atm);
        }
        #endregion
    }
}
