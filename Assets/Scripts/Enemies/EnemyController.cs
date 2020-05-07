using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour, IEnemyController
{
    [Header("Initialize")]
    public GameObject activeModel;

    [Header("General Info")]
    public int health;
    public float speed;
    public int damage;

    [Header("States")]
    public bool isMoving;
    public bool isIdle;
    public bool isHit;

    [HideInInspector]
    public Animator anim;

    protected Rigidbody2D rb;
    protected Vector2 direction;
    protected Vector2 directionRotate;
    protected GameObject player;
    protected PlayerController pController;

    public float startDelay;
    float startDelayCounter;
    bool isWinning;

    // Start is called before the first frame update
    public virtual void Start()
    {
        Init();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        pController = player.GetComponent<PlayerController>();
        ChangeDirectionOnStart();
    }

    void Init() { 
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
    public virtual void Update()
    {
        UpdateStates();

        if (IsBeingHit()) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction * Time.deltaTime, Vector3.back), 0.05f);
            return;
        }

        if (!pController.isAlive) {
            if (!isWinning) PlayVictoryAnimation();
            return;
        }

        Move();
    }

    public virtual void Move() {
        var inputVector = direction;
        var movementOffset = inputVector.normalized * speed * Time.fixedDeltaTime;
        var newPosition = rb.position + movementOffset;

        if (inputVector != Vector2.zero) {
            isMoving = true;
            isIdle = false;
        }
        else {
            isMoving = false;
            isIdle = true;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVector * Time.deltaTime, Vector3.back), 0.05f);

        if (startDelayCounter <= startDelay) { // Don't move if X seconds had not passed.
            startDelayCounter += Time.deltaTime;
            return;
        }

        if (isHit) return;
        rb.MovePosition(newPosition);
    }

    #region Direction Changes
    public virtual void ChangeDirectionOnStart() {
        float randX = Random.Range(-1f, 1f);
        float randY = Random.Range(-1f, 1f);
        direction = new Vector2(randX, randY);
    }
    public virtual void ChangeDirectionOnHitToPlayer() {
        float posX = -(transform.position - player.transform.position).normalized.x, posY = -(transform.position - player.transform.position).normalized.y;
        posX = Random.Range(posX - 0.3f, posX + 0.3f);
        posY = Random.Range(posY - 0.3f, posY + 0.3f);

        direction = new Vector2(posX, posY);
    }
    public virtual void ChangeDirectionOnHitToPlayerNoRandom() {
        float posX = -(transform.position - player.transform.position).normalized.x, posY = -(transform.position - player.transform.position).normalized.y;
        direction = new Vector2(posX, posY);
    }
    public virtual void ChangeDirectionOnHit(float normalX, float normalY) {
        float randX, randY;

        if (normalX > 0) randX = Random.Range(0.25f, 0.75f); // was Random.Range(0, 1f); Made the change so enemies will not hug the wall.
        else if (normalX < 0) randX = Random.Range(-0.75f, -0.25f); // was Random.Range(-1f, 0);
        else randX = Random.Range(-1f, 1f);

        if (normalY > 0) randY = Random.Range(0.25f, 0.75f); // was Random.Range(0, 1f);
        else if (normalY < 0) randY = Random.Range(-0.75f, -0.25f); // was Random.Range(-1f, 0);
        else randY = Random.Range(-1f, 1f);

        direction = new Vector2(randX, randY);
    }
    #endregion

    #region Damage
    public virtual void ReceiveDamage(int amount) {
        health -= amount;
        if (health <= 0) {
            PlayDeathAnimation();
        }
        else {
            PlayHitAnimation();
        }
    }
    public void PlayHitAnimation() {
        anim.CrossFade("GetHit", 0.1f);
        isHit = true;
        isMoving = false;
        ChangeDirectionOnHitToPlayerNoRandom();
    }
    public void PlayDeathAnimation() {
        anim.CrossFade("Die", 0.1f);
        isHit = true;
        isMoving = false;
        rb.simulated = false;
        StartCoroutine(Die());
    }
    public virtual IEnumerator Die() {
        while (true) {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Die") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f) {
                Destroy(gameObject);
                break;
            }
            else { 
                yield return new WaitForEndOfFrame();
            }
        }
    }
    public virtual bool IsBeingHit() {
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
    public virtual void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.tag == "Collider") {
            if (collision.transform.name == "Player") {
                // Damage Player 
                collision.gameObject.GetComponent<PlayerController>().ReceiveDamage(damage);
                ChangeDirectionOnHit(collision.GetContact(0).normal.x, collision.GetContact(0).normal.y);
            }
            else { 
                ChangeDirectionOnHitToPlayer();
            }
        }
    }
    #endregion

    public void PlayVictoryAnimation() {
        anim.CrossFade("Victory", 1f);
        isMoving = false;
        isWinning = true;
    }

    public virtual void UpdateStates() {
        anim.SetBool("isMoving", isMoving);
    }

}
