using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraLogic : MonoBehaviour
{
    public GameObject playerObject;
    public GameObject playerPointObject;
    public Image flashingScreen;
    private PlayerController playerController;

    private bool refocus = false;

    // Start is called before the first frame update
    void Start()
    {
        playerController = playerObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.currentRoomMain == null) return;

        if (refocus) {
            // Focus camera on player when flagged. using playerPointObject in order to follow player to new location.
            ReFocus();
            return;
        }

        // Keep camera from going over the bounds.
        HandleCameraRoomBounds();
    }

    private void HandleCameraRoomBounds() {
        if (LevelManager.currentRoomType == RoomType.Tiny || LevelManager.currentRoomType == RoomType.Small || LevelManager.currentRoomType == RoomType.Medium)
        {
            // These rooms are always in frame, no need to follow player.
            transform.position = new Vector3(playerController.currentRoomMain.transform.position.x,
                 playerController.currentRoomMain.transform.position.y, -1.205f);
        }
        else if (LevelManager.currentRoomType == RoomType.LargeH)
        {
            // Move camera only when it's inside the bounds.
            if (playerController.transform.position.y + 0.24f < playerController.currentRoomMain.transform.position.y + 0.68f
                && playerController.transform.position.y - 0.44f > playerController.currentRoomMain.transform.position.y - 0.68f)
            {
                transform.position = new Vector3(playerController.currentRoomMain.transform.position.x,
                    playerController.transform.position.y - 0.1f, -1.205f);
            }
        }
        else if (LevelManager.currentRoomType == RoomType.LargeV)
        {
            if (playerController.transform.position.y + 0.24f < playerController.currentRoomMain.transform.position.y + 1.02f
                && playerController.transform.position.y - 0.44f > playerController.currentRoomMain.transform.position.y - 1.02f)
            {
                transform.position = new Vector3(playerController.currentRoomMain.transform.position.x,
                    playerController.transform.position.y - 0.1f, -1.205f);
            }
        }
        else if (LevelManager.currentRoomType == RoomType.Huge)
        {
            if (playerController.transform.position.y + 0.24f < playerController.currentRoomMain.transform.position.y + 1.02f
                && playerController.transform.position.y - 0.44f > playerController.currentRoomMain.transform.position.y - 1.02f)
            {
                transform.position = new Vector3(playerController.currentRoomMain.transform.position.x,
                    playerController.transform.position.y - 0.1f, -1.205f);
            }
        }
        else if (LevelManager.currentRoomType == RoomType.Gigantic)
        {
            ////// Vertical //////
            if (playerController.transform.position.y + 0.24f < playerController.currentRoomMain.transform.position.y + 1.7f
                && playerController.transform.position.y - 0.44f > playerController.currentRoomMain.transform.position.y - 1.7f)
            {
                transform.position = new Vector3(transform.position.x,
                    playerController.transform.position.y - 0.1f, -1.205f);
            }
            ////// Horizontal ///////
            if (playerController.transform.position.x + 1.05f < playerController.currentRoomMain.transform.position.x + 1.7f
                && playerController.transform.position.x - 1.05f > playerController.currentRoomMain.transform.position.x - 1.7f)
            {
                transform.position = new Vector3(playerController.transform.position.x,
                    transform.position.y, -1.205f);
            }
        }
    }

    public void ReFocus() {
        if (playerPointObject.transform.position.Equals(playerObject.transform.position)) { 
            refocus = false;
            return;
        }
        if (LevelManager.currentRoomType == RoomType.Tiny || LevelManager.currentRoomType == RoomType.Small || LevelManager.currentRoomType == RoomType.Medium)
        {
            // These rooms are always in frame, no need to follow player.
            transform.position = new Vector3(playerController.currentRoomMain.transform.position.x,
                 playerController.currentRoomMain.transform.position.y, -1.205f);
            playerPointObject.transform.position = playerController.transform.position;
        }
        else if (LevelManager.currentRoomType == RoomType.LargeH)
        {
            // Move camera only when it's inside the bounds.
            if (playerPointObject.transform.position.y + 0.24f < playerController.currentRoomMain.transform.position.y + 0.68f
                && playerPointObject.transform.position.y - 0.44f > playerController.currentRoomMain.transform.position.y - 0.68f)
            {
                transform.position = new Vector3(playerController.currentRoomMain.transform.position.x,
                    playerPointObject.transform.position.y - 0.1f, -1.205f);
            }
        }
        else if (LevelManager.currentRoomType == RoomType.LargeV)
        {
            if (playerPointObject.transform.position.y + 0.24f < playerController.currentRoomMain.transform.position.y + 1.02f
                && playerPointObject.transform.position.y - 0.44f > playerController.currentRoomMain.transform.position.y - 1.02f)
            {
                transform.position = new Vector3(playerController.currentRoomMain.transform.position.x,
                    playerPointObject.transform.position.y - 0.1f, -1.205f);
            }
        }
        else if (LevelManager.currentRoomType == RoomType.Huge)
        {
            if (playerPointObject.transform.position.y + 0.24f < playerController.currentRoomMain.transform.position.y + 1.02f
                && playerPointObject.transform.position.y - 0.44f > playerController.currentRoomMain.transform.position.y - 1.02f)
            {
                transform.position = new Vector3(playerController.currentRoomMain.transform.position.x,
                    playerPointObject.transform.position.y - 0.1f, -1.205f);
            }
        }
        else if (LevelManager.currentRoomType == RoomType.Gigantic)
        {
            ////// Vertical //////
            if (playerPointObject.transform.position.y + 0.24f < playerController.currentRoomMain.transform.position.y + 1.7f
                && playerPointObject.transform.position.y - 0.44f > playerController.currentRoomMain.transform.position.y - 1.7f)
            {
                transform.position = new Vector3(transform.position.x,
                    playerPointObject.transform.position.y - 0.1f, -1.205f);
            }
            ////// Horizontal ///////
            if (playerPointObject.transform.position.x + 1.05f < playerController.currentRoomMain.transform.position.x + 1.7f
                && playerPointObject.transform.position.x - 1.05f > playerController.currentRoomMain.transform.position.x - 1.7f)
            {
                transform.position = new Vector3(playerPointObject.transform.position.x,
                    transform.position.y, -1.205f);
            }
        }
    }

    public void SetFocus (bool state) {
        // Focus on playerPointObject while in transition.
        if (state) {
            playerPointObject.transform.position = new Vector3(0, 0, playerPointObject.transform.position.z);
        }
        refocus = state;
    }

    #region Room Transition

    public void CallFlashMethod() {
        StartCoroutine(FlashRoomChange());
    }

    IEnumerator FlashRoomChange() {
        // Handle screen transition between rooms.
        bool doneLoop = false;
        while (!doneLoop) {
            flashingScreen.color = new Color(0, 0, 0, flashingScreen.color.a - 0.025f);
            if (flashingScreen.color.a <= 0) doneLoop = true;
            yield return new WaitForEndOfFrame();

        }
        yield return new WaitForEndOfFrame();
    }

    #endregion
}
