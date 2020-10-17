using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailController : EnemyController
{
    public override void PlayDeathAnimation()
    {
        base.PlayDeathAnimation();
        stats.KillTurtles++;
        stats.KillTotalEnemies++;
    }
}
