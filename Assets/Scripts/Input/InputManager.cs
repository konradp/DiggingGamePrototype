using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, IInputManager
{
    [SerializeField] private PlayerInput _playerInput;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public Vector2 GetMovement()
    {
        return _playerInput.currentActionMap.actions[0].ReadValue<Vector2>();
    }

    public Vector2 GetLook()
    {
        return _playerInput.currentActionMap.actions[1].ReadValue<Vector2>();
    }

    public float GetInteract()
    {
        return _playerInput.currentActionMap.actions[2].ReadValue<float>();
    }

    public float GetSprint()
    {
        return _playerInput.currentActionMap.actions[8].ReadValue<float>();
    }
}

public interface IInputManager
{
    public Vector2 GetMovement();
    public Vector2 GetLook();
    public float GetInteract();
    public float GetSprint();
}
