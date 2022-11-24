using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_DashAnim : StateMachineBehaviour
{
    Player.MainPlayerController player;
    Vector2 posDashTo;
    Vector2 newPos;
    [SerializeField]
    float speed;
    [SerializeField]
    float dashRange;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.GetComponent<Player.MainPlayerController>();

        // KO cho attack khi dash
        player.setIsAttack(false);

        if (player.normalAttackDelay != null)
            player.StopCoroutine(player.normalAttackDelay);

        //player.GetComponent<Collider2D>().isTrigger = true;

        player.setIsMove(false);
        player.GetComponent<Rigidbody2D>().gravityScale = 0f;

        // Get new pos
        if (player.isRight())
        {
            posDashTo = new Vector2(player.transform.position.x + dashRange, player.transform.position.y);
        }
        else
        {
            posDashTo = new Vector2(player.transform.position.x - dashRange, player.transform.position.y);
        }
        //
        Physics2D.IgnoreLayerCollision(7, 6, true);
        Physics2D.IgnoreLayerCollision(7, 9, true);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        newPos = Vector2.MoveTowards(player.transform.position, posDashTo, Time.deltaTime * speed);
        player.GetComponent<Rigidbody2D>().MovePosition(newPos);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.setIsAttack(true);

        player.GetComponent<Rigidbody2D>().gravityScale = 3f;
        player.GetComponent<Rigidbody2D>().velocity = Vector2.right * 0f;
        player.StartCoroutine(player.DelayDash());
        player.setIsMove(true);
        //player.GetComponent<Collider2D>().isTrigger = false;
        //Physics2D.IgnoreLayerCollision(7, 6, false);
        Physics2D.IgnoreLayerCollision(7, 6, false);
        Physics2D.IgnoreLayerCollision(7, 9, true);

        player.normalAttackDelay = player.StartCoroutine(player.InscreaseMana());
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
