using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonController : EnemyController
{ // worst fucking code, right here.
    public bool shieldHit = false;
    public bool isAttacking = false;
    public bool shouldRun = false;

    bool _isHit = false;

    Transform bodyHolder;
    FieldOfViewEnemy fov;

    float movement = 0.0f;

    public override void Start()
    {
        base.Start();
        fov = GetComponent<FieldOfViewEnemy>();
        bodyHolder = transform.Find("BodyHolder");
    }

    public override void Update()
    {
        base.Update();

        if (!isAlive) return;

        fov.SetOrigin(new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.1f));
        fov.SetAimDirection(direction);
        shouldRun = fov.DetectInArea();

        HandleMovementAndSpeed();
    }

    public override void FixedUpdate()
    {
        if (Time.timeScale == 0 || Time.deltaTime == 0) return; // skip if game is paused.

        if (!pController.isAlive || !isAlive) return;

        Move();
    }

    void HandleMovementAndSpeed() {
        if (shouldRun) {
            // Lock on to player..
            ChangeDirectionToPlayerDelay();

            //if (isAttacking) return;
            if (DistanceFromPlayer() <= 0.25f && !_isHit) {
                anim.CrossFade("Attack02", 0f, 0);
                isAttacking = true;
                speed = 0;
                movement = 0;
                return;
            }

            if (isAttacking) return;

            // Start Charging...
            if (movement < 1.0f) {
                movement += Time.deltaTime;
                if (movement >= 0.5f) speed = 0.5f * movement - 0.1f;
                else speed = 0.3f * movement;
            }

        }
        else {
            // Walk...
            if (movement > 0.5f) movement -= Time.deltaTime;
            else movement += Time.deltaTime;
            if (movement >= 0.5f) speed = 0.5f * movement - 0.1f;
            else speed = 0.3f * movement;
        }
    }

    public override void UpdateStates()
    {
        base.UpdateStates();
        anim.SetFloat("movement", movement);
    }

    public override void ReceiveDamage(float amount)
    {
        if (shieldHit) amount /= 2;
        health -= amount;
        if (health <= 0)
        {
            ProjectileController.damageCounter += (amount + health);
            pController.enemiesKilled++;
            isAlive = false;
            PlayDeathAnimation();   
        }
        else
        {
            ProjectileController.damageCounter += amount;
            PlayHitAnimation();
        }
        AudioManager.instance.Play("EnemyHit0" + Random.Range(1, 4));
    }

    public override IEnumerator Die()
    {
        while (true) {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Die") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.2f) {

                if (Random.Range(1, 101) <= ItemManager.instance.dropRate)
                    ItemManager.instance.SpawnItemDropped(transform.position);

                Destroy(gameObject);
                break;
            }
            else {
                if (bodyHolder.localRotation.eulerAngles.x >= 340) {
                    bodyHolder.localRotation = Quaternion.Euler(bodyHolder.localRotation.eulerAngles.x + 0.25f, 0, 0);
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public override void PlayHitAnimation()
    {
        if (shieldHit) anim.CrossFade("DefenseGetHit", 0.1f, 1);
        else anim.CrossFade("GetHit", 0.1f, 1);
        shieldHit = false;
        _isHit = true;
        isMoving = false;
        ChangeDirectionOnHitToPlayerNoRandom();
    }

    public override bool IsBeingHit()
    {
        if (_isHit) {
            if (anim.GetCurrentAnimatorStateInfo(1).IsName("GetHit") &&
                anim.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.9f ||
                anim.GetCurrentAnimatorStateInfo(1).IsName("DefenseGetHit") &&
                anim.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.9f) {
                _isHit = false;
                return false;
            }
            return true;
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.name == "Player") {
            EnemyManager.enemiesTouching.Add(gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody.name == "Player") { 
            EnemyManager.enemiesTouching.Remove(gameObject);
        }
    }

}
