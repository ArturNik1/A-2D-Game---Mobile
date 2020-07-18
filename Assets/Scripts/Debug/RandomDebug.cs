using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomDebug : MonoBehaviour
{
    public int fpsTarget;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
    }

    private void Update()
    {
        Application.targetFrameRate = fpsTarget;
    }

    public void SpawnEnemy(Text text) {
        if (text.text != "") {
            EnemyManager enemy = GameObject.Find("Enemies").GetComponent<EnemyManager>();
            GameObject player = GameObject.Find("Player");

            int num;
            if (int.TryParse(text.text, out num))
            {
                if (num < 0 || num > enemy.enemyPrefabs.Length - 1 || num == 2) return;
            }
            else return;

            Vector3 pos = new Vector3(player.transform.position.x, player.transform.position.y + 0.35f, 0);
            var obj = Instantiate(enemy.enemyPrefabs[num], pos, transform.rotation);
            obj.transform.SetParent(enemy.enemyParent.transform);

        }
    }

    public void ChangeTextValueUP(Text text) {
        EnemyManager enemy = GameObject.Find("Enemies").GetComponent<EnemyManager>();
        int num;
        if (int.TryParse(text.text, out num)) {
            if (num >= enemy.enemyPrefabs.Length - 1) return;
            num++;
            if (num == 2) num++;
            transform.Find("InputField").GetComponent<InputField>().text = num + "";
        }
    }

    public void ChangeTextValueDOWN(Text text) {
        EnemyManager enemy = GameObject.Find("Enemies").GetComponent<EnemyManager>();
        int num;
        if (int.TryParse(text.text, out num)) {
            if (num == 0) return;
            num--;
            if (num == 2) num--;
            transform.Find("InputField").GetComponent<InputField>().text = num + "";
        }
    }
}
