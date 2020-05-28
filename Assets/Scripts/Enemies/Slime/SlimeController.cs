using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : EnemyController
{
    public override void Move()
    {
        base.Move();

        if (Vector3.Distance(player.transform.position, transform.position) <= 0.5f && pController.isAlive) {
            // Keep tracking player... 
            ChangeDirectionOnHitToPlayerNoRandom();
        }
    }

    public override void OnCollisionEnter(Collision collision) {
        rb.velocity = Vector3.zero;
        if (collision.transform.tag == "Collider") {
            if (collision.transform.name == "Player") {
                // Damage Player 
                collision.gameObject.GetComponent<PlayerController>().ReceiveDamage(damage);
                collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            else {
                ChangeDirectionOnHitToPlayer();
            }
        }
    }

    private void OnCollisionStay(Collision collision) {
        OnCollisionEnter(collision);
    }

}
