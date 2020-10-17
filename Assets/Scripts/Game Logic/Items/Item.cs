using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected GameObject player;
    AnimatorManager animManager;
    PlayerController pController;
    StatsTracker stats;

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

    float originalY;
    bool goingUP = true;

    // Start is called before the first frame update
    public virtual void Start()
    {
        player = GameObject.Find("Player");
        animManager = player.GetComponent<AnimatorManager>();
        pController = player.GetComponent<PlayerController>();
        stats = player.GetComponent<StatsTracker>();

        originalY = transform.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Bop up and down... 50 cycles per second.
        if (goingUP) transform.position = new Vector3(transform.position.x, transform.position.y + (Time.fixedDeltaTime / 150f), transform.position.z);
        else transform.position = new Vector3(transform.position.x, transform.position.y - (Time.fixedDeltaTime / 150f), transform.position.z);

        if (transform.position.y >= originalY + 0.0075f) goingUP = false;
        else if (transform.position.y <= originalY) goingUP = true;
    }

    public virtual void PickUPItem() {
        if (pickedUp) return;

        stats.CollectItems++;
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
            room.GetComponent<RoomLogic>().chestItem = null;
            ItemManager.instance.HandlePickUpItemRoom(gameObject);
        }
        else { 
            ItemManager.instance.HandlePickUp(gameObject);
        }
        Destroy(gameObject, 0.5f);
    }

    public abstract void ChangeValues();

}
