using RoR2.UI;
using UnityEngine;

namespace Chen.GradiusMod
{
    public class BodyRotation : MonoBehaviour
    {
        public float rotationSpeed = 8f;
        public sbyte rotationDirection = 0;
        public bool accelerate = false;
        public float acceleration = .05f;
        public float maxRotation = 24f;

        private float currentAccel = 0f;

        public void Update()
        {
            if (rotationDirection == 0) rotationDirection = (sbyte)(Random.Range(0, 2) * 2 - 1);
            if (PauseScreenController.paused) return;
            HandleAcceleration();
            transform.Rotate(Vector3.forward, (rotationSpeed + currentAccel) * rotationDirection);
        }

        private void HandleAcceleration()
        {
            if (accelerate && currentAccel < maxRotation)
            {
                currentAccel += acceleration;
                currentAccel = Mathf.Min(maxRotation, currentAccel);
            }
            else if (currentAccel > 0f)
            {
                currentAccel -= acceleration * 2f;
                currentAccel = Mathf.Max(0f, currentAccel);
            }
        }
    }
}
