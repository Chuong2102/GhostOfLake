using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
public class Main_PariAnim : StateMachineBehaviour
{
    MainPlayerController player;
    SlowMotion slow;
    bool isSlow;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.GetComponent<MainPlayerController>();
        slow = player.GetComponent<SlowMotion>();
        isSlow = true;

        if (player.normalAttackDelay != null)
            player.StopCoroutine(player.normalAttackDelay);

        player.setIsPari(true);
        player.setIsMove(false);

        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player.GetIsAttackedByEnemy() && isSlow)
        {
            isSlow = false;
            slow.StartSlowMotion();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.setIsPari(false);
        player.setIsTakeParry(true);
        player.setStunParry(true);
        player.setIsMove(true);
        player.GetComponent<Rigidbody2D>().velocity = Vector2.right * 0f;
        player.normalAttackDelay = player.StartCoroutine(player.InscreaseMana());

        if (player.GetIsAttackedByEnemy())
        {
            slow.StopSlowMotion();
            player.SetIsAttackedByEnemy(false);
            isSlow = true;
        }


    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
