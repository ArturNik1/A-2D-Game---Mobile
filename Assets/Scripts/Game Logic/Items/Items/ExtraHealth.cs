﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraHealth : Item
{
    public override void Start() {
        base.Start();
        itemType = ItemInformation.ItemType.ExtraHealth;
        message = "+5 Health";
    }

    public override void ChangeValues() {
        player.GetComponent<PlayerInfo>().maxHealth += 5;
        player.GetComponent<PlayerInfo>().health += 5;
        StartCoroutine(UpdateHealthBarDelay(0.4f));
    }

    IEnumerator UpdateHealthBarDelay(float seconds) {
        yield return new WaitForSeconds(seconds);
        player.GetComponent<PlayerController>().UpdateHealthBar();
    }

}
