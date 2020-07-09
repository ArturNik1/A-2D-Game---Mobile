using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPlantController : EnemyController
{
    public float maxAttackCooldown = 2.5f;

    float attackCooldownCounter = 2.5f;
    [HideInInspector] public bool isAttacking = false;

    public override void Update()
    {
        base.Update();
        attackCooldownCounter += Time.deltaTime;
        if (DistanceFromPlayer() <= 0.3f && attackCooldownCounter >= maxAttackCooldown && IsPlayerLookingAway()) {
            ChangeDirectionOnHitToPlayerNoRandom();
            attackCooldownCounter = 0.0f;
            isAttacking = true;
            anim.CrossFade("Attack02", 0.1f);
        }
        if (isAttacking) ChangeDirectionOnHitToPlayerNoRandom();
    }

    bool IsPlayerLookingAway() {
        Vector2 dir = (player.transform.position - transform.position).normalized;
        float dot = Vector2.Dot(dir, player.transform.up);

        if (dot >= 0.5f) return true;
        return false;
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if (!isAttacking) return;
        if (collision.transform.tag == "Collider") {
            if (collision.transform.name == "Player") {
                // Damage Player 
                EnemyManager.enemiesTouching.Add(gameObject);
            }
        }
    }

}
