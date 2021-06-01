#define DEBUG

using Chen.GradiusMod.Items.OptionSeed.Components;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using EntityStates.Drone.DroneWeapon;
using EntityStates.Engi.EngiWeapon;
using EntityStates.EngiTurret.EngiTurretWeapon;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;
using EngiMissilePainterFire = EntityStates.Engi.EngiMissilePainter.Fire;

namespace Chen.GradiusMod.Items.OptionSeed
{
    public partial class OptionSeed
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public override void Install()
        {
            base.Install();
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            On.EntityStates.Engi.EngiWeapon.FireGrenades.FireGrenade += FireGrenades_FireGrenade;
            On.EntityStates.Commando.CommandoWeapon.FirePistol2.FireBullet += FirePistol2_FireBullet;
            On.EntityStates.GenericBulletBaseState.FireBullet += GenericBulletBaseState_FireBullet;
            On.EntityStates.Commando.CommandoWeapon.FireBarrage.FireBullet += FireBarrage_FireBullet;
            On.EntityStates.GenericProjectileBaseState.FireProjectile += GenericProjectileBaseState_FireProjectile;
            On.EntityStates.Engi.EngiMissilePainter.Fire.FireMissile += Fire_FireMissile;
            On.EntityStates.EngiTurret.EngiTurretWeapon.FireGauss.OnEnter += FireGauss_OnEnter;
            On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.FixedUpdate += FireBeam_FixedUpdate;
            On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.OnEnter += FireBeam_OnEnter;
            On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.OnExit += FireBeam_OnExit;
#if DEBUG
            On.EntityStates.EntityState.OnEnter += EntityState_OnEnter;
#endif
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.OnInventoryChanged -= CharacterBody_OnInventoryChanged;
            CharacterBody.onBodyStartGlobal -= CharacterBody_onBodyStartGlobal;
            On.EntityStates.Engi.EngiWeapon.FireGrenades.FireGrenade -= FireGrenades_FireGrenade;
            On.EntityStates.Commando.CommandoWeapon.FirePistol2.FireBullet -= FirePistol2_FireBullet;
            On.EntityStates.GenericBulletBaseState.FireBullet -= GenericBulletBaseState_FireBullet;
            On.EntityStates.Commando.CommandoWeapon.FireBarrage.FireBullet -= FireBarrage_FireBullet;
            On.EntityStates.GenericProjectileBaseState.FireProjectile -= GenericProjectileBaseState_FireProjectile;
            On.EntityStates.Engi.EngiMissilePainter.Fire.FireMissile -= Fire_FireMissile;
            On.EntityStates.EngiTurret.EngiTurretWeapon.FireGauss.OnEnter -= FireGauss_OnEnter;
            On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.FixedUpdate -= FireBeam_FixedUpdate;
            On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.OnEnter -= FireBeam_OnEnter;
            On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.OnExit -= FireBeam_OnExit;
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (!self.master) return;
            if (GetCount(self) > 0) SeedTracker.SpawnSeeds(self.gameObject);
            else SeedTracker.DestroySeeds(self.gameObject);
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody obj)
        {
            if (!obj.master) return;
            if (GetCount(obj) > 0) SeedTracker.SpawnSeeds(obj.gameObject);
        }

        private void FireBeam_FixedUpdate(On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.orig_FixedUpdate orig, FireBeam self)
        {
            float seedFireTimer = self.fireTimer + Time.fixedDeltaTime;
            orig(self);
            FireForSeeds(self.characterBody, (seed, tracker) =>
            {
                if (tracker.O[seed].ContainsKey("activated") && (bool)tracker.O[seed]["activated"])
                {
                    Ray laserRay = new Ray(seed.transform.position, self.GetAimRay().direction);
                    float rate = self.fireFrequency * self.characterBody.attackSpeed;
                    float fraction = 1f / rate;
                    if (seedFireTimer > fraction)
                    {
                        if (self.effectPrefab) seed.MuzzleEffect(self.effectPrefab, false);
                        if (self.isAuthority)
                        {
                            BulletAttack bulletAttack = new BulletAttack()
                            {
                                owner = self.gameObject,
                                weapon = seed,
                                origin = laserRay.origin,
                                aimVector = laserRay.direction,
                                minSpread = self.minSpread,
                                maxSpread = self.maxSpread,
                                bulletCount = 1U,
                                damage = self.damageCoefficient * self.damageStat / self.fireFrequency * damageMultiplier,
                                procCoefficient = self.procCoefficient * self.fireFrequency,
                                force = self.force * damageMultiplier,
                                muzzleName = self.muzzleString,
                                hitEffectPrefab = self.hitEffectPrefab,
                                isCrit = self.characterBody.RollCrit(),
                                HitEffectNormal = false,
                                radius = 0f,
                                maxDistance = self.maxDistance
                            };
                            self.ModifyBullet(bulletAttack);
                            if (!mobileTurretsSeedSyncEffect) bulletAttack.tracerEffectPrefab = FireGatling.tracerEffectPrefab;
                            bulletAttack.Fire();
                        }
                    }
                    if (mobileTurretsSeedSyncEffect && tracker.U[seed].SafeCheck("laserEffectInstance") && tracker.U[seed].SafeCheck("laserEffectInstanceEndTransform"))
                    {
                        Vector3 position = ((GameObject)tracker.U[seed]["laserEffectInstance"]).transform.parent.position;
                        Vector3 point = laserRay.GetPoint(self.maxDistance);
                        if (Physics.Raycast(laserRay.origin, laserRay.direction, out RaycastHit raycastHit, self.maxDistance,
                                            LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
                        {
                            point = raycastHit.point;
                        }
                        ((Transform)tracker.U[seed]["laserEffectInstanceEndTransform"]).position = point;
                    }
                }
            });
        }

        private void FireBeam_OnExit(On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.orig_OnExit orig, FireBeam self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, tracker) =>
            {
                if (tracker.O[seed].ContainsKey("activated") && (bool)tracker.O[seed]["activated"])
                {
                    if (tracker.U[seed].SafeCheck("laserEffectInstance")) EntityState.Destroy(tracker.U[seed]["laserEffectInstance"]);
                    if (tracker.U[seed].SafeCheck("laserEffectInstanceEndTransform")) EntityState.Destroy(tracker.U[seed]["laserEffectInstanceEndTransform"]);
                }
            });
        }

        private void FireBeam_OnEnter(On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.orig_OnEnter orig, FireBeam self)
        {
            orig(self);
            if (!mobileTurretsSeedSyncEffect) return;
            FireForSeeds(self.characterBody, (seed, tracker) =>
            {
                if (self.laserPrefab)
                {
                    if (tracker.U[seed].SafeCheck("laserEffectInstance")) EntityState.Destroy(tracker.U[seed]["laserEffectInstance"]);
                    if (tracker.U[seed].SafeCheck("laserEffectInstanceEndTransform")) EntityState.Destroy(tracker.U[seed]["laserEffectInstanceEndTransform"]);
                    Transform transform = seed.transform;
                    tracker.U[seed]["laserEffectInstance"] = Object.Instantiate(self.laserPrefab, transform.position, transform.rotation);
                    ((GameObject)tracker.U[seed]["laserEffectInstance"]).transform.parent = transform;
                    tracker.U[seed]["laserEffectInstanceEndTransform"] = ((GameObject)tracker.U[seed]["laserEffectInstance"]).GetComponent<ChildLocator>().FindChild("LaserEnd");
                }
            }, (chance, seed, tracker) =>
            {
                bool activated = Util.CheckRoll(chance, tracker.characterMaster);
                tracker.O[seed]["activated"] = activated;
                return activated;
            });
        }

        private void FireGauss_OnEnter(On.EntityStates.EngiTurret.EngiTurretWeapon.FireGauss.orig_OnEnter orig, FireGauss self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, _tracker) =>
            {
                if (FireGauss.effectPrefab) seed.MuzzleEffect(FireGauss.effectPrefab, false);
                if (self.isAuthority)
                {
                    new BulletAttack
                    {
                        owner = self.gameObject,
                        weapon = seed,
                        origin = seed.transform.position,
                        aimVector = self.GetAimRay().direction,
                        minSpread = FireGauss.minSpread,
                        maxSpread = FireGauss.maxSpread,
                        bulletCount = 1U,
                        damage = FireGauss.damageCoefficient * self.damageStat * damageMultiplier,
                        force = FireGauss.force * damageMultiplier,
                        tracerEffectPrefab = FireGauss.tracerEffectPrefab,
                        muzzleName = "Muzzle",
                        hitEffectPrefab = FireGauss.hitEffectPrefab,
                        isCrit = Util.CheckRoll(self.critStat, self.characterBody.master),
                        HitEffectNormal = false,
                        radius = 0.1f
                    }.Fire();
                }
            });
        }

        private void Fire_FireMissile(On.EntityStates.Engi.EngiMissilePainter.Fire.orig_FireMissile orig, EngiMissilePainterFire self, HurtBox target, Vector3 position)
        {
            orig(self, target, position);
            FireForSeeds(self.characterBody, (seed, _tracker) =>
            {
                if (EngiMissilePainterFire.muzzleflashEffectPrefab) seed.MuzzleEffect(EngiMissilePainterFire.muzzleflashEffectPrefab, true);
                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo()
                {
                    position = seed.transform.position,
                    rotation = Quaternion.LookRotation(Vector3.up),
                    crit = self.RollCrit(),
                    damage = self.damageStat * EngiMissilePainterFire.damageCoefficient * damageMultiplier,
                    damageColorIndex = DamageColorIndex.Default,
                    owner = self.gameObject,
                    projectilePrefab = EngiMissilePainterFire.projectilePrefab,
                    target = target.gameObject
                };
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);
            });
        }

        private void GenericProjectileBaseState_FireProjectile(On.EntityStates.GenericProjectileBaseState.orig_FireProjectile orig, GenericProjectileBaseState self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, _tracker) =>
            {
                if (self.effectPrefab) seed.MuzzleEffect(self.effectPrefab, false);
                if (self.isAuthority)
                {
                    Ray aimRay = self.GetAimRay();
                    aimRay = self.ModifyProjectileAimRay(aimRay);
                    aimRay.direction = Util.ApplySpread(aimRay.direction, self.minSpread, self.maxSpread, 1f, 1f, 0f, self.projectilePitchBonus);
                    ProjectileManager.instance.FireProjectile(self.projectilePrefab, seed.transform.position, Util.QuaternionSafeLookRotation(aimRay.direction),
                                                              self.gameObject, self.damageStat * self.damageCoefficient * damageMultiplier,
                                                              self.force * damageMultiplier, Util.CheckRoll(self.critStat, self.characterBody.master),
                                                              DamageColorIndex.Default, null, -1f);
                }
            });
        }

        private void FireBarrage_FireBullet(On.EntityStates.Commando.CommandoWeapon.FireBarrage.orig_FireBullet orig, FireBarrage self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, _tracker) =>
            {
                if (FireBarrage.effectPrefab) seed.MuzzleEffect(FireBarrage.effectPrefab, false);
                if (self.isAuthority)
                {
                    new BulletAttack
                    {
                        owner = self.gameObject,
                        weapon = seed,
                        origin = seed.transform.position + (self.GetAimRay().direction * 1.3f),
                        aimVector = self.GetAimRay().direction,
                        minSpread = FireBarrage.minSpread,
                        maxSpread = FireBarrage.maxSpread,
                        bulletCount = 1U,
                        damage = FireBarrage.damageCoefficient * self.damageStat * damageMultiplier,
                        force = FireBarrage.force * damageMultiplier,
                        tracerEffectPrefab = FireBarrage.tracerEffectPrefab,
                        muzzleName = "MuzzleRight",
                        hitEffectPrefab = FireBarrage.hitEffectPrefab,
                        isCrit = Util.CheckRoll(self.critStat, self.characterBody.master),
                        radius = 0.1f,
                        smartCollision = true,
                        damageType = DamageType.Stun1s
                    }.Fire();
                }
            });
        }

        private void GenericBulletBaseState_FireBullet(On.EntityStates.GenericBulletBaseState.orig_FireBullet orig, GenericBulletBaseState self, Ray aimRay)
        {
            orig(self, aimRay);
            FireForSeeds(self.characterBody, (seed, _tracker) =>
            {
                if (self.muzzleFlashPrefab) seed.MuzzleEffect(self.muzzleFlashPrefab, false);
                if (self.isAuthority)
                {
                    Ray seedRay = new Ray(seed.transform.position, aimRay.direction);
                    BulletAttack bulletAttack = self.GenerateBulletAttack(seedRay);
                    bulletAttack.weapon = seed;
                    bulletAttack.damage *= damageMultiplier;
                    bulletAttack.force *= damageMultiplier;
                    bulletAttack.radius = 0.1f;
                    self.ModifyBullet(bulletAttack);
                    bulletAttack.Fire();
                }
            });
        }

        private void FirePistol2_FireBullet(On.EntityStates.Commando.CommandoWeapon.FirePistol2.orig_FireBullet orig, FirePistol2 self, string targetMuzzle)
        {
            orig(self, targetMuzzle);
            FireForSeeds(self.characterBody, (seed, _tracker) =>
            {
                if (FirePistol2.muzzleEffectPrefab) seed.MuzzleEffect(FirePistol2.muzzleEffectPrefab, false);
                if (self.isAuthority)
                {
                    new BulletAttack
                    {
                        owner = self.gameObject,
                        weapon = seed,
                        origin = seed.transform.position,
                        aimVector = self.aimRay.direction,
                        minSpread = 0f,
                        maxSpread = self.characterBody.spreadBloomAngle,
                        damage = FirePistol2.damageCoefficient * self.damageStat * damageMultiplier,
                        force = FirePistol2.force * damageMultiplier,
                        tracerEffectPrefab = FirePistol2.tracerEffectPrefab,
                        muzzleName = targetMuzzle,
                        hitEffectPrefab = FirePistol2.hitEffectPrefab,
                        isCrit = Util.CheckRoll(self.critStat, self.characterBody.master),
                        radius = 0.1f,
                        smartCollision = true
                    }.Fire();
                }
            });
        }

        private void FireGrenades_FireGrenade(On.EntityStates.Engi.EngiWeapon.FireGrenades.orig_FireGrenade orig, FireGrenades self, string targetMuzzle)
        {
            orig(self, targetMuzzle);
            FireForSeeds(self.characterBody, (seed, _tracker) =>
            {
                if (FireGrenades.effectPrefab) seed.MuzzleEffect(FireGrenades.effectPrefab, false);
                if (self.isAuthority)
                {
                    float x = Random.Range(0f, self.characterBody.spreadBloomAngle);
                    float z = Random.Range(0f, 360f);
                    Vector3 up = Vector3.up;
                    Vector3 axis = Vector3.Cross(up, self.projectileRay.direction);
                    Vector3 vector = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
                    float y = vector.y;
                    vector.y = 0f;
                    float angle = Mathf.Atan2(vector.z, vector.x) * 57.29578f - 90f;
                    float angle2 = Mathf.Atan2(y, vector.magnitude) * 57.29578f + FireGrenades.arcAngle;
                    Vector3 forward = Quaternion.AngleAxis(angle, up) * (Quaternion.AngleAxis(angle2, axis) * self.projectileRay.direction);
                    ProjectileManager.instance.FireProjectile(FireGrenades.projectilePrefab, seed.transform.position, Util.QuaternionSafeLookRotation(forward),
                                                              self.gameObject, self.damageStat * FireGrenades.damageCoefficient * damageMultiplier, 0f,
                                                              Util.CheckRoll(self.critStat, self.characterBody.master), DamageColorIndex.Default, null, -1f);
                }
            });
        }

#if DEBUG

        private void EntityState_OnEnter(On.EntityStates.EntityState.orig_OnEnter orig, EntityStates.EntityState self)
        {
            orig(self);
            if (!self.characterBody || !self.characterBody.master) return;
            if (Helpers.GeneralHelpers.Instances.hostMasterObject == self.characterBody.masterObject)
            {
                Log.Message($"EntityState.OnEnter: {self.GetType().FullName}");
            }
        }

#endif

        internal static bool DebugHookCheck()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}