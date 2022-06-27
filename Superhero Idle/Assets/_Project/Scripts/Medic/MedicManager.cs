using UnityEngine;
using System.Collections.Generic;

namespace SuperheroIdle
{
    public class MedicManager : MonoBehaviour
    {
        #region STATIC CAR LIST SYSTEM
        public static List<Ambulance> _freeAmbulances = new List<Ambulance>();
        public static Ambulance GetNextFreeAmbulance()
        {
            if (_freeAmbulances.Count == 0 || _freeAmbulances == null)
                return null;
            else
                return _freeAmbulances[0];
        }

        public static void AddFreeAmbulance(Ambulance ambulance)
        {
            if (!_freeAmbulances.Contains(ambulance))
                _freeAmbulances.Add(ambulance);
        }

        public static void RemoveFreeAmbulance(Ambulance ambulance)
        {
            if (_freeAmbulances.Contains(ambulance))
                _freeAmbulances.Remove(ambulance);
        }
        #endregion
    }
}
