﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
    public List<GameObject> droppedItems;
    [HideInInspector]
    public List<ItemInformation> pickedItems;

    [HideInInspector]
    public float dropRate = 5.0f;
    bool isCoroutineRunning = false;

    [HideInInspector]
    public int itemsPicked = 0;

    private void Awake() {
        if (_instance != null && _instance != this) Destroy(this.gameObject);
        else _instance = this;

        itemsHolder = gameObject;
        
        availableItems = new List<GameObject>(items);
        pickedItems = new List<ItemInformation>();
        droppedItems = new List<GameObject>();
    }

    #region Item Room

    int FindGameObjectInList(GameObject obj) {
        for (int i = 0; i < availableItems.Count; i++) {
            if (availableItems[i] == obj) {
                return i;
            }
        }
        return -1;
    }

    public GameObject DetermineItem(GameObject room) {
        // Determine item spawned in item rooms.
        int rand = 0;
        if (room.GetComponent<RoomLogic>().chestItem != null) {

            int n = FindGameObjectInList(room.GetComponent<RoomLogic>().chestItem);
            if (n == -1) { 
                rand = Random.Range(0, availableItems.Count); // Nothing found, random it.
            }
            else {
                rand = n;
            }
        }
        else { 
            rand = Random.Range(0, availableItems.Count);
        }
        GameObject item = availableItems[rand];

        AudioManager.instance.Play("ItemSpawn0" + Random.Range(1, 3));

        return item;
    }
    public void HandlePickUpItemRoom(GameObject item) {
        // Handling of picked up items from item rooms.
        GameObject itemFromList = ReturnItemFromItems(item);
        if (!IsInPickedItems(itemFromList)) {
            pickedItems.Add(new ItemInformation(itemFromList, item.GetComponent<Item>().itemType, 1, item.GetComponent<Item>().maxAmount));
            droppedItems.Add(itemFromList);
            availableItems.Remove(itemFromList);

            AudioManager.instance.Play("ItemPickup01");
            itemsPicked++;
        }
    }

    #endregion

    #region Dropped Item

    public GameObject DetermineItemDropped() {
        // Determine item dropped.
        int rand = Random.Range(0, droppedItems.Count);
        GameObject item = droppedItems[rand];

        // Check if item can be dropped or there is a max amount of floor already.
        if (pickedItems[FindItemInPicked(item)].canBeDroppedAmount == 0) {
            // Make a new list with all availabe options.
            List<int> valid = new List<int>();
            for(int i = 0; i <  droppedItems.Count; i++) {
                if (i == rand) continue;
                valid.Add(i);
            }

            if (valid.Count == 0) return null;
            rand = valid[Random.Range(0, valid.Count)];
            item = droppedItems[rand];
        }
        else {
            pickedItems[FindItemInPicked(item)].canBeDroppedAmount--;
        }

        return item;
    }

    public void HandlePickUp(GameObject item) {
        // Handling picking up of dropped items.
        GameObject itemFromList = ReturnItemFromItems(item);
        int index = FindItemInPicked(itemFromList);
        pickedItems[index].itemAmount++;
        if (pickedItems[index].itemAmount >= pickedItems[index].maxItemAmount && pickedItems[index].maxItemAmount != -1) {
            droppedItems.Remove(itemFromList);
        }
        AudioManager.instance.Play("ItemPickup02");
        itemsPicked++;
    }

    public void SpawnItemDropped(Vector3 pos) {
        // Spawn the dropped item.
        if (droppedItems.Count == 0) return;

        GameObject determined = DetermineItemDropped();
        if (determined == null) return;

        GameObject obj = Instantiate(determined);
        obj.transform.position = pos;
        obj.transform.SetParent(itemsHolder.transform);

        AudioManager.instance.Play("ItemSpawn0" + Random.Range(1, 3));
    }

    #endregion

    public int AmountInList(ItemInformation.ItemType itemType) {
        foreach (ItemInformation item in pickedItems) {
            if (item.itemType == itemType) return item.itemAmount;
        }
        return -1;
    }

    bool IsInPickedItems(GameObject item) {
        // check if the item is in pickedItem list.
        foreach (ItemInformation picked in pickedItems) {
            if (picked.item == item) return true;
        }
        return false;
    }

    public GameObject ReturnItemFromItems(GameObject itemDeleted) {
        // take the GameObject item that is about to be destroyed and find the static one in the items list.
        foreach (GameObject item in items)
        {
            if (item.name == itemDeleted.name.Substring(0, itemDeleted.name.Length-7)) return item;
        }
        return itemDeleted;
    }

    public int FindItemInPicked(GameObject item) { 
        // find the item index in pickedItems list.
        for (int i = 0; i < pickedItems.Count; i++) {
            if (item == pickedItems[i].item) return i;
        }
        return -1;
    }

    #region Item Popup Fading
    public void callDoFade(CanvasGroup group, float duration, float idleDuration, string message) {
        if (isCoroutineRunning) return;
        group.transform.GetChild(0).GetComponent<Text>().text = message;
        StartCoroutine(DoFade(group, duration, idleDuration));
    }

    IEnumerator DoFade(CanvasGroup group, float duration, float idleDuration) {
        isCoroutineRunning = true;
        float counter = 0f;
        while (counter < duration) {
            counter += Time.deltaTime;
            group.alpha = Mathf.Lerp(0, 1, counter / duration);
            yield return null;
        }

        counter = 0f;
        while (counter < idleDuration) {
            counter += Time.deltaTime;
            yield return null;
        }

        counter = 0f;
        while (counter < duration) {
            counter += Time.deltaTime;
            group.alpha = Mathf.Lerp(1, 0, counter / duration);
            yield return null;
        }
        isCoroutineRunning = false;
    }

    #endregion
}

public class ItemInformation {

    public enum ItemType { ExtraHealth, ExtraAttackSpeed, ExtraDamage, ExtraMovementSpeed, ExtraCritChance, ExtraCritDamage, ExtraItemDropRate }
    public GameObject item;
    public ItemType itemType;
    public int itemAmount;
    public int maxItemAmount;
    public int canBeDroppedAmount;

    public ItemInformation(GameObject item, ItemType type, int amount, int max) {
        this.item = item;
        this.itemType = type;
        this.itemAmount = amount;
        this.maxItemAmount = max;
        this.canBeDroppedAmount = max - amount;
    }

}