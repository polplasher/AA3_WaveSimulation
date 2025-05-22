using UnityEngine;

namespace Waves
{
    public abstract class WaveSystem : MonoBehaviour
    {
        // Shader property IDs
        private static readonly int AmplitudeID = Shader.PropertyToID("_Amplitude"),
            WaveLengthID = Shader.PropertyToID("_WaveLength"),
            DirectionID = Shader.PropertyToID("_Direction"),
            SpeedID = Shader.PropertyToID("_Speed"),
            PhaseID = Shader.PropertyToID("_Phase");

        [Header("Wave Properties")] public Material waveMaterial;
        public float amplitude = 1.0f;
        public float waveLength = 10.0f;
        public Vector2 direction = new(1.0f, 0.0f);
        public float speed = 1.0f;
        public float phase;
        private MeshRenderer waterMesh;

        protected virtual void Start()
        {
            waterMesh = GetComponent<MeshRenderer>();
            waveMaterial = waterMesh.material;

            UpdateShaderParameters();
        }

        private void Update()
        {
            UpdateShaderParameters();
        }

        private void OnValidate()
        {
            if (!waveMaterial)
            {
                waterMesh = GetComponent<MeshRenderer>();
                waveMaterial = waterMesh.material;
            }

            UpdateShaderParameters();
        }

        private void UpdateShaderParameters()
        {
            waveMaterial.SetFloat(AmplitudeID, amplitude);
            waveMaterial.SetFloat(WaveLengthID, waveLength);
            waveMaterial.SetVector(DirectionID, new Vector4(direction.x, direction.y, 0, 0));
            waveMaterial.SetFloat(SpeedID, speed);
            waveMaterial.SetFloat(PhaseID, phase);
        }

        // Method to calculate water height at any position
        public abstract float GetWaterHeightAt(Vector3 position);

        // Method to calculate wave normal at any position
        public abstract Vector3 CalculateWaveNormal(Vector3 position);
    }
}