using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraCritDamage : Item
{
    public override void Start()
    {
        base.Start();
        itemType = ItemInformation.ItemType.ExtraCritDamage;
        message = "+10% Crit Damage";
    }
    public override void ChangeValues()
    {
        ProjectileController.critMultiplier += 0.1f;
    }


}
