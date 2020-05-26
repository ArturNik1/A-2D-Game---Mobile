using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemController : MonoBehaviour
{
    public enum GolemStates { Idle, Scanning, Moving, Locked, Attacking, Dizzy, Winning, Dying }
    GolemStates previousState;
    GolemStates currentState;

    [Header("Initialize")]
    public GameObject activeModel;

    [Header("General Info")]
    public float health;
    public float speed;
    public int damage;

    [HideInInspector]
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        currentState = GolemStates.Idle;
    }

    void Init() { 
        // Setup active model and animator.
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
