using EntityStates;
using RoR2;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Chens.GradiusMod
{
    internal class FireLaser : BaseSkillState
    {
        [SerializeField]
        public GameObject effectPrefab;
        [SerializeField]
        public GameObject hitEffectPrefab;
        [SerializeField]
        public GameObject laserPrefab;

        public static string playAttackSoundString;
        public static string playLoopSoundString;
        public static string stopLoopSoundString;
        public static float damageCoefficient;
        public static float force;
        public static float minSpread;
        public static float maxSpread;
        public static int bulletCount;
        public static float fireFrequency;
        public static float maxDistance;
        public static float minimumDuration;
        public static float maximumDuration;
        public static float lockOnAngle;
        public static float procCoefficientPerTick;

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

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.SetAimTimer(FireLaser.maximumDuration);
            Util.PlaySound(FireLaser.playAttackSoundString, base.gameObject);
            Util.PlaySound(FireLaser.playLoopSoundString, base.gameObject);
            base.PlayCrossfade("Gesture, Additive", "FireLaserLoop", 0.25f);
            enemyFinder = new BullseyeSearch
            {
                maxDistanceFilter = FireLaser.maxDistance,
                maxAngleFilter = FireLaser.lockOnAngle,
                searchOrigin = this.aimRay.origin,
                searchDirection = this.aimRay.direction,
                filterByLoS = false,
                sortMode = BullseyeSearch.SortMode.Angle,
                teamMaskFilter = TeamMask.allButNeutral
            };
            if (base.teamComponent)
            {
                this.enemyFinder.teamMaskFilter.RemoveTeam(base.teamComponent.teamIndex);
            }
            this.aimRay = base.GetAimRay();
            this.modelTransform = base.GetModelTransform();
            if (this.modelTransform)
            {
                ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
                if (component)
                {
                    this.muzzleTransform = component.FindChild("MuzzleLaser");
                    if (this.muzzleTransform && this.laserPrefab)
                    {
                        this.laserEffect = UnityEngine.Object.Instantiate<GameObject>(this.laserPrefab, this.muzzleTransform.position, this.muzzleTransform.rotation);
                        this.laserEffect.transform.parent = this.muzzleTransform;
                        this.laserChildLocator = this.laserEffect.GetComponent<ChildLocator>();
                        this.laserEffectEnd = this.laserChildLocator.FindChild("LaserEnd");
                    }
                }
            }
            this.UpdateLockOn();
        }

        // Token: 0x06003BE2 RID: 15330 RVA: 0x000F9B68 File Offset: 0x000F7D68
        public override void OnExit()
        {
            if (this.laserEffect)
            {
                EntityState.Destroy(this.laserEffect);
            }
            base.characterBody.SetAimTimer(2f);
            Util.PlaySound(FireLaser.stopLoopSoundString, base.gameObject);
            base.PlayCrossfade("Gesture, Additive", "FireLaserEnd", 0.25f);
            base.OnExit();
        }

        // Token: 0x06003BE3 RID: 15331 RVA: 0x000F9BCC File Offset: 0x000F7DCC
        private void UpdateLockOn()
        {
            if (base.isAuthority)
            {
                this.enemyFinder.searchOrigin = this.aimRay.origin;
                this.enemyFinder.searchDirection = this.aimRay.direction;
                this.enemyFinder.RefreshCandidates();
                HurtBox exists = this.enemyFinder.GetResults().FirstOrDefault<HurtBox>();
                this.lockedOnHurtBox = exists;
                this.foundAnyTarget = exists;
            }
        }

        // Token: 0x06003BE4 RID: 15332 RVA: 0x000F9C3C File Offset: 0x000F7E3C
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.fireStopwatch += Time.fixedDeltaTime;
            this.stopwatch += Time.fixedDeltaTime;
            this.aimRay = base.GetAimRay();
            if (base.isAuthority && !this.lockedOnHurtBox && this.foundAnyTarget)
            {
                this.outer.SetNextState(new FireLaser
                {
                    stopwatch = this.stopwatch
                });
                return;
            }
            Vector3 vector = this.aimRay.origin;
            if (this.muzzleTransform)
            {
                vector = this.muzzleTransform.position;
            }
            Vector3 vector2;
            RaycastHit raycastHit;
            if (this.lockedOnHurtBox)
            {
                vector2 = this.lockedOnHurtBox.transform.position;
            }
            else if (Util.CharacterRaycast(base.gameObject, this.aimRay, out raycastHit, FireLaser.maxDistance, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore))
            {
                vector2 = raycastHit.point;
            }
            else
            {
                vector2 = this.aimRay.GetPoint(FireLaser.maxDistance);
            }
            Ray ray = new Ray(vector, vector2 - vector);
            bool flag = false;
            if (this.laserEffect && this.laserChildLocator)
            {
                RaycastHit raycastHit2;
                if (Util.CharacterRaycast(base.gameObject, ray, out raycastHit2, (vector2 - vector).magnitude, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
                {
                    vector2 = raycastHit2.point;
                    RaycastHit raycastHit3;
                    if (Util.CharacterRaycast(base.gameObject, new Ray(vector2 - ray.direction * 0.1f, -ray.direction), out raycastHit3, raycastHit2.distance, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
                    {
                        vector2 = ray.GetPoint(0.1f);
                        flag = true;
                    }
                }
                this.laserEffect.transform.rotation = Util.QuaternionSafeLookRotation(vector2 - vector);
                this.laserEffectEnd.transform.position = vector2;
            }
            if (this.fireStopwatch > 1f / FireLaser.fireFrequency)
            {
                string targetMuzzle = "MuzzleLaser";
                if (!flag)
                {
                    this.FireBullet(this.modelTransform, ray, targetMuzzle, (vector2 - ray.origin).magnitude + 0.1f);
                }
                this.fireStopwatch -= 1f / FireLaser.fireFrequency;
            }
            if (base.isAuthority && (((!base.inputBank || !base.inputBank.skill4.down) && this.stopwatch > FireLaser.minimumDuration) || this.stopwatch > FireLaser.maximumDuration))
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        // Token: 0x06003BE5 RID: 15333 RVA: 0x0000D2C7 File Offset: 0x0000B4C7
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        // Token: 0x06003BE6 RID: 15334 RVA: 0x000F9F40 File Offset: 0x000F8140
        private void FireBullet(Transform modelTransform, Ray aimRay, string targetMuzzle, float maxDistance)
        {
            if (this.effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(this.effectPrefab, base.gameObject, targetMuzzle, false);
            }
            if (base.isAuthority)
            {
                new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = FireLaser.minSpread,
                    maxSpread = FireLaser.maxSpread,
                    bulletCount = 1U,
                    damage = FireLaser.damageCoefficient * this.damageStat / FireLaser.fireFrequency,
                    force = FireLaser.force,
                    muzzleName = targetMuzzle,
                    hitEffectPrefab = this.hitEffectPrefab,
                    isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
                    procCoefficient = FireLaser.procCoefficientPerTick,
                    HitEffectNormal = false,
                    radius = 0f,
                    maxDistance = maxDistance
                }.Fire();
            }
        }

        // Token: 0x06003BE7 RID: 15335 RVA: 0x000FA048 File Offset: 0x000F8248
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(HurtBoxReference.FromHurtBox(this.lockedOnHurtBox));
            writer.Write(this.stopwatch);
        }

        // Token: 0x06003BE8 RID: 15336 RVA: 0x000FA070 File Offset: 0x000F8270
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