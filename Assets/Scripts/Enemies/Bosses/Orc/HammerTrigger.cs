using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerTrigger : MonoBehaviour
{
    OrcController orc;

    private void Start()
    {
        orc = GetComponentInParent<OrcController>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.attachedRigidbody.name == "Player") {
            EnemyManager.enemiesTouching.Add(orc.gameObject);
        }

        if (!orc.isCharging && orc.isAttacking && !orc.startNormalAttackCooldown) {
            orc.startNormalAttackCooldown = true;
            orc.anim.CrossFade("New State", 0.5f, 1);
            return;
        }

        if (orc.isSpinStageTwo) orc.EndChargeAttack();
    }
    private void OnTriggerExit(Collider other) {
        if (other.attachedRigidbody.name == "Player") {
            EnemyManager.enemiesTouching.Remove(orc.gameObject);
        }
    }
}
