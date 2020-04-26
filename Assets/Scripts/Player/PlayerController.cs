using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerController : MonoBehaviour
{

    private PlayerInputActions playerInputActions;
    private Rigidbody2D rb;
    private AnimatorManager playerAnim;

    public FixedJoystick joystick;
    private bool isOnMobile;

    [HideInInspector]
    public GameObject currentRoomObject;
    [HideInInspector]
    public RoomLogic currentRoomMain;

    [HideInInspector]
    public float movementSpeed;
    private float value;

    #region lifeCycle

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<AnimatorManager>();

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
        
    }

    // Update is called once per frame
    void Update()
    {
        PollInput();
    }

    #endregion
    #region input

    private void PollInput() {
        Vector2 movementInputVector;
        if (isOnMobile) {
            movementInputVector = joystick.Direction;
            // Any time movementInputVector is not zero, rotate the player's transform.
            if (movementInputVector != Vector2.zero) {
                value = (Mathf.Atan2(joystick.Direction.x, joystick.Direction.y) / Mathf.PI) * 180f;
                if (value < 0) value += 360f;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, -value)), Time.deltaTime * 1000f);
            }
        }
        else {
            movementInputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
            // Any time movementInputVector is not zero, rotate the player's transform.
            if (movementInputVector != Vector2.zero) {
                value = (Mathf.Atan2(movementInputVector.x, movementInputVector.y) / Mathf.PI) * 180f;
                if (value < 0) value += 360f;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, -value)), Time.deltaTime * 1000f);
            }
        }
        Move(movementInputVector);

        if (playerInputActions.Player.Interact.triggered) {
            // Player presses E, check if there is a door / chest in front.
            AttemptInteraction();
        }
    }

#endregion

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

    private void AttemptInteraction() {
        // Attempt to interact with different interactables.
        RaycastHit2D[] raycastHit2Ds = Physics2D.CircleCastAll(new Vector2(transform.position.x, transform.position.y), 
            0.15f, new Vector2(transform.position.x, transform.position.y), 0.15f);
        for (int i = 0; i < raycastHit2Ds.Length; i++) {
            if (raycastHit2Ds[i].transform.name == "Player") continue;
            if (raycastHit2Ds[i].transform.tag == "Door") {
                raycastHit2Ds[i].transform.gameObject.GetComponent<DoorLogic>().HandleDoorAction();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Gizmos.DrawSphere(new Vector3 (transform.position.x, transform.position.y, transform.position.z), 0.15f);
    }

}
