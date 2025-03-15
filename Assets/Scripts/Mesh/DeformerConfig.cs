using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Create DeformerConfig", fileName = "DeformerConfig", order = 1)]
public class DeformerConfig : ScriptableObject
{
    [SerializeField] private int gridSize = 20;
    [SerializeField] private float cellSize = 0.5f;
    [SerializeField] private float deformationRadius = 1f;
    [SerializeField] private float deformationDepth = 0.3f;

    public int GridSize => gridSize;

    public float CellSize => cellSize;

    public float DeformationRadius => deformationRadius;

    public float DeformationDepth => deformationDepth;
}

