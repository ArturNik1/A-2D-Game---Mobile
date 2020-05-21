using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraItemDropRate : Item
{
    public override void Start()
    {
        base.Start();
        itemType = ItemInformation.ItemType.ExtraItemDropRate;
        message = "+1% Item Drop Rate";
    }
    public override void ChangeValues()
    {
        ItemManager.instance.dropRate += 1;
    }


}
