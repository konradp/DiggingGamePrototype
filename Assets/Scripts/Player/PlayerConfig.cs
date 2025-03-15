using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Create PlayerConfig", fileName = "PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    public PlayerMovementConfig PlayerMovementConfig;
    public PlayerInteractionConfig PlayerInteractionConfig;
}

[Serializable]
public struct PlayerMovementConfig
{
    [SerializeField] private float speed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float mouseSensitivityX;
    [SerializeField] private float mouseSensitivityY;
    [SerializeField] private float smoothTimeX;
    [SerializeField] private float jumpForce;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;

    public float Speed => speed;
    public float SprintSpeed => sprintSpeed;
    public float MouseSensitivityX => mouseSensitivityX;
    public float MouseSensitivityY => mouseSensitivityY;
    public float SmoothTimeX => smoothTimeX;
    public float JumpForce => jumpForce;
    public float MinAngle => minAngle;
    public float MaxAngle => maxAngle;
}

[Serializable]
public struct PlayerInteractionConfig
{
    [SerializeField] private float interactionRange;
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private int baseEnergy;
    [SerializeField] private int energyDepletion;
    [SerializeField] private float interactionCooldown;

    public float InteractionRange => interactionRange;

    public LayerMask InteractionLayer => interactionLayer;

    public int BaseEnergy => baseEnergy;

    public int EnergyDepletion => energyDepletion;

    public float InteractionCooldown => interactionCooldown;
}