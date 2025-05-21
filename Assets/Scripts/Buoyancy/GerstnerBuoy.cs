using UnityEngine;
using Waves;

namespace Buoyancy
{
    public class GerstnerBuoy : Buoy
    {
        public GerstnerWaves waveSimulation;

        private void FixedUpdate()
        {
            // Simplified to a single point
            Vector3 position = transform.position;
        
            // Calculate displaced volume
            float submergence = CalculateSubmergence(position);
            float displacedVolume = volume * Mathf.Clamp01(submergence / transform.localScale.y);
        
            // Get normal and apply buoyancy
            Vector3 waterNormal = GetWaterNormal(position);
            ApplyBuoyancy(displacedVolume, waterNormal);
        }

        protected override float CalculateSubmergence(Vector3 position)
        {
            float waterHeight = waveSimulation.GetWaterHeightAt(position);
            float submergence = waterHeight - (position.y - transform.localScale.y * 0.5f);
            return Mathf.Max(0, submergence);
        }

        protected override Vector3 GetWaterNormal(Vector3 position)
        {
            return waveSimulation.CalculateWaveNormal(position);
        }
    }
}