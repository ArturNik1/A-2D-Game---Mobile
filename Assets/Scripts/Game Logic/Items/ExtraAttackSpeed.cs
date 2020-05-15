using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraAttackSpeed : Item
{
    public override void ChangeValues()
    {
        // limit to 5.0f and then disable spawn for item...
        player.GetComponent<AnimatorManager>().anim.SetFloat("Multiplier_AttackSpeed", player.GetComponent<AnimatorManager>().anim.GetFloat("Multiplier_AttackSpeed") + 0.5f);
    }

}
