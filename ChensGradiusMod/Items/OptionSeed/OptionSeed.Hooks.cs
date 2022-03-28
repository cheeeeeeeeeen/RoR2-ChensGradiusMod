#undef DEBUG

using Chen.GradiusMod.Items.OptionSeed.Components;
using EntityStates;
using EntityStates.Bandit2.Weapon;
using EntityStates.Captain.Weapon;
using EntityStates.Commando.CommandoWeapon;
using EntityStates.Croco;
using EntityStates.Drone.DroneWeapon;
using EntityStates.Engi.EngiWeapon;
using EntityStates.EngiTurret.EngiTurretWeapon;
using EntityStates.Huntress;
using EntityStates.Huntress.HuntressWeapon;
using EntityStates.Loader;
using EntityStates.Mage.Weapon;
using EntityStates.Merc;
using EntityStates.Toolbot;
using EntityStates.Treebot.Weapon;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using static Chen.GradiusMod.GradiusModPlugin;
using EngiMissilePainterFire = EntityStates.Engi.EngiMissilePainter.Fire;
using MageFlamethrower = EntityStates.Mage.Weapon.Flamethrower;

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
            On.EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.FireOrbArrow += FireSeekingArrow_FireOrbArrow;
            On.EntityStates.Huntress.HuntressWeapon.ThrowGlaive.FireOrbGlaive += ThrowGlaive_FireOrbGlaive;
            On.EntityStates.BasicMeleeAttack.OnEnter += BasicMeleeAttack_OnEnter;
            On.EntityStates.BasicMeleeAttack.AuthorityFireAttack += BasicMeleeAttack_AuthorityFireAttack;
            On.EntityStates.Merc.Evis.FixedUpdate += Evis_FixedUpdate;
            On.EntityStates.Merc.WhirlwindBase.OnEnter += WhirlwindBase_OnEnter;
            On.EntityStates.Merc.WhirlwindBase.FixedUpdate += WhirlwindBase_FixedUpdate;
            On.EntityStates.Merc.FocusedAssaultDash.OnMeleeHitAuthority += FocusedAssaultDash_OnMeleeHitAuthority;
            On.EntityStates.Merc.Uppercut.OnEnter += Uppercut_OnEnter;
            On.EntityStates.Merc.Uppercut.FixedUpdate += Uppercut_FixedUpdate;
            On.EntityStates.Bandit2.Weapon.FireShotgun2.FireBullet += FireShotgun2_FireBullet;
            On.EntityStates.Bandit2.Weapon.BaseFireSidearmRevolverState.OnEnter += BaseFireSidearmRevolverState_OnEnter;
            On.EntityStates.Bandit2.Weapon.Bandit2FireShiv.FireShiv += Bandit2FireShiv_FireShiv;
            On.EntityStates.Toolbot.BaseNailgunState.FireBullet += BaseNailgunState_FireBullet;
            On.EntityStates.AimThrowableBase.FireProjectile += AimThrowableBase_FireProjectile;
            On.EntityStates.Toolbot.RecoverAimStunDrone.OnEnter += RecoverAimStunDrone_OnEnter;
            On.EntityStates.Toolbot.FireBuzzsaw.FixedUpdate += FireBuzzsaw_FixedUpdate;
            On.EntityStates.Mage.Weapon.FireFireBolt.FireGauntlet += FireFireBolt_FireGauntlet;
            On.EntityStates.Mage.Weapon.BaseThrowBombState.Fire += BaseThrowBombState_Fire;
            On.EntityStates.Mage.Weapon.Flamethrower.OnEnter += Flamethrower_OnEnter;
            On.EntityStates.Mage.Weapon.Flamethrower.FireGauntlet += Flamethrower_FireGauntlet;
            On.EntityStates.Mage.Weapon.Flamethrower.FixedUpdate += Flamethrower_FixedUpdate;
            On.EntityStates.Mage.Weapon.Flamethrower.OnExit += Flamethrower_OnExit;
            On.EntityStates.Treebot.Weapon.FireSyringe.FixedUpdate += FireSyringe_FixedUpdate;
            On.EntityStates.Treebot.TreebotFireFruitSeed.OnEnter += TreebotFireFruitSeed_OnEnter;
            On.EntityStates.FireFlower2.OnEnter += FireFlower2_OnEnter;
            On.EntityStates.Huntress.ArrowRain.DoFireArrowRain += ArrowRain_DoFireArrowRain;
            On.EntityStates.Loader.ThrowPylon.OnEnter += ThrowPylon_OnEnter;
            On.EntityStates.Loader.SwingZapFist.OnMeleeHitAuthority += SwingZapFist_OnMeleeHitAuthority;
            On.EntityStates.Loader.GroundSlam.DetonateAuthority += GroundSlam_DetonateAuthority;
            On.EntityStates.Croco.BaseLeap.DetonateAuthority += BaseLeap_DetonateAuthority;
            On.EntityStates.Croco.BaseLeap.DropAcidPoolAuthority += BaseLeap_DropAcidPoolAuthority;
            On.EntityStates.Croco.FireSpit.OnEnter += FireSpit_OnEnter;
            On.EntityStates.Captain.Weapon.FireTazer.Fire += FireTazer_Fire;
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
            On.EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.FireOrbArrow -= FireSeekingArrow_FireOrbArrow;
            On.EntityStates.Huntress.HuntressWeapon.ThrowGlaive.FireOrbGlaive -= ThrowGlaive_FireOrbGlaive;
            On.EntityStates.BasicMeleeAttack.OnEnter -= BasicMeleeAttack_OnEnter;
            On.EntityStates.BasicMeleeAttack.AuthorityFireAttack -= BasicMeleeAttack_AuthorityFireAttack;
            On.EntityStates.Merc.Evis.FixedUpdate -= Evis_FixedUpdate;
            On.EntityStates.Merc.WhirlwindBase.OnEnter -= WhirlwindBase_OnEnter;
            On.EntityStates.Merc.WhirlwindBase.FixedUpdate -= WhirlwindBase_FixedUpdate;
            On.EntityStates.Merc.FocusedAssaultDash.OnMeleeHitAuthority -= FocusedAssaultDash_OnMeleeHitAuthority;
            On.EntityStates.Merc.Uppercut.OnEnter -= Uppercut_OnEnter;
            On.EntityStates.Merc.Uppercut.FixedUpdate -= Uppercut_FixedUpdate;
            On.EntityStates.Bandit2.Weapon.FireShotgun2.FireBullet -= FireShotgun2_FireBullet;
            On.EntityStates.Bandit2.Weapon.BaseFireSidearmRevolverState.OnEnter -= BaseFireSidearmRevolverState_OnEnter;
            On.EntityStates.Bandit2.Weapon.Bandit2FireShiv.FireShiv -= Bandit2FireShiv_FireShiv;
            On.EntityStates.Toolbot.BaseNailgunState.FireBullet -= BaseNailgunState_FireBullet;
            On.EntityStates.AimThrowableBase.FireProjectile -= AimThrowableBase_FireProjectile;
            On.EntityStates.Toolbot.RecoverAimStunDrone.OnEnter -= RecoverAimStunDrone_OnEnter;
            On.EntityStates.Toolbot.FireBuzzsaw.FixedUpdate -= FireBuzzsaw_FixedUpdate;
            On.EntityStates.Mage.Weapon.FireFireBolt.FireGauntlet -= FireFireBolt_FireGauntlet;
            On.EntityStates.Mage.Weapon.BaseThrowBombState.Fire -= BaseThrowBombState_Fire;
            On.EntityStates.Mage.Weapon.Flamethrower.OnEnter -= Flamethrower_OnEnter;
            On.EntityStates.Mage.Weapon.Flamethrower.FireGauntlet -= Flamethrower_FireGauntlet;
            On.EntityStates.Mage.Weapon.Flamethrower.FixedUpdate -= Flamethrower_FixedUpdate;
            On.EntityStates.Mage.Weapon.Flamethrower.OnExit -= Flamethrower_OnExit;
            On.EntityStates.Treebot.Weapon.FireSyringe.FixedUpdate -= FireSyringe_FixedUpdate;
            On.EntityStates.Treebot.TreebotFireFruitSeed.OnEnter -= TreebotFireFruitSeed_OnEnter;
            On.EntityStates.FireFlower2.OnEnter -= FireFlower2_OnEnter;
            On.EntityStates.Huntress.ArrowRain.DoFireArrowRain -= ArrowRain_DoFireArrowRain;
            On.EntityStates.Loader.ThrowPylon.OnEnter -= ThrowPylon_OnEnter;
            On.EntityStates.Loader.SwingZapFist.OnMeleeHitAuthority -= SwingZapFist_OnMeleeHitAuthority;
            On.EntityStates.Loader.GroundSlam.DetonateAuthority -= GroundSlam_DetonateAuthority;
            On.EntityStates.Croco.BaseLeap.DetonateAuthority -= BaseLeap_DetonateAuthority;
            On.EntityStates.Croco.BaseLeap.DropAcidPoolAuthority -= BaseLeap_DropAcidPoolAuthority;
            On.EntityStates.Croco.FireSpit.OnEnter -= FireSpit_OnEnter;
            On.EntityStates.Captain.Weapon.FireTazer.Fire -= FireTazer_Fire;
#if DEBUG
            On.EntityStates.EntityState.OnEnter -= EntityState_OnEnter;
#endif
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

        private void FireTazer_Fire(On.EntityStates.Captain.Weapon.FireTazer.orig_Fire orig, FireTazer self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (FireTazer.muzzleflashEffectPrefab) seed.MuzzleEffect(FireTazer.muzzleflashEffectPrefab, false);
                if (self.isAuthority)
                {
                    FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                    {
                        projectilePrefab = FireTazer.projectilePrefab,
                        position = seed.transform.position,
                        rotation = Util.QuaternionSafeLookRotation(self.GetAimRay().direction),
                        owner = self.gameObject,
                        damage = self.damageStat * FireTazer.damageCoefficient * multiplier,
                        force = FireTazer.force * multiplier,
                        crit = Util.CheckRoll(self.critStat, self.characterBody.master)
                    };
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                }
            });
        }

        private void FireSpit_OnEnter(On.EntityStates.Croco.FireSpit.orig_OnEnter orig, FireSpit self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (self.effectPrefab) seed.MuzzleEffect(self.effectPrefab, false);
                if (self.isAuthority)
                {
                    DamageType value = self.crocoDamageTypeController ? self.crocoDamageTypeController.GetDamageType() : DamageType.Generic;
                    FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                    {
                        projectilePrefab = self.projectilePrefab,
                        position = seed.transform.position,
                        rotation = Util.QuaternionSafeLookRotation(self.GetAimRay().direction),
                        owner = self.gameObject,
                        damage = self.damageStat * self.damageCoefficient * multiplier,
                        damageTypeOverride = new DamageType?(value),
                        force = self.force * multiplier,
                        crit = Util.CheckRoll(self.critStat, self.characterBody.master)
                    };
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                }
            });
        }

        private void BaseLeap_DropAcidPoolAuthority(On.EntityStates.Croco.BaseLeap.orig_DropAcidPoolAuthority orig, BaseLeap self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                Vector3 footPosition = seed.transform.position;
                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                {
                    projectilePrefab = BaseLeap.projectilePrefab,
                    crit = self.isCritAuthority,
                    force = 0f,
                    damage = self.damageStat * multiplier,
                    owner = self.gameObject,
                    rotation = Quaternion.identity,
                    position = footPosition
                };
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);
            });
        }

        private BlastAttack.Result BaseLeap_DetonateAuthority(On.EntityStates.Croco.BaseLeap.orig_DetonateAuthority orig, BaseLeap self)
        {
            BlastAttack.Result result = orig(self);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                new BlastAttack
                {
                    attacker = self.gameObject,
                    baseDamage = self.damageStat * self.blastDamageCoefficient * multiplier,
                    baseForce = self.blastForce * multiplier,
                    bonusForce = self.blastBonusForce,
                    crit = self.RollCrit(),
                    damageType = self.GetBlastDamageType(),
                    falloffModel = BlastAttack.FalloffModel.None,
                    procCoefficient = BaseLeap.blastProcCoefficient,
                    radius = BaseLeap.blastRadius,
                    position = seed.transform.position,
                    attackerFiltering = AttackerFiltering.NeverHitSelf,
                    impactEffect = EffectCatalog.FindEffectIndexFromPrefab(self.blastImpactEffectPrefab),
                    teamIndex = self.teamComponent.teamIndex
                }.Fire();
            });
            return result;
        }

        private BlastAttack.Result GroundSlam_DetonateAuthority(On.EntityStates.Loader.GroundSlam.orig_DetonateAuthority orig, GroundSlam self)
        {
            BlastAttack.Result result = orig(self);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                new BlastAttack
                {
                    attacker = self.gameObject,
                    baseDamage = self.damageStat * GroundSlam.blastDamageCoefficient * multiplier,
                    baseForce = GroundSlam.blastForce * multiplier,
                    bonusForce = GroundSlam.blastBonusForce,
                    crit = self.RollCrit(),
                    damageType = DamageType.Stun1s,
                    falloffModel = BlastAttack.FalloffModel.None,
                    procCoefficient = GroundSlam.blastProcCoefficient,
                    radius = GroundSlam.blastRadius,
                    position = seed.transform.position,
                    attackerFiltering = AttackerFiltering.NeverHitSelf,
                    impactEffect = EffectCatalog.FindEffectIndexFromPrefab(GroundSlam.blastImpactEffectPrefab),
                    teamIndex = self.teamComponent.teamIndex
                }.Fire();
            });
            return result;
        }

        private void SwingZapFist_OnMeleeHitAuthority(On.EntityStates.Loader.SwingZapFist.orig_OnMeleeHitAuthority orig, SwingZapFist self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (self.FindModelChild(self.swingEffectMuzzleString))
                {
                    FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                    {
                        position = seed.transform.position,
                        rotation = Quaternion.LookRotation(self.punchVelocity),
                        crit = self.isCritAuthority,
                        damage = self.damageStat * multiplier,
                        owner = self.gameObject,
                        projectilePrefab = loaderZapCone
                    };
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                }
            });
        }

        private void ThrowPylon_OnEnter(On.EntityStates.Loader.ThrowPylon.orig_OnEnter orig, ThrowPylon self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (ThrowPylon.muzzleflashObject) seed.MuzzleEffect(ThrowPylon.muzzleflashObject, false);
                if (self.isAuthority)
                {
                    Ray aimRay = self.GetAimRay();
                    FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                    {
                        crit = self.RollCrit(),
                        damage = self.damageStat * ThrowPylon.damageCoefficient * multiplier,
                        damageColorIndex = DamageColorIndex.Default,
                        force = 0f,
                        owner = self.gameObject,
                        position = seed.transform.position,
                        procChainMask = default,
                        projectilePrefab = ThrowPylon.projectilePrefab,
                        rotation = Quaternion.LookRotation(aimRay.direction),
                        target = null
                    };
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                }
            });
        }

        private void ArrowRain_DoFireArrowRain(On.EntityStates.Huntress.ArrowRain.orig_DoFireArrowRain orig, ArrowRain self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (ArrowRain.muzzleFlashEffect) seed.MuzzleEffect(ArrowRain.muzzleFlashEffect, false);
                if (self.isAuthority && self.areaIndicatorInstance && self.shouldFireArrowRain)
                {
                    ProjectileManager.instance.FireProjectile(ArrowRain.projectilePrefab, self.areaIndicatorInstance.transform.position,
                                                              self.areaIndicatorInstance.transform.rotation, self.gameObject,
                                                              self.damageStat * ArrowRain.damageCoefficient * multiplier, 0f,
                                                              Util.CheckRoll(self.critStat, self.characterBody.master), DamageColorIndex.Default, null, -1f);
                }
            });
        }

        private void FireFlower2_OnEnter(On.EntityStates.FireFlower2.orig_OnEnter orig, FireFlower2 self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (FireFlower2.muzzleFlashPrefab) seed.MuzzleEffect(FireFlower2.muzzleFlashPrefab, false);
                if (self.isAuthority)
                {
                    Ray aimRay = self.GetAimRay();
                    FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                    {
                        crit = self.RollCrit(),
                        damage = FireFlower2.damageCoefficient * self.damageStat * multiplier,
                        damageColorIndex = DamageColorIndex.Default,
                        force = 0f,
                        owner = self.gameObject,
                        position = seed.transform.position,
                        procChainMask = default,
                        projectilePrefab = FireFlower2.projectilePrefab,
                        rotation = Quaternion.LookRotation(aimRay.direction),
                        useSpeedOverride = false
                    };
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                }
            });
        }

        private void TreebotFireFruitSeed_OnEnter(On.EntityStates.Treebot.TreebotFireFruitSeed.orig_OnEnter orig, EntityStates.Treebot.TreebotFireFruitSeed self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (self.muzzleFlashPrefab) seed.MuzzleEffect(self.muzzleFlashPrefab, false);
                if (self.isAuthority)
                {
                    Ray aimRay = self.GetAimRay();
                    FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                    {
                        crit = self.RollCrit(),
                        damage = self.damageCoefficient * self.damageStat * multiplier,
                        damageColorIndex = DamageColorIndex.Default,
                        force = 0f,
                        owner = self.gameObject,
                        position = seed.transform.position,
                        procChainMask = default,
                        projectilePrefab = self.projectilePrefab,
                        rotation = Quaternion.LookRotation(aimRay.direction),
                        useSpeedOverride = false
                    };
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                }
            });
        }

        private void FireSyringe_FixedUpdate(On.EntityStates.Treebot.Weapon.FireSyringe.orig_FixedUpdate orig, FireSyringe self)
        {
            int projectilesFired = self.projectilesFired;
            orig(self);
            int rate = Mathf.FloorToInt(self.fixedAge / self.fireDuration * FireSyringe.projectileCount);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (projectilesFired <= rate && projectilesFired < FireSyringe.projectileCount)
                {
                    GameObject prefab = FireSyringe.projectilePrefab;
                    string soundString = FireSyringe.attackSound;
                    if (projectilesFired == FireSyringe.projectileCount - 1)
                    {
                        prefab = FireSyringe.finalProjectilePrefab;
                        soundString = FireSyringe.finalAttackSound;
                    }
                    if (FireSyringe.muzzleflashEffectPrefab) seed.MuzzleEffect(FireSyringe.muzzleflashEffectPrefab, false);
                    if (self.isAuthority)
                    {
                        Ray aimRay = self.GetAimRay();
                        float bonusYaw = self.projectilesFired - (FireSyringe.projectileCount - 1) / 2f;
                        bonusYaw = Mathf.FloorToInt(bonusYaw) / (FireSyringe.projectileCount - 1) * FireSyringe.totalYawSpread;
                        Vector3 forward = Util.ApplySpread(aimRay.direction, 0f, 0f, 1f, 1f, bonusYaw, 0f);
                        ProjectileManager.instance.FireProjectile(prefab, seed.transform.position, Util.QuaternionSafeLookRotation(forward), self.gameObject,
                                                                  self.damageStat * FireSyringe.damageCoefficient * multiplier, FireSyringe.force * multiplier,
                                                                  Util.CheckRoll(self.critStat, self.characterBody.master), DamageColorIndex.Default, null, -1f);
                    }
                }
            });
        }

        private void Flamethrower_OnEnter(On.EntityStates.Mage.Weapon.Flamethrower.orig_OnEnter orig, MageFlamethrower self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (_s, behavior, _t, _m) => behavior.O[$"{self.GetType().FullName}.IsCrit"] = self.RollCrit());
        }

        private void Flamethrower_OnExit(On.EntityStates.Mage.Weapon.Flamethrower.orig_OnExit orig, MageFlamethrower self)
        {
            orig(self);
            if (!flamethrowerSeedSyncEffect) return;
            FireForSeeds(self.characterBody, (_s, behavior, _t, _m) =>
            {
                string flamethrowerKey = $"{self.GetType().FullName}.flamethrower";
                if (behavior.U.SafeCheck(flamethrowerKey)) EntityState.Destroy(behavior.U[flamethrowerKey]);
            });
        }

        private void Flamethrower_FixedUpdate(On.EntityStates.Mage.Weapon.Flamethrower.orig_FixedUpdate orig, MageFlamethrower self)
        {
            bool oldBegunFlamethrower = self.hasBegunFlamethrower;
            orig(self);
            if (!flamethrowerSeedSyncEffect) return;
            FireForSeeds(self.characterBody, (seed, behavior, _t, _m) =>
            {
                string flamethrowerKey = $"{self.GetType().FullName}.flamethrower";
                bool perMinionOldBegunFlamethrower = oldBegunFlamethrower;
                if (self.stopwatch >= self.entryDuration && !perMinionOldBegunFlamethrower)
                {
                    perMinionOldBegunFlamethrower = true;
                    if (behavior.U.SafeCheck(flamethrowerKey)) EntityState.Destroy(behavior.U[flamethrowerKey]);
                    behavior.U[flamethrowerKey] = Object.Instantiate(self.flamethrowerEffectPrefab, seed.transform);
                    ((GameObject)behavior.U[flamethrowerKey]).GetComponent<ScaleParticleSystemDuration>().newDuration = self.flamethrowerDuration;
                }
                if (perMinionOldBegunFlamethrower && behavior.U[flamethrowerKey])
                {
                    ((GameObject)behavior.U[flamethrowerKey]).transform.forward = self.GetAimRay().direction;
                }
            });
        }

        private void Flamethrower_FireGauntlet(On.EntityStates.Mage.Weapon.Flamethrower.orig_FireGauntlet orig, MageFlamethrower self, string muzzleString)
        {
            orig(self, muzzleString);
            FireForSeeds(self.characterBody, (seed, behavior, _t, multiplier) =>
            {
                if (self.isAuthority)
                {
                    BulletAttack attack = new BulletAttack
                    {
                        owner = self.gameObject,
                        weapon = seed,
                        origin = seed.transform.position,
                        aimVector = self.GetAimRay().direction,
                        minSpread = 0f,
                        damage = self.tickDamageCoefficient * self.damageStat * multiplier,
                        force = MageFlamethrower.force * multiplier,
                        muzzleName = muzzleString,
                        hitEffectPrefab = MageFlamethrower.impactEffectPrefab,
                        isCrit = (bool)behavior.O[$"{self.GetType().FullName}.IsCrit"],
                        falloffModel = BulletAttack.FalloffModel.None,
                        stopperMask = LayerIndex.world.mask,
                        procCoefficient = MageFlamethrower.procCoefficientPerTick,
                        maxDistance = self.maxDistance,
                        damageType = Util.CheckRoll(MageFlamethrower.ignitePercentChance, self.characterBody.master) ? DamageType.IgniteOnHit : DamageType.Generic,
                        radius = MageFlamethrower.radius,
                        smartCollision = true
                    };
                    if (!flamethrowerSeedSyncEffect) attack.tracerEffectPrefab = FireGatling.tracerEffectPrefab;
                    attack.FilterOutOwnerFromAttack();
                    attack.Fire();
                }
            });
        }

        private void BaseThrowBombState_Fire(On.EntityStates.Mage.Weapon.BaseThrowBombState.orig_Fire orig, BaseThrowBombState self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (self.isAuthority && self.projectilePrefab)
                {
                    float damage = Util.Remap(self.charge, 0f, 1f, self.minDamageCoefficient, self.maxDamageCoefficient) * self.damageStat;
                    float force = self.charge * self.force;
                    FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                    {
                        projectilePrefab = self.projectilePrefab,
                        position = seed.transform.position,
                        rotation = Util.QuaternionSafeLookRotation(self.GetAimRay().direction),
                        owner = self.gameObject,
                        damage = damage,
                        force = force,
                        crit = self.RollCrit()
                    };
                    self.ModifyProjectile(ref fireProjectileInfo);
                    fireProjectileInfo.damage *= multiplier;
                    fireProjectileInfo.force *= multiplier;
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                }
            });
        }

        private void FireFireBolt_FireGauntlet(On.EntityStates.Mage.Weapon.FireFireBolt.orig_FireGauntlet orig, FireFireBolt self)
        {
            bool hasFiredGauntlet = self.hasFiredGauntlet;
            orig(self);
            if (hasFiredGauntlet) return;
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (self.muzzleflashEffectPrefab) seed.MuzzleEffect(self.muzzleflashEffectPrefab, false);
                if (self.isAuthority)
                {
                    ProjectileManager.instance.FireProjectile(self.projectilePrefab, seed.transform.position, Util.QuaternionSafeLookRotation(self.GetAimRay().direction),
                                                              self.gameObject, self.damageCoefficient * self.damageStat * multiplier, 0f, self.RollCrit(),
                                                              DamageColorIndex.Default, null, -1f);
                }
            });
        }

        private void FireBuzzsaw_FixedUpdate(On.EntityStates.Toolbot.FireBuzzsaw.orig_FixedUpdate orig, FireBuzzsaw self)
        {
            float fireAge = self.fireAge + Time.fixedDeltaTime;
            orig(self);
            FireForSeeds(self.characterBody, (_s, _b, _t, multiplier) =>
            {
                if (self.isAuthority && fireAge >= 1f / self.fireFrequency)
                {
                    OverlapAttack attack = new OverlapAttack
                    {
                        attacker = self.gameObject,
                        inflictor = self.gameObject,
                        teamIndex = TeamComponent.GetObjectTeam(self.attack.attacker),
                        damage = FireBuzzsaw.damageCoefficientPerSecond * self.damageStat / FireBuzzsaw.baseFireFrequency * multiplier,
                        procCoefficient = FireBuzzsaw.procCoefficientPerSecond / FireBuzzsaw.baseFireFrequency,
                        hitBoxGroup = self.attack.hitBoxGroup,
                        isCrit = self.characterBody.RollCrit()
                    };
                    if (FireBuzzsaw.impactEffectPrefab) attack.hitEffectPrefab = FireBuzzsaw.impactEffectPrefab;
                    attack.ResetIgnoredHealthComponents();
                    attack.Fire();
                }
            });
        }

        private void AimThrowableBase_FireProjectile(On.EntityStates.AimThrowableBase.orig_FireProjectile orig, AimThrowableBase self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                {
                    crit = self.RollCrit(),
                    owner = self.gameObject,
                    position = seed.transform.position,
                    projectilePrefab = self.projectilePrefab,
                    rotation = Util.QuaternionSafeLookRotation(self.currentTrajectoryInfo.finalRay.direction, Vector3.up),
                    speedOverride = self.currentTrajectoryInfo.speedOverride,
                    damage = self.damageCoefficient * self.damageStat
                };
                if (self.setFuse) fireProjectileInfo.fuseOverride = self.currentTrajectoryInfo.travelTime;
                self.ModifyProjectile(ref fireProjectileInfo);
                fireProjectileInfo.force *= multiplier;
                fireProjectileInfo.damage *= multiplier;
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);
            });
        }

        private void RecoverAimStunDrone_OnEnter(On.EntityStates.Toolbot.RecoverAimStunDrone.orig_OnEnter orig, RecoverAimStunDrone self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, _b, _t, _m) =>
            {
                if (RecoverAimStunDrone.muzzleEffectPrefab) seed.MuzzleEffect(RecoverAimStunDrone.muzzleEffectPrefab, false);
            });
        }

        private void BaseNailgunState_FireBullet(On.EntityStates.Toolbot.BaseNailgunState.orig_FireBullet orig, BaseNailgunState self,
                                                 Ray aimRay, int bulletCount, float spreadPitchScale, float spreadYawScale)
        {
            orig(self, aimRay, bulletCount, spreadPitchScale, spreadYawScale);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (BaseNailgunState.muzzleFlashPrefab) seed.MuzzleEffect(BaseNailgunState.muzzleFlashPrefab, false);
                if (self.isAuthority)
                {
                    BulletAttack attack = new BulletAttack
                    {
                        aimVector = aimRay.direction,
                        origin = seed.transform.position,
                        owner = self.gameObject,
                        weapon = seed,
                        bulletCount = (uint)bulletCount,
                        damage = self.damageStat * BaseNailgunState.damageCoefficient * multiplier,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.Generic,
                        falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                        force = BaseNailgunState.force * multiplier,
                        HitEffectNormal = false,
                        procChainMask = default,
                        procCoefficient = BaseNailgunState.procCoefficient,
                        maxDistance = BaseNailgunState.maxDistance,
                        isCrit = self.RollCrit(),
                        muzzleName = "Muzzle",
                        minSpread = 0f,
                        hitEffectPrefab = BaseNailgunState.hitEffectPrefab,
                        maxSpread = self.characterBody.spreadBloomAngle,
                        smartCollision = false,
                        sniper = false,
                        spreadPitchScale = spreadPitchScale * spreadPitchScale,
                        spreadYawScale = spreadYawScale * spreadYawScale,
                        tracerEffectPrefab = BaseNailgunState.tracerEffectPrefab,
                        radius = 0f
                    };
                    attack.FilterOutOwnerFromAttack();
                    attack.Fire();
                }
            });
        }

        private void Bandit2FireShiv_FireShiv(On.EntityStates.Bandit2.Weapon.Bandit2FireShiv.orig_FireShiv orig, Bandit2FireShiv self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (Bandit2FireShiv.muzzleEffectPrefab) seed.MuzzleEffect(Bandit2FireShiv.muzzleEffectPrefab, false);
                if (self.isAuthority && self.projectilePrefab)
                {
                    FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                    {
                        projectilePrefab = self.projectilePrefab,
                        position = seed.transform.position,
                        rotation = Util.QuaternionSafeLookRotation(self.GetAimRay().direction),
                        owner = self.gameObject,
                        damage = self.damageStat * self.damageCoefficient * multiplier,
                        force = self.force * multiplier,
                        crit = self.RollCrit(),
                        damageTypeOverride = new DamageType?(DamageType.SuperBleedOnCrit)
                    };
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                }
            });
        }

        private void BaseFireSidearmRevolverState_OnEnter(On.EntityStates.Bandit2.Weapon.BaseFireSidearmRevolverState.orig_OnEnter orig, BaseFireSidearmRevolverState self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (self.effectPrefab) seed.MuzzleEffect(self.effectPrefab, false);
                if (self.isAuthority)
                {
                    BulletAttack bulletAttack = new BulletAttack
                    {
                        owner = self.gameObject,
                        weapon = seed,
                        origin = seed.transform.position,
                        aimVector = self.GetAimRay().direction,
                        minSpread = self.minSpread,
                        maxSpread = self.maxSpread,
                        bulletCount = 1U,
                        damage = self.damageCoefficient * self.damageStat,
                        force = self.force,
                        falloffModel = BulletAttack.FalloffModel.None,
                        tracerEffectPrefab = self.tracerEffectPrefab,
                        muzzleName = "MuzzlePistol",
                        hitEffectPrefab = self.hitEffectPrefab,
                        isCrit = self.RollCrit(),
                        HitEffectNormal = false,
                        smartCollision = true,
                        radius = self.bulletRadius
                    };
                    bulletAttack.damageType |= DamageType.BonusToLowHealth;
                    self.ModifyBullet(bulletAttack);
                    bulletAttack.damage *= multiplier;
                    bulletAttack.force *= multiplier;
                    bulletAttack.FilterOutOwnerFromAttack();
                    bulletAttack.Fire();
                }
            });
        }

        private void FireShotgun2_FireBullet(On.EntityStates.Bandit2.Weapon.FireShotgun2.orig_FireBullet orig, FireShotgun2 self, Ray aimRay)
        {
            orig(self, aimRay);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (self.muzzleFlashPrefab) seed.MuzzleEffect(self.muzzleFlashPrefab, false);
                if (self.isAuthority)
                {
                    Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
                    Vector3 axis = Vector3.Cross(aimRay.direction, rhs);
                    float spread = 0f;
                    if (self.characterBody)
                    {
                        spread = self.characterBody.spreadBloomAngle;
                    }
                    float angle = 0f;
                    float computedValue = 0f;
                    if (self.bulletCount > 1)
                    {
                        computedValue = Random.Range(self.minFixedSpreadYaw + spread, self.maxFixedSpreadYaw + spread) * 2f;
                        angle = computedValue / (self.bulletCount - 1f);
                    }
                    Vector3 direction = Quaternion.AngleAxis(-computedValue * .5f, axis) * aimRay.direction;
                    Quaternion rotation = Quaternion.AngleAxis(angle, axis);
                    Ray bulletRay = new Ray(seed.transform.position, direction);
                    for (int i = 0; i < self.bulletCount; i++)
                    {
                        BulletAttack bulletAttack = self.GenerateBulletAttack(bulletRay);
                        self.ModifyBullet(bulletAttack);
                        bulletAttack.force *= multiplier;
                        bulletAttack.damage *= multiplier;
                        bulletAttack.weapon = seed;
                        bulletAttack.FilterOutOwnerFromAttack();
                        bulletAttack.Fire();
                        bulletRay.direction = rotation * bulletRay.direction;
                    }
                }
            });
        }

        private void Uppercut_OnEnter(On.EntityStates.Merc.Uppercut.orig_OnEnter orig, Uppercut self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (_s, behavior, _t, multiplier) =>
            {
                behavior.O["Uppercut.OverlapAttack"] = self.InitMeleeOverlap(Uppercut.baseDamageCoefficient, Uppercut.hitEffectPrefab, self.GetModelTransform(), Uppercut.hitboxString);
                ((OverlapAttack)behavior.O["Uppercut.OverlapAttack"]).forceVector = Uppercut.upwardForceStrength * multiplier * Vector3.up;
                ((OverlapAttack)behavior.O["Uppercut.OverlapAttack"]).damage *= multiplier;
            });
        }

        private void Uppercut_FixedUpdate(On.EntityStates.Merc.Uppercut.orig_FixedUpdate orig, Uppercut self)
        {
            bool hasSwung = self.hasSwung;
            orig(self);
            FireForSeeds(self.characterBody, (_s, behavior, _t, _m) =>
            {
                if (self.isAuthority)
                {
                    bool localHasSwung = hasSwung;
                    if (self.animator.GetFloat("Sword.active") > 0.2f && !localHasSwung)
                    {
                        localHasSwung = true;
                    }
                    self.FireMeleeOverlap((OverlapAttack)behavior.O["Uppercut.OverlapAttack"], self.animator, "Sword.active", 0f, false);
                    if (self.fixedAge >= self.duration)
                    {
                        if (localHasSwung)
                        {
                            localHasSwung = true;
                            ((OverlapAttack)behavior.O["Uppercut.OverlapAttack"]).Fire();
                        }
                    }
                }
            });
        }

        private void FocusedAssaultDash_OnMeleeHitAuthority(On.EntityStates.Merc.FocusedAssaultDash.orig_OnMeleeHitAuthority orig, FocusedAssaultDash self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (_s, _b, _t, multiplier) =>
            {
                foreach (HurtBox victimHurtBox in self.hitResults)
                {
                    self.currentHitCount++;
                    float damageValue = self.characterBody.damage * self.delayedDamageCoefficient * multiplier;
                    float actualDelay = self.delay + self.delayPerHit * self.currentHitCount;
                    bool isCrit = self.RollCrit();
                    FocusedAssaultDash.HandleHit(self.gameObject, victimHurtBox, damageValue, self.delayedProcCoefficient, isCrit, actualDelay, self.orbEffect, self.delayedEffectPrefab);
                }
            });
        }

        private void WhirlwindBase_OnEnter(On.EntityStates.Merc.WhirlwindBase.orig_OnEnter orig, WhirlwindBase self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (_s, behavior, _t, multiplier) =>
            {
                behavior.O["Whirlwind.OverlapAttack"] = self.InitMeleeOverlap(self.baseDamageCoefficient, WhirlwindBase.hitEffectPrefab, self.GetModelTransform(), self.hitboxString);
                ((OverlapAttack)behavior.O["Whirlwind.OverlapAttack"]).damage *= multiplier;
            });
        }

        private void WhirlwindBase_FixedUpdate(On.EntityStates.Merc.WhirlwindBase.orig_FixedUpdate orig, WhirlwindBase self)
        {
            int swingCount = self.swingCount;
            orig(self);
            FireForSeeds(self.characterBody, (_s, behavior, _t, _m) =>
            {
                int localSwingCount = swingCount;
                if (self.animator.GetFloat("Sword.active") > localSwingCount)
                {
                    localSwingCount++;
                    if (self.isAuthority) ((OverlapAttack)behavior.O["Whirlwind.OverlapAttack"]).ResetIgnoredHealthComponents();
                }
                if (self.isAuthority)
                {
                    self.FireMeleeOverlap((OverlapAttack)behavior.O["Whirlwind.OverlapAttack"], self.animator, "Sword.active", 0f, true);
                    if (self.fixedAge >= self.duration)
                    {
                        while (localSwingCount < 2)
                        {
                            localSwingCount++;
                            ((OverlapAttack)behavior.O["Whirlwind.OverlapAttack"]).Fire();
                        }
                    }
                }
            });
        }

        private void Evis_FixedUpdate(On.EntityStates.Merc.Evis.orig_FixedUpdate orig, Evis self)
        {
            float attackStopwatch = self.attackStopwatch + Time.fixedDeltaTime;
            float tick = 1f / Evis.damageFrequency / self.attackSpeedStat;
            orig(self);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (attackStopwatch >= tick)
                {
                    BullseyeSearch bullseyeSearch = new BullseyeSearch
                    {
                        searchOrigin = seed.transform.position,
                        searchDirection = Random.onUnitSphere,
                        maxDistanceFilter = Evis.maxRadius,
                        teamMaskFilter = TeamMask.GetUnprotectedTeams(self.GetTeam()),
                        sortMode = BullseyeSearch.SortMode.Distance
                    };
                    bullseyeSearch.RefreshCandidates();
                    bullseyeSearch.FilterOutGameObject(self.gameObject);
                    HurtBox hurtBox = bullseyeSearch.GetResults().FirstOrDefault();
                    if (hurtBox)
                    {
                        HurtBoxGroup hurtBoxGroup = hurtBox.hurtBoxGroup;
                        HurtBox chosenHurtbox = hurtBoxGroup.hurtBoxes[Random.Range(0, hurtBoxGroup.hurtBoxes.Length - 1)];
                        if (chosenHurtbox)
                        {
                            Vector3 position = chosenHurtbox.transform.position;
                            Vector2 normalized = Random.insideUnitCircle.normalized;
                            Vector3 normal = new Vector3(normalized.x, 0f, normalized.y);
                            EffectManager.SimpleImpactEffect(Evis.hitEffectPrefab, position, normal, false);
                            Transform transform = hurtBox.hurtBoxGroup.transform;
                            TemporaryOverlay temporaryOverlay = transform.gameObject.AddComponent<TemporaryOverlay>();
                            temporaryOverlay.duration = tick;
                            temporaryOverlay.animateShaderAlpha = true;
                            temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                            temporaryOverlay.destroyComponentOnEnd = true;
                            temporaryOverlay.originalMaterial = evisTargetMaterial;
                            temporaryOverlay.AddToCharacerModel(transform.GetComponent<CharacterModel>());
                            if (NetworkServer.active)
                            {
                                DamageInfo damageInfo = new DamageInfo
                                {
                                    damage = Evis.damageCoefficient * self.damageStat * multiplier,
                                    attacker = self.gameObject,
                                    procCoefficient = Evis.procCoefficient,
                                    position = chosenHurtbox.transform.position,
                                    crit = self.crit
                                };
                                chosenHurtbox.healthComponent.TakeDamage(damageInfo);
                                GlobalEventManager.instance.OnHitEnemy(damageInfo, chosenHurtbox.healthComponent.gameObject);
                                GlobalEventManager.instance.OnHitAll(damageInfo, chosenHurtbox.healthComponent.gameObject);
                            }
                        }
                    }
                }
            });
        }

        private void BasicMeleeAttack_AuthorityFireAttack(On.EntityStates.BasicMeleeAttack.orig_AuthorityFireAttack orig, BasicMeleeAttack self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (_s, behavior, _t, multiplier) =>
            {
                OverlapAttack attack = (OverlapAttack)behavior.O[$"{self.GetType().FullName}.OverlapAttack"];
                self.AuthorityModifyOverlapAttack(attack);
                attack.pushAwayForce *= multiplier;
                attack.damage *= multiplier;
                attack.Fire();
            });
        }

        private void BasicMeleeAttack_OnEnter(On.EntityStates.BasicMeleeAttack.orig_OnEnter orig, BasicMeleeAttack self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (_s, behavior, _t, _m) =>
            {
                if (self.isAuthority && self.hitBoxGroup)
                {
                    OverlapAttack overlapAttack = new OverlapAttack
                    {
                        attacker = self.gameObject,
                        damage = self.damageCoefficient * self.damageStat,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.Generic,
                        forceVector = self.forceVector,
                        hitBoxGroup = self.hitBoxGroup,
                        hitEffectPrefab = self.hitEffectPrefab,
                        inflictor = self.gameObject,
                        isCrit = self.characterBody.RollCrit(),
                        procChainMask = default,
                        pushAwayForce = self.pushAwayForce,
                        procCoefficient = self.procCoefficient,
                        teamIndex = self.GetTeam()
                    };
                    behavior.O[$"{self.GetType().FullName}.OverlapAttack"] = overlapAttack;
                }
            });
        }

        private void ThrowGlaive_FireOrbGlaive(On.EntityStates.Huntress.HuntressWeapon.ThrowGlaive.orig_FireOrbGlaive orig, ThrowGlaive self)
        {
            bool hasTriedToThrowGlaive = self.hasTriedToThrowGlaive;
            orig(self);
            if (!NetworkServer.active || hasTriedToThrowGlaive) return;
            FireForSeeds(self.characterBody, (seed, behavior, _t, multiplier) =>
            {
                float speedMultiplier = 1f;
                switch (behavior.numbering)
                {
                    case 1:
                        speedMultiplier = .8f;
                        break;

                    case -1:
                        speedMultiplier = .6f;
                        break;

                    default:
                        break;
                }
                LightningOrb lightningOrb = new LightningOrb
                {
                    lightningType = LightningOrb.LightningType.HuntressGlaive,
                    damageValue = self.characterBody.damage * ThrowGlaive.damageCoefficient * multiplier,
                    isCrit = Util.CheckRoll(self.characterBody.crit, self.characterBody.master),
                    teamIndex = TeamComponent.GetObjectTeam(self.gameObject),
                    attacker = self.gameObject,
                    procCoefficient = ThrowGlaive.glaiveProcCoefficient,
                    bouncesRemaining = ThrowGlaive.maxBounceCount,
                    speed = ThrowGlaive.glaiveTravelSpeed * speedMultiplier,
                    bouncedObjects = new List<HealthComponent>(),
                    range = ThrowGlaive.glaiveBounceRange,
                    damageCoefficientPerBounce = ThrowGlaive.damageCoefficientPerBounce
                };
                HurtBox hurtBox = self.initialOrbTarget;
                if (hurtBox)
                {
                    Transform transform = seed.transform;
                    seed.MuzzleEffect(ThrowGlaive.muzzleFlashPrefab, true);
                    lightningOrb.origin = transform.position;
                    lightningOrb.target = hurtBox;
                    OrbManager.instance.AddOrb(lightningOrb);
                }
            });
        }

        private void FireSeekingArrow_FireOrbArrow(On.EntityStates.Huntress.HuntressWeapon.FireSeekingArrow.orig_FireOrbArrow orig, FireSeekingArrow self)
        {
            int firedArrowCount = self.firedArrowCount;
            float arrowReloadTimer = self.arrowReloadTimer;
            orig(self);
            if (firedArrowCount >= self.maxArrowCount || arrowReloadTimer > 0f || !NetworkServer.active) return;
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                GenericDamageOrb genericDamageOrb = self.CreateArrowOrb();
                genericDamageOrb.damageValue = self.characterBody.damage * self.orbDamageCoefficient * multiplier;
                genericDamageOrb.isCrit = self.characterBody.RollCrit();
                genericDamageOrb.teamIndex = TeamComponent.GetObjectTeam(self.gameObject);
                genericDamageOrb.attacker = self.gameObject;
                genericDamageOrb.procCoefficient = self.orbProcCoefficient;
                HurtBox hurtBox = self.initialOrbTarget;
                if (hurtBox)
                {
                    Transform transform = seed.transform;
                    seed.MuzzleEffect(self.muzzleflashEffectPrefab, true);
                    genericDamageOrb.origin = transform.position;
                    genericDamageOrb.target = hurtBox;
                    OrbManager.instance.AddOrb(genericDamageOrb);
                }
            });
        }

        private void FireBeam_FixedUpdate(On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.orig_FixedUpdate orig, FireBeam self)
        {
            float seedFireTimer = self.fireTimer + Time.fixedDeltaTime;
            orig(self);
            FireForSeeds(self.characterBody, (seed, behavior, _t, multiplier) =>
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
                            damage = self.damageCoefficient * self.damageStat / self.fireFrequency,
                            procCoefficient = self.procCoefficient / self.fireFrequency,
                            force = self.force,
                            muzzleName = self.muzzleString,
                            hitEffectPrefab = self.hitEffectPrefab,
                            isCrit = self.characterBody.RollCrit(),
                            HitEffectNormal = false,
                            radius = 0f,
                            maxDistance = self.maxDistance
                        };
                        self.ModifyBullet(bulletAttack);
                        bulletAttack.damage *= multiplier;
                        bulletAttack.force *= multiplier;
                        bulletAttack.FilterOutOwnerFromAttack();
                        if (!mobileTurretsSeedSyncEffect) bulletAttack.tracerEffectPrefab = FireGatling.tracerEffectPrefab;
                        bulletAttack.Fire();
                    }
                }
                if (mobileTurretsSeedSyncEffect && behavior.U.SafeCheck("laserEffectInstance") && behavior.U.SafeCheck("laserEffectInstanceEndTransform"))
                {
                    Vector3 position = ((GameObject)behavior.U["laserEffectInstance"]).transform.parent.position;
                    Vector3 point = laserRay.GetPoint(self.maxDistance);
                    if (Physics.Raycast(laserRay.origin, laserRay.direction, out RaycastHit raycastHit, self.maxDistance,
                                        LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
                    {
                        point = raycastHit.point;
                    }
                    ((Transform)behavior.U["laserEffectInstanceEndTransform"]).position = point;
                }
            });
        }

        private void FireBeam_OnExit(On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.orig_OnExit orig, FireBeam self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, behavior, _t, _m) =>
            {
                if (behavior.U.SafeCheck("laserEffectInstance")) EntityState.Destroy(behavior.U["laserEffectInstance"]);
                if (behavior.U.SafeCheck("laserEffectInstanceEndTransform")) EntityState.Destroy(behavior.U["laserEffectInstanceEndTransform"]);
            });
        }

        private void FireBeam_OnEnter(On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.orig_OnEnter orig, FireBeam self)
        {
            orig(self);
            if (!mobileTurretsSeedSyncEffect) return;
            FireForSeeds(self.characterBody, (seed, behavior, _t, _m) =>
            {
                if (self.laserPrefab)
                {
                    if (behavior.U.SafeCheck("laserEffectInstance")) EntityState.Destroy(behavior.U["laserEffectInstance"]);
                    if (behavior.U.SafeCheck("laserEffectInstanceEndTransform")) EntityState.Destroy(behavior.U["laserEffectInstanceEndTransform"]);
                    Transform transform = seed.transform;
                    behavior.U["laserEffectInstance"] = Object.Instantiate(self.laserPrefab, transform.position, transform.rotation);
                    ((GameObject)behavior.U["laserEffectInstance"]).transform.parent = transform;
                    behavior.U["laserEffectInstanceEndTransform"] = ((GameObject)behavior.U["laserEffectInstance"]).GetComponent<ChildLocator>().FindChild("LaserEnd");
                }
            });
        }

        private void FireGauss_OnEnter(On.EntityStates.EngiTurret.EngiTurretWeapon.FireGauss.orig_OnEnter orig, FireGauss self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (FireGauss.effectPrefab) seed.MuzzleEffect(FireGauss.effectPrefab, false);
                if (self.isAuthority)
                {
                    BulletAttack attack = new BulletAttack
                    {
                        owner = self.gameObject,
                        weapon = seed,
                        origin = seed.transform.position,
                        aimVector = self.GetAimRay().direction,
                        minSpread = FireGauss.minSpread,
                        maxSpread = FireGauss.maxSpread,
                        bulletCount = 1U,
                        damage = FireGauss.damageCoefficient * self.damageStat * multiplier,
                        force = FireGauss.force * multiplier,
                        tracerEffectPrefab = FireGauss.tracerEffectPrefab,
                        muzzleName = "Muzzle",
                        hitEffectPrefab = FireGauss.hitEffectPrefab,
                        isCrit = Util.CheckRoll(self.critStat, self.characterBody.master),
                        HitEffectNormal = false,
                        radius = 0.15f
                    };
                    attack.FilterOutOwnerFromAttack();
                    attack.Fire();
                }
            });
        }

        private void Fire_FireMissile(On.EntityStates.Engi.EngiMissilePainter.Fire.orig_FireMissile orig, EngiMissilePainterFire self, HurtBox target, Vector3 position)
        {
            orig(self, target, position);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (EngiMissilePainterFire.muzzleflashEffectPrefab) seed.MuzzleEffect(EngiMissilePainterFire.muzzleflashEffectPrefab, true);
                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo()
                {
                    position = seed.transform.position,
                    rotation = Quaternion.LookRotation(Vector3.up),
                    crit = self.RollCrit(),
                    damage = self.damageStat * EngiMissilePainterFire.damageCoefficient * multiplier,
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
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (self.effectPrefab) seed.MuzzleEffect(self.effectPrefab, false);
                if (self.isAuthority)
                {
                    Ray aimRay = self.GetAimRay();
                    aimRay = self.ModifyProjectileAimRay(aimRay);
                    aimRay.direction = Util.ApplySpread(aimRay.direction, self.minSpread, self.maxSpread, 1f, 1f, 0f, self.projectilePitchBonus);
                    ProjectileManager.instance.FireProjectile(self.projectilePrefab, seed.transform.position, Util.QuaternionSafeLookRotation(aimRay.direction),
                                                              self.gameObject, self.damageStat * self.damageCoefficient * multiplier,
                                                              self.force * multiplier, Util.CheckRoll(self.critStat, self.characterBody.master),
                                                              DamageColorIndex.Default, null, -1f);
                }
            });
        }

        private void FireBarrage_FireBullet(On.EntityStates.Commando.CommandoWeapon.FireBarrage.orig_FireBullet orig, FireBarrage self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (FireBarrage.effectPrefab) seed.MuzzleEffect(FireBarrage.effectPrefab, false);
                if (self.isAuthority)
                {
                    BulletAttack attack = new BulletAttack
                    {
                        owner = self.gameObject,
                        weapon = seed,
                        origin = seed.transform.position + (self.GetAimRay().direction * 1.3f),
                        aimVector = self.GetAimRay().direction,
                        minSpread = FireBarrage.minSpread,
                        maxSpread = FireBarrage.maxSpread,
                        bulletCount = 1U,
                        damage = FireBarrage.damageCoefficient * self.damageStat * multiplier,
                        force = FireBarrage.force * multiplier,
                        tracerEffectPrefab = FireBarrage.tracerEffectPrefab,
                        muzzleName = "MuzzleRight",
                        hitEffectPrefab = FireBarrage.hitEffectPrefab,
                        isCrit = Util.CheckRoll(self.critStat, self.characterBody.master),
                        damageType = DamageType.Stun1s,
                        radius = FireBarrage.bulletRadius,
                        smartCollision = true
                    };
                    attack.FilterOutOwnerFromAttack();
                    attack.Fire();
                }
            });
        }

        private void GenericBulletBaseState_FireBullet(On.EntityStates.GenericBulletBaseState.orig_FireBullet orig, GenericBulletBaseState self, Ray aimRay)
        {
            orig(self, aimRay);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (self.muzzleFlashPrefab) seed.MuzzleEffect(self.muzzleFlashPrefab, false);
                if (self.isAuthority)
                {
                    Ray seedRay = new Ray(seed.transform.position, aimRay.direction);
                    BulletAttack bulletAttack = self.GenerateBulletAttack(seedRay);
                    self.ModifyBullet(bulletAttack);
                    bulletAttack.weapon = seed;
                    bulletAttack.damage *= multiplier;
                    bulletAttack.force *= multiplier;
                    bulletAttack.FilterOutOwnerFromAttack();
                    bulletAttack.Fire();
                }
            });
        }

        private void FirePistol2_FireBullet(On.EntityStates.Commando.CommandoWeapon.FirePistol2.orig_FireBullet orig, FirePistol2 self, string targetMuzzle)
        {
            orig(self, targetMuzzle);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
            {
                if (FirePistol2.muzzleEffectPrefab) seed.MuzzleEffect(FirePistol2.muzzleEffectPrefab, false);
                if (self.isAuthority)
                {
                    BulletAttack attack = new BulletAttack
                    {
                        owner = self.gameObject,
                        weapon = seed,
                        origin = seed.transform.position,
                        aimVector = self.aimRay.direction,
                        minSpread = 0f,
                        maxSpread = self.characterBody.spreadBloomAngle,
                        damage = FirePistol2.damageCoefficient * self.damageStat * multiplier,
                        force = FirePistol2.force * multiplier,
                        tracerEffectPrefab = FirePistol2.tracerEffectPrefab,
                        muzzleName = targetMuzzle,
                        hitEffectPrefab = FirePistol2.hitEffectPrefab,
                        isCrit = Util.CheckRoll(self.critStat, self.characterBody.master),
                        radius = 0.1f,
                        smartCollision = true
                    };
                    attack.FilterOutOwnerFromAttack();
                    attack.Fire();
                }
            });
        }

        private void FireGrenades_FireGrenade(On.EntityStates.Engi.EngiWeapon.FireGrenades.orig_FireGrenade orig, FireGrenades self, string targetMuzzle)
        {
            orig(self, targetMuzzle);
            FireForSeeds(self.characterBody, (seed, _b, _t, multiplier) =>
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
                                                              self.gameObject, self.damageStat * FireGrenades.damageCoefficient * multiplier, 0f,
                                                              Util.CheckRoll(self.critStat, self.characterBody.master), DamageColorIndex.Default, null, -1f);
                }
            });
        }

#if DEBUG

        private void EntityState_OnEnter(On.EntityStates.EntityState.orig_OnEnter orig, EntityState self)
        {
            orig(self);
            if (!self.characterBody || !self.characterBody.master) return;
            if (Helpers.GeneralHelpers.Instances.hostMasterObject == self.characterBody.masterObject)
            {
                Log.Message($"EntityState.OnEnter: {self.GetType().FullName}");
                if (self.characterBody) Log.Message($"EntityState.OnEnter: -> Body: {self.characterBody.name}");
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