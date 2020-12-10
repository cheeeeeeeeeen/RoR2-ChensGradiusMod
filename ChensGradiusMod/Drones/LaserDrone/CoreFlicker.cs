using RoR2.UI;
using UnityEngine;
using static Chen.Helpers.MathHelpers.Wave;

namespace Chen.GradiusMod.Drones.LaserDrone
{
    internal class CoreFlicker : MonoBehaviour
    {
        private readonly float baseValue = .6f;
        private readonly float amplitude = .1f;
        private readonly float frequency = .4f;

        private float originalRange;
        private Light light;

        private void Awake()
        {
            light = gameObject.GetComponent<Light>();
            originalRange = light.range;
        }

        private void Update()
        {
            if (PauseScreenController.paused) return;
            light.range = originalRange * Sine(0f, frequency, amplitude, baseValue);
        }
    }
}