using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class ComboCharacter : MonoBehaviour
{

    private StateMachine meleeStateMachine;

    [SerializeField]
    public Collider2D hitbox;
    [SerializeField]
    public GameObject Hiteffect;

    Keyboard key;

    // Start is called before the first frame update
    void Start()
    {
        key = Keyboard.current;

        meleeStateMachine = GetComponent<StateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        if (key.xKey.IsPressed() && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
        {
            meleeStateMachine.SetNextState(new GroundEntryState());
        }
    }
}