using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class MirrorLaserController : MonoBehaviour
    {
        private const int MaxPositions = 100;

        public Vector3 direction { get; set; }
        public Transform source { get; set; }
        public bool complete { get; private set; }
        public bool curved { get; private set; }

        private LineRenderer lineRenderer;
        private ParticleSystem spawnerEffect;
        private ParticleSystem muzzleEffect;
        private Queue<Vector3> vertices;

        private void Awake()
        {
            lineRenderer = GetComponentInChildren<LineRenderer>();
            lineRenderer.positionCount = 0;
            vertices = new Queue<Vector3>();
            spawnerEffect = transform.Find("Spawn").GetComponent<ParticleSystem>();
            muzzleEffect = transform.Find("Laser Emission").GetComponent<ParticleSystem>();
        }

        private void FixedUpdate()
        {
            Vector3 newEndPosition;
            if (vertices.Count <= 0) newEndPosition = transform.position;
            else newEndPosition = vertices.Last() + direction;
            vertices.Enqueue(newEndPosition);
            if (complete) vertices.Dequeue();
            else if (vertices.Count >= MaxPositions)
            {
                complete = true;
                spawnerEffect.Stop();
                muzzleEffect.Stop();
            }
        }

        private void Update()
        {
            lineRenderer.positionCount = vertices.Count;
            lineRenderer.SetPositions(vertices.ToArray());
        }
    }
}