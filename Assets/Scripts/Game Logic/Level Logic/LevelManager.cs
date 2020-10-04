using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum RoomType { Tiny, Small, Medium, LargeH, LargeV, Huge, Gigantic }

public class LevelManager : MonoBehaviour
{
    public static int currentWorld = 0;
    public static int currentRoomGenerated = 0;
    public static RoomType currentRoomType;

    List<Tuple<int, int>> forwardMinMax = new List<Tuple<int, int>>();
    List<Tuple<int, int>> itemMinMax = new List<Tuple<int, int>>();
    List<Tuple<int, int>> shopMinMax = new List<Tuple<int, int>>();

    public GameObject roomPrefab;
    public GameObject roomsHolder;

    public GameObject[] _roomItem;
    Dictionary<int, List<GameObject>> roomItem = new Dictionary<int, List<GameObject>>();

    public GameObject[] _roomTiny;
    Dictionary<int, List<GameObject>> roomTiny = new Dictionary<int, List<GameObject>>();

    public GameObject[] _roomSmall;
    Dictionary<int, List<GameObject>> roomSmall = new Dictionary<int, List<GameObject>>();

    public GameObject[] _roomMedium;
    Dictionary<int, List<GameObject>> roomMedium = new Dictionary<int, List<GameObject>>();

    public GameObject[] _roomLargeH;
    Dictionary<int, List<GameObject>> roomLargeH = new Dictionary<int, List<GameObject>>();

    public GameObject[] _roomLargeV;
    Dictionary<int, List<GameObject>> roomLargeV = new Dictionary<int, List<GameObject>>();

    public GameObject[] _roomHuge;
    Dictionary<int, List<GameObject>> roomHuge = new Dictionary<int, List<GameObject>>();

    public GameObject[] _roomGigantic;
    Dictionary<int, List<GameObject>> roomGigantic = new Dictionary<int, List<GameObject>>();

    private Dictionary<int, GameObject> worldRooms;

    private void Awake() {
        SceneManager.sceneLoaded += OnSceneWasSwitched;
    }

    private void OnEnable() {
        worldRooms = new Dictionary<int, GameObject>();
        if (currentWorld == 0) {
            PopulateTuples();
            PopulateRoomDictionaries();
            CreateRooms();
        }
    }

    void PopulateTuples() {
        forwardMinMax.Add(new Tuple<int, int>(2, 5)); // 2-4 rooms
        itemMinMax.Add(new Tuple<int, int>(1, 2)); // 1 room

        forwardMinMax.Add(new Tuple<int, int>(3, 6)); // 3-5 rooms
        itemMinMax.Add(new Tuple<int, int>(1, 3)); // 1-2 rooms

        forwardMinMax.Add(new Tuple<int, int>(3, 7)); // 3-6 rooms
        itemMinMax.Add(new Tuple<int, int>(1, 3)); // 1-2 rooms

        forwardMinMax.Add(new Tuple<int, int>(4, 7)); // 4-6 rooms
        itemMinMax.Add(new Tuple<int, int>(1, 3)); // 1-2 rooms

        forwardMinMax.Add(new Tuple<int, int>(5, 8)); // 5-7 rooms
        itemMinMax.Add(new Tuple<int, int>(2, 4)); // 2-3 rooms

        forwardMinMax.Add(new Tuple<int, int>(6, 7)); // 6-7 rooms
        itemMinMax.Add(new Tuple<int, int>(2, 4)); // 2-3 rooms
    }

    void PopulateRoomDictionaries() {
        foreach (var room in _roomItem) {
            int world = int.Parse(room.name.Split('_')[1]);
            Add(roomItem, world, room);
        }
        foreach (var room in _roomTiny) {
            int world = int.Parse(room.name.Split('_')[1]);
            Add(roomTiny, world, room);
        }
        foreach (var room in _roomSmall) {
            int world = int.Parse(room.name.Split('_')[1]);
            Add(roomSmall, world, room);
        }
        foreach (var room in _roomMedium) {
            int world = int.Parse(room.name.Split('_')[1]);
            Add(roomMedium, world, room);
        }
        foreach (var room in _roomLargeH) {
            int world = int.Parse(room.name.Split('_')[1]);
            Add(roomLargeH, world, room);
        }
        foreach (var room in _roomLargeV) {
            int world = int.Parse(room.name.Split('_')[1]);
            Add(roomLargeV, world, room);
        }
        foreach (var room in _roomHuge) {
            int world = int.Parse(room.name.Split('_')[1]);
            Add(roomHuge, world, room);
        }
        foreach (var room in _roomGigantic) {
            int world = int.Parse(room.name.Split('_')[1]);
            Add(roomGigantic, world, room);
        }
    }

    void Add(Dictionary<int, List<GameObject>> dic, int key, GameObject value) {
        if (!dic.ContainsKey(key)) {
            dic[key] = new List<GameObject>();
        }
        dic[key].Add(value);
    }

    Dictionary<int, List<GameObject>> RoomTypeToDictionary(RoomType type) { 
        switch (type) {
            case RoomType.Tiny:
                return roomTiny;
            case RoomType.Small:
                return roomSmall;
            case RoomType.Medium:
                return roomMedium;
            case RoomType.LargeH:
                return roomLargeH;
            case RoomType.LargeV:
                return roomLargeV;
            case RoomType.Huge:
                return roomHuge;
            case RoomType.Gigantic:
                return roomGigantic;
        }
        return roomLargeH;
    }

    public void CreateRooms() {
        worldRooms = new Dictionary<int, GameObject>();
        GameObject newRoom;
        // Create first welcoming room and add it to worldRooms.

        if (roomTiny.ContainsKey(currentWorld)) newRoom = Instantiate(roomTiny[currentWorld][Random.Range(0, roomTiny[currentWorld].Count)], roomsHolder.transform);
        else {
            int rand = Random.Range(0, roomTiny.Keys.Count);
            newRoom = Instantiate(roomTiny[rand][Random.Range(0, roomTiny[rand].Count)], roomsHolder.transform);
        } 

        newRoom.GetComponent<RoomLogic>().GenerateWelcomingRoom();
        worldRooms.Add(currentRoomGenerated, newRoom);
        currentRoomGenerated++;

        int forwardRooms, itemRooms, shopRooms;
        // Amount of forward rooms until getting to the boss.
        if (forwardMinMax[currentWorld] != null)
            forwardRooms = Random.Range(forwardMinMax[currentWorld].Item1, forwardMinMax[currentWorld].Item2);
        else {
            int index = Random.Range(0, forwardMinMax.Count);
            forwardRooms = Random.Range(forwardMinMax[index].Item1, forwardMinMax[index].Item2);
        }
        // Amount of item rooms generated.
        if (itemMinMax[currentWorld] != null)
            itemRooms = Random.Range(itemMinMax[currentWorld].Item1, itemMinMax[currentWorld].Item2);
        else {
            int index = Random.Range(0, itemMinMax.Count);
            itemRooms = Random.Range(itemMinMax[index].Item1, itemMinMax[index].Item2);
        }
        // Amount of shop rooms generated.
        shopRooms = Random.Range(1, 1);

        // Add random item room locations to rooms list, used later after forward rooms are generated.
        int generatedItemRooms = 0;
        List<int> rooms = new List<int>();
        rooms.Add(Random.Range(1, forwardRooms + 1));
        generatedItemRooms++;
        while (generatedItemRooms != itemRooms) {
            int num = Random.Range(1, forwardRooms + 1);
            if (!rooms.Contains(num)) {
                rooms.Add(num);
                generatedItemRooms++;
            }
        }
        rooms.Sort();

        for (int i = 0; i < forwardRooms; i++) {
            // Generate new forward room, set position and add it to worldRooms.

            RoomType type = RoomLogic.GetForwardRoomTypeWithoutSettingDim();
            var r = RoomTypeToDictionary(type);
            if (r.ContainsKey(currentWorld)) newRoom = Instantiate(r[currentWorld][Random.Range(0, r[currentWorld].Count)], roomsHolder.transform);
            else {
                int rand = Random.Range(0, r.Keys.Count);
                newRoom = Instantiate(r[rand][Random.Range(0, r[rand].Count)], roomsHolder.transform);
            }

            newRoom.transform.localPosition = new Vector3(newRoom.transform.localPosition.x,
                worldRooms[i].transform.localPosition.y + 4, newRoom.transform.localPosition.z);
            newRoom.GetComponent<RoomLogic>().GenerateForwardRoom(type);
            worldRooms.Add(currentRoomGenerated, newRoom);

            // if hallway and item room should be generated, proceed to generate and setup both.
            if (rooms.Count > 0 && rooms[0] == i + 1) {
                GameObject specialRoom, specialItemRoom;
                //specialRoom = Instantiate(roomPrefab, newRoom.transform);

                type = RoomLogic.GetHallwayRoomTypeWithoutSettingDim();
                r = RoomTypeToDictionary(type);
                if (r.ContainsKey(currentWorld)) specialRoom = Instantiate(r[currentWorld][Random.Range(0, r[currentWorld].Count)], newRoom.transform);
                else {
                    int rand = Random.Range(0, r.Keys.Count);
                    specialRoom = Instantiate(r[rand][Random.Range(0, r[rand].Count)], newRoom.transform);
                }

                // Randomly choose hallway direction, setup it's transform.
                bool left = Random.Range(0, 2) == 0 ? true : false;
                if (left) {
                    specialRoom.transform.localPosition = new Vector3(specialRoom.transform.localPosition.x - 0.3f,
                        specialRoom.transform.localPosition.y, specialRoom.transform.localPosition.z);

                    if (roomItem.ContainsKey(currentWorld)) specialItemRoom = Instantiate(roomItem[currentWorld][Random.Range(0, roomItem[currentWorld].Count)], specialRoom.transform);
                    else {
                        int rand = Random.Range(0, roomItem.Keys.Count);
                        specialItemRoom = Instantiate(roomItem[rand][Random.Range(0, roomItem[rand].Count)], specialRoom.transform);
                    }

                    specialItemRoom.transform.localPosition = new Vector3(specialItemRoom.transform.localPosition.x - 0.2f,
                        specialItemRoom.transform.localPosition.y, specialItemRoom.transform.localPosition.z);
                }
                else {
                    specialRoom.transform.localPosition = new Vector3(specialRoom.transform.localPosition.x + 0.3f,
                        specialRoom.transform.localPosition.y, specialRoom.transform.localPosition.z);

                    if (roomItem.ContainsKey(currentWorld)) specialItemRoom = Instantiate(roomItem[currentWorld][Random.Range(0, roomItem[currentWorld].Count)], specialRoom.transform);
                    else {
                        int rand = Random.Range(0, roomItem.Keys.Count);
                        specialItemRoom = Instantiate(roomItem[rand][Random.Range(0, roomItem[rand].Count)], specialRoom.transform);
                    }

                    specialItemRoom.transform.localPosition = new Vector3(specialItemRoom.transform.localPosition.x + 0.2f,
                        specialItemRoom.transform.localPosition.y, specialItemRoom.transform.localPosition.z);
                }
                // Generate noth rooms.
                specialRoom.GetComponent<RoomLogic>().GenerateHallwayRoom(left, type);
                specialItemRoom.GetComponent<RoomLogic>().GenerateItemRoom(left);
                // Remove item and hallway room from the list.
                rooms.RemoveAt(0);
            }

            currentRoomGenerated++;
        }
        // Generate and setup boss room.
        RoomType _type = RoomLogic.GetBossRoomTypeWithoutSettingDim();
        var _r = RoomTypeToDictionary(_type);
        if (_r.ContainsKey(currentWorld)) newRoom = Instantiate(_r[currentWorld][Random.Range(0, _r[currentWorld].Count)], roomsHolder.transform);
        else {
            int rand = Random.Range(0, _r.Keys.Count);
            newRoom = Instantiate(_r[rand][Random.Range(0, _r[rand].Count)], roomsHolder.transform);
        }

        newRoom.transform.localPosition = new Vector3(newRoom.transform.localPosition.x,
                worldRooms[worldRooms.Count - 1].transform.localPosition.y + 4, newRoom.transform.localPosition.z);
        newRoom.GetComponent<RoomLogic>().GenerateBossRoom(_type);
        worldRooms.Add(currentRoomGenerated, newRoom);
        currentRoomGenerated++;

        // fill door actions.
        for (int i = 0; i < worldRooms.Count; i++) {
            if (i == worldRooms.Count - 1) // head to next world
                // boss room
                worldRooms[i].GetComponent<RoomLogic>().PopulateDoors(roomDown: worldRooms[i - 1], nextWorld: true);
            else {
                if (i == 0) {
                    // welcoming room
                    worldRooms[i].GetComponent<RoomLogic>().PopulateDoors(roomUp: worldRooms[i + 1]);
                }
                else {
                    // forward rooms
                    RoomLogic roomInfo = worldRooms[i].GetComponent<RoomLogic>();

                    // if there is no hallway just populate it's doors, if it has them, do the opposite.
                    if (!roomInfo.hasHallway) {
                        roomInfo.PopulateDoors(roomUp: worldRooms[i + 1], roomDown: worldRooms[i - 1]);
                    }
                    else {
                        newRoom = worldRooms[i].transform.Find("HallwayRoom").gameObject;
                        GameObject specialRoom = newRoom.transform.Find("SpecialRoom").gameObject;
                        if (roomInfo.hallwayIsLeft) {
                            roomInfo.PopulateDoors(roomUp: worldRooms[i + 1], roomDown: worldRooms[i - 1], roomLeft: newRoom);
                            newRoom.GetComponent<RoomLogic>().PopulateDoors(roomRight: worldRooms[i], roomLeft: specialRoom);
                            specialRoom.GetComponent<RoomLogic>().PopulateDoors(roomRight: newRoom);
                        }
                        else {
                            roomInfo.PopulateDoors(roomUp: worldRooms[i + 1], roomDown: worldRooms[i - 1], roomRight: newRoom);
                            newRoom.GetComponent<RoomLogic>().PopulateDoors(roomLeft: worldRooms[i], roomRight: specialRoom);
                            specialRoom.GetComponent<RoomLogic>().PopulateDoors(roomLeft: newRoom);
                        }
                    }
                }
            }
        }

    }

    void OnSceneWasSwitched(Scene scene, LoadSceneMode mode) {
        currentWorld = 0;
        currentRoomGenerated = 0;

        ProjectileController.damageAmount = 5f;
        ProjectileController.critMultiplier = 1.1f;
        ProjectileController.critProcChance = 5f;
        ProjectileController.damageCounter = 0;
    }

    private void OnDestroy() {
        AudioManager.instance.UnMuteSound();
        AudioManager.instance.UnMuteMusic();
        SceneManager.sceneLoaded -= OnSceneWasSwitched;
    }

}
