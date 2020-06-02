using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemController : BossController
{
    public enum GolemStates { Idle, Scanning, Moving, Locked, Attacking, Dizzy, Winning, Dying }
    GolemStates previousState;
    GolemStates currentState;


    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        currentState = GolemStates.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HandleStates() { 
        
    }

    public override void KillBoss()
    {
        previousState = currentState;
        currentState = GolemStates.Dying;
        PlayDeathAnimation();
    }
}
