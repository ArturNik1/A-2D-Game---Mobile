using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public int id;
    public bool isFree;

    public static float damageAmount = 5.0f;
    public static float critProcChance = 10f; // 10 percent at base...
    public static float critMultiplier = 1.1f; // 10 percent at base...
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

    void DoDamage(GameObject target, Collision2D collision) {
        if (Random.Range(1, 101) <= critProcChance) // Crit attack... 0% at base...
        { 
            target.GetComponent<EnemyController>().ReceiveDamage(damageAmount * critMultiplier);
            target.GetComponent<EnemyController>().particle_crit.Play();
        }
        else
            target.GetComponent<EnemyController>().ReceiveDamage(damageAmount);
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.tag == "Collider" || collision.transform.tag == "Item") {
            if (!collision.transform.name.StartsWith("Wall") && collision.transform.tag != "Item") DoDamage(collision.gameObject, collision);
            pController.ResetProjectile(id);
        }
    }

}
