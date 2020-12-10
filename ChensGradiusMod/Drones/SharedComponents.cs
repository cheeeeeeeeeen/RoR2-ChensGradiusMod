using RoR2.UI;
using UnityEngine;

namespace Chen.GradiusMod.Drones
{
    /// <summary>
    /// A component that allows the model to be rotated along the Z-axis.
    /// May be useful to certain custom drones and some behavioral effects.
    /// </summary>
    public class BodyRotation : MonoBehaviour
    {
        /// <summary>
        /// The constant base speed of which the object will rotate around the Z-axis.
        /// </summary>
        public float rotationSpeed = 8f;

        /// <summary>
        /// The direction of the rotation. This should only be 1, 0 or -1.
        /// Anything less or greater will cause faster rotation. 0 will not let it rotate.
        /// </summary>
        public sbyte rotationDirection = 0;

        /// <summary>
        /// A flag to toggle if the model should accelerate in rotating.
        /// </summary>
        public bool accelerate = false;

        /// <summary>
        /// The rate at which the rotation speed will accelerate.
        /// </summary>
        public float acceleration = .05f;

        /// <summary>
        /// The maximum rotation speed that the model can achieve.
        /// </summary>
        public float maxRotationSpeed = 24f;

        private float currentAccel = 0f;

        private void Update()
        {
            if (rotationDirection == 0) rotationDirection = (sbyte)(Random.Range(0, 2) * 2 - 1);
            if (PauseScreenController.paused) return;
            HandleAcceleration();
            transform.Rotate(Vector3.forward, (rotationSpeed + currentAccel) * rotationDirection);
        }

        private void HandleAcceleration()
        {
            if (accelerate && currentAccel < maxRotationSpeed)
            {
                currentAccel += acceleration;
                currentAccel = Mathf.Min(maxRotationSpeed, currentAccel);
            }
            else if (currentAccel > 0f)
            {
                currentAccel -= acceleration * 2f;
                currentAccel = Mathf.Max(0f, currentAccel);
            }
        }
    }
}