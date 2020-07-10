using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : EnemyController
{
    public override void ReceiveDamage(float amount)
    {
        base.ReceiveDamage(amount);
        speed += 0.1f;
    }
}
