using UnityEngine;
#if UNITY_2020_1_OR_NEWER
using UnityEngine.XR;
#endif

namespace OnlyUsRemains.XR
{
    /// <summary>
    /// Manages VR camera setup and initialization.
    /// This script sets up the XR camera rig and handles switching between standard and VR cameras.
    /// </summary>
    public class XRCameraRig : MonoBehaviour
    {
        [SerializeField] private GameObject xrOrigin;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private bool autoInitializeXR = true;

        private bool xrActive = false;

        private static XRCameraRig instance;

        public static XRCameraRig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<XRCameraRig>();
                }
                return instance;
            }
        }

        public bool IsXRActive => xrActive;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void Start()
        {
            if (autoInitializeXR)
            {
                InitializeXR();
            }
        }

        /// <summary>
        /// Initialize XR support
        /// </summary>
        public void InitializeXR()
        {
#if UNITY_2020_1_OR_NEWER
            try
            {
                // Start XR subsystems
                var xrSettings = XRSettings.LoadedDeviceName;
                
                if (!string.IsNullOrEmpty(xrSettings) && xrSettings != "MockHMD")
                {
                    xrActive = true;
                    Debug.Log($"XR Initialized: {xrSettings}");

                    if (xrOrigin != null)
                    {
                        xrOrigin.SetActive(true);
                    }
                }
                else
                {
                    xrActive = false;
                    Debug.Log("XR not available. Running in standard mode.");
                    
                    if (xrOrigin != null)
                    {
                        xrOrigin.SetActive(false);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"XR initialization failed: {e.Message}. Continuing in standard mode.");
                xrActive = false;
            }
#else
            Debug.LogWarning("XR not supported in this Unity version");
            xrActive = false;
#endif
        }

        /// <summary>
        /// Get the main camera (either XR or standard)
        /// </summary>
        public Camera GetMainCamera()
        {
            if (xrActive && xrOrigin != null)
            {
                Camera xrCamera = xrOrigin.GetComponentInChildren<Camera>();
                if (xrCamera != null)
                    return xrCamera;
            }

            return mainCamera;
        }

        /// <summary>
        /// Get the XR origin transform
        /// </summary>
        public Transform GetXROriginTransform()
        {
            if (xrOrigin != null)
                return xrOrigin.transform;
            
            return null;
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
