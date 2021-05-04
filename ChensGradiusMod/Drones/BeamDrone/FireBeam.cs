using Chen.GradiusMod.Compatibility;
using Chen.GradiusMod.Items.GradiusOption;
using EntityStates;
using EntityStates.TitanMonster;
using RoR2;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Chen.GradiusMod.Drones.BeamDrone
{
    internal class FireBeam : BaseState
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

        private HurtBox lockedOnHurtBox;
        private float fireStopwatch;
        private float stopwatch;
        private Ray aimRay;
        private GameObject laserEffect;
        private ChildLocator laserChildLocator;
        private Transform laserEffectEnd;
        private BullseyeSearch enemyFinder;
        private bool foundAnyTarget;
        private Transform muzzle;
        private BodyRotation bodyRotation;

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

        private void FireBullet(GameObject weapon, float damage, float force, Ray aimRay, string targetMuzzle, float maxDistance)
        {
            if (isAuthority)
            {
                new BulletAttack
                {
                    owner = gameObject,
                    weapon = weapon,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = minSpread,
                    maxSpread = maxSpread,
                    bulletCount = 1U,
                    damage = damage,
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

        private static void Initialize()
        {
            if (initialized) return;
            initialized = true;
            FireMegaLaser fmlState = new FireMegaLaser();
            hitEffectPrefab = fmlState.hitEffectPrefab;
            laserPrefab = fmlState.laserPrefab;
            playAttackSoundString = FireMegaLaser.playAttackSoundString;
            playLoopSoundString = FireMegaLaser.playLoopSoundString;
            stopLoopSoundString = FireMegaLaser.stopLoopSoundString;
            damageCoefficient = LaserDrone1.instance.damageCoefficient;
            maxSpread = minSpread = force = 0f;
            fireFrequency = 8f;
            maxDistance = 180f;
            minimumDuration = LaserDrone1.instance.laserDuration;
            maximumDuration = minimumDuration;
            lockOnAngle = .3f;
            procCoefficientPerTick = .25f;
        }

        public override void OnEnter()
        {
            Initialize();
            base.OnEnter();
            characterBody.SetAimTimer(maximumDuration);
            Util.PlaySound(playAttackSoundString, gameObject);
            Util.PlaySound(playLoopSoundString, gameObject);
            aimRay = GetAimRay();
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
            muzzle = transform.Find("ModelBase").Find("mdlBeamDrone").Find("AimOrigin");
            laserEffect = Object.Instantiate(laserPrefab, muzzle.position, muzzle.rotation);
            laserEffect.transform.parent = muzzle;
            laserChildLocator = laserEffect.GetComponent<ChildLocator>();
            laserEffectEnd = laserChildLocator.FindChild("LaserEnd");
            UpdateLockOn();
            GradiusOption.instance.FireForAllOptions(characterBody, (option, behavior, _t, _d) =>
            {
                if (behavior.U.ContainsKey("laserFire") && behavior.U["laserFire"]) Destroy(behavior.U["laserFire"]);
                if (behavior.U.ContainsKey("laserChildLocator") && behavior.U["laserChildLocator"]) Destroy(behavior.U["laserChildLocator"]);
                if (behavior.U.ContainsKey("laserFireEnd") && behavior.U["laserFireEnd"]) Destroy(behavior.U["laserFireEnd"]);
                Transform transform = option.transform;
                behavior.U["laserFire"] = Object.Instantiate(laserPrefab, transform.position, transform.rotation);
                ((GameObject)behavior.U["laserFire"]).transform.parent = transform;
                behavior.U["laserChildLocator"] = ((GameObject)behavior.U["laserFire"]).GetComponent<ChildLocator>();
                behavior.U["laserFireEnd"] = ((ChildLocator)behavior.U["laserChildLocator"]).FindChild("LaserEnd");
            });
            bodyRotation = transform.GetComponentInChildren<BodyRotation>();
            bodyRotation.accelerate = true;
        }

        public override void OnExit()
        {
            if (laserEffect) Destroy(laserEffect);
            characterBody.SetAimTimer(maximumDuration);
            Util.PlaySound(stopLoopSoundString, gameObject);
            GradiusOption.instance.FireForAllOptions(characterBody, (option, behavior, _t, _d) =>
            {
                if (behavior.U["laserFire"]) Destroy(behavior.U["laserFire"]);
                if (behavior.U["laserChildLocator"]) Destroy(behavior.U["laserChildLocator"]);
                if (behavior.U["laserFireEnd"]) Destroy(behavior.U["laserFireEnd"]);
            });
            bodyRotation.accelerate = false;
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            fireStopwatch += Time.fixedDeltaTime;
            stopwatch += Time.fixedDeltaTime;
            aimRay = GetAimRay();
            float optionFireStopwatch = fireStopwatch;
            if (isAuthority && !lockedOnHurtBox && foundAnyTarget)
            {
                outer.SetNextState(new FireBeam { stopwatch = stopwatch });
                return;
            }
            Vector3 origin = aimRay.origin;
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
            if (laserEffect && laserChildLocator && laserEffectEnd)
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
                laserEffectEnd.position = target;
            }
            if (fireStopwatch > 1f / fireFrequency)
            {
                if (!flag)
                {
                    float damage = damageCoefficient * damageStat;
                    FireBullet(gameObject, damage, force, ray, "Muzzle", (target - ray.origin).magnitude + 0.1f);
                    if (ChensClassicItems.enabled)
                    {
                        ChensClassicItems.TriggerArtillery(characterBody, damage, Util.CheckRoll(critStat, characterBody.master));
                    }
                }
                fireStopwatch -= 1f / fireFrequency;
            }
            GradiusOption.instance.FireForAllOptions(characterBody, (option, behavior, optionTarget, direction) =>
            {
                Vector3 position = option.transform.position;
                if (!optionTarget && lockedOnHurtBox) direction = (lockedOnHurtBox.transform.position - position).normalized;

                Vector3 point = direction * maxDistance;
                if (Physics.Raycast(position, point, out RaycastHit raycastHit3, maxDistance,
                                    LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore))
                {
                    point = raycastHit3.point;
                }
                Ray optionRay = new Ray(position, point - position);
                bool optionFlag = false;
                if (behavior.U["laserFire"] && behavior.U["laserChildLocator"] && behavior.U["laserFireEnd"])
                {
                    if (Physics.Raycast(optionRay.origin, optionRay.direction, out RaycastHit raycastHit4, optionRay.direction.magnitude,
                                        LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
                    {
                        point = raycastHit4.point;
                        if (Physics.Raycast(point - optionRay.direction * .1f, -optionRay.direction, out _, raycastHit4.distance,
                                            LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
                        {
                            point = optionRay.GetPoint(.1f);
                            optionFlag = true;
                        }
                    }
                    ((GameObject)behavior.U["laserFire"]).transform.rotation = Util.QuaternionSafeLookRotation(point - position);
                    ((Transform)behavior.U["laserFireEnd"]).position = point;
                }
                if (optionFireStopwatch > 1f / fireFrequency)
                {
                    if (!optionFlag)
                    {
                        float damage = damageCoefficient * damageStat * GradiusOption.instance.damageMultiplier;
                        FireBullet(option, damage, force * GradiusOption.instance.damageMultiplier, optionRay,
                                   "Muzzle", (point - optionRay.origin).magnitude + .1f);
                    }
                }
            });
            if (isAuthority && (((!inputBank || !inputBank.skill4.down) && stopwatch > minimumDuration) || stopwatch > maximumDuration))
            {
                outer.SetNextStateToMain();
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
            if (gameObject) lockedOnHurtBox = gameObject.GetComponent<HurtBox>();
            else lockedOnHurtBox = null;
        }
    }
}