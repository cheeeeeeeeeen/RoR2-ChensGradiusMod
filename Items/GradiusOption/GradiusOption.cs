using EntityStates;
using EntityStates.BeetleGuardMonster;
using EntityStates.Drone.DroneWeapon;
using EntityStates.Squid.SquidWeapon;
using EntityStates.TitanMonster;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Orbs;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TILER2;
using UnityEngine;
using UnityEngine.Networking;
using static Chen.GradiusMod.SyncOptionTargetForClients;
using static TILER2.MiscUtil;
using MageWeapon = EntityStates.Mage.Weapon;
using Object = UnityEngine.Object;

namespace Chen.GradiusMod
{
    public class GradiusOption : Item_V2<GradiusOption>
    {
        public override string displayName => "Gradius' Option";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility });

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage multiplier of Options/Multiples. Also applies for Healing Drones. 1 = 100%. Server only.", AutoConfigFlags.None, 0f, float.MaxValue)]
        public float damageMultiplier { get; private set; } = 1f;

        [AutoConfig("Whether to support Auelionite in using Options. Set to true to enable. All attacks of Aurelionite will be copied by the Options/Multiples. " +
                    "Server and Client.", AutoConfigFlags.PreventNetMismatch)]
        public bool allowAurelionite { get; private set; } = false;

        [AutoConfig("Whether to support Beetle Guards in using Options. Set to true to enable. Only their ranged attacks are copied. " +
                    "Their melee damage is multiplied instead. Server and Client.", AutoConfigFlags.PreventNetMismatch)]
        public bool allowBeetleGuard { get; private set; } = false;

        [AutoConfig("Whether to support Squid Turrets in using Options. Set to false to disable. Squid Polyps may have weird interactrions with other mods. " +
                    "Server and Client. Disable if the Options/Multiples are misbehaving.", AutoConfigFlags.PreventNetMismatch)]
        public bool allowSquidPolyp { get; private set; } = true;

        [AutoConfig("Determines the Option/Multiple movement type. 0 = Regular, 1 = Rotate. Server and Client.",
                    AutoConfigFlags.PreventNetMismatch, 0, 1)]
        public int beetleGuardOptionType { get; private set; } = 1;

        [AutoConfig("Set to true for Options/Multiples of Flame Drones to generate a flamethrower sound. Client only. WARNING: Turning this on may cause earrape.")]
        public bool flamethrowerSoundCopy { get; private set; } = false;

        [AutoConfig("Set to true for Options/Multiples of Gatling Turrets to generate a firing sound. Client only. WARNING: Turning this on may cause earrape.")]
        public bool gatlingSoundCopy { get; private set; } = false;

        [AutoConfig("Set to true for Options/Multiples of Gunner Drones to generate a firing sound. Client only. WARNING: Turning this on may cause earrape.")]
        public bool gunnerSoundCopy { get; private set; } = false;

        [AutoConfig("Set to true for Options/Multiples of TC-280 drones to generate gun shot sounds. Client only. WARNING: Turning this on may cause earrape.")]
        public bool tc280SoundCopy { get; private set; } = false;

        [AutoConfig("Set to true for Options/Multiples of Aurelionite to generate a mega laser sound. Client only. WARNING: Turning this on may cause earrape.")]
        public bool aurelioniteMegaLaserSoundCopy { get; private set; } = false;

        [AutoConfig("Set to true for Options/Multiples of Beetle Guards to generate sound effects upon charging. Client only. WARNING: Turning this on may cause earrape.")]
        public bool beetleGuardChargeSoundCopy { get; private set; } = false;

        [AutoConfig("Allows displaying and syncing the flamethrower effect of Options/Multiples. Disabling this will replace the effect with bullets. " +
                    "Damage will stay the same. Server and Client. The server and client must have the same settings for an optimized experience. " +
                    "Disable this if you are experiencing FPS drops or network lag.", AutoConfigFlags.PreventNetMismatch)]
        public bool flamethrowerOptionSyncEffect { get; private set; } = true;

        [AutoConfig("Allows displaying and syncing some of Aurelionite's Options/Multiples. This reduces the effects generated. Damage will stay the same. " +
                    "Server and Client. The server and client must have the same settings for an optimized experience. " +
                    "Disable this if you are experiencing FPS drops or network lag.", AutoConfigFlags.PreventNetMismatch)]
        public bool aurelioniteOptionSyncEffect { get; private set; } = true;

        [AutoConfig("Allows displaying and syncing some of allied Beelte Guards' Options/Multiples. This reduces the effects generated. Damage will stay the same. " +
                    "Server and Client. The server and client must have the same settings for an optimized experience. " +
                    "Disable this if you are experiencing FPS drops or network lag.", AutoConfigFlags.PreventNetMismatch)]
        public bool beetleGuardOptionSyncEffect { get; private set; } = true;

        [AutoConfig("Set to true for the Orbs to have the Option Pickup model in the center. Server and Client. Cosmetic only. " +
                    "Turning this off could lessen resource usage.", AutoConfigFlags.PreventNetMismatch)]
        public bool includeModelInsideOrb { get; private set; } = true;

        [AutoConfig("Play a sound effect when an Option is acquired. 0 = disabled, 1 = Play sound in Owner, 2 = Play sound for all Drones. Client only.",
                    AutoConfigFlags.None, 0, 2)]
        public int playOptionGetSoundEffect { get; private set; } = 2;

        [AutoConfig("For Equipment Drones. Determines the number of uses the Equipment Drone will perform the equipment's effect. e.g. " +
                    "A value of 0.3 and having an Option number of 4 will result to the Equipment Drone performing the effect 1 more time. " +
                    "(4 * 0.3 = 1.2 -> Floored to 1). Having a value of 0 will disable the config.", AutoConfigFlags.None, 0f, 1f)]
        public float equipmentDuplicationMultiplier { get; private set; } = .5f;

        public override bool itemIsAIBlacklisted { get; protected set; } = true;

        protected override string GetNameString(string langid = null) => displayName;

        protected override string GetPickupString(string langid = null) => $"Deploy the Option, an ultimate weapon from the Gradius Federation, for each owned Drone.";

        protected override string GetDescString(string langid = null)
        {
            return $"Deploy <style=cIsDamage>1</style> <style=cStack>(+1 per stack)</style> Option for <style=cIsDamage>each drone you own</style>. " +
                   $"Options will copy all the attacks of the drone for <style=cIsDamage>{Pct(damageMultiplier, 0)}</style> of the damage dealt.";
        }

        protected override string GetLoreString(string langid = null) =>
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

        internal static GameObject gradiusOptionPrefab { get; private set; }
        internal static GameObject flamethrowerEffectPrefab { get; private set; }
        internal static GameObject laserChargeEffectPrefab { get; private set; }
        internal static GameObject laserChargeLaserPrefab { get; private set; }
        internal static GameObject laserFireLaserPrefab { get; private set; }
        internal static GameObject fistChargeEffectPrefab { get; private set; }
        internal static uint getOptionSoundId { get; } = 649757048;
        internal static uint getOptionLowSoundId { get; } = 553829614;
        internal static uint loseOptionSoundId { get; } = 2603869165;
        internal static uint loseOptionLowSoundId { get; } = 4084766013;

        private static readonly List<string> MinionsList = new List<string>
        {
            "DroneBackup",
            "Drone1",
            "Drone2",
            "EmergencyDrone",
            "FlameDrone",
            "MegaDrone",
            "DroneMissile",
            "Turret1",
            "TitanGoldAlly",
            "BeetleGuardAlly",
            "SquidTurret",
            "EquipmentDrone"
        };

        private static readonly List<string> RotateUsers = new List<string>
        {
            "Turret1",
            "TitanGoldAlly",
            "BeetleGuardAlly",
            "SquidTurret",
            "EquipmentDrone"
        };

        private static readonly Dictionary<string, float> RotateMultipliers = new Dictionary<string, float>
        {
            { "TitanGoldAlly", 12f },
            { "BeetleGuardAlly", 4f }
        };

        private static readonly Dictionary<string, Vector3> RotateOffsets = new Dictionary<string, Vector3>
        {
            { "SquidTurret", Vector3.up },
            { "EquipmentDrone", Vector3.down * 1.3f }
        };

        public GradiusOption()
        {
            modelResourcePath = "@ChensGradiusMod:assets/option/model/optionmodel.prefab";
            iconResourcePath = "@ChensGradiusMod:assets/option/icon/gradiusoption_icon.png";
        }

        public override void SetupConfig()
        {
            base.SetupConfig();
            if (!allowAurelionite)
            {
                MinionsList.Remove("TitanGoldAlly");
                aurelioniteMegaLaserSoundCopy = false;
                aurelioniteOptionSyncEffect = false;
            }
            if (!allowBeetleGuard)
            {
                MinionsList.Remove("BeetleGuardAlly");
                beetleGuardChargeSoundCopy = false;
                beetleGuardOptionSyncEffect = false;
            }
            if (beetleGuardOptionType != 1) RotateUsers.Remove("BeetleGuardAlly");
            if (!allowSquidPolyp) MinionsList.Remove("SquidTurret");
            if (equipmentDuplicationMultiplier <= 0) MinionsList.Remove("EquipmentDrone");
        }

        public override void SetupBehavior()
        {
            InitializeAssets();
            RegisterNetworkMessages();
            if (Compat_ItemStats.enabled)
            {
                Compat_ItemStats.CreateItemStatDef(itemDef,
                (
                    (count, inv, master) => { return count; },
                    (value, inv, master) => { return $"Options per Drone: {value}"; }
                ),
                (
                    (count, inv, master) => { return damageMultiplier; },
                    (value, inv, master) => { return $"Damage: {Pct(value, 0)}"; }
                ));
            }
        }

        public override void Install()
        {
            base.Install();
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
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
            On.EntityStates.Squid.SquidWeapon.FireSpine.FireOrbArrow += FireSpine_FireOrbArrow;
            On.EntityStates.BeetleGuardMonster.FireSunder.OnEnter += FireSunder_OnEnter;
            On.EntityStates.BeetleGuardMonster.FireSunder.OnExit += FireSunder_OnExit;
            On.EntityStates.BeetleGuardMonster.FireSunder.FixedUpdate += FireSunder_FixedUpdate;
            On.EntityStates.BeetleGuardMonster.GroundSlam.OnEnter += GroundSlam_OnEnter;
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.OnInventoryChanged -= CharacterBody_OnInventoryChanged;
            CharacterBody.onBodyStartGlobal -= CharacterBody_onBodyStartGlobal;
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
            On.EntityStates.Squid.SquidWeapon.FireSpine.FireOrbArrow -= FireSpine_FireOrbArrow;
            On.EntityStates.BeetleGuardMonster.FireSunder.OnEnter -= FireSunder_OnEnter;
            On.EntityStates.BeetleGuardMonster.FireSunder.OnExit -= FireSunder_OnExit;
            On.EntityStates.BeetleGuardMonster.FireSunder.FixedUpdate -= FireSunder_FixedUpdate;
            On.EntityStates.BeetleGuardMonster.GroundSlam.OnEnter -= GroundSlam_OnEnter;
            On.RoR2.EquipmentSlot.PerformEquipmentAction -= EquipmentSlot_PerformEquipmentAction;
        }

        private void InitializeAssets()
        {
            itemDef.pickupModelPrefab.transform.localScale *= 2f;

            string path;
            if (includeModelInsideOrb) path = "@ChensGradiusMod:assets/option/orb/optionorbwithmodel.prefab";
            else path = "@ChensGradiusMod:assets/option/orb/optionorb.prefab";
            gradiusOptionPrefab = Resources.Load<GameObject>(path);
            if (gradiusOptionPrefab)
            {
                gradiusOptionPrefab.AddComponent<NetworkIdentity>();
                gradiusOptionPrefab.AddComponent<OptionBehavior>();
                gradiusOptionPrefab.AddComponent<Flicker>();
                Log.Debug("Successfully initialized OptionOrb prefab.");
            }
            else Log.Error("Failed to create GradiusOption: Resource not found or is null.");

            flamethrowerEffectPrefab = Resources.Load<GameObject>("prefabs/effects/DroneFlamethrowerEffect");
            laserChargeEffectPrefab = Resources.Load<GameObject>("Assets/PrefabInstance/ChargeGolemGold.prefab");
            ChargeMegaLaser cmlState = EntityState.Instantiate(typeof(ChargeMegaLaser)) as ChargeMegaLaser;
            laserChargeEffectPrefab = cmlState.effectPrefab;
            laserChargeLaserPrefab = cmlState.laserPrefab;
            FireMegaLaser fmlState = EntityState.Instantiate(typeof(FireMegaLaser)) as FireMegaLaser;
            laserFireLaserPrefab = fmlState.laserPrefab;
            FireFist ffState = EntityState.Instantiate(typeof(FireFist)) as FireFist;
            fistChargeEffectPrefab = ffState.chargeEffectPrefab;
        }

        private void RegisterNetworkMessages()
        {
            Log.Debug("Registering custom network messages needed for GradiusOption...");
            NetworkingAPI.RegisterMessageType<SyncFlamethrowerEffectForClients>();
            NetworkingAPI.RegisterMessageType<SyncOptionTargetForClients>();
            NetworkingAPI.RegisterMessageType<SyncAurelioniteEffectsForClients>();
            NetworkingAPI.RegisterMessageType<SyncBeetleGuardEffectsForClients>();
            NetworkingAPI.RegisterMessageType<SyncSimpleSound>();
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody obj)
        {
            // This hook runs on Client and on Server
            CharacterMaster master = obj.master;
            if (FilterMinions(master) && master.minionOwnership)
            {
                AssignAurelioniteOwner(master);
                CharacterMaster masterMaster = master.minionOwnership.ownerMaster;
                if (masterMaster && GetCount(masterMaster) > 0)
                {
                    OptionMasterTracker masterTracker = OptionMasterTracker.GetOrCreateComponent(masterMaster);
                    for (int t = 1; t <= masterTracker.optionItemCount; t++) OptionMasterTracker.SpawnOption(obj.gameObject, t);
                }
            }
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
                    Log.Message($"OnInventoryChanged: OldCount: {oldCount}, NewCount: {newCount}, Difference: {diff}");
                    if (diff > 0)
                    {
                        if (playOptionGetSoundEffect == 1) AkSoundEngine.PostEvent(getOptionSoundId, self.gameObject);
                        LoopAllMinions(self.master, (minion) =>
                        {
                            if (playOptionGetSoundEffect == 2) AkSoundEngine.PostEvent(getOptionLowSoundId, minion);
                            for (int t = oldCount + 1; t <= newCount; t++) OptionMasterTracker.SpawnOption(minion, t);
                        });
                    }
                    else
                    {
                        if (playOptionGetSoundEffect == 1) AkSoundEngine.PostEvent(loseOptionSoundId, self.gameObject);
                        LoopAllMinions(self.master, (minion) =>
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
            FireForAllOptions(self, (option, behavior, target) =>
            {
                float healRate = (HealBeam.healCoefficient * self.damageStat / self.duration) * damageMultiplier;
                Transform transform = option.transform;
                if (transform && self.target)
                {
                    if (behavior.healBeamController) behavior.healBeamController.BreakServer();
                    GameObject gameObject = Object.Instantiate(HealBeam.healBeamPrefab, transform);
                    HealBeamController hbc = behavior.healBeamController = gameObject.GetComponent<HealBeamController>();
                    hbc.healRate = healRate;
                    hbc.target = self.target;
                    hbc.ownership.ownerObject = option.gameObject;
                    NetworkServer.Spawn(gameObject);
                }
            });
        }

        private void HealBeam_OnExit(On.EntityStates.Drone.DroneWeapon.HealBeam.orig_OnExit orig, HealBeam self)
        {
            orig(self);
            FireForAllOptions(self, (option, behavior, target) =>
            {
                if (behavior.healBeamController) behavior.healBeamController.BreakServer();
            });
        }

        private void StartHealBeam_OnEnter(On.EntityStates.Drone.DroneWeapon.StartHealBeam.orig_OnEnter orig, StartHealBeam self)
        {
            orig(self);
            if (!NetworkServer.active) return;
            FireForAllOptions(self, (option, behavior, target) =>
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
            });
        }

        private void Flamethrower_OnExit(On.EntityStates.Mage.Weapon.Flamethrower.orig_OnExit orig, MageWeapon.Flamethrower self)
        {
            orig(self);
            if (flamethrowerOptionSyncEffect && self.characterBody.name.Contains("FlameDrone") && self.characterBody.master.name.Contains("FlameDrone"))
            {
                FireForAllOptions(self, (option, behavior, target) =>
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
            if (flamethrowerOptionSyncEffect && self.characterBody.name.Contains("FlameDrone") && self.characterBody.master.name.Contains("FlameDrone"))
            {
                FireForAllOptions(self, (option, behavior, target) =>
                {
                    bool perMinionOldBegunFlamethrower = oldBegunFlamethrower;
                    if (self.stopwatch >= self.entryDuration && !perMinionOldBegunFlamethrower)
                    {
                        perMinionOldBegunFlamethrower = true;
                        if (flamethrowerSoundCopy) Util.PlaySound(MageWeapon.Flamethrower.startAttackSoundString, option);
                        if (behavior.flamethrower) EntityState.Destroy(behavior.flamethrower);
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
                    if (perMinionOldBegunFlamethrower && behavior.flamethrower && target)
                    {
                        Vector3 direction = (target.transform.position - option.transform.position).normalized;
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
                FireForAllOptions(self, (option, behavior, target) =>
                {
                    if (target && self.isAuthority)
                    {
                        BulletAttack attack = new BulletAttack
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
                        };
                        if (!flamethrowerOptionSyncEffect) attack.tracerEffectPrefab = FireGatling.tracerEffectPrefab;
                        attack.Fire();
                    }
                });
            }
        }

        private void FireGatling_OnEnter(On.EntityStates.Drone.DroneWeapon.FireGatling.orig_OnEnter orig, FireGatling self)
        {
            orig(self);
            FireForAllOptions(self, (option, behavior, target) =>
            {
                if (target)
                {
                    if (gatlingSoundCopy) Util.PlaySound(FireGatling.fireGatlingSoundString, option);
                    OptionSync(self, (networkIdentity, optionTracker) =>
                    {
                        new SyncSimpleSound(networkIdentity.netId, (short)behavior.numbering, FireGatling.fireGatlingSoundString, -1).Send(NetworkDestination.Clients);
                    }, false);
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
                }
            });
        }

        private void FireTurret_OnEnter(On.EntityStates.Drone.DroneWeapon.FireTurret.orig_OnEnter orig, FireTurret self)
        {
            orig(self);
            FireForAllOptions(self, (option, behavior, target) =>
            {
                if (target)
                {
                    if (gunnerSoundCopy) Util.PlaySound(FireTurret.attackSoundString, option);
                    OptionSync(self, (networkIdentity, optionTracker) =>
                    {
                        new SyncSimpleSound(networkIdentity.netId, (short)behavior.numbering, FireTurret.attackSoundString, -1).Send(NetworkDestination.Clients);
                    }, false);
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
                }
            });
        }

        private void FireMegaTurret_FireBullet(On.EntityStates.Drone.DroneWeapon.FireMegaTurret.orig_FireBullet orig, FireMegaTurret self, string muzzleString)
        {
            orig(self, muzzleString);
            FireForAllOptions(self, (option, behavior, target) =>
            {
                if (target)
                {
                    if (tc280SoundCopy) Util.PlayScaledSound(FireMegaTurret.attackSoundString, option, FireMegaTurret.attackSoundPlaybackCoefficient);
                    OptionSync(self, (networkIdentity, optionTracker) =>
                    {
                        new SyncSimpleSound(networkIdentity.netId, (short)behavior.numbering, FireTurret.attackSoundString,
                                            FireMegaTurret.attackSoundPlaybackCoefficient).Send(NetworkDestination.Clients);
                    }, false);
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
                }
            });
        }

        private void FireMissileBarrage_FireMissile(On.EntityStates.Drone.DroneWeapon.FireMissileBarrage.orig_FireMissile orig, FireMissileBarrage self, string targetMuzzle)
        {
            orig(self, targetMuzzle);
            FireForAllOptions(self, (option, behavior, target) =>
            {
                if (FireMissileBarrage.effectPrefab)
                {
                    EffectManager.SimpleMuzzleFlash(FireMissileBarrage.effectPrefab, option, targetMuzzle, true);
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
            FireForAllOptions(self, (option, behavior, target) =>
            {
                if (FireTwinRocket.muzzleEffectPrefab)
                {
                    EffectManager.SimpleMuzzleFlash(FireTwinRocket.muzzleEffectPrefab, option, muzzleString, false);
                }
                if (self.isAuthority && FireTwinRocket.projectilePrefab != null)
                {
                    float maxDistance = 1000f;
                    Vector3 forward = self.GetAimRay().direction;
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
            if (!aurelioniteOptionSyncEffect) return;
            FireForAllOptions(self, (option, behavior, target) =>
            {
                Transform transform = option.transform;
                if (self.effectPrefab)
                {
                    if (behavior.laserChargeEffect) EntityState.Destroy(behavior.laserChargeEffect);
                    behavior.laserChargeEffect = Object.Instantiate(self.effectPrefab, transform.position, transform.rotation);
                    behavior.laserChargeEffect.transform.parent = transform;
                    ScaleParticleSystemDuration component = behavior.laserChargeEffect.GetComponent<ScaleParticleSystemDuration>();
                    if (component) component.newDuration = self.duration;
                }
                if (self.laserPrefab)
                {
                    if (behavior.laserFireEffect) EntityState.Destroy(behavior.laserFireEffect);
                    if (behavior.laserLineEffect) EntityState.Destroy(behavior.laserLineEffect);
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
            if (!aurelioniteOptionSyncEffect) return;
            FireForAllOptions(self, (option, behavior, target) =>
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
            if (!aurelioniteOptionSyncEffect) return;
            FireForAllOptions(self, (option, behavior, target) =>
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
            if (!aurelioniteOptionSyncEffect || !self.laserPrefab) return;
            FireForAllOptions(self, (option, behavior, target) =>
            {
                if (aurelioniteMegaLaserSoundCopy)
                {
                    Util.PlaySound(FireMegaLaser.playAttackSoundString, option);
                    Util.PlaySound(FireMegaLaser.playLoopSoundString, option);
                }
                if (self.laserPrefab)
                {
                    if (behavior.laserFire) EntityState.Destroy(behavior.laserFire);
                    if (behavior.laserChildLocator) EntityState.Destroy(behavior.laserChildLocator);
                    if (behavior.laserFireEnd) EntityState.Destroy(behavior.laserFireEnd);
                    Transform transform = option.transform;
                    behavior.laserFire = Object.Instantiate(self.laserPrefab, transform.position, transform.rotation);
                    behavior.laserFire.transform.parent = transform;
                    behavior.laserChildLocator = behavior.laserFire.GetComponent<ChildLocator>();
                    behavior.laserFireEnd = behavior.laserChildLocator.FindChild("LaserEnd");
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
            if (!aurelioniteOptionSyncEffect) return;
            FireForAllOptions(self, (option, behavior, target) =>
            {
                if (aurelioniteMegaLaserSoundCopy) Util.PlaySound(FireMegaLaser.stopLoopSoundString, option);
                if (behavior.laserFire) EntityState.Destroy(behavior.laserFire);
                if (behavior.laserChildLocator) EntityState.Destroy(behavior.laserChildLocator);
                if (behavior.laserFireEnd) EntityState.Destroy(behavior.laserFireEnd);
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
            FireForAllOptions(self, (option, behavior, target) =>
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
                if (behavior.laserFire && behavior.laserChildLocator)
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
                    behavior.laserFire.transform.rotation = Util.QuaternionSafeLookRotation(point - position);
                    behavior.laserFireEnd.transform.position = point;
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
            FireForAllOptions(self, (option, behavior, target) =>
            {
                int fistNumber = 0;
                float multiplier = behavior.ownerOt.GetRotateMultiplier();
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
            if (!aurelioniteOptionSyncEffect) return;
            FireForAllOptions(self, (option, behavior, target) =>
            {
                if (behavior.fistChargeEffect) EntityState.Destroy(behavior.fistChargeEffect);
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
            if (!aurelioniteOptionSyncEffect) return;
            FireForAllOptions(self, (option, behavior, target) =>
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

        private void FireSpine_FireOrbArrow(On.EntityStates.Squid.SquidWeapon.FireSpine.orig_FireOrbArrow orig, FireSpine self)
        {
            bool oldFireArrow = self.hasFiredArrow;
            orig(self);
            if (oldFireArrow || !NetworkServer.active) return;
            FireForAllOptions(self, (option, behavior, target) =>
            {
                HurtBox hurtBox = self.enemyFinder.GetResults().FirstOrDefault();
                SquidOrb squidOrb = new SquidOrb
                {
                    damageValue = self.characterBody.damage * FireSpine.damageCoefficient,
                    isCrit = Util.CheckRoll(self.characterBody.crit, self.characterBody.master),
                    teamIndex = TeamComponent.GetObjectTeam(self.gameObject),
                    attacker = self.gameObject,
                    procCoefficient = FireSpine.damageCoefficient,
                    origin = option.transform.position,
                    target = hurtBox
                };
                if (FireSpine.muzzleflashEffectPrefab)
                {
                    EffectManager.SimpleMuzzleFlash(FireSpine.muzzleflashEffectPrefab, option, "Muzzle", true);
                }
                OrbManager.instance.AddOrb(squidOrb);
            });
        }

        private void FireSunder_OnEnter(On.EntityStates.BeetleGuardMonster.FireSunder.orig_OnEnter orig, FireSunder self)
        {
            orig(self);
            if (!beetleGuardOptionSyncEffect) return;
            FireForAllOptions(self, (option, behavior, target) =>
            {
                if (behavior.sunderEffect) EntityState.Destroy(behavior.sunderEffect);
                if (beetleGuardChargeSoundCopy) Util.PlaySound(FireSunder.initialAttackSoundString, option);
                if (FireSunder.chargeEffectPrefab) behavior.sunderEffect = Object.Instantiate(FireSunder.chargeEffectPrefab, option.transform);
                OptionSync(self, (networkIdentity, optionTracker) =>
                {
                    optionTracker.guardNetIds.Add(Tuple.Create(
                        SyncBeetleGuardEffectsForClients.MessageType.Create, networkIdentity.netId, (short)behavior.numbering
                    ));
                });
            });
        }

        private void FireSunder_OnExit(On.EntityStates.BeetleGuardMonster.FireSunder.orig_OnExit orig, FireSunder self)
        {
            orig(self);
            if (!beetleGuardOptionSyncEffect) return;
            FireForAllOptions(self, (option, behavior, target) =>
            {
                if (behavior.sunderEffect) EntityState.Destroy(behavior.sunderEffect);
                OptionSync(self, (networkIdentity, optionTracker) =>
                {
                    optionTracker.guardNetIds.Add(Tuple.Create(
                        SyncBeetleGuardEffectsForClients.MessageType.Destroy, networkIdentity.netId, (short)behavior.numbering
                    ));
                });
            });
        }

        private void FireSunder_FixedUpdate(On.EntityStates.BeetleGuardMonster.FireSunder.orig_FixedUpdate orig, FireSunder self)
        {
            bool oldHasAttacked = self.hasAttacked;
            orig(self);
            FireForAllOptions(self, (option, behavior, target) =>
            {
                if (self.modelAnimator && self.modelAnimator.GetFloat("FireSunder.activate") > 0.5f && !oldHasAttacked)
                {
                    if (self.isAuthority && self.modelTransform && FireSunder.projectilePrefab)
                    {
                        Ray aimRay;
                        if (target) aimRay = new Ray(option.transform.position, (target.transform.position - option.transform.position).normalized);
                        else aimRay = new Ray(option.transform.position, self.GetAimRay().direction);
                        ProjectileManager.instance.FireProjectile(FireSunder.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction),
                                                                  self.gameObject, self.damageStat * FireSunder.damageCoefficient, FireSunder.forceMagnitude,
                                                                  Util.CheckRoll(self.critStat, self.characterBody.master), DamageColorIndex.Default, null, -1f);
                    }
                    if (beetleGuardOptionSyncEffect)
                    {
                        if (behavior.sunderEffect) EntityState.Destroy(behavior.sunderEffect);
                        OptionSync(self, (networkIdentity, optionTracker) =>
                        {
                            optionTracker.guardNetIds.Add(Tuple.Create(
                                SyncBeetleGuardEffectsForClients.MessageType.Destroy, networkIdentity.netId, (short)behavior.numbering
                            ));
                        });
                    }
                }
            });
        }

        private void GroundSlam_OnEnter(On.EntityStates.BeetleGuardMonster.GroundSlam.orig_OnEnter orig, GroundSlam self)
        {
            orig(self);
            OptionTracker tracker = self.characterBody.GetComponent<OptionTracker>();
            if (tracker) self.attack.damage *= tracker.existingOptions.Count + 1;
        }

        private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentIndex equipmentIndex)
        {
            bool returnValue = orig(self, equipmentIndex);
            if (!returnValue) return false;
            CharacterBody body = self.characterBody;
            if (body)
            {
                CharacterMaster master = self.characterBody.master;
                if (master && master.name.Contains("EquipmentDrone"))
                {
                    OptionTracker tracker = OptionTracker.GetOrCreateComponent(body);
                    int numberOfTimes = Mathf.FloorToInt(tracker.existingOptions.Count * equipmentDuplicationMultiplier);
                    for (int i = 0; i < numberOfTimes; i++)
                    {
                        orig(self, equipmentIndex);
                    }
                }
            }
            return true;
        }

        private bool FilterMinions(CharacterMaster master) => master && MinionsList.Exists((item) => master.name.Contains(item));

        private void AssignAurelioniteOwner(CharacterMaster goldMaster)
        {
            if (!goldMaster.name.Contains("TitanGoldAlly")) return;
            CharacterMaster trueMaster = null;
            foreach (PlayerCharacterMasterController pcmc in PlayerCharacterMasterController.instances)
            {
                if (!trueMaster || pcmc.master.inventory.GetItemCount(ItemIndex.TitanGoldDuringTP) > trueMaster.inventory.GetItemCount(ItemIndex.TitanGoldDuringTP))
                {
                    trueMaster = pcmc.master;
                }
            }
            if (!trueMaster) return;
            if (NetworkServer.active) goldMaster.minionOwnership.SetOwner(trueMaster);
            else
            {
                NetworkIdentity netTrueMaster = trueMaster.gameObject.GetComponent<NetworkIdentity>();
                if (!netTrueMaster)
                {
                    Log.Warning("AssignAurelioniteOwner: Network Identity is missing!");
                    return;
                }
                goldMaster.minionOwnership.ownerMasterId = netTrueMaster.netId;
                MinionOwnership.MinionGroup.SetMinionOwner(goldMaster.minionOwnership, netTrueMaster.netId);
            }
        }

        internal bool IsRotateUser(string masterName) => RotateUsers.Exists((name) => masterName.Contains(name));

        internal float GetRotateMultiplier(string name)
        {
            float multiplier = 0f;
            foreach (var pair in RotateMultipliers)
            {
                if (name.Contains(pair.Key))
                {
                    multiplier += pair.Value;
                }
            }
            if (multiplier <= 0f) multiplier = 1f;
            return multiplier;
        }

        internal Vector3 GetRotateOffset(string name)
        {
            Vector3 offset = Vector3.zero;
            foreach (var pair in RotateOffsets)
            {
                if (name.Contains(pair.Key))
                {
                    offset += pair.Value;
                }
            }
            return offset;
        }

        /// <summary>
        /// Adds a support for a minion for them to gain Options.
        /// </summary>
        /// <param name="masterName">The CharacterMaster name of the minion.</param>
        /// <returns>True if the minion is supported. False if it is already supported.</returns>
        public bool SupportMinionType(string masterName)
        {
            if (MinionsList.Contains(masterName)) return false;
            MinionsList.Add(masterName);
            return true;
        }

        /// <summary>
        /// Lets the minion use Rotate Options.
        /// </summary>
        /// <param name="masterName">The CharacterMaster name of the minion.</param>
        /// <returns>True if the minion is successfully set to use Rotate Options. False if it is already using Rotate Options.</returns>
        public bool SetToRotateOptions(string masterName)
        {
            if (RotateUsers.Contains(masterName)) return false;
            RotateUsers.Add(masterName);
            return true;
        }

        /// <summary>
        /// Sets the rotation multiplier for a minion type. This multiplier affects the distance and speed of rotation.
        /// </summary>
        /// <param name="masterName">The CharacterMaster name of the minion.</param>
        /// <param name="newValue">The multiplier value.</param>
        /// <returns>True if the values are set. False if not.</returns>
        public bool SetRotateOptionMultiplier(string masterName, float newValue)
        {
            if (IsRotateUser(masterName))
            {
                RotateMultipliers[masterName] = newValue;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the offset center position for a minion type. Options will rotate around the offset.
        /// </summary>
        /// <param name="masterName">The CharacterMaster name of the minion.</param>
        /// <param name="newValue">The offset value.</param>
        /// <returns>True if the values are set. False if not.</returns>
        public bool SetRotateOptionOffset(string masterName, Vector3 newValue)
        {
            if (IsRotateUser(masterName))
            {
                RotateOffsets[masterName] = newValue;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Lets the minion use Regular Options.
        /// </summary>
        /// <param name="masterName">The CharacterMaster name of the minion.</param>
        /// <returns>True if the minion is successfully set to use Regular Options. False if it is already using Regular Options.</returns>
        public bool SetToRegularOptions(string masterName)
        {
            if (!RotateUsers.Contains(masterName)) return false;
            RotateUsers.Remove(masterName);
            RotateMultipliers.Remove(masterName);
            RotateOffsets.Remove(masterName);
            return true;
        }

        /// <summary>
        /// Loops through the all the minions of the owner.
        /// </summary>
        /// <param name="ownerMaster">The owner of the minions.</param>
        /// <param name="actionToRun">An action to execute for each minion. The minion's CharacterBody GameObject is given as the input.</param>
        public void LoopAllMinions(CharacterMaster ownerMaster, Action<GameObject> actionToRun)
        {
            MinionOwnership[] minionOwnerships = Object.FindObjectsOfType<MinionOwnership>();
            foreach (MinionOwnership minionOwnership in minionOwnerships)
            {
                if (minionOwnership && minionOwnership.ownerMaster && minionOwnership.ownerMaster == ownerMaster)
                {
                    CharacterMaster minionMaster = minionOwnership.GetComponent<CharacterMaster>();
                    if (FilterMinions(minionMaster))
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

        /// <summary>
        /// Loops through all the Options of the minion.
        /// </summary>
        /// <param name="entityState">The state of the of which the attack was made.</param>
        /// <param name="actionToRun">An action to execute for each Option. The inputs are as follows: GameObject option, OptionBehavior behavior, GameObject target.</param>
        public void FireForAllOptions(BaseState entityState, Action<GameObject, OptionBehavior, GameObject> actionToRun)
        {
            CharacterBody body = entityState.characterBody;
            if (!body) return;
            OptionTracker optionTracker = body.GetComponent<OptionTracker>();
            if (!optionTracker) return;

            GameObject target = null;
            GameObject masterObject = body.masterObject;
            if (masterObject)
            {
                BaseAI ai = masterObject.GetComponent<BaseAI>();
                BaseAI.Target mainTarget = ai.currentEnemy;
                if (mainTarget != null && mainTarget.gameObject)
                {
                    target = mainTarget.gameObject;
                }
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
                            OptionSync(entityState, (networkIdentity, nullTracker) =>
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

        /// <summary>
        /// Syncs the Option from the server to clients. Sync logic should be provided in actionToRun.
        /// </summary>
        /// <param name="entityState">The state of which the attack was made.</param>
        /// <param name="actionToRun">The sync action to perform. Inputs are as follows: NetworkIdentity optionIdentity, OptionTracker tracker.</param>
        /// <param name="queryTracker">If true, the Option tracker is automatically queried. If false, the Option tracker will not be queried.</param>
        public void OptionSync(BaseState entityState, Action<NetworkIdentity, OptionTracker> actionToRun, bool queryTracker = true)
        {
            if (!NetworkServer.active) return;
            NetworkIdentity networkIdentity = entityState.characterBody.gameObject.GetComponent<NetworkIdentity>();
            if (!networkIdentity) return;
            OptionTracker tracker = null;
            if (queryTracker) tracker = entityState.characterBody.gameObject.GetComponent<OptionTracker>();
            if (!queryTracker || tracker) actionToRun(networkIdentity, tracker);
        }
    }
}