using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_2020_1_OR_NEWER
using UnityEngine.XR;
#endif
using System.Collections.Generic;

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
            // Controllers will be found automatically through gamepad
            Debug.Log("XR Input Manager initialized");
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
            try
            {
                return XRSettings.isDeviceActive;
            }
            catch
            {
                return false;
            }
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
