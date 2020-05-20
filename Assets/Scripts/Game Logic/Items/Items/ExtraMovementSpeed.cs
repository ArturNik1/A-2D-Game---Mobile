using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraMovementSpeed : Item
{
    public override void Start() {
        base.Start();
        itemType = ItemInformation.ItemType.ExtraMovementSpeed;
        message = "+7.5% Base Movement Speed";
    }

    public override void ChangeValues() {
        player.GetComponent<PlayerInfo>().movementSpeed += 0.05f;
        player.GetComponent<PlayerInfo>().UpdateControllerValues();
    }

}
