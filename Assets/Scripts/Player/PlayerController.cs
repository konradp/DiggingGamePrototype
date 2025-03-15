using System;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerController
{
    [SerializeField] private PlayerConfig playerConfig;
    [SerializeField] private GameObject coreRef;
    [SerializeField] private GameObject cameraPivot;
    
    private IInputManager inputManager;
    private Rigidbody rigidbody;
    
    private Quaternion targetRotationX;
    private bool grounded;
    private int currentEnergy;
    private bool canInteract;
    private float interactTimer;
    
    public event Action<int> OnDeform;
    public PlayerConfig PlayerConfig => playerConfig;

    private void Start()
    {
        inputManager = coreRef.GetComponent<IInputManager>();
        rigidbody = GetComponent<Rigidbody>();
        targetRotationX = rigidbody.rotation;
        currentEnergy = playerConfig.PlayerInteractionConfig.BaseEnergy;
    }

    private void Update()
    {
        if (inputManager == null)
        {
            Debug.LogError("No input manager. No Movement can perform.");
            return;
        }

        MouseRotationY();
        InteractionCooldown();
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
        float rot = -inputManager.GetLook().y * playerConfig.PlayerMovementConfig.MouseSensitivityY * Time.deltaTime;

        var rotator = cameraPivot.transform;
        rotator.Rotate(rot, 0f, 0f, Space.Self);
        var rotval = rotator.localEulerAngles.x;

        if (rotval < playerConfig.PlayerMovementConfig.MaxAngle && rotval > 180f)
        {
            rotator.localEulerAngles = new
                Vector3(playerConfig.PlayerMovementConfig.MaxAngle, rotator.localEulerAngles.y, rotator.localEulerAngles.z);
        }

        else if (rotval > playerConfig.PlayerMovementConfig.MinAngle && rotval < 180f)
        {
            rotator.localEulerAngles = new
                Vector3(playerConfig.PlayerMovementConfig.MinAngle, rotator.localEulerAngles.y, rotator.localEulerAngles.z);
        }
    }
    
    private void MouseRotationX()
    {
        var horizontal = inputManager.GetLook().x * playerConfig.PlayerMovementConfig.MouseSensitivityX * Time.fixedDeltaTime;
        targetRotationX *= Quaternion.AngleAxis(horizontal, Vector3.up);
        Quaternion yRotation = Quaternion.Lerp(rigidbody.rotation, targetRotationX, playerConfig.PlayerMovementConfig.SmoothTimeX);
        rigidbody.MoveRotation(yRotation);
    }

    private void PlayerMovement()
    {
        if (Mathf.Approximately(inputManager.GetMovement().magnitude, 0))
        {
            return;
        }
        
        var speed = inputManager.GetSprint() > 0.1f
            ? playerConfig.PlayerMovementConfig.SprintSpeed : playerConfig.PlayerMovementConfig.Speed;

        Vector3 moveAxis = speed * Time.fixedDeltaTime * (gameObject.transform.forward * inputManager.GetMovement().y
                                                          + gameObject.transform.right * inputManager.GetMovement().x);
        rigidbody.MovePosition(gameObject.transform.position + moveAxis);
    }

    private void PlayerJump()
    {
        if (grounded && inputManager.GetJump())
        {
            rigidbody.AddForce(Vector3.up * playerConfig.PlayerMovementConfig.JumpForce, ForceMode.Impulse);
        }
    }
    #endregion
    
    #region === Interaction ===

    private void InteractionCooldown()
    {
        if (!canInteract)
        {
            interactTimer -= Time.deltaTime;
            if (interactTimer <= 0)
            {
                canInteract = true;
            }
        }
    }
    
    private void TryDeform()
    {
        if (!inputManager.GetInteractPerformed() || currentEnergy <= 0 || !canInteract)
        {
            return;
        }

        RaycastHit hit;
        
        if (Physics.Raycast(cameraPivot.transform.position, cameraPivot.transform.forward, out hit, 
                playerConfig.PlayerInteractionConfig.InteractionRange, playerConfig.PlayerInteractionConfig.InteractionLayer))
        {
            var deformer = hit.transform.GetComponent<IMeshDeformer>();
            if (deformer != null)
            {
                deformer.Deform(hit.point);
                currentEnergy -= playerConfig.PlayerInteractionConfig.EnergyDepletion;
                OnDeform?.Invoke(playerConfig.PlayerInteractionConfig.EnergyDepletion);
                canInteract = false;
                interactTimer = playerConfig.PlayerInteractionConfig.InteractionCooldown;
            }
        }
    }
    
    #endregion
}

public interface IPlayerController
{
    event Action<int> OnDeform;
    public PlayerConfig PlayerConfig { get; }
}