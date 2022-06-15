using UnityEngine;
using System;

namespace SuperheroIdle
{
    public class PoliceManager : MonoBehaviour
    {
        public static Action<Criminal> OnTakeCriminal;

        private void Start()
        {
            foreach (PoliceCar policeCar in CharacterManager.PoliceCarsInScene)
                policeCar.gameObject.SetActive(false);

            OnTakeCriminal += SendPolice;
        }

        private void OnDisable()
        {
            OnTakeCriminal -= SendPolice;
        }

        private void SendPolice(Criminal criminal)
        {
            Debug.Log(CharacterManager.PoliceCarsInScene.Count);
            foreach (PoliceCar policeCar in CharacterManager.PoliceCarsInScene)
            {
                if (policeCar.Idle || !policeCar.gameObject.activeSelf)
                {
                    policeCar.gameObject.SetActive(true);
                    policeCar.transform.position = new Vector3(0,0, -5f);
                    policeCar.SetCriminalTarget(criminal);
                    break;
                }
                else
                    Debug.Log("Police car not available!");
            }
        }
    }
}
