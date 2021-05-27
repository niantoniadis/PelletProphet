using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAnimSlide : StateMachineBehaviour
{
    float slideBuffer = 0.40f;
    bool shot = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        slideBuffer = 0.4f;
        shot = false;

        animator.SetInteger("ShootResult", 3);
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
}
