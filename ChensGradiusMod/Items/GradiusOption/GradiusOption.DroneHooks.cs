using Chen.Helpers.UnityHelpers;
using EntityStates;
using EntityStates.Drone.DroneBombardment;
using EntityStates.Drone.DroneCopycat;
using EntityStates.Drone.DroneJailer;
using EntityStates.Drone.DroneWeapon;
using EntityStates.DroneWeaponsChainGun;
using RoR2;
using RoR2.Orbs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Chen.GradiusMod.Items.GradiusOption
{
    // Dedicated support for the drones whose attacks are not plain bullets, projectiles, melee hits or explosions.
    public partial class GradiusOption
    {
        private void InstallDroneHooks()
        {
            On.EntityStates.Drone.DroneWeapon.BombsAway.SpawnDestructible += BombsAway_SpawnDestructible;
            On.EntityStates.Drone.DroneWeapon.RechargeBeam.OnEnter += RechargeBeam_OnEnter;
            On.EntityStates.Drone.DroneWeapon.RechargeBeam.OnExit += RechargeBeam_OnExit;
            On.EntityStates.Drone.DroneBombardment.BombardmentDroneSkill.SpawnBombardmentRays += BombardmentDroneSkill_SpawnBombardmentRays;
            On.EntityStates.Drone.DroneJailer.JailerDroneSkill.DeployCage += JailerDroneSkill_DeployCage;
            On.EntityStates.Drone.DroneCopycat.FireLaserDisc.Fire += FireLaserDisc_Fire;
            On.EntityStates.DroneWeaponsChainGun.FireChainGun.OnEnter += FireChainGun_OnEnter;
        }

        private void UninstallDroneHooks()
        {
            On.EntityStates.Drone.DroneWeapon.BombsAway.SpawnDestructible -= BombsAway_SpawnDestructible;
            On.EntityStates.Drone.DroneWeapon.RechargeBeam.OnEnter -= RechargeBeam_OnEnter;
            On.EntityStates.Drone.DroneWeapon.RechargeBeam.OnExit -= RechargeBeam_OnExit;
            On.EntityStates.Drone.DroneBombardment.BombardmentDroneSkill.SpawnBombardmentRays -= BombardmentDroneSkill_SpawnBombardmentRays;
            On.EntityStates.Drone.DroneJailer.JailerDroneSkill.DeployCage -= JailerDroneSkill_DeployCage;
            On.EntityStates.Drone.DroneCopycat.FireLaserDisc.Fire -= FireLaserDisc_Fire;
            On.EntityStates.DroneWeaponsChainGun.FireChainGun.OnEnter -= FireChainGun_OnEnter;
        }

        // Lt. Droneboy: each Option drops its own dynamite.
        private void BombsAway_SpawnDestructible(On.EntityStates.Drone.DroneWeapon.BombsAway.orig_SpawnDestructible orig, BombsAway self)
        {
            orig(self);
            if (!NetworkServer.active) return;
            GameObject destructiblePrefab = BombsAway.destructiblePrefab.gameObject;
            if (!destructiblePrefab) return;
            FireForAllOptions(self.characterBody, (option, _b, _t, _d) =>
            {
                NetworkServer.Spawn(Object.Instantiate(destructiblePrefab, option.transform.position, Quaternion.identity));
            });
        }

        // Barrier Drone: each Option projects its own barrier beam on the same target.
        private void RechargeBeam_OnEnter(On.EntityStates.Drone.DroneWeapon.RechargeBeam.orig_OnEnter orig, RechargeBeam self)
        {
            orig(self);
            if (!NetworkServer.active || !self.target) return;
            GameObject beamPrefab = RechargeBeam.rechargeBeamPrefab.gameObject;
            if (!beamPrefab) return;
            float healRate = RechargeBeam.rechargeCoefficient * self.damageStat / self.duration * self.rechargeSpeedBonus * damageMultiplier;
            FireForAllOptions(self.characterBody, (option, behavior, _t, _d) =>
            {
                if (behavior.U.SafeCheck("rechargeBeamController")) ((HealBeamController)behavior.U["rechargeBeamController"]).BreakServer();
                GameObject beamInstance = Object.Instantiate(beamPrefab, option.transform);
                HealBeamController beamController = beamInstance.GetComponent<HealBeamController>();
                beamController.healBarrier = true;
                beamController.isRechargeBeam = true;
                beamController.healRate = healRate;
                beamController.target = self.target;
                beamController.ownership.ownerObject = option;
                behavior.U["rechargeBeamController"] = beamController;
                NetworkServer.Spawn(beamInstance);
            });
        }

        private void RechargeBeam_OnExit(On.EntityStates.Drone.DroneWeapon.RechargeBeam.orig_OnExit orig, RechargeBeam self)
        {
            orig(self);
            if (!NetworkServer.active) return;
            FireForAllOptions(self.characterBody, (_o, behavior, _t, _d) =>
            {
                if (behavior.U.SafeCheck("rechargeBeamController"))
                {
                    ((HealBeamController)behavior.U["rechargeBeamController"]).BreakServer();
                    behavior.U.Remove("rechargeBeamController");
                }
            });
        }

        // Bombardment Drone: each Option bombards the area below it with every pulse.
        private void BombardmentDroneSkill_SpawnBombardmentRays(On.EntityStates.Drone.DroneBombardment.BombardmentDroneSkill.orig_SpawnBombardmentRays orig, BombardmentDroneSkill self)
        {
            orig(self);
            if (!NetworkServer.active || !self.teamComponent) return;
            FireForAllOptions(self.characterBody, (option, _b, _t, _d) =>
            {
                Vector3 origin = FindGroundBelow(self.gameObject, option.transform.position, 18f, LayerIndex.world.mask);
                EffectManager.SpawnEffect(self.impactEffectPrefab, new EffectData { origin = origin, scale = BombardmentDroneSkill.radius }, true);
                foreach (HurtBox hurtBox in FindEnemyHurtBoxes(origin, BombardmentDroneSkill.radius, self.teamComponent.teamIndex))
                {
                    DamageInfo damageInfo = new DamageInfo
                    {
                        attacker = self.gameObject,
                        inflictor = option,
                        crit = self.crit,
                        damage = BombardmentDroneSkill.damageCoefficient * self.damageStat * damageMultiplier,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageTypeExtended.Electrical,
                        force = Vector3.up * BombardmentDroneSkill.juggleForce,
                        position = hurtBox.transform.position,
                        procChainMask = default,
                        procCoefficient = BombardmentDroneSkill.procCoefficient,
                        inflictedHurtbox = hurtBox
                    };
                    hurtBox.healthComponent.TakeDamage(damageInfo);
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, hurtBox.healthComponent.gameObject);
                    GlobalEventManager.instance.OnHitAll(damageInfo, hurtBox.healthComponent.gameObject);
                    if (hurtBox.healthComponent.TryGetComponent(out SetStateOnHurt setStateOnHurt) && setStateOnHurt.canBeStunned)
                    {
                        setStateOnHurt.SetCustomState(EntityStateCatalog.GetStateIndex(typeof(ShockStateRed)), InterruptPriority.Stun);
                    }
                }
            });
        }

        // Jailer Drone: each Option cages the area below it too. The extra prisoners are released together with the original cage.
        private void JailerDroneSkill_DeployCage(On.EntityStates.Drone.DroneJailer.JailerDroneSkill.orig_DeployCage orig, JailerDroneSkill self)
        {
            orig(self);
            if (!NetworkServer.active || !self.teamComponent || self.rootedBodies == null) return;
            FireForAllOptions(self.characterBody, (option, _b, _t, _d) =>
            {
                Vector3 origin = FindGroundBelow(self.gameObject, option.transform.position, 20f, LayerIndex.CommonMasks.bullet);
                EffectManager.SimpleEffect(JailerDroneSkill.effectPrefab, origin, Quaternion.identity, true);
                foreach (HurtBox hurtBox in FindEnemyHurtBoxes(origin, JailerDroneSkill.radius, self.teamComponent.teamIndex))
                {
                    CharacterBody body = hurtBox.healthComponent.body;
                    if (!body || self.rootedBodies.Contains(body)) continue;
                    self.rootedBodies.Add(body);
                    body.AddBuff(DLC3Content.Buffs.Jailed);
                }
            });
        }

        // Freeze Drone: each Option throws its own freezing disc at the same target.
        private void FireLaserDisc_Fire(On.EntityStates.Drone.DroneCopycat.FireLaserDisc.orig_Fire orig, FireLaserDisc self)
        {
            bool alreadyFired = self.hasFired;
            orig(self);
            if (alreadyFired || !self.hasFired || !NetworkServer.active || !self.initialOrbTarget) return;
            FireForAllOptions(self.characterBody, (option, _b, _t, _d) =>
            {
                LaserDiscOrb laserDiscOrb = new LaserDiscOrb
                {
                    orbEffect = FireLaserDisc.orbEffectPrefab,
                    damageValue = self.characterBody.damage * FireLaserDisc.damageCoefficient * damageMultiplier,
                    isCrit = self.RollCrit(),
                    teamIndex = self.teamComponent.teamIndex,
                    attacker = self.gameObject,
                    procCoefficient = self.totalProcCoefficient,
                    bouncesRemaining = self.totalBounceCount,
                    speed = self.totalSpeed,
                    bouncedObjects = new List<HealthComponent>(),
                    range = FireLaserDisc.discBounceRange,
                    damageCoefficientPerBounce = FireLaserDisc.damageCoefficientPerBounce,
                    damageType = DamageType.Freeze2s,
                    origin = option.transform.position,
                    target = self.initialOrbTarget
                };
                laserDiscOrb.damageType.damageSource = DamageSource.Primary;
                OrbManager.instance.AddOrb(laserDiscOrb);
            });
        }

        // Col. Droneman, and any drone armed by Spare Drone Parts: each Option fires its own chaingun shot.
        private void FireChainGun_OnEnter(On.EntityStates.DroneWeaponsChainGun.FireChainGun.orig_OnEnter orig, FireChainGun self)
        {
            orig(self);
            if (!self.isAuthority || !self.body || !self.targetHurtBox) return;
            // The chaingun is an orb, which automatic mirroring does not copy, so this also runs for automatically mirrored drones.
            ForEachOption(self.body, (option, _b, _t, _d) =>
            {
                ChainGunOrb chainGunOrb = new ChainGunOrb(self.orbEffectObject)
                {
                    damageValue = self.body.damage * self.damageCoefficient * damageMultiplier,
                    isCrit = Util.CheckRoll(self.body.crit, self.body.master),
                    teamIndex = TeamComponent.GetObjectTeam(self.body.gameObject),
                    attacker = self.body.gameObject,
                    procCoefficient = self.procCoefficient,
                    procChainMask = default,
                    origin = option.transform.position,
                    target = self.targetHurtBox,
                    speed = self.orbSpeed,
                    bouncesRemaining = self.additionalBounces,
                    bounceRange = self.bounceRange,
                    damageCoefficientPerBounce = self.damageCoefficientPerBounce,
                    targetsToFindPerBounce = self.targetsToFindPerBounce,
                    canBounceOnSameTarget = self.canBounceOnSameTarget,
                    damageColorIndex = DamageColorIndex.Item
                };
                OrbManager.instance.AddOrb(chainGunOrb);
            });
        }

        private static Vector3 FindGroundBelow(GameObject caster, Vector3 position, float maxDistance, LayerMask mask)
        {
            if (Util.CharacterRaycast(caster, new Ray(position, Vector3.down), out RaycastHit hitInfo, maxDistance, mask, QueryTriggerInteraction.Ignore))
            {
                return hitInfo.point;
            }
            return position + Vector3.down * maxDistance;
        }

        private static HurtBox[] FindEnemyHurtBoxes(Vector3 origin, float radius, TeamIndex teamIndex)
        {
            return new SphereSearch
            {
                origin = origin,
                radius = radius,
                mask = LayerIndex.entityPrecise.mask
            }.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamIndex)).OrderCandidatesByDistance()
                .FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes();
        }
    }
}
