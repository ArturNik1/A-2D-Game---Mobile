using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public int id;
    public bool isFree;

    public float shotSpeed = 1.25f;
    public float lifeTime = 5.0f;
    private float currentTime = 0;

    [HideInInspector]
    public PlayerController pController;
    private Rigidbody2D rb;
    [HideInInspector]
    public Vector2 direction;

    // Start is called before the first frame update
    void OnEnable() {
        rb = GetComponent<Rigidbody2D>();
        currentTime = 0;
    }

    // Update is called once per frame
    void Update() {
        currentTime += Time.deltaTime;
        if (currentTime >= lifeTime) {
            pController.ResetProjectile(id);
        }

        var inputVector = direction;
        if (inputVector == Vector2.zero) inputVector = Vector2.up;
        var movementOffSet = inputVector.normalized * shotSpeed * Time.fixedDeltaTime;
        var newPosition = rb.position + movementOffSet;

        rb.MovePosition(newPosition);
    }

    void DoDamage(int amount, GameObject target, Collision2D collision) { 
        target.GetComponent<EnemyController>().ReceiveDamage(amount);
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.tag == "Collider" || collision.transform.tag == "Item") {
            if (!collision.transform.name.StartsWith("Wall") && collision.transform.tag != "Item") DoDamage(5, collision.gameObject, collision);
            pController.ResetProjectile(id);
        }
    }

}
