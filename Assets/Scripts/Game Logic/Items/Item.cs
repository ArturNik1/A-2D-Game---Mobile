using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected GameObject player;

    bool pickedUp = false;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Bop up and down...
    }

    public virtual void PickUPItem() {
        if (pickedUp) return;

        player.GetComponent<AnimatorManager>().anim.SetTrigger("PickUp");
        player.GetComponent<PlayerController>().BlockMovement();
        player.GetComponent<PlayerController>().UnBlockMovement(2.5f);
        ChangeValues();
        pickedUp = true;
        Destroy(gameObject, 0.5f);
    }

    public abstract void ChangeValues();

}
