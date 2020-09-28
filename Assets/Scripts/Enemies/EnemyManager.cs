using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    //public enum EnemySpawnPattern { Small_3, Small_5, Medium_4, Medium_6, LargeH_4, LargeH_5, LargeH_6, LargeV_4, LargeV_5, LargeV_6, LargeV_7, Huge_5, Huge_6, Huge_7, Huge_8, Gigantic_8, Gigantic_10, Gigantic_12, Nothing}

    [Header("Enemies")]
    public GameObject[] enemyPrefabs; // 0 - slime ; 1 - snail 2 - chest;

    [HideInInspector] public GameObject enemyParent;
    public GameObject playerGameObject;
    PlayerController pController;

    public static List<GameObject> enemiesTouching = new List<GameObject>();

    List<int> enemySpawnIndex = new List<int>();
    private int chanceForOneType;

    // Start is called before the first frame update
    void Start()
    {
        if (enemyPrefabs.Length == 0) Debug.LogError("EnemyPrefab array is empty!");

        enemyParent = GameObject.Find("Enemies");
        pController = playerGameObject.GetComponent<PlayerController>();
    }

    public void SpawnEnemies(GameObject room) {
        if (room.GetComponent<RoomLogic>().cleared) return;

        RemoveAllEnemies();
        pController.ResetAllProjectiles();

        if (room.GetComponent<RoomLogic>().roomAction == RoomLogic.RoomAction.Boss) {
            // On odd worlds do waves, on even worlds do Bosses.
            if ((LevelManager.currentWorld + 1) % 2 != 0) gameObject.GetComponent<BossManager>().StartWaves();
            else gameObject.GetComponent<BossManager>().SpawnBoss(room);
            return;
        }
        else if (room.GetComponent<RoomLogic>().roomAction == RoomLogic.RoomAction.Special) {
            SpawnChest(room);
            return;
        }

        SpawnEnemiesAfterPatternRoll(EnemySpawnPatterns.RollForSpawnPattern(room), room);
    }

    void SpawnChest(GameObject room) {
        Transform wall_R = room.transform.Find("Wall_R");
        Transform wall_L = room.transform.Find("Wall_L");
        Transform wall_U = room.transform.Find("Wall_U");
        Transform wall_B = room.transform.Find("Wall_B");
        float middle_X = (wall_R.position.x + wall_L.position.x) / 2f;
        float maxPerHalf_X = wall_R.position.x - middle_X - wall_R.GetComponentInChildren<BoxCollider>().size.z; // z instead of x
        float middle_Y = (wall_U.position.y + wall_B.position.y) / 2f;
        float maxPerHalf_Y = wall_U.position.y - middle_Y - wall_U.GetComponentInChildren<BoxCollider>().size.x; // x instead of y

        Vector3 pos = new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0);

        GameObject chest = Instantiate(enemyPrefabs[2], pos, enemyPrefabs[2].transform.rotation);
        chest.transform.SetParent(enemyParent.transform);
        chest.GetComponent<ChestController>().room = room;
    }

    public void SpawnBossEnemeis(GameObject room) {
        Transform wall_R = room.transform.Find("Wall_R");
        Transform wall_L = room.transform.Find("Wall_L");
        Transform wall_U = room.transform.Find("Wall_U");
        Transform wall_B = room.transform.Find("Wall_B");
        float middle_X = (wall_R.position.x + wall_L.position.x) / 2f;
        float maxPerHalf_X = wall_R.position.x - middle_X - wall_R.GetComponentInChildren<BoxCollider>().size.z; // z instead of x
        float middle_Y = (wall_U.position.y + wall_B.position.y) / 2f;
        float maxPerHalf_Y = wall_U.position.y - middle_Y - wall_U.GetComponentInChildren<BoxCollider>().size.x; // x instead of y

        int enemyType;
        GameObject enemy;
        Vector3 pos;

        pos = new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0);
        if (Vector2.Distance(pos, pController.transform.position) >= 0.4f) { 
            enemyType = Random.Range(0, 2);
            enemy = Instantiate(enemyPrefabs[enemyType], pos, transform.rotation);
            enemy.transform.SetParent(enemyParent.transform);
        }

        pos = new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0);
        if (Vector2.Distance(pos, pController.transform.position) >= 0.4f) {
            enemyType = Random.Range(0, 2);
            enemy = Instantiate(enemyPrefabs[enemyType], pos, transform.rotation);
            enemy.transform.SetParent(enemyParent.transform);
        }

        pos = new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0);
        if (Vector2.Distance(pos, pController.transform.position) >= 0.4f) {
            enemyType = Random.Range(0, 2);
            enemy = Instantiate(enemyPrefabs[enemyType], pos, transform.rotation);
            enemy.transform.SetParent(enemyParent.transform);
        }

        pos = new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0);
        if (Vector2.Distance(pos, pController.transform.position) >= 0.4f) {
            enemyType = Random.Range(0, 2);
            enemy = Instantiate(enemyPrefabs[enemyType], pos, transform.rotation);
            enemy.transform.SetParent(enemyParent.transform);
        }
    }

    public GameObject SpawnSingleEnemy(Vector3 pos, Quaternion rot) {
        GameObject enemy;
        enemy = Instantiate(enemyPrefabs[0], pos, rot);
        enemy.transform.SetParent(enemyParent.transform);
        return enemy;
    }

    
    void SpawnEnemiesAfterPatternRoll (EnemySpawnPatterns.EnemySpawnPattern pattern, GameObject room) {
        GameObject enemy;
        Transform wall_R = room.transform.Find("Wall_R");
        Transform wall_L = room.transform.Find("Wall_L");
        Transform wall_U = room.transform.Find("Wall_U");
        Transform wall_B = room.transform.Find("Wall_B");
        float middle_X = (wall_R.position.x + wall_L.position.x) / 2f;
        float maxPerHalf_X = wall_R.position.x - middle_X - wall_R.GetComponentInChildren<BoxCollider>().size.z; // z instead of x
        float middle_Y = (wall_U.position.y + wall_B.position.y) / 2f;
        float maxPerHalf_Y = wall_U.position.y - middle_Y - wall_U.GetComponentInChildren<BoxCollider>().size.x; // x instead of y

        PopulateEnemySpawnIndexList(pattern);

        switch (pattern) {

            #region LargeH_4
            case EnemySpawnPatterns.EnemySpawnPattern.LargeH_4:

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region LargeH_5
            case EnemySpawnPatterns.EnemySpawnPattern.LargeH_5:

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 55, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 55, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 55, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 55, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[4]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region LargeH_6
            case EnemySpawnPatterns.EnemySpawnPattern.LargeH_6:

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[4]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[5]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 90, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region LargeV_4
            case EnemySpawnPatterns.EnemySpawnPattern.LargeV_4:

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region LargeV_5
            case EnemySpawnPatterns.EnemySpawnPattern.LargeV_5:

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[4]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region LargeV_6
            case EnemySpawnPatterns.EnemySpawnPattern.LargeV_6:

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 10, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[4]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 10, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[5]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 10, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region LargeV_7
            case EnemySpawnPatterns.EnemySpawnPattern.LargeV_7:

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 20, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[4]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 20, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[5]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 20, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[6]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region Huge_5
            case EnemySpawnPatterns.EnemySpawnPattern.Huge_5:

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[4]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region Huge_6
            case EnemySpawnPatterns.EnemySpawnPattern.Huge_6:

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 10, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 10, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 10, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[4]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[5]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region Huge_7
            case EnemySpawnPatterns.EnemySpawnPattern.Huge_7:

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[4]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[5]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[6]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region Huge_8
            case EnemySpawnPatterns.EnemySpawnPattern.Huge_8:

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[4]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[5]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[6]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[7]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region Gigantic_8
            case EnemySpawnPatterns.EnemySpawnPattern.Gigantic_8:

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[4]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[5]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[6]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[7]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region Gigantic_10
            case EnemySpawnPatterns.EnemySpawnPattern.Gigantic_10:

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[4]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[5]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[6]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[7]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[8]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[9]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region Gigantic_12
            case EnemySpawnPatterns.EnemySpawnPattern.Gigantic_12:

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[4]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[5]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[6]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[7]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[8]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[9]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[10]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[11]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region Small_3
            case EnemySpawnPatterns.EnemySpawnPattern.Small_3:

                if (room.GetComponent<RoomLogic>().hallwayIsLeft) {
                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 10, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                }

                else {
                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 10, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                }

                break;
            #endregion
            #region Small_5
            case EnemySpawnPatterns.EnemySpawnPattern.Small_5:

                if (room.GetComponent<RoomLogic>().hallwayIsLeft) {
                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 35, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[4]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 35, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                }

                else {
                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 35, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[4]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 35, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                }

                break;
            #endregion
            #region Medium_4
            case EnemySpawnPatterns.EnemySpawnPattern.Medium_4:

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                enemy.transform.SetParent(enemyParent.transform);

                break;
            #endregion
            #region Medium_6
            case EnemySpawnPatterns.EnemySpawnPattern.Medium_6:

                if (room.GetComponent<RoomLogic>().hallwayIsLeft) {
                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 10, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[4]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 10, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[5]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                }

                else {
                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[0]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[1]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[2]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[3]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 10, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[4]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 10, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);

                    enemy = Instantiate(enemyPrefabs[enemySpawnIndex[5]], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                }

                break;
            #endregion

        }
    }

    void PopulateEnemySpawnIndexList(EnemySpawnPatterns.EnemySpawnPattern pattern) {
        enemySpawnIndex = new List<int>();
        int currentWorld = LevelManager.currentWorld;

        int rand = Random.Range(0, 100);
        if (rand <= 20) {
            // Same type...
            // 0-Slime ; 1-Snail ; 2-Bat ; 3-Spider ; 4-Skeleton ; 5-Dragon
            int type = RandomToEnemyArrayIndex(Random.Range(0, currentWorld + 2));
            int amount = (int)char.GetNumericValue(pattern.ToString()[pattern.ToString().Length-1]);
            for (int i = 0; i < amount; i++) {
                enemySpawnIndex.Add(type);
            }
            return;
        }
        else if (currentWorld > 4) {
            int type;
            //type = RandomToEnemyArrayIndex(Random.Range(0, currentWorld + 2));
            int amount = (int)char.GetNumericValue(pattern.ToString()[pattern.ToString().Length - 1]);
            for (int i = 0; i < amount; i++) {
                type = RandomToEnemyArrayIndex(Random.Range(0, 6));
                enemySpawnIndex.Add(type);
            }
            return;
        }

        // Possible to add more variations to every pattern...
        switch (pattern) {
            // 0-Slime ; 1-Snail ; 2-Bat ; 3-Spider ; 4-Skeleton ; 5-Dragon
            case EnemySpawnPatterns.EnemySpawnPattern.LargeH_4:
                if (currentWorld == 0) enemySpawnIndex.AddRange(new int[] { 1, 1, 0, 0 });
                else if (currentWorld == 1) enemySpawnIndex.AddRange(new int[] { 2, 2, 1, 1 });
                else if (currentWorld == 2) enemySpawnIndex.AddRange(new int[] { 2, 2, 3, 3 });
                else if (currentWorld == 3) enemySpawnIndex.AddRange(new int[] { 2, 2, 4, 4 });
                else if (currentWorld == 4) enemySpawnIndex.AddRange(new int[] { 5, 5, 3, 3 });
                break;
            case EnemySpawnPatterns.EnemySpawnPattern.LargeH_5:
                if (currentWorld == 0) enemySpawnIndex.AddRange(new int[] { 1, 1, 0, 0, 1 });
                else if (currentWorld == 1) enemySpawnIndex.AddRange(new int[] { 2, 2, 2, 2, 0 });
                else if (currentWorld == 2) enemySpawnIndex.AddRange(new int[] { 1, 1, 2, 2, 3 });
                else if (currentWorld == 3) enemySpawnIndex.AddRange(new int[] { 2, 2, 4, 4, 3 });
                else if (currentWorld == 4) enemySpawnIndex.AddRange(new int[] { 5, 5, 4, 4, 5 });
                break;
            case EnemySpawnPatterns.EnemySpawnPattern.LargeH_6:
                if (currentWorld == 0) enemySpawnIndex.AddRange(new int[] { 1, 1, 0, 0, 0, 1 });
                else if (currentWorld == 1) enemySpawnIndex.AddRange(new int[] { 2, 2, 0, 0, 1, 2 });
                else if (currentWorld == 2) enemySpawnIndex.AddRange(new int[] { 2, 2, 0, 0, 3, 2 });
                else if (currentWorld == 3) enemySpawnIndex.AddRange(new int[] { 1, 1, 3, 3, 0, 4 });
                else if (currentWorld == 4) enemySpawnIndex.AddRange(new int[] { 4, 4, 5, 5, 5, 3 });
                break;
            case EnemySpawnPatterns.EnemySpawnPattern.LargeV_4:
                if (currentWorld == 0) enemySpawnIndex.AddRange(new int[] { 1, 1, 0, 0 });
                else if (currentWorld == 1) enemySpawnIndex.AddRange(new int[] { 2, 2, 0, 0 });
                else if (currentWorld == 2) enemySpawnIndex.AddRange(new int[] { 3, 3, 2, 2 });
                else if (currentWorld == 3) enemySpawnIndex.AddRange(new int[] { 4, 4, 3, 3 });
                else if (currentWorld == 4) enemySpawnIndex.AddRange(new int[] { 4, 4, 5, 5 });
                break;
            case EnemySpawnPatterns.EnemySpawnPattern.LargeV_5:
                if (currentWorld == 0) enemySpawnIndex.AddRange(new int[] { 1, 1, 0, 0, 0 });
                else if (currentWorld == 1) enemySpawnIndex.AddRange(new int[] { 1, 1, 1, 1, 0 });
                else if (currentWorld == 2) enemySpawnIndex.AddRange(new int[] { 2, 2, 3, 3, 3 });
                else if (currentWorld == 3) enemySpawnIndex.AddRange(new int[] { 4, 4, 0, 0, 3 });
                else if (currentWorld == 4) enemySpawnIndex.AddRange(new int[] { 2, 2, 2, 2, 5 });
                break;
            case EnemySpawnPatterns.EnemySpawnPattern.LargeV_6:
                if (currentWorld == 0) enemySpawnIndex.AddRange(new int[] { 1, 1, 1, 0, 0, 0 });
                else if (currentWorld == 1) enemySpawnIndex.AddRange(new int[] { 2, 2, 2, 0, 1, 0 });
                else if (currentWorld == 2) enemySpawnIndex.AddRange(new int[] { 2, 2, 2, 3, 1, 3 });
                else if (currentWorld == 3) enemySpawnIndex.AddRange(new int[] { 1, 4, 1, 2, 2, 2 });
                else if (currentWorld == 4) enemySpawnIndex.AddRange(new int[] { 4, 1, 4, 4, 5, 4 });
                break;
            case EnemySpawnPatterns.EnemySpawnPattern.LargeV_7:
                if (currentWorld == 0) enemySpawnIndex.AddRange(new int[] { 0, 1, 0, 0, 1, 0, 0 });
                else if (currentWorld == 1) enemySpawnIndex.AddRange(new int[] { 1, 0, 1, 0, 2, 0, 2 });
                else if (currentWorld == 2) enemySpawnIndex.AddRange(new int[] { 0, 2, 0, 3, 2, 3, 3 });
                else if (currentWorld == 3) enemySpawnIndex.AddRange(new int[] { 3, 1, 3, 2, 4, 2, 0 });
                else if (currentWorld == 4) enemySpawnIndex.AddRange(new int[] { 5, 4, 5, 4, 2, 4, 2 });
                break;
            case EnemySpawnPatterns.EnemySpawnPattern.Huge_5:
                if (currentWorld == 0) enemySpawnIndex.AddRange(new int[] { 1, 0, 1, 0, 0 });
                else if (currentWorld == 1) enemySpawnIndex.AddRange(new int[] { 1, 2, 1, 2, 2 });
                else if (currentWorld == 2) enemySpawnIndex.AddRange(new int[] { 3, 0, 3, 0, 3 });
                else if (currentWorld == 3) enemySpawnIndex.AddRange(new int[] { 3, 3, 3, 3, 4 });
                else if (currentWorld == 4) enemySpawnIndex.AddRange(new int[] { 5, 5, 5, 5, 4 });
                break;
            case EnemySpawnPatterns.EnemySpawnPattern.Huge_6:
                if (currentWorld == 0) enemySpawnIndex.AddRange(new int[] { 0, 0, 0, 1, 1, 1 });
                else if (currentWorld == 1) enemySpawnIndex.AddRange(new int[] { 0, 0, 0, 2, 1, 2 });
                else if (currentWorld == 2) enemySpawnIndex.AddRange(new int[] { 3, 0, 3, 0, 1, 0 });
                else if (currentWorld == 3) enemySpawnIndex.AddRange(new int[] { 2, 2, 2, 4, 4, 4 });
                else if (currentWorld == 4) enemySpawnIndex.AddRange(new int[] { 5, 3, 5, 4, 2 ,4 });
                break;
            case EnemySpawnPatterns.EnemySpawnPattern.Huge_7:
                if (currentWorld == 0) enemySpawnIndex.AddRange(new int[] { 0, 0, 0, 0, 1, 0, 0 });
                else if (currentWorld == 1) enemySpawnIndex.AddRange(new int[] { 2, 1, 2, 1, 2, 0, 0 });
                else if (currentWorld == 2) enemySpawnIndex.AddRange(new int[] { 3, 3, 3, 3, 2, 3, 3 });
                else if (currentWorld == 3) enemySpawnIndex.AddRange(new int[] { 2, 0, 2, 0, 2, 3, 3 });
                else if (currentWorld == 4) enemySpawnIndex.AddRange(new int[] { 4, 2, 4, 2, 5 ,2 ,2 });
                break;
            case EnemySpawnPatterns.EnemySpawnPattern.Huge_8:
                if (currentWorld == 0) enemySpawnIndex.AddRange(new int[] { 1, 0, 1, 0, 1, 0, 0, 1 });
                else if (currentWorld == 1) enemySpawnIndex.AddRange(new int[] { 1, 0, 1, 0, 1, 0, 0, 2 });
                else if (currentWorld == 2) enemySpawnIndex.AddRange(new int[] { 3, 1, 3, 1, 3, 0, 0, 0 });
                else if (currentWorld == 3) enemySpawnIndex.AddRange(new int[] { 2, 4, 2, 4, 1, 3, 3, 4 });
                else if (currentWorld == 4) enemySpawnIndex.AddRange(new int[] { 4, 2, 4, 2, 4, 0, 0, 3 });
                break;
            case EnemySpawnPatterns.EnemySpawnPattern.Gigantic_8:
                if (currentWorld == 0) enemySpawnIndex.AddRange(new int[] { 1, 1, 1, 1, 0, 0, 0, 0 });
                else if (currentWorld == 1) enemySpawnIndex.AddRange(new int[] { 1, 1, 1, 1, 2, 0, 2, 0 });
                else if (currentWorld == 2) enemySpawnIndex.AddRange(new int[] { 2, 2, 2, 2, 3, 3, 3, 3 });
                else if (currentWorld == 3) enemySpawnIndex.AddRange(new int[] { 3, 3, 3, 3, 0, 4, 0, 4 });
                else if (currentWorld == 4) enemySpawnIndex.AddRange(new int[] { 2, 1, 2, 1, 1, 5, 1, 5 });
                break;
            case EnemySpawnPatterns.EnemySpawnPattern.Gigantic_10:
                if (currentWorld == 0) enemySpawnIndex.AddRange(new int[] { 1, 0, 1, 0, 0, 0, 0, 0, 1, 1 });
                else if (currentWorld == 1) enemySpawnIndex.AddRange(new int[] { 1, 0, 1, 0, 1, 0, 1, 0, 1, 2 });
                else if (currentWorld == 2) enemySpawnIndex.AddRange(new int[] { 2, 2, 2, 2, 3, 3, 3, 3, 1, 3 });
                else if (currentWorld == 3) enemySpawnIndex.AddRange(new int[] { 2, 2, 2, 2, 4, 4, 4, 4, 1, 3 });
                else if (currentWorld == 4) enemySpawnIndex.AddRange(new int[] { 4, 4, 4, 4, 3, 5, 3, 5, 2, 5 });
                break;
            case EnemySpawnPatterns.EnemySpawnPattern.Gigantic_12:
                if (currentWorld == 0) enemySpawnIndex.AddRange(new int[] { 1, 1, 1, 1, 0, 0, 0, 0, 1, 0, 0, 0 });
                else if (currentWorld == 1) enemySpawnIndex.AddRange(new int[] { 2, 0, 2, 0, 2, 0, 2, 0, 2, 1, 2, 2 });
                else if (currentWorld == 2) enemySpawnIndex.AddRange(new int[] { 0, 3, 0, 3, 2, 2, 2, 2, 3, 2, 3, 3 });
                else if (currentWorld == 3) enemySpawnIndex.AddRange(new int[] { 4, 4, 4, 4, 0, 1, 0, 1, 4, 3, 3, 3 });
                else if (currentWorld == 4) enemySpawnIndex.AddRange(new int[] { 2, 2, 2, 2, 5, 4, 5, 4, 2, 4, 4, 4 });
                break;
            case EnemySpawnPatterns.EnemySpawnPattern.Small_3:
                if (currentWorld == 0) enemySpawnIndex.AddRange(new int[] { 1, 1, 0 });
                else if (currentWorld == 1) enemySpawnIndex.AddRange(new int[] { 2, 2, 0 });
                else if (currentWorld == 2) enemySpawnIndex.AddRange(new int[] { 2, 2, 3 });
                else if (currentWorld == 3) enemySpawnIndex.AddRange(new int[] { 4, 4, 1 });
                else if (currentWorld == 4) enemySpawnIndex.AddRange(new int[] { 4, 4, 5 });
                break;
            case EnemySpawnPatterns.EnemySpawnPattern.Small_5:
                if (currentWorld == 0) enemySpawnIndex.AddRange(new int[] { 1, 1, 1, 0, 0 });
                else if (currentWorld == 1) enemySpawnIndex.AddRange(new int[] { 2, 1, 2, 0, 0 });
                else if (currentWorld == 2) enemySpawnIndex.AddRange(new int[] { 1, 3, 1, 2, 2 });
                else if (currentWorld == 3) enemySpawnIndex.AddRange(new int[] { 3, 3, 3, 4, 4 });
                else if (currentWorld == 4) enemySpawnIndex.AddRange(new int[] { 2, 2, 2, 5, 5 });
                break;
            case EnemySpawnPatterns.EnemySpawnPattern.Medium_4:
                if (currentWorld == 0) enemySpawnIndex.AddRange(new int[] { 1, 0, 0, 1 });
                else if (currentWorld == 1) enemySpawnIndex.AddRange(new int[] { 2, 2, 2, 2 });
                else if (currentWorld == 2) enemySpawnIndex.AddRange(new int[] { 3, 2, 2, 3 });
                else if (currentWorld == 3) enemySpawnIndex.AddRange(new int[] { 3, 3, 3, 3 });
                else if (currentWorld == 4) enemySpawnIndex.AddRange(new int[] { 3, 3, 5, 5 });
                break;
            case EnemySpawnPatterns.EnemySpawnPattern.Medium_6:
                if (currentWorld == 0) enemySpawnIndex.AddRange(new int[] { 1, 1, 1, 0, 0, 0 });
                else if (currentWorld == 1) enemySpawnIndex.AddRange(new int[] { 1, 1, 1, 2, 2, 2 });
                else if (currentWorld == 2) enemySpawnIndex.AddRange(new int[] { 3, 3, 3, 0, 0, 1 });
                else if (currentWorld == 3) enemySpawnIndex.AddRange(new int[] { 4, 4, 4, 3, 3, 2 });
                else if (currentWorld == 4) enemySpawnIndex.AddRange(new int[] { 5, 5, 5, 4, 4, 1 });
                break;
        }

        ListToEnemyArrayIndex(enemySpawnIndex);

        if (enemySpawnIndex.Count == 0) { 
            // Fill 20 slots with 0 to avoid null error.
            for (int i = 0; i < 20; i++) {
                enemySpawnIndex.Add(0);
            }
        }

    }

    int RandomToEnemyArrayIndex(int rand) { 
        switch (rand) {
            case 0:
                return 0; // Slime
            case 1:
                return 1; // Snail
            case 2:
                return 4; // Bat
            case 3:
                return 5; // Spider
            case 4:
                return 6; // Dragon
            case 5:
                return 3; // Skeleton
        }
        return 0;
    }

    void ListToEnemyArrayIndex(List<int> enemySpawn) {
        for (int i = 0; i < enemySpawn.Count; i++) {
            enemySpawn[i] = RandomToEnemyArrayIndex(enemySpawn[i]);
        }
    }

    void RemoveAllEnemies() {
        // Technically should not be called, a place holder for now.
        for (int i = 0; i < enemyParent.transform.childCount; i++) {
            Destroy(enemyParent.transform.GetChild(i).gameObject);
        }
    }

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
