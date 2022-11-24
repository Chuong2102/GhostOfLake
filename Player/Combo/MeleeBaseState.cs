using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class MeleeBaseState : State
{
    // How long this state should be active for before moving on
    public float duration;
    // Cached animator component
    protected Animator animator;
    // bool to check whether or not the next attack in the sequence should be played or not
    protected bool shouldCombo;
    // The attack index in the sequence of attacks
    protected int attackIndex;

    // Input buffer Timer
    private float AttackPressedTimer = 0;

    // Keyboard
    Keyboard keyboard;


    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);
        animator = GetComponent<Animator>();
        keyboard = Keyboard.current;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        AttackPressedTimer -= Time.deltaTime;

        

        if (keyboard.xKey.wasPressedThisFrame)
        {
            AttackPressedTimer = 2;
        }

        if (animator.GetFloat("AttackWindow.Open") > 0f && AttackPressedTimer > 0)
        {
            shouldCombo = true;
        }


    }

    public override void OnExit()
    {
        base.OnExit();
    }

    protected void Attack()
    {

    }
}
