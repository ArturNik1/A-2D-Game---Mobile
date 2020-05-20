using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    #region Item Popup Fading
    public void callDoFade(CanvasGroup group, float duration, float idleDuration, string message) {
        group.transform.GetChild(0).GetComponent<Text>().text = message;
        StartCoroutine(DoFade(group, duration, idleDuration));
    }

    IEnumerator DoFade(CanvasGroup group, float duration, float idleDuration) {
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
    }

    #endregion
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