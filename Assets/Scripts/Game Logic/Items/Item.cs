using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected GameObject player;
    AnimatorManager animManager;
    PlayerController pController;

    [HideInInspector]
    public ItemInformation.ItemType itemType;
    public int maxAmount;
    [HideInInspector]
    public string message;

    [HideInInspector]
    public GameObject room;
    bool pickedUp = false;
    [HideInInspector]
    public bool fromItemRoom;

    // Start is called before the first frame update
    public virtual void Start()
    {
        player = GameObject.Find("Player");
        animManager = player.GetComponent<AnimatorManager>();
        pController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Bop up and down...
    }

    public virtual void PickUPItem() {
        if (pickedUp) return;

        animManager.anim.SetTrigger("PickUp");
        animManager.vertical = 0;
        animManager.horizontal = 0;
        pController.BlockMovement();
        pController.UnBlockMovement(2.5f);
        ChangeValues();
        ItemManager.instance.callDoFade(GameObject.Find("Item Popup").GetComponent<CanvasGroup>(), 0.5f, 3f, message);
        pickedUp = true;
        if (fromItemRoom) {
            room.GetComponent<RoomLogic>().cleared = true;
            ItemManager.instance.HandlePickUpItemRoom(gameObject);
        }
        else { 
            ItemManager.instance.HandlePickUp(gameObject);
        }
        Destroy(gameObject, 0.5f);
    }

    public abstract void ChangeValues();

}
