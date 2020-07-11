using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : EnemyController
{
    Transform bodyHolder;

    public override void Start()
    {
        base.Start();
        bodyHolder = transform.Find("BodyHolder");
    }


    public override void ReceiveDamage(float amount)
    {
        base.ReceiveDamage(amount);
        speed += 0.1f;
    }

    public override IEnumerator Die()
    {
        while (true)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Die") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.2f)
            {

                if (Random.Range(1, 101) <= ItemManager.instance.dropRate)
                    ItemManager.instance.SpawnItemDropped(transform.position);

                Destroy(gameObject);
                break;
            }
            else
            {
                if (bodyHolder.localRotation.eulerAngles.x >= 330)
                {
                    bodyHolder.localRotation = Quaternion.Euler(bodyHolder.localRotation.eulerAngles.x + 0.5f, 0, 0);
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }

}
