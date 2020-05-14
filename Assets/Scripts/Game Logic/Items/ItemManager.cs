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

    private void Awake() {
        if (_instance != null && _instance != this) Destroy(this.gameObject);
        else _instance = this;

        itemsHolder = gameObject;
    }

    public GameObject DetermineItem() {
        // Determine type - base on items you have or completely random?
        return items[Random.Range(0, items.Count)];
    }

}
