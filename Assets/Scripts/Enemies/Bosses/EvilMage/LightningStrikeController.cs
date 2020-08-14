using Lean.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningStrikeController : MonoBehaviour
{
    [HideInInspector] public bool shouldHold = false;

    Transform rotHandler;
    ParticleSystem strike;
    bool isStrikePlaying = false;

    public float lifetimeMax = 5f;
    public float speed = 1f;

    [HideInInspector] public PlayerController pController;
    float lifetimeCounter = 0.0f;

    // Start is called before the first frame update
    void OnEnable()
    {
        pController = GameObject.Find("Player").GetComponent<PlayerController>();
        lifetimeCounter = 0.0f;

        strike = transform.GetChild(0).Find("LightningStrike").GetComponent<ParticleSystem>();
        transform.rotation = new Quaternion(0, 0, 0, 0);

        rotHandler = transform.GetChild(1).transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldHold) return;

        //LayerMask mask = LayerMask.GetMask("Player"); value is 512
        var overlap = Physics.OverlapSphere(transform.position, 0.45f, 512);

        isStrikePlaying = false;
        foreach (var item in overlap)
        {
            if (item.tag == "Player") {

                UnityEngine.ParticleSystem.ShapeModule edit = strike.shape;
                isStrikePlaying = true;

                float posX = -(transform.position - pController.transform.position).normalized.x, posY = -(transform.position - pController.transform.position).normalized.y;
                Vector2 direction = new Vector2(posX, posY);
                float degree = (float)Math.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                if (degree > 0 && degree <= 180) degree *= -1;
                else degree = (degree + 360) * -1;

                edit.rotation = new Vector3(90, degree, 0);

                break;
            }
        }
        strike.gameObject.SetActive(isStrikePlaying);

        lifetimeCounter += Time.deltaTime;
        if (lifetimeCounter >= lifetimeMax) {
            try {
                strike.gameObject.SetActive(false);
                LeanPool.Despawn(gameObject);
            }
            catch (NullReferenceException) {
                Debug.LogWarning("Could not despawn projectile properly!");
                Destroy(gameObject);
            }
        }

        transform.Translate(transform.InverseTransformDirection(rotHandler.forward) * Time.deltaTime * speed, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Collider") {
            if (other.transform.parent.childCount > 1) return;
            strike.gameObject.SetActive(false);
            LeanPool.Despawn(gameObject);
            shouldHold = false;
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
        else if (other.tag == "Player") {
            other.attachedRigidbody.velocity = Vector3.zero;
            strike.gameObject.SetActive(false);
            pController.ReceiveDamage(5);
            LeanPool.Despawn(gameObject);
            shouldHold = false;
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
    }

    private void OnDisable()
    {
        AudioManager.instance.StopPlaying("EvilMageSparkLightning");
    }

}
