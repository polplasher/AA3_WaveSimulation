using UnityEngine;
using Waves;

namespace Buoyancy
{
    [RequireComponent(typeof(Rigidbody))]
    public class Buoy : MonoBehaviour
    {
        [Header("Physical Properties")]
        [SerializeField] private float density = 997f; // Water density in kg/m³
        [SerializeField] private float volume = 1.0f; // Total buoy volume
        [SerializeField] private float damping = 0.1f; // Damping
        [SerializeField] private WaveSystem waveSystem;

        [Header("Buoyancy Points")]
        [SerializeField] private Vector3[] buoyancyPoints; // Points where buoyancy forces are applied
        [SerializeField] private float pointRadius = 0.2f; // Radius of each buoyancy point
        [SerializeField] private bool visualizePoints = true; // Debug visualization
        [SerializeField] private Color pointColor = Color.red; // Debug point color

        [Header("Advanced Settings")]
        [SerializeField] private float angularDamping = 0.1f; // Damping for rotational motion
        [SerializeField] private float waterLevelOffset = 0f; // Fine-tune water level detection

        protected Rigidbody rb;
        protected float gravity;
        protected float pointVolume; // Volume per buoyancy point

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody>();
            gravity = Physics.gravity.y;

            // If no buoyancy points are defined, create a default set
            if (buoyancyPoints == null || buoyancyPoints.Length == 0)
            {
                GenerateDefaultBuoyancyPoints();
            }

            // Calculate volume per point
            pointVolume = volume / buoyancyPoints.Length;
        }

        private void GenerateDefaultBuoyancyPoints()
        {
            // Create a simple cube-like arrangement of 8 points
            Vector3 extents = transform.localScale * 0.5f;
            buoyancyPoints = new Vector3[8];

            buoyancyPoints[0] = new Vector3(-extents.x, -extents.y, -extents.z);
            buoyancyPoints[1] = new Vector3(extents.x, -extents.y, -extents.z);
            buoyancyPoints[2] = new Vector3(-extents.x, -extents.y, extents.z);
            buoyancyPoints[3] = new Vector3(extents.x, -extents.y, extents.z);
            buoyancyPoints[4] = new Vector3(-extents.x, extents.y, -extents.z);
            buoyancyPoints[5] = new Vector3(extents.x, extents.y, -extents.z);
            buoyancyPoints[6] = new Vector3(-extents.x, extents.y, extents.z);
            buoyancyPoints[7] = new Vector3(extents.x, extents.y, extents.z);
        }

        private void FixedUpdate()
        {
            // Apply buoyancy at each point
            foreach (Vector3 localPoint in buoyancyPoints)
            {
                // Convert local point to world position
                Vector3 worldPoint = transform.TransformPoint(localPoint);

                // Calculate submergence at this point
                float submergence = CalculateSubmergence(worldPoint);

                // If point is underwater, apply buoyancy
                if (submergence > 0)
                {
                    // Calculate displaced volume for this point
                    float displacedVolume = pointVolume * Mathf.Clamp01(submergence / (pointRadius * 2));

                    // Get water normal at this point
                    Vector3 waterNormal = waveSystem.CalculateWaveNormal(worldPoint);

                    // Apply buoyancy force at this specific point
                    ApplyBuoyancyAtPoint(worldPoint, displacedVolume, waterNormal);
                }
            }

            // Apply damping to both linear and angular velocity
            rb.AddForce(-rb.velocity * damping, ForceMode.Force);
            rb.AddTorque(-rb.angularVelocity * angularDamping, ForceMode.Force);
        }

        private float CalculateSubmergence(Vector3 position)
        {
            float waterHeight = waveSystem.GetWaterHeightAt(position) + waterLevelOffset;
            float submergence = waterHeight - position.y;
            return submergence;
        }

        protected void ApplyBuoyancyAtPoint(Vector3 point, float displacedVolume, Vector3 normal)
        {
            if (displacedVolume <= 0)
                return;

            // Buoyancy force F = ρgV
            float buoyancyForce = density * Mathf.Abs(gravity) * displacedVolume;

            // Apply force at the specific point
            rb.AddForceAtPosition(normal * buoyancyForce, point, ForceMode.Force);
        }

        private void OnDrawGizmos()
        {
            if (!visualizePoints || buoyancyPoints == null)
                return;

            Gizmos.color = pointColor;

            foreach (Vector3 localPoint in buoyancyPoints)
            {
                Vector3 worldPoint = transform.TransformPoint(localPoint);
                Gizmos.DrawSphere(worldPoint, pointRadius);

                // If in play mode, visualize underwater status
                if (Application.isPlaying && waveSystem != null)
                {
                    float submergence = CalculateSubmergence(worldPoint);
                    if (submergence > 0)
                    {
                        // Draw line showing water level
                        float waterHeight = waveSystem.GetWaterHeightAt(worldPoint) + waterLevelOffset;
                        Vector3 waterPos = new Vector3(worldPoint.x, waterHeight, worldPoint.z);
                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(worldPoint, waterPos);
                        Gizmos.color = pointColor;
                    }
                }
            }
        }
    }
}
