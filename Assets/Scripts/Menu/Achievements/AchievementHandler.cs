using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class AchievementHandler : MonoBehaviour
{
    public enum AchievementType { KillSlimes, KillTurtles, KillBats, KillSpiders, KillSkeletons, KillDragons, KillTotalEnemies, KillChests, OpenChests, KillGolems, KillOrcs, KillEvilMages, KillTotalBosses,
        FinishWorld0, FinishWorld1, FinishWorld2, FinishWorld3, FinishWorld4, FinishWorld5, CollectItems }  

    public static Dictionary<AchievementType, AchievementInfo> achievements = new Dictionary<AchievementType, AchievementInfo>();

#pragma warning disable CS0649
    [SerializeField] Transform achievementParent;
    [SerializeField] GameObject achievementGameObject;
#pragma warning restore CS0649

    // Start is called before the first frame update
    void Start()
    {
        if (achievements.Count != 0) achievements = new Dictionary<AchievementType, AchievementInfo>();
        LoadAchievements();
        OrderWrapper();
    }
    
    void AddAchievement(AchievementType type, int current, int[] target, int star, string title, int[] prize, bool finished) { 
        var obj = Instantiate(achievementGameObject, achievementParent);
        AchievementLogic al = obj.GetComponent<AchievementLogic>();
        al.FillAchievement(current, target, star, title, prize, finished);
        AchievementInfo ai = new AchievementInfo(type, current, target[star], prize[star], star, al);
        achievements.Add(type, ai);
    }

    public void SaveWrapper() {
        SaveAchievements();
    }

    public void OrderWrapper() {
        OrderDictionaryByProgressBar();
        OrderGameObjectByProgressBar();
    }

    public static void SaveAchievements() {
        // Turn the dictionary into a list (Json won't accept dictionary)

        List<DataStorer> ds = new List<DataStorer>();
        foreach(var item in achievements.Values) {
            ds.Add(item.GetAchievementData());
        }

        string json = JsonConvert.SerializeObject(ds);
        string path = Path.Combine(Application.persistentDataPath, "saved files", "data.json");
        string folder = Path.GetDirectoryName(path);
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        File.WriteAllText(path, json);

        //print(json);
    }

    void LoadAchievements() {
        string path = Path.Combine(Application.persistentDataPath, "saved files", "data.json");
        if (File.Exists(path)) {
            string json = File.ReadAllText(path);
            DataStorer[] ds = JsonConvert.DeserializeObject<DataStorer[]>(json);
        
            for (int i = 0; i < ds.Length; i++) {
                AddAchievement(ds[i].type, ds[i].current, ds[i].target, ds[i].star, ds[i].title, ds[i].prize, ds[i].finished);
            }
        }
    }

    public static void OrderDictionaryByProgressBar() {
        achievements = achievements.OrderByDescending(x => x.Value.logic.IsCollectButtonActive())
            .ThenByDescending(x => x.Value.logic.GetProgressBarValue()).ToDictionary(x => x.Key, x => x.Value);
    }

    public static void OrderGameObjectByProgressBar() {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Achievement");
        GameObject[] ordered = objects.OrderByDescending(x => x.GetComponent<AchievementLogic>().IsCollectButtonActive())
            .ThenByDescending(x => x.GetComponent<AchievementLogic>().GetProgressBarValue()).ToArray();

        for (int i = 0; i < ordered.Length; i++) {
            ordered[i].transform.SetSiblingIndex(i);
        }
    }

    public static void UpdateValuesInDictionary() {
        // Update the info....
        GameObject.Find("Player").GetComponent<StatsTracker>().AddValuesToDictionary();
        // Order it again.... will be ordered when menu loads up.
        //OrderDictionaryByProgressBar();
        //OrderGameObjectByProgressBar();
        // Save the dictionary....
        SaveAchievements();
    }

}
