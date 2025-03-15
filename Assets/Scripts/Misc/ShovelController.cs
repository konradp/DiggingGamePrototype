using System;
using UnityEngine;

public class ShovelController : MonoBehaviour
{
    [SerializeField] private GameObject coreRef;
    [SerializeField] private Animator animator;
    private IInputManager inputManager;

    private void Start()
    {
        inputManager = coreRef.GetComponent<IInputManager>();
    }

    private void Update()
    {
        if (inputManager == null)
        {
            Debug.LogError("No input manager. ShovelController won't work.");
            return;
        }
        
        if (inputManager.GetInteract() > 0.1f)
        {
            animator.SetBool("Shove",true);
        }
        else
        {
            animator.SetBool("Shove",false);
        }
        
    }
}
