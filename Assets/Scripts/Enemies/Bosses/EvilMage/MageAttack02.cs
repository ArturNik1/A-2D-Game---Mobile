using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageAttack02 : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<EvilMageController>().EndSpecialAttack();
    }

}
