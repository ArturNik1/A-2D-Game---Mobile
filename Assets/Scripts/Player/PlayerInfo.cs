using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [Header("Variables")]
    public int health;
    public float attackSpeed;
    public float movementSpeed;

    [HideInInspector]
    public int maxHealth;

    private PlayerController pController;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;

        pController = GetComponent<PlayerController>();

        UpdateControllerValues();
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateControllerValues();
    }

    public void UpdateControllerValues() {
        pController.movementSpeed = this.movementSpeed;
    }

}
