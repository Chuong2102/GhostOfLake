using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
    Player.MainPlayerController.JumpCommand jumpCommand = new Player.MainPlayerController.JumpCommand();
    MainPlayerController.MoveCommand moveCommand = new MainPlayerController.MoveCommand();
    MainPlayerController.AttackCommand attackNgangCommand = new MainPlayerController.AttackCommand();
    MainPlayerController.PariCommand pariCommand = new MainPlayerController.PariCommand();
    MainPlayerController.CancelPariCommand cancelPariCommand = new MainPlayerController.CancelPariCommand();
    MainPlayerController.DashCommand dashCommand = new MainPlayerController.DashCommand();
    MainPlayerController.HeavyAttackCommand heavyAttackComd = new MainPlayerController.HeavyAttackCommand();
    MainPlayerController.MoveCamera moveCamComd = new MainPlayerController.MoveCamera();

    public KeyCode keyJump;
    public KeyCode keyAttackNgang;
    public KeyCode keyPari;
    public KeyCode keyDash;
    public KeyCode keyHeavyAttack;
    public KeyCode keyRight;
    public KeyCode keyLeft;
    public KeyCode keyMoveCam;

    [SerializeField]
    Player.MainPlayerController player;

    private void FixedUpdate()
    {
        //// Move
        ////
        //if (Input.GetKey(keyRight) || Input.GetKey(keyLeft))
        //    moveCommand.Execute(player);

        
        //// Move Camera
        ////
        //if (Input.GetKey(keyMoveCam))
        //    moveCamComd.Execute(player);
        //else if (Input.GetKeyUp(keyMoveCam))
        //    player.MoveVirtualCamBack();

    }

    void ProcessInput()
    {
        // Reset
        //if (Input.GetKeyDown(KeyCode.R))
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        // Pari
        //
        
    }
}
