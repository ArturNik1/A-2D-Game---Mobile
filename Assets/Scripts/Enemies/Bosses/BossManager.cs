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

    TextCountdown textCountdown;

    // Start is called before the first frame update
    void Start()
    {
        if (bossPrefabs.Length == 0) Debug.LogError("BossPrefab array is empty!");

        enemyParent = gameObject;
        pController = playerGameObject.GetComponent<PlayerController>();
        textCountdown = GameObject.Find("Canvas").transform.Find("CinematicPanel").GetComponent<TextCountdown>();
    }

    public void SpawnBoss(GameObject room) {
        GameObject boss = Instantiate(bossPrefabs[DetermineBoss()], Vector3.zero, transform.rotation);
        boss.transform.SetParent(enemyParent.transform);
        boss.transform.LookAt(room.transform.Find("Wall_B").transform, -Vector3.forward);
        boss.transform.rotation = Quaternion.Euler(110f, boss.transform.rotation.eulerAngles.z, boss.transform.rotation.eulerAngles.y);
        bossRoom = room;
    }

    public void StartWaveCountdown() {
        // UI countdown and then start waves..
        StartCoroutine(textCountdown.DoCountdown(0f, this, pController));
    }

    public void StartWaves() {
        // 3 Waves...
        // Character should go to the middle, while looking around.
        // Start countdown when standing at the middle.
        // Have black bars when entering the room.
        // Boss bar should be wave progress.

        StartCoroutine(textCountdown.gameObject.GetComponent<CinematicBars>().Show(300, 0.3f));
        StartCoroutine(textCountdown.gameObject.GetComponent<CinematicBars>().Walk(this));
    }

    int DetermineBoss() { 
        if (LevelManager.currentWorld == 0) {
            return Random.Range(0, 3);
        }
        return 0;
    }

}
