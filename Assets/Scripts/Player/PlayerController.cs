using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerConfig playerConfig;
    [SerializeField] private GameObject coreRef;
    [SerializeField] private GameObject cameraPivot;
    
    private IInputManager inputManager;
    private Rigidbody rigidbody;
    
    private Quaternion targetRotationX;
    private bool grounded;

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

        MouseRotationY();
        TryDeform();
    }
    
    private void FixedUpdate()
    {
        if (inputManager == null)
        {
            Debug.LogError("No input manager. No Movement can perform.");
            return;
        }
        
        PlayerMovement();
        MouseRotationX();
        PlayerJump();
    }
    
    private void OnCollisionStay(Collision other)
    {
        grounded = true;
    }

    private void OnCollisionExit(Collision other)
    {
        grounded = false;
    }

    #region === Movement & Rotation ===
    
    private void MouseRotationY()
    {
        float rot = -inputManager.GetLook().y * playerConfig.playerMovementConfig.MouseSensitivityY * Time.deltaTime;

        var rotator = cameraPivot.transform;
        rotator.Rotate(rot, 0f, 0f, Space.Self);
        var rotval = rotator.localEulerAngles.x;

        if (rotval < playerConfig.playerMovementConfig.MaxAngle && rotval > 180f)
        {
            rotator.localEulerAngles = new
                Vector3(playerConfig.playerMovementConfig.MaxAngle, rotator.localEulerAngles.y, rotator.localEulerAngles.z);
        }

        else if (rotval > playerConfig.playerMovementConfig.MinAngle && rotval < 180f)
        {
            rotator.localEulerAngles = new
                Vector3(playerConfig.playerMovementConfig.MinAngle, rotator.localEulerAngles.y, rotator.localEulerAngles.z);
        }
    }
    
    private void MouseRotationX()
    {
        var horizontal = inputManager.GetLook().x * playerConfig.playerMovementConfig.MouseSensitivityX * Time.fixedDeltaTime;
        targetRotationX *= Quaternion.AngleAxis(horizontal, Vector3.up);
        Quaternion yRotation = Quaternion.Lerp(rigidbody.rotation, targetRotationX, playerConfig.playerMovementConfig.SmoothTimeX);
        rigidbody.MoveRotation(yRotation);
    }

    private void PlayerMovement()
    {
        if (Mathf.Approximately(inputManager.GetMovement().magnitude, 0))
        {
            return;
        }
        
        var speed = inputManager.GetSprint() > 0.1f
            ? playerConfig.playerMovementConfig.SprintSpeed : playerConfig.playerMovementConfig.Speed;

        Vector3 moveAxis = speed * Time.fixedDeltaTime * (gameObject.transform.forward * inputManager.GetMovement().y
                                                          + gameObject.transform.right * inputManager.GetMovement().x);
        rigidbody.MovePosition(gameObject.transform.position + moveAxis);
    }

    private void PlayerJump()
    {
        if (grounded && inputManager.GetJump())
        {
            rigidbody.AddForce(Vector3.up * playerConfig.playerMovementConfig.JumpForce, ForceMode.Impulse);
        }
    }
    #endregion
    
    #region === Interaction ===

    private void TryDeform()
    {
        if (!inputManager.GetInteractPerformed())
        {
            return;
        }
        
        RaycastHit hit;
        
        if (Physics.Raycast(cameraPivot.transform.position, cameraPivot.transform.forward, out hit, 
                playerConfig.playerInteractionConfig.InteractionRange, playerConfig.playerInteractionConfig.InteractionLayer))
        {
            var deformer = hit.transform.GetComponent<IMeshDeformer>();
            if (deformer != null)
            {
                deformer.Deform(hit.point);
            }
        }
    }
    
    #endregion
}


