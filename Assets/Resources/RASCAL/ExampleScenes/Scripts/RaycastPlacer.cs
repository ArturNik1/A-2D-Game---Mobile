using UnityEngine;
using System.Collections;

namespace RASCAL {
    public class RaycastPlacer : MonoBehaviour {

        public Transform PlaceObj;

        public delegate void HitEvent(RaycastHit hit);
        public HitEvent OnHit;

        void Update() {
            if (Input.GetMouseButton(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {
                    PlaceObj.SetParent(hit.transform);
                    PlaceObj.rotation = Quaternion.LookRotation(hit.normal);
                    PlaceObj.position = hit.point;
                    PlaceObj.localScale = Vector3.one;

                    if(OnHit != null) {
                        OnHit(hit);
                    }
                }
            }

        }
    }
}