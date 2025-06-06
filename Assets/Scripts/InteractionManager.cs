using UnityEngine;
using TMPro;


public class InteractionManager : MonoBehaviour
{
    [Header("Wave Systems")]
    [SerializeField] private GameObject sinusoidalWavesObject;
    [SerializeField] private GameObject gerstnerWavesObject;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI simulation;

    private void Start()
    {
        sinusoidalWavesObject.SetActive(true);
        gerstnerWavesObject.SetActive(false);
        simulation.text = "Using Sinusoidal Waves";
    }

    public void ToggleWaves()
    {
        bool usingSinusoidal = sinusoidalWavesObject.activeSelf;

        sinusoidalWavesObject.SetActive(!usingSinusoidal);
        gerstnerWavesObject.SetActive(usingSinusoidal);

        simulation.text = usingSinusoidal ? "Using Gerstner Waves" : "Using Sinusoidal Waves";
    }
}
