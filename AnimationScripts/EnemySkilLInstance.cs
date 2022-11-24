using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkilLInstance : StateMachineBehaviour
{
    [SerializeField]
    GameObject skill;
    [SerializeField]
    float timeDelay;
    [SerializeField]
    float left;
    [SerializeField]
    float right;

    Player.MainPlayerController player;

    bool isInstance;

    Enemy enemy;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
        player = Player.MainPlayerController.Instance;
        isInstance = true;

        enemy.StartCoroutine(DelayTime());
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (isInstance)
        {
            isInstance = false;
            var point = enemy.transform.position.x > player.transform.position.x ? left : right;
            GameObject.Instantiate(skill, new Vector2(enemy.transform.position.x + point, enemy.transform.position.y), Quaternion.Euler(new Vector3(-90f, 0f, 0f))).GetComponent<ParticleSystem>().Play();
        }
            
    }

    IEnumerator DelayTime()
    {
        yield return new WaitForSeconds(timeDelay);
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

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
