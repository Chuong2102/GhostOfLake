using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDashToPlayer : StateMachineBehaviour
{
    Enemy enemy;
    Player.MainPlayerController player;

    Vector3 startPos;
    Vector3 endPos;
    [SerializeField]
    float speed;
    [SerializeField]
    float distanceThroughToPlayer;
    [SerializeField]
    AnimationCurve curve;

    bool isGetPlayerPos;

    float time;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
        player = Player.MainPlayerController.Instance;
        time = 0f;

        isGetPlayerPos = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (isGetPlayerPos)
        {
            isGetPlayerPos = false;
            startPos = enemy.transform.position;
            // trai phai?
            int direction = 1;
            if (player != null)
            {
                direction = player.transform.position.x > enemy.transform.position.x ? 1 : -1;

                endPos = new Vector3(player.transform.position.x + direction * distanceThroughToPlayer, player.transform.position.y, 0f);
            }

        }

        // Dash to player
        time += Time.deltaTime;
        Vector3 pos = Vector3.Lerp(startPos, endPos, time * speed);
        pos.x += curve.Evaluate(time);
        enemy.transform.position = pos;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isGetPlayerPos = true;
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
