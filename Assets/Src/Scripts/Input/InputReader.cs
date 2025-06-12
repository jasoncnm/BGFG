
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Input/Input Reader", fileName = "Input Reader")]
public class InputReader : ScriptableObject
{
    
    public InputActionAsset asset;

    public event UnityAction<Vector2> moveEvent;

    public event UnityAction jumpEvent;
    public event UnityAction jumpCancelledEvent;


    public event UnityAction pauseEvent;
    public event UnityAction unPauseEvent;


    InputAction moveAction;
    InputAction jumpAction;
    InputAction pauseAction;
    InputAction unPauseAction;

    private void OnEnable()
    {

        moveAction = asset.FindAction("Move", true);
        jumpAction = asset.FindAction("Jump");
        pauseAction = asset.FindAction("Pause", true);
        unPauseAction = asset.FindAction("UnPause");


        moveAction.started += OnMove;
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;

        jumpAction.started += OnJump;
        jumpAction.performed += OnJump;
        jumpAction.canceled += OnJump;
        jumpAction.Enable();


        pauseAction.started += OnPause;
        pauseAction.performed += OnPause;
        pauseAction.canceled += OnPause;

        unPauseAction.started += OnUnPause;
        unPauseAction.performed += OnUnPause;
        unPauseAction.canceled += OnUnPause;

        unPauseAction.Enable();


        moveAction.Enable();
        pauseAction.Enable();

    }

    private void OnDisable()
    {
        moveAction.started -= OnMove;
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;


        jumpAction.started -= OnJump;
        jumpAction.performed -= OnJump;
        jumpAction.canceled -= OnJump;

        pauseAction.started -= OnPause;
        pauseAction.performed -= OnPause;
        pauseAction.canceled -= OnPause;

        unPauseAction.started -= OnUnPause;
        unPauseAction.performed -= OnUnPause;
        unPauseAction.canceled -= OnUnPause;

        moveAction.Disable();
        jumpAction.Disable();
        pauseAction.Disable();
        unPauseAction.Disable();
    }


    void OnUnPause(InputAction.CallbackContext context)
    {
        if (unPauseEvent != null && context.started)
        {
            unPauseEvent.Invoke();
        }
    }

    void OnPause(InputAction.CallbackContext context)
    {
        if (pauseEvent != null && context.started)
        {
            pauseEvent.Invoke();
        }
    }

    void OnMove(InputAction.CallbackContext context)
    {
        moveEvent?.Invoke(context.ReadValue<Vector2>());

    }


    void OnJump(InputAction.CallbackContext context)
    {
        if (jumpEvent != null && context.started)
        {
            jumpEvent.Invoke();
        }

        if (jumpCancelledEvent != null && context.canceled)
        {
            jumpCancelledEvent.Invoke();
        }
    }


}
