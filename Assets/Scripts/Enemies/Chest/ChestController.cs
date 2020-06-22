using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    [Header("Initialize")]
    public GameObject activeModel;

    [Header("General Info")]
    public float health;
    public float speed;
    public int damage;

    [Header("States")]
    public bool isCharging;
    public bool isIdle;
    public bool isHit;

    [Header("Animation Variables")]
    public float movement = 0f;

    public Dictionary<string, ParticleSystem> particles = new Dictionary<string, ParticleSystem>();

    Animator anim;
    Rigidbody rb;
    GameObject player;
    PlayerController pController;
    RotationHandler rotHandler;

    public bool isActive = false;
    bool isAlive = true;
    bool isWinning = false;

    // Start is called before the first frame update
    void Start() {
        Init();
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        pController = player.GetComponent<PlayerController>();
        rotHandler = GetComponent<RotationHandler>();

        pController.playerDeath += OnPlayerDeath;

        GameObject particleHolder = transform.Find("Particles").gameObject;
        for (int i = 0; i < particleHolder.transform.childCount; i++) {
            string p = particleHolder.transform.GetChild(i).name.Split('_')[1];
            particles.Add(p, particleHolder.transform.GetChild(i).GetComponent<ParticleSystem>());
        }
    }

    void Init() {
        // Setup active model and animator.
        if (activeModel == null) {
            anim = GetComponentInChildren<Animator>();
            if (anim == null) {
                Debug.Log("No Model!");
            }
            else {
                activeModel = anim.gameObject;
            }
        }

        if (anim == null)
            anim = activeModel.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {

        if (Time.timeScale == 0 || Time.deltaTime == 0) return;

        if (!isAlive) rb.velocity = Vector3.zero;

        UpdateStates();
    }

    void FixedUpdate() {
        if (Time.time == 0 || Time.deltaTime == 0) return;

        if (!pController.isAlive || !isAlive) return;

        Move();
    }

    void Move() {
        if (movement == 0) return;

        var inputVector = rotHandler.direction;
        var movementOffset = new Vector3(inputVector.x, inputVector.y, 0).normalized * Mathf.Pow(movement, 2.5f) * Time.fixedDeltaTime;
        var newPosition = rb.position + movementOffset;

        rb.MovePosition(newPosition);
    }

    public void InteractionWithChest() {
        if (isActive) return;
        isActive = true;

        int random = Random.Range(0, 100);
        if (random <= 100) { // 25%?
            // Chest becomes enemy...
            anim.SetTrigger("OptionEnemy");
            //movement = 0.5f;
        }
        else {
            // Chest opens up, item pops out. 
            anim.SetTrigger("OptionOpen");
        }
    }

    public void ChargeForward() {
        movement = 0.0f;
        isCharging = true;
        StartCoroutine(StartSpeedingUp());
    }

    IEnumerator StartSpeedingUp() {
        while (movement < 1 && isCharging) {
            movement += speed * Time.deltaTime;
            yield return null;
        }
        isCharging = false;
    }

    void UpdateStates() {
        anim.SetFloat("movement", movement);
    }

    bool IsBeingHit() {
        if (isHit) { 
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("GetHit") &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) {
                isHit = false;
                return false;
            }
            return true;
        }
        return false;
    }

    public void ReceiveDamage(float amount) {
        health -= amount;
        if (health <= 0) {
            ProjectileController.damageCounter += (amount + health);
            pController.enemiesKilled++;
            isActive = false;
            PlayDeathAnimation();
        }
        else {
            ProjectileController.damageCounter += amount;
            PlayHitAnimation();
        }
        AudioManager.instance.Play("EnemyHit0" + Random.Range(1, 4));
    }

    void PlayDeathAnimation() {
        anim.CrossFade("Die", 0.1f);
        isHit = true;
        isAlive = false;
        rb.detectCollisions = false;
        rb.Sleep();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        movement = 0.0f;
        StartCoroutine(Die());
    }

    IEnumerator Die() { 
        while (true) { 
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Die") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) {
                // Spawn Item...
                // Open Chest...
                break;
            }
            else {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    void PlayHitAnimation() {
        anim.CrossFade("GetHit", 0.05f, 1);
        isHit = true;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.tag == "Collider") {
            if (rotHandler.startingToRotate) return;
            movement = 0;
            isCharging = false;
            rb.AddForce(collision.GetContact(0).normal, ForceMode.Impulse);
            anim.CrossFade("GetHit", 0.025f);
            if (collision.transform.name == "Player") {
                EnemyManager.enemiesTouching.Add(gameObject);
            }
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (collision.transform.name == "Player") {
            rb.velocity = Vector3.zero;
            collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            EnemyManager.enemiesTouching.Remove(gameObject);
        }
    }

    void OnPlayerDeath() {
        pController.playerDeath -= OnPlayerDeath;
        anim.CrossFade("Victory", 0.5f);
    }

    private void OnDestroy() {
        EnemyManager.enemiesTouching.Remove(gameObject);
        pController.playerDeath -= OnPlayerDeath;
    }


}
