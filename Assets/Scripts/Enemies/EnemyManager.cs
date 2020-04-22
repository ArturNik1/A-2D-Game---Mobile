using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public enum EnemySpawnPattern { LargeH_4, LargeH_5, LargeH_6, Nothing}

    [Header("Enemies")]
    public GameObject[] enemyPrefabs; // 0 - slime ; 1 - snail ;

    GameObject enemyParent;

    // Start is called before the first frame update
    void Start()
    {
        if (enemyPrefabs.Length == 0) Debug.LogError("EnemyPrefab array is empty!");

        enemyParent = GameObject.Find("Enemies");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnEnemies(GameObject room) {
        if (room.GetComponent<RoomLogic>().cleared) return;
        RemoveAllEnemies();

        SpawnEnemiesAfterPatternRoll(RollForSpawnPattern(room), room);
    }

    EnemySpawnPattern RollForSpawnPattern(GameObject room) {
        RoomType type = room.GetComponent<RoomLogic>().roomType;
        int rand;
        switch (LevelManager.currentWorld) {
            case 0: // World one.

                if (type == RoomType.LargeH) {

                    rand = Random.Range(1, 3);
                    if (rand == 1) return EnemySpawnPattern.LargeH_4;
                    else if (rand == 2) return EnemySpawnPattern.LargeH_5;
                    else if (rand == 3) return EnemySpawnPattern.LargeH_6;

                }

                break;
            case 1:
                break;
        }
        return EnemySpawnPattern.Nothing;
    }

    void SpawnEnemiesAfterPatternRoll (EnemySpawnPattern pattern, GameObject room) {
        GameObject enemy;
        Transform wall_R = room.transform.Find("Wall_R");
        Transform wall_L = room.transform.Find("Wall_L");
        Transform wall_U = room.transform.Find("Wall_U");
        Transform wall_B = room.transform.Find("Wall_B");
        switch (pattern) {
            #region LargeH_4
            case EnemySpawnPattern.LargeH_4:

                enemy = Instantiate(enemyPrefabs[0], new Vector3(wall_R.position.x - 0.56f, wall_U.position.y - 0.56f, 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);
                enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(wall_L.position.x + 0.56f, wall_U.position.y - 0.56f, 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);
                enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(wall_R.position.x - 0.56f, wall_B.position.y + 0.56f, 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);
                enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(wall_L.position.x + 0.56f, wall_B.position.y + 0.56f, 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);
                enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                break;
            #endregion
            #region LargeH_5
            case EnemySpawnPattern.LargeH_5:

                enemy = Instantiate(enemyPrefabs[0], new Vector3(wall_R.position.x - 0.56f, wall_U.position.y - 0.56f, 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);
                enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(wall_L.position.x + 0.56f, wall_U.position.y - 0.56f, 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);
                enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(wall_R.position.x - 0.56f, wall_B.position.y + 0.56f, 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);
                enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(wall_L.position.x + 0.56f, wall_B.position.y + 0.56f, 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);
                enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                enemy = Instantiate(enemyPrefabs[0], new Vector3((wall_R.position.x + wall_L.position.x) / 2, (wall_U.position.y + wall_B.position.y) / 2, 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);
                enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                break;
            #endregion
            #region LargeH_6
            case EnemySpawnPattern.LargeH_6:

                enemy = Instantiate(enemyPrefabs[0], new Vector3(wall_R.position.x - 0.56f, wall_U.position.y - 0.56f, 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);
                enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(wall_L.position.x + 0.56f, wall_U.position.y - 0.56f, 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);
                enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(wall_R.position.x - 0.56f, wall_B.position.y + 0.56f, 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);
                enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                enemy = Instantiate(enemyPrefabs[0], new Vector3(wall_L.position.x + 0.56f, wall_B.position.y + 0.56f, 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);
                enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                enemy = Instantiate(enemyPrefabs[0], new Vector3((wall_R.position.x + wall_L.position.x) / 2, wall_U.position.y - 0.26f, 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);
                enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                enemy = Instantiate(enemyPrefabs[0], new Vector3((wall_R.position.x + wall_L.position.x) / 2, (wall_U.position.y + wall_B.position.y) / 2, 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);
                enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                break;
                #endregion
        }
    }

    void RemoveAllEnemies() {
        // Technically should not be called, a place holder for now.
        for (int i = 0; i < enemyParent.transform.childCount; i++) {
            Destroy(enemyParent.transform.GetChild(i).gameObject);
        }
    }

}
