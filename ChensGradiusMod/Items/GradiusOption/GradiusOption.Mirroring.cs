using Chen.GradiusMod.Items.GradiusOption.Components;
using Chen.GradiusMod.Mirroring;
using RoR2;
using RoR2.CharacterAI;
using System.Collections.Generic;
using UnityEngine;

namespace Chen.GradiusMod.Items.GradiusOption
{
    public partial class GradiusOption : IMirrorProvider
    {
        // Minions that are always copied by the AttackMirror, regardless of the config for modded drones.
        private static readonly List<string> BuiltInAutoMinions = new List<string>
        {
            "FriendUnit"
        };

        private readonly Dictionary<string, bool> autoMirroringOverrides = new Dictionary<string, bool>();

        /// <summary>
        /// Sets if a minion type gains Options that copy its attacks automatically, by copying the bullets, projectiles, melee hits and explosions it creates.
        /// Use this for modded drones and minions. Minions supported with SupportMinionType rely on manual support with FireForAllOptions instead.
        /// </summary>
        /// <param name="masterName">The CharacterMaster name of the minion. Partial names are matched, the same way as SupportMinionType.</param>
        /// <param name="enabled">True to give the minion Options that copy attacks automatically, false to disable it.</param>
        public void SetAutoMirroring(string masterName, bool enabled) => autoMirroringOverrides[masterName] = enabled;

        /// <summary>
        /// Checks if the minion's Options copy its attacks automatically.
        /// </summary>
        /// <param name="master">The minion to check.</param>
        /// <returns>True if the attacks are copied automatically. False if the minion relies on manual support or is not supported.</returns>
        public bool UsesAutoMirroring(CharacterMaster master)
        {
            if (!master || !master.minionOwnership || !master.minionOwnership.ownerMaster) return false;
            string masterName = master.name;
            foreach (KeyValuePair<string, bool> pair in autoMirroringOverrides)
            {
                if (masterName.Contains(pair.Key)) return pair.Value;
            }
            if (MinionsList.Exists(item => masterName.Contains(item))) return false;
            if (BuiltInAutoMinions.Exists(item => masterName.Contains(item))) return true;
            return autoSupportModdedDrones && IsDrone(master);
        }

        void IMirrorProvider.CollectMirrorPoints(CharacterBody attacker, List<MirrorPoint> points)
        {
            if (!UsesAutoMirroring(attacker.master)) return;
            OptionTracker optionTracker = attacker.GetComponent<OptionTracker>();
            if (!optionTracker) return;
            GameObject target = null;
            BaseAI ai = attacker.master.GetComponent<BaseAI>();
            if (ai && ai.currentEnemy != null && ai.currentEnemy.gameObject) target = ai.currentEnemy.gameObject;
            foreach (GameObject option in optionTracker.existingOptions)
            {
                if (!option) continue;
                Vector3? direction = null;
                if (target) direction = (target.transform.position - option.transform.position).normalized;
                points.Add(new MirrorPoint { origin = option, multiplier = damageMultiplier, direction = direction });
            }
        }

        private static bool IsDrone(CharacterMaster master)
        {
            CharacterBody bodyPrefab = master.bodyPrefab ? master.bodyPrefab.GetComponent<CharacterBody>() : null;
            return bodyPrefab && (bodyPrefab.bodyFlags & CharacterBody.BodyFlags.Drone) == CharacterBody.BodyFlags.Drone;
        }

        private void SetupAttackMirroring()
        {
            AttackMirror.RegisterProvider(this);
        }
    }
}
