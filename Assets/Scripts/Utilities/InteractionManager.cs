using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Utilities
{
    public class InteractionManager : MonoBehaviour
    {
        [Header("Wave Systems")] [SerializeField]
        private GameObject sinusoidalWavesObject;

        [SerializeField] private GameObject gerstnerWavesObject;

        [Header("UI Elements")] [SerializeField]
        private TextMeshProUGUI simulation;

        private bool usingSinusoidal = true;

        private void Start()
        {
            sinusoidalWavesObject.SetActive(true);
            gerstnerWavesObject.SetActive(false);
            simulation.text = "Using Sinusoidal Waves";
        }

        public void ToggleWaves()
        {
            // Toggle between sinusoidal and Gerstner wave systems
            usingSinusoidal = !usingSinusoidal;

            // Get the currently active wave system and the one to switch to
            GameObject previousActiveObject = usingSinusoidal ? gerstnerWavesObject : sinusoidalWavesObject;
            GameObject newActiveObject = usingSinusoidal ? sinusoidalWavesObject : gerstnerWavesObject;

            // Reparent all buoys to the new active wave system
            Buoy[] buoys = previousActiveObject.GetComponentsInChildren<Buoy>();
            WaveSystem newWaveSystem = newActiveObject.GetComponent<WaveSystem>();
            foreach (Buoy b in buoys)
            {
                b.transform.SetParent(newActiveObject.transform, true);
                b.transform.position += Vector3.up * 4; // Upward offset to avoid z-fighting
                b.waveSystem = newWaveSystem;
            }

            // Activate the appropriate wave system
            sinusoidalWavesObject.SetActive(usingSinusoidal);
            gerstnerWavesObject.SetActive(!usingSinusoidal);

            // Change the UI text
            simulation.text = usingSinusoidal ? "Using Sinusoidal Waves" : "Using Gerstner Waves";
        }

        public void ChangeTarget(int indexDelta)
        {
            CameraController cc = CameraController.Instance;
            int nextIndex = cc.CurrentBuoyIndex + indexDelta;

            // Wrap around the buoy index
            if (nextIndex < 0)
            {
                nextIndex = cc.Buoys.Length - 1;
            }
            else if (nextIndex >= cc.Buoys.Length)
            {
                nextIndex = 0;
            }

            cc.CurrentBuoyIndex = nextIndex;
        }
    }
}