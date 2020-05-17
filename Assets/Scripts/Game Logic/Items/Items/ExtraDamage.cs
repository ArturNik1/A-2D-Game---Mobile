using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraDamage : Item
{
    public override void Start()
    {
        base.Start();
        itemType = ItemInformation.ItemType.ExtraDamage;
    }

    public override void ChangeValues()
    {
        ProjectileController.damageAmount++;
    }

}
