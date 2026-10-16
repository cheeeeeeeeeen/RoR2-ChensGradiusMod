using Chen.GradiusMod.Items.OptionSeed.Components;
using Chen.GradiusMod.Mirroring;
using RoR2;
using System.Collections.Generic;

namespace Chen.GradiusMod.Items.OptionSeed
{
    public partial class OptionSeed : IMirrorProvider
    {
        // Survivors whose skills have dedicated support in OptionSeed.Hooks. Everything else is copied by the AttackMirror.
        private static readonly HashSet<string> DedicatedBodies = new HashSet<string>
        {
            "CommandoBody", "HuntressBody", "Bandit2Body", "ToolbotBody", "EngiBody", "EngiTurretBody", "EngiWalkerTurretBody",
            "MageBody", "MercBody", "TreebotBody", "LoaderBody", "CrocoBody", "CaptainBody", "RailgunnerBody",
            "VoidSurvivorBody", "SeekerBody", "ChefBody", "FalseSonBody"
        };

        // Survivors that are always copied by the AttackMirror, regardless of the config for modded survivors.
        private static readonly HashSet<string> BuiltInAutoBodies = new HashSet<string>
        {
            "AcolyteBody", "DrifterBody", "DroneTechBody", "Enforcer2Body"
        };

        private readonly Dictionary<string, bool> autoMirroringOverrides = new Dictionary<string, bool>();

        /// <summary>
        /// Sets if the Option Seeds of the survivor copy its attacks automatically, by copying the bullets, projectiles, melee hits and explosions it creates.
        /// Use this for modded survivors. Set to false if the survivor's skills are supported manually with FireForSeeds instead.
        /// </summary>
        /// <param name="bodyName">The CharacterBody name of the survivor, e.g. "CommandoBody".</param>
        /// <param name="enabled">True to copy attacks automatically, false to only use manual support.</param>
        public void SetAutoMirroring(string bodyName, bool enabled) => autoMirroringOverrides[bodyName] = enabled;

        /// <summary>
        /// Checks if the Option Seeds of the character copy its attacks automatically.
        /// </summary>
        /// <param name="body">The character to check.</param>
        /// <returns>True if the attacks are copied automatically. False if the character relies on manual support through FireForSeeds.</returns>
        public bool UsesAutoMirroring(CharacterBody body)
        {
            if (!body) return false;
            string bodyName = BodyCatalog.GetBodyName(body.bodyIndex);
            if (bodyName == null) return false;
            if (autoMirroringOverrides.TryGetValue(bodyName, out bool overridden)) return overridden;
            if (BuiltInAutoBodies.Contains(bodyName)) return true;
            return autoSupportModdedSurvivors && !DedicatedBodies.Contains(bodyName);
        }

        void IMirrorProvider.CollectMirrorPoints(CharacterBody attacker, List<MirrorPoint> points)
        {
            if (!UsesAutoMirroring(attacker)) return;
            SeedTracker seedTracker = attacker.GetComponent<SeedTracker>();
            if (!seedTracker) return;
            float multiplier = ComputeMultiplier(attacker);
            points.Add(new MirrorPoint { origin = seedTracker.leftSeed, multiplier = multiplier });
            points.Add(new MirrorPoint { origin = seedTracker.rightSeed, multiplier = multiplier });
        }

        private void SetupAttackMirroring()
        {
            AttackMirror.RegisterProvider(this);
            // These throw something unique, like the creature in Drifter's bag or one of Operator's drones, so they are not copied.
            AttackMirror.ExcludeEntityState(typeof(EntityStates.Drifter.EmptyBag));
            AttackMirror.ExcludeEntityState(typeof(EntityStates.Drifter.Repossess));
            AttackMirror.ExcludeEntityState(typeof(EntityStates.DroneTech.Weapon.ThrowDroneBall));
        }
    }
}
