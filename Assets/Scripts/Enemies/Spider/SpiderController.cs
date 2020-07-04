﻿using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : EnemyController
{
    public bool isShooting = false;
    public float shotCooldownMax = 2.5f;

    Transform projectiles;
    float shotCooldownCounter = 0.0f;

    public override void Start()
    {
        base.Start();
        shotCooldownMax = Random.Range(shotCooldownMax - 0.5f, shotCooldownMax + 0.5f);
        shotCooldownCounter = shotCooldownMax;
        projectiles = GameObject.Find("ProjectilesEnemy").transform;
    }

    public override void Move()
    {
        shotCooldownCounter += Time.deltaTime;
        if (Vector2.Distance(player.transform.position, gameObject.transform.position) <= 0.9f) {
            skipMove = true;
            isMoving = false;
            ChangeDirectionOnHitToPlayerNoRandom();
            // Check if shooting/cooldown. Check if looking at player (aprox). Shoot with some error margin.
            if (!isShooting && shotCooldownCounter >= shotCooldownMax && isLookingAtPlayerAprox()) {
                shotCooldownCounter = 0.0f;
                isShooting = true;
                anim.CrossFade("Attack03", 0.1f);
                Invoke("Shoot", 0.5f);
            }
            else isShooting = false;
        }
        else {
            isMoving = true;
            skipMove = false;
        }
        base.Move();
    }

    void Shoot() {
        var obj = GetComponent<LeanGameObjectPool>().Spawn(transform.position, transform.rotation, projectiles);
        obj.GetComponent<SpiderWebController>().parent = gameObject;
        AudioManager.instance.Play("SpiderWeb01");
    }

    bool isLookingAtPlayerAprox() { 
        return Mathf.Abs(Vector2.Angle(transform.forward, player.transform.position - transform.position)) <= 5f;
    }

}
