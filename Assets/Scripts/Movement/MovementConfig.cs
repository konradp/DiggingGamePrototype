using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Create MovementConfig", fileName = "MovementConfig", order = 0)]
public class MovementConfig : ScriptableObject
{
    public PlayerMovementConfig playerMovementConfig;
}

[Serializable]
public struct PlayerMovementConfig
{
    [SerializeField] private float speed; //def 2f
    [SerializeField] private float sprintSpeed; //def 5f
    [SerializeField] private float mouseSensitivityX; // def 10f;
    [SerializeField] private float mouseSensitivityY; // def 10f;
    [SerializeField] private float smoothTimeX; // def 0.8f;
    [SerializeField] private float jumpForce; // def 150f;
    [SerializeField] private float minAngle; // def 85f; // Minimum angle 
    [SerializeField] private float maxAngle; // def 280f; // Maximum angle 

    public float Speed => speed;

    public float SprintSpeed => sprintSpeed;

    public float MouseSensitivityX => mouseSensitivityX;

    public float MouseSensitivityY => mouseSensitivityY;

    public float SmoothTimeX => smoothTimeX;

    public float JumpForce => jumpForce;

    public float MinAngle => minAngle;

    public float MaxAngle => maxAngle;
}