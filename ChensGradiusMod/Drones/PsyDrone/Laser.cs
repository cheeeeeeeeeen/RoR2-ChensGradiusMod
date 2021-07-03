using UnityEngine;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class Laser : MonoBehaviour
    {
        public LineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        private void FixedUpdate()
        {
        }

        private void Update()
        {
        }
    }
}