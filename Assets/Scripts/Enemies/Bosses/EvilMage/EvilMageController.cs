using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EvilMageController : BossController
{
    public enum EvilMageStates { Idle, Locked, Moving, Attacking, SpecialAttack, Dizzy, Winning, Dying }
    EvilMageStates previousState;
    EvilMageStates currentState;

    [Header("Animation Variables")]
    public float movement = 0f;

    bool isAlive = true;
    bool firstUpdate = true;

    // Each cooldown starts after that particular attack animation is done.
    [Header("Variables")]
    [SerializeField] float normalShotCooldown = 2f;
    [SerializeField] float specialShotCooldown = 10f;
    [SerializeField] float walkAmount = 4f;

    float normalShotCounter = 0f;
    float specialShotCounter = 0f;
    float walkCounter = 0f;

    int normalShotAttacksAmount = 6;
    int normalShotAttacksCount = 0;

    bool startNormalShotCooldown = false;
    bool canUseNormalShot = false;
    bool startSpecialShotCooldown = false;
    bool canUseSpecialShot = false;
    bool ableToMove = false;
    bool isWalking = false;
    
    EnemyManager enemyManager;
    Vector2 lastMovedDirection;

    [Header ("Objects")]
    [SerializeField] GameObject shootingPoint;

    public override void Start() {
        base.Start();
        currentState = EvilMageStates.Idle;
        pController.playerDeath += OnPlayerDeath;

        healthBar.SetActive(true);
        healthBar.GetComponent<Slider>().value = 1f;
        healthBar.transform.Find("Boss Name Text").GetComponent<Text>().text = "MAGE";

        enemyManager = GameObject.Find("Enemies").GetComponent<EnemyManager>();
    }

    public override void Update() {
        base.Update();

        if (firstUpdate) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(0, -1, 0) * Time.fixedDeltaTime, Vector3.back), 1);
            firstUpdate = false;
        }

        HandleStates();
        UpdateAnimator();

        if (startNormalShotCooldown) {
            normalShotCounter += Time.deltaTime;
            if (normalShotCounter >= normalShotCooldown) {
                // NormalShotCounter is being reset at the end of the animation.
                startNormalShotCooldown = false;
                canUseNormalShot = true;
            }
        }
        if (startSpecialShotCooldown) {
            specialShotCounter += Time.deltaTime;
            if (specialShotCounter >= specialShotCooldown) {
                // SpecialShotCounter is being reset at the end of the animation.
                startSpecialShotCooldown = false;
                canUseSpecialShot = true;
            }
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!pController.isAlive || currentState == EvilMageStates.Dying) return;

        if (currentState == EvilMageStates.Moving || currentState == EvilMageStates.Attacking) {
            FollowPlayer();
        }
    }

    void HandleStates() {

        if (currentState == EvilMageStates.Idle) {

        }

        else if (currentState == EvilMageStates.Locked) {
            // Check for special shot cooldown, if can shoot + normal shot cooldown is fine aswell, switch to SpecialShot.
            // else if normal shot cooldown is fine, switch to Attacking.
            // else, switch to Moving.
            if (canUseSpecialShot && canUseNormalShot) {
                previousState = currentState;
                currentState = EvilMageStates.SpecialAttack;
            }
            else if (canUseNormalShot) {
                previousState = currentState;
                currentState = EvilMageStates.Attacking;
            }
            else {
                previousState = currentState;
                currentState = EvilMageStates.Moving;
            }
        }

        else if (currentState == EvilMageStates.Moving) {
            // Slowly move towards the player, if can use shots, switch to Locked.

            if (Vector2.Distance(transform.position, player.transform.position) <= 0.8f) {
                if (_speed > 0) {
                    _speed -= Time.deltaTime / 5f;
                    movement -= Time.deltaTime / 2.5f;
                }
                else { 
                    ableToMove = false;
                    movement = 0f;
                    isWalking = false;
                    walkCounter = 0f;
                    previousState = currentState;
                    currentState = EvilMageStates.Locked;
                }
            }
            else if (isWalking) {
                if (_speed <= speed) { 
                    _speed += Time.deltaTime / 5f;
                    movement += Time.deltaTime / 2.5f;
                }
                else {
                    movement = _speed * 2f;
                }
                ableToMove = true;
            }

            if (isWalking) {
                walkCounter += Time.deltaTime;
                if (walkCounter >= walkAmount)
                {
                    isWalking = false;
                    walkCounter = 0f;
                }
                return;
            }

            if (canUseNormalShot || canUseSpecialShot) {
                ableToMove = false;
                _speed = 0f;
                movement = 0f;
                previousState = currentState;
                currentState = EvilMageStates.Locked;
            }

        }

        else if (currentState == EvilMageStates.Attacking && previousState == EvilMageStates.Locked) {
            // Use one of the normal shots, switch to Locked afterwards.
            if (isAttacking) return;
            isAttacking = true;
            normalShotAttacksCount++;
            anim.CrossFade("Attack01", 0.1f, 2);
        }

        else if (currentState == EvilMageStates.SpecialAttack && previousState == EvilMageStates.Locked) {
            // Use one of the special shots, switch to Dizzy when done.
            if (isAttacking) return;
            isAttacking = true;
            anim.CrossFade("Attack02", 0.25f, 2);
        }

        else if (currentState == EvilMageStates.Dizzy && previousState == EvilMageStates.SpecialAttack) { 
            // Stays Dizzy for a short while, switch to Locked afterwards.
        }

        else if (currentState == EvilMageStates.Winning) { 
            // Does not need logic.
        }

        else if (currentState == EvilMageStates.Dizzy) { 
            // Does not need logic.
        }

    }

    void FollowPlayer() {
        float posX = -(transform.position - player.transform.position).normalized.x, posY = -(transform.position - player.transform.position).normalized.y;
        Vector2 direction = new Vector2(posX, posY);
        lastMovedDirection = direction;
        var inputVector = direction;
        var movementOffset = new Vector3(inputVector.x, inputVector.y, 0).normalized * _speed * Time.fixedDeltaTime;
        var newPosition = rb.position + movementOffset;

        if (!isAttacking) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVector * Time.fixedDeltaTime, Vector3.back), 0.08f);
        else transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVector * Time.fixedDeltaTime * 0.01f, Vector3.back), Time.fixedDeltaTime * 2f);

        if (!ableToMove) return;
        rb.MovePosition(newPosition);
    }

    #region Attacks

    public void UseNormalAttack() {
        NormalAttack01();
    }

    void NormalAttack01() {
        // Shoot Slime...
        Vector3 pos = new Vector3(shootingPoint.transform.position.x, shootingPoint.transform.position.y);
        GameObject slime = enemyManager.SpawnSingleEnemy(pos, transform.rotation);
        slime.GetComponent<SlimeController>().isCharging = true;
        slime.GetComponent<SlimeController>().ChangeDirection(lastMovedDirection.x, lastMovedDirection.y);
    }

    public void EndNormalAttack() {
        isAttacking = false;
        canUseNormalShot = false;
        startNormalShotCooldown = true;
        normalShotCounter = 0f;
        previousState = currentState;
        currentState = EvilMageStates.Locked;

        if (normalShotAttacksCount >= normalShotAttacksAmount) {
            normalShotAttacksCount = 0;
            normalShotAttacksAmount = Random.Range(4, 9);
            walkCounter = Random.Range(3f, 6f);
            isWalking = true;
            walkCounter = 0f;
        }

    }

    public void EndSpecialAttack() {
        isAttacking = false;
        canUseSpecialShot = false;
        startSpecialShotCooldown = true;
        specialShotCounter = 0f;
        previousState = currentState;
        currentState = EvilMageStates.Dizzy;
    }

    public void EndDizzyState() {
        canUseNormalShot = false;
        startNormalShotCooldown = true;
        normalShotCounter = 0f;
        canUseSpecialShot = false;
        startSpecialShotCooldown = true;
        specialShotCounter = 0f;
        previousState = currentState;
        currentState = EvilMageStates.Locked;
    }

    #endregion
    
    public override void ReceiveDamage(float amount) {
        base.ReceiveDamage(amount);
        if (currentState == EvilMageStates.Idle) {
            previousState = currentState;
            currentState = EvilMageStates.Moving;
            isWalking = true;
            startNormalShotCooldown = true;
            startSpecialShotCooldown = true;
        }
    }

    public override void KillBoss() {
        previousState = currentState;
        currentState = EvilMageStates.Dying;
        PlayDeathAnimation();
    }

    public override void UpdateAnimator() {
        anim.SetFloat("movement", movement);
    }

    public void OnPlayerDeath() {
        pController.playerDeath -= OnPlayerDeath;
        previousState = currentState;
        currentState = EvilMageStates.Winning;
        anim.CrossFade("Victory", 1f);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        pController.playerDeath -= OnPlayerDeath;
    }

}
