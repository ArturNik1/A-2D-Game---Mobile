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

}
