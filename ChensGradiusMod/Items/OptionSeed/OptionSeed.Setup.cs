using Chen.GradiusMod.Items.GradiusOption.Components;
using Chen.GradiusMod.Items.OptionSeed.Components;
using RoR2;
using System.Collections.Generic;
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
        [AutoConfig("Damage multiplier of Option Seeds. This also affects Proc Coefficients and the knockback force of the attack. " +
                    "1 = 100%. Server only.", AutoConfigFlags.None, 0f, float.MaxValue)]
        public float damageMultiplier { get; private set; } = .12f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Additive increase to the damage multiplier when the item is stacked.", AutoConfigFlags.None, 0f, float.MaxValue)]
        public float stackDamageMultiplier { get; private set; } = .03f;

        [AutoConfig("Allows displaying and syncing the laser effect of Option Seeds. Disabling this will replace the effect with bullets. " +
                    "Damage will stay the same. Server and Client. The server and client must have the same settings for an optimized experience. " +
                    "Disable this if you are experiencing FPS drops or network lag.", AutoConfigFlags.PreventNetMismatch)]
        public bool mobileTurretsSeedSyncEffect { get; private set; } = true;

        [AutoConfig("Allows displaying and syncing the flamethrower effect of Option Seeds. Disabling this will replace the effect with bullets. " +
                    "Damage will stay the same. Server and Client. The server and client must have the same settings for an optimized experience. " +
                    "Disable this if you are experiencing FPS drops or network lag.", AutoConfigFlags.PreventNetMismatch)]
        public bool flamethrowerSeedSyncEffect { get; private set; } = true;

        [AutoConfig("Set to true for the Orbs to have the Option Seed Pickup model in the center. Server and Client. Cosmetic only. " +
                    "Turning this off could lessen resource usage.", AutoConfigFlags.PreventNetMismatch)]
        public bool includeModelInsideOrb { get; private set; } = true;

        [AutoConfig("Sets the position of the Option Seeds to be static instead of them rotating.", AutoConfigFlags.PreventNetMismatch)]
        public bool staticPositions { get; private set; } = false;

        [AutoConfig("The rotational speed of the seeds in angle difference. A value of 10 means that the angular computation is increased by 10 every frame.",
                    AutoConfigFlags.PreventNetMismatch)]
        public float rotationSpeed { get; private set; } = 10f;

        public override bool itemIsAIBlacklisted { get; protected set; } = true;

        protected override string GetNameString(string langid = null) => displayName;

        protected override string GetPickupString(string langid = null) => $"Deploy the Option Seeds, fragments of the ultimate weapon from the Gradius Federation.";

        protected override string GetDescString(string langid = null)
        {
            string str = $"Deploy <style=cIsDamage>2</style> Option Seeds. Option Seeds copy the wielder's attacks";
            str += $" for <style=cIsDamage>{Pct(damageMultiplier, 0)}</style> <style=cStack>(+{Pct(stackDamageMultiplier)} per stack)</style> of the damage dealt.";
            return str;
        }

        protected override string GetLoreString(string langid = null) =>
            "\"CASE, was it? I assume this another odd-looking lollipop is from your place?\" I asked the powerful, sentient A.I.\n\n" +
            "We found another one of those things that look like the Gradius' Options. It is a really powerful weapon, and thus I can only imagine the power of this item.\n\n" +
            "\"Analyzing,\" says CASE through the built speakers we made her, her female voice seemingly like a person more than a robot, \"Ah, it's the Seed.\"\n\n" +
            "\"The Seed?\"\n\n\"Yes, the Option Seed. It is a fragment of the Options you now know. While it is a fragment, it is more organic, thus, more alive.\"\n\n" +
            "I paused. I myself am not sure how to comprehend what she just said. Organic? Alive? Is she saying that it is some kind of alien creature instead?\n\n" +
            "\"Think of it as a child who can become attached to the wielder,\" she continues to explain, \"It is somewhat complicated. When it reaches its " +
            "full potential, it becomes fully mechanical.\"\n\nThere was silence in between us. As I was just going to set the odd lollipop aside, it brought itself to life " +
            "and started circling around me intelligently without trying to collide with anything in its path.\n\n" +
            "\"It has initiated and registered you as its host.\"\n\nI looked around while it circles around me, trying to find a way to stop it without a word. " +
            "This is all too astonishing for me to say a word.\n\n\"This was first seen a long time ago in our planetary system. Victory Viper and Lord British set out together to " +
            "save Planet Latis from the great evil called Salamander,\" she says.\n\nShe fell silent after, seemingly like reminiscing.\n\n" +
            "The great evil? She sounds like she went through a lot. Now, she faces yet another chaotic scene of survival. She must have had it rougher than I expected.\n\n" +
            "Can she feel pain? She had old allies and friends for sure. She sounds more human than me, now that I think about it!\n\n" +
            "\"You mentioned about Katswell in your audio log when you were lost in space. Who is she?\" I boldly asked her.\n\n" +
            "She made a soft, short giggle, then said, \"She is one of the legends who played a big role in saving our race.\"\n\n" +
            "I nodded, and it strangely felt satisfying to hear that.";

        internal const uint getOptionEventId = GradiusOptionItem.getOptionEventId;
        internal const uint loseOptionEventId = GradiusOptionItem.loseOptionEventId;
        internal const float defaultVerticalOffsetMultiplier = .4f;
        internal const float defaultHorizontalOffsetMultiplier = 1f;

        internal static GameObject optionSeedPrefab { get; private set; }

        private static readonly Dictionary<string, float> VerticalOffsetMultipliers = new Dictionary<string, float>()
        {
            { "EngiTurretBody", .5f },
            { "EngiWalkerTurretBody", 1.3f },
            { "ToolbotBody", .5f },
            { "MageBody", .8f },
            { "LoaderBody", .7f }
        };

        private static readonly Dictionary<string, float> HorizontalOffsetMultipliers = new Dictionary<string, float>();

        public OptionSeed()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("assets/option/model/seedmodel.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("assets/option/icon/optionseed_icon.png");
        }

        public override void SetupConfig()
        {
            base.SetupConfig();
        }

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            InitializeAssets();
            if (Compat_ItemStats.enabled)
            {
                Compat_ItemStats.CreateItemStatDef(itemDef,
                (
                    (count, inv, master) => ComputeMultiplier((int)count),
                    (value, inv, master) => $"Damage: {Pct(value)}"
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
                optionSeedPrefab.AddComponent<SeedBehavior>();
                Log.Debug("Successfully initialized Option Seed prefab.");
            }
            else Log.Error("Failed to create OptionSeed: Resource not found or is null.");
        }

        private float ComputeMultiplier(int itemCount) => damageMultiplier + (stackDamageMultiplier * (itemCount - 1));

        private float ComputeMultiplier(CharacterBody ownerBody) => ComputeMultiplier(GetCount(ownerBody));

        internal float GetVerticalOffsetMultiplier(string bodyName)
        {
            foreach (var pair in VerticalOffsetMultipliers)
            {
                if (bodyName.Contains(pair.Key)) return pair.Value;
            }
            return defaultVerticalOffsetMultiplier;
        }

        internal float GetHorizontalOffsetMultiplier(string bodyName)
        {
            foreach (var pair in HorizontalOffsetMultipliers)
            {
                if (bodyName.Contains(pair.Key)) return pair.Value;
            }
            return defaultVerticalOffsetMultiplier;
        }
    }
}