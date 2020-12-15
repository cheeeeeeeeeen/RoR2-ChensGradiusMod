using RoR2.UI;
using UnityEngine;
using static Chen.Helpers.MathHelpers.Wave;

namespace Chen.GradiusMod.Items.GradiusOption.Components
{
    internal class Flicker : MonoBehaviour
    {
        // Child Objects in Order:
        // 0. sphere1:     Light
        // 1. sphere2:     Light
        // 2. sphere3:     Light
        // 3. sphere4:     MeshRenderer, MeshFilter (only in OptionOrb)
        // 4. sphere5:     MeshRenderer, MeshFilter (only in OptionOrbWithModel)
        // 5. OptionModel: The option model (only in OptionOrbWithModel)

        private readonly float baseValue = 1f;
        private readonly float amplitude = .25f;
        private readonly float phase = 0f;
        private readonly float frequency = 1f;

        private readonly Light[] lightObjects = new Light[3];
        private readonly float[] originalRange = new float[3];
        private readonly float[] ampMultiplier = new float[4] { 1.2f, 1f, .8f, .4f };
        private Vector3 originalLocalScale;
        private GameObject meshObject;

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                Light childLight = child.GetComponent<Light>();
                switch (child.name)
                {
                    case "sphere1":
                        originalRange[0] = childLight.range;
                        lightObjects[0] = childLight;
                        break;

                    case "sphere2":
                        originalRange[1] = childLight.range;
                        lightObjects[1] = childLight;
                        break;

                    case "sphere3":
                        originalRange[2] = childLight.range;
                        lightObjects[2] = childLight;
                        break;

                    case "sphere4":
                        originalLocalScale = child.transform.localScale;
                        meshObject = child;
                        break;

                    case "sphere5":
                        child.transform.localScale *= 1.5f;
                        break;

                    case "option":
                        child.transform.localScale = new Vector3(2f, 2f, 2f);
                        break;
                }
            }
        }

        private void FixedUpdate()
        {
            if (PauseScreenController.paused) return;
            for (int i = 0; i < lightObjects.Length; i++)
            {
                lightObjects[i].range = originalRange[i] * Sine(phase, frequency, amplitude * ampMultiplier[i], baseValue);
            }
            if (meshObject && originalLocalScale != null)
            {
                meshObject.transform.localScale = originalLocalScale * Sine(phase, frequency, amplitude * ampMultiplier[3], baseValue);
            }
        }
    }
}