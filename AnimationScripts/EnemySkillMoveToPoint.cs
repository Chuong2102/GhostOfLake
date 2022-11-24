using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkillMoveToPoint : StateMachineBehaviour
{
    [SerializeField]
    float countOfSkill;
    [SerializeField]
    GameObject skill;
    [SerializeField]
    Transform rightPoint;
    [SerializeField]
    Transform leftPoint;
    Enemy enemy;
    Player.MainPlayerController player;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
        player = Player.MainPlayerController.Instance;
        enemy.StartCoroutine(Delay());
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {


    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    public IEnumerator Delay()
    {
        while(countOfSkill > 0)
        {
            var endPos = enemy.transform.position.x < player.transform.position.x ? rightPoint : leftPoint;
            Object.Instantiate(skill, endPos);
            yield return new WaitForSeconds(0.25f);
            countOfSkill--;
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
