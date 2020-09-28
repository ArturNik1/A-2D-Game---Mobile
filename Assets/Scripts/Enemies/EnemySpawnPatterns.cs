using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemySpawnPatterns
{
    public enum EnemySpawnPattern { Small_3, Small_5, Medium_4, Medium_6, LargeH_4, LargeH_5, LargeH_6, LargeV_4, LargeV_5, LargeV_6, LargeV_7, Huge_5, Huge_6, Huge_7, Huge_8, Gigantic_8, Gigantic_10, Gigantic_12, Nothing }

    public static EnemySpawnPattern RollForSpawnPattern(GameObject room) {
        RoomType type = room.GetComponent<RoomLogic>().roomType;
        switch (LevelManager.currentWorld) {
            case 0: // World one.
                return RollPatternWorldOne(type);
            case 1: 
                return RollPatternWorldTwo(type);
            case 2: 
                return RollPatternWorldThree(type);
            case 3:
                return RollPatternWorldFour(type);
            case 4:
                return RollPatternWorldFive(type);
            case 5:
                return RollPatternWorldSix(type);
        }
        return EnemySpawnPattern.Nothing;
    }

    static EnemySpawnPattern RollPatternWorldOne(RoomType type) {
        int rand;

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

        return EnemySpawnPattern.Nothing;
    }

    static EnemySpawnPattern RollPatternWorldTwo(RoomType type) {
        int rand;

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

        return EnemySpawnPattern.Nothing;
    }

    static EnemySpawnPattern RollPatternWorldThree(RoomType type) {
        int rand;

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

        return EnemySpawnPattern.Nothing;
    }

    static EnemySpawnPattern RollPatternWorldFour(RoomType type) {
        int rand;

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

        return EnemySpawnPattern.Nothing;
    }

    static EnemySpawnPattern RollPatternWorldFive(RoomType type) {
        int rand;

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

        return EnemySpawnPattern.Nothing;
    }

    static EnemySpawnPattern RollPatternWorldSix(RoomType type) {
        int rand;

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

        return EnemySpawnPattern.Nothing;
    }
}
