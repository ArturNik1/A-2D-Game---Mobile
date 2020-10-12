using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopHandler : StateMachineBehaviour
{
    int rand;

    private void Awake()
    {
        rand = Random.Range(2, 5);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= rand) MenuLogic.doTransition = true;
    }

}
