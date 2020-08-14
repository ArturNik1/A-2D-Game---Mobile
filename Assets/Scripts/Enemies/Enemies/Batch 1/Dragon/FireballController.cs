using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    [HideInInspector]
    public GameObject parent;
    [HideInInspector]
    public bool shouldHold = false; // don't move until changed...
    [HideInInspector]
    public bool isFromBoss = false;

    public float lifetimeMax = 5f;
    public float speed = 1f;

    PlayerController pController;
    float lifetimeCounter = 0.0f;

    void OnEnable() {
        pController = GameObject.Find("Player").GetComponent<PlayerController>();
        lifetimeCounter = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldHold) return;

        lifetimeCounter += Time.deltaTime;
        if (lifetimeCounter >= lifetimeMax) {
            if (parent != null && !isFromBoss) parent.GetComponent<LeanGameObjectPool>().Despawn(gameObject);
            else if (parent != null && isFromBoss) LeanPool.Despawn(gameObject);
            else Destroy(gameObject);
        }

        transform.Translate(transform.InverseTransformDirection(transform.forward) * Time.deltaTime * speed, Space.Self);

    }

    private void OnTriggerEnter(Collider other) {
        if (other.name == "Collider") {
            if (other.transform.parent.childCount > 1) return;
            if (parent != null) {
                if (!isFromBoss) parent.GetComponent<LeanGameObjectPool>().Despawn(gameObject);
                else LeanPool.Despawn(gameObject);
                shouldHold = false;
                transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            } 
            else Destroy(gameObject);
        }
        else if (other.tag == "Player") {
            other.attachedRigidbody.velocity = Vector3.zero;
            pController.ReceiveDamage(5);
            if (parent != null) {
                if (!isFromBoss) parent.GetComponent<LeanGameObjectPool>().Despawn(gameObject);
                else LeanPool.Despawn(gameObject);
                shouldHold = false;
                transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            }
            else Destroy(gameObject);
        }
    }

}
