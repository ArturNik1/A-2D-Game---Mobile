using UnityEngine;
using System.Collections;

namespace RASCAL {
    public class HandExample : MonoBehaviour {

        public GameObject ballSpawner;
        public GameObject pSys;

        public void SwitchToBalls() {
            pSys.SetActive(false);
            ballSpawner.SetActive(true);
        }
        public void SwitchToParticles() {
            pSys.SetActive(true);
            ballSpawner.SetActive(false);
        }
    }
}
