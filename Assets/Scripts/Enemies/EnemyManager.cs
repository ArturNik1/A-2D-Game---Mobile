using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public enum EnemySpawnPattern { Small_3, Small_5, Medium_4, Medium_6, LargeH_4, LargeH_5, LargeH_6, LargeV_4, LargeV_5, LargeV_6, LargeV_7, Huge_5, Huge_6, Huge_7, Huge_8, Gigantic_8, Gigantic_10, Gigantic_12, Nothing}

    [Header("Enemies")]
    public GameObject[] enemyPrefabs; // 0 - slime ; 1 - snail ;

    GameObject enemyParent;
    public GameObject playerGameObject;
    PlayerController pController;

    private int chanceForOneType;

    // Start is called before the first frame update
    void Start()
    {
        if (enemyPrefabs.Length == 0) Debug.LogError("EnemyPrefab array is empty!");

        enemyParent = GameObject.Find("Enemies");
        pController = playerGameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnEnemies(GameObject room) {
        if (room.GetComponent<RoomLogic>().cleared) return;
        RemoveAllEnemies();
        pController.ResetAllProjectiles();

        SpawnEnemiesAfterPatternRoll(RollForSpawnPattern(room), room);
    }

    EnemySpawnPattern RollForSpawnPattern(GameObject room) {
        RoomType type = room.GetComponent<RoomLogic>().roomType;
        int rand;
        switch (LevelManager.currentWorld) {
            case 0: // World one.

                if (type == RoomType.LargeH) {

                    rand = Random.Range(1, 4); // max is exclusive
                    if (rand == 1) return EnemySpawnPattern.LargeH_4;
                    else if (rand == 2) return EnemySpawnPattern.LargeH_5;
                    else if (rand == 3) return EnemySpawnPattern.LargeH_6;

                }
                
                else if (type == RoomType.LargeV) {

                    rand = Random.Range(1, 5); 
                    if (rand == 1) return EnemySpawnPattern.LargeV_4;
                    else if (rand == 2) return EnemySpawnPattern.LargeV_5;
                    else if (rand == 3) return EnemySpawnPattern.LargeV_6;
                    else if (rand == 4) return EnemySpawnPattern.LargeV_7; // should be rare.

                }

                else if (type == RoomType.Huge) {

                    rand = Random.Range(1, 5);
                    if (rand == 1) return EnemySpawnPattern.Huge_5;
                    else if (rand == 2) return EnemySpawnPattern.Huge_6;
                    else if (rand == 3) return EnemySpawnPattern.Huge_7;
                    else if (rand == 4) return EnemySpawnPattern.Huge_8; // should be rare.
                }

                else if (type == RoomType.Gigantic) {

                    rand = Random.Range(1, 4);
                    if (rand == 1) return EnemySpawnPattern.Gigantic_8;
                    else if (rand == 2) return EnemySpawnPattern.Gigantic_10;
                    else if (rand == 3) return EnemySpawnPattern.Gigantic_12;

                }

                else if (type == RoomType.Small) {

                    rand = Random.Range(1, 3);
                    if (rand == 1) return EnemySpawnPattern.Small_3;
                    else if (rand == 2) return EnemySpawnPattern.Small_5;

                }

                else if (type == RoomType.Medium) {

                    rand = Random.Range(1, 3);
                    if (rand == 1) return EnemySpawnPattern.Medium_4;
                    else if (rand == 2) return EnemySpawnPattern.Medium_6;

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
        float middle_X = (wall_R.position.x + wall_L.position.x) / 2f;
        float maxPerHalf_X = wall_R.position.x - middle_X - wall_R.GetComponent<BoxCollider2D>().size.x;
        float middle_Y = (wall_U.position.y + wall_B.position.y) / 2f;
        float maxPerHalf_Y = wall_U.position.y - middle_Y - wall_U.GetComponent<BoxCollider2D>().size.y;

        switch (pattern) {

            #region LargeH_4
            case EnemySpawnPattern.LargeH_4:

                chanceForOneType = Random.Range(0, 3); // 0 - Slime, 1 - Snail, 2 - Mixed.

                if (chanceForOneType == 2) { // Mixed.

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                else {

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }


                break;
            #endregion
            #region LargeH_5
            case EnemySpawnPattern.LargeH_5:

                chanceForOneType = Random.Range(0, 3); // 0 - Slime, 1 - Snail, 2 - Mixed.

                if (chanceForOneType == 2) {

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 55, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 55, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 55, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 55, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                else {

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 55, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 55, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 55, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 55, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                break;
            #endregion
            #region LargeH_6
            case EnemySpawnPattern.LargeH_6:

                chanceForOneType = Random.Range(0, 3); // 0 - Slime, 1 - Snail, 2 - Mixed.

                if (chanceForOneType == 2) {

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 90, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                else {

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 50, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 90, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                break;
            #endregion
            #region LargeV_4
            case EnemySpawnPattern.LargeV_4:

                chanceForOneType = Random.Range(0, 3); // 0 - Slime, 1 - Snail, 2 - Mixed.

                if (chanceForOneType == 2) {

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                else {

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                break;
            #endregion
            #region LargeV_5
            case EnemySpawnPattern.LargeV_5:

                chanceForOneType = Random.Range(0, 3); // 0 - Slime, 1 - Snail, 2 - Mixed.

                if (chanceForOneType == 2) {

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                else {

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                break;
            #endregion
            #region LargeV_6
            case EnemySpawnPattern.LargeV_6:

                chanceForOneType = Random.Range(0, 3); // 0 - Slime, 1 - Snail, 2 - Mixed.

                if (chanceForOneType == 2) {

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 10, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 10, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 10, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                else {

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 10, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 10, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 10, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                break;
            #endregion
            #region LargeV_7
            case EnemySpawnPattern.LargeV_7:

                chanceForOneType = Random.Range(0, 3); // 0 - Slime, 1 - Snail, 2 - Mixed.

                if (chanceForOneType == 2) {

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 20, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 20, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 20, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                else {

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 20, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 20, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 20, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 30, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                break;
            #endregion
            #region Huge_5
            case EnemySpawnPattern.Huge_5:

                chanceForOneType = Random.Range(0, 3); // 0 - Slime, 1 - Snail, 2 - Mixed.

                if (chanceForOneType == 2) {

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                else {

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                break;
            #endregion
            #region Huge_6
            case EnemySpawnPattern.Huge_6:

                chanceForOneType = Random.Range(0, 3); // 0 - Slime, 1 - Snail, 2 - Mixed.

                if (chanceForOneType == 2) {

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 10, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 10, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 10, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                else { 
                
                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 10, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 10, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 10, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 60, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);
                
                }

                break;
            #endregion
            #region Huge_7
            case EnemySpawnPattern.Huge_7:

                chanceForOneType = Random.Range(0, 3); // 0 - Slime, 1 - Snail, 2 - Mixed.

                if (chanceForOneType == 2) {

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                else { 

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);
                
                }

                break;
            #endregion
            #region Huge_8
            case EnemySpawnPattern.Huge_8:

                chanceForOneType = Random.Range(0, 3); // 0 - Slime, 1 - Snail, 2 - Mixed.

                if (chanceForOneType == 2) {

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                else { 
                
                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                break;
            #endregion
            #region Gigantic_8
            case EnemySpawnPattern.Gigantic_8:

                chanceForOneType = Random.Range(0, 3); // 0 - Slime, 1 - Snail, 2 - Mixed.

                if (chanceForOneType == 2) {

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                else { 

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                break;
            #endregion
            #region Gigantic_10
            case EnemySpawnPattern.Gigantic_10:

                chanceForOneType = Random.Range(0, 3); // 0 - Slime, 1 - Snail, 2 - Mixed.

                if (chanceForOneType == 2) {

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                else { 
                
                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                break;
            #endregion
            #region Gigantic_12
            case EnemySpawnPattern.Gigantic_12:

                chanceForOneType = Random.Range(0, 3); // 0 - Slime, 1 - Snail, 2 - Mixed.

                if (chanceForOneType == 2) {

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                else { 

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 70, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }


                break;
            #endregion
            #region Small_3
            case EnemySpawnPattern.Small_3:

                chanceForOneType = Random.Range(0, 3); // 0 - Slime, 1 - Snail, 2 - Mixed.

                if (chanceForOneType == 2) {

                    if (room.GetComponent<RoomLogic>().hallwayIsLeft) {
                        enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 10, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);
                    }

                    else {
                        enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 10, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);
                    }

                }

                else { 
                
                    if (room.GetComponent<RoomLogic>().hallwayIsLeft) { 
                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 10, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);
                    }

                    else {
                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 10, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);
                    }

                }


                break;
            #endregion
            #region Small_5
            case EnemySpawnPattern.Small_5:

                chanceForOneType = Random.Range(0, 3); // 0 - Slime, 1 - Snail, 2 - Mixed.

                if (chanceForOneType == 2) {

                    if (room.GetComponent<RoomLogic>().hallwayIsLeft) {
                        enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 35, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 35, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);
                    }

                    else {
                        enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 35, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 35, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);
                    }

                }

                else { 

                    if (room.GetComponent<RoomLogic>().hallwayIsLeft) { 
                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 35, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 35, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);
                    }

                    else {
                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 40, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 70, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 35, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 0, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 35, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);
                    }

                }


                break;
            #endregion
            #region Medium_4
            case EnemySpawnPattern.Medium_4:

                chanceForOneType = Random.Range(0, 3); // 0 - Slime, 1 - Snail, 2 - Mixed.

                if (chanceForOneType == 2) {

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                else {

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                    enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 80, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                    enemy.transform.SetParent(enemyParent.transform);
                    enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                }

                break;
            #endregion
            #region Medium_6
            case EnemySpawnPattern.Medium_6:

                chanceForOneType = Random.Range(0, 3); // 0 - Slime, 1 - Snail, 2 - Mixed.

                if (chanceForOneType == 2) {

                    if (room.GetComponent<RoomLogic>().hallwayIsLeft) {
                        enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 10, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 10, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);
                    }

                    else {
                        enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[1], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 10, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 10, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[0], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);
                    }

                }

                else {

                    if (room.GetComponent<RoomLogic>().hallwayIsLeft) {
                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 10, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 10, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);
                    }

                    else {
                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 50, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 80, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 10, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 10, true), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 40, false), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);

                        enemy = Instantiate(enemyPrefabs[chanceForOneType], new Vector3(CalculateByPercentOffMiddle_X(middle_X, maxPerHalf_X, 30, false), CalculateByPercentOffMiddle_Y(middle_Y, maxPerHalf_Y, 0, true), 0), transform.rotation);
                        enemy.transform.SetParent(enemyParent.transform);
                        enemy.GetComponent<EnemyInfo>().UpdateRoomInfo(room);
                    }

                }

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
