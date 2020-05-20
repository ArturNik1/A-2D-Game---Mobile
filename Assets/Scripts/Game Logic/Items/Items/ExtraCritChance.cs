using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraCritChance : Item
{
    public override void Start()
    {
        base.Start();
        itemType = ItemInformation.ItemType.ExtraCritChance;
        message = "+10% Crit Chance";
    }

    public override void ChangeValues()
    {
        ProjectileController.critProcChance += 10;
    }

}
