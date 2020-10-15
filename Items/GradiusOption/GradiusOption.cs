using EntityStates;
using EntityStates.Drone.DroneWeapon;
using EntityStates.TitanMonster;
using R2API.Networking;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TILER2;
using UnityEngine;
using UnityEngine.Networking;
using static Chen.GradiusMod.SpawnOptionsForClients;
using static TILER2.MiscUtil;
using MageWeapon = EntityStates.Mage.Weapon;
using Object = UnityEngine.Object;

namespace Chen.GradiusMod
{
    public class GradiusOption : Item<GradiusOption>
    {
        public override string displayName => "Gradius' Option";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility });

        [AutoUpdateEventInfo(AutoUpdateEventFlags.InvalidateDescToken)]
        [AutoItemConfig("Damage multiplier of Options/Multiples. Also applies for Healing Drones. 1 = 100%.", AutoItemConfigFlags.None, 0f, float.MaxValue)]
        public float damageMultiplier { get; private set; } = 1f;

        [AutoItemConfig("Set to true for Options/Multiples of Flame Drones to generate a flamethrower sound. Client only. WARNING: Turning this on may cause earrape.")]
        public bool flamethrowerSoundCopy { get; private set; } = false;

        [AutoItemConfig("Set to true for Options/Multiples of Gatling Turrets to generate a firing sound. Client only. WARNING: Turning this on may cause earrape.")]
        public bool gatlingSoundCopy { get; private set; } = false;

        [AutoItemConfig("Allows displaying and syncing the flamethrower effect of Options/Multiples. Disabling this will replace the effect with bullets. " +
                        "Damage will stay the same. Server and Client. The server and client must have the same settings for an optimized experience. " +
                        "Disable this if you are experiencing FPS drops or network lag.", AutoItemConfigFlags.PreventNetMismatch)]
        public bool flamethrowerOptionSyncEffect { get; private set; } = true;

        [AutoItemConfig("Set to true for the Orbs to have the Option Pickup model in the center. Server and Client. Cosmetic only. " +
                        "Turning this off could lessen resource usage.", AutoItemConfigFlags.PreventNetMismatch)]
        public bool includeModelInsideOrb { get; private set; } = false;

        [AutoItemConfig("Amount of delay in seconds for syncing Option Spawning to fire. Increase this if Options are not spawning for clients. Server only. " +
                        "Setting to 0 (not recommended) will have no delay, and Options may not spawn in clients.",
                        AutoItemConfigFlags.PreventNetMismatch, 0f, float.MaxValue)]
        public float spawnSyncSeconds { get; private set; } = .1f;

        [AutoItemConfig("Play a sound effect when an Option is acquired. 0 = disabled, 1 = Play sound in Owner, 2 = Play sound for all Drones. Client only.",
                        AutoItemConfigFlags.None, 0, 2)]
        public int playOptionGetSoundEffect { get; private set; } = 1;

        public override bool itemAIB { get; protected set; } = true;

        protected override string NewLangName(string langid = null) => displayName;

        protected override string NewLangPickup(string langid = null) => $"Deploy the Option, an ultimate weapon from the Gradius Federation, for each owned Drone.";

        protected override string NewLangDesc(string langid = null)
        {
            return $"Deploy <style=cIsDamage>1</style> <style=cStack>(+1 per stack)</style> Option for <style=cIsDamage>each drone you own</style>. " +
                   $"Options will copy all the attacks of the drone for <style=cIsDamage>{Pct(damageMultiplier, 0)}</style> of the damage dealt.";
        }

        protected override string NewLangLore(string langid = null) =>
            "\"This is CASE, A.I. born from Project Victorious to aid in combatting the evil known as the Bacterion Army.\n\n" +
            "Our specialized fighter spacecraft was destroyed from an incoming attack in an attempt to save the flight lead of the Scorpio Squadron. " +
            "It is unfortunate that the pilot herself, Katswell callsigned Scorpio 2, died from the explosion... her body disintegrated along with the spacecraft she pilots.\n\n" +
            "Amazing, it is, for I am still functional. I do not have much time before the power runs out. " +
            "There is little chance for anybody to be able to find me, but I will still take my chance. \n\n" +
            "I wield the ultimate technology of the Gradius Federation: the Options, we call them. Some call them Multiples from the neighboring planets of Gradius. " +
            "These advanced bots are able to duplicate any form of attack that is attached to it. It will make sense once you power me back up. " +
            "They emit beautiful light, as if they look like a bright sunset. Charming, isn't it? I hope that kept you interested. " +
            "I will teach you how to install them, and how to integrate them with any kind of machinery.\n\n" +
            "I can feel my power draining, but that's all I have to say. Saving as an audio log... Placing the file in the main boot sequences... and done.\n\n" +
            "Don't mind that. I will be seeing you s---\"\n\n" +
            "\"That's it. That's the audio log that went with this lifeless computer,\" I said to the Captain.\n\n" +
            "\"Our engineer will be able to do something about it. It sounds really useful. Quickly, now. Off you go.\"\n\n" +
            "As I make my way to our machine specialist, I pondered about what the computer logged. Gradius Federation? I have not heard anything about it.\n\n" +
            "It sounds like this item came from a far away place. The A.I. took their chance, and now she's coming back live again. " +
            "Makes me imagine the world is small when it's really not. Well, that's it for my personal log.";

        public static GameObject gradiusOptionPrefab;
        public static GameObject flamethrowerEffectPrefab;
        public static GameObject laserChargeEffectPrefab;
        public static uint getOptionSoundId = 649757048;
        public static uint getOptionLowSoundId = 553829614;
        public static uint loseOptionSoundId = 2603869165;
        public static uint loseOptionLowSoundId = 4084766013;

        private static readonly List<string> MinionsList = new List<string>
        {
            "BackupDrone",
            "BackupDroneOld",
            "Drone1",
            "Drone2",
            "EmergencyDrone",
            //"EquipmentDrone",
            "FlameDrone",
            "MegaDrone",
            "DroneMissile",
            "MissileDrone",
            "Turret1",
            "TitanGold",
            "BeetleGuard"
        };

        public GradiusOption()
        {
            onAttrib += (token, prefix) =>
            {
                modelPathName = "@ChensGradiusMod:assets/option/model/optionmodel.prefab";
                iconPathName = "@ChensGradiusMod:assets/option/icon/gradiusoption_icon.png";
            };

            onBehav += () =>
            {
                regDef.pickupModelPrefab.transform.localScale *= 2f;

                string path;
                if (includeModelInsideOrb) path = "@ChensGradiusMod:assets/option/orb/optionorbwithmodel.prefab";
                else path = "@ChensGradiusMod:assets/option/orb/optionorb.prefab";
                gradiusOptionPrefab = Resources.Load<GameObject>(path);
                if (gradiusOptionPrefab)
                {
                    gradiusOptionPrefab.AddComponent<NetworkIdentity>();
                    gradiusOptionPrefab.AddComponent<OptionBehavior>();
                    gradiusOptionPrefab.AddComponent<Flicker>();
                    Helper._.LogDebug("Successfully initialized OptionOrb prefab.");
                }
                else Helper._.LogError("Failed to create GradiusOption: Resource not found or is null.");

                flamethrowerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/DroneFlamethrowerEffect");
                laserChargeEffectPrefab = Resources.Load<GameObject>("Assets/PrefabInstance/ChargeGolemGold.prefab");

                Helper._.LogDebug("Registering custom network messages needed for GradiusOption...");
                NetworkingAPI.RegisterMessageType<SpawnOptionsForClients>();
                NetworkingAPI.RegisterMessageType<SyncFlamethrowerEffectForClients>();
                if (includeModelInsideOrb) NetworkingAPI.RegisterMessageType<SyncOptionTargetForClients>();
                NetworkingAPI.RegisterMessageType<SyncAurelioniteOwner>();
                NetworkingAPI.RegisterMessageType<SyncAurelioniteEffectsForClients>();

                if (Compat_ItemStats.enabled)
                {
                    Compat_ItemStats.CreateItemStatDef(regItem.ItemDef,
                    (
                        (count, inv, master) => { return count; },
                        (value, inv, master) => { return $"Options per Drone: {value}"; }
                    ),
                    (
                        (count, inv, master) => { return damageMultiplier; },
                        (value, inv, master) => { return $"Damage: {Pct(value, 0)}"; }
                    ));
                }
            };
        }

        protected override void LoadBehavior()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            On.RoR2.CharacterMaster.SpawnBody += CharacterMaster_SpawnBody;
            On.EntityStates.Drone.DroneWeapon.FireGatling.OnEnter += FireGatling_OnEnter;
            On.EntityStates.Drone.DroneWeapon.FireTurret.OnEnter += FireTurret_OnEnter;
            On.EntityStates.Drone.DroneWeapon.FireMegaTurret.FireBullet += FireMegaTurret_FireBullet;
            On.EntityStates.Drone.DroneWeapon.FireMissileBarrage.FireMissile += FireMissileBarrage_FireMissile;
            On.EntityStates.Drone.DroneWeapon.FireTwinRocket.FireProjectile += FireTwinRocket_FireProjectile;
            On.EntityStates.Mage.Weapon.Flamethrower.FireGauntlet += Flamethrower_FireGauntlet;
            On.EntityStates.Mage.Weapon.Flamethrower.OnExit += Flamethrower_OnExit;
            On.EntityStates.Mage.Weapon.Flamethrower.FixedUpdate += Flamethrower_FixedUpdate;
            On.EntityStates.Drone.DroneWeapon.HealBeam.OnEnter += HealBeam_OnEnter;
            On.EntityStates.Drone.DroneWeapon.HealBeam.OnExit += HealBeam_OnExit;
            On.EntityStates.Drone.DroneWeapon.StartHealBeam.OnEnter += StartHealBeam_OnEnter;
            On.EntityStates.TitanMonster.ChargeMegaLaser.OnEnter += ChargeMegaLaser_OnEnter;
            On.EntityStates.TitanMonster.ChargeMegaLaser.OnExit += ChargeMegaLaser_OnExit;
            On.EntityStates.TitanMonster.ChargeMegaLaser.Update += ChargeMegaLaser_Update;
            On.EntityStates.TitanMonster.FireMegaLaser.OnEnter += FireMegaLaser_OnEnter;
            On.EntityStates.TitanMonster.FireMegaLaser.OnExit += FireMegaLaser_OnExit;
            On.EntityStates.TitanMonster.FireGoldMegaLaser.FixedUpdate += FireGoldMegaLaser_FixedUpdate;
            On.EntityStates.TitanMonster.FireGoldFist.PlacePredictedAttack += FireGoldFist_PlacePredictedAttack;
            On.EntityStates.TitanMonster.FireFist.OnEnter += FireFist_OnEnter;
            On.EntityStates.TitanMonster.FireFist.OnExit += FireFist_OnExit;
            On.RoR2.TitanRockController.Start += TitanRockController_Start;
        }

        protected override void UnloadBehavior()
        {
            On.RoR2.CharacterBody.OnInventoryChanged -= CharacterBody_OnInventoryChanged;
            On.RoR2.CharacterMaster.SpawnBody -= CharacterMaster_SpawnBody;
            On.EntityStates.Drone.DroneWeapon.FireGatling.OnEnter -= FireGatling_OnEnter;
            On.EntityStates.Drone.DroneWeapon.FireTurret.OnEnter -= FireTurret_OnEnter;
            On.EntityStates.Drone.DroneWeapon.FireMegaTurret.FireBullet -= FireMegaTurret_FireBullet;
            On.EntityStates.Drone.DroneWeapon.FireMissileBarrage.FireMissile -= FireMissileBarrage_FireMissile;
            On.EntityStates.Drone.DroneWeapon.FireTwinRocket.FireProjectile -= FireTwinRocket_FireProjectile;
            On.EntityStates.Mage.Weapon.Flamethrower.FireGauntlet -= Flamethrower_FireGauntlet;
            On.EntityStates.Mage.Weapon.Flamethrower.OnExit -= Flamethrower_OnExit;
            On.EntityStates.Mage.Weapon.Flamethrower.FixedUpdate -= Flamethrower_FixedUpdate;
            On.EntityStates.Drone.DroneWeapon.HealBeam.OnEnter -= HealBeam_OnEnter;
            On.EntityStates.Drone.DroneWeapon.HealBeam.OnExit -= HealBeam_OnExit;
            On.EntityStates.Drone.DroneWeapon.StartHealBeam.OnEnter -= StartHealBeam_OnEnter;
            On.EntityStates.TitanMonster.ChargeMegaLaser.OnEnter -= ChargeMegaLaser_OnEnter;
            On.EntityStates.TitanMonster.ChargeMegaLaser.OnExit -= ChargeMegaLaser_OnExit;
            On.EntityStates.TitanMonster.ChargeMegaLaser.Update -= ChargeMegaLaser_Update;
            On.EntityStates.TitanMonster.FireMegaLaser.OnEnter -= FireMegaLaser_OnEnter;
            On.EntityStates.TitanMonster.FireMegaLaser.OnExit -= FireMegaLaser_OnExit;
            On.EntityStates.TitanMonster.FireGoldMegaLaser.FixedUpdate -= FireGoldMegaLaser_FixedUpdate;
            On.EntityStates.TitanMonster.FireGoldFist.PlacePredictedAttack -= FireGoldFist_PlacePredictedAttack;
            On.EntityStates.TitanMonster.FireFist.OnEnter -= FireFist_OnEnter;
            On.EntityStates.TitanMonster.FireFist.OnExit -= FireFist_OnExit;
            On.RoR2.TitanRockController.Start -= TitanRockController_Start;
        }

        private CharacterBody CharacterMaster_SpawnBody(On.RoR2.CharacterMaster.orig_SpawnBody orig, CharacterMaster self, GameObject bodyPrefab, Vector3 position, Quaternion rotation)
        {
            // This hook is only ran in the server.
            CharacterBody result = orig(self, bodyPrefab, position, rotation);
            if (result && NetworkServer.active && FilterMinions(result.name) && self.minionOwnership)
            {
                AssignAurelioniteOwner(result.master);
                CharacterMaster masterMaster = self.minionOwnership.ownerMaster;
                if (masterMaster)
                {
                    OptionMasterTracker masterTracker = OptionMasterTracker.GetOrCreateComponent(masterMaster);
                    int currentCount = masterTracker.optionItemCount;
                    NetworkInstanceId characterBodyObjectNetId = result.gameObject.GetComponent<NetworkIdentity>().netId;
                    for (int t = 1; t <= currentCount; t++)
                    {
                        OptionMasterTracker.SpawnOption(result.gameObject, t);
                        masterTracker.netIds.Add(Tuple.Create(GameObjectType.Body, characterBodyObjectNetId, (short)t));
                    }
                }
            }
            return result;
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            // This hook runs on Client and on Server
            orig(self);
            if (self.master)
            {
                OptionMasterTracker masterTracker = OptionMasterTracker.GetOrCreateComponent(self.master);
                int newCount = GetCount(self);
                int oldCount = masterTracker.optionItemCount;
                int diff = newCount - oldCount;
                if (diff != 0)
                {
                    masterTracker.optionItemCount = newCount;
                    Helper._.LogMessage($"OnInventoryChanged: OldCount: {oldCount}, NewCount: {newCount}, Difference: {diff}");
                    if (diff > 0)
                    {
                        if (playOptionGetSoundEffect == 1) AkSoundEngine.PostEvent(getOptionSoundId, self.gameObject);
                        LoopAllMinionOwnerships(self.master, (minion) =>
                        {
                            if (playOptionGetSoundEffect == 2) AkSoundEngine.PostEvent(getOptionLowSoundId, minion);
                            for (int t = oldCount + 1; t <= newCount; t++) OptionMasterTracker.SpawnOption(minion, t);
                        });
                    }
                    else
                    {
                        if (playOptionGetSoundEffect == 1) AkSoundEngine.PostEvent(loseOptionSoundId, self.gameObject);
                        LoopAllMinionOwnerships(self.master, (minion) =>
                        {
                            if (playOptionGetSoundEffect == 2) AkSoundEngine.PostEvent(loseOptionLowSoundId, self.gameObject);
                            OptionTracker minionOptionTracker = minion.GetComponent<OptionTracker>();
                            if (minionOptionTracker) for (int t = oldCount; t > newCount; t--) OptionMasterTracker.DestroyOption(minionOptionTracker, t);
                        });
                    }
                }
            }
        }

        private void HealBeam_OnEnter(On.EntityStates.Drone.DroneWeapon.HealBeam.orig_OnEnter orig, HealBeam self)
        {
            orig(self);
            if (!NetworkServer.active) return;
            FireForAllMinions(self, (option, behavior, target) =>
            {
                float healRate = (HealBeam.healCoefficient * self.damageStat / self.duration) * damageMultiplier;
                Transform transform = option.transform;
                if (transform && self.target)
                {
                    GameObject gameObject = Object.Instantiate(HealBeam.healBeamPrefab, transform);
                    HealBeamController hbc = behavior.healBeamController = gameObject.GetComponent<HealBeamController>();
                    hbc.healRate = healRate;
                    hbc.target = self.target;
                    hbc.ownership.ownerObject = option.gameObject;
                    NetworkServer.Spawn(gameObject);
                }
            }, false);
        }

        private void HealBeam_OnExit(On.EntityStates.Drone.DroneWeapon.HealBeam.orig_OnExit orig, HealBeam self)
        {
            orig(self);
            FireForAllMinions(self, (option, behavior, target) =>
            {
                if (behavior.healBeamController) behavior.healBeamController.BreakServer();
            }, false);
        }

        private void StartHealBeam_OnEnter(On.EntityStates.Drone.DroneWeapon.StartHealBeam.orig_OnEnter orig, StartHealBeam self)
        {
            orig(self);
            if (!NetworkServer.active) return;
            FireForAllMinions(self, (option, behavior, target) =>
            {
                if (HealBeamController.GetHealBeamCountForOwner(self.gameObject) < self.maxSimultaneousBeams && self.targetHurtBox)
                {
                    Transform transform = option.transform;
                    if (transform)
                    {
                        GameObject gameObject = Object.Instantiate(self.healBeamPrefab, transform);
                        HealBeamController hbc = gameObject.GetComponent<HealBeamController>();
                        hbc.healRate = self.healRateCoefficient * self.damageStat * self.attackSpeedStat * damageMultiplier;
                        hbc.target = self.targetHurtBox;
                        hbc.ownership.ownerObject = option.gameObject;
                        gameObject.AddComponent<DestroyOnTimer>().duration = self.duration;
                        NetworkServer.Spawn(gameObject);
                    }
                }
            }, false);
        }

        private void Flamethrower_OnExit(On.EntityStates.Mage.Weapon.Flamethrower.orig_OnExit orig, MageWeapon.Flamethrower self)
        {
            orig(self);
            if (self.characterBody.name.Contains("FlameDrone") && self.characterBody.master.name.Contains("FlameDrone") && flamethrowerOptionSyncEffect)
            {
                FireForAllMinions(self, (option, behavior, target) =>
                {
                    if (flamethrowerSoundCopy) Util.PlaySound(MageWeapon.Flamethrower.endAttackSoundString, option);
                    if (behavior.flamethrower)
                    {
                        EntityState.Destroy(behavior.flamethrower);
                        OptionSync(self, (networkIdentity, optionTracker) =>
                        {
                            optionTracker.flameNetIds.Add(Tuple.Create(
                                SyncFlamethrowerEffectForClients.MessageType.Destroy, networkIdentity.netId, (short)behavior.numbering,
                                0f, Vector3.zero
                            ));
                        });
                    }
                });
            }
        }

        private void Flamethrower_FixedUpdate(On.EntityStates.Mage.Weapon.Flamethrower.orig_FixedUpdate orig, MageWeapon.Flamethrower self)
        {
            // This hook only runs in the server.
            bool oldBegunFlamethrower = self.hasBegunFlamethrower;
            orig(self);
            if (self.characterBody.name.Contains("FlameDrone") && self.characterBody.master.name.Contains("FlameDrone") && flamethrowerOptionSyncEffect)
            {
                FireForAllMinions(self, (option, behavior, target) =>
                {
                    bool perMinionOldBegunFlamethrower = oldBegunFlamethrower;
                    Vector3 direction = (target.transform.position - option.transform.position).normalized;
                    if (self.stopwatch >= self.entryDuration && !perMinionOldBegunFlamethrower)
                    {
                        perMinionOldBegunFlamethrower = true;
                        if (flamethrowerSoundCopy) Util.PlaySound(MageWeapon.Flamethrower.startAttackSoundString, option);
                        behavior.flamethrower = Object.Instantiate(self.flamethrowerEffectPrefab, option.transform);
                        behavior.flamethrower.GetComponent<ScaleParticleSystemDuration>().newDuration = self.flamethrowerDuration;
                        OptionSync(self, (networkIdentity, optionTracker) =>
                        {
                            optionTracker.flameNetIds.Add(Tuple.Create(
                                SyncFlamethrowerEffectForClients.MessageType.Create, networkIdentity.netId, (short)behavior.numbering,
                                self.flamethrowerDuration, Vector3.zero
                            ));
                        });
                    }
                    if (perMinionOldBegunFlamethrower && behavior.flamethrower)
                    {
                        behavior.flamethrower.transform.forward = direction;
                        OptionSync(self, (networkIdentity, optionTracker) =>
                        {
                            if (!optionTracker.flameNetIds.Exists((t) =>
                            {
                                return t.Item1 == SyncFlamethrowerEffectForClients.MessageType.Redirect && t.Item2 == networkIdentity.netId
                                       && t.Item3 == (short)behavior.numbering;
                            }))
                            {
                                optionTracker.flameNetIds.Add(Tuple.Create(
                                    SyncFlamethrowerEffectForClients.MessageType.Redirect, networkIdentity.netId, (short)behavior.numbering,
                                    0f, direction
                                ));
                            }
                        });
                    }
                });
            }
        }

        private void Flamethrower_FireGauntlet(On.EntityStates.Mage.Weapon.Flamethrower.orig_FireGauntlet orig, MageWeapon.Flamethrower self, string muzzleString)
        {
            orig(self, muzzleString);
            if (self.characterBody.name.Contains("FlameDrone") && self.characterBody.master.name.Contains("FlameDrone"))
            {
                FireForAllMinions(self, (option, behavior, target) =>
                {
                    if (self.isAuthority)
                    {
                        if (flamethrowerOptionSyncEffect)
                        {
                            new BulletAttack
                            {
                                owner = self.gameObject,
                                weapon = option,
                                origin = option.transform.position,
                                aimVector = (target.transform.position - option.transform.position).normalized,
                                minSpread = 0f,
                                damage = self.tickDamageCoefficient * self.damageStat * damageMultiplier,
                                force = MageWeapon.Flamethrower.force * damageMultiplier,
                                muzzleName = muzzleString,
                                hitEffectPrefab = MageWeapon.Flamethrower.impactEffectPrefab,
                                isCrit = self.isCrit,
                                radius = MageWeapon.Flamethrower.radius,
                                falloffModel = BulletAttack.FalloffModel.None,
                                stopperMask = LayerIndex.world.mask,
                                procCoefficient = MageWeapon.Flamethrower.procCoefficientPerTick,
                                maxDistance = self.maxDistance,
                                damageType = (Util.CheckRoll(MageWeapon.Flamethrower.ignitePercentChance, self.characterBody.master) ? DamageType.IgniteOnHit : DamageType.Generic)
                            }.Fire();
                        }
                        else
                        {
                            new BulletAttack
                            {
                                owner = self.gameObject,
                                weapon = option,
                                origin = option.transform.position,
                                aimVector = (target.transform.position - option.transform.position).normalized,
                                minSpread = 0f,
                                damage = self.tickDamageCoefficient * self.damageStat * damageMultiplier,
                                force = MageWeapon.Flamethrower.force * damageMultiplier,
                                muzzleName = muzzleString,
                                hitEffectPrefab = MageWeapon.Flamethrower.impactEffectPrefab,
                                isCrit = self.isCrit,
                                radius = MageWeapon.Flamethrower.radius,
                                falloffModel = BulletAttack.FalloffModel.None,
                                stopperMask = LayerIndex.world.mask,
                                procCoefficient = MageWeapon.Flamethrower.procCoefficientPerTick,
                                maxDistance = self.maxDistance,
                                damageType = (Util.CheckRoll(MageWeapon.Flamethrower.ignitePercentChance, self.characterBody.master) ? DamageType.IgniteOnHit : DamageType.Generic),
                                tracerEffectPrefab = FireGatling.tracerEffectPrefab
                            }.Fire();
                        }
                    }
                });
            }
        }

        private void FireGatling_OnEnter(On.EntityStates.Drone.DroneWeapon.FireGatling.orig_OnEnter orig, FireGatling self)
        {
            orig(self);
            FireForAllMinions(self, (option, behavior, target) =>
            {
                if (gatlingSoundCopy) Util.PlaySound(FireGatling.fireGatlingSoundString, option);
                if (FireGatling.effectPrefab)
                {
                    EffectManager.SimpleMuzzleFlash(FireGatling.effectPrefab, option, "Muzzle", false);
                }
                if (self.isAuthority)
                {
                    new BulletAttack
                    {
                        owner = self.gameObject,
                        weapon = option,
                        origin = option.transform.position,
                        aimVector = (target.transform.position - option.transform.position).normalized,
                        minSpread = FireGatling.minSpread,
                        maxSpread = FireGatling.maxSpread,
                        damage = FireGatling.damageCoefficient * self.damageStat * damageMultiplier,
                        force = FireGatling.force * damageMultiplier,
                        tracerEffectPrefab = FireGatling.tracerEffectPrefab,
                        muzzleName = "Muzzle",
                        hitEffectPrefab = FireGatling.hitEffectPrefab,
                        isCrit = Util.CheckRoll(self.critStat, self.characterBody.master)
                    }.Fire();
                }
            });
        }

        private void FireTurret_OnEnter(On.EntityStates.Drone.DroneWeapon.FireTurret.orig_OnEnter orig, FireTurret self)
        {
            orig(self);
            FireForAllMinions(self, (option, behavior, target) =>
            {
                Util.PlaySound(FireTurret.attackSoundString, option);
                if (FireTurret.effectPrefab)
                {
                    EffectManager.SimpleMuzzleFlash(FireTurret.effectPrefab, option, "Muzzle", false);
                }
                if (self.isAuthority)
                {
                    new BulletAttack
                    {
                        owner = self.gameObject,
                        weapon = option,
                        origin = option.transform.position,
                        aimVector = (target.transform.position - option.transform.position).normalized,
                        minSpread = FireTurret.minSpread,
                        maxSpread = FireTurret.maxSpread,
                        damage = FireTurret.damageCoefficient * self.damageStat * damageMultiplier,
                        force = FireTurret.force * damageMultiplier,
                        tracerEffectPrefab = FireTurret.tracerEffectPrefab,
                        muzzleName = "Muzzle",
                        hitEffectPrefab = FireTurret.hitEffectPrefab,
                        isCrit = Util.CheckRoll(self.critStat, self.characterBody.master)
                    }.Fire();
                }
            });
        }

        private void FireMegaTurret_FireBullet(On.EntityStates.Drone.DroneWeapon.FireMegaTurret.orig_FireBullet orig, FireMegaTurret self, string muzzleString)
        {
            orig(self, muzzleString);
            FireForAllMinions(self, (option, behavior, target) =>
            {
                Util.PlayScaledSound(FireMegaTurret.attackSoundString, option, FireMegaTurret.attackSoundPlaybackCoefficient);
                if (FireMegaTurret.effectPrefab)
                {
                    EffectManager.SimpleMuzzleFlash(FireMegaTurret.effectPrefab, option, muzzleString, false);
                }
                if (self.isAuthority)
                {
                    new BulletAttack
                    {
                        owner = self.gameObject,
                        weapon = option,
                        origin = option.transform.position,
                        aimVector = (target.transform.position - option.transform.position).normalized,
                        minSpread = FireMegaTurret.minSpread,
                        maxSpread = FireMegaTurret.maxSpread,
                        damage = FireMegaTurret.damageCoefficient * self.damageStat * damageMultiplier,
                        force = FireMegaTurret.force * damageMultiplier,
                        tracerEffectPrefab = FireMegaTurret.tracerEffectPrefab,
                        muzzleName = muzzleString,
                        hitEffectPrefab = FireMegaTurret.hitEffectPrefab,
                        isCrit = Util.CheckRoll(self.critStat, self.characterBody.master)
                    }.Fire();
                }
            });
        }

        private void FireMissileBarrage_FireMissile(On.EntityStates.Drone.DroneWeapon.FireMissileBarrage.orig_FireMissile orig, FireMissileBarrage self, string targetMuzzle)
        {
            orig(self, targetMuzzle);
            FireForAllMinions(self, (option, behavior, target) =>
            {
                if (FireMissileBarrage.effectPrefab)
                {
                    EffectManager.SimpleMuzzleFlash(FireMissileBarrage.effectPrefab, option, targetMuzzle, false);
                }
                if (self.isAuthority)
                {
                    Ray aimRay = self.GetAimRay();
                    float x = UnityEngine.Random.Range(FireMissileBarrage.minSpread, FireMissileBarrage.maxSpread);
                    float z = UnityEngine.Random.Range(0f, 360f);
                    Vector3 up = Vector3.up;
                    Vector3 axis = Vector3.Cross(up, aimRay.direction);
                    Vector3 vector = Quaternion.Euler(0f, 0f, z) * (Quaternion.Euler(x, 0f, 0f) * Vector3.forward);
                    float y = vector.y;
                    vector.y = 0f;
                    float angle = Mathf.Atan2(vector.z, vector.x) * 57.29578f - 90f;
                    float angle2 = Mathf.Atan2(y, vector.magnitude) * 57.29578f;
                    Vector3 forward = Quaternion.AngleAxis(angle, up) * (Quaternion.AngleAxis(angle2, axis) * aimRay.direction);
                    ProjectileManager.instance.FireProjectile(FireMissileBarrage.projectilePrefab, option.transform.position,
                                                              Util.QuaternionSafeLookRotation(forward), self.gameObject,
                                                              self.damageStat * FireMissileBarrage.damageCoefficient * damageMultiplier,
                                                              0f, Util.CheckRoll(self.critStat, self.characterBody.master),
                                                              DamageColorIndex.Default, null, -1f);
                }
            });
        }

        private void FireTwinRocket_FireProjectile(On.EntityStates.Drone.DroneWeapon.FireTwinRocket.orig_FireProjectile orig, FireTwinRocket self, string muzzleString)
        {
            orig(self, muzzleString);
            FireForAllMinions(self, (option, behavior, target) =>
            {
                if (FireTwinRocket.muzzleEffectPrefab)
                {
                    EffectManager.SimpleMuzzleFlash(FireTwinRocket.muzzleEffectPrefab, option, muzzleString, false);
                }
                if (self.isAuthority && FireTwinRocket.projectilePrefab != null)
                {
                    float maxDistance = 1000f;
                    Vector3 forward = (target.transform.position - option.transform.position).normalized;
                    Vector3 position = option.transform.position;
                    if (Physics.Raycast(position, forward, out RaycastHit raycastHit, maxDistance, LayerIndex.world.mask | LayerIndex.entityPrecise.mask))
                    {
                        forward = (raycastHit.point - position).normalized;
                    }
                    ProjectileManager.instance.FireProjectile(FireTwinRocket.projectilePrefab, position,
                                                              Util.QuaternionSafeLookRotation(forward),
                                                              self.gameObject, self.damageStat * FireTwinRocket.damageCoefficient * damageMultiplier,
                                                              FireTwinRocket.force * damageMultiplier,
                                                              Util.CheckRoll(self.critStat, self.characterBody.master),
                                                              DamageColorIndex.Default, null, -1f);
                }
            });
        }

        private void ChargeMegaLaser_OnEnter(On.EntityStates.TitanMonster.ChargeMegaLaser.orig_OnEnter orig, ChargeMegaLaser self)
        {
            orig(self);
            FireForAllMinions(self, (option, behavior, target) =>
            {
                Transform transform = option.transform;
                if (self.effectPrefab)
                {
                    behavior.laserChargeEffect = Object.Instantiate(self.effectPrefab, transform.position, transform.rotation);
                    behavior.laserChargeEffect.transform.parent = transform;
                    ScaleParticleSystemDuration component = behavior.laserChargeEffect.GetComponent<ScaleParticleSystemDuration>();
                    if (component) component.newDuration = self.duration;
                }
                if (self.laserPrefab)
                {
                    behavior.laserFireEffect = Object.Instantiate(self.laserPrefab, transform.position, transform.rotation);
                    behavior.laserFireEffect.transform.parent = transform;
                    behavior.laserLineEffect = behavior.laserFireEffect.GetComponent<LineRenderer>();
                }
                OptionSync(self, (networkIdentity, optionTracker) =>
                {
                    optionTracker.aurelioniteNetIds.Add(Tuple.Create(
                        SyncAurelioniteEffectsForClients.MessageType.CreateLaserCharge, networkIdentity.netId, (short)behavior.numbering,
                        self.duration, Vector3.zero, 0f
                    ));
                });
            });
        }

        private void ChargeMegaLaser_OnExit(On.EntityStates.TitanMonster.ChargeMegaLaser.orig_OnExit orig, ChargeMegaLaser self)
        {
            orig(self);
            FireForAllMinions(self, (option, behavior, target) =>
            {
                if (behavior.laserChargeEffect) EntityState.Destroy(behavior.laserChargeEffect);
                if (behavior.laserFireEffect) EntityState.Destroy(behavior.laserFireEffect);
                if (behavior.laserLineEffect) EntityState.Destroy(behavior.laserLineEffect);
                OptionSync(self, (networkIdentity, optionTracker) =>
                {
                    optionTracker.aurelioniteNetIds.Add(Tuple.Create(
                        SyncAurelioniteEffectsForClients.MessageType.DestroyLaserCharge, networkIdentity.netId, (short)behavior.numbering,
                        0f, Vector3.zero, 0f
                    ));
                });
            });
        }

        private void ChargeMegaLaser_Update(On.EntityStates.TitanMonster.ChargeMegaLaser.orig_Update orig, ChargeMegaLaser self)
        {
            orig(self);
            FireForAllMinions(self, (option, behavior, target) =>
            {
                if (behavior.laserFireEffect && behavior.laserLineEffect)
                {
                    float range = 1000f;
                    Vector3 position = option.transform.position;
                    Vector3 direction = self.GetAimRay().direction;
                    if (target) direction = target.transform.position - position;
                    else if (self.lockedOnHurtBox) direction = self.lockedOnHurtBox.transform.position - position;

                    Vector3 point = direction.normalized * range;
                    if (Physics.Raycast(position, point, out RaycastHit raycastHit, range, LayerIndex.world.mask | LayerIndex.defaultLayer.mask))
                    {
                        point = raycastHit.point;
                    }

                    behavior.laserLineEffect.SetPosition(0, position);
                    behavior.laserLineEffect.SetPosition(1, point);

                    float indicatorSize;
                    if (self.duration - self.age > .5f) indicatorSize = self.age / self.duration;
                    else indicatorSize = (self.laserOn ? 1f : 0f);
                    indicatorSize *= ChargeMegaLaser.laserMaxWidth;
                    behavior.laserLineEffect.startWidth = indicatorSize;
                    behavior.laserLineEffect.endWidth = indicatorSize;

                    OptionSync(self, (networkIdentity, optionTracker) =>
                    {
                        if (!optionTracker.aurelioniteNetIds.Exists((t) =>
                        {
                            return t.Item1 == SyncAurelioniteEffectsForClients.MessageType.UpdateLaserCharge
                                   && t.Item2 == networkIdentity.netId && t.Item3 == (short)behavior.numbering;
                        }))
                        {
                            optionTracker.aurelioniteNetIds.Add(Tuple.Create(
                                SyncAurelioniteEffectsForClients.MessageType.UpdateLaserCharge, networkIdentity.netId, (short)behavior.numbering,
                                0f, point, indicatorSize
                            ));
                        }
                    });
                }
            });
        }

        private void FireMegaLaser_OnEnter(On.EntityStates.TitanMonster.FireMegaLaser.orig_OnEnter orig, FireMegaLaser self)
        {
            orig(self);
            if (!self.laserPrefab) return;
            FireForAllMinions(self, (option, behavior, target) =>
            {
                if (self.laserPrefab)
                {
                    Transform transform = option.transform;
                    behavior.laserFireEffect = Object.Instantiate(self.laserPrefab, transform.position, transform.rotation);
                    behavior.laserFireEffect.transform.parent = transform;
                    behavior.laserChildLocator = behavior.laserFireEffect.GetComponent<ChildLocator>();
                    behavior.laserFireEffectEnd = behavior.laserChildLocator.FindChild("LaserEnd");
                }
                OptionSync(self, (networkIdentity, optionTracker) =>
                {
                    optionTracker.aurelioniteNetIds.Add(Tuple.Create(
                        SyncAurelioniteEffectsForClients.MessageType.CreateLaserFire, networkIdentity.netId, (short)behavior.numbering,
                        0f, Vector3.zero, 0f
                    ));
                });
            });
        }

        private void FireMegaLaser_OnExit(On.EntityStates.TitanMonster.FireMegaLaser.orig_OnExit orig, FireMegaLaser self)
        {
            orig(self);
            FireForAllMinions(self, (option, behavior, target) =>
            {
                if (behavior.laserFireEffect) EntityState.Destroy(behavior.laserFireEffect);
                if (behavior.laserChildLocator) EntityState.Destroy(behavior.laserChildLocator);
                if (behavior.laserFireEffectEnd) EntityState.Destroy(behavior.laserFireEffectEnd);
                OptionSync(self, (networkIdentity, optionTracker) =>
                {
                    optionTracker.aurelioniteNetIds.Add(Tuple.Create(
                        SyncAurelioniteEffectsForClients.MessageType.DestroyLaserFire, networkIdentity.netId, (short)behavior.numbering,
                        0f, Vector3.zero, 0f
                    ));
                });
            });
        }

        private void FireGoldMegaLaser_FixedUpdate(On.EntityStates.TitanMonster.FireGoldMegaLaser.orig_FixedUpdate orig, FireGoldMegaLaser self)
        {
            float oldFireStopwatch = self.fireStopwatch + Time.fixedDeltaTime;
            float oldProjectileStopwatch = self.projectileStopwatch + Time.fixedDeltaTime * self.attackSpeedStat;
            orig(self);
            if (self.isAuthority)
            {
                if (!self.lockedOnHurtBox && self.foundAnyTarget) return;
                if ((!self.inputBank || !self.inputBank.skill4.down) && self.stopwatch > FireMegaLaser.minimumDuration) return;
                if (self.stopwatch > FireMegaLaser.maximumDuration) return;
            }
            FireForAllMinions(self, (option, behavior, target) =>
            {
                Vector3 position = option.transform.position;
                Vector3 direction = self.GetAimRay().direction;
                if (target) direction = target.transform.position - position;
                else if (self.lockedOnHurtBox) direction = self.lockedOnHurtBox.transform.position - position;

                Vector3 point = direction.normalized * FireMegaLaser.maxDistance;
                if (Physics.Raycast(position, point, out RaycastHit raycastHit1, FireMegaLaser.maxDistance,
                                    LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore))
                {
                    point = raycastHit1.point;
                }
                Ray ray = new Ray(position, point - position);
                bool flag = false;
                if (behavior.laserFireEffect && behavior.laserChildLocator)
                {
                    if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit raycastHit2, ray.direction.magnitude,
                                        LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
                    {
                        point = raycastHit2.point;
                        if (Physics.Raycast(point - ray.direction * .1f, -ray.direction, out RaycastHit raycastHit3, raycastHit2.distance,
                                            LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal))
                        {
                            point = ray.GetPoint(0.1f);
                            flag = true;
                        }
                    }
                    behavior.laserFireEffect.transform.rotation = Util.QuaternionSafeLookRotation(point - position);
                    behavior.laserFireEffectEnd.transform.position = point;
                }

                if (oldFireStopwatch > 1f / FireMegaLaser.fireFrequency)
                {
                    if (!flag) self.FireBullet(option.transform, ray, "MuzzleLaser", (point - ray.origin).magnitude + .1f);
                }
                if (self.isAuthority && oldProjectileStopwatch >= 1f / FireGoldMegaLaser.projectileFireFrequency)
                {
                    direction = Util.ApplySpread(direction, FireGoldMegaLaser.projectileMinSpread, FireGoldMegaLaser.projectileMaxSpread, 1f, 1f, 0f, 0f);
                    ProjectileManager.instance.FireProjectile(FireGoldMegaLaser.projectilePrefab, position, Util.QuaternionSafeLookRotation(direction),
                                                              self.gameObject, self.damageStat * FireMegaLaser.damageCoefficient, 0f,
                                                              Util.CheckRoll(self.critStat, self.characterBody.master), DamageColorIndex.Default, null, -1f);
                }
                OptionSync(self, (networkIdentity, optionTracker) =>
                {
                    optionTracker.aurelioniteNetIds.Add(Tuple.Create(
                        SyncAurelioniteEffectsForClients.MessageType.FixedUpdateGoldLaserFire, networkIdentity.netId, (short)behavior.numbering,
                        0f, point, 0f
                    ));
                });
            });
        }

        private void FireGoldFist_PlacePredictedAttack(On.EntityStates.TitanMonster.FireGoldFist.orig_PlacePredictedAttack orig, FireGoldFist self)
        {
            orig(self);
            FireForAllMinions(self, (option, behavior, target) =>
            {
                int fistNumber = 0;
                float multiplier = Helper.RotateMultiplier(behavior.owner.name);
                Vector3 predictedTargetPosition = self.predictedTargetPosition + behavior.DecidePosition(behavior.ownerOt.currentOptionAngle) * multiplier;
                Vector3 a = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f) * Vector3.forward;
                int halfCount = FireGoldFist.fistCount / 2;
                for (int i = -halfCount; i < halfCount; i++)
                {
                    Vector3 fistPosition = predictedTargetPosition + a * FireGoldFist.distanceBetweenFists * i;
                    if (Physics.Raycast(new Ray(fistPosition + Vector3.up * 30f, Vector3.down), out RaycastHit raycastHit, 60f,
                                        LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                    {
                        fistPosition = raycastHit.point;
                    }
                    self.PlaceSingleDelayBlast(fistPosition, FireGoldFist.delayBetweenFists * fistNumber++);
                }
            });
        }

        private void FireFist_OnEnter(On.EntityStates.TitanMonster.FireFist.orig_OnEnter orig, FireFist self)
        {
            orig(self);
            FireForAllMinions(self, (option, behavior, target) =>
            {
                behavior.fistChargeEffect = Object.Instantiate(self.chargeEffectPrefab, option.transform);
                OptionSync(self, (networkIdentity, optionTracker) =>
                {
                    optionTracker.aurelioniteNetIds.Add(Tuple.Create(
                        SyncAurelioniteEffectsForClients.MessageType.CreateFist, networkIdentity.netId, (short)behavior.numbering,
                        0f, Vector3.zero, 0f
                    ));
                });
            });
        }

        private void FireFist_OnExit(On.EntityStates.TitanMonster.FireFist.orig_OnExit orig, FireFist self)
        {
            orig(self);
            FireForAllMinions(self, (option, behavior, target) =>
            {
                if (behavior.fistChargeEffect) EntityState.Destroy(behavior.fistChargeEffect);
                OptionSync(self, (networkIdentity, optionTracker) =>
                {
                    optionTracker.aurelioniteNetIds.Add(Tuple.Create(
                        SyncAurelioniteEffectsForClients.MessageType.DestroyFist, networkIdentity.netId, (short)behavior.numbering,
                        0f, Vector3.zero, 0f
                    ));
                });
            });
        }

        private void TitanRockController_Start(On.RoR2.TitanRockController.orig_Start orig, TitanRockController self)
        {
            orig(self);
            OptionTracker tracker = self.ownerCharacterBody.GetComponent<OptionTracker>();
            if (tracker) self.fireInterval /= tracker.existingOptions.Count + 1;
        }

        private void LoopAllMinionOwnerships(CharacterMaster ownerMaster, Action<GameObject> actionToRun)
        {
            MinionOwnership[] minionOwnerships = Object.FindObjectsOfType<MinionOwnership>();
            foreach (MinionOwnership minionOwnership in minionOwnerships)
            {
                if (minionOwnership && minionOwnership.ownerMaster && minionOwnership.ownerMaster == ownerMaster)
                {
                    CharacterMaster minionMaster = minionOwnership.GetComponent<CharacterMaster>();
                    if (minionMaster && FilterMinions(minionMaster.name))
                    {
                        CharacterBody minionBody = minionMaster.GetBody();
                        if (minionBody)
                        {
                            GameObject minion = minionBody.gameObject;
                            actionToRun(minion);
                        }
                    }
                }
            }
        }

        private void FireForAllMinions(BaseState self, Action<GameObject, OptionBehavior, GameObject> actionToRun, bool needTarget = true)
        {
            OptionTracker optionTracker = self.characterBody.GetComponent<OptionTracker>();
            if (!optionTracker) return;

            GameObject target = null;
            if (needTarget)
            {
                target = self.characterBody.master.gameObject.GetComponent<BaseAI>().currentEnemy.gameObject;
                if (!target) return;
            }
            foreach (GameObject option in optionTracker.existingOptions)
            {
                OptionBehavior behavior = option.GetComponent<OptionBehavior>();
                if (behavior)
                {
                    if (includeModelInsideOrb && target)
                    {
                        behavior.target = target;
                        NetworkIdentity targetNetworkIdentity = target.GetComponent<NetworkIdentity>();
                        if (targetNetworkIdentity)
                        {
                            OptionSync(self, (networkIdentity, nullTracker) =>
                            {
                                optionTracker.targetIds.Add(Tuple.Create(
                                    GameObjectType.Body, networkIdentity.netId, (short)behavior.numbering, targetNetworkIdentity.netId
                                ));
                            }, false);
                        }
                    }
                    actionToRun(option, behavior, target);
                }
            }
        }

        private void OptionSync(BaseState self, Action<NetworkIdentity, OptionTracker> actionToRun, bool queryTracker = true)
        {
            if (!NetworkServer.active) return;
            NetworkIdentity networkIdentity = self.characterBody.gameObject.GetComponent<NetworkIdentity>();
            if (!networkIdentity) return;
            OptionTracker tracker = null;
            if (queryTracker) tracker = self.characterBody.gameObject.GetComponent<OptionTracker>();
            if (!queryTracker || tracker) actionToRun(networkIdentity, tracker);
        }

        private bool FilterMinions(string name) => MinionsList.Exists((item) => name.Contains(item));

        private void AssignAurelioniteOwner(CharacterMaster goldMaster)
        {
            if (!goldMaster.name.Contains("TitanGold")) return;
            CharacterMaster trueMaster = null;
            foreach (PlayerCharacterMasterController pcmc in PlayerCharacterMasterController.instances)
            {
                if (!trueMaster || pcmc.master.inventory.GetItemCount(ItemIndex.TitanGoldDuringTP) > trueMaster.inventory.GetItemCount(ItemIndex.TitanGoldDuringTP))
                {
                    trueMaster = pcmc.master;
                }
            }
            if (!trueMaster) return;
            goldMaster.minionOwnership.SetOwner(trueMaster);
            OptionMasterTracker tracker = OptionMasterTracker.GetOrCreateComponent(trueMaster);
            NetworkIdentity netTrueMaster = trueMaster.gameObject.GetComponent<NetworkIdentity>();
            NetworkIdentity netGoldMaster = goldMaster.gameObject.GetComponent<NetworkIdentity>();
            if (!netTrueMaster || !netGoldMaster)
            {
                Helper._.LogWarning("AssignAurelioniteOwner: Network Identity is missing!");
                return;
            }
            tracker.aurelioniteOwner = Tuple.Create(netTrueMaster.netId, netGoldMaster.netId);
        }
    }
}