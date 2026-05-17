using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;

public class PlayerSanityHealthSystem : MonoBehaviour
{
    [Header("References")]
    public SanityBar sanityBar;
    public HealthBar healthBar;

    [Header("Drain Settings")]
    public int sanityDrainWalk = 1;
    public int sanityDrainJump = 5;

    [Tooltip("Co ile sekund ma się drainować sanity podczas chodzenia")]
    public float walkDrainInterval = 0.5f;

    [Header("Death Settings")]
    public int deathSceneIndex = 2;

    private StarterAssetsInputs _input;
    private float _walkTimer;

    private void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        _walkTimer = walkDrainInterval;
    }

    private void Update()
    {
        HandleWalkDrain();
        HandleDeath();
    }

    private void HandleWalkDrain()
    {
        if (_input.move == Vector2.zero)
        {
            _walkTimer = walkDrainInterval;
            return;
        }

        _walkTimer -= Time.deltaTime;

        if (_walkTimer <= 0f)
        {
            DrainSanityOrHealth(sanityDrainWalk);
            _walkTimer = walkDrainInterval;
        }
    }

    public void OnJumpDrain()
    {
        DrainSanityOrHealth(sanityDrainJump);
    }

    private void DrainSanityOrHealth(int amount)
    {
        if (sanityBar.sanitySystem.getSanity() == 0)
        {
            healthBar.healthSystem.Damage(amount);
            healthBar.UpdateHealthBar(healthBar.healthSystem.GetHealthPercent());
        }
        else
        {
            sanityBar.sanitySystem.Damage(amount);
            sanityBar.UpdateSanityBar(sanityBar.sanitySystem.GetSanityPercent());
        }
    }

    private void HandleDeath()
    {
        if (healthBar.healthSystem.getHealth() <= 0)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(deathSceneIndex);
        }
    }
}