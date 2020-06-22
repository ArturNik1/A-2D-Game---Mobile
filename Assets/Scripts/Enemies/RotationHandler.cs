using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationHandler : MonoBehaviour
{
    public Vector2 direction;

    GameObject lockedPlayer;
    Vector3 pos;
    ChestController cController;
    bool lookingAtPlayer = false;

    public bool startingToRotate = false;
    public float speed = 150f;

    // Start is called before the first frame update
    void Start()
    {
        lockedPlayer = GameObject.Find("Player");
        cController = GetComponent<ChestController>();
        direction = Vector2.down;
    }

    // Update is called once per frame
    void Update()
    {
        if (!lookingAtPlayer) return;

        /* Not needed for the behavior I am going for.
        pos = new Vector3(transform.position.x, transform.position.y, lockedPlayer.transform.position.z);
        float posX = -(pos - lockedPlayer.transform.position).normalized.x, posY = -(pos - lockedPlayer.transform.position).normalized.y;
        direction = new Vector2(posX, posY);
        HandleRotation();
        */
    }

    IEnumerator StartLookingAtPlayer() {
        bool left = false;
        float angle = Mathf.Atan2(lockedPlayer.transform.position.y - transform.position.y, lockedPlayer.transform.position.x - transform.position.x) * Mathf.Rad2Deg + 180;
        float chestAngle = DirectionToAngle(direction) + 180;
        if (angle >= chestAngle) {
            if ((angle - chestAngle) >= 180) left = true;
            else left = false;
        }
        else {
            if ((chestAngle - angle) >= 180) left = false;
            else left = true;
        }

        while (true) {
            float angleTarget = Mathf.Atan2(lockedPlayer.transform.position.y - transform.position.y, lockedPlayer.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
            float currentAngle = DirectionToAngle(direction);

            if (!left) direction = AngleToDirection(currentAngle + Time.deltaTime * speed);
            else direction = AngleToDirection(currentAngle - Time.deltaTime * speed);

            HandleRotation();

            if (Mathf.Abs(angleTarget - currentAngle) <= 5f) break;

            yield return null;
        }
        lookingAtPlayer = true;
        cController.ChargeForward();
        startingToRotate = false;
    }

    public void LookAtPlayersDirection() {
        startingToRotate = true;
        StartCoroutine(StartLookingAtPlayer());
    }

    void HandleRotation() {
        float x = GetEularXRotationFromDirection(direction.y);
        float y = GetEularYRotationFromDirection(direction.x, direction.y);
        float z = GetEularZRotationFromDirection(direction.x);
        if (x == 0 && y == 0 && z == 0) return;
        transform.rotation = Quaternion.Euler(x, y, z);
    }

    float DirectionToAngle(Vector2 dir) {
        if (dir.x > 0 && dir.y >= 0) {
            return Mathf.Acos(dir.x) * Mathf.Rad2Deg;
        } else if (dir.x <= 0 && dir.y > 0) {
            return Mathf.Acos(dir.x) * Mathf.Rad2Deg;
        } else if (dir.x < 0 && dir.y <= 0) {
            return -Mathf.Acos(dir.x) * Mathf.Rad2Deg;
        } else if (dir.x >= 0 && dir.y < 0) {
            return -Mathf.Acos(dir.x) * Mathf.Rad2Deg;
        }

        Debug.LogWarning("Angle is zero!");
        return 0;
    }

    Vector2 AngleToDirection(float angle) {
        angle = angle * Mathf.Deg2Rad;
        if (angle >= 0 && angle <= 90 || angle >= -180 && angle <= -90) {
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        } else if (angle > 90 && angle <= 180) { 
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        } else if (angle > -90 && angle < 0) {
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        Debug.LogWarning("Vector2 is zero!");
        return Vector2.zero;
    }

    float GetEularXRotationFromDirection(float y) {
        if (y >= 0) return (-50 * Mathf.Pow(y, 2));
        else if (y < 0) return (50 * Mathf.Pow(y, 2));

        Debug.LogWarning("Rotation is invalid!");
        return 0f;
    }

    float GetEularYRotationFromDirection(float x, float y) {
        float rotY = (31.1f * x * (1.4f * Mathf.Pow(x, 2) + 1));

        if (y >= 0) return rotY;
        else if (x >= 0) return (1 - (rotY / 75f)) * 180 + rotY; 
        else if (x < 0)  return -((1 + (rotY / 75f)) * 180 - rotY);

        Debug.LogWarning("Rotation is invalid!");
        return 0f;
    }

    float GetEularZRotationFromDirection(float x) {
        return (-37.3f * x * (1.4f * Mathf.Pow(x, 2) + 1));
    }

}
