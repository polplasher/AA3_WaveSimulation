using UnityEngine;

namespace Waves
{
    public class GerstnerWaveSystem : WaveSystem
    {
        public override float GetWaterHeightAt(Vector3 position)
        {
            // Normalize direction
            Vector2 dir = direction.normalized;

            // Calculate wave parameters
            float k = 2.0f * Mathf.PI / waveLength; // Wave number (2π/L)

            // Estimate the horizontal displacement effect
            float xOffset = -amplitude * Mathf.Cos(k * (position.x * dir.x + position.z * dir.y) + phase);

            // Adjust position to account for horizontal displacement
            float adjustedX = position.x - xOffset * dir.x;
            float adjustedZ = position.z - xOffset * dir.y;

            // Calculate phase for the adjusted position
            float x = adjustedX * dir.x + adjustedZ * dir.y;
            float f = k * (x - speed * Time.time) + phase;

            // Now calculate the vertical displacement for this adjusted position
            float heightDisplacement = amplitude * Mathf.Sin(f);

            return transform.position.y + heightDisplacement;
        }

        public override Vector3 CalculateWaveNormal(Vector3 position)
        {
            Vector2 dir = direction.normalized;
            float k = 2.0f * Mathf.PI / waveLength;
            float f = k * (dir.x * position.x + dir.y * position.z - speed * Time.time) + phase;

            // Partial derivatives for normal - these match your shader calculation
            float tanX = -dir.x * k * amplitude * Mathf.Cos(f);
            float tanZ = -dir.y * k * amplitude * Mathf.Cos(f);

            return new Vector3(-tanX, 1, -tanZ).normalized;
        }
    }
}