using RoR2.UI;
using UnityEngine;
using static Chen.Helpers.MathHelpers.Wave;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class CoreFlicker : MonoBehaviour
    {
        private readonly float baseValue = .75f;
        private readonly float amplitude = .06f;
        private readonly float frequency = .9f;

        private float originalRange;
        private Light light;

        private void Awake()
        {
            light = gameObject.GetComponent<Light>();
            originalRange = light.range;
        }

        private void FixedUpdate()
        {
            if (PauseScreenController.paused) return;
            light.range = originalRange * Sine(0f, frequency, amplitude, baseValue);
        }
    }
}