using System;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [SerializeField] private MovementConfig movementConfig;
    [SerializeField] private GameObject coreRef;
    [SerializeField] private GameObject cameraPivot;
    
    private IInputManager inputManager;
    private Rigidbody rigidbody;
    private Quaternion targetRotationX;

    private void Start()
    {
        inputManager = coreRef.GetComponent<IInputManager>();
        rigidbody = GetComponent<Rigidbody>();
        targetRotationX = rigidbody.rotation;
        
    }

    private void Update()
    {
        if (inputManager == null)
        {
            Debug.LogError("No input manager. No Movement can perform.");
            return;
        }

        float rot = -inputManager.GetLook().y * movementConfig.playerMovementConfig.MouseSensitivityY * Time.deltaTime;

        Transform _rotator = cameraPivot.transform;
        _rotator.Rotate(rot, 0f, 0f, Space.Self);
        var rotval = _rotator.localEulerAngles.x;

        if (rotval < movementConfig.playerMovementConfig.MaxAngle && rotval > 180f)
        {
            _rotator.localEulerAngles = new
                Vector3(movementConfig.playerMovementConfig.MaxAngle, _rotator.localEulerAngles.y, _rotator.localEulerAngles.z);
        }

        else if (rotval > movementConfig.playerMovementConfig.MinAngle && rotval < 180f)
        {
            _rotator.localEulerAngles = new
                Vector3(movementConfig.playerMovementConfig.MinAngle, _rotator.localEulerAngles.y, _rotator.localEulerAngles.z);
        }
    }

    private void FixedUpdate()
    {
        if (inputManager.GetMovement().magnitude > 0)
        {
            var speed = inputManager.GetSprint() > 0.1f
                ? movementConfig.playerMovementConfig.SprintSpeed : movementConfig.playerMovementConfig.Speed;

            Vector3 moveAxis = speed * Time.fixedDeltaTime * (gameObject.transform.forward * inputManager.GetMovement().y
                                                              + gameObject.transform.right * inputManager.GetMovement().x);
            rigidbody.MovePosition(gameObject.transform.position + moveAxis);
        }
        var horizontal = inputManager.GetLook().x * movementConfig.playerMovementConfig.MouseSensitivityX * Time.fixedDeltaTime;
        targetRotationX *= Quaternion.AngleAxis(horizontal, Vector3.up);
        Quaternion yRotation = Quaternion.Lerp(rigidbody.rotation, targetRotationX, movementConfig.playerMovementConfig.SmoothTimeX);
        rigidbody.MoveRotation(yRotation);

    }
}


