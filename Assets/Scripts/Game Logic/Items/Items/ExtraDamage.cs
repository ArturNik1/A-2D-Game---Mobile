﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraDamage : Item
{
    public override void Start() {
        base.Start();
        itemType = ItemInformation.ItemType.ExtraDamage;
        message = "+1 Damage";
    }

    public override void ChangeValues() {
        ProjectileController.damageAmount++;
    }

}
