using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OrcController : BossController
{
    public enum OrcStates { Idle, Locked, Moving, Attacking, Charging, Dizzy, Winning, Dying }
    OrcStates previousState;
    OrcStates currentState;

    [Header("Animation Variables")]
    public float movement = 0f;

    [Header("Variables")]
    [SerializeField] float chargeAttackCooldown = 10f;
    [SerializeField] float chaseTimeMax = 10f;
    [SerializeField] float NormalAttackCooldown = 1f;

    float chargeAttackCounter = 0f;
    float chaseTimeCounter = 0f;
    float normalAttackCounter = 0f;

    float _rotateSpeed = 0.08f;
    bool isAlive = true;

    public bool startNormalAttackCooldown = false;
    public bool isCharging = false;
    public bool isSpinStageTwo = false;
    bool canCharge = false;

    EnemyManager enemyManager;
    Vector2 lastMovedDirection;

    public override void Start()
    {
        base.Start();
        currentState = OrcStates.Idle;
        pController.playerDeath += OnPlayerDeath;

        healthBar.SetActive(true);
        healthBar.GetComponent<Slider>().value = 1f;
        healthBar.transform.Find("Boss Name Text").GetComponent<Text>().text = "ORC";

        enemyManager = GameObject.Find("Enemies").GetComponent<EnemyManager>();
    }

    public override void Update() {
        base.Update();

        if (!isAlive) return;

        HandleStates();
        UpdateAnimator();

        if (currentState != OrcStates.Idle) {
            chargeAttackCounter += Time.deltaTime;
            if (chargeAttackCounter >= chargeAttackCooldown) {
                canCharge = true;
            }
        }

        if (startNormalAttackCooldown) {
            normalAttackCounter += Time.deltaTime;
            if (normalAttackCounter >= NormalAttackCooldown) {
                normalAttackCounter = 0f;
                startNormalAttackCooldown = false;
            }
        }

    }

    public override void FixedUpdate() {
        base.FixedUpdate();
        if (!pController.isAlive || currentState == OrcStates.Dying) return;

        if (currentState == OrcStates.Moving || currentState == OrcStates.Attacking && previousState == OrcStates.Moving) {
            FollowPlayer();
        }
        else if (currentState == OrcStates.Charging) {
            FollowLastMovedDirection();
        }
    }


    void HandleStates() { 
        if (currentState == OrcStates.Idle) { 
        
        }

        else if (currentState == OrcStates.Locked) {
            // Check for distance
            // if player is too far + no cooldown on Charge, switch to Charging.
            // else switch to Moving.
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance >= 0.5f && canCharge) {
                print("Charge & Start spinning...");
                previousState = currentState;
                currentState = OrcStates.Charging;
            }
            else {
                // Move towards player
                previousState = currentState;
                currentState = OrcStates.Moving;
            }
        }

        else if (currentState == OrcStates.Moving) {
            // Just go towards player, when in range use Normal Attack.
            // if x amount of time passes and Orc does not catchup, switch to Locked.
            if (Vector2.Distance(transform.position, player.transform.position) <= 0.5f && !isAttacking) {

                if (chaseTimeCounter >= chaseTimeMax / 2f) chaseTimeCounter = chaseTimeMax / 2f;

                previousState = currentState;
                currentState = OrcStates.Attacking;
                return;
            }

            chaseTimeCounter += Time.deltaTime;
            if (chaseTimeCounter >= chaseTimeMax) {
                chaseTimeCounter = 0f;
                previousState = currentState;
                currentState = OrcStates.Locked;
            }
            else if (chaseTimeCounter >= chaseTimeMax / 2.5f) {
                _speed = speed * 1.5f;
                if (movement < 0.75f) movement += Time.deltaTime;
                return;
            }

            if (movement >= 0.325f) movement -= Time.deltaTime;
            else if (movement < 0.325f) movement += Time.deltaTime;
            _speed = speed;
        }

        else if (currentState == OrcStates.Attacking && previousState == OrcStates.Moving) {
            if (startNormalAttackCooldown) return;
            UseNormalAttack();
            previousState = currentState;
            currentState = OrcStates.Locked;
        }

        else if (currentState == OrcStates.Charging && previousState == OrcStates.Locked) {
            // Start spinning until hitting wall or something else, switch to Dizzy afterwards(short).
            if (movement > 0f) movement -= Time.deltaTime;
            else if (movement < 0f) movement = 0f;
            if (isAttacking) return;
            isAttacking = true;
            chaseTimeCounter = 0f;
            anim.CrossFade("Taunting", 0.1f, 1);
            Invoke("UseChargeAttack", 1.75f);
        }

        else if (currentState == OrcStates.Dizzy && previousState == OrcStates.Charging) { 
            // Stay Dizzy for a second or two, switch to Locked afterwards.
        }

        else if (currentState == OrcStates.Winning) { 
            // Empty for now.
        }

        else if (currentState == OrcStates.Dying) { 
            // Does not need logic.
        }

    }

    #region Damage And Attacking
    public override void ReceiveDamage(float amount) {
        base.ReceiveDamage(amount);
        if (currentState == OrcStates.Idle) {
            previousState = currentState;
            currentState = OrcStates.Locked;
        }
    }


    void UseNormalAttack() {
        SetRotateSpeed();
        isAttacking = true;
        StartCoroutine(DecreaseMovement(0.1f));
        _speed = 0.1f;

        if (chargeAttackCounter >= chargeAttackCooldown) chargeAttackCounter = chargeAttackCooldown * 0.75f;

        anim.CrossFade("Attack01", 0f, 1);
        Invoke("PlayNormalAttackSound", 0.25f);
    }

    void PlayNormalAttackSound() { 
        AudioManager.instance.Play("OrcNormal01");
    }

    IEnumerator DecreaseMovement(float target) {
        while (movement >= target) {
            movement -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public void EndNormalAttack() {
        startNormalAttackCooldown = true;
        isAttacking = false;
        _speed = speed;
    }


    void UseChargeAttack() {
        isAttacking = true;
        isCharging = true;
        _speed *= 1.75f;
        anim.CrossFade("Attack02_Start", 0.5f, 1);
        anim.SetBool("spinning", true);
        chargeAttackCounter = 0f;
    }

    public void EndChargeAttack() {
        if (!isCharging) return;
        isAttacking = false;
        isCharging = false;
        StartCoroutine(SpinCycleDone());
        
        _speed = speed;
        chargeAttackCounter = 0f;
        canCharge = false;

        previousState = currentState;
        currentState = OrcStates.Dizzy;
        anim.CrossFade("Dizzy", 0.1f, 0);
    }

    IEnumerator SpinCycleDone() {
        while (true) {
            bool name = anim.GetCurrentAnimatorStateInfo(1).IsName("Attack02_Spin");
            float t = anim.GetCurrentAnimatorStateInfo(1).normalizedTime % 1;
            if (name && t <= 0.05f ||
                name && t >= 0.3f && t <= 0.35f ||
                name && t >= 0.65f && t <= 0.7f ||
                name && t >= 0.99f) {
                anim.SetBool("spinning", false);
                break;
            }
            else if (!name) break;
            yield return new WaitForEndOfFrame();
        }
        chargeAttackCounter = 0f;
    }

    public void SwitchToLocked() {
        chaseTimeCounter = 0f;
        previousState = currentState;
        currentState = OrcStates.Locked;
    }

    #endregion


    #region Movement
    void FollowPlayer() {
        float posX = -(transform.position - player.transform.position).normalized.x, posY = -(transform.position - player.transform.position).normalized.y;
        Vector2 direction = new Vector2(posX, posY);
        lastMovedDirection = direction;
        var inputVector = direction;
        var movementOffset = new Vector3(inputVector.x, inputVector.y, 0).normalized * _speed * Time.fixedDeltaTime;
        var newPosition = rb.position + movementOffset;

        if (!isAttacking) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVector * Time.fixedDeltaTime, Vector3.back), _rotateSpeed);
        else transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVector * Time.fixedDeltaTime * 0.01f, Vector3.back), Time.fixedDeltaTime / 2);

        rb.MovePosition(newPosition);
    }

    public void SetRotateSpeed() {
        Vector2 dir = (transform.position - player.transform.position).normalized;
        float dot = Vector2.Dot(dir, transform.forward);

        _rotateSpeed = dot >= -0.1f ? 0.04f : 0.08f;
    }

    void FollowLastMovedDirection() {
        float posX = -(transform.position - player.transform.position).normalized.x, posY = -(transform.position - player.transform.position).normalized.y;
        Vector2 direction = new Vector2(posX, posY);
        if (!isCharging) lastMovedDirection = direction;
        var inputVector = lastMovedDirection;
        var movementOffset = new Vector3(inputVector.x, inputVector.y, 0).normalized * _speed * Time.fixedDeltaTime;
        var newPosition = rb.position + movementOffset;

        if (!isCharging) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVector * Time.fixedDeltaTime, Vector3.back), 0.1f);
        else transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVector * Time.fixedDeltaTime * 0.01f, Vector3.back), Time.fixedDeltaTime);

        if (!isCharging) return;
        rb.MovePosition(newPosition);
    }

    #endregion

    public override void PlayHitAnimation()
    {
        if (isAttacking) return;
        anim.CrossFade("GetHit", 0.05f, 2);
        isHit = true;
    }

    public override void KillBoss()
    {
        isAlive = false;
        anim.CrossFade("New State", 0.1f, 1);
        previousState = currentState;
        currentState = OrcStates.Dying;
        PlayDeathAnimation();
    }

    public override void UpdateAnimator()
    {
        anim.SetFloat("movement", movement);
    }

    public void OnPlayerDeath() {
        pController.playerDeath -= OnPlayerDeath;
        previousState = currentState;
        currentState = OrcStates.Winning;
        // Winning animation...
        anim.CrossFade("Victory", 1f);
    }

    public override void OnDestroy() {
        base.OnDestroy();
        pController.playerDeath -= OnPlayerDeath;
    }

    public override void PlayDeathAnimation()
    {
        base.PlayDeathAnimation();
        stats.KillOrcs++;
        stats.KillTotalBosses++;
    }

}
