using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraHealth : Item
{

    public override void ChangeValues()
    {
        player.GetComponent<PlayerInfo>().maxHealth += 5;
        player.GetComponent<PlayerInfo>().health += 5;
    }

}
