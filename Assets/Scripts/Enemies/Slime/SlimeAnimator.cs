﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAnimator : MonoBehaviour
{
    [Header("Initialize")]
    public GameObject activeModel;

    Animator anim;
    SlimeController slimeController;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }


    // Update is called once per frame
    void Update()
    {
        UpdateStates();
    }

    void Init() {
        slimeController = GetComponent<SlimeController>();

        if (activeModel == null) {
            anim = GetComponentInChildren<Animator>();
            if (anim == null) {
                Debug.Log("No Model!");
            }
            else {
                activeModel = anim.gameObject;
            }
        }

        if (anim == null)
            anim = activeModel.GetComponent<Animator>();
    }

    void UpdateStates() {

        anim.SetBool("isMoving", slimeController.isMoving);

    }

}