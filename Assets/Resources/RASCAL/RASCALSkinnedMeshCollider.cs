using System.Collections;
using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using RASCAL;

[AddComponentMenu("Physics/RASCAL Skinned Mesh Collider", 0)]
[ExecuteInEditMode]
public class RASCALSkinnedMeshCollider : MonoBehaviour {

    [Header("Generation")]

    [Tooltip("This will (re)generate all necessary data to create the bone-meshes used in collision and also generate some initial collision shapes when the game starts.")]
    public bool generateOnStart = false;

    [Tooltip("This will enable continuous asynchronous updating of collision shapes when the game starts. You may find that the initial collision shapes work well enough " +
        "for you and you dont even need to update them which would save on performance for sure, but if the mesh deforms due to its bones being moved the shapes wont match " +
        "and could cause some pretty big inaccuracies. Alternatively, if you know your mesh will only need to be updated at certain points, you can call either immediate or " +
        "asynchronous reconstruction of the collision shapes manually via the provided functions in the script.")]
    public bool enableUpdatingOnStart = false;

    [Tooltip("This generates collision on startup immediately, rather than using the asynchronous method which could take second or two to fully generate. " +
        "This obviously comes at the cost of taking longer to generate on start, which is why its recommended to just pre-generate everything rather than on startup.")]
    public bool immediateStartupCollision = false;

    [Tooltip("When enabled, only unique triangles will be used between all the bone-meshes. This prevents mesh overlapping but because of how triangles are chosen " +
        "without any care for which bone the triangle is more significant to, this can lead to messy bone-meshes that may impact results of certain collisions. It's bit faster and uses less memory, " +
        "this option should probably be on unless you notice some otherwise bad collision meshes being generated as a result of it.")]
    public bool onlyUniqueTriangles = true;

    [Tooltip("Enable the convex setting of the mesh colliders. This option obviously leads to inaccuracies in the overall mesh and you may need to lower the max polygons per mesh to avoid errors, " +
        "but convex meshes should allow for non-kinematic rigid bodies to be used if that's something that you need.")]
    public bool convexMeshColliders = false;

    [Tooltip("Splits each collision mesh up by material. For example if your skinned mesh has 2 materials it will create 1 collider for all triangles with the first material, and another for the second material. " +
        "This is useful and required mostly for applying different physics materials to the colliders based on material or excluding mesh parts based on material. To do that, use the material association list and " +
        "the exclusion list.")]
    public bool splitCollisionMeshesByMaterial = false;

    [Tooltip("You almost certainly don't need this. But it's included just in case. It basically makes it so meshes with no bones and only blendshapes get transformed differently. " +
        "But it should be fine by default, this should be a last resort troubleshooting step.")]
    public bool zeroBoneMeshAlternateTransform = false;

    [Tooltip("Clears ALL mesh colliders under the component when calling the clear function, not just colliders currently associated with this component. Be careful with this.")]
    public bool clearAllMeshColliders = false;


    [Tooltip("Material to apply to the mesh colliders. If you need more granular control for materials on what bones youll need to add a RASCAL Phys Material Properties component to the bone transform or skinned-mesh transform. " +
        "The priority goes from highest to lowest: Material-Association-List -> Bone-Transform -> SkinnedMesh-Transform -> PhysicsMaterial-Variable")]
    public PhysicMaterial physicsMaterial = null;

    [Tooltip("The maximum number of triangles/polygons to allow in each collision shape. The mesh will be split up into multiple mesh colliders if it contains more than this amount. " +
        "Splitting large meshes up can help with asynchronous collision generate frame rate stability as large meshes may cause hitching when updating mesh colliders. " +
        "Splitting the mesh up however can very slightly make it take longer to update all collision shapes, this is probably negligible though.")]
    public int maxColliderTriangles = 1000;

    [Tooltip("When generating colliders, the bone weight for the vertex must be above this for the vertex to be added to the collider.")]
    public float boneWeightThreshold = 0f;

    [Tooltip("Use this list to associate materials in the skinned mesh(es) with physics materials that will be added to the mesh colliders. If your mesh has multiple materials make sure to check the option to " +
        "split the collision meshes by material.")]
    public List<PhysicsMaterialAssociation> materialAssociationList = new List<PhysicsMaterialAssociation>();

    [Header("Updating")]

    [Tooltip("Amount of time in milliseconds the asynchronous generation should be allowed to run per frame while idling. It is idling when the mesh isnt changing enough " +
        "to warrant rebuilding the collision shapes. (this setting doesn't affect immediate updating)")]
    public double idleCpuBudget = 0.2;

    [Tooltip("Amount of time in milliseconds the asynchronous generation should be allowed to run per frame while active. It is active when any of the collision shapes are actively being rebuilt. " +
        "This allows more time to rebuild collision shapes which means they will update faster at the cost of performance. (this setting doesn't affect immediate updating)")]
    public double activeCpuBudget = 1;

    [Tooltip("An amount by which the bone mesh needs to change in order for its collision to be rebuilt. The purpose of this is to allow for slight changes " +
        "in the mesh before comepletely rebuilding, which slightly improves performance at the cost of accuracy. The value for this should likely be quite small but you should play around with it to see what works best for you.")]
    public float meshUpdateThreshold = 0.02f;
    private float updateThreshold;


    [Serializable]
    public class PhysicsMaterialAssociation {
        public Material material;
        public PhysicMaterial physicsMaterial;
    }

    public List<SkinnedMeshRenderer> excludedSkins = new List<SkinnedMeshRenderer>();
    public List<Transform> excludedBones = new List<Transform>();
    public List<Material> excludedMaterials = new List<Material>();


    [HideInInspector]
    public Skinfo[] skinfos = null;

    public bool noMeshData { get { return skinfos == null || skinfos.Length == 0; } }



    private void Start() {
        if (serialized) {
            RebuildFromSerialized();
        }

        if (Application.isPlaying) {
            if (generateOnStart) {
                ProcessMesh();

                if (immediateStartupCollision) {
                    ImmediateUpdateColliders(true);

                    if (enableUpdatingOnStart) {
                        StartAsyncUpdating(true);
                    }
                } else {
                    StartAsyncUpdating(enableUpdatingOnStart);
                }
            } else if (!noMeshData) {
                SetBoneParents();
                if (enableUpdatingOnStart) {
                    StartAsyncUpdating(true);
                }
            }
        }
    }


    /// <summary>
    /// Process the skinned mesh(es) to calculate required data and build the data structure
    /// </summary>
    public void ProcessMesh() {
        CleanUpMeshes();

        HashSet<SkinnedMeshRenderer> skins = new HashSet<SkinnedMeshRenderer>();
        SkinnedMeshRenderer[] childSkins = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer skin in childSkins) skins.Add(skin);

        SkinnedMeshRenderer thisSkin; if (thisSkin = GetComponent<SkinnedMeshRenderer>()) skins.Add(thisSkin);

        //var skins = new SkinnedMeshRenderer[] { GetComponentsInChildren<SkinnedMeshRenderer>()[10] };
        skinfos = skins.Where(skin => !excludedSkins.Contains(skin)).Select(skin => new Skinfo() { skinnedMesh = skin, host = this }.Init()).ToArray();
        if (skinfos.Length == 0) {
            Debug.Log("No skinned meshes were found under object: " + gameObject);
        }
    }

    /// <summary>
    /// Immediately updates all colliders.
    /// </summary>
    /// <param name="force">If true, will override the meshUpdateThreshold and force the update.</param>
    public void ImmediateUpdateColliders(bool force = false) {
        if (noMeshData) {
            Debug.Log("No skinned collision mesh data found on " + this.ToString() + ". Processing mesh...");
            ProcessMesh();
        }

        updateThreshold = meshUpdateThreshold * meshUpdateThreshold;
        foreach (var skin in skinfos) skin.Update(force);
    }



    double timerAcc = 0;
    double totalTimeAcc = 0;
    System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();

    public delegate void RASCALTimedEvent(double time);
    public event RASCALTimedEvent OnAsyncUpdateYield;
    public event RASCALTimedEvent OnAsyncPassComplete;

    private void AddTime() {
        timerAcc += timer.LapTime();
    }

    private bool CheckFrameYield(double budget) {
        AddTime();
        return timerAcc > budget;
    }

    //double avgCpuDeficit = 0;
    //const double asyncI = 350;
    //const double asyncI2 = asyncI + 1;

    private void ResetTimer(bool doEvent = true) {
        timer.Restart();

        //some weird code measuring how overbudget we went on calculations
        //resetting the timer accumulator to the deficit may help stabilize frame times but im not sure
        //also some stuff for calculating a rolling average of the deficit for diagnostic purposes

        //double deficit = timerAcc - cpuBudget;
        //avgCpuDeficit = (avgCpuDeficit * asyncI + deficit) / asyncI2;
        //timerAcc = deficit;

        if (OnAsyncUpdateYield != null) OnAsyncUpdateYield(timerAcc);

        totalTimeAcc += timerAcc;
        timerAcc = 0;
    }

    [HideInInspector]
    public bool asyncUpdating = false;

    private Coroutine updatingCoroutine;
    /// <summary>
    /// Starts the asynchronous updating routine, using the CPU budget values.
    /// </summary>
    /// <param name="continuous">If true, will run continuously until stopped, otherwise only runs once.</param>
    /// <returns>A reference to the started coroutine.</returns>
    public Coroutine StartAsyncUpdating(bool continuous) {
        if (noMeshData) {
            Debug.Log("No skinned collision mesh data found on " + this.ToString() + ". Make sure you process the mesh first!");
            return null;
        } else {
            StopAsyncUpdating();
            updatingCoroutine = StartCoroutine(AsynchronousUpdate(continuous));
            return updatingCoroutine;
        }
    }

    /// <summary>
    /// Stops the asynchronous updating coroutine.
    /// </summary>
    public void StopAsyncUpdating() {
        if (updatingCoroutine != null) {
            StopCoroutine(updatingCoroutine);
        }
        asyncUpdating = false;
    }

    private IEnumerator AsynchronousUpdate(bool continuous) {
        asyncUpdating = true;

        do {
            if (enabled) {
                updateThreshold = meshUpdateThreshold * meshUpdateThreshold;

                ResetTimer();

                foreach (Skinfo skinfo in skinfos) {
                    skinfo.skinnedMesh.BakeMesh(skinfo.bakedMesh);
                    skinfo.UpdateRootMatrix();

                    foreach (BoneInfo bone in skinfo.bones) {
                        bone.cachedMatrix = bone.transform.worldToLocalMatrix;
                    }

                    if (CheckFrameYield(idleCpuBudget)) { yield return new WaitForFixedUpdate(); ResetTimer(); }

                    Vector3[] bakedVerts = skinfo.bakedMesh.vertices;

                    foreach (BoneInfo bone in skinfo.bones) {
                        foreach (BoneMeshColl boneMesh in bone.boneMeshes) {

                            Vector3[] newVerts = boneMesh.TransformVertices(bakedVerts);

                            if (boneMesh.PastThreshold(newVerts)) {
                                boneMesh.collMesh.vertices = newVerts;

                                if (CheckFrameYield(activeCpuBudget)) { yield return new WaitForFixedUpdate(); ResetTimer(); }

                                boneMesh.meshCol.sharedMesh = boneMesh.collMesh;
                            } else {
                                if (CheckFrameYield(idleCpuBudget)) { yield return new WaitForFixedUpdate(); ResetTimer(); }
                            }
                        }
                    }
                }

                if (CheckFrameYield(idleCpuBudget)) { yield return new WaitForFixedUpdate(); ResetTimer(); }

                AddTime();
                totalTimeAcc += timerAcc;
                if (OnAsyncPassComplete != null) OnAsyncPassComplete(totalTimeAcc);
                timerAcc = 0; totalTimeAcc = 0;

            } else {
                yield return new WaitForFixedUpdate();
            }

        } while (asyncUpdating && continuous);

        asyncUpdating = false;
    }



    void SetBoneParents() {
        foreach (var skin in skinfos) {
            skin.bakedMesh = new Mesh();
            foreach (var bone in skin.bones) {
                bone.parentSkinfo = skin;
                foreach (var boneMesh in bone.boneMeshes) {
                    boneMesh.parentBone = bone;
                }
            }
        }
    }

    //you probably dont want this bit on but it might be useful if you find yourself in a pickel
    /// <summary>
    /// Removes ALL mesh collider components under the RASCAL component. Perhaps useful for cleaning up colliders created by glitch or mistake.
    /// </summary>
    public void CleanUpAllMeshColliders() {
        MeshCollider[] mcs = GetComponentsInChildren<MeshCollider>();
        foreach (var mc in mcs) {
            DestroyImmediate(mc);
        }
    }

    /// <summary>
    /// Cleans up colliders and other data associated with this RASCAL component.
    /// </summary>
    public void CleanUpMeshes() {

        if (noMeshData) {
            if (clearAllMeshColliders) CleanUpAllMeshColliders();
            return;
        }

        if (updatingCoroutine != null) {
            asyncUpdating = false;
            StopCoroutine(updatingCoroutine);
        }

        UnserializeForPrefab();

        foreach (var skin in skinfos) {
            DestroyImmediate(skin.bakedMesh);
            foreach (var bone in skin.bones) {
                foreach (var boneMesh in bone.boneMeshes) {
                    DestroyImmediate(boneMesh.collMesh);
                    DestroyImmediate(boneMesh.meshCol, true);
                }
                bone.boneMeshes = null;
            }
            skin.bones = null;
        }
        skinfos = null;

        if (clearAllMeshColliders) CleanUpAllMeshColliders();
    }

    [HideInInspector] [SerializeField] private bool _serialized = false;
    public bool serialized { get { return _serialized; } }

    /// <summary>
    /// Serializes all collision mesh data so that it can be stored in a prefab.
    /// DO NOT use this unless you absolutely need to have the mesh data serialized.
    /// </summary>
    public void SerializeForPrefab() {
        if (serialized)
            return;

        foreach (var skin in skinfos) {
            foreach (var bone in skin.bones) {
                foreach (var boneMesh in bone.boneMeshes) {
                    boneMesh.SerializeForPrefab();
                }
            }
        }

        _serialized = true;
    }

    /// <summary>
    /// Removes all serialized collision mesh data.
    /// </summary>
    public void UnserializeForPrefab() {
        if (!serialized)
            return;

        foreach (var skin in skinfos) {
            foreach (var bone in skin.bones) {
                foreach (var boneMesh in bone.boneMeshes) {
                    boneMesh.UnserializeForPrefab();
                }
            }
        }

        _serialized = false;
    }

    private void RebuildFromSerialized() {
        SetBoneParents();
        foreach (var skin in skinfos) {
            skin.bakedMesh = new Mesh();
            foreach (var bone in skin.bones) {
                foreach (var boneMesh in bone.boneMeshes) {
                    boneMesh.RebuildFromSerialized();
                }
            }
        }
    }


    [Serializable]
    public class Skinfo {
        public List<BoneInfo> bones;
        public Mesh bakedMesh;
        public SkinnedMeshRenderer skinnedMesh;
        public RASCALSkinnedMeshCollider host;
        public RASCALPhysMaterialProperties materialProperties;
        public Matrix4x4 meshRootMatrix = new Matrix4x4();
        public bool noBones = false;

        internal Skinfo Init() {
            bakedMesh = new Mesh();
            materialProperties = skinnedMesh.GetComponent<RASCALPhysMaterialProperties>();

            if (skinnedMesh.bones.Length == 0) {
                noBones = true;
                BoneInfo tBone = new BoneInfo() { srcSkin = skinnedMesh, transform = skinnedMesh.transform, host = host, parentSkinfo = this }.Init();
                for (int v = 0; v < skinnedMesh.sharedMesh.vertexCount; v++) {
                    tBone.affectedVerts.Add(v);
                }

                bones = new List<BoneInfo>() { tBone };
            } else {
                List<BoneInfo> tBones = skinnedMesh.bones.Select(x => new BoneInfo() {
                    srcSkin = skinnedMesh, transform = x, host = host, parentSkinfo = this
                }.Init()).ToList();

                BoneInfo tBone;

                BoneWeight[] weights = skinnedMesh.sharedMesh.boneWeights;
                //v is vertex index in the mesh
                for (int v = 0; v < weights.Length; v++) {
                    var weight = weights[v];
                    Weight wInfo = new Weight[] {
                        new Weight() {weight = weight.weight0, boneIndex = weight.boneIndex0 },
                        new Weight() {weight = weight.weight1, boneIndex = weight.boneIndex1 },
                        new Weight() {weight = weight.weight2, boneIndex = weight.boneIndex2 },
                        new Weight() {weight = weight.weight3, boneIndex = weight.boneIndex3 },
                    }.OrderBy(x => x.weight).Last();

                    tBone = tBones[wInfo.boneIndex];

                    if (tBone.materialProperties) {
                        if (wInfo.weight > tBone.materialProperties.boneWeightThreshold) {
                            tBone.affectedVerts.Add(v);
                        }
                    } else if (materialProperties) {
                        if (wInfo.weight > materialProperties.boneWeightThreshold) {
                            tBone.affectedVerts.Add(v);
                        }
                    } else if (wInfo.weight > host.boneWeightThreshold) {
                        tBone.affectedVerts.Add(v);
                    }
                }

                bones = tBones.Where(b => b.affectedVerts.Count > 0 && !host.excludedBones.Contains(b.transform)).ToList();
            }


            List<int> skinTriList;
            int subMeshIdx = 0;
            int subMeshCount = skinnedMesh.sharedMesh.subMeshCount;

            boneStart:
            if (host.splitCollisionMeshesByMaterial) {
                skinTriList = skinnedMesh.sharedMesh.GetTriangles(subMeshIdx).ToList();
            } else {
                skinTriList = skinnedMesh.sharedMesh.triangles.ToList();
            }

            if (!host.excludedMaterials.Contains(skinnedMesh.sharedMaterials[subMeshIdx])) {
                foreach (BoneInfo boneInfo in bones) {
                    List<int> listTris = new List<int>();
                    for (int t = 0; t < skinTriList.Count;) {
                        int[] triSet = new int[] { skinTriList[t++], skinTriList[t++], skinTriList[t++] };

                        if (boneInfo.affectedVerts.Contains(triSet[0]) || boneInfo.affectedVerts.Contains(triSet[1]) || boneInfo.affectedVerts.Contains(triSet[2])) {
                            if (host.onlyUniqueTriangles) {
                                t -= 3;
                                skinTriList.RemoveRange(t, 3);
                            }

                            listTris.AddRange(triSet);
                        }
                    }
                    if (listTris.Count == 0) {
                        continue;
                    }

                    int polyCount = listTris.Count / 3;
                    BoneMeshColl boneMesh = new BoneMeshColl() { parentBone = boneInfo, host = host, skinnedMeshMaterialIndex = subMeshIdx }.Init();
                    boneInfo.boneMeshes.Add(boneMesh);

                    if (polyCount > host.maxColliderTriangles) {
                        int meshGroups = Mathf.CeilToInt((float)polyCount / host.maxColliderTriangles);

                        boneMesh.SetTris(listTris.Take(host.maxColliderTriangles * 3));

                        for (int i = 1; i < meshGroups; i++) {
                            BoneMeshColl tBoneMesh = new BoneMeshColl() { parentBone = boneInfo, host = host }.Init();

                            tBoneMesh.SetTris(listTris.Skip(i * host.maxColliderTriangles * 3).Take(host.maxColliderTriangles * 3));
                            boneInfo.boneMeshes.Add(tBoneMesh);
                        }
                    } else {
                        boneMesh.SetTris(listTris);
                    }
                }
            }

            if (host.splitCollisionMeshesByMaterial && subMeshIdx < subMeshCount - 1) {
                subMeshIdx++;
                goto boneStart;
            }

            bones.RemoveAll(b => b.boneMeshes.Count == 0);

            foreach (BoneInfo bone in bones) {
                bone.affectedVerts = null;
            }

            return this;
        }

        public void UpdateRootMatrix() {
            meshRootMatrix.SetTRS(skinnedMesh.transform.position, skinnedMesh.transform.rotation, Vector3.one);
        }

        public void Update(bool force = false) {
            skinnedMesh.BakeMesh(bakedMesh);

            Vector3[] verts = bakedMesh.vertices;

            UpdateRootMatrix();
            foreach (BoneInfo bone in bones) {
                bone.Update(verts, force);
            }
        }
    }

    struct Weight {
        public int boneIndex;
        public float weight;
    }

    [Serializable]
    public class BoneInfo {
        public Transform transform;
        public Matrix4x4 cachedMatrix;
        public SkinnedMeshRenderer srcSkin;
        public HashSet<int> affectedVerts = new HashSet<int>();
        public List<BoneMeshColl> boneMeshes = new List<BoneMeshColl>();
        public RASCALSkinnedMeshCollider host;
        public RASCALPhysMaterialProperties materialProperties;

        [NonSerialized]
        public Skinfo parentSkinfo;

        public BoneInfo Init() {
            materialProperties = transform.GetComponent<RASCALPhysMaterialProperties>();
            return this;
        }

        public void Update(Vector3[] verts, bool force = false) {
            cachedMatrix = transform.worldToLocalMatrix;
            foreach (BoneMeshColl boneMesh in boneMeshes) {
                boneMesh.Update(verts, force);
            }
        }
    }

    [Serializable]
    public class BoneMeshColl {
        public Mesh collMesh;
        public MeshCollider meshCol;
        public int[] tris;
        public int vertexCount { get { return distinctVerts.Length; } }
        public int[] distinctVerts;
        public RASCALSkinnedMeshCollider host;
        public int skinnedMeshMaterialIndex;

        [NonSerialized]
        public BoneInfo parentBone;

        internal BoneMeshColl Init() {
            collMesh = new Mesh();

            meshCol = parentBone.transform.gameObject.AddComponent<MeshCollider>();
            meshCol.convex = host.convexMeshColliders;

            HandlePhysMatInheritence();
            return this;
        }

        void HandlePhysMatInheritence() {
            Material skinMat = parentBone.srcSkin.sharedMaterials[skinnedMeshMaterialIndex];
            PhysicsMaterialAssociation materialMatch = host.materialAssociationList.Find(m => m.material == skinMat);

            if (materialMatch != null) {
                meshCol.sharedMaterial = materialMatch.physicsMaterial;
            }

            if (parentBone.materialProperties) {
                if (!meshCol.sharedMaterial || parentBone.materialProperties.overrideOthers) {
                    meshCol.sharedMaterial = parentBone.materialProperties.physicsMaterial;
                }
            }

            if (parentBone.parentSkinfo.materialProperties) {
                if (!meshCol.sharedMaterial || parentBone.parentSkinfo.materialProperties.overrideOthers) {
                    meshCol.sharedMaterial = parentBone.parentSkinfo.materialProperties.physicsMaterial;
                }
            }

            if (parentBone.materialProperties && parentBone.materialProperties.overrideOthers) {
                meshCol.sharedMaterial = parentBone.materialProperties.physicsMaterial;
            }

            if (!meshCol.sharedMaterial) {
                meshCol.sharedMaterial = host.physicsMaterial;
            }
        }

        public void SetTris(IEnumerable<int> inTriList) {
            tris = inTriList.ToArray();
            distinctVerts = tris.Distinct().ToArray();

            Dictionary<int, int> triMap = new Dictionary<int, int>(distinctVerts.Length);
            for (int i = 0; i < distinctVerts.Length; i++) {
                triMap.Add(distinctVerts[i], i);
            }

            for (int i = 0; i < tris.Length; i++) {
                tris[i] = triMap[tris[i]];
            }

            collMesh.vertices = new Vector3[vertexCount];
            collMesh.triangles = tris;

            CopyExtraMeshData();
        }

        internal void CopyExtraMeshData() {
            //if theres other mesh data you need to copy, do it here i guess
            Mesh srcMesh = parentBone.srcSkin.sharedMesh;

            for (int uvidx = 0; uvidx < 4; uvidx++) {
                List<Vector2> sUVs = new List<Vector2>();
                srcMesh.GetUVs(uvidx, sUVs);
                if (sUVs.Count == 0) {
                    continue;
                }
                var tUVs = new List<Vector2>();
                for (int i = 0; i < distinctVerts.Length; i++) tUVs.Add(sUVs[distinctVerts[i]]);
                collMesh.SetUVs(uvidx, tUVs);
            }

            Vector3[] sNorms = srcMesh.normals;
            Vector3[] tNorms = new Vector3[distinctVerts.Length];
            for (int i = 0; i < distinctVerts.Length; i++) tNorms[i] = sNorms[distinctVerts[i]];
            collMesh.normals = tNorms;
        }

        //serialization stuff
        [SerializeField] Vector2[] serializedUV1;
        [SerializeField] Vector2[] serializedUV2;
        [SerializeField] Vector2[] serializedUV3;
        [SerializeField] Vector2[] serializedUV4;
        [SerializeField] Vector3[] serializedVerticies;
        [SerializeField] Vector3[] serializedNormals;
        [SerializeField] int[] serializedTriangles;
        [SerializeField]
        //bool serialized = false;
        internal void SerializeForPrefab() {
            serializedUV1 = collMesh.uv;
            serializedUV2 = collMesh.uv2;
            serializedUV3 = collMesh.uv3;
            serializedUV4 = collMesh.uv4;

            serializedNormals = collMesh.normals;
            serializedVerticies = collMesh.vertices;
            serializedTriangles = collMesh.triangles;

            //serialized = true;
        }

        internal void UnserializeForPrefab() {
            serializedUV1 = null;
            serializedUV2 = null;
            serializedUV3 = null;
            serializedUV4 = null;
            serializedVerticies = null;
            serializedNormals = null;
            serializedTriangles = null;
            //serialized = false;
        }

        internal void RebuildFromSerialized() {
            if (collMesh) {
                DestroyImmediate(collMesh);
            }

            collMesh = new Mesh();
            collMesh.vertices = serializedVerticies;
            collMesh.triangles = serializedTriangles;
            collMesh.normals = serializedNormals;

            collMesh.uv = serializedUV1;
            collMesh.uv2 = serializedUV2;
            collMesh.uv3 = serializedUV3;
            collMesh.uv4 = serializedUV4;

            meshCol.sharedMesh = collMesh;
        }



        public bool PastThreshold(Vector3[] newVerts) {
            float totalDelta = 0;
            Vector3[] oldVerts = collMesh.vertices;
            for (int i = 0; i < distinctVerts.Length; i++) {
                totalDelta += (newVerts[i] - oldVerts[i]).sqrMagnitude;
            }
            return totalDelta >= host.updateThreshold;
        }

        /// <summary>
        /// Based on the actual mesh vertices, transforms the required verts for this bone into the bones coordinate space.
        /// </summary>
        /// <param name="actualVerts">Full list of vertices from the baked mesh.</param>
        /// <returns>List of vertices for this bone-mesh.</returns>
        internal Vector3[] TransformVertices(Vector3[] actualVerts) {
            Vector3[] tVerts = new Vector3[vertexCount];
            if (parentBone.parentSkinfo.noBones && !host.zeroBoneMeshAlternateTransform) {
                for (int i = 0; i < distinctVerts.Length; i++) tVerts[i] = actualVerts[distinctVerts[i]];
            } else {
                Matrix4x4 matrix = parentBone.cachedMatrix * parentBone.parentSkinfo.meshRootMatrix;
                for (int i = 0; i < distinctVerts.Length; i++) tVerts[i] = matrix.MultiplyPoint3x4(actualVerts[distinctVerts[i]]);
            }

            return tVerts;
        }

        public void Update(Vector3[] bakedVerts, bool force = false) {
            Vector3[] newVerts = TransformVertices(bakedVerts);
            bool thresh = force || PastThreshold(newVerts);

            if (thresh) {
                collMesh.vertices = newVerts;

                meshCol.sharedMesh = collMesh;
            }
        }
    }

}
