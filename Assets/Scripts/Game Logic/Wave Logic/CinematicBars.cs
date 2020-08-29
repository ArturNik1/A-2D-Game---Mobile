using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CinematicBars : MonoBehaviour
{
    GameObject player;

    private RectTransform topBar, bottomBar;
    private float changeSizeAmount;
    private float targetSize;

    void Awake() {
        GameObject obj = new GameObject("topBar", typeof(Image));
        obj.transform.SetParent(transform, false);
        obj.GetComponent<Image>().color = Color.black;
        topBar = obj.GetComponent<RectTransform>();
        topBar.anchorMin = new Vector2(0, 1);
        topBar.anchorMax = new Vector2(1, 1);
        topBar.sizeDelta = new Vector2(0, 0);

        obj = new GameObject("bottomBar", typeof(Image));
        obj.transform.SetParent(transform, false);
        obj.GetComponent<Image>().color = Color.black;
        bottomBar = obj.GetComponent<RectTransform>();
        bottomBar.anchorMin = new Vector2(0, 0);
        bottomBar.anchorMax = new Vector2(1, 0);
        bottomBar.sizeDelta = new Vector2(0, 0);

    }

    public IEnumerator Show(float targetSize, float time) {
        while (true) {
            yield return new WaitForEndOfFrame();
            
            this.targetSize = targetSize;
            changeSizeAmount = (targetSize - topBar.sizeDelta.y) / time;
            Vector2 sizeDelta = topBar.sizeDelta;
            sizeDelta.y += changeSizeAmount * Time.deltaTime;
            topBar.sizeDelta = sizeDelta;
            bottomBar.sizeDelta = sizeDelta;

            if (sizeDelta.y >= targetSize-1) break;
        }
    }

    public IEnumerator Hide(float time) {
        while (true) {  
            yield return new WaitForEndOfFrame();
        
            targetSize = 0f;
            changeSizeAmount = (targetSize - topBar.sizeDelta.y) / time;
            Vector2 sizeDelta = topBar.sizeDelta;
            sizeDelta.y += changeSizeAmount * Time.deltaTime;
            topBar.sizeDelta = sizeDelta;
            bottomBar.sizeDelta = sizeDelta;

            if (sizeDelta.y <= targetSize + 1) {
                sizeDelta.y = 0;
                topBar.sizeDelta = sizeDelta;
                bottomBar.sizeDelta = sizeDelta;
                break;
            }
        }

        GameObject.Find("Enemies").GetComponent<WaveManager>().SpawnFirstWave();

    }

    public IEnumerator Walk(BossManager bossManager) {
        player = GameObject.Find("Player");
        var playerC = player.GetComponent<PlayerController>();
        playerC.BlockMovement();
        var rb = player.GetComponent<Rigidbody>();
        while (true) { 
            yield return new WaitForEndOfFrame();

            var movementOffset = new Vector3(0, 1, 0) * 0.15f * Time.deltaTime;
            var newPosition = rb.position + movementOffset;

            playerC.UpdatePlayerAnim(0.425f, 0);

            rb.MovePosition(newPosition);

            if (player.transform.position.y >= 0f) {
                StartCoroutine(ChangeToIdleMovement(playerC, bossManager));
                break;
            }
        }
    }

    IEnumerator ChangeToIdleMovement(PlayerController pController, BossManager bossManager) { 
        while (true) {
            yield return new WaitForEndOfFrame();
            var y = pController.GetPlayersMovementY();
            pController.UpdatePlayerAnim(y - Time.deltaTime, 0);
            if (y <= 0) {
                pController.UpdatePlayerAnim(0, 0);
                break;
            }
        }
        EndWalk(pController, bossManager);
    }

    void EndWalk(PlayerController pController, BossManager bossManager) {
        // Start wave countdown...
        bossManager.StartWaveCountdown();
    }

}
