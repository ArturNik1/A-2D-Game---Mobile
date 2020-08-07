﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageDizzy : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<EvilMageController>().EndDizzyState();    
    }
}
