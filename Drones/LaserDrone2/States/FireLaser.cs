using EntityStates;
using RoR2;
using UnityEngine;
using GolemLaser = EntityStates.GolemMonster.FireLaser;
using GolemChargeLaser = EntityStates.GolemMonster.ChargeLaser;

namespace Chen.GradiusMod
{
    public class FireLaser : BaseState
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
                AkSoundEngine.PostEvent(chargeLaserEventId, gameObject);
            }
            GameObject muzzle = transform.Find("ModelBase").Find("mdlLaserDrone").Find("AimOrigin").gameObject;
            chargeEffect = muzzle.GetComponent<ChargeEffect>();
            chargeEffect.startCharging = true;
            if (particleEffectPrefab)
            {
                particleChargeEffect = Object.Instantiate(particleEffectPrefab, muzzle.transform.position, muzzle.transform.rotation);
                particleChargeEffect.transform.parent = transform;
                ScaleParticleSystemDuration particleSystem = particleChargeEffect.GetComponent<ScaleParticleSystemDuration>();
                if (particleSystem) particleSystem.newDuration = duration;
            }
        }

        public override void OnExit()
        {
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
                    Ray aimRay = GetAimRay();
                    Vector3 vector = aimRay.origin + aimRay.direction * maxDistance;
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
                        baseDamage = damageStat * damageCoefficient,
                        baseForce = force,
                        position = vector,
                        radius = blastRadius,
                        falloffModel = BlastAttack.FalloffModel.Linear,
                        bonusForce = force * aimRay.direction
                    }.Fire();
                    if (tracerEffectPrefab)
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
                Util.PlaySound(attackSoundString, gameObject);
                AkSoundEngine.PostEvent(dissipateLaserEventId, gameObject);

                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Skill;
    }
}
