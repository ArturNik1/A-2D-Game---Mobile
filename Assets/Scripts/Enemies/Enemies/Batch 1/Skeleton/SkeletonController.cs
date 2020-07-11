using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonController : EnemyController
{
    public bool shieldHit = false;

    Transform bodyHolder;

    public override void Start()
    {
        base.Start();
        bodyHolder = transform.Find("BodyHolder");
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
        isHit = true;
        isMoving = false;
        ChangeDirectionOnHitToPlayerNoRandom();
    }


}
