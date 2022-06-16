using UnityEngine;
using System.Collections.Generic;

namespace SuperheroIdle
{
    public class PoliceManager : MonoBehaviour
    {
        #region STATIC CAR LIST SYSTEM
        public static List<PoliceCar> _freePoliceCars = new List<PoliceCar>();
        public static PoliceCar GetNextFreePoliceCar()
        {
            if (_freePoliceCars.Count == 0 || _freePoliceCars == null)
                return null;
            else
                return _freePoliceCars[0];
        }

        public static void AddFreePoliceCar(PoliceCar policeCar)
        {
            if (!_freePoliceCars.Contains(policeCar))
                _freePoliceCars.Add(policeCar);
        }

        public static void RemoveFreePoliceCar(PoliceCar policeCar)
        {
            if (_freePoliceCars.Contains(policeCar))
                _freePoliceCars.Remove(policeCar);
        }
        #endregion
    }
}
