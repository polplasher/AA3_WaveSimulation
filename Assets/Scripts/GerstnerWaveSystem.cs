using UnityEngine;

namespace Waves
{
    public class GerstnerWaveSystem : WaveSystem
    {
        public override float GetWaterHeightAt(Vector3 position)
        {
            // 1. Normalize the wave direction in XZ plane
            Vector2 dir = direction.normalized;

            // 2. Calculate wave number k = 2π / wavelength
            float k = 2f * Mathf.PI / waveLength;

            // 3. Project position onto wave direction using dot product
            //    dot = dir.x * position.x + dir.y * position.z
            //    This gives the distance along the wave vector
            float dot = Vector2.Dot(dir, new Vector2(position.x, position.z));

            // 4. Compute phase term: k * (distance - speed * time) + phase
            float phaseTerm = k * (dot - speed * Time.time) + phase;

            // 5. Vertical displacement using cosine of the phase
            return transform.position.y + amplitude * Mathf.Cos(phaseTerm);
        }

        public override Vector3 CalculateWaveNormal(Vector3 position)
        {
            Vector2 dir = direction.normalized;
            float k = 2f * Mathf.PI / waveLength;
            float dot = Vector2.Dot(dir, new Vector2(position.x, position.z));
            float phaseTerm = k * (dot - speed * Time.time) + phase;

            // Height derivatives (y = A·cos(phase))
            float dYdX = dir.x * k * amplitude * Mathf.Sin(phaseTerm);
            float dYdZ = dir.y * k * amplitude * Mathf.Sin(phaseTerm);

            // Normal perpendicular to the tangent plane
            return new Vector3(-dYdX, 1f, -dYdZ).normalized;
        }
    }
}