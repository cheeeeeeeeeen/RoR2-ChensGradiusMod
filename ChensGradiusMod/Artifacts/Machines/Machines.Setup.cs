using System.Collections.Generic;
using TILER2;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;
using static TILER2.MiscUtil;

namespace Chen.GradiusMod.Artifacts.Machines
{
    /// <summary>
    /// As artifact class which provides the main API related to the controlling which drones the enemy may spawn with. It is powered by TILER2.
    /// </summary>
    public partial class Machines : Artifact<Machines>
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string displayName => "Artifact of Machines";

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Percentage chance of enemies getting drones. 25 means 25% chance.", AutoConfigFlags.None, 0f, 100f)]
        public float hasDroneChance { get; private set; } = 100f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("The least number of drones an enemy can spawn with.", AutoConfigFlags.None, 1, int.MaxValue)]
        public int minimumEnemyDroneCount { get; private set; } = 1;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("The max number of drones an enemy can spawn with.", AutoConfigFlags.None, 1, int.MaxValue)]
        public int maximumEnemyDroneCount { get; private set; } = 2;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("The max number of TC-280 Prototypes each player can own.", AutoConfigFlags.None, 0, int.MaxValue)]
        public int maxPrototypePlayerCount { get; private set; } = 1;

        protected override string GetNameString(string langid = null) => displayName;

        protected override string GetDescString(string langid = null)
        {
            string str = "Enemies ";
            if (hasDroneChance == 100f) str += "will ";
            else str += "have a chance to ";
            str += $"spawn with ";
            if (minimumEnemyDroneCount == maximumEnemyDroneCount) str += $"{minimumEnemyDroneCount} ";
            else str += $"{minimumEnemyDroneCount} - {maximumEnemyDroneCount} ";
            str += $"drone{NPlur(maximumEnemyDroneCount)}.";
            if (maxPrototypePlayerCount > 0) str = $"Survivors will spawn with a TC-280 Prototype. " + str;
            return str;
        }

        private readonly List<GameObject> EnemyDrones = new List<GameObject>()
        {
            Resources.Load<GameObject>("prefabs/charactermasters/DroneBackupMaster"),
            Resources.Load<GameObject>("prefabs/charactermasters/Drone1Master"),
            Resources.Load<GameObject>("prefabs/charactermasters/Drone2Master"),
            Resources.Load<GameObject>("prefabs/charactermasters/EmergencyDroneMaster"),
            Resources.Load<GameObject>("prefabs/charactermasters/FlameDroneMaster"),
            Resources.Load<GameObject>("prefabs/charactermasters/DroneMissileMaster")
        };

        private readonly GameObject tc280DroneMaster = Resources.Load<GameObject>("prefabs/charactermasters/MegaDroneMaster");

        public Machines()
        {
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Artifacts/machines_artifact_on_icon.png");
            iconResourceDisabled = assetBundle.LoadAsset<Sprite>("Assets/Artifacts/machines_artifact_off_icon.png");
        }

        public override void SetupConfig()
        {
            base.SetupConfig();
            if (minimumEnemyDroneCount > maximumEnemyDroneCount)
            {
                int origMinCount = minimumEnemyDroneCount;
                minimumEnemyDroneCount = maximumEnemyDroneCount;
                maximumEnemyDroneCount = origMinCount;
            }
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}