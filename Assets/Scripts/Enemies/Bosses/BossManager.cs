using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    [Header("Bosses")]
    public GameObject[] bossPrefabs; // 0 - golem 

    public GameObject bossRoom;

    List<GameObject> availableBosses;
    List<GameObject> usedBosses = new List<GameObject>();

    GameObject enemyParent;
    public GameObject playerGameObject;
    PlayerController pController;

    TextCountdown textCountdown;

    // Start is called before the first frame update
    void Start()
    {
        if (bossPrefabs.Length == 0) Debug.LogError("BossPrefab array is empty!");

        availableBosses = bossPrefabs.ToList<GameObject>();

        enemyParent = gameObject;
        pController = playerGameObject.GetComponent<PlayerController>();
        textCountdown = GameObject.Find("Canvas").transform.Find("Group").transform.Find("CinematicPanel").GetComponent<TextCountdown>();
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
        int index = Random.Range(0, availableBosses.Count);
        usedBosses.Add(availableBosses[index]);

        for (int i = 0; i < bossPrefabs.Length; i++) {
            if (availableBosses[index] == bossPrefabs[i]) { 
                availableBosses.RemoveAt(index);
                return i;
            }
        }
        
        return 0;
    }

}
