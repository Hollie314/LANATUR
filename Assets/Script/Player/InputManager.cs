using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    public PlayerControls Controls { get; private set; }

    private PlayerControls.OnFootActions onFoot;
    private PlayerControls.UIActions uiActions;
    
    public PlayerControls.OnFootActions OnFoot => onFoot;
    public PlayerControls.UIActions UI => uiActions;


    private PlayerMotor motor;
    private PlayerLook look;

    public bool canMove = true;

    void Awake()
    {
        Controls = new PlayerControls();
        onFoot = Controls.OnFoot;
        uiActions = Controls.UI;
        
        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();
        
        onFoot.Jump.performed += ctx => motor.Jump();

        onFoot.Crouch.performed += ctx => motor.Crouch();
        onFoot.Sprint.performed += ctx => motor.Sprint();
    }

    void FixedUpdate()
    {
        //tell the playmotor to move using the value from our movement action.
        if(canMove)
        {
            motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
        }
    }

    private void LateUpdate()
    {
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }
    private void OnEnable()
    {
        onFoot.Enable();
        uiActions.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
        uiActions.Disable();
    }
}
