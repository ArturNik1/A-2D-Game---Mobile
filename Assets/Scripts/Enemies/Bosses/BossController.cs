using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossController : MonoBehaviour
{
    [Header("Initialize")]
    public GameObject activeModel;

    [Header("General Info")]
    public float health;
    public float speed;
    public int damage;

    [HideInInspector]
    public Animator anim;

    protected GameObject player;
    protected PlayerController pController;

    // Start is called before the first frame update
    public virtual void Start()
    {
        Init();
        player = GameObject.Find("Player");
        pController = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Init()
    {
        // Setup active model and animator.
        if (activeModel == null)
        {
            anim = GetComponentInChildren<Animator>();
            if (anim == null)
            {
                Debug.Log("No Model!");
            }
            else
            {
                activeModel = anim.gameObject;
            }
        }

        if (anim == null)
            anim = activeModel.GetComponent<Animator>();
    }

    public virtual void ReceiveDamage(float amount) {
        health -= amount;
        if (health <= 0) {
            ProjectileController.damageCounter += (amount + health);
            pController.enemiesKilled++;
            KillBoss();
        }
        else {
            ProjectileController.damageCounter += amount;
            // Play hit animation?
        }
        AudioManager.instance.Play("EnemyHit0" + Random.Range(1, 4));
    }

    public abstract void KillBoss();

    private void OnDestroy() {
        LevelManager.inBossRoom = false;
    }
}
