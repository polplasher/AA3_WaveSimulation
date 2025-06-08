using Buoyancy;
using Cinemachine;
using UnityEngine;

namespace Utilities
{
    public class CameraController : Singleton<CameraController>
    {
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float minRadius = 5f;
        [SerializeField] private float maxRadius = 20f;
        
        private CinemachineFreeLook freeLookCamera;
        private Buoy[] buoys;
        private int currentBuoyIndex;
        private Transform Target => buoys[currentBuoyIndex].transform;

        private void Start()
        {
            freeLookCamera = GetComponent<CinemachineFreeLook>();
            buoys = FindObjectsOfType<Buoy>();
        }

        private void Update()
        {
            if (!Target)
                return;
            
            // Manage camera zoom with mouse scroll
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0f)
            {
                for (int i = 0; i < 3; i++)
                {
                    float newRadius = freeLookCamera.m_Orbits[i].m_Radius - scroll * zoomSpeed;
                    newRadius = Mathf.Clamp(newRadius, minRadius, maxRadius);
                    freeLookCamera.m_Orbits[i].m_Radius = newRadius;
                }
            }

            // Get mouse input for rotation
            if (Input.GetMouseButton(1)) // Left mouse button for rotation
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;

                freeLookCamera.m_XAxis.m_InputAxisName = "Mouse X";
                freeLookCamera.m_YAxis.m_InputAxisName = "Mouse Y";
            }
            else
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                // Deactivate input
                freeLookCamera.m_XAxis.m_InputAxisName = "";
                freeLookCamera.m_YAxis.m_InputAxisName = "";

                // Reset input values (clear accumulated inertia)
                freeLookCamera.m_XAxis.m_InputAxisValue = 0;
                freeLookCamera.m_YAxis.m_InputAxisValue = 0;
            }
        }

        public void SetBuoys(Buoy[] buoys)
        {
            this.buoys = buoys;
            currentBuoyIndex = 0;
        }
    }
}