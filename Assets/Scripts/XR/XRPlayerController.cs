using UnityEngine;
using StarterAssets;

namespace OnlyUsRemains.XR
{
    /// <summary>
    /// Integrates XR input with the existing FirstPersonController.
    /// Provides XR-compatible input handling while maintaining compatibility with keyboard/mouse.
    /// </summary>
    public class XRPlayerController : MonoBehaviour
    {
        private FirstPersonController playerController;
        private StarterAssetsInputs playerInput;
        private XRInputManager xrInputManager;

        [Header("XR Settings")]
        [SerializeField] private bool enableXRMode = true;
        [SerializeField] private float xrMovementSensitivity = 1.0f;
        [SerializeField] private float xrLookSensitivity = 0.5f;

        private bool xrEnabled = false;

        private void Start()
        {
            playerController = GetComponent<FirstPersonController>();
            playerInput = GetComponent<StarterAssetsInputs>();
            xrInputManager = XRInputManager.Instance;

            // Check if XR is available and enabled
            if (enableXRMode && xrInputManager != null)
            {
                xrEnabled = xrInputManager.IsXRActive();
                if (xrEnabled)
                {
                    Debug.Log("XR Player Controller initialized in XR mode");
                }
                else
                {
                    Debug.Log("XR not available - using standard mode");
                    xrEnabled = false;
                }
            }
            else
            {
                Debug.Log("XR Input Manager not found or disabled");
                xrEnabled = false;
            }
        }

        private void Update()
        {
            // Safety check for playerInput
            if (playerInput == null)
            {
                Debug.LogWarning("StarterAssetsInputs component not found!");
                return;
            }

            // Only process XR input if XR is enabled AND available
            if (xrEnabled && xrInputManager != null)
            {
                UpdateXRInput();
            }
            // Fall back to standard input if XR is not active
            else
            {
                playerInput.look = Vector2.zero;
            }
        }

        /// <summary>
        /// Update player input from XR controllers
        /// </summary>
        private void UpdateXRInput()
        {
            // Left controller - Movement
            Vector2 xrMovement = xrInputManager.GetMovementInput();
            playerInput.move = xrMovement * xrMovementSensitivity;

            // Right controller - Look (head tracking would go here)
            playerInput.look = Vector2.zero;

            // Right controller - Interactions
            // Trigger = Shoot
            if (xrInputManager.IsShooting())
            {
                // TODO: Call shooting system
                // Example: GetComponent<WeaponSystem>().Fire();
                Debug.Log("XR Trigger Pressed - Shooting");
            }

            // Grip = Pick up item
            if (xrInputManager.IsPickingUp())
            {
                // TODO: Call item pickup system
                // Example: GetComponent<ItemPickupSystem>().TryPickup();
                Debug.Log("XR Grip Pressed - Pick up item");
            }

            // Primary Button (A/X) = Flashlight toggle
            if (xrInputManager.IsFlashlightToggle())
            {
                // TODO: Call flashlight system
                // Example: GetComponent<FlashlightSystem>().Toggle();
                Debug.Log("XR Primary Button Pressed - Flashlight toggle");
            }
        }

        /// <summary>
        /// Enable XR input mode
        /// </summary>
        public void EnableXRMode()
        {
            xrEnabled = true;
            Debug.Log("XR mode enabled");
        }

        /// <summary>
        /// Disable XR input mode (fall back to keyboard/mouse)
        /// </summary>
        public void DisableXRMode()
        {
            xrEnabled = false;
            Debug.Log("XR mode disabled");
        }

        /// <summary>
        /// Check if currently using XR
        /// </summary>
        public bool IsXREnabled()
        {
            return xrEnabled;
        }
    }
}
