using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RASCAL {
    public class SimpSpawner : MonoBehaviour {

        public float delay = 0.1f;
        public float lifeTime = 0.1f;
        public Vector3 force;
        public Vector3 pos;
        public GameObject prefab;

        Transform spawns;

        private void Start() {
            //spawns = new GameObject("Objects").transform;
            spawns = transform;

            Invoke("Spawn", delay);
        }

        void Spawn() {
            if (enabled && gameObject.activeInHierarchy) {
                GameObject inst = Instantiate(prefab, spawns.position + RandVec(pos), transform.rotation) as GameObject;
                inst.transform.SetParent(spawns);

                Rigidbody rb; if(rb = inst.GetComponent<Rigidbody>()) rb.AddForce(force);

                inst.AddComponent<DestroyAfter>().Init(lifeTime);
            }

            Invoke("Spawn", delay);
        }


        public class DestroyAfter : MonoBehaviour {

            public void Init(float time) {
                Invoke("Kill", time);
            }

            void Kill() {
                Destroy(gameObject);
            }
        }


        Vector3 RandVec(Vector3 vec) {
            return new Vector3(Random.Range(-vec.x, vec.x), Random.Range(-vec.y, vec.y), Random.Range(-vec.y, vec.y));
        }

    }

}