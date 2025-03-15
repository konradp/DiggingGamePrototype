using UnityEngine;

public class ShovelController : MonoBehaviour
{
    [SerializeField] private GameObject playerRef;
    [SerializeField] private Animator animator;
    private IPlayerController playerController;

    private bool shoveTimer;
    private float timer = 0.0f;
    private float timerMax = 0.3f;

    private void Start()
    {
        playerController = playerRef.GetComponent<IPlayerController>();
        playerController.OnDeform += OnDeformHandler;
    }

    private void OnDeformHandler(int p_depletion)
    {
        animator.SetBool("Shove",true);
        shoveTimer = true;
    }

    private void Update()
    {
        if (!shoveTimer) return;
        
        timer += Time.deltaTime;
        if (timer >= timerMax)
        {
            animator.SetBool("Shove",false);
            shoveTimer = false;
            timer = 0.0f;
        }
    }

    private void OnDestroy()
    {
        playerController.OnDeform -= OnDeformHandler;
    }
}
