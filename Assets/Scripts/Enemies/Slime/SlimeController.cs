using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    [Header ("States")]
    public bool isMoving;
    public bool isIdle;

    private Rigidbody2D rb;
    private Vector2 direction;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ChangeDirecionOnStart();
    }

    // Update is called once per frame
    void Update()
    {
        var inputVector = direction;
        var movementSpeed = 0.2f;
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

        rb.MovePosition(newPosition);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVector * Time.deltaTime, Vector3.back), 0.05f);
    }

    void ChangeDirecionOnStart() {
        float randX = Random.Range(-1f, 1f);
        float randY = Random.Range(-1f, 1f);
        direction = new Vector2(randX, randY);
    }
    void ChangeDirectionOnHit(float normalX, float normalY) {
        float randX, randY;

        if (normalX > 0) randX = Random.Range(0, 1f);
        else if (normalX < 0) randX = Random.Range(-1f, 0);
        else randX = Random.Range(-1f, 1f);

        if (normalY > 0) randY = Random.Range(0, 1f);
        else if (normalY < 0) randY = Random.Range(-1f, 0);
        else randY = Random.Range(-1f, 1f);

        direction = new Vector2(randX, randY);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.transform.tag == "Collider") { 
            //print("HIT: " + collision.GetContact(0).normal);
            ChangeDirectionOnHit(collision.GetContact(0).normal.x, collision.GetContact(0).normal.y);
        }
    }

}
