using UnityEngine;

public class InteractableController : MonoBehaviour
{
    [SerializeField] private InteractableType interactableType;

    public InteractableType Type => interactableType;
}

public enum InteractableType
{
    Recharge, Sell, CollectableStone
}
