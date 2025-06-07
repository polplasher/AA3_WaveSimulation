using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    [Header("Wave Systems")]
    [SerializeField] private GameObject sinusoidalWavesObject;
    [SerializeField] private GameObject gerstnerWavesObject;

    private void Start()
    {
        sinusoidalWavesObject.SetActive(true);
        gerstnerWavesObject.SetActive(false);
    }

    public void ToggleWaves()
    {
        bool usingSinusoidal = sinusoidalWavesObject.activeSelf;

        sinusoidalWavesObject.SetActive(!usingSinusoidal);
        gerstnerWavesObject.SetActive(usingSinusoidal);
    }
}
