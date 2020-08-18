using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLogic : MonoBehaviour
{
    [Header("Room Info")]
    public RoomAction roomAction;
    [HideInInspector]
    public enum RoomAction { Welcome, Forward, Hallway, Special, Boss }
    public float width;
    public float height;
    public RoomType roomType;
    public bool hasHallway;
    public bool hallwayIsLeft;

    private float screenRatio;
    private static float width_16, width_12;

    public GameObject wall;
    public GameObject player;

    public bool cleared = false;
    public int aliveEnemies = 0;

    public bool hasChest = false;
    public GameObject chestObject;
    public GameObject chestItem;

    private int currentWorld = LevelManager.currentWorld;
    private int currentRoom = LevelManager.currentRoomGenerated;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");

        screenRatio = (float)Screen.width / (float)Screen.height;
        width_16 = screenRatio * 8.015625f;
        width_12 = screenRatio * 6.75f;
    }

    #region Room Generation

    public void GenerateWelcomingRoom() {
        if (player == null) Start();
        gameObject.name = "Room" + currentWorld + "_" + currentRoom;
        roomType = RoomType.Tiny;
        roomAction = RoomAction.Welcome;
        width = 8;
        height = 8;
        transform.localScale = new Vector3(width, height, 1);
        LevelManager.currentRoomType = roomType; // change when entered to room and not generated.
        player.GetComponent<PlayerController>().currentRoomObject = gameObject;
        player.GetComponent<PlayerController>().currentRoomMain = this;
        MakeDoorsVisible(up: true);
    }

    public void GenerateBossRoom() {
        gameObject.name = "Room" + currentWorld + "_" + currentRoom;
        roomType = DetermineBossRoomType();
        roomAction = RoomAction.Boss;
        transform.localScale = new Vector3(width, height, 1);
        MakeDoorsVisible(up: true, down: true);
        gameObject.SetActive(false);
    }

    public void GenerateForwardRoom() {
        gameObject.name = "Room" + currentWorld + "_" + currentRoom;
        roomType = DetermineForwardRoomType();
        roomAction = RoomAction.Forward;
        transform.localScale = new Vector3(width, height, 1);
        MakeDoorsVisible(up: true, down: true);
        gameObject.SetActive(false);
    }

    public void GenerateHallwayRoom(bool isLeft) {
        // Generate hallway + item room... (small/medium + tiny)
        gameObject.name = "HallwayRoom";
        roomType = DetermineHallwayRoomType();
        roomAction = RoomAction.Hallway;
        transform.localScale = new Vector3(width, height, 1);
        MakeDoorsVisible(right: true, left: true);
        if (isLeft) transform.parent.GetComponent<RoomLogic>().MakeDoorsVisible(left: true);
        else transform.parent.GetComponent<RoomLogic>().MakeDoorsVisible(right: true);
        transform.parent.GetComponent<RoomLogic>().SetHallwayParentInfo(isLeft);
        SetHallwayParentInfo(isLeft);
    }

    public void GenerateItemRoom(bool isLeft) {
        gameObject.name = "SpecialRoom";
        roomType = RoomType.Tiny;
        roomAction = RoomAction.Special;

        RoomType parentType = transform.parent.GetComponent<RoomLogic>().roomType;
        if (parentType == RoomType.Small || parentType == RoomType.Medium) {
            ApplyLocalScale(transform, 8, 8, false); 
        }

        if (isLeft)  MakeDoorsVisible(right: true);
        else  MakeDoorsVisible(left: true);
    }

    #endregion

    #region Room RNG

    RoomType DetermineBossRoomType() {
        int rand = Random.Range(1, 100);
        if (rand <= 60) {
            width = width_16;
            height = 16;
            return RoomType.Huge;
        } else {
            width = 24;
            height = 24;
            return RoomType.Gigantic;
        }
    }

    RoomType DetermineForwardRoomType() {
        int rand = Random.Range(1, 100);
        switch (currentWorld) {
            case 0:
                if (rand <= 32) { // 30
                    width = width_16;
                    height = 12;

                    return RoomType.LargeH;
                }
                else if (rand <= 64) { // 60
                    width = width_12;
                    height = 16;

                    return RoomType.LargeV;
                }
                else if (rand <= 95) { // 90
                    width = width_16;
                    height = 16;

                    return RoomType.Huge;
                } 
                else {
                    width = 24;
                    height = 24;

                    return RoomType.Gigantic;
                }
        }
        return RoomType.LargeH;

    }

    RoomType DetermineHallwayRoomType() {
        RoomType type = Random.Range(0, 2) == 0 ? RoomType.Small : RoomType.Medium; // 0 2 
        float parentX = transform.parent.localScale.x, parentY = transform.parent.localScale.y;
        RoomType parentRoomType = transform.parent.GetComponent<RoomLogic>().roomType;
        if (type == RoomType.Small) {
            if (parentRoomType == RoomType.LargeV) width = 1;
            else if (parentRoomType == RoomType.Medium || parentRoomType == RoomType.LargeH || parentRoomType == RoomType.Huge) width = width_12 / width_16;
            else if (parentRoomType == RoomType.Gigantic) width = width_12 / 24f;
            if (parentY == 12) height = 0.6667f;
            else if (parentY == 16) height = 0.5f;
            else if (parentY == 24) height = 0.3334f;
        } else {
            if (parentRoomType == RoomType.LargeV) width = width_16 / width_12;
            else if (parentRoomType == RoomType.Medium || parentRoomType == RoomType.LargeH || parentRoomType == RoomType.Huge) width = 1;
            else if (parentRoomType == RoomType.Gigantic) width = width_16 / 24f;
            if (parentY == 12) height = 0.6667f;
            else if (parentY == 16) height = 0.5f;
            else if (parentY == 24) height = 0.3334f;
        }
        return type;
    }

    #endregion

    void SetHallwayParentInfo(bool isLeft) {
        this.hasHallway = true;
        this.hallwayIsLeft = isLeft;
    }

    bool MakeDoorsVisible(bool up = false, bool right = false, bool down = false, bool left = false) {
        // Active different doors at different rooms.
        if (up) {
            transform.Find("DoorUp").gameObject.SetActive(true);
        }
        if (right) {
            transform.Find("DoorRight").gameObject.SetActive(true);
        }
        if (down) {
            transform.Find("DoorDown").gameObject.SetActive(true);
        }
        if (left) {
            transform.Find("DoorLeft").gameObject.SetActive(true);
        }
        return false;
    }

    public void PopulateDoors(GameObject roomUp = null, GameObject roomDown = null, GameObject roomLeft = null, GameObject roomRight = null, bool nextWorld = false) {
        DoorLogic tempDoor;
        // Add correct fromRoom and toRoom to every room.
        switch (roomAction) {
            case RoomAction.Welcome:
                tempDoor = transform.Find("DoorUp").GetComponent<DoorLogic>();
                tempDoor.PopulateGameObjects(gameObject, roomUp, nextWorld);
                break;
            case RoomAction.Forward:
                tempDoor = transform.Find("DoorUp").GetComponent<DoorLogic>();
                tempDoor.PopulateGameObjects(gameObject, roomUp, nextWorld);
                tempDoor = transform.Find("DoorDown").GetComponent<DoorLogic>();
                tempDoor.PopulateGameObjects(gameObject, roomDown, nextWorld);
                if (roomLeft != null) {
                    tempDoor = transform.Find("DoorLeft").GetComponent<DoorLogic>();
                    tempDoor.PopulateGameObjects(gameObject, roomLeft, nextWorld);
                }
                if (roomRight != null) {
                    tempDoor = transform.Find("DoorRight").GetComponent<DoorLogic>();
                    tempDoor.PopulateGameObjects(gameObject, roomRight, nextWorld);
                }
                break;
            case RoomAction.Hallway:
                tempDoor = transform.Find("DoorLeft").GetComponent<DoorLogic>();
                tempDoor.PopulateGameObjects(gameObject, roomLeft, nextWorld);
                tempDoor = transform.Find("DoorRight").GetComponent<DoorLogic>();
                tempDoor.PopulateGameObjects(gameObject, roomRight, nextWorld);
                break;
            case RoomAction.Special:
                if (roomLeft != null) {
                    tempDoor = transform.Find("DoorLeft").GetComponent<DoorLogic>();
                    tempDoor.PopulateGameObjects(gameObject, roomLeft, nextWorld);
                }
                if (roomRight != null) {
                    tempDoor = transform.Find("DoorRight").GetComponent<DoorLogic>();
                    tempDoor.PopulateGameObjects(gameObject, roomRight, nextWorld);
                }
                break;
            case RoomAction.Boss:
                tempDoor = transform.Find("DoorUp").GetComponent<DoorLogic>();
                tempDoor.PopulateGameObjects(gameObject, roomUp, nextWorld);
                tempDoor = transform.Find("DoorDown").GetComponent<DoorLogic>();
                tempDoor.PopulateGameObjects(gameObject, roomDown, nextWorld);
                break;
        }
    }

    private void ApplyLocalScale(Transform trans, float width, float height, bool isHallway) {
        if (isHallway) { 
            Transform parent = trans.parent;
            trans.parent = null;
            trans.localScale = new Vector3(width, height, 1);
            trans.parent = parent;
        }
        else {
            Transform parent = trans.parent;
            trans.parent = null;
            trans.parent = null;
            trans.localScale = new Vector3(width, height, 1);
            trans.SetParent(parent);
        }
        this.width = width;
        this.height = height;
    }

}
