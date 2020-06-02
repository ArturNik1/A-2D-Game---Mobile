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

    protected Rigidbody rb;
    protected GameObject player;
    protected PlayerController pController;

    [HideInInspector] public ParticleSystem particle_crit;

    // Start is called before the first frame update
    public virtual void Start()
    {
        Init();
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        pController = player.GetComponent<PlayerController>();
        particle_crit = transform.Find("Body").Find("Particle_Crit").GetComponent<ParticleSystem>();
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
            PlayHitAnimation();
        }
        AudioManager.instance.Play("EnemyHit0" + Random.Range(1, 4));
    }

    public abstract void KillBoss();

    public virtual void PlayHitAnimation() {
        anim.CrossFade("GetHit", 0.05f, 1);
    }

    public virtual void PlayDeathAnimation() {
        anim.CrossFade("Die", 0.1f);
        rb.detectCollisions = false;
        rb.Sleep();
    }

    public virtual void OnCollisionEnter(Collision collision) {
        if (collision.transform.tag == "Collider") { 
            if (collision.transform.name == "Player") {
                // Damage player...
                collision.gameObject.GetComponent<PlayerController>().ReceiveDamage(damage);
                EnemyManager.enemiesTouching.Add(gameObject);
            }
        }
    }

    public virtual void OnCollisionExit(Collision collision) {
        rb.velocity = Vector3.zero;
        if (collision.transform.name == "Player") {
            collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            EnemyManager.enemiesTouching.Remove(gameObject);
        }
    }

    private void OnDestroy() {
        LevelManager.inBossRoom = false;
        EnemyManager.enemiesTouching.Remove(gameObject);
    }
}
