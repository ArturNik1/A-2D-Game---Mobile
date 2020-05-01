using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    [Header("Initialize")]
    public GameObject activeModel;

    [Header("Inputs")]
    public float vertical;
    public float horizontal;

    [Header("States")]
    public bool canMove;
    [HideInInspector]
    public bool isMoving;
    [HideInInspector]
    public bool isIdle;

    PlayerController pController;
    public Animator anim;
    [HideInInspector]
    public Rigidbody2D rigid;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        SetupAnimator();
        rigid = GetComponent<Rigidbody2D>();
    }

    void Init() {
        pController = GetComponent<PlayerController>();
    }

    void SetupAnimator() { 
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
        UpdateStates();
    }

    void UpdateStates() {
        canMove = anim.GetBool("canMove");

        if (isMoving && canMove) { 
            //anim.
        }

        HandleMovementAnimations();
    }

    void HandleMovementAnimations() {
        anim.SetFloat("vertical", Mathf.Abs(vertical));
        anim.SetFloat("horizontal", Mathf.Abs(horizontal));
    }

}
