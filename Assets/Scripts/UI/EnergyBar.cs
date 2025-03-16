using UnityEngine;

public class EnergyBar : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    
    private RectTransform rectTransform;
    private float originalHeight;
    private int originalEnergy;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalHeight = rectTransform.sizeDelta.y;
        originalEnergy = playerController.PlayerConfig.PlayerInteractionConfig.BaseEnergy;
        
        playerController.OnDeform += EnergyDepletionHandler;
        playerController.OnEnergyRestored += EnergyRestorationHandler;
    }

    private void EnergyDepletionHandler(int p_depletion)
    {
        var depletionValue = originalHeight * p_depletion / originalEnergy;
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y - depletionValue);
    }

    private void EnergyRestorationHandler()
    {
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, originalHeight);
    }

    private void OnDestroy()
    {
        playerController.OnDeform -= EnergyDepletionHandler;
        playerController.OnEnergyRestored -= EnergyRestorationHandler;
    }
}
