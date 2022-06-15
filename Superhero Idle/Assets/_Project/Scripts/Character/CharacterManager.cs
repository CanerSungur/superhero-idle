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

        #region POLICE CARS
        private static List<PoliceCar> _policeCarsInScene;
        public static List<PoliceCar> PoliceCarsInScene => _policeCarsInScene == null ? _policeCarsInScene = new List<PoliceCar>() : _policeCarsInScene;

        public static void AddPoliceCar(PoliceCar policeCar)
        {
            if (!PoliceCarsInScene.Contains(policeCar))
                PoliceCarsInScene.Add(policeCar);
        }

        public static void RemovePoliceCar(PoliceCar policeCar)
        {
            if (PoliceCarsInScene.Contains(policeCar))
                PoliceCarsInScene.Remove(policeCar);
        }
        #endregion
    }
}
