using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DataStorer
{
    public AchievementHandler.AchievementType type;
    public int current;
    public int[] target;
    public int star;
    public string title;
    public int[] prize;
    public bool finished;

    public DataStorer (AchievementHandler.AchievementType type, int current, int[] target, 
        int star, string title, int[] prize, bool finished) {
        this.type = type;
        this.current = current;
        this.target = target;
        this.star = star;
        this.title = title;
        this.prize = prize;
        this.finished = finished;
    }
}
