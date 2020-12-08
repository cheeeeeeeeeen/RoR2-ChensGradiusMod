using RoR2.UI;
using UnityEngine;
using static Chen.Helpers.MathHelpers.Wave;

namespace Chen.GradiusMod
{
    internal class ChargeEffect : MonoBehaviour
    {
        public bool startCharging = false;

        private Light haloLight;
        private float currentBaseRange = 0f;

        private readonly float incrementalIncrease = .01f;
        private readonly float minHaloRange = -.25f;
        private readonly float maxHaloRange = .25f;

        public void Reset()
        {
            currentBaseRange = 0f;
            startCharging = false;
        }

        private void Awake()
        {
            haloLight = gameObject.GetComponent<Light>();
        }

        private void Update()
        {
            if (PauseScreenController.paused || !haloLight) return;
            if (startCharging)
            {
                currentBaseRange += incrementalIncrease;
                haloLight.range = currentBaseRange + Random.Range(minHaloRange, maxHaloRange);
                haloLight.range = KeepZeroOrPositive(haloLight.range);
            }
            else if (haloLight.range > 0)
            {
                haloLight.range -= incrementalIncrease * 10f;
                haloLight.range = KeepZeroOrPositive(haloLight.range);
            }
        }

        private float KeepZeroOrPositive(float range) => Mathf.Max(range, 0f);
    }

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