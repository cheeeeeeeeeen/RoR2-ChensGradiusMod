using Chen.GradiusMod;
using EntityStates;
using EntityStates.TitanMonster;
using RoR2;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Chens.GradiusMod
{
    internal class FireLaser : BaseState
    {
        public static GameObject hitEffectPrefab;
        public static GameObject laserPrefab;
        public static string playAttackSoundString;
        public static string playLoopSoundString;
        public static string stopLoopSoundString;
        public static float damageCoefficient;
        public static float force;
        public static float minSpread;
        public static float maxSpread;
        public static float fireFrequency;
        public static float maxDistance;
        public static float minimumDuration;
        public static float maximumDuration;
        public static float lockOnAngle;
        public static float procCoefficientPerTick;

        private static bool initialized = false;

        protected Transform muzzleTransform;

        private HurtBox lockedOnHurtBox;
        private float fireStopwatch;
        private float stopwatch;
        private Ray aimRay;
        private Transform modelTransform;
        private GameObject laserEffect;
        private ChildLocator laserChildLocator;
        private Transform laserEffectEnd;
        private BullseyeSearch enemyFinder;
        private bool foundAnyTarget;

        private void UpdateLockOn()
        {
            if (isAuthority)
            {
                enemyFinder.searchOrigin = aimRay.origin;
                enemyFinder.searchDirection = aimRay.direction;
                enemyFinder.RefreshCandidates();
                HurtBox exists = enemyFinder.GetResults().FirstOrDefault();
                lockedOnHurtBox = exists;
                foundAnyTarget = exists;
            }
        }

        private void FireBullet(Ray aimRay, string targetMuzzle, float maxDistance)
        {
            if (isAuthority)
            {
                new BulletAttack
                {
                    owner = gameObject,
                    weapon = gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = minSpread,
                    maxSpread = maxSpread,
                    bulletCount = 1U,
                    damage = damageCoefficient * damageStat,
                    force = force,
                    muzzleName = targetMuzzle,
                    hitEffectPrefab = hitEffectPrefab,
                    isCrit = Util.CheckRoll(critStat, characterBody.master),
                    procCoefficient = procCoefficientPerTick,
                    HitEffectNormal = false,
                    radius = 0f,
                    maxDistance = maxDistance
                }.Fire();
            }
        }

        private void Initialize()
        {
            if (initialized) return;
            initialized = true;
            FireMegaLaser fmlState = Instantiate(typeof(FireMegaLaser)) as FireMegaLaser;
            hitEffectPrefab = fmlState.hitEffectPrefab;
            laserPrefab = fmlState.laserPrefab;
            playAttackSoundString = FireMegaLaser.playAttackSoundString;
            playLoopSoundString = FireMegaLaser.playLoopSoundString;
            stopLoopSoundString = FireMegaLaser.stopLoopSoundString;
            damageCoefficient = 1f;
            force = 0f;
            minSpread = FireMegaLaser.minSpread;
            maxSpread = FireMegaLaser.maxSpread;
            fireFrequency = FireMegaLaser.fireFrequency;
            maxDistance = FireMegaLaser.maxDistance;
            minimumDuration = 4f;
            maximumDuration = 4f;
            lockOnAngle = FireMegaLaser.lockOnAngle;
            procCoefficientPerTick = .5f;
        }

        public override void OnEnter()
        {
            Initialize();
            base.OnEnter();
            characterBody.SetAimTimer(maximumDuration);
            Util.PlaySound(playAttackSoundString, gameObject);
            Util.PlaySound(playLoopSoundString, gameObject);
            enemyFinder = new BullseyeSearch
            {
                maxDistanceFilter = maxDistance,
                maxAngleFilter = lockOnAngle,
                searchOrigin = aimRay.origin,
                searchDirection = aimRay.direction,
                filterByLoS = false,
                sortMode = BullseyeSearch.SortMode.Angle,
                teamMaskFilter = TeamMask.allButNeutral
            };
            if (teamComponent) enemyFinder.teamMaskFilter.RemoveTeam(teamComponent.teamIndex);
            aimRay = GetAimRay();
            modelTransform = GetModelTransform();
            if (modelTransform)
            {
                ChildLocator locator = modelTransform.GetComponent<ChildLocator>();
                if (locator)
                {
                    muzzleTransform = locator.FindChild("Muzzle");
                    if (muzzleTransform && laserPrefab)
                    {
                        laserEffect = Object.Instantiate(laserPrefab, muzzleTransform.position, muzzleTransform.rotation);
                        laserEffect.transform.parent = muzzleTransform;
                        laserChildLocator = laserEffect.GetComponent<ChildLocator>();
                        laserEffectEnd = laserChildLocator.FindChild("LaserEnd");
                    }
                }
            }
            UpdateLockOn();
        }

        public override void OnExit()
        {
            if (laserEffect) Destroy(laserEffect);
            characterBody.SetAimTimer(2f);
            Util.PlaySound(stopLoopSoundString, gameObject);
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            fireStopwatch += Time.fixedDeltaTime;
            stopwatch += Time.fixedDeltaTime;
            aimRay = GetAimRay();
            if (isAuthority && !lockedOnHurtBox && foundAnyTarget)
            {
                outer.SetNextState(new FireLaser { stopwatch = stopwatch });
                return;
            }
            Vector3 origin = aimRay.origin;
            if (muzzleTransform) origin = muzzleTransform.position;
            Vector3 target;
            if (lockedOnHurtBox) target = lockedOnHurtBox.transform.position;
            else if (Util.CharacterRaycast(gameObject, aimRay, out RaycastHit raycastHit, maxDistance,
                                           LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore))
            {
                target = raycastHit.point;
            }
            else target = aimRay.GetPoint(maxDistance);
            Ray ray = new Ray(origin, target - origin);
            bool flag = false;
            if (laserEffect && laserChildLocator)
            {
                if (Util.CharacterRaycast(gameObject, ray, out RaycastHit raycastHit2, (target - origin).magnitude,
                                          LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
                {
                    target = raycastHit2.point;
                    if (Util.CharacterRaycast(gameObject, new Ray(target - ray.direction * 0.1f, -ray.direction), out _, raycastHit2.distance,
                                              LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
                    {
                        target = ray.GetPoint(.1f);
                        flag = true;
                    }
                }
                laserEffect.transform.rotation = Util.QuaternionSafeLookRotation(target - origin);
                laserEffectEnd.transform.position = target;
            }
            if (fireStopwatch > 1f / fireFrequency)
            {
                if (!flag) FireBullet(ray, "Muzzle", (target - ray.origin).magnitude + 0.1f);
                fireStopwatch -= 1f / fireFrequency;
            }
            if (isAuthority && (((!inputBank || !inputBank.skill4.down) && stopwatch > minimumDuration) || stopwatch > maximumDuration))
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Skill;

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(HurtBoxReference.FromHurtBox(lockedOnHurtBox));
            writer.Write(stopwatch);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            HurtBoxReference hurtBoxReference = reader.ReadHurtBoxReference();
            stopwatch = reader.ReadSingle();
            GameObject gameObject = hurtBoxReference.ResolveGameObject();
            lockedOnHurtBox = gameObject?.GetComponent<HurtBox>();
        }
    }
}