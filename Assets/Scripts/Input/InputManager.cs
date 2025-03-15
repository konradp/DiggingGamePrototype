using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, IInputManager
{
    [SerializeField] private PlayerInput playerInput;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public Vector2 GetMovement()
    {
        return playerInput.currentActionMap.actions[0].ReadValue<Vector2>();
    }

    public Vector2 GetLook()
    {
        return playerInput.currentActionMap.actions[1].ReadValue<Vector2>();
    }

    public float GetAttack()
    {
        return playerInput.currentActionMap.actions[2].ReadValue<float>();
    }

    public bool GetAttackPerformed()
    {
        return playerInput.currentActionMap.actions[2].WasPerformedThisFrame();
    }

    public bool GetInteractPerformed()
    {
        return playerInput.currentActionMap.actions[3].WasPerformedThisFrame();
    }

    public float GetSprint()
    {
        return playerInput.currentActionMap.actions[8].ReadValue<float>();
    }

    public bool GetJump()
    {
        return playerInput.currentActionMap.actions[5].ReadValue<float>() > 0.01f;

    }
}

public interface IInputManager
{
    public Vector2 GetMovement();
    public Vector2 GetLook();
    public float GetAttack();
    public bool GetAttackPerformed();
    public bool GetInteractPerformed();
    public float GetSprint();
    public bool GetJump();
}
