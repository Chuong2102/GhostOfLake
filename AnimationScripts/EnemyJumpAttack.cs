using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumpAttack : StateMachineBehaviour
{
    Enemy enemy;
    Player.MainPlayerController player;
    
    Vector3 startPos;
    Vector3 endPos;
    [SerializeField]
    bool isGroundedEnemy;
    [SerializeField]
    float speed;
    [SerializeField]
    float distanceToPlayer;
    [SerializeField]
    AnimationCurve curveX;
    [SerializeField]
    AnimationCurve curveY;
    [SerializeField]
    float Y;
    [SerializeField]
    float delayTime;

    bool isGetPlayerPos;
    bool isJump;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.GetComponent<Enemy>();
        player = Player.MainPlayerController.Instance;

        isJump = false;
        isGetPlayerPos = true;

        enemy.StartCoroutine(DelayStartTime());
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
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
                direction = player.transform.position.x > enemy.transform.position.x ? -1 : 1;

                float posY = 0f;
                if (!isGroundedEnemy)
                    posY = player.transform.position.y + Y;
                else
                    posY = enemy.transform.position.y + Y;

                endPos = new Vector3(player.transform.position.x + distanceToPlayer * direction, posY, 0f);
            }

        }
        // Jump to
        if (isJump)
            enemy.JumpTo(startPos, endPos, speed, curveX, curveY);
    }

    IEnumerator DelayStartTime()
    {
        yield return new WaitForSeconds(delayTime);
        isJump = true;
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy.setTime(0f);
        isJump = false;
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
