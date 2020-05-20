using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraAttackSpeed : Item
{
    public override void Start() {
        base.Start();
        itemType = ItemInformation.ItemType.ExtraAttackSpeed;
        message = "+50% Base Attack Speed";
    }

    public override void ChangeValues()
    {
        // limit to 5.0f and then disable spawn for item...
        player.GetComponent<AnimatorManager>().anim.SetFloat("Multiplier_AttackSpeed", player.GetComponent<AnimatorManager>().anim.GetFloat("Multiplier_AttackSpeed") + 0.5f);
    }

}
