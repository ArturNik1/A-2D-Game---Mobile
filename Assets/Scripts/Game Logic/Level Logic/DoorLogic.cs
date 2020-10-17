using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLogic : MonoBehaviour
{
    public GameObject fromRoom;
    public GameObject toRoom;
    public bool nextWorld;

    // if we are in boss room, just set nextWorld flag to true. Otherwise, set both GameObjects.
    public void PopulateGameObjects(GameObject fromRoom, GameObject toRoom, bool nextWorld = false) {
        if (nextWorld && toRoom == null) {
            this.nextWorld = true;
            this.fromRoom = fromRoom;
            return;
        }
        this.fromRoom = fromRoom;
        this.toRoom = toRoom;
    }

    public void HandleDoorAction() {
        if (fromRoom.GetComponent<RoomLogic>().aliveEnemies > 0) return;

        AudioManager.instance.Play("DoorClose0" + Random.Range(1, 4));

        if (nextWorld) {
            // head to next world. 
            print("NEXT WORLD");
            StartCoroutine(LoadNextWorld());
        } else {
            Camera.main.GetComponent<CameraLogic>().flashingScreen.color = new Color(0, 0, 0, 1f);
            Camera.main.GetComponent<CameraLogic>().CallFlashMethod();
            // teleport player to toRoom.
            if (toRoom.name != "SpecialRoom" && toRoom.name != "HallwayRoom" && fromRoom.name != "HallwayRoom") {
                fromRoom.SetActive(false);
                toRoom.SetActive(true);
                toRoom.transform.localPosition = new Vector3(0, 0, toRoom.transform.localPosition.z);
            }
            // Spawn enemies here....
            GameObject.Find("Enemies").GetComponent<EnemyManager>().SpawnEnemies(toRoom);

            // Makes sure that the items left on the ground will despawn properly and will be able to appear in the future.
            ResetGroundItems();

            if (fromRoom.name == "SpecialRoom") {
                // Leaving item room...
                var rl = fromRoom.GetComponent<RoomLogic>();
                if (rl.hasChest) {
                    Destroy(rl.chestObject);
                }
            }
            else if (toRoom.name == "SpecialRoom") {
                // Enters item room...
                var rl = toRoom.GetComponent<RoomLogic>();
                if (rl.chestItem != null && !rl.hasChest) {
                    GameObject obj = Instantiate(rl.chestItem);
                    var item = obj.GetComponent<Item>();
                    item.fromItemRoom = true;
                    obj.transform.position = rl.itemPos;
                    obj.transform.SetParent(ItemManager.instance.itemsHolder.transform);
                    item.room = toRoom;
                }
            }


            // Change currentRoomType and current room info.
            LevelManager.currentRoomType = toRoom.GetComponent<RoomLogic>().roomType;
            GameObject player = GameObject.Find("Player");
            player.GetComponent<PlayerController>().currentRoomObject = toRoom;
            player.GetComponent<PlayerController>().currentRoomMain = toRoom.GetComponent<RoomLogic>();
            HandlePlayerTransform(player);
        }
    }

    IEnumerator LoadNextWorld() {
        MenuLogic menuLogic = GameObject.Find("Canvas").GetComponentInChildren<MenuLogic>();
        menuLogic.worldTransitionAnim.gameObject.SetActive(true);
        menuLogic.worldTransitionAnim.SetTrigger("Start");

        yield return new WaitForSeconds(1.0f);

        var player = GameObject.Find("Player");
        StatsTracker stats = player.GetComponent<StatsTracker>();
        switch (LevelManager.currentWorld) {
            case 0:
                stats.FinishedWorld0++;
                break;
            case 1:
                stats.FinishedWorld1++;
                break;
            case 2:
                stats.FinishedWorld2++;
                break;
            case 3:
                stats.FinishedWorld3++;
                break;
            case 4:
                stats.FinishedWorld4++;
                break;
            case 5:
                stats.FinishedWorld5++;
                break;
        }

        LevelManager.currentWorld++;
        LevelManager.currentRoomGenerated = 0;

        GameObject rooms = GameObject.Find("Rooms");
        for (int i = 0; i < rooms.transform.childCount; i++) {
            Destroy(rooms.transform.GetChild(i).gameObject);
        }
        
        player.GetComponent<LevelManager>().CreateRooms();
        player.transform.position = new Vector3(0, 0, player.transform.position.z);

        menuLogic.worldTransitionAnim.SetTrigger("End");
    }

    void ResetGroundItems() {
        for (int i = 0; i < ItemManager.instance.itemsHolder.transform.childCount; i++) {
            int index = ItemManager.instance.FindItemInPicked(ItemManager.instance.ReturnItemFromItems(ItemManager.instance.itemsHolder.transform.GetChild(i).gameObject));
            if (index != -1) ItemManager.instance.pickedItems[index].canBeDroppedAmount = ItemManager.instance.pickedItems[index].maxItemAmount - ItemManager.instance.pickedItems[index].itemAmount;
            Destroy(ItemManager.instance.itemsHolder.transform.GetChild(i).gameObject);
        }
    }

    void HandlePlayerTransform(GameObject player) {
        if (gameObject.name == "DoorUp") { // position player at DoorDown of new room - looking up + set camera to focus on playerPointObject.
            player.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            Vector3 doorPos = toRoom.transform.Find("Objects").Find("DoorDown").position;
            player.transform.position = new Vector3(doorPos.x, doorPos.y + 0.15f, player.transform.position.z);
            Camera.main.GetComponent<CameraLogic>().SetFocus(true);
        }
        else if (gameObject.name == "DoorDown") { // position player at DoorUp of new room - looking down + set camera to focus on playerPointObject.
            player.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
            Vector3 doorPos = toRoom.transform.Find("Objects").Find("DoorUp").position;
            player.transform.position = new Vector3(doorPos.x, doorPos.y - 0.15f, player.transform.position.z);
            Camera.main.GetComponent<CameraLogic>().SetFocus(true);
        }
        else if (gameObject.name == "DoorRight") { // position player at DoorLeft of new room - looking left + set camera to focus on playerPointObject.
            player.transform.rotation = Quaternion.Euler(0f, 0f, 270f);
            Vector3 doorPos = toRoom.transform.Find("Objects").Find("DoorLeft").position;
            player.transform.position = new Vector3(doorPos.x + 0.12f, 0f, player.transform.position.z);
            Camera.main.GetComponent<CameraLogic>().SetFocus(true);
        }
        else if (gameObject.name == "DoorLeft") { // position player at DoorRight of new room - looking right + set camera to focus on playerPointObject.
            player.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            Vector3 doorPos = toRoom.transform.Find("Objects").Find("DoorRight").position;
            player.transform.position = new Vector3(doorPos.x - 0.12f, 0f, player.transform.position.z);
            Camera.main.GetComponent<CameraLogic>().SetFocus(true);
        }
    }

}
