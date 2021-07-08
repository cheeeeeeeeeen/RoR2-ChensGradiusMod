using Chen.Helpers.UnityHelpers;
using EntityStates;
using RoR2;
using UnityEngine;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class MirrorLaser : BaseState
    {
        private Transform modelTransform;
        private Transform aimOrigin;
        private Transform modelBaseTransform;
        private MirrorLaserController firstLC;
        private MirrorLaserController secondLC;
        private MirrorLaserController thirdLC;
        private MirrorLaserController fourthLC;
        private MirrorLaserController fifthLC;
        private ParticleSystem muzzleEffect;

        public override void OnEnter()
        {
            base.OnEnter();
            modelTransform = GetModelTransform();
            modelBaseTransform = modelTransform.parent;
            aimOrigin = modelTransform.Find("AimOrigin");
            GameObject laserObject = Object.Instantiate(PsyDrone.mirrorLaserPrefab, aimOrigin.position, aimOrigin.rotation);
            firstLC = laserObject.GetOrAddComponent<MirrorLaserController>();
            firstLC.direction = ComputeDirection(Vector3.forward);
            firstLC.spawnEffects = true;
            laserObject = Object.Instantiate(PsyDrone.mirrorLaserPrefab, aimOrigin.position, aimOrigin.rotation);
            secondLC = laserObject.GetOrAddComponent<MirrorLaserController>();
            secondLC.direction = ComputeDirection(new Vector3(1, 1, 1));
            laserObject = Object.Instantiate(PsyDrone.mirrorLaserPrefab, aimOrigin.position, aimOrigin.rotation);
            thirdLC = laserObject.GetOrAddComponent<MirrorLaserController>();
            thirdLC.direction = ComputeDirection(new Vector3(-1, 1, 1));
            laserObject = Object.Instantiate(PsyDrone.mirrorLaserPrefab, aimOrigin.position, aimOrigin.rotation);
            fourthLC = laserObject.GetOrAddComponent<MirrorLaserController>();
            fourthLC.direction = ComputeDirection(new Vector3(-1, -1, 1));
            laserObject = Object.Instantiate(PsyDrone.mirrorLaserPrefab, aimOrigin.position, aimOrigin.rotation);
            fifthLC = laserObject.GetOrAddComponent<MirrorLaserController>();
            fifthLC.direction = ComputeDirection(new Vector3(1, -1, 1));
            firstLC.owner = secondLC.owner = thirdLC.owner = fourthLC.owner = fifthLC.owner = characterBody;
            muzzleEffect = aimOrigin.gameObject.GetComponent<ParticleSystem>();
            muzzleEffect.Play();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (AllLasersComplete()) outer.SetNextStateToMain();
        }

        public override void OnExit()
        {
            muzzleEffect.Stop();
            base.OnExit();
        }

        private Vector3 ComputeDirection(Vector3 localDirection)
        {
            return (Util.QuaternionSafeLookRotation(modelBaseTransform.forward) * localDirection).normalized;
        }

        private bool AllLasersComplete()
        {
            return firstLC.complete && secondLC.complete && thirdLC.complete && fourthLC.complete && fifthLC.complete;
        }
    }
}