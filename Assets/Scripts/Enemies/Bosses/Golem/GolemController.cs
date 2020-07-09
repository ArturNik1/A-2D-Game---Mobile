using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GolemController : BossController
{
    public enum GolemStates { Idle, Scanning, Moving, Locked, Attacking, Dizzy, Winning, Dying }
    GolemStates previousState;
    GolemStates currentState;

    [Header("Animation Variables")]
    public float movement = 0f;
    
    GameObject lockedPlayer;
    EnemyManager enemyManager;
    FieldOfView fov;
    Vector2 lastMovedDirection;

    [Header("Variables")]
    [SerializeField] float smashAttackCooldown = 10f;
    [SerializeField] float dizzyCouterMax = 10f;
    [SerializeField] float dizzyCouterMaxShort = 2f;
    [SerializeField] float dizzyHitMax = 10f;
    [SerializeField] float chaseTimeMax = 10f;

    float smashCounter = 0f;
    bool canSmash = false;
    public bool isSmashing = false;
    bool inSmashingCoro = false;
    bool inAttackingDelayCoro = false;

    float chaseCounter = 0f;
    float dizzyCounter = 0f;
    int dizzyHitCounter = 0;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        currentState = GolemStates.Idle;
        pController.playerDeath += OnPlayerDeath;

        healthBar.SetActive(true);
        healthBar.GetComponent<Slider>().value = 1f;
        healthBar.transform.Find("Boss Name Text").GetComponent<Text>().text = "GOLEM";

        fov = GetComponent<FieldOfView>();
        enemyManager = GameObject.Find("Enemies").GetComponent<EnemyManager>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        HandleStates();
        UpdateAnimator();

        if (fov.isEnabled) {
            fov.SetOrigin(new Vector3 (transform.position.x, transform.position.y, transform.position.z - 0.1f));
            fov.SetAimDirection(lastMovedDirection);
        }

        IsAttacking();
        IsBeingHit();
        if (currentState != GolemStates.Idle) {

            smashCounter += Time.deltaTime;
            if (smashCounter >= smashAttackCooldown) {
                canSmash = true;
            }

        }
    }

    public override void FixedUpdate() {
        base.FixedUpdate();
        if (!pController.isAlive || currentState == GolemStates.Dying) return;

        if (currentState == GolemStates.Moving || currentState == GolemStates.Attacking && previousState == GolemStates.Moving) {
            FollowPlayer();
        }
    }

    void HandleStates() { 
        
        if (currentState == GolemStates.Idle) { 
        
        }

        else if (currentState == GolemStates.Scanning) { 
            if (lockedPlayer == null) {
                lockedPlayer = player;
            }
            previousState = currentState;
            currentState = GolemStates.Locked;
        }

        else if (currentState == GolemStates.Locked) {
            // Check for distance between lockedPlayer and golem. 
            // if lockedPlayer is too far + in range of smash attack + smash attack has no cooldown, attack with it.
            // else change states to Moving.
            _speed = speed;

            float distance = Vector2.Distance(transform.position, lockedPlayer.transform.position);
            if (distance >= 0.5f && distance <= 1.5f && canSmash) { 
                print("Using Smash Attack...");
                previousState = currentState;
                currentState = GolemStates.Attacking;
            }
            else {
                print("Moving Towards Player...");
                previousState = currentState;
                currentState = GolemStates.Moving;
            }
        }

        else if (currentState == GolemStates.Moving) {
            // Go towards the lockedPlayer, if in 10 seconds the golem can't close the gap for a normal attack then change state to Dizzy.
            // if golem managed to get in range within 10 seconds, change to Attacking and use normal attack.

            movement = 0.325f;
            if (Vector2.Distance(transform.position, lockedPlayer.transform.position) < 0.5f) {
                // Attack...
                previousState = currentState;
                currentState = GolemStates.Attacking;
            }
            else {
                chaseCounter += Time.deltaTime;
                if (chaseCounter >= chaseTimeMax) {
                    chaseCounter = 0;
                    previousState = currentState;
                    currentState = GolemStates.Dizzy;
                    anim.SetBool("isDizzy", true);
                    anim.CrossFade("Dizzy", 0.5f);
                }
            }
        }

        else if (currentState == GolemStates.Attacking && previousState == GolemStates.Locked) {
            // Coming from Locked state straight into Attacking state, meaning Golem is using a smash attack.
            // Switch to Dizzy state after the smash attack, just for a second or two. 
            if (isAttacking || !pController.isAlive) return;
            // Short Delay....
            if (!inSmashingCoro) {
                inSmashingCoro = true;
                fov.isEnabled = true;
                fov.SetFoVAndViewDistance(45, 5);
                StartCoroutine(DelaySmashAttack(0.5f));
            }
            if (!isSmashing) return;
            UseSmashAttack();
            StartCoroutine(WaitForEndSmashAttack());
        }

        else if (currentState == GolemStates.Attacking && previousState == GolemStates.Moving) {
            // Coming from Moving state to Attacking state, meaning Golem managed to close the distance gap within the 10 seconds. 
            // Use normal attack (while moving?) and continue back to Locked State.
            if (isAttacking) return;

            //chaseCounter = 0;
            UseNormalAttack();
            StartCoroutine(WaitForEndAttack());
        }

        else if (currentState == GolemStates.Attacking && previousState == GolemStates.Dizzy) {
            // Charged attack...
            if (isAttacking || !pController.isAlive) return;

            if (!inSmashingCoro) {
                inSmashingCoro = true;
                fov.isEnabled = true;
                fov.SetFoVAndViewDistance(60, 5);
                StartCoroutine(DelaySmashAttack(0.5f));
            }
            if (!isSmashing) return;
            UseSuperChargedAttack();
            StartCoroutine(WaitForEndSpecialAttack());
        }

        else if (currentState == GolemStates.Dizzy && previousState == GolemStates.Moving) {
            // Coming from Moving state to Dizzy state, meaning Golem didn't close the gap. 
            // Golem will be in this state and animation for a few seconds/a few hits (eachever will be first). 
            // Use charged smash (this puts smash attack on cooldown) and spawn enemies at the end of this state. Continue to locked state.

            dizzyCounter += Time.deltaTime;
            if (dizzyCounter >= dizzyCouterMax || dizzyHitCounter >= dizzyHitMax) {
                dizzyCounter = 0f;
                dizzyHitCounter = 0;
                //UseSuperChargedAttack();
                // Switch at the end of animation...
                previousState = currentState;
                currentState = GolemStates.Attacking;
                anim.SetBool("isDizzy", false);
            }
        }

        else if (currentState == GolemStates.Dizzy && previousState == GolemStates.Attacking) {
            // Coming from Attacking state (normal smash) to Dizzy state, meaning Golem used smash and should be incapacitated for a moment.
            // As soon as that moment is over, Golem will return to Locked state.

            dizzyCounter += Time.deltaTime;
            if (dizzyCounter >= dizzyCouterMaxShort) {
                dizzyCounter = 0f;
                previousState = currentState;
                currentState = GolemStates.Locked;
                anim.SetBool("isDizzy", false);
            }
        }

        else if (currentState == GolemStates.Winning) {
            // This will be empty here, likely to setup an action event for player's death.
            isAttacking = false;
            isSmashing = false;
        }

        else if (currentState == GolemStates.Dying) { 
            // This will be empty here, no logic needed for now.
        }

    }

    #region Attacking

    void UseNormalAttack() {
        isAttacking = true;
        _speed = 0.25f;
        anim.CrossFade("NormalAttack", 0.1f, 2);
    }

    IEnumerator WaitForEndAttack() { 
        while (isAttacking) {
            yield return new WaitForEndOfFrame();
        }
        previousState = currentState;
        currentState = GolemStates.Locked;
    }

    void UseSmashAttack() {
        canSmash = false;
        smashCounter = 0f;
        // Attack...
        isAttacking = true;
        isSmashing = true;
        inSmashingCoro = false;
        anim.CrossFade("SpecialAttack", 0.1f, 2);
        Invoke("StartCameraShake", 1.125f);
    }

    void StartCameraShake() {
        particles["Slam"].Play();
        StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake(0.5f, 0.1f));
        fov.DetectInArea();
    }

    IEnumerator WaitForEndSmashAttack() {
        while (isAttacking) {
            yield return new WaitForEndOfFrame();
        }
        //fov.DetectInArea();
        previousState = currentState;
        currentState = GolemStates.Dizzy;
        anim.SetBool("isDizzy", true);
        anim.CrossFade("Dizzy", 0.5f);
        fov.isEnabled = false;
    }

    void SpawnEnemies() {
        enemyManager.SpawnBossEnemeis(bossRoom);
        AudioManager.instance.Play("EnemySpawn01");
    }

    IEnumerator WaitForEndSpecialAttack() { 
        while (isAttacking) {
            yield return new WaitForEndOfFrame();
        }
        //fov.DetectInArea();
        previousState = currentState;
        currentState = GolemStates.Dizzy;
        anim.SetBool("isDizzy", true);
        anim.CrossFade("Dizzy", 0.5f);
        fov.isEnabled = false;
        SpawnEnemies();
    }

    IEnumerator DelaySmashAttack(float seconds) {
        while (true) { 
            float posX = -(transform.position - lockedPlayer.transform.position).normalized.x, posY = -(transform.position - lockedPlayer.transform.position).normalized.y;
            Vector2 direction = new Vector2(posX, posY);
            lastMovedDirection = direction;
            var inputVector = direction;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVector * Time.fixedDeltaTime * 0.01f, Vector3.back), Time.fixedDeltaTime * 5f);
            if (Mathf.Abs(Vector2.Angle(transform.forward, lockedPlayer.transform.position - transform.position)) <= 7.5f) break;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(seconds);
        isSmashing = true;

    }

    void UseSuperChargedAttack() {
        canSmash = false;
        smashCounter = 0f;
        isAttacking = true;
        isSmashing = true;
        inSmashingCoro = false;
        anim.CrossFade("SpecialAttack", 0.1f, 2);
        Invoke("StartCameraShake", 1.125f);
    }

    void IsAttacking() {
        if (isAttacking) {
            if (anim.GetCurrentAnimatorStateInfo(2).IsName("NormalAttack") &&
                anim.GetCurrentAnimatorStateInfo(2).normalizedTime >= 0.93f || 
                anim.GetCurrentAnimatorStateInfo(2).IsName("SpecialAttack") &&
                anim.GetCurrentAnimatorStateInfo(2).normalizedTime >= 0.8f) {
                isAttacking = false;
                isSmashing = false;
            }
        }
    }


    #endregion

    void FollowPlayer() {
        if (isSmashing) return;

        float posX = -(transform.position - lockedPlayer.transform.position).normalized.x, posY = -(transform.position - lockedPlayer.transform.position).normalized.y;
        Vector2 direction = new Vector2(posX, posY);
        lastMovedDirection = direction;
        var inputVector = direction;
        var movementOffset = new Vector3(inputVector.x, inputVector.y, 0).normalized * _speed * Time.fixedDeltaTime;
        var newPosition = rb.position + movementOffset;

        if (!isAttacking) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVector * Time.fixedDeltaTime, Vector3.back), 0.1f);
        else transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVector * Time.fixedDeltaTime * 0.01f, Vector3.back), Time.fixedDeltaTime);

        if (Vector2.Distance(transform.position, lockedPlayer.transform.position) <= 0.45f) return;

        rb.MovePosition(newPosition);
    }

    public override void ReceiveDamage(float amount) {
        base.ReceiveDamage(amount);
        if (currentState == GolemStates.Idle) { 
            previousState = currentState;
            currentState = GolemStates.Scanning;
        }
        if (currentState == GolemStates.Dizzy && previousState == GolemStates.Moving) {
            dizzyHitCounter++;
        }
    }

    public override void UpdateAnimator() {
        anim.SetFloat("movement", movement);
    }

    public override void KillBoss()
    {
        previousState = currentState;
        currentState = GolemStates.Dying;
        PlayDeathAnimation();
    }

    public override void OnCollisionEnter(Collision collision) {
        base.OnCollisionEnter(collision);
        if (collision.transform.name == "Player" && collision.GetContact(0).thisCollider.tag.Contains("Hand")) {
            if (isAttacking && !isSmashing && !inAttackingDelayCoro) {
                chaseCounter = 0;
                anim.CrossFade("New State", 0.25f, 2);
                StartCoroutine(SetIsAttackingDelay(0.5f));
            }
        }
    }

    IEnumerator SetIsAttackingDelay(float seconds) {
        inAttackingDelayCoro = true;
        yield return new WaitForSeconds(seconds);
        inAttackingDelayCoro = false;
        isAttacking = false;
    }

    public void OnPlayerDeath() {
        pController.playerDeath -= OnPlayerDeath;
        previousState = currentState;
        currentState = GolemStates.Winning;
        // Winning animation...
        anim.CrossFade("Victory", 1f);
    }

    public override void OnDestroy() {
        base.OnDestroy();
        pController.playerDeath -= OnPlayerDeath;
    }

}
