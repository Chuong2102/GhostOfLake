using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFlyAttackToPoint : StateMachineBehaviour
{
    Enemy enemy;
    Player.MainPlayerController player;

    Vector3 startPos;
    Vector3 endPos;
    [SerializeField]
    float speed;
    float time;
    [SerializeField]
    AnimationCurve curve;

    private float baseScaleX;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
        player = Player.MainPlayerController.Instance;
        

        time = 0f;

        startPos = enemy.transform.position;
        // trai phai?
        int direction = 1;
        if (player != null)
        {
            direction = player.transform.position.x - enemy.transform.position.x < 0 ? -1 : 1;

            endPos = enemy.GetComponent<FlyEnemy>().NextPointToFlyAttack();
        }

    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Fly to
        time += Time.deltaTime;
        Vector3 pos = Vector3.Lerp(startPos, endPos, time * speed);
        pos.y += curve.Evaluate(time);
        enemy.transform.position = pos;
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        time = 0f;
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
