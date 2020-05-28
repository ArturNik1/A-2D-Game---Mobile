using UnityEngine;
using System.Collections;

public class HumanoidExample : MonoBehaviour {

    public RASCAL.MouseOrbit mouseOrbit;

    public Animator[] animators;

    int targetIdx = 0;
    int animIdx = 0;

    readonly string[] anims = { "Walk", "Run", "Idle"};

    public void SwitchTarget(int delta) {
        targetIdx += delta;
        if(targetIdx < 0) {
            targetIdx = animators.Length - 1;
        }else if(targetIdx >= animators.Length) {
            targetIdx = 0;
        }

        mouseOrbit.target = animators[targetIdx].transform;
    }

    public void SetAnim(int delta) {
        animIdx += delta;
        if (animIdx < 0) {
            animIdx = anims.Length - 1;
        } else if (animIdx >= anims.Length) {
            animIdx = 0;
        }

        foreach (Animator animator in animators) {
            animator.Play(anims[animIdx]);
        }
    }

}
