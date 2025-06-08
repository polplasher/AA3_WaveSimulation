using UnityEngine;

public abstract class WaveSystem : MonoBehaviour
{
    // Shader property IDs
    private static readonly int AmplitudeID = Shader.PropertyToID("_Amplitude"),
        WaveLengthID = Shader.PropertyToID("_WaveLength"),
        DirectionID = Shader.PropertyToID("_Direction"),
        SpeedID = Shader.PropertyToID("_Speed"),
        PhaseID = Shader.PropertyToID("_Phase");

    [Header("Wave Properties")] 
    [SerializeField] protected float amplitude = 1.0f;
    [SerializeField] protected float waveLength = 10.0f;
    [SerializeField] protected Vector2 direction = new(1.0f, 0.0f);
    [SerializeField] protected float speed = 1.0f;
    [SerializeField] protected float phase;
    private MeshRenderer waterMesh;
    private Material waveMaterial;

    protected virtual void Start()
    {
        waterMesh = GetComponent<MeshRenderer>();
        waveMaterial = waterMesh.sharedMaterial;

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
            waveMaterial = waterMesh.sharedMaterial;
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