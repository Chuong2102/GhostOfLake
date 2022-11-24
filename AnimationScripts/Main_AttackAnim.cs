using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_AttackAnim : StateMachineBehaviour
{
    Player.MainPlayerController player;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.GetComponent<Player.MainPlayerController>();
        player.SetCancelAttack(true);
        
        if(player.normalAttackDelay != null)
            player.StopCoroutine(player.normalAttackDelay);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Ti sua lai
        //Debug.Log("Cancel attack:" + player.GetCancelAttack());
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //player.SetCancelAttack(true);
        player.setTakeDamageCount(0);

        //Debug.Log("Set cancel");
        //player.SetCancelAttack(false);
        player.StartCoroutine(player.DelayAttack());
        player.setIsTakeDamage(false);
        player.setIsTakeDamageToEnemy(false);
        if (player.getIsHeavyAttack() == false)
        {
            player.setIsHeavyAttack(true);
            player.setDamage(player.getNormalDamage());
        }
        player.normalAttackDelay = player.StartCoroutine(player.InscreaseMana());
        player.SetAttackAgian();
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
