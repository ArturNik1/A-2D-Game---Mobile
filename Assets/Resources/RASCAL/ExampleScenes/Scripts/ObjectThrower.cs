using UnityEngine;
using System.Collections;

namespace RASCAL {
    public class ObjectThrower : MonoBehaviour {

        [Header("Throw Settings:")]
        public float throwforce = 5f;
        public float lifetime = 5f;
        public int throwRate = 1;
        public GameObject[] throwables;

        GameObject mainCam;
        GameObject oParent;

        // Use this for initialization
        void Start() {
            mainCam = GameObject.FindGameObjectWithTag("MainCamera");
            oParent = new GameObject();
            oParent.name = "Throwables";
            Time.fixedDeltaTime = 0.01f;
        }

        int tt = 0;
        void Update() {
            tt += throwRate;
            if (Input.GetMouseButton(2) && tt > 60) {
                tt = 0;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {
                    Vector3 launchpos = ray.origin + -Camera.main.transform.up;
                    GameObject throwable = throwables[Random.Range(0, throwables.Length)];
                    GameObject throwInst = Instantiate(throwable, launchpos, mainCam.transform.rotation) as GameObject;
                    throwInst.AddComponent<SimpSpawner.DestroyAfter>().Init(lifetime);
                    Rigidbody tmpRB = throwInst.GetComponent<Rigidbody>();
                    tmpRB.AddForce((hit.point - launchpos).normalized * throwforce);
                    throwInst.transform.SetParent(oParent.transform);

                    Renderer tR = throwInst.GetComponent<Renderer>();
                    tR.material = Instantiate(tR.material) as Material;
                    tR.material.color = RandomColor();
                }
            }
        }

        Color RandomColor() {
            float r = Random.Range(0.0f, 1.0f);
            float g = Random.Range(0.0f, 1.0f);
            float b = Random.Range(0.0f, 1.0f);
            return new Color(r, g, b);
        }

        //END CLASS
    }
}