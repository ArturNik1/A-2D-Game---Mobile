using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class AchievementInfo
{
    AchievementHandler.AchievementType type;
    int current;
    int target;
    int prize;
    int star;
    public AchievementLogic logic;

    public AchievementInfo (AchievementHandler.AchievementType type, int current, int target, int prize, int star, AchievementLogic logic) {
        this.type = type;
        this.current = current;
        this.target = target;
        this.prize = prize;
        this.star = star;
        this.logic = logic;

        logic.info = this;
    }

    public DataStorer GetAchievementData() {
        current = logic._current;
        target = logic._target[star];
        return new DataStorer(type, logic._current, logic._target, star, logic._title, logic._prize, logic._finished);
    }

    public void IncreaseStar() {
        star++;
        prize = logic._prize[star];
        target = logic._target[star];
    }

    public void IncreaseCurrent(int amount) {
        current += amount;
        logic._current += amount;
        if (current >= target) { 
            current = target;
            logic._current = target;
        }
    }
}
