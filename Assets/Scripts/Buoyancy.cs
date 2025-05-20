using UnityEngine;

public class Buoyancy : MonoBehaviour
{
    [Header("Referencias")]
    public WavesManager wavesManager;

    [Header("Propiedades Físicas")]
    public float density = 997f; // Densidad del agua en kg/m³
    public float volume = 1.0f; // Volumen total de la boya en m³
    public float damping = 0.1f; // Amortiguación del movimiento
    public float angularDamping = 0.05f; // Amortiguación de rotación

    [Header("Puntos de Muestreo")]
    public int samplePoints = 8; // Número de puntos para muestrear la flotabilidad
    private Vector3[] sampleOffsets; // Posiciones relativas de los puntos de muestreo

    private Rigidbody rb;
    private float gravity;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (!rb)
        {
            Debug.LogError("La boya necesita un componente Rigidbody");
            enabled = false;
            return;
        }

        gravity = Physics.gravity.y;
        InitializeSamplePoints();
    }

    private void InitializeSamplePoints()
    {
        // Crear puntos de muestreo distribuidos en la boya
        sampleOffsets = new Vector3[samplePoints];
        
        // Distribuir puntos en forma de cubo
        float radius = transform.localScale.x * 0.5f;
        
        // Ejemplo para 8 puntos (las esquinas de un cubo)
        if (samplePoints >= 8)
        {
            sampleOffsets[0] = new Vector3(-radius, -radius, -radius);
            sampleOffsets[1] = new Vector3(radius, -radius, -radius);
            sampleOffsets[2] = new Vector3(-radius, -radius, radius);
            sampleOffsets[3] = new Vector3(radius, -radius, radius);
            sampleOffsets[4] = new Vector3(-radius, radius, -radius);
            sampleOffsets[5] = new Vector3(radius, radius, -radius);
            sampleOffsets[6] = new Vector3(-radius, radius, radius);
            sampleOffsets[7] = new Vector3(radius, radius, radius);
        }
        
        // Si se necesitan más puntos, se pueden agregar aquí
    }

    private void FixedUpdate()
    {
        // Volumen por punto de muestreo
        float volumePerPoint = volume / samplePoints;
        float totalDisplacedVolume = 0f;
        
        // Para calcular la normal promedio
        Vector3 normalSum = Vector3.zero;
        int submergedPoints = 0;

        // Comprobar cada punto de muestreo
        for (int i = 0; i < samplePoints; i++)
        {
            // Posición del punto de muestreo en el mundo
            Vector3 samplePos = transform.TransformPoint(sampleOffsets[i]);
            
            // Altura del agua en esa posición
            float waterHeight = wavesManager.GetWaterHeightAt(samplePos);
            
            // Profundidad de inmersión
            float submergedDepth = waterHeight - samplePos.y;
            
            if (submergedDepth > 0)
            {
                // Este punto está sumergido
                totalDisplacedVolume += volumePerPoint;
                submergedPoints++;
                
                // Calcular la normal de la ola en este punto para alineación
                Vector3 normal = CalculateWaveNormal(samplePos);
                normalSum += normal;
            }
        }
        
        if (totalDisplacedVolume > 0)
        {
            // Calcular la fuerza de flotabilidad F = ρgV
            float buoyancyForce = density * Mathf.Abs(gravity) * totalDisplacedVolume;
            
            // Aplicar la fuerza en dirección opuesta a la gravedad
            rb.AddForce(Vector3.up * buoyancyForce, ForceMode.Force);
            
            // Aplicar amortiguación para simular resistencia del agua
            rb.AddForce(-rb.velocity * damping, ForceMode.Force);
            rb.AddTorque(-rb.angularVelocity * angularDamping, ForceMode.Force);
            
            // Alinear la boya con la normal promedio de la ola
            if (submergedPoints > 0)
            {
                Vector3 averageNormal = normalSum / submergedPoints;
                AlignWithNormal(averageNormal);
            }
        }
    }

    private Vector3 CalculateWaveNormal(Vector3 position)
    {
        // Offset pequeño para calcular pendientes
        float offset = 0.1f;
        
        // Muestrear la altura del agua en puntos cercanos
        float heightRight = wavesManager.GetWaterHeightAt(position + new Vector3(offset, 0, 0));
        float heightLeft = wavesManager.GetWaterHeightAt(position + new Vector3(-offset, 0, 0));
        float heightForward = wavesManager.GetWaterHeightAt(position + new Vector3(0, 0, offset));
        float heightBack = wavesManager.GetWaterHeightAt(position + new Vector3(0, 0, -offset));
        
        // Calcular gradiente (derivada parcial) en X y Z
        float gradientX = (heightRight - heightLeft) / (2 * offset);
        float gradientZ = (heightForward - heightBack) / (2 * offset);
        
        // La normal es perpendicular al gradiente
        Vector3 normal = new Vector3(-gradientX, 1, -gradientZ).normalized;
        return normal;
    }

    private void AlignWithNormal(Vector3 normal)
    {
        // Aplicar rotación suave hacia la normal
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, normal);
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 2.0f);
    }
}