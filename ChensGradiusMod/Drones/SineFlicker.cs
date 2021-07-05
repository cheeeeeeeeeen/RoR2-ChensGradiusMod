using RoR2.UI;
using UnityEngine;
using static Chen.Helpers.MathHelpers.Wave;

namespace Chen.GradiusMod.Drones
{
    /// <summary>
    /// An abstract component that needs to be implemented.
    /// It allows the Light component to flicker in a sine math function pattern.
    /// </summary>
    public abstract class SineFlicker : MonoBehaviour
    {
        /// <summary>
        /// The offset from the base range of the light.
        /// </summary>
        public abstract float baseValue { get; }

        /// <summary>
        /// The maximum and minimum value of the light's range.
        /// </summary>
        public abstract float amplitude { get; }

        /// <summary>
        /// Determines how fast the light flickers.
        /// </summary>
        public abstract float frequency { get; }

        private float originalRange;
        private Light light;

        private void Awake()
        {
            light = gameObject.GetComponent<Light>();
            if (!light) Destroy(this);
            else originalRange = light.range;
        }

        private void FixedUpdate()
        {
            if (PauseScreenController.paused) return;
            light.range = originalRange * Sine(0f, frequency, amplitude, baseValue);
        }
    }
}