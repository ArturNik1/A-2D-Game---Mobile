using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCountdown : MonoBehaviour
{
    Text text;
    bool goingUP = true;
    int[] count = { 3, 2, 1 };

    private void Start() {
        text = GetComponentInChildren<Text>();
    }


    public IEnumerator DoCountdown(float delay, BossManager bossManager, PlayerController playerController) {
        yield return new WaitForSeconds(delay);
        int i = 0;
        text.text = count[i] + "";
        while (true) {

            if (goingUP) text.fontSize += 5;
            else text.fontSize -= 5;

            text.fontSize++;
            if (text.fontSize >= 300) goingUP = false;
            else if (text.fontSize <= 1) { 
                goingUP = true;
                i++;
                if (i >= 3) break;
                text.text = count[i] + "";
            }
            yield return new WaitForSeconds(Time.fixedDeltaTime / 2); // should be 0.02 seconds unless changed. This makes font go up by 50 every second.
        }
        playerController.UnBlockMovement(0);
        StartCoroutine(GetComponent<CinematicBars>().Hide(0.3f));
    }

}
