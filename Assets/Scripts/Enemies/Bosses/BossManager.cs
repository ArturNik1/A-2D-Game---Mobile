using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    [Header("Bosses")]
    public GameObject[] bossPrefabs; // 0 - golem 

    public GameObject bossRoom;

    GameObject enemyParent;
    public GameObject playerGameObject;
    PlayerController pController;

    // Start is called before the first frame update
    void Start()
    {
        if (bossPrefabs.Length == 0) Debug.LogError("BossPrefab array is empty!");

        enemyParent = gameObject;
        pController = playerGameObject.GetComponent<PlayerController>();
    }

    public void SpawnBoss(GameObject room) {
        GameObject boss = Instantiate(bossPrefabs[DetermineBoss()], Vector3.zero, transform.rotation);
        boss.transform.SetParent(enemyParent.transform);
        boss.transform.LookAt(room.transform.Find("Wall_B").transform, -Vector3.forward);
        boss.transform.rotation = Quaternion.Euler(110f, boss.transform.rotation.eulerAngles.z, boss.transform.rotation.eulerAngles.y);
        bossRoom = room;
    }

    int DetermineBoss() { 
        if (LevelManager.currentWorld == 0) {
            return Random.Range(0, 3);
        }
        return 0;
    }

}
