using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailController : MonoBehaviour
{
    [Header("States")]
    public bool isMoving;
    public bool isIdle;

    private Rigidbody2D rb;
    private Vector2 direction;
    private GameObject player;

    public float startDelaySeconds;
    private float startDelayCounter;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        ChangeDirecionOnStart();
    }

    // Update is called once per frame
    void Update()
    {
        var inputVector = direction;
        var movementSpeed = 0.25f;
        var movementOffset = inputVector.normalized * movementSpeed * Time.fixedDeltaTime;
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

        if (startDelayCounter <= startDelaySeconds)
        { // Don't move if X seconds had not passed.
            startDelayCounter += Time.deltaTime;
            return;
        }

        rb.MovePosition(newPosition);
    }

    void ChangeDirecionOnStart() {
        float randX = Random.Range(-1f, 1f);
        float randY = Random.Range(-1f, 1f);
        direction = new Vector2(randX, randY);
    }

    void ChangeDirectionOnHit(float normalX, float normalY) {
        // going to player's general location.
        float posX = -(transform.position - player.transform.position).normalized.x, posY = -(transform.position - player.transform.position).normalized.y;
        posX = Random.Range(posX - 0.3f, posX + 0.3f);
        posY = Random.Range(posY - 0.3f, posY + 0.3f);

        direction = new Vector2(posX, posY);
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.tag == "Collider") {
            ChangeDirectionOnHit(collision.GetContact(0).normal.x, collision.GetContact(0).normal.y);
        }
    }


}
