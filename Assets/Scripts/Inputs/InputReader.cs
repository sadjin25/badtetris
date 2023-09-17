using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]
public class InputReader : DescriptionBaseSO
{
    public UnityAction<Vector2> MoveEvent = delegate { };
    public UnityAction SoftDropEvent = delegate { };
    public UnityAction SoftDropCancelEvent = delegate { };
    public UnityAction HardDropEvent = delegate { };
    public UnityAction RotateLEvent = delegate { };
    public UnityAction RotateREvent = delegate { };
    public UnityAction HoldEvent = delegate { };

    GameInput gameInput;

    void OnEnable()
    {
        if (gameInput == null)
        {
            gameInput = new GameInput();
        }
    }

    void OnDisable()
    {
        DisableAllInput();
    }

    public void DisableAllInput()
    {
        gameInput.Keyboard.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }
        if (context.canceled)
        {
            MoveEvent?.Invoke(Vector2.zero);
        }
    }

    public void OnSoftDrop(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SoftDropEvent?.Invoke();
        }
    }

    public void OnSoftDropCanceled(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            SoftDropCancelEvent?.Invoke();
        }
    }

    public void OnHardDrop(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            HardDropEvent?.Invoke();
        }
    }

    public void OnRotateL(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            RotateLEvent?.Invoke();
        }
    }

    public void OnRotateR(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            RotateREvent?.Invoke();
        }
    }

    public void OnHold(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            HoldEvent?.Invoke();
        }
    }
}
