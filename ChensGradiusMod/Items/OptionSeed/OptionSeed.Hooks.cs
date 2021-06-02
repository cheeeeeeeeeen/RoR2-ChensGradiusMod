#define DEBUG

using Chen.GradiusMod.Items.OptionSeed.Components;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using EntityStates.Drone.DroneWeapon;
using EntityStates.Engi.EngiWeapon;
using EntityStates.EngiTurret.EngiTurretWeapon;
using EntityStates.Huntress.HuntressWeapon;
using EntityStates.Merc;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
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

        private void Uppercut_OnEnter(On.EntityStates.Merc.Uppercut.orig_OnEnter orig, Uppercut self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, behavior) =>
            {
                behavior.O["Uppercut.OverlapAttack"] = self.InitMeleeOverlap(Uppercut.baseDamageCoefficient, Uppercut.hitEffectPrefab, self.GetModelTransform(), Uppercut.hitboxString);
                ((OverlapAttack)behavior.O["Uppercut.OverlapAttack"]).forceVector = Vector3.up * Uppercut.upwardForceStrength * damageMultiplier;
                ((OverlapAttack)behavior.O["Uppercut.OverlapAttack"]).damage *= damageMultiplier;
            }, (chance, behavior) => StoreProcCheck(chance, behavior, "Uppercut.Activated"));
        }

        private void Uppercut_FixedUpdate(On.EntityStates.Merc.Uppercut.orig_FixedUpdate orig, Uppercut self)
        {
            bool hasSwung = self.hasSwung;
            orig(self);
            FireForSeeds(self.characterBody, (seed, behavior) =>
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
            }, (_chance, behavior) => CheckStoredProc(behavior, "Uppercut.Activated"));
        }

        private void FocusedAssaultDash_OnMeleeHitAuthority(On.EntityStates.Merc.FocusedAssaultDash.orig_OnMeleeHitAuthority orig, FocusedAssaultDash self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (_seed, _behavior) =>
            {
                foreach (HurtBox victimHurtBox in self.hitResults)
                {
                    self.currentHitCount++;
                    float damageValue = self.characterBody.damage * self.delayedDamageCoefficient * damageMultiplier;
                    float actualDelay = self.delay + self.delayPerHit * self.currentHitCount;
                    bool isCrit = self.RollCrit();
                    FocusedAssaultDash.HandleHit(self.gameObject, victimHurtBox, damageValue, self.delayedProcCoefficient, isCrit, actualDelay, self.orbEffect, self.delayedEffectPrefab);
                }
            });
        }

        private void WhirlwindBase_OnEnter(On.EntityStates.Merc.WhirlwindBase.orig_OnEnter orig, WhirlwindBase self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (_seed, behavior) =>
            {
                behavior.O["Whirlwind.OverlapAttack"] = self.InitMeleeOverlap(self.baseDamageCoefficient, WhirlwindBase.hitEffectPrefab, self.GetModelTransform(), self.hitboxString);
                ((OverlapAttack)behavior.O["Whirlwind.OverlapAttack"]).damage *= damageMultiplier;
            }, (chance, behavior) => StoreProcCheck(chance, behavior, "Whirlwind.Activated"));
        }

        private void WhirlwindBase_FixedUpdate(On.EntityStates.Merc.WhirlwindBase.orig_FixedUpdate orig, WhirlwindBase self)
        {
            int swingCount = self.swingCount;
            orig(self);
            FireForSeeds(self.characterBody, (_seed, behavior) =>
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
            }, (_chance, behavior) => CheckStoredProc(behavior, "Whirlwind.Activated"));
        }

        private void Evis_FixedUpdate(On.EntityStates.Merc.Evis.orig_FixedUpdate orig, Evis self)
        {
            float attackStopwatch = self.attackStopwatch + Time.fixedDeltaTime;
            float tick = 1f / Evis.damageFrequency / self.attackSpeedStat;
            orig(self);
            FireForSeeds(self.characterBody, (seed, _behavior) =>
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
                            temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matMercEvisTarget");
                            temporaryOverlay.AddToCharacerModel(transform.GetComponent<CharacterModel>());
                            if (NetworkServer.active)
                            {
                                DamageInfo damageInfo = new DamageInfo
                                {
                                    damage = Evis.damageCoefficient * self.damageStat * damageMultiplier,
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
            FireForSeeds(self.characterBody, (_seed, behavior) =>
            {
                string overlapKey = $"{self.GetType().FullName}.OverlapAttack";
                if (!BasicMeleeAttackSkipModification.Contains(self.GetType().FullName))
                {
                    self.AuthorityModifyOverlapAttack((OverlapAttack)behavior.O[overlapKey]);
                }
                ((OverlapAttack)behavior.O[overlapKey]).Fire();
            }, (_chance, behavior) => CheckStoredProc(behavior, $"{self.GetType().FullName}.Activated"));
        }

        private void BasicMeleeAttack_OnEnter(On.EntityStates.BasicMeleeAttack.orig_OnEnter orig, BasicMeleeAttack self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (_seed, behavior) =>
            {
                if (self.isAuthority && self.hitBoxGroup)
                {
                    OverlapAttack overlapAttack = new OverlapAttack
                    {
                        attacker = self.gameObject,
                        damage = self.damageCoefficient * self.damageStat * damageMultiplier,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.Generic,
                        forceVector = self.forceVector,
                        hitBoxGroup = self.hitBoxGroup,
                        hitEffectPrefab = self.hitEffectPrefab,
                        inflictor = self.gameObject,
                        isCrit = self.characterBody.RollCrit(),
                        procChainMask = default,
                        pushAwayForce = self.pushAwayForce * damageMultiplier,
                        procCoefficient = self.procCoefficient,
                        teamIndex = self.GetTeam()
                    };
                    behavior.O[$"{self.GetType().FullName}.OverlapAttack"] = overlapAttack;
                }
            }, (chance, behavior) => StoreProcCheck(chance, behavior, $"{self.GetType().FullName}.Activated"));
        }

        private void ThrowGlaive_FireOrbGlaive(On.EntityStates.Huntress.HuntressWeapon.ThrowGlaive.orig_FireOrbGlaive orig, ThrowGlaive self)
        {
            bool hasTriedToThrowGlaive = self.hasTriedToThrowGlaive;
            orig(self);
            if (!NetworkServer.active || hasTriedToThrowGlaive) return;
            FireForSeeds(self.characterBody, (seed, behavior) =>
            {
                float speedMultiplier = 1f;
                switch (behavior.numbering)
                {
                    case 1:
                        speedMultiplier = .75f;
                        break;

                    case -1:
                        speedMultiplier = .5f;
                        break;

                    default:
                        break;
                }
                LightningOrb lightningOrb = new LightningOrb
                {
                    lightningType = LightningOrb.LightningType.HuntressGlaive,
                    damageValue = self.characterBody.damage * ThrowGlaive.damageCoefficient * damageMultiplier,
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
            FireForSeeds(self.characterBody, (seed, _behavior) =>
            {
                GenericDamageOrb genericDamageOrb = self.CreateArrowOrb();
                genericDamageOrb.damageValue = self.characterBody.damage * self.orbDamageCoefficient * damageMultiplier;
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
            FireForSeeds(self.characterBody, (seed, behavior) =>
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
            }, (_chance, behavior) => CheckStoredProc(behavior, "beamActivated"));
        }

        private void FireBeam_OnExit(On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.orig_OnExit orig, FireBeam self)
        {
            orig(self);
            FireForSeeds(self.characterBody, (seed, behavior) =>
            {
                if (behavior.U.SafeCheck("laserEffectInstance")) EntityState.Destroy(behavior.U["laserEffectInstance"]);
                if (behavior.U.SafeCheck("laserEffectInstanceEndTransform")) EntityState.Destroy(behavior.U["laserEffectInstanceEndTransform"]);
            }, (_chance, behavior) => CheckStoredProc(behavior, "beamActivated"));
        }

        private void FireBeam_OnEnter(On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.orig_OnEnter orig, FireBeam self)
        {
            orig(self);
            if (!mobileTurretsSeedSyncEffect) return;
            FireForSeeds(self.characterBody, (seed, behavior) =>
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
            }, (chance, behavior) => StoreProcCheck(chance, behavior, "beamActivated"));
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