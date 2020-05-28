using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace RASCAL {
    public class UnityChanExample : MonoBehaviour {

        public Text meshGenText;
        public Text updateText;

        public Text yieldText;
        public Text passText;
        public Text materialText;

        public RASCALSkinnedMeshCollider rascal;
        public Shader matShader;


        RaycastPlacer rayPlacer;

        Color GetRndClr() {
            return new Color(Random.value, Random.value, Random.value) * 1.5f;
        }

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        private void Awake() {
            rayPlacer = GetComponent<RaycastPlacer>();

            ProcessMesh();
            UpdateBoneMeshes();
        }

        List<MeshRenderer> collMeshDisplayRends = new List<MeshRenderer>();


        private void Start() {
            foreach (var skinfo in rascal.skinfos) {
                foreach (var bone in skinfo.bones) {
                    foreach (var boneMesh in bone.boneMeshes) {
                        GameObject meshChild = new GameObject("boneMesh demo");
                        meshChild.transform.SetParent(bone.transform);
                        meshChild.transform.localPosition = Vector3.zero;
                        meshChild.transform.localEulerAngles = Vector3.zero;
                        meshChild.transform.localScale = Vector3.one;

                        MeshFilter meshFilter = meshChild.AddComponent<MeshFilter>();
                        meshFilter.sharedMesh = boneMesh.collMesh;

                        MeshRenderer meshRenderer = meshChild.AddComponent<MeshRenderer>();
                        meshRenderer.sharedMaterial = new Material(matShader);
                        meshRenderer.sharedMaterial.color = GetRndClr();
                        meshRenderer.enabled = false;

                        collMeshDisplayRends.Add(meshRenderer);
                    }
                }
            }

            rascal.OnAsyncUpdateYield += UpdateYieldText;
            rascal.OnAsyncPassComplete += UpdatePassText;

            rayPlacer.OnHit += UpdateMaterialText;
        }

        private void UpdateYieldText(double time) {
            yieldText.text = "Async Update Yield Time: " + time.ToString("F3");
        }
        private void UpdatePassText(double time) {
            passText.text = "Async Update Total Pass Time: " + time.ToString("F3");
        }

        private void UpdateMaterialText(RaycastHit hit) {
            Material mat;
            if (mat = GetMaterial(hit.collider))
                materialText.text = "Raycasted Material: " + mat.name;
            else
                materialText.text = "Raycasted Material: -";
        }

        Material GetMaterial(Collider collider) {
            foreach (var skinfo in rascal.skinfos) {
                foreach (var bone in skinfo.bones) {
                    foreach (var boneMesh in bone.boneMeshes) {
                        if (boneMesh.meshCol == collider) {
                            return skinfo.skinnedMesh.sharedMaterials[boneMesh.skinnedMeshMaterialIndex];
                        }
                    }
                }
            }

            return null;
        }

        bool meshColVisibleState = false;
        public void ShowMeshCol(Text btnText) {
            meshColVisibleState = !meshColVisibleState;

            if (meshColVisibleState) {
                btnText.text = "Show Skinned Mesh";
            } else {
                btnText.text = "Show Collision Mesh";
            }

            var skins = rascal.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var skin in skins) {
                skin.enabled = !meshColVisibleState;
            }

            foreach (var meshRend in collMeshDisplayRends) {
                meshRend.enabled = meshColVisibleState;
            }
        }


        public void ProcessMesh() {
            rascal.CleanUpMeshes();

            sw.Restart();
            rascal.ProcessMesh();
            double time = sw.LapTime();
            meshGenText.text = "Mesh processed in: " + time.ToString("F2") + "ms";
        }

        public void UpdateBoneMeshes() {
            sw.Restart();
            rascal.ImmediateUpdateColliders(true);
            double time = sw.LapTime();
            updateText.text = "Bone-Meshes updated in: " + time.ToString("F2") + "ms";
        }

        bool updatingState = false;
        public void SetUpdating(Text btnText) {
            updatingState = !updatingState;

            if (updatingState) {
                btnText.text = "Stop Async Updating";
                rascal.StartAsyncUpdating(true);
            } else {
                btnText.text = "Start Async Updating";
                rascal.StopAsyncUpdating();
            }
        }

    }
}