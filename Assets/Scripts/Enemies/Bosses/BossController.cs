using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BossController : MonoBehaviour
{
    [Header("Initialize")]
    public GameObject activeModel;

    [Header("General Info")]
    public float health;
    public float speed;
    public int damage;

    float maxHealth;

    protected GameObject healthBar;

    [HideInInspector]
    public Animator anim;

    protected GameObject bossRoom;
    protected Rigidbody rb;
    protected GameObject player;
    protected PlayerController pController;
    protected float _speed;
    protected bool isHit = false;
    public bool isAttacking = false;

    public Dictionary<string, ParticleSystem> particles = new Dictionary<string, ParticleSystem>();

    // Start is called before the first frame update
    public virtual void Start()
    {
        Init();
        bossRoom = GameObject.Find("Enemies").GetComponent<BossManager>().bossRoom;
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        pController = player.GetComponent<PlayerController>();
        _speed = speed;
        maxHealth = health;

        healthBar = GameObject.Find("Canvas").transform.Find("Boss Health Slider").gameObject;

        GameObject particleHolder = transform.Find("Particles").gameObject;
        for (int i = 0; i < particleHolder.transform.childCount; i++) {
            string p = particleHolder.transform.GetChild(i).name.Split('_')[1];
            particles.Add(p, particleHolder.transform.GetChild(i).GetComponent<ParticleSystem>());
        }

        bossRoom.GetComponent<RoomLogic>().aliveEnemies++;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (Time.timeScale == 0 || Time.deltaTime == 0) return; // Skip if game is paused.
    }

    public virtual void FixedUpdate() { 
        if (Time.timeScale == 0 || Time.deltaTime == 0) return; // Skip if game is paused.
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
        UpdateHealthBar();
        AudioManager.instance.Play("EnemyHit0" + Random.Range(1, 4));
    }

    public virtual bool IsBeingHit() {
        if (isHit) {
            if (anim.GetCurrentAnimatorStateInfo(1).IsName("GetHit") &&
                anim.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.9f) {
                isHit = false;
                return false;
            }
            return true;
        }
        return false;
    }

    public abstract void KillBoss();

    public virtual void PlayHitAnimation() {
        if (isAttacking) return;
        anim.CrossFade("GetHit", 0.05f, 1);
        isHit = true;
    }

    public virtual void PlayDeathAnimation() {
        anim.CrossFade("Die", 0.1f);
        isHit = true;
        rb.detectCollisions = false;
        rb.Sleep();
    }

    public virtual void UpdateHealthBar() {
        healthBar.GetComponent<Slider>().value = health / maxHealth;
    }

    public abstract void UpdateAnimator();

    public virtual void OnCollisionEnter(Collision collision) {
        if (collision.transform.tag == "Collider") { 
            if (collision.transform.name == "Player") {
                // Damage player...
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

    public virtual void OnDestroy() {
        player.GetComponent<PlayerController>().currentRoomMain.aliveEnemies--;
        if (player.GetComponent<PlayerController>().currentRoomMain.aliveEnemies <= 0) player.GetComponent<PlayerController>().currentRoomMain.cleared = true;

        EnemyManager.enemiesTouching.Remove(gameObject);
        if (healthBar == null) return;
        healthBar.SetActive(false);
    }
}
