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

        //Debug.DrawRay(new Vector3(transform.position.x + 0.0225f, transform.position.y, transform.position.z + 1f), -Vector3.forward, Color.red, 5f);
        //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 0.0225f, transform.position.z + 1f), -Vector3.forward, Color.red, 5f);
        //Debug.DrawRay(new Vector3(transform.position.x - 0.0225f, transform.position.y, transform.position.z + 1f), -Vector3.forward, Color.red, 5f);
        //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y - 0.0225f, transform.position.z + 1f), -Vector3.forward, Color.red, 5f);

        // check for raycast above and below in a 2x2 'box'....
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x + 0.0225f, transform.position.y, transform.position.z), -Vector3.forward, out hit, 5) || Physics.Raycast(new Vector3(transform.position.x - 0.0225f, transform.position.y, transform.position.z), -Vector3.forward, out hit, 5) ||
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.0225f, transform.position.z), -Vector3.forward, out hit, 5) || Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.0225f, transform.position.z), -Vector3.forward, out hit, 5) || 
            Physics.Raycast(new Vector3(transform.position.x + 0.0225f, transform.position.y, transform.position.z), Vector3.forward, out hit, 5) || Physics.Raycast(new Vector3(transform.position.x - 0.0225f, transform.position.y, transform.position.z), Vector3.forward, out hit, 5) ||
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.0225f, transform.position.z), Vector3.forward, out hit, 5) || Physics.Raycast(new Vector3(transform.position.x, transform.position.y - 0.0225f, transform.position.z), Vector3.forward, out hit, 5)) { // Up
            
            if (hit.collider.tag.Contains("UnHittable")) {
                print(hit.collider.name);
                if (hit.collider.name.Contains("Shield")) { 
                    hit.collider.GetComponentInParent<SkeletonController>().shieldHit = true;
                    DoDamage(hit.transform.gameObject);
                }
                pController.ResetProjectile(id);
            } 
            else if (hit.collider.tag == "BossHittable") {
                if (hit.collider.name == "Back") DoDamageBoss(hit.transform.gameObject, true);
                else  DoDamageBoss(hit.transform.gameObject);
                pController.ResetProjectile(id);
            }
            else if (hit.collider.tag == "ChestHittable") {
                if (!hit.collider.gameObject.GetComponentInParent<ChestController>().isEnemy) return;
                DoDamageChest(hit.transform.gameObject);
                pController.ResetProjectile(id);
            }
            else if (hit.collider.tag == "RegularHittable") {
                if (hit.collider.name.Contains("Shield")) hit.collider.GetComponentInParent<SkeletonController>().shieldHit = true;
                DoDamage(hit.transform.gameObject);
                pController.ResetProjectile(id);
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
            target.GetComponent<EnemyController>().damageToBeTaken.Add(damageAmount * critMultiplier);
            target.GetComponent<EnemyController>().particles["Crit"].Play();
        }
        else { 
            target.GetComponent<EnemyController>().damageToBeTaken.Add(damageAmount);
        }
    }

    void DoDamageChest(GameObject target) {
        if (Random.Range(1, 101) <= critProcChance) {
            target.GetComponent<ChestController>().damageToBeTaken.Add(damageAmount * critMultiplier);
            target.GetComponent<ChestController>().particles["Crit"].Play();
        }
        else {
            target.GetComponent<ChestController>().damageToBeTaken.Add(damageAmount);
        }
    }

    void DoDamageBoss(GameObject target, bool critZone = false) {
        float _critProcChance;
        if (critZone) _critProcChance = critProcChance * 3;
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
        if (collision.transform.tag == "Collider") 
        {
            if (!collision.GetContact(0).otherCollider.transform.tag.Contains("UnHittable") && !collision.transform.name.StartsWith("Wall")) 
            {
                if (collision.GetContact(0).otherCollider.transform.name.Contains("Shield")) collision.gameObject.GetComponentInParent<SkeletonController>().shieldHit = true;
                if (!collision.transform.name.Contains("Chest")) DoDamage(collision.gameObject);
                else DoDamageChest(collision.gameObject);
            }
            pController.ResetProjectile(id);
        }
        else if (collision.transform.tag == "Item") pController.ResetProjectile(id);
        else if (collision.transform.tag == "BossCollider")
        {
            if (collision.GetContact(0).otherCollider.transform.tag == "BossHittable") DoDamageBoss(collision.gameObject, collision.GetContact(0).otherCollider.name == "Back" ? true : false);
            pController.ResetProjectile(id);
        }
    }

}
