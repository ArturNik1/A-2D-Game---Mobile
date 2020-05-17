using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum RoomType { Tiny, Small, Medium, LargeH, LargeV, Huge, Gigantic }

public class LevelManager : MonoBehaviour
{
    public static int currentWorld = 0;
    public static int currentRoomGenerated = 0;
    public static RoomType currentRoomType;

    public GameObject roomPrefab;
    public GameObject roomsHolder;

    private Dictionary<int, GameObject> worldRooms;

    private void Awake() {
        SceneManager.sceneLoaded += OnSceneWasSwitched;
    }

    private void OnEnable() {
        worldRooms = new Dictionary<int, GameObject>();
        if (currentWorld == 0) {
            CreateRooms();
        }
    }

    void CreateRooms() {
        switch (currentWorld) {
            case 0:
                #region World_One
                GameObject newRoom;
                // Create first welcoming room and add it to worldRooms.
                newRoom = Instantiate(roomPrefab, roomsHolder.transform);
                newRoom.GetComponent<RoomLogic>().GenerateWelcomingRoom();
                worldRooms.Add(currentRoomGenerated, newRoom);
                currentRoomGenerated++;

                // Amount of forward rooms until getting to the boss.
                int forwardRooms = Random.Range(3, 6); // how many monster rooms 3-5
                // Amount of item rooms generated.
                int itemRooms = Random.Range(1, 3); // 1-2 rooms
                // Amount of shop rooms generated.
                int shopRooms = Random.Range(1, 1);

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

                for (int i=0; i<forwardRooms; i++) {
                    // Generate new forward room, set position and add it to worldRooms.
                    newRoom = Instantiate(roomPrefab, roomsHolder.transform);
                    newRoom.transform.localPosition = new Vector3(newRoom.transform.localPosition.x,
                        worldRooms[i].transform.localPosition.y + 4, newRoom.transform.localPosition.z);
                    newRoom.GetComponent<RoomLogic>().GenerateForwardRoom();
                    worldRooms.Add(currentRoomGenerated, newRoom);

                    // if hallway and item room should be generated, proceed to generate and setup both.
                    if (rooms.Count > 0 && rooms[0] == i+1) {
                        GameObject specialRoom, specialItemRoom;
                        specialRoom = Instantiate(roomPrefab, newRoom.transform);
                        // Randomly choose hallway direction, setup it's transform.
                        bool left = Random.Range(0, 2) == 0 ? true : false;
                        if (left) {
                            specialRoom.transform.localPosition = new Vector3(specialRoom.transform.localPosition.x - 0.3f,
                                specialRoom.transform.localPosition.y, specialRoom.transform.localPosition.z);
                            specialItemRoom = Instantiate(roomPrefab, specialRoom.transform);
                            specialItemRoom.transform.localPosition = new Vector3(specialItemRoom.transform.localPosition.x - 0.2f,
                                specialItemRoom.transform.localPosition.y, specialItemRoom.transform.localPosition.z);
                        }
                        else {
                            specialRoom.transform.localPosition = new Vector3(specialRoom.transform.localPosition.x + 0.3f,
                                specialRoom.transform.localPosition.y, specialRoom.transform.localPosition.z);
                            specialItemRoom = Instantiate(roomPrefab, specialRoom.transform);
                            specialItemRoom.transform.localPosition = new Vector3(specialItemRoom.transform.localPosition.x + 0.2f,
                                specialItemRoom.transform.localPosition.y, specialItemRoom.transform.localPosition.z);
                        }
                        // Generate noth rooms.
                        specialRoom.GetComponent<RoomLogic>().GenerateHallwayRoom(left);
                        specialItemRoom.GetComponent<RoomLogic>().GenerateItemRoom(left);
                        // Remove item and hallway room from the list.
                        rooms.RemoveAt(0);
                    }

                    currentRoomGenerated++;
                }
                // Generate and setup boss room.
                newRoom = Instantiate(roomPrefab, roomsHolder.transform);
                newRoom.transform.localPosition = new Vector3(newRoom.transform.localPosition.x,
                        worldRooms[worldRooms.Count - 1].transform.localPosition.y + 4, newRoom.transform.localPosition.z);
                newRoom.GetComponent<RoomLogic>().GenerateBossRoom();
                worldRooms.Add(currentRoomGenerated, newRoom);
                currentRoomGenerated++;

                // fill door actions.
                for (int i = 0; i < worldRooms.Count; i++) {
                    if (i == worldRooms.Count-1) // head to next world
                        // boss room
                        worldRooms[i].GetComponent<RoomLogic>().PopulateDoors(roomDown: worldRooms[i - 1], nextWorld: true);
                    else {
                        if (i == 0) {
                            // welcoming room
                            worldRooms[i].GetComponent<RoomLogic>().PopulateDoors(roomUp: worldRooms[i+1]);
                        } else {
                            // forward rooms
                            RoomLogic roomInfo = worldRooms[i].GetComponent<RoomLogic>();

                            // if there is no hallway just populate it's doors, if it has them, do the opposite.
                            if (!roomInfo.hasHallway) {
                                roomInfo.PopulateDoors(roomUp: worldRooms[i + 1], roomDown: worldRooms[i - 1]);
                            } else {
                                newRoom = worldRooms[i].transform.Find("HallwayRoom").gameObject;
                                GameObject specialRoom = newRoom.transform.Find("SpecialRoom").gameObject;
                                if (roomInfo.hallwayIsLeft) {
                                    roomInfo.PopulateDoors(roomUp: worldRooms[i + 1], roomDown: worldRooms[i - 1], roomLeft: newRoom);
                                    newRoom.GetComponent<RoomLogic>().PopulateDoors(roomRight: worldRooms[i], roomLeft: specialRoom);
                                    specialRoom.GetComponent<RoomLogic>().PopulateDoors(roomRight: newRoom);
                                } else {
                                    roomInfo.PopulateDoors(roomUp: worldRooms[i + 1], roomDown: worldRooms[i - 1], roomRight: newRoom);
                                    newRoom.GetComponent<RoomLogic>().PopulateDoors(roomLeft: worldRooms[i], roomRight: specialRoom);
                                    specialRoom.GetComponent<RoomLogic>().PopulateDoors(roomLeft: newRoom);
                                }
                            }
                        }
                    }
                }

                break;
            #endregion
            case 1:
                break;
            case 2:
                break;
        }
    }

    void OnSceneWasSwitched(Scene scene, LoadSceneMode mode) {
        currentWorld = 0;
        currentRoomGenerated = 0;
        ProjectileController.damageAmount = 5;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneWasSwitched;
    }

}
