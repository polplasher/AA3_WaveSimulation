using UnityEngine;
using Waves;

namespace Buoyancy
{
    [RequireComponent(typeof(Rigidbody))]
    public class Buoy : MonoBehaviour
    {
        [Header("Physical Properties")]
        [SerializeField] private float density = 997f; // Water density in kg/m³
        [SerializeField] private float volume = 1.0f; // Buoy volume
        [SerializeField] private float damping = 0.1f; // Damping
        [SerializeField] private WaveSystem waveSystem;

        protected Rigidbody rb;
        protected float gravity;

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody>();
            gravity = Physics.gravity.y;
        }

        private void FixedUpdate()
        {
            // Simplified to a single point
            Vector3 position = transform.position;

            // Calculate displaced volume
            float submergence = CalculateSubmergence(position);
            float displacedVolume = volume * Mathf.Clamp01(submergence / transform.localScale.y);

            // Get normal and apply buoyancy
            Vector3 waterNormal = waveSystem.CalculateWaveNormal(position);
            ApplyBuoyancy(displacedVolume, waterNormal);
        }

        private float CalculateSubmergence(Vector3 position)
        {
            float waterHeight = waveSystem.GetWaterHeightAt(position);
            float submergence = waterHeight - (position.y - transform.localScale.y * 0.5f);
            return Mathf.Max(0, submergence);
        }

        protected void ApplyBuoyancy(float displacedVolume, Vector3 normal)
        {
            if (displacedVolume <= 0)
                return;

            // Buoyancy force F = ρgV
            float buoyancyForce = density * Mathf.Abs(gravity) * displacedVolume;

            // Apply force in normal direction
            rb.AddForce(normal * buoyancyForce, ForceMode.Force);

            // Damping
            rb.AddForce(-rb.velocity * damping, ForceMode.Force);
        }
    }
}