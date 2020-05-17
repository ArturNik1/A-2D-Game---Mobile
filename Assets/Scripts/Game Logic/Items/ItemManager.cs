using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private static ItemManager _instance;
    public static ItemManager instance { get { return _instance; } }

    [HideInInspector]
    public GameObject itemsHolder;
    public List<GameObject> items;

    [HideInInspector]
    public List<GameObject> availableItems;
    [HideInInspector]
    public List<ItemInformation> pickedItems;

    private void Awake() {
        if (_instance != null && _instance != this) Destroy(this.gameObject);
        else _instance = this;

        itemsHolder = gameObject;

        availableItems = items;
        pickedItems = new List<ItemInformation>();
    }

    public GameObject DetermineItem() {

        int rand = Random.Range(0, availableItems.Count);
        GameObject item = availableItems[rand];

        if (IsInPickedItems(item)) {
            int index = FindItemInPicked(item);
            pickedItems[index].itemAmount++;
            if (pickedItems[index].itemAmount >= pickedItems[index].maxItemAmount && pickedItems[index].maxItemAmount != -1) { 
                availableItems.RemoveAt(rand);
            }
        }
        else {
            pickedItems.Add(new ItemInformation(item, item.GetComponent<Item>().itemType, 1, item.GetComponent<Item>().maxAmount));
        }

        return item;
    }

    bool IsInPickedItems(GameObject item) {
        foreach (ItemInformation picked in pickedItems) {
            if (picked.item == item) return true;
        }
        return false;
    }

    int FindItemInPicked(GameObject item) { 
        for (int i = 0; i < pickedItems.Count; i++) {
            if (item == pickedItems[i].item) return i;
        }
        return 0;
    }

}

public class ItemInformation {

    public enum ItemType { ExtraHealth, ExtraAttackSpeed, ExtraDamage, ExtraMovementSpeed }
    public GameObject item;
    public ItemType itemType;
    public int itemAmount;
    public int maxItemAmount;

    public ItemInformation(GameObject item, ItemType type, int amount, int max) {
        this.itemType = type;
        this.itemAmount = amount;
        this.maxItemAmount = max;
    }

}