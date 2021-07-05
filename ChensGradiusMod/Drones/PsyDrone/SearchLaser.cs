using Chen.Helpers.UnityHelpers;
using EntityStates;
using UnityEngine;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class SearchLaser : BaseState
    {
        private const uint FireSoundEffect = 4131085986;
        private const int TotalLasers = 10;
        private const float FireInterval = .15f;

        private Transform modelTransform;
        private Transform aimOrigin;
        private float fireTimer;
        private int laserNumber;

        public override void OnEnter()
        {
            base.OnEnter();
            modelTransform = GetModelTransform();
            aimOrigin = modelTransform.Find("AimOrigin");
            laserNumber = 0;
            fireTimer = FireInterval;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fireTimer >= FireInterval)
            {
                GameObject searchLaser = Object.Instantiate(PsyDrone.searchLaserPrefab, aimOrigin.position, aimOrigin.rotation);
                AkSoundEngine.PostEvent(FireSoundEffect, searchLaser);
                SearchLaserController laserController = searchLaser.GetOrAddComponent<SearchLaserController>();
                laserController.owner = characterBody;
                fireTimer -= FireInterval;
                laserNumber++;
            }
            fireTimer += Time.fixedDeltaTime;
            if (laserNumber >= TotalLasers) outer.SetNextStateToMain();
        }
    }
}