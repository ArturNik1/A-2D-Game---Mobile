using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using UnityEngine.Diagnostics;

public class FieldOfViewEnemy : MonoBehaviour {

    public bool showMesh = false;
    #pragma warning disable CS0649
    [SerializeField] private LayerMask layerMask;
    #pragma warning restore CS0649
    private Mesh mesh;
    [Range (0, 360)][SerializeField] private float fov = 90;
    [SerializeField] private float viewDistance = 1;
    private Vector3 origin;
    private float startingAngle = 135;
    private MeshFilter meshFilter;

    GameObject player;

    private void Start() {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        origin = Vector3.zero;
    }

    private void LateUpdate() {
        int rayCount = 50;
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = transform.InverseTransformPoint(origin);

        int vertexIndex = 1;
        int triangleIndex = 0;
        bool rayHasPlayer = false;
        for (int i = 0; i <= rayCount; i++) {
            Vector3 vertex;
            RaycastHit raycastHit;
            Physics.Raycast(origin, UtilsClass.GetVectorFromAngle(angle), out raycastHit, viewDistance, layerMask);
            if (raycastHit.collider == null) {
                // No hit
                vertex = origin + UtilsClass.GetVectorFromAngle(angle) * viewDistance;
            } else {
                // Hit
                if (player == null) player = raycastHit.collider.transform.root.gameObject;
                rayHasPlayer = true;

                vertex = raycastHit.point;
            }
            vertices[vertexIndex] = transform.InverseTransformPoint(vertex);

            if (i > 0) {
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;
        }

        if (!rayHasPlayer) player = null;

        if (showMesh)
        {
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.bounds = new Bounds(origin, Vector3.one * 1000f);
        }
    }

    public void SetOrigin(Vector3 origin) {
        this.origin = origin;
    }

    public void SetAimDirection(Vector3 aimDirection) {
        startingAngle = UtilsClass.GetAngleFromVectorFloat(aimDirection) + fov / 2f;
    }

    public void SetAimDirectionWithAngle(float angle) {
        startingAngle = angle;
    }

    public void SetFoV(float fov) {
        this.fov = fov;
    }

    public void SetViewDistance(float viewDistance) {
        this.viewDistance = viewDistance;
    }

    public void SetFoVAndViewDistance(float fov, float viewDistance) {
        this.fov = fov;
        this.viewDistance = viewDistance;
    }

    public bool DetectInArea() {
        if (player == null) return false;
        return true;
    }   

    private void OnDisable() {
        if (mesh != null) {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = null;
        } 
    }

}
