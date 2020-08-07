using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcSpinStageTwo : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioManager.instance.Play("OrcSpinning01");
        animator.GetComponent<OrcController>().isSpinStageTwo = true;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AudioManager.instance.StopPlaying("OrcSpinning01");
        animator.GetComponent<OrcController>().isSpinStageTwo = false;
        animator.GetComponent<OrcController>().SetRotateSpeed();
    }

}
