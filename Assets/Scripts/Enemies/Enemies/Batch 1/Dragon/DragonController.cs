using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController : EnemyController
{
    public bool isShooting = false;
    public float shotCooldownMax = 2.5f;

    int shotCounter = 0;

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
        if (Vector2.Distance(player.transform.position, gameObject.transform.position) <= 1.25f) {
            skipMove = true;
            ChangeDirectionOnHitToPlayerNoRandom();
            // Check if shooting/cooldown. Check if looking at player (aprox). Shoot with some error margin.
            if (!isShooting && shotCooldownCounter >= shotCooldownMax && isLookingAtPlayerAprox()) {
                shotCooldownCounter = 0.0f;
                isShooting = true;
                anim.CrossFade("Attack03", 0.1f);
                Invoke("Shoot", 1f);
            } 
            else isShooting = false;
        }
        else {
            skipMove = false;
        }
        base.Move();
    }

    void Shoot() {
        if (rb.detectCollisions == false) return; // In process of dying, don't shoot.

        shotCounter++;

        var obj = GetComponent<LeanGameObjectPool>().Spawn(transform.position, transform.rotation, projectiles);
        obj.GetComponent<FireballController>().parent = gameObject;
        AudioManager.instance.Play("Fireball01");

        if (shotCounter >= 3) {
            shotCounter = 0;

            Vector3 rot = transform.rotation.eulerAngles;

            obj = GetComponent<LeanGameObjectPool>().Spawn(transform.position, Quaternion.Euler(rot.x + 15f, -90f, 90f), projectiles);
            obj.GetComponent<FireballController>().parent = gameObject;

            obj = GetComponent<LeanGameObjectPool>().Spawn(transform.position, Quaternion.Euler(rot.x - 15f, -90f, 90f), projectiles);
            obj.GetComponent<FireballController>().parent = gameObject;
        }

    }

    bool isLookingAtPlayerAprox() {
        return Mathf.Abs(Vector2.Angle(transform.forward, player.transform.position - transform.position)) <= 5f;
    }

}
