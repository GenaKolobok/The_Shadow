using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance {get; private set; }

    private PlayerInputActions _playerInputActions;

    public event EventHandler OnPlayerAttack;
    public event EventHandler OnInteract;
    public event EventHandler OnPlayerDash;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();
        _playerInputActions.Player.Attack.started += PlayerAttack_started;
        _playerInputActions.Player.Interact.started += Interact_started;
        _playerInputActions.Player.Dash.performed += PlayerDash_performed;

    }

    private void PlayerDash_performed(InputAction.CallbackContext obj)
    {
        OnPlayerDash?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_started(InputAction.CallbackContext obj)
    {
        OnInteract?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVector()
    {
        Vector2 InputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
        return InputVector;
    }

    public Vector3 GetMousePosition()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        return mousePos;
    }

    public void DisableMovement()
    {
        _playerInputActions.Disable();
    }

    public void EnableMovement()
    {
        _playerInputActions.Enable();
    }

    private void PlayerAttack_started(InputAction.CallbackContext obj)
    {
        OnPlayerAttack?.Invoke(this, EventArgs.Empty);
    }
}
