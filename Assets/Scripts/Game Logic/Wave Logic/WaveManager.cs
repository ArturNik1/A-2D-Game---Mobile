using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public int currentWave = 1;
    public float spawnedEnemies = 0;
    public float aliveEnemies = 0;

    public bool enemyDied = false;
    public bool waveEnabled = false;

    PlayerController pController;
    GameObject currentRoom;
    GameObject healthBar;
    GameObject enemyParent;
    GameObject[] enemyPrefabs;

    // Start is called before the first frame update
    void Start() {
        pController = GameObject.Find("Player").GetComponent<PlayerController>();
        healthBar = GameObject.Find("Canvas").transform.Find("Boss Health Slider").gameObject;
        enemyParent = GetComponent<EnemyManager>().enemyParent;
        enemyPrefabs = GetComponent<EnemyManager>().enemyPrefabs;
    }

    // Update is called once per frame
    void Update()
    {
        if (!waveEnabled) return;

        if (enemyDied) {
            enemyDied = false;
            healthBar.GetComponent<Slider>().value = aliveEnemies / spawnedEnemies;

            if (aliveEnemies == 0) { 
                if (currentWave < 3) {
                    currentWave++;
                    healthBar.transform.Find("Boss Name Text").GetComponent<Text>().text = "WAVE " + currentWave;
                    StartCoroutine(FillUpHealthBar());
                    // Start next wave - do wave start health bar animation...
                }
                else {
                    // End Waves..
                    pController.currentRoomMain.cleared = true;
                    waveEnabled = false;
                    healthBar.SetActive(false);
                    // Lightning strike doors + sound + change them
                    Invoke("DoorSequence", 0.5f);
                }
            }
        }
    }

    void DoorSequence() {
        pController.currentRoomMain.gameObject.transform.Find("Wall_U").transform.Find("Door_Lightning").GetComponent<ParticleSystem>().Play();
        AudioManager.instance.Play("DoorLightning");
        // Change Doors..
    }

    IEnumerator FillUpHealthBar() {
        AudioManager.instance.Play("WaveFillingUp");
        var value = healthBar.GetComponent<Slider>();
        value.value = 0;
        while (true) {
            value.value += Time.deltaTime / 3f;
            if (value.value >= 1.0f) break;
            yield return new WaitForEndOfFrame();
        }
        AudioManager.instance.StopPlaying("WaveFillingUp");
        SpawnWaveEnemies(currentRoom);
    }

    public void SpawnFirstWave() {
        waveEnabled = true;

        currentWave = 1;
        spawnedEnemies = 5;
        aliveEnemies = 5;

        currentRoom = pController.currentRoomObject;

        healthBar.SetActive(true);
        healthBar.GetComponent<Slider>().value = aliveEnemies / spawnedEnemies;
        healthBar.transform.Find("Boss Name Text").GetComponent<Text>().text = "WAVE 1";

        // Spawn enemies....
        SpawnWaveEnemies(currentRoom);
    }

    void SpawnWaveEnemies(GameObject room) {
        AudioManager.instance.Play("EnemySpawn02");
        switch (LevelManager.currentWorld) {
            case 0:
                RollAndSpawnWorldOne(room);
                break;
        }

    }

    #region WorldOne
    void RollAndSpawnWorldOne(GameObject room) {
        Transform wall_R = room.transform.Find("Wall_R");
        Transform wall_L = room.transform.Find("Wall_L");
        Transform wall_U = room.transform.Find("Wall_U");
        Transform wall_B = room.transform.Find("Wall_B");
        float middle_X = (wall_R.position.x + wall_L.position.x) / 2f;
        float maxPerHalf_X = wall_R.position.x - middle_X - wall_R.GetComponentInChildren<BoxCollider>().size.z; // z instead of x
        float middle_Y = (wall_U.position.y + wall_B.position.y) / 2f;
        float maxPerHalf_Y = wall_U.position.y - middle_Y - wall_U.GetComponentInChildren<BoxCollider>().size.x; // x instead of y

        GameObject enemy;

        int rand = Random.Range(0, 7);
        switch (rand) {

            #region Pattern 1
            case 0:

                aliveEnemies = 12;
                spawnedEnemies = 12;

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region Pattern 2
            case 1:

                aliveEnemies = 16;
                spawnedEnemies = 16;

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 45, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 45, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 45, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 45, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region Pattern 3
            case 2:

                aliveEnemies = 12;
                spawnedEnemies = 12;

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region Pattern 4
            case 3:

                aliveEnemies = 10;
                spawnedEnemies = 10;

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 25, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 75, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 25, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 75, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 25, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 75, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 25, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 75, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 60, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region Pattern 5
            case 4:

                aliveEnemies = 8;
                spawnedEnemies = 8;

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 75, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 75, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 75, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 75, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region Pattern 6
            case 5:

                aliveEnemies = 8;
                spawnedEnemies = 8;

                enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 65, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 65, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 65, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 65, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 65, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 65, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 65, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 65, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region Pattern 7
            case 6:

                aliveEnemies = 8;
                spawnedEnemies = 8;

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 65, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 65, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 65, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 65, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 65, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 65, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[4], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 65, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 65, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
        }

    }
    #endregion


    float CalculateByPercentOffMiddle_X(float middle, float maxPerHalf, float percent, bool isRight) {
        if (isRight) {
            return middle + (percent * maxPerHalf / 100f);
        }
        else {
            return middle - (percent * maxPerHalf / 100f);
        }
    }
    float CalculateByPercentOffMiddle_Y(float middle, float maxPerHalf, float percent, bool isUp) {
        if (isUp) {
            return middle + (percent * maxPerHalf / 100f);
        }
        else {
            return middle - (percent * maxPerHalf / 100f);
        }
    }


}
