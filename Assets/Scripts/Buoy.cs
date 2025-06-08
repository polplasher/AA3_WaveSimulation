using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Buoy : MonoBehaviour
{
    private const float WaterDensity = 997f; // Water density in kg/m³

    public WaveSystem waveSystem;

    [Header("Physical Properties")] [SerializeField]
    private float volume = 1.0f; // Total buoy volume

    [SerializeField] private float damping = 0.1f; // Damping
    [SerializeField] private float angularDamping = 0.1f; // Damping for rotational motion
    [SerializeField] private float waterLevelOffset; // Fine-tune water level detection

    [Header("Buoyancy Points")] [SerializeField]
    private Vector3[] buoyancyPoints; // Points where buoyancy forces are applied

    [SerializeField] private float pointRadius = 0.2f; // Radius of each buoyancy point
    [SerializeField] private bool visualizePoints = true; // Debug visualization
    [SerializeField] private Color pointColor = Color.red; // Debug point color

    private Rigidbody rb;
    private float pointVolume; // Volume per buoyancy point

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Calculate volume per point
        pointVolume = volume / buoyancyPoints.Length;
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

    private void ApplyBuoyancyAtPoint(Vector3 point, float displacedVolume, Vector3 normal)
    {
        if (displacedVolume <= 0)
            return;

        // Buoyancy force F = ρgV
        float buoyancyForce = WaterDensity * Mathf.Abs(Physics.gravity.y) * displacedVolume;

        // Apply force at the specific point
        rb.AddForceAtPosition(normal * buoyancyForce, point, ForceMode.Force);
    }

    private void OnDrawGizmosSelected()
    {
        if (!visualizePoints || buoyancyPoints == null)
            return;

        Gizmos.color = pointColor;

        foreach (Vector3 localPoint in buoyancyPoints)
        {
            Vector3 worldPoint = transform.TransformPoint(localPoint);
            Gizmos.DrawSphere(worldPoint, pointRadius);

            // If in play mode, visualize underwater status
            if (Application.isPlaying && waveSystem)
            {
                float submergence = CalculateSubmergence(worldPoint);
                if (submergence > 0)
                {
                    // Draw line showing water level
                    float waterHeight = waveSystem.GetWaterHeightAt(worldPoint) + waterLevelOffset;
                    Vector3 waterPos = new(worldPoint.x, waterHeight, worldPoint.z);
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(worldPoint, waterPos);
                    Gizmos.color = pointColor;
                }
            }
        }
    }
}