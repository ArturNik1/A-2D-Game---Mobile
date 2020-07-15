using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterPlantIdleNormal : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<MonsterPlantController>().isAttacking = false;
    }

}
