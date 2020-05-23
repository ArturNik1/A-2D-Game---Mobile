using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    PlayerInputActions playerInputActions;
    Rigidbody2D rb;
    AnimatorManager playerAnim;
    PlayerInfo pInfo;
    public bool isAlive = true;

    [Header("Joystick")]
    public Joystick fixedJoystick;
    public Joystick floatingJoystick;
    [HideInInspector]
    public Joystick joystick;

    private Vector2 lastMovedJoystickDirection;
    private bool isOnMobile;

    [HideInInspector]
    public GameObject currentRoomObject;
    [HideInInspector]
    public RoomLogic currentRoomMain;

    [HideInInspector]
    public float movementSpeed;
    private float value;
    
    [HideInInspector]
    private bool canAttack = true;
    [HideInInspector]
    private bool canMove = true;

    [Header("GameObjects")]
    public GameObject projectileGameObject;
    public GameObject projectileHolder;
    public GameObject generalCollider;
    public Slider healthBar;

    Text healthText;

    private Dictionary<int, ProjectileController> projectiles_all = new Dictionary<int, ProjectileController>();
    private Dictionary<int, ProjectileController> projectiles_free = new Dictionary<int, ProjectileController>();
    private Dictionary<int, ProjectileController> projectiles_inUse = new Dictionary<int, ProjectileController>();
    private GameObject fireLocation;


    #region lifeCycle

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<AnimatorManager>();
        pInfo = GetComponent<PlayerInfo>();
        fireLocation = transform.Find("FireLocation").gameObject;

        healthText = healthBar.transform.Find("Health Text").GetComponent<Text>();

        isOnMobile = Application.isMobilePlatform;
    }

    private void OnEnable() {
        playerInputActions.Enable();
    }

    private void OnDisable() {
        playerInputActions.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (IsMoreProjectilesNeeded()) 
            GenerateProjectileBatch();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) return;

        PollInput();
    }

    private void LateUpdate()
    {
        if (!IsThrowing()) canAttack = true; // moved from Update()
    }

    #endregion

    #region Movement

    private void PollInput() {
        if (!canMove) return;

        Vector2 movementInputVector;
        if (isOnMobile) {
            movementInputVector = joystick.Direction;
            // Any time movementInputVector is not zero, rotate the player's transform.
            if (movementInputVector != Vector2.zero) {
                value = (Mathf.Atan2(joystick.Direction.x, joystick.Direction.y) / Mathf.PI) * 180f;
                if (value < 0) value += 360f;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, -value)), Time.deltaTime * 1000f);
                lastMovedJoystickDirection = movementInputVector;
            }
        }
        else {
            movementInputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
            // Any time movementInputVector is not zero, rotate the player's transform.
            if (movementInputVector != Vector2.zero) {
                value = (Mathf.Atan2(movementInputVector.x, movementInputVector.y) / Mathf.PI) * 180f;
                if (value < 0) value += 360f;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, -value)), Time.deltaTime * 1000f);
                lastMovedJoystickDirection = movementInputVector;
            }
        }
        Move(movementInputVector);

    }

    private void Move(Vector2 inputVector) {
        // Simply move player.
        var movementOffset = inputVector * movementSpeed * Time.fixedDeltaTime;
        var newPosition = rb.position + movementOffset;

        if (inputVector != Vector2.zero) {
            playerAnim.isMoving = true;
            playerAnim.isIdle = false;
        } else {
            playerAnim.isMoving = false;
            playerAnim.isIdle = true;
        }
        playerAnim.vertical = inputVector.y;
        playerAnim.horizontal = inputVector.x;

        rb.MovePosition(newPosition);
    }

    public void BlockMovement() {
        canMove = false;
    }

    public void UnBlockMovement(float time) {
        StartCoroutine(UnBlockMovementCoro(time));
    }
    IEnumerator UnBlockMovementCoro(float time) {
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    #endregion

    public void AttemptInteraction() {
        if (!canMove) return;
        // Attempt to interact with different interactables.
        RaycastHit2D[] raycastHit2Ds = Physics2D.CircleCastAll(new Vector2(transform.position.x, transform.position.y), 
            0.15f, new Vector2(transform.position.x, transform.position.y), 0.15f);
        for (int i = 0; i < raycastHit2Ds.Length; i++) {
            if (raycastHit2Ds[i].transform.name == "Player") continue;
            if (raycastHit2Ds[i].transform.tag == "Door") {
                raycastHit2Ds[i].transform.gameObject.GetComponent<DoorLogic>().HandleDoorAction();
                break;
            }
            else if (raycastHit2Ds[i].transform.tag == "Item") {
                raycastHit2Ds[i].transform.gameObject.GetComponent<Item>().PickUPItem();
                break;
            }
        }
    }

    #region Damage

    public void Attack() {
        if (canAttack && canMove) { // add !isThrowing() if there is the button issue.
            playerAnim.anim.CrossFade("Throw", 0f, 1);
            UseProjectile();
            canAttack = false;
        }
    }

    bool IsThrowing() {
        if (playerAnim.anim.GetCurrentAnimatorStateInfo(1).IsName("Throw") && playerAnim.anim.GetCurrentAnimatorStateInfo(1).normalizedTime < 1) return true;
        return false;
    }

    public void ReceiveDamage(int amount) {
        if (IsBeingHit()) return;

        AudioManager.instance.Play("PlayerHit0" + Random.Range(1, 4));

        pInfo.health -= amount;
        if (pInfo.health <= 0) {
            playerAnim.anim.CrossFade("Die", 0.1f);
            isAlive = false;
            canMove = false;
            rb.simulated = false;
            StartCoroutine(HidePlayer());
        }
        else {
            playerAnim.anim.CrossFade("GetHit", 0f, 1);
        }
        UpdateHealthBar();
    }

    public void UpdateHealthBar() {
        healthBar.value = ((float)pInfo.health / pInfo.maxHealth) + 0.046f;
        if (pInfo.health < 0) pInfo.health = 0;
        healthText.text = pInfo.health + "/" + pInfo.maxHealth;
    }

    bool IsBeingHit() {
        if (playerAnim.anim.GetCurrentAnimatorStateInfo(1).IsName("GetHit")) return true;
        return false;
    }

    IEnumerator HidePlayer() { 
        while (true) {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + 0.001f);
            if (transform.localPosition.z >= 0.05f) {
                GameObject.Find("Scene Transition").GetComponent<MenuLogic>().LoadScene();
                break;
            } 
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion

    #region Projectile Caching


    private bool IsMoreProjectilesNeeded() {
        if (projectiles_free.Count == 0 || projectiles_all.Count == 0) // if list in empty or last one in list is not free, return true.
            return true;
        return false;
    }

    private void GenerateProjectileBatch() {
        int sum = projectiles_all.Count;
        for (int i = sum; i < sum + 20; i++) {
            ProjectileController prj = Instantiate(projectileGameObject, projectileHolder.transform).GetComponent<ProjectileController>();
            prj.transform.position = new Vector3(-100, -100, -100);
            prj.id = i;
            prj.isFree = true;
            prj.pController = this;
            projectiles_all.Add(i, prj);
            projectiles_free.Add(i, prj);
            prj.gameObject.SetActive(false);
        }
    }

    public void ResetProjectile(int prjID) {
        if (!projectiles_inUse.ContainsKey(prjID)) return;
        ProjectileController prj = projectiles_inUse[prjID];

        prj.isFree = true;
        prj.transform.position = new Vector3(-100, -100, -100);
        projectiles_inUse.Remove(prjID);
        projectiles_free.Add(prjID, prj);
        prj.gameObject.SetActive(false);
    }

    public void ResetAllProjectiles() {
        if (projectiles_inUse.Count == 0) return;

        List<int> keyList = new List<int>(projectiles_inUse.Keys);
        keyList.Sort();

        for (int i = 0; i < keyList.Count; i++) {
            if (projectiles_inUse.ContainsKey(keyList[i]))
                ResetProjectile(keyList[i]);
        }
    }

    private void UseProjectile() {
        if (projectiles_free.Count == 0) GenerateProjectileBatch();

        List<int> keyList = new List<int>(projectiles_free.Keys);
        keyList.Sort();
        ProjectileController prj = projectiles_free[keyList[0]];

        prj.transform.position = fireLocation.transform.position;
        prj.transform.rotation = transform.rotation;
        prj.direction = lastMovedJoystickDirection;

        prj.isFree = false;
        projectiles_free.Remove(keyList[0]);
        projectiles_inUse.Add(keyList[0], prj);
        Physics2D.IgnoreCollision(generalCollider.GetComponent<BoxCollider2D>(), prj.GetComponent<CircleCollider2D>());
        prj.gameObject.SetActive(true);

        AudioManager.instance.Play("PlayerShoot01");
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawSphere(new Vector3 (transform.position.x, transform.position.y, transform.position.z), 0.15f);
    }

}
