using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAnimMix : StateMachineBehaviour
{
    float slideBuffer = 0.40f;
    bool shot = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        slideBuffer = 0.40f;
        shot = false;

        if (stateInfo.IsName("SlideRShootL"))
            animator.SetInteger("ShootResult", 1);
        else
            animator.SetInteger("ShootResult", 2);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (slideBuffer <= 0)
        {
            animator.SetBool("SlideFire", true);
            shot = true;
            slideBuffer = 1f;
        }
        else if(!shot)
        {
            slideBuffer -= Time.deltaTime;
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Fire", true);
    }
}
