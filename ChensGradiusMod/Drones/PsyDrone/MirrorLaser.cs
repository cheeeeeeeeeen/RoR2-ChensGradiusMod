using Chen.Helpers.UnityHelpers;
using EntityStates;
using RoR2;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class MirrorLaser : BaseState
    {
        private static bool initialized = false;
        private static GameObject laserPrefab;

        private Transform modelTransform;
        private Transform aimOrigin;
        private Transform modelBaseTransform;
        private MirrorLaserController firstLC;
        private MirrorLaserController secondLC;
        private MirrorLaserController thirdLC;

        public override void OnEnter()
        {
            if (!initialized)
            {
                initialized = true;
                laserPrefab = assetBundle.LoadAsset<GameObject>("Assets/Drones/PsiBits/Model/MirrorLaser.prefab");
            }
            base.OnEnter();
            modelTransform = GetModelTransform();
            modelBaseTransform = modelTransform.parent;
            aimOrigin = modelTransform.Find("AimOrigin");
            GameObject laserObject = Object.Instantiate(laserPrefab, aimOrigin.position, aimOrigin.rotation);
            firstLC = laserObject.GetOrAddComponent<MirrorLaserController>();
            firstLC.direction = ComputeDirection(Vector3.forward);
            firstLC.source = aimOrigin;
            laserObject = Object.Instantiate(laserPrefab, aimOrigin.position, aimOrigin.rotation);
            secondLC = laserObject.GetOrAddComponent<MirrorLaserController>();
            secondLC.direction = ComputeDirection(new Vector3(1, 0, 1));
            secondLC.source = aimOrigin;
            laserObject = Object.Instantiate(laserPrefab, aimOrigin.position, aimOrigin.rotation);
            thirdLC = laserObject.GetOrAddComponent<MirrorLaserController>();
            thirdLC.direction = ComputeDirection(new Vector3(-1, 0, 1));
            thirdLC.source = aimOrigin;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (firstLC.complete && secondLC.complete && thirdLC.complete) outer.SetNextStateToMain();
        }

        private Vector3 ComputeDirection(Vector3 localDirection)
        {
            return (Util.QuaternionSafeLookRotation(modelBaseTransform.forward) * localDirection).normalized;
        }
    }
}