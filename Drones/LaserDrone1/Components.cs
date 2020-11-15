using RoR2.UI;
using UnityEngine;

namespace Chen.GradiusMod
{
    public class BodyRotation : MonoBehaviour
    {
        private int rotationDirection;
        private readonly float rotationSpeed = 4f;

        public void Awake()
        {
            rotationDirection = Random.Range(0, 2) * 2 - 1;
        }

        public void Update()
        {
            if (PauseScreenController.paused) return;
            transform.Rotate(Vector3.forward, rotationSpeed * rotationDirection);
        }
    }

    public class ThrusterFlicker : MonoBehaviour
    {
        private GameObject thrusterHalo;
        private GameObject thrusterOrb;
        private Light haloLight;
        private bool thrustOrbDirection = false;

        private readonly float thrustOrbScaleMax = 2f;
        private readonly float thrustOrbScaleMin = .5f;
        private readonly Vector3 addThrust = new Vector3(0, 0, .5f);
        private readonly Vector3 minusThrust = new Vector3(0, 0, .125f);
        private readonly float minHaloRange = 1f;
        private readonly float maxHaloRange = 1.1f;

        public void Awake()
        {
            thrusterHalo = gameObject.transform.Find("ThrusterLight").gameObject;
            thrusterOrb = gameObject.transform.Find("ThrusterLightOrb").gameObject;
            haloLight = thrusterHalo.GetComponent<Light>();
        }

        public void Update()
        {
            if (PauseScreenController.paused) return;
            UpdateOrb();
            UpdateHalo();
        }

        private void UpdateOrb()
        {
            if (thrustOrbDirection)
            {
                thrusterOrb.transform.localScale += addThrust;
                if (thrusterOrb.transform.localScale.z >= thrustOrbScaleMax)
                {
                    thrustOrbDirection = !thrustOrbDirection;
                }
            }
            else
            {
                thrusterOrb.transform.localScale -= minusThrust;
                if (thrusterOrb.transform.localScale.z <= thrustOrbScaleMin)
                {
                    thrustOrbDirection = !thrustOrbDirection;
                }
            }
        }

        private void UpdateHalo()
        {
            haloLight.range = Random.Range(minHaloRange, maxHaloRange);
        }
    }
}