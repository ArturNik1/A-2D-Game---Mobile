using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitAttackStateSkeleton : StateMachineBehaviour
{

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<SkeletonController>().isAttacking = false;
    }
}
