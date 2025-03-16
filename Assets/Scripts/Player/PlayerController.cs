using System;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerController
{
    [SerializeField] private PlayerConfig playerConfig;
    [SerializeField] private GameObject coreRef;
    [SerializeField] private GameObject cameraPivot;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI stoneText;
    [SerializeField] private TextMeshProUGUI moneyText;
    
    private IInputManager inputManager;
    private Rigidbody rb;
    
    private Quaternion targetRotationX;
    private bool grounded;
    private bool canInteract;
    private float interactTimer;
    
    private int currentEnergy;
    private int stoneAmt;
    private int moneyAmt;
    
    public PlayerConfig PlayerConfig => playerConfig;
    public event Action<int> OnDeform;
    public event Action OnEnergyRestored;

    private void Start()
    {
        inputManager = coreRef.GetComponent<IInputManager>();
        rb = GetComponent<Rigidbody>();
        targetRotationX = rb.rotation;
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
        TryInteract();
        TrySetStatusText();
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
        Quaternion yRotation = Quaternion.Lerp(rb.rotation, targetRotationX, playerConfig.PlayerMovementConfig.SmoothTimeX);
        rb.MoveRotation(yRotation);
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
        rb.MovePosition(gameObject.transform.position + moveAxis);
    }

    private void PlayerJump()
    {
        if (grounded && inputManager.GetJump())
        {
            rb.AddForce(Vector3.up * playerConfig.PlayerMovementConfig.JumpForce, ForceMode.Impulse);
        }
    }
    
    #endregion
    
    #region === Interaction ===

    private void InteractionCooldown()
    {
        if (canInteract)
        {
            return;
        }
        
        interactTimer -= Time.deltaTime;
        if (interactTimer <= 0)
        {
            canInteract = true;
        }
    }
    
    private void TryDeform()
    {
        if (!inputManager.GetAttackPerformed() || currentEnergy <= 0 || !canInteract)
        {
            return;
        }

        RaycastHit hit;
        
        if (Physics.Raycast(cameraPivot.transform.position, cameraPivot.transform.forward, out hit, 
                playerConfig.PlayerInteractionConfig.InteractionRange, playerConfig.PlayerInteractionConfig.DeformablesLayer))
        {
            var deformer = hit.transform.GetComponent<IMeshDeformer>();
            if (deformer != null)
            {
                deformer.Deform(hit.point);
                currentEnergy -= playerConfig.PlayerInteractionConfig.EnergyDepletion;
                canInteract = false;
                interactTimer = playerConfig.PlayerInteractionConfig.InteractionCooldown;
                OnDeform?.Invoke(playerConfig.PlayerInteractionConfig.EnergyDepletion);
            }
        }
    }

    private void TryInteract()
    {
        if (!inputManager.GetInteractPerformed())
        {
            return;
        }
        
        RaycastHit hit;

        if (Physics.Raycast(cameraPivot.transform.position, cameraPivot.transform.forward, out hit,
                playerConfig.PlayerInteractionConfig.InteractionRange, playerConfig.PlayerInteractionConfig.InteractionLayer))
        {
            var controller = hit.transform.GetComponent<InteractableController>();
            if (controller!= null)
            {
                switch (controller.Type)
                {
                    case InteractableType.Recharge:
                        if (moneyAmt <= 0)
                        {
                            break;
                        }
                        currentEnergy = playerConfig.PlayerInteractionConfig.BaseEnergy;
                        moneyAmt--;
                        SetMoneyText();
                        OnEnergyRestored?.Invoke();
                        break;
                    case InteractableType.Sell:
                        if (stoneAmt == 0)
                        {
                            break;
                        }
                        moneyAmt += stoneAmt;
                        stoneAmt = 0;
                        SetStoneText();
                        SetMoneyText();
                        break;
                    case InteractableType.CollectableStone:
                        stoneAmt++;
                        Destroy(hit.transform.gameObject);
                        SetStoneText();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    private void TrySetStatusText()
    {
        RaycastHit hit;

        if (Physics.Raycast(cameraPivot.transform.position, cameraPivot.transform.forward, out hit,
                playerConfig.PlayerInteractionConfig.InteractionRange,
                playerConfig.PlayerInteractionConfig.InteractionLayer))
        {
            var controller = hit.transform.GetComponent<InteractableController>();
            if (controller != null)
            {
                switch (controller.Type)
                {
                    case InteractableType.Recharge:
                        statusText.text = "Press E to recharge for $1";
                        break;
                    case InteractableType.Sell:
                        statusText.text = "Press E to sell";
                        break;
                    case InteractableType.CollectableStone:
                        statusText.text = "Press E to grab stone";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        else
        {
            statusText.text = "";
        }
    }

    private void SetStoneText()
    {
        stoneText.text = $"Stone: {stoneAmt}";
    }

    private void SetMoneyText()
    {
        moneyText.text = $"Money: {moneyAmt}$";
    }
    
    #endregion
}

public interface IPlayerController
{
    event Action<int> OnDeform;
    event Action OnEnergyRestored;
    public PlayerConfig PlayerConfig { get; }
}