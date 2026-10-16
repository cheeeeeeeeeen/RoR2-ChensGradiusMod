using Chen.GradiusMod.Modules;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Chen.GradiusMod.Items
{
    /// <summary>
    /// Allows for making item classes into singleton classes.
    /// </summary>
    /// <typeparam name="T">The item class name</typeparam>
    public abstract class ItemBase<T> : ItemBase where T : ItemBase<T>
    {
        /// <summary>
        /// The instance of the singleton class.
        /// </summary>
        public static T instance { get; private set; }

        /// <summary>
        /// Constructor that creates the instance of the singleton class.
        /// </summary>
        protected ItemBase()
        {
            if (instance != null) throw new InvalidOperationException($"Singleton class \"{typeof(T).Name}\" instantiated twice.");
            instance = this as T;
        }
    }

    /// <summary>
    /// The base class of the items of this mod.
    /// </summary>
    public abstract class ItemBase : ModuleBase
    {
        /// <summary>
        /// The ItemDef of this item. Available after SetupAttributes.
        /// </summary>
        public ItemDef itemDef { get; private set; }

        /// <summary>
        /// The display name of the item.
        /// </summary>
        public abstract string displayName { get; }

        /// <summary>
        /// The tier of the item.
        /// </summary>
        public abstract ItemTier itemTier { get; }

        /// <summary>
        /// The tags of the item.
        /// </summary>
        public virtual ItemTag[] itemTags => new ItemTag[0];

        /// <summary>
        /// If true, AI will not receive this item.
        /// </summary>
        public virtual bool itemIsAIBlacklisted => false;

        /// <summary>
        /// The pickup model of the item.
        /// </summary>
        protected GameObject modelResource { get; set; }

        /// <summary>
        /// The icon of the item.
        /// </summary>
        protected Sprite iconResource { get; set; }

        /// <inheritdoc/>
        protected override string configCategoryPrefix => "Items";

        /// <summary>
        /// The prefix of the language tokens of the item.
        /// </summary>
        protected string tokenPrefix => $"CHENSGRADIUSMOD_{name.ToUpperInvariant()}";

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        protected abstract string GetNameString(string langid = null);

        protected abstract string GetPickupString(string langid = null);

        protected abstract string GetDescString(string langid = null);

        protected abstract string GetLoreString(string langid = null);

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        /// <inheritdoc/>
        public override void SetupAttributes()
        {
            List<ItemTag> tags = new List<ItemTag>(itemTags);
            if (itemIsAIBlacklisted && !tags.Contains(ItemTag.AIBlacklist)) tags.Add(ItemTag.AIBlacklist);

            itemDef = ScriptableObject.CreateInstance<ItemDef>();
            itemDef.name = $"CGM{name}";
            itemDef.nameToken = $"{tokenPrefix}_NAME_RENDERED";
            itemDef.pickupToken = $"{tokenPrefix}_PICKUP_RENDERED";
            itemDef.descriptionToken = $"{tokenPrefix}_DESC_RENDERED";
            itemDef.loreToken = $"{tokenPrefix}_LORE_RENDERED";
            itemDef.pickupModelPrefab = modelResource;
            itemDef.pickupIconSprite = iconResource;
            itemDef.tags = tags.ToArray();
            itemDef.canRemove = true;
            itemDef.hidden = false;
            // The tier setter needs the ItemTierCatalog, which is not available yet at this point. Assign the ItemTierDef directly instead.
            itemDef._itemTierDef = LoadItemTierDef(itemTier);

            LanguageAPI.Add(itemDef.nameToken, GetNameString());
            LanguageAPI.Add(itemDef.pickupToken, GetPickupString());
            LanguageAPI.Add(itemDef.descriptionToken, GetDescString());
            LanguageAPI.Add(itemDef.loreToken, GetLoreString());

            ItemAPI.Add(new CustomItem(itemDef, new ItemDisplayRuleDict()));
        }

        /// <summary>
        /// Gets the number of this item the body has.
        /// </summary>
        /// <param name="body">The body to check</param>
        /// <returns>The item count. 0 if the body or its inventory is missing.</returns>
        public int GetCount(CharacterBody body)
        {
            if (!itemDef || !body || !body.inventory) return 0;
            return body.inventory.GetItemCountEffective(itemDef);
        }

        /// <summary>
        /// Gets the number of this item the master has.
        /// </summary>
        /// <param name="master">The master to check</param>
        /// <returns>The item count. 0 if the master or its inventory is missing.</returns>
        public int GetCount(CharacterMaster master)
        {
            if (!itemDef || !master || !master.inventory) return 0;
            return master.inventory.GetItemCountEffective(itemDef);
        }

        /// <summary>
        /// Gets the number of a specific item the body has.
        /// </summary>
        /// <param name="body">The body to check</param>
        /// <param name="specificItemDef">The item to count</param>
        /// <returns>The item count. 0 if the body or its inventory is missing.</returns>
        public int GetCountSpecific(CharacterBody body, ItemDef specificItemDef)
        {
            if (!specificItemDef || !body || !body.inventory) return 0;
            return body.inventory.GetItemCountEffective(specificItemDef);
        }

        private static ItemTierDef LoadItemTierDef(ItemTier tier)
        {
            string key;
            switch (tier)
            {
                case ItemTier.Tier1: key = "RoR2/Base/Common/Tier1Def.asset"; break;
                case ItemTier.Tier2: key = "RoR2/Base/Common/Tier2Def.asset"; break;
                case ItemTier.Tier3: key = "RoR2/Base/Common/Tier3Def.asset"; break;
                default: return null;
            }
            return Addressables.LoadAssetAsync<ItemTierDef>(key).WaitForCompletion();
        }
    }
}
