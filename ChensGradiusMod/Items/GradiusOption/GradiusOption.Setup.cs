using Chen.GradiusMod.Items.GradiusOption.Components;
using Chen.Helpers.GeneralHelpers;
using R2API.Networking;
using RoR2;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TILER2;
using UnityEngine;
using UnityEngine.Networking;
using static Chen.GradiusMod.GradiusModPlugin;
using static TILER2.MiscUtil;
using RoR2Items = RoR2.RoR2Content.Items;

namespace Chen.GradiusMod.Items.GradiusOption
{
    /// <summary>
    /// An item class which provides the main API related to the Options/Multiples. It is powered by TILER2.
    /// </summary>
    public partial class GradiusOption : Item<GradiusOption>
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string displayName => "Gradius' Option";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility });

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage multiplier of Options/Multiples. Also applies for Healing Drones. 1 = 100%. Server only.", AutoConfigFlags.None, 0f, float.MaxValue)]
        public float damageMultiplier { get; private set; } = 1f;

        [AutoConfig("Whether to support Aurelionite in using Options. Set to true to enable. All attacks of Aurelionite will be copied by the Options/Multiples. " +
                    "Server and Client.", AutoConfigFlags.PreventNetMismatch)]
        public bool allowAurelionite { get; private set; } = false;

        [AutoConfig("Whether to support Beetle Guards in using Options. Set to true to enable. Only their ranged attacks are copied. " +
                    "Their melee damage is multiplied instead. Server and Client.", AutoConfigFlags.PreventNetMismatch)]
        public bool allowBeetleGuard { get; private set; } = false;

        [AutoConfig("Whether to support Squid Turrets in using Options. Set to false to disable. Squid Polyps may have weird interactions with other mods. " +
                    "Server and Client. Disable if the Options/Multiples are misbehaving.", AutoConfigFlags.PreventNetMismatch)]
        public bool allowSquidPolyp { get; private set; } = true;

        [AutoConfig("Whether to support Empathy Core probes in using Options. Set to false to disable." +
                    "Server and Client. Disable if the Options/Multiples are misbehaving.", AutoConfigFlags.PreventNetMismatch)]
        public bool allowEmpathyCores { get; private set; } = true;

        [AutoConfig("Determines the Option/Multiple movement type. 0 = Regular, 1 = Rotate. Server and Client.",
                    AutoConfigFlags.PreventNetMismatch, 0, 1)]
        public int beetleGuardOptionType { get; private set; } = 1;

        [AutoConfig("Allows displaying and syncing the flamethrower effect of Options/Multiples. Disabling this will replace the effect with bullets. " +
                    "Damage will stay the same. Server and Client. The server and client must have the same settings for an optimized experience. " +
                    "Disable this if you are experiencing FPS drops or network lag.", AutoConfigFlags.PreventNetMismatch)]
        public bool flamethrowerOptionSyncEffect { get; private set; } = true;

        [AutoConfig("Allows displaying and syncing some of Aurelionite's Options/Multiples. This reduces the effects generated. Damage will stay the same. " +
                    "Server and Client. The server and client must have the same settings for an optimized experience. " +
                    "Disable this if you are experiencing FPS drops or network lag.", AutoConfigFlags.PreventNetMismatch)]
        public bool aurelioniteOptionSyncEffect { get; private set; } = true;

        [AutoConfig("Allows displaying and syncing some of allied Beetle Guards' Options/Multiples. This reduces the effects generated. Damage will stay the same. " +
                    "Server and Client. The server and client must have the same settings for an optimized experience. " +
                    "Disable this if you are experiencing FPS drops or network lag.", AutoConfigFlags.PreventNetMismatch)]
        public bool beetleGuardOptionSyncEffect { get; private set; } = true;

        [AutoConfig("Allows displaying and syncing some of allied Empathy Cores' Options/Multiples. This reduces the effects generated. Damage will stay the same. " +
                    "Server and Client. The server and client must have the same settings for an optimized experience. " +
                    "Disable this if you are experiencing FPS drops or network lag.", AutoConfigFlags.PreventNetMismatch)]
        public bool empathyCoresOptionSyncEffect { get; private set; } = true;

        [AutoConfig("Set to true for the Orbs to have the Option Pickup model in the center. Server and Client. Cosmetic only. " +
                    "Turning this off could lessen resource usage.", AutoConfigFlags.PreventNetMismatch)]
        public bool includeModelInsideOrb { get; private set; } = true;

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
            "\"This is CASE, A.I. born from Project Victorious to aid in combating the evil known as the Bacterion Army.\n\n" +
            "Our specialized fighter spacecraft was destroyed from an incoming attack in an attempt to save the flight lead of the Scorpio Squadron. " +
            "It is unfortunate that the pilot herself, Katswell call-signed Scorpio 2, died from the explosion... her body disintegrated along with the spacecraft she pilots.\n\n" +
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

        internal const uint GetOptionEventId = 649757048;
        internal const uint LoseOptionEventId = 2603869165;

        internal static GameObject gradiusOptionPrefab { get; private set; }

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
            "EquipmentDrone",
            "RoboBallGreenBuddy",
            "RoboBallRedBuddy"
        };

        private static readonly List<string> RotateUsers = new List<string>
        {
            "Turret1",
            "TitanGoldAlly",
            "BeetleGuardAlly",
            "SquidTurret",
            "EquipmentDrone",
            "RoboBallGreenBuddy"
        };

        private static readonly Dictionary<string, float> RotateMultipliers = new Dictionary<string, float>
        {
            { "TitanGoldAlly", 12f },
            { "BeetleGuardAlly", 4f },
            { "RoboBallGreenBuddy", 4f }
        };

        private static readonly Dictionary<string, Vector3> RotateOffsets = new Dictionary<string, Vector3>
        {
            { "SquidTurret", Vector3.up },
            { "EquipmentDrone", Vector3.down * 1.3f },
            { "Turret1", Vector3.up }
        };

        public GradiusOption()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("assets/option/model/optionmodel.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("assets/option/icon/gradiusoption_icon.png");
        }

        public override void SetupConfig()
        {
            base.SetupConfig();
            if (!allowAurelionite)
            {
                MinionsList.Remove("TitanGoldAlly");
                aurelioniteOptionSyncEffect = false;
            }
            if (!allowBeetleGuard)
            {
                MinionsList.Remove("BeetleGuardAlly");
                beetleGuardOptionSyncEffect = false;
            }
            if (beetleGuardOptionType != 1) RotateUsers.Remove("BeetleGuardAlly");
            if (!allowSquidPolyp) MinionsList.Remove("SquidTurret");
            if (equipmentDuplicationMultiplier <= 0) MinionsList.Remove("EquipmentDrone");
            if (!allowEmpathyCores)
            {
                MinionsList.Remove("RoboBallGreenBuddy");
                MinionsList.Remove("RoboBallRedBuddy");
                empathyCoresOptionSyncEffect = false;
            }
        }

        public override void SetupBehavior()
        {
            base.SetupBehavior();
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

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private void InitializeAssets()
        {
            itemDef.pickupModelPrefab.transform.localScale *= 2f;

            string path;
            if (includeModelInsideOrb) path = "assets/option/orb/optionorbwithmodel.prefab";
            else path = "assets/option/orb/optionorb.prefab";
            gradiusOptionPrefab = assetBundle.LoadAsset<GameObject>(path);
            if (gradiusOptionPrefab)
            {
                gradiusOptionPrefab.AddComponent<NetworkIdentity>();
                gradiusOptionPrefab.AddComponent<OptionBehavior>();
                gradiusOptionPrefab.AddComponent<Flicker>();
                Log.Debug("Successfully initialized Gradius Option prefab.");
            }
            else Log.Error("Failed to create GradiusOption: Resource not found or is null.");
        }

        private void RegisterNetworkMessages()
        {
            Log.Debug("Registering custom network messages needed for GradiusOption...");
            NetworkingAPI.RegisterMessageType<SyncOptionTarget>();
        }

        private bool FilterMinions(CharacterMaster master) => master && MinionsList.Exists((item) => master.name.Contains(item));

        private void AssignAurelioniteOwner(CharacterMaster goldMaster)
        {
            if (!goldMaster.name.Contains("TitanGoldAlly")) return;
            CharacterMaster trueMaster = null;
            foreach (PlayerCharacterMasterController pcmc in PlayerCharacterMasterController.instances)
            {
                if (!trueMaster || pcmc.master.inventory.GetItemCount(RoR2Items.TitanGoldDuringTP) > trueMaster.inventory.GetItemCount(RoR2Items.TitanGoldDuringTP))
                {
                    trueMaster = pcmc.master;
                }
            }
            if (!trueMaster) return;
            goldMaster.AssignOwner(trueMaster);
        }

        private bool DroneNameCheck(CharacterBody body, string name)
        {
            return body && body.master && body.name.Contains(name) && body.master.name.Contains(name);
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
    }
}