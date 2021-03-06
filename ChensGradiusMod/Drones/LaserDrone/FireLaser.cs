﻿using Chen.GradiusMod.Compatibility;
using Chen.GradiusMod.Items.GradiusOption;
using EntityStates;
using RoR2;
using UnityEngine;
using GolemChargeLaser = EntityStates.GolemMonster.ChargeLaser;
using GolemLaser = EntityStates.GolemMonster.FireLaser;

namespace Chen.GradiusMod.Drones.LaserDrone
{
    internal class FireLaser : BaseState
    {
        public const uint ChargeLaserEventId = 3954655918;
        public const uint DissipateLaserEventId = 2908790270;

        public static GameObject effectPrefab;
        public static GameObject hitEffectPrefab;
        public static GameObject tracerEffectPrefab;
        public static GameObject particleEffectPrefab;
        public static float damageCoefficient;
        public static float blastRadius;
        public static float force;
        public static float minSpread;
        public static float maxSpread;
        public static int bulletCount;
        public static float baseDuration;
        public static string attackSoundString;
        public static float maxDistance;

        private static bool initialized = false;

        private float duration;
        private ChargeEffect chargeEffect;
        private GameObject particleChargeEffect;
        private BodyRotation bodyRotation;
        private GameObject muzzle;

        private void GenerateLaser(Ray aimRay, Vector3 vector, float damage, float computedForce)
        {
            if (Physics.Raycast(aimRay, out RaycastHit raycastHit, maxDistance,
                                LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.entityPrecise.mask))
            {
                vector = raycastHit.point;
            }
            new BlastAttack
            {
                attacker = gameObject,
                inflictor = gameObject,
                teamIndex = teamComponent.teamIndex,
                baseDamage = damage,
                baseForce = computedForce,
                position = vector,
                radius = blastRadius,
                falloffModel = BlastAttack.FalloffModel.Linear,
                bonusForce = computedForce * aimRay.direction
            }.Fire();
            if (tracerEffectPrefab && hitEffectPrefab)
            {
                EffectData effectData = new EffectData
                {
                    origin = vector,
                    start = aimRay.origin + (aimRay.direction.normalized * 2f)
                };
                EffectManager.SpawnEffect(tracerEffectPrefab, effectData, true);
                EffectManager.SpawnEffect(hitEffectPrefab, effectData, true);
            }
            if (ChensClassicItems.enabled)
            {
                ChensClassicItems.TriggerArtillery(characterBody, damage, Util.CheckRoll(critStat, characterBody.master));
            }
        }

        private static void Initialize()
        {
            if (initialized) return;
            initialized = true;
            effectPrefab = GolemLaser.effectPrefab;
            hitEffectPrefab = GolemLaser.hitEffectPrefab;
            tracerEffectPrefab = GolemLaser.tracerEffectPrefab;
            damageCoefficient = LaserDrone2.instance.damageCoefficient;
            baseDuration = LaserDrone2.instance.chargeTime;
            blastRadius = GolemLaser.blastRadius * 3f;
            force = GolemLaser.force * .2f;
            minSpread = GolemLaser.minSpread;
            maxSpread = GolemLaser.maxSpread;
            bulletCount = GolemLaser.bulletCount;
            attackSoundString = GolemLaser.attackSoundString;
            maxDistance = 270f;
            particleEffectPrefab = GolemChargeLaser.effectPrefab;
        }

        public override void OnEnter()
        {
            Initialize();
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            if (characterBody)
            {
                characterBody.SetAimTimer(duration);
                if (isAuthority) AkSoundEngine.PostEvent(ChargeLaserEventId, gameObject);
            }
            muzzle = transform.Find("ModelBase").Find("mdlLaserDrone").Find("AimOrigin").gameObject;
            chargeEffect = muzzle.GetComponent<ChargeEffect>();
            chargeEffect.startCharging = true;
            if (particleEffectPrefab)
            {
                particleChargeEffect = Object.Instantiate(particleEffectPrefab, muzzle.transform.position, muzzle.transform.rotation);
                particleChargeEffect.transform.parent = transform;
                var particleSystem = particleChargeEffect.GetComponent<ScaleParticleSystemDuration>();
                if (particleSystem) particleSystem.newDuration = duration;
                GradiusOption.instance.FireForAllOptions(characterBody, (option, behavior, _t, _d) =>
                {
                    behavior.U["laserChargeEffect"] = Object.Instantiate(particleEffectPrefab, option.transform.position, option.transform.rotation);
                    ((GameObject)behavior.U["laserChargeEffect"]).transform.parent = option.transform;
                    var optionParticleSystem = ((GameObject)behavior.U["laserChargeEffect"]).GetComponent<ScaleParticleSystemDuration>();
                    if (optionParticleSystem) optionParticleSystem.newDuration = duration;
                });
            }
            bodyRotation = transform.GetComponentInChildren<BodyRotation>();
            bodyRotation.accelerate = true;
        }

        public override void OnExit()
        {
            if (isAuthority) AkSoundEngine.PostEvent(DissipateLaserEventId, gameObject);
            if (particleChargeEffect) Destroy(particleChargeEffect);
            GradiusOption.instance.FireForAllOptions(characterBody, (option, behavior, _t, _d) =>
            {
                if (behavior.U.SafeCheck("laserChargeEffect")) Destroy(behavior.U["laserChargeEffect"]);
            });
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                chargeEffect.Reset();
                if (effectPrefab)
                {
                    EffectData data = new EffectData
                    {
                        origin = muzzle.transform.position,
                        rotation = muzzle.transform.rotation,
                        rootObject = muzzle
                    };
                    EffectManager.SpawnEffect(effectPrefab, data, false);
                }
                if (isAuthority)
                {
                    Ray aimRay = new Ray(muzzle.transform.position, GetAimRay().direction);
                    Vector3 vector = aimRay.origin + aimRay.direction * maxDistance;
                    GenerateLaser(aimRay, vector, damageStat * damageCoefficient, force);
                    //Vector3 direction;
                    //GameObject target = null;
                    //BaseAI baseAi = characterBody.masterObject.GetComponent<BaseAI>();
                    //BaseAI.Target mainTarget = baseAi.currentEnemy;
                    //if (mainTarget != null && mainTarget.gameObject)
                    //{
                    //    target = mainTarget.gameObject;
                    //}
                    //if (target) direction = (target.transform.position - muzzle.transform.position).normalized;
                    //else direction = GetAimRay().direction;
                    //Ray aimRay = new Ray(muzzle.transform.position, direction);
                    //Vector3 vector = aimRay.origin + aimRay.direction * maxDistance;
                    //GenerateLaser(aimRay, vector, damageStat * damageCoefficient, force);
                }
                Util.PlaySound(attackSoundString, gameObject);
                GradiusOption.instance.FireForAllOptions(characterBody, (option, behavior, _t, direction) =>
                {
                    if (effectPrefab) option.MuzzleEffect(effectPrefab, false);
                    if (isAuthority)
                    {
                        Ray aimRay = new Ray(option.transform.position, direction);
                        Vector3 vector = aimRay.origin + aimRay.direction * maxDistance;
                        GenerateLaser(aimRay, vector, damageStat * damageCoefficient * GradiusOption.instance.damageMultiplier,
                                      force * GradiusOption.instance.damageMultiplier);
                    }
                });
                bodyRotation.accelerate = false;
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Skill;
    }
}