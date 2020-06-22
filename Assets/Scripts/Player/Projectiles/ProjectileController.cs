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
    public static float damageCounter = 0;
    public float shotSpeed = 1.25f;
    public float lifeTime = 5.0f;
    private float currentTime = 0;

    [HideInInspector]
    public PlayerController pController;
    private Rigidbody rb;
    [HideInInspector]
    public Vector2 direction;

    // Start is called before the first frame update
    void OnEnable() {
        rb = GetComponent<Rigidbody>();
        currentTime = 0;
    }

    // Update is called once per frame
    void Update() {
        currentTime += Time.deltaTime;
        if (currentTime >= lifeTime) {
            pController.ResetProjectile(id);
        }

        if (LevelManager.inBossRoom) {
            // check for raycast above...
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -Vector3.forward, out hit, 10)) {
                if (hit.collider.tag.Contains("UnHittable")) {
                    pController.ResetProjectile(id);
                } 
                else if (hit.collider.tag == "BossHittable") {
                    if (hit.collider.name == "Back") DoDamageBoss(hit.transform.gameObject, true);
                    else  DoDamageBoss(hit.transform.gameObject);
                    pController.ResetProjectile(id);
                }
            }
        }
    }

    void FixedUpdate() {
        var inputVector = direction;
        if (inputVector == Vector2.zero) inputVector = Vector2.up;
        var movementOffSet = new Vector3(inputVector.x, inputVector.y, 0).normalized * shotSpeed * Time.fixedDeltaTime;
        var newPosition = rb.position + movementOffSet;

        rb.MovePosition(newPosition);
    }

    void DoDamage(GameObject target) {
        if (Random.Range(1, 101) <= critProcChance)  { 
            target.GetComponent<EnemyController>().ReceiveDamage(damageAmount * critMultiplier);
            target.GetComponent<EnemyController>().particles["Crit"].Play();
        }
        else { 
            target.GetComponent<EnemyController>().ReceiveDamage(damageAmount);
        }
    }

    void DoDamageChest(GameObject target) {
        if (Random.Range(1, 101) <= critProcChance) {
            target.GetComponent<ChestController>().ReceiveDamage(damageAmount * critMultiplier);
            target.GetComponent<ChestController>().particles["Crit"].Play();
        }
        else {
            target.GetComponent<ChestController>().ReceiveDamage(damageAmount);
        }
    }

    void DoDamageBoss(GameObject target, bool isBack = false) {
        float _critProcChance;
        if (isBack) _critProcChance = critProcChance * 3;
        else _critProcChance = critProcChance;
        if (Random.Range(1, 101) <= _critProcChance) {
            target.GetComponent<BossController>().ReceiveDamage(damageAmount * critMultiplier);
            target.GetComponent<BossController>().particles["Crit"].Play();
        } 
        else {
            target.GetComponent<BossController>().ReceiveDamage(damageAmount);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.tag == "Collider" || collision.transform.tag == "Item") {
            if (!collision.transform.name.StartsWith("Wall") && collision.transform.tag != "Item" && !collision.transform.name.Contains("Chest")) DoDamage(collision.gameObject);
            else if (collision.transform.name.Contains("Chest")) DoDamageChest(collision.gameObject);
            pController.ResetProjectile(id);
        }
        else if (collision.transform.tag == "BossCollider") {
            if (collision.GetContact(0).otherCollider.transform.tag == "BossHittable") DoDamageBoss(collision.gameObject, collision.GetContact(0).otherCollider.name == "Back" ? true : false);
            pController.ResetProjectile(id);
        }
    }

}
