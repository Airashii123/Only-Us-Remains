using UnityEngine;
using StarterAssets;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private string walkingBoolName = "Iswalking";

    private StarterAssetsInputs _input;

    private void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        bool isWalking = _input.move != Vector2.zero;
        animator.SetBool(walkingBoolName, isWalking);
    }
}