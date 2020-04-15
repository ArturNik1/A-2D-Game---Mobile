using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPointHandler : MonoBehaviour
{
    public GameObject player;

    private PlayerController playerController;
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        speed = playerController.movementSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // Always follow player.
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }
}
