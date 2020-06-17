using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationHandler : MonoBehaviour
{
    public Vector2 direction;
    GameObject lockedPlayer;
    Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {
        lockedPlayer = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        pos = new Vector3(transform.position.x, transform.position.y, lockedPlayer.transform.position.z);
        float posX = -(pos - lockedPlayer.transform.position).normalized.x, posY = -(pos - lockedPlayer.transform.position).normalized.y;
        direction = new Vector2(posX, posY);
        HandleRotation();
    }

    void HandleRotation() {
        float x = GetEularXRotationFromDirection();
        float y = GetEularYRotationFromDirection();
        float z = GetEularZRotationFromDirection();
        if (x == 0 && y == 0 && z == 0) return;
        transform.rotation = Quaternion.Euler(x, y, z);
    }

    float GetEularXRotationFromDirection() {
        if (direction.y >= 0) return (-50 * Mathf.Pow(direction.y, 2));
        else if (direction.y < 0) return (50 * Mathf.Pow(direction.y, 2));

        Debug.LogWarning("Rotation is invalid!");
        return 0f;
    }

    float GetEularYRotationFromDirection() {
        float rotY = (31.1f * direction.x * (1.4f * Mathf.Pow(direction.x, 2) + 1));

        if (direction.y >= 0) return rotY;
        else if (direction.x >= 0) return (1 - (rotY / 75f)) * 180 + rotY; 
        else if (direction.x < 0)  return -((1 + (rotY / 75f)) * 180 - rotY);

        Debug.LogWarning("Rotation is invalid!");
        return 0f;
    }

    float GetEularZRotationFromDirection() {
        return (-37.3f * direction.x * (1.4f * Mathf.Pow(direction.x, 2) + 1));
    }

}
