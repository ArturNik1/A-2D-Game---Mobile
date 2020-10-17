using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsTracker : MonoBehaviour
{
    public int KillSlimes = 0;
    public int KillTurtles = 0;
    public int KillBats = 0;
    public int KillSpiders = 0;
    public int KillSkeletons = 0;
    public int KillDragons = 0;
    public int KillTotalEnemies = 0;
    public int KillChests = 0;
    public int OpenChests = 0;
    public int KillGolems = 0;
    public int KillOrcs = 0;
    public int KillEvilMages = 0;
    public int KillTotalBosses = 0;
    public int FinishedWorld0 = 0;
    public int FinishedWorld1 = 0;
    public int FinishedWorld2 = 0;
    public int FinishedWorld3 = 0;
    public int FinishedWorld4 = 0;
    public int FinishedWorld5 = 0;
    public int CollectItems = 0;

    public void AddValuesToDictionary() {
        List<int> stats = new List<int>();
        stats = GetAllVariables();
        for(int i = 0; i < AchievementHandler.achievements.Count; i++) {
            AchievementHandler.achievements[(AchievementHandler.AchievementType)i].IncreaseCurrent(stats[i]);
        }
    }

    public List<int> GetAllVariables() {
        var propertyValues = this.GetType().GetFields();
        List<int> result = new List<int>();
        for (int i = 0; i < propertyValues.Length; i++) {
            result.Add((int)propertyValues[i].GetValue(this));
        }
        return result;
    }

}
