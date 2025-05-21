using UnityEngine;

namespace Waves
{
    public class GerstnerWaves : WaveSimulation
    {
        public override float GetWaterHeightAt(Vector3 position)
        {
            // Normalize direction
            Vector2 dir = direction.normalized;

            // Calculate wave parameters
            float k = 2.0f * Mathf.PI / waveLength;
            float f = k * (dir.x * position.x + dir.y * position.z - speed * Time.time) + phase;

            // Calculate vertical displacement
            float displacement = amplitude * Mathf.Sin(f);

            return transform.position.y + displacement;
        }

        public override Vector3 CalculateWaveNormal(Vector3 position)
        {
            Vector2 dir = direction.normalized;
            float k = 2.0f * Mathf.PI / waveLength;
            float f = k * (dir.x * position.x + dir.y * position.z - speed * Time.time) + phase;

            // Partial derivatives for normal
            float tanX = -dir.x * k * amplitude * Mathf.Cos(f);
            float tanZ = -dir.y * k * amplitude * Mathf.Cos(f);

            return new Vector3(-tanX, 1, -tanZ).normalized;
        }
    }
}