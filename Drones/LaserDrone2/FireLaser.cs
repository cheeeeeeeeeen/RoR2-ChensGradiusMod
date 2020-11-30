using EntityStates;
using RoR2;
using UnityEngine;
using GolemChargeLaser = EntityStates.GolemMonster.ChargeLaser;
using GolemLaser = EntityStates.GolemMonster.FireLaser;

namespace Chen.GradiusMod
{
    internal class FireLaser : BaseState
    {
        public const uint chargeLaserEventId = 3954655918;
        public const uint dissipateLaserEventId = 2908790270;

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
                    start = aimRay.origin
                };
                EffectManager.SpawnEffect(tracerEffectPrefab, effectData, true);
                EffectManager.SpawnEffect(hitEffectPrefab, effectData, true);
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
                characterBody.SetAimTimer(baseDuration);
                if (isAuthority) AkSoundEngine.PostEvent(chargeLaserEventId, gameObject);
            }
            muzzle = transform.Find("ModelBase").Find("mdlLaserDrone").Find("AimOrigin").gameObject;
            chargeEffect = muzzle.GetComponent<ChargeEffect>();
            chargeEffect.startCharging = true;
            if (particleEffectPrefab)
            {
                particleChargeEffect = Object.Instantiate(particleEffectPrefab, muzzle.transform.position, muzzle.transform.rotation);
                particleChargeEffect.transform.parent = transform;
                ScaleParticleSystemDuration particleSystem = particleChargeEffect.GetComponent<ScaleParticleSystemDuration>();
                if (particleSystem) particleSystem.newDuration = duration;
                GradiusOption.instance.FireForAllOptions(characterBody, (option, behavior, target) =>
                {
                    behavior.laserChargeEffect = Object.Instantiate(particleEffectPrefab, option.transform.position, option.transform.rotation);
                    behavior.laserChargeEffect.transform.parent = option.transform;
                    ScaleParticleSystemDuration optionParticleSystem = behavior.laserChargeEffect.GetComponent<ScaleParticleSystemDuration>();
                    if (optionParticleSystem) optionParticleSystem.newDuration = duration;
                });
            }
            bodyRotation = transform.GetComponentInChildren<BodyRotation>();
            bodyRotation.accelerate = true;
        }

        public override void OnExit()
        {
            AkSoundEngine.PostEvent(dissipateLaserEventId, gameObject);
            if (particleChargeEffect) Destroy(particleChargeEffect);
            GradiusOption.instance.FireForAllOptions(characterBody, (option, behavior, target) =>
            {
                if (behavior.laserChargeEffect) Destroy(behavior.laserChargeEffect);
            });
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                chargeEffect.Reset();
                if (effectPrefab) EffectManager.SimpleMuzzleFlash(effectPrefab, gameObject, "Muzzle", false);
                if (isAuthority)
                {
                    Ray aimRay = new Ray(muzzle.transform.position, GetAimRay().direction);
                    Vector3 vector = aimRay.origin + aimRay.direction * maxDistance;
                    GenerateLaser(aimRay, vector, damageStat * damageCoefficient, force);
                }
                Util.PlaySound(attackSoundString, gameObject);
                GradiusOption.instance.FireForAllOptions(characterBody, (option, behavior, target) =>
                {
                    if (effectPrefab) EffectManager.SimpleMuzzleFlash(effectPrefab, option, "Muzzle", false);
                    if (isAuthority)
                    {
                        Vector3 direction = (target.transform.position - option.transform.position).normalized;
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