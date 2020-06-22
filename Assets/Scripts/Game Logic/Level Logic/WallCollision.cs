using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollision : MonoBehaviour
{
    private void OnCollisionExit(Collision collision) {
        if (collision.transform.name == "Player") {
            collision.rigidbody.velocity = Vector3.zero;
        }
    }
}
