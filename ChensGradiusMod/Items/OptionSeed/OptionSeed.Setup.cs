using Chen.GradiusMod.Items.GradiusOption.Components;
using RoR2;
using System.Collections.ObjectModel;
using TILER2;
using UnityEngine;
using UnityEngine.Networking;
using static Chen.GradiusMod.GradiusModPlugin;
using static TILER2.MiscUtil;
using GradiusOptionItem = Chen.GradiusMod.Items.GradiusOption.GradiusOption;

namespace Chen.GradiusMod.Items.OptionSeed
{
    /// <summary>
    /// An item class powered by TILER2 which provides the main API related to the Option Seeds.
    /// </summary>
    public partial class OptionSeed : Item<OptionSeed>
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string displayName => "Gradius' Option Seed";

        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility });

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage multiplier of Option Seeds. 1 = 100%. Server only.", AutoConfigFlags.None, 0f, float.MaxValue)]
        public float damageMultiplier { get; private set; } = .5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Proc value for Option Seeds. A value of 0.25 means 25%. Multiplicative stacking.", AutoConfigFlags.None, 0f, float.MaxValue)]
        public float procValue { get; private set; } = .25f;

        [AutoConfig("Set to true for the Orbs to have the Option Seed Pickup model in the center. Server and Client. Cosmetic only. " +
                    "Turning this off could lessen resource usage.", AutoConfigFlags.PreventNetMismatch)]
        public bool includeModelInsideOrb { get; private set; } = true;

        public override bool itemIsAIBlacklisted { get; protected set; } = true;

        protected override string GetNameString(string langid = null) => displayName;

        protected override string GetPickupString(string langid = null) => $"Deploy the Option Seeds, fragments of the ultimate weapon from the Gradius Federation.";

        protected override string GetDescString(string langid = null)
        {
            string str = $"Deploy <style=cIsDamage>2</style> Option Seeds. Option Seeds have a <style=cIsDamage>{Pct(procValue)}</style>";
            str += $" <style=cStack>(+{Pct(procValue)} per stack, multiplicative)</style> chance to copy the wielder's attacks";
            str += $" for <style=cIsDamage>{Pct(damageMultiplier, 0)}</style> of the damage dealt.";
            return str;
        }

        protected override string GetLoreString(string langid = null) =>
            "";

        internal const uint getOptionEventId = GradiusOptionItem.getOptionEventId;
        internal const uint loseOptionEventId = GradiusOptionItem.loseOptionEventId;

        internal static GameObject optionSeedPrefab { get; private set; }

        public OptionSeed()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("assets/option/model/optionmodel.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("assets/option/icon/gradiusoption_icon.png");
        }

        public override void SetupConfig()
        {
            base.SetupConfig();
        }

        public override void SetupBehavior()
        {
            InitializeAssets();
            if (Compat_ItemStats.enabled)
            {
                Compat_ItemStats.CreateItemStatDef(itemDef,
                (
                    (count, inv, master) => { return ProcComputation(procValue, (int)count); },
                    (value, inv, master) => { return $"Copy Chance: {Pct(value)}"; }
                ),
                (
                    (count, inv, master) => { return damageMultiplier; },
                    (value, inv, master) => { return $"Damage: {Pct(value)}"; }
                ));
            }
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private void InitializeAssets()
        {
            itemDef.pickupModelPrefab.transform.localScale *= 2f;

            string path;
            if (includeModelInsideOrb) path = "assets/option/orb/seedorbwithmodel.prefab";
            else path = "assets/option/orb/optionorb.prefab";
            optionSeedPrefab = assetBundle.LoadAsset<GameObject>(path);
            if (optionSeedPrefab)
            {
                optionSeedPrefab.AddComponent<NetworkIdentity>();
                optionSeedPrefab.AddComponent<Flicker>();
                Log.Debug("Successfully initialized Option Seed prefab.");
            }
            else Log.Error("Failed to create OptionSeed: Resource not found or is null.");
        }

        private float ProcComputation(float procChance, int stack) => (1f - Mathf.Pow(1f - procChance / 100f, stack)) * 100f;
    }
}