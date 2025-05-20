using UnityEngine;

public class WavesManager : MonoBehaviour
{
    [Header("Wave Properties")]
    public Material waveMaterial;
    public float amplitude = 1.0f;
    public float waveLength = 10.0f;
    public Vector2 direction = new(1.0f, 0.0f);
    public float speed = 1.0f;
    public float phase;

    [Header("References")]
    public MeshRenderer waterSurface;

    private void Start()
    {
        if (waterSurface)
            waterSurface.material = waveMaterial;

        UpdateShaderParameters();
    }

    private void Update()
    {
        UpdateShaderParameters();
    }

    private void UpdateShaderParameters()
    {
        // Update the shader parameters with current values
        waveMaterial.SetFloat("_Amplitude", amplitude);
        waveMaterial.SetFloat("_WaveLength", waveLength);
        waveMaterial.SetVector("_Direction", new Vector4(direction.x, direction.y, 0, 0));
        waveMaterial.SetFloat("_Speed", speed);
        waveMaterial.SetFloat("_Phase", phase);
    }

    // Method to calculate water height at any position, useful for buoyancy
    public float GetWaterHeightAt(Vector3 position)
    {
        // Get position
        float x = position.x;
        float z = position.z;

        // Normalize direction
        Vector2 dir = direction.normalized;

        // Calculate x coordinate based on direction
        float xCoord = x * dir.x + z * dir.y;

        // Apply formula: A sin(2π/L(x − vt) + φ)
        float k = 2.0f * Mathf.PI / waveLength;
        float f = k * (xCoord - speed * Time.time) + phase;
        float height = amplitude * Mathf.Sin(f);

        // Add base water height
        return transform.position.y + height;
    }
}