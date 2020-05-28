using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RASCAL {
    public class Sticky : MonoBehaviour {

        bool stuck = false;

        private void OnCollisionEnter(Collision collision) {
            if (!stuck) {
                if (!collision.transform.GetComponent<Sticky>()) {
                    stuck = true;
                    transform.position = collision.contacts[0].point;
                    transform.SetParent(collision.transform);
                    Destroy(GetComponent<Rigidbody>());
                }
            }
        }
    }
}
