using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    [Header("General Info")]
    public int health;
    public float speed;

    [Header("Room Info")]
    public int worldNumber;
    public int roomNumber;
    public int subRoomNumber;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReceiveDamage(int amount) {
        health -= amount;
        if (health <= 0) Destroy(gameObject);
    }

    public void UpdateRoomInfo(GameObject room) {
        if (room.name.StartsWith("Hall") || room.name.StartsWith("Special")) return;
        string[] s = room.name.Split('_');
        this.worldNumber = int.Parse(s[0].Substring(s[0].Length - 1, 1));
        this.roomNumber = int.Parse(s[1]);
    }

}
