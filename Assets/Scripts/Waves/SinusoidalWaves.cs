using UnityEngine;

namespace Waves
{
    public class SinusoidalWaves : WaveSimulation
    {
        public override float GetWaterHeightAt(Vector3 position)
        {
            // Normalize direction
            Vector2 dir = direction.normalized;

            // Calculate x coordinate based on the direction
            float xCoord = position.x * dir.x + position.z * dir.y;

            // Apply formula: A sin(2π/L(x − vt) + φ)
            float k = 2.0f * Mathf.PI / waveLength;
            float f = k * (xCoord - speed * Time.time) + phase;
            float height = amplitude * Mathf.Sin(f);

            return transform.position.y + height;
        }

        public override Vector3 CalculateWaveNormal(Vector3 position)
        {
            const float offset = 0.1f;
        
            float heightRight = GetWaterHeightAt(position + new Vector3(offset, 0, 0));
            float heightLeft = GetWaterHeightAt(position + new Vector3(-offset, 0, 0));
            float heightForward = GetWaterHeightAt(position + new Vector3(0, 0, offset));
            float heightBack = GetWaterHeightAt(position + new Vector3(0, 0, -offset));
        
            float gradientX = (heightRight - heightLeft) / (2 * offset);
            float gradientZ = (heightForward - heightBack) / (2 * offset);
        
            return new Vector3(-gradientX, 1, -gradientZ).normalized;
        }
    }
}