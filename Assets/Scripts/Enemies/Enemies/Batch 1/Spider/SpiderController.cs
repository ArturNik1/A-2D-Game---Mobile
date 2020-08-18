using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : EnemyController
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

    public override void Update()
    {
        shotCooldownCounter += Time.deltaTime;
        base.Update();
    }

    public override void Move()
    {
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
        if (rb.detectCollisions == false) return; // In process of dying, don't shoot.

        shotCounter++;

        var obj = GetComponent<LeanGameObjectPool>().Spawn(transform.position, transform.rotation, projectiles);
        obj.GetComponent<SpiderWebController>().parent = gameObject;
        AudioManager.instance.Play("SpiderWeb01");

        if (shotCounter >= 3) {
            shotCounter = 0;

            StartCoroutine(QuickShots(0.5f));
        }
    }

    IEnumerator QuickShots(float cooldown) { 
        for (int i = 0; i < 2; i++) {
            yield return new WaitForSeconds(cooldown);
            var obj = GetComponent<LeanGameObjectPool>().Spawn(transform.position, transform.rotation, projectiles);

            Transform _obj = obj.transform.GetChild(0);
            _obj.GetChild(0).gameObject.SetActive(false);
            _obj.GetChild(1).gameObject.SetActive(false);
            _obj.GetChild(2).gameObject.SetActive(true);
            _obj.GetChild(3).gameObject.SetActive(true);

            obj.GetComponent<SpiderWebController>().parent = gameObject;
            AudioManager.instance.Play("SpiderWeb01");
        }
        shotCooldownCounter = 0f;
    }

    bool isLookingAtPlayerAprox() { 
        return Mathf.Abs(Vector2.Angle(transform.forward, player.transform.position - transform.position)) <= 5f;
    }

    public override void PlayHitAnimation() {
        anim.CrossFade("GetHit", 0.1f, 1);
        isHit = true;
        isMoving = false;
        ChangeDirectionOnHitToPlayerNoRandom();
    }

    public override bool IsBeingHit() {
        if (isHit) {
            if (anim.GetCurrentAnimatorStateInfo(1).IsName("GetHit") &&
                anim.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.9f) {
                isHit = false;
                return false;
            }
            return true;
        }
        return false;
    }

}
