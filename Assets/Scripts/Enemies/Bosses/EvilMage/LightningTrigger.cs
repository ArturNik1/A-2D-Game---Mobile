using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningTrigger : MonoBehaviour
{
    ParticleSystem strike;
    LightningStrikeController lsController;

    private void OnEnable()
    {
        strike = GetComponent<ParticleSystem>();
        strike.trigger.SetCollider(0, GameObject.Find("Player").transform.Find("PlayerModel").transform.Find("GeneralCollider"));

        lsController = GetComponentInParent<LightningStrikeController>();
    }

    private void OnParticleTrigger()
    {
        List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();
        int amount = strike.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

        if (amount != 0) {
            lsController.pController.ReceiveDamage(5);
        }
    }
}
