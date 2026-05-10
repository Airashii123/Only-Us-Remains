using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
#if UNITY_2020_1_OR_NEWER
using UnityEngine.XR;
#endif

namespace OnlyUsRemains.XR
{
    /// <summary>
    /// Manages XR controller input for VR devices.
    /// Left controller: Movement (thumbstick)
    /// Right controller: Interactions (buttons)
    /// </summary>
    public class XRInputManager : MonoBehaviour
    {
        [Header("Movement Input")]
        public Vector2 LeftThumbstickValue { get; private set; } = Vector2.zero;

        [Header("Right Controller Input")]
        public bool TriggerPressed { get; private set; } = false;
        public bool GripPressed { get; private set; } = false;
        public bool MenuButtonPressed { get; private set; } = false;
        public bool PrimaryButtonPressed { get; private set; } = false;
        public bool SecondaryButtonPressed { get; private set; } = false;

        private XRController leftController;
        private XRController rightController;

        private float triggerDeadzone = 0.1f;
        private float gripDeadzone = 0.1f;

        private static XRInputManager instance;

        public static XRInputManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<XRInputManager>();
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        private void Start()
        {
            InitializeControllers();
        }

        private void InitializeControllers()
        {
            // Try to find XR controller devices
            var leftDevices = InputSystem.devices.FindAll(d => d is XRController && d.displayName.Contains("Left"));
            var rightDevices = InputSystem.devices.FindAll(d => d is XRController && d.displayName.Contains("Right"));

            if (leftDevices.Count > 0)
            {
                leftController = (XRController)leftDevices[0];
                Debug.Log("Left XR Controller found: " + leftController.displayName);
            }

            if (rightDevices.Count > 0)
            {
                rightController = (XRController)rightDevices[0];
                Debug.Log("Right XR Controller found: " + rightController.displayName);
            }

            if (leftController == null || rightController == null)
            {
                Debug.LogWarning("XR Controllers not fully detected. This is normal if running without a headset. Controllers will still work when connected.");
            }
        }

        private void Update()
        {
            UpdateLeftControllerInput();
            UpdateRightControllerInput();
        }

        private void UpdateLeftControllerInput()
        {
            // Get thumbstick input for movement
            try
            {
                var gamepad = Gamepad.current;
                if (gamepad != null)
                {
                    LeftThumbstickValue = gamepad.leftStick.ReadValue();
                }
                else
                {
                    LeftThumbstickValue = Vector2.zero;
                }
            }
            catch
            {
                LeftThumbstickValue = Vector2.zero;
            }
        }

        private void UpdateRightControllerInput()
        {
            try
            {
                var gamepad = Gamepad.current;
                if (gamepad != null)
                {
                    // Right Trigger (Shooting/Action)
                    float triggerValue = gamepad.rightTrigger.ReadValue();
                    TriggerPressed = triggerValue > triggerDeadzone;

                    // Right Grip (Pick up items)
                    float gripValue = gamepad.rightShoulder.ReadValue();
                    GripPressed = gripValue > gripDeadzone;

                    // Button South (A on Xbox, X on PlayStation) - Flashlight toggle
                    PrimaryButtonPressed = gamepad.buttonSouth.wasPressedThisFrame;

                    // Button East (B on Xbox, Circle on PlayStation) - Menu/secondary action
                    SecondaryButtonPressed = gamepad.buttonEast.wasPressedThisFrame;

                    // Menu button
                    MenuButtonPressed = gamepad.startButton.wasPressedThisFrame;
                }
            }
            catch
            {
                TriggerPressed = false;
                GripPressed = false;
                PrimaryButtonPressed = false;
                SecondaryButtonPressed = false;
                MenuButtonPressed = false;
            }
        }

        /// <summary>
        /// Check if XR devices are active
        /// </summary>
        public bool IsXRActive()
        {
#if UNITY_2020_1_OR_NEWER
            return XRSettings.isDeviceActive;
#else
            return false;
#endif
        }

        /// <summary>
        /// Get movement input from left controller
        /// </summary>
        public Vector2 GetMovementInput()
        {
            return LeftThumbstickValue;
        }

        /// <summary>
        /// Check if should shoot (right trigger)
        /// </summary>
        public bool IsShooting()
        {
            return TriggerPressed;
        }

        /// <summary>
        /// Check if should pick up item (right grip)
        /// </summary>
        public bool IsPickingUp()
        {
            return GripPressed;
        }

        /// <summary>
        /// Check if flashlight toggle requested (primary button)
        /// </summary>
        public bool IsFlashlightToggle()
        {
            return PrimaryButtonPressed;
        }

        private void OnEnable()
        {
            if (instance == null)
                instance = this;
        }

        private void OnDisable()
        {
            if (instance == this)
                instance = null;
        }
    }
}
