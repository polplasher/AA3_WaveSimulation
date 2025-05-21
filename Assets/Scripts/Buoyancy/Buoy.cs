using UnityEngine;

namespace Buoyancy
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Buoy : MonoBehaviour
    {
        [Header("Physical Properties")]
        public float density = 997f; // Water density in kg/m³
        public float volume = 1.0f; // Buoy volume
        public float damping = 0.1f; // Damping

        protected Rigidbody rb;
        protected float gravity;

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody>();
            gravity = Physics.gravity.y;
        }

        protected abstract float CalculateSubmergence(Vector3 position);
        protected abstract Vector3 GetWaterNormal(Vector3 position);

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