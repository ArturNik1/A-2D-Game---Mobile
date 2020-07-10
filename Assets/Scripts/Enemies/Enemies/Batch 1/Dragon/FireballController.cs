using Lean.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    [HideInInspector]
    public GameObject parent;

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
        lifetimeCounter += Time.deltaTime;
        if (lifetimeCounter >= lifetimeMax) {
            if (parent != null) parent.GetComponent<LeanGameObjectPool>().Despawn(gameObject);
            else Destroy(gameObject);
        }

        transform.Translate(transform.InverseTransformDirection(transform.forward) * Time.deltaTime * speed, Space.Self);

    }

    private void OnTriggerEnter(Collider other) {
        if (other.name == "Collider") {
            if (other.transform.parent.childCount > 1) return;
            if (parent != null) parent.GetComponent<LeanGameObjectPool>().Despawn(gameObject);
            else Destroy(gameObject);
        }
        else if (other.tag == "Player") {
            other.attachedRigidbody.velocity = Vector3.zero;
            pController.ReceiveDamage(5);
            if (parent != null) parent.GetComponent<LeanGameObjectPool>().Despawn(gameObject);
            else Destroy(gameObject);
        }
    }

}
