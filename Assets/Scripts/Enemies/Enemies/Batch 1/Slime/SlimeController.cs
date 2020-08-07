using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : EnemyController
{
    public bool isCharging = false; // Only charging when Evil Mage spawned it.

    public override void Start()
    {
        if (!isCharging) {
            base.Start();
            return;
        }

        Init();
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        pController = player.GetComponent<PlayerController>();

        GameObject particleHolder = transform.Find("Particles").gameObject;
        for (int i = 0; i < particleHolder.transform.childCount; i++) {
            string p = particleHolder.transform.GetChild(i).name.Split('_')[1];
            particles.Add(p, particleHolder.transform.GetChild(i).GetComponent<ParticleSystem>());
        }

        if (pController.currentRoomMain.roomAction == RoomLogic.RoomAction.Boss) particles["Spawn"].Play();

        AudioManager.instance.Play("EvilMageNormal01");

        AddVelocityForward();
    }

    public override void Move()
    {
        base.Move();

        if (Vector3.Distance(player.transform.position, transform.position) <= 0.5f && pController.isAlive) {
            // Keep tracking player... 
            ChangeDirectionOnHitToPlayerNoRandom();
        }
    }

    public void AddVelocityForward() {
        rb.AddForce(transform.forward, ForceMode.Impulse);
    }

    public override void OnCollisionEnter(Collision collision) {
        rb.velocity = Vector3.zero;
        if (collision.transform.tag == "Collider") {
            if (collision.transform.name == "Player") {
                // Damage Player 
                collision.gameObject.GetComponent<PlayerController>().ReceiveDamage(damage);
                collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                EnemyManager.enemiesTouching.Add(gameObject);
            }
            else {
                ChangeDirectionOnHitToPlayer();
            }

            if (isCharging) {
                isCharging = false;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

}
