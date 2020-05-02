﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    [Header("Variables")]
    public int health;
    public float attackSpeed;
    public float movementSpeed;

    private PlayerController pController;

    // Start is called before the first frame update
    void Start()
    {
        pController = GetComponent<PlayerController>();

        UpdateControllerValues();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateControllerValues() {
        pController.movementSpeed = this.movementSpeed;
    }

}
