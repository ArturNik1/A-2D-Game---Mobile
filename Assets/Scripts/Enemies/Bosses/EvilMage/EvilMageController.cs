using Lean.Pool;
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

    bool firstUpdate = true;
    bool isTaunting = false;
    bool isWinning = false;
    bool isAlive = true;

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
    Transform projectiles;

    [Header ("Objects")]
    #pragma warning disable CS0649
    [SerializeField] GameObject shootingPoint;
    [SerializeField] GameObject fireball;
    [SerializeField] GameObject fireballCharge;
    [SerializeField] GameObject lightingStrike;
    #pragma warning restore CS0649

    public override void Start() {
        base.Start();
        currentState = EvilMageStates.Idle;
        pController.playerDeath += OnPlayerDeath;

        healthBar.SetActive(true);
        healthBar.GetComponent<Slider>().value = 1f;
        healthBar.transform.Find("Boss Name Text").GetComponent<Text>().text = "MAGE";

        enemyManager = GameObject.Find("Enemies").GetComponent<EnemyManager>();
        projectiles = GameObject.Find("ProjectilesEnemy").transform;

        normalShotAttacksAmount = Random.Range(4, 9);
    }

    public override void Update() {
        base.Update();

        if (!isAlive) return;

        if (firstUpdate) {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(0, -1, 0) * Time.fixedDeltaTime, Vector3.back), 1);
            firstUpdate = false;
        }

        if (isWinning) currentState = EvilMageStates.Winning;

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

        if (!isAttacking) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVector * Time.fixedDeltaTime, Vector3.back), 0.14f);
        else transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVector * Time.fixedDeltaTime * 0.01f, Vector3.back), Time.fixedDeltaTime * 2f);

        if (!ableToMove) return;
        rb.MovePosition(newPosition);
    }

    #region Attacks

    public void UseNormalAttack() {
        switch (normalShotAttacksAmount) {
            case 4:
                NormalAttack02();
                break;
            case 5:
                if (normalShotAttacksCount == 3) NormalAttack01();
                else NormalAttack02();
                break;
            case 6:
                if (normalShotAttacksCount == 1 || normalShotAttacksCount == 2) NormalAttack01();
                else NormalAttack02();
                break;
            case 7:
                if (normalShotAttacksCount == 4 || normalShotAttacksCount == 7) NormalAttack01();
                else NormalAttack02();
                break;
            case 8:
                if (normalShotAttacksCount == 3 || normalShotAttacksCount == 4 || normalShotAttacksCount == 7) NormalAttack01();
                else NormalAttack02();
                break;
        }
    }

    void NormalAttack01() {
        // Shoot Slime...
        Vector3 pos = new Vector3(shootingPoint.transform.position.x, shootingPoint.transform.position.y);
        GameObject slime = enemyManager.SpawnSingleEnemy(pos, transform.rotation);
        slime.GetComponent<SlimeController>().isCharging = true;
        slime.GetComponent<SlimeController>().ChangeDirection(lastMovedDirection.x, lastMovedDirection.y);
    }

    void NormalAttack02() {
        Vector3 originalPos = shootingPoint.transform.localPosition;
        shootingPoint.transform.localPosition = new Vector3(shootingPoint.transform.localPosition.x, 0.15f, 0);
        Vector3 pos = new Vector3(shootingPoint.transform.position.x, shootingPoint.transform.position.y);
        var obj = LeanPool.Spawn(fireball, pos, transform.rotation, projectiles);

        float posX = -(obj.transform.position - player.transform.position).normalized.x, posY = -(obj.transform.position - player.transform.position).normalized.y;
        Vector2 direction = new Vector2(posX, posY);
        obj.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction * Time.fixedDeltaTime, Vector3.back), 1);

        var fire = obj.GetComponent<FireballController>();
        fire.parent = gameObject;
        fire.isFromBoss = true;
        shootingPoint.transform.localPosition = originalPos;

        AudioManager.instance.Play("EvilMageNormal02");
    }

    public void UseSpecialAttack(float time) {
        if (Random.Range(0, 2) == 0) StartCoroutine(SpecialAttack01(time));
        else StartCoroutine(SpecialAttack02(time));
    }

    IEnumerator SpecialAttack01(float time) {
        Vector3 originalPos = shootingPoint.transform.localPosition;
        shootingPoint.transform.localPosition = new Vector3(shootingPoint.transform.localPosition.x, 0.15f, 0);
        Vector3 pos = new Vector3(shootingPoint.transform.position.x, shootingPoint.transform.position.y);
        var obj = LeanPool.Spawn(fireballCharge, pos, transform.rotation, projectiles);
        var fire = obj.GetComponent<FireballController>();
        fire.parent = gameObject;
        fire.isFromBoss = true;
        fire.shouldHold = true;
        shootingPoint.transform.localPosition = originalPos;
        obj.transform.localScale = Vector3.zero;

        AudioManager.instance.Play("EvilMageBuildFireball");

        float current = 0f;
        while (true) {
            current += Time.deltaTime;
            if (current >= time) break;
            obj.transform.localScale += Vector3.one * Time.deltaTime * 0.25f; 
            
            yield return new WaitForEndOfFrame();
        }

        AudioManager.instance.Play("EvilMageShootFireball");

        obj.GetComponent<FireballController>().shouldHold = false;
    }

    IEnumerator SpecialAttack02(float time) {
        Vector3 originalPos = shootingPoint.transform.localPosition;
        shootingPoint.transform.localPosition = new Vector3(shootingPoint.transform.localPosition.x, 0.15f, 0);
        Vector3 pos = new Vector3(shootingPoint.transform.position.x, shootingPoint.transform.position.y);
        var obj = LeanPool.Spawn(lightingStrike, pos, transform.rotation, projectiles);
        obj.transform.GetChild(1).transform.rotation = transform.rotation;
        var fire = obj.GetComponent<LightningStrikeController>();
        fire.shouldHold = true;
        shootingPoint.transform.localPosition = originalPos;
        obj.transform.localScale = Vector3.zero;

        AudioManager.instance.Play("EvilMageBuildLightning");

        float current = 0f;
        while (true) {
            current += Time.deltaTime;
            if (current >= time) break;
            obj.transform.localScale += Vector3.one * Time.deltaTime * 0.4f;

            yield return new WaitForEndOfFrame();
        }

        AudioManager.instance.Play("EvilMageShootLightning");
        AudioManager.instance.Play("EvilMageSparkLightning");

        obj.GetComponent<LightningStrikeController>().shouldHold = false;
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

    public void HitFloor(int num) { 
        if (num == 1) {
            AudioManager.instance.Play("EvilMageHitFloor01");
            return;
        }
        else if (num == 2) {
            AudioManager.instance.Play("EvilMageHitFloor02");
            return;
        }
    }

    IEnumerator HideBoss() {
        while (true) {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 0.001f);
            if (transform.localPosition.z >= 0.17f) {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion

    public override void ReceiveDamage(float amount) {
        if (currentState == EvilMageStates.Idle && !isTaunting) {
            anim.CrossFade("Taunt", 0.1f, 0);
            isTaunting = true;
            AudioManager.instance.Play("EvilMageLaugh");
            return;
        }
        else if (isTaunting) return;

        base.ReceiveDamage(amount);
    }

    public void ExitTauntAnimation() {
        previousState = currentState;
        currentState = EvilMageStates.Moving;
        isWalking = true;
        isTaunting = false;
        startNormalShotCooldown = true;
        startSpecialShotCooldown = true;
    }

    public override void KillBoss() {
        StartCoroutine(HideBoss());
        previousState = currentState;
        currentState = EvilMageStates.Dying;
        isAlive = false;
        PlayDeathAnimation();
    }

    public override void UpdateAnimator() {
        anim.SetFloat("movement", movement);
    }

    public void OnPlayerDeath() {
        pController.playerDeath -= OnPlayerDeath;
        isWinning = true;                       
        previousState = currentState;
        currentState = EvilMageStates.Winning;
        anim.CrossFade("Victory", 1f);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        pController.playerDeath -= OnPlayerDeath;

        GameObject obj;
        obj = GameObject.Find("LeanPool (Projectile_Fireball)");
        if (obj != null) Destroy(obj, 1f);

        obj = GameObject.Find("LeanPool (Projectile_FireballCharge)");
        if (obj != null) Destroy(obj, 1f);
    }

    public override void PlayDeathAnimation()
    {
        base.PlayDeathAnimation();
        stats.KillEvilMages++;
        stats.KillTotalBosses++;
    }

}
