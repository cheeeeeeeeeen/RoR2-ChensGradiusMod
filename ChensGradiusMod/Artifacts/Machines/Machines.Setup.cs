using Chen.Helpers.GeneralHelpers;
using RoR2;
using System.Collections.Generic;
using TILER2;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;
using static RoR2.Navigation.MapNodeGroup;
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
        [AutoConfig("Percentage chance of enemies getting drones. 25 means 25% chance.", AutoConfigFlags.PreventNetMismatch, 0f, 100f)]
        public float hasDroneChance { get; private set; } = 25f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("The least number of drones an enemy can spawn with.", AutoConfigFlags.PreventNetMismatch, 1, int.MaxValue)]
        public int minimumEnemyDroneCount { get; private set; } = 1;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("The max number of drones an enemy can spawn with.", AutoConfigFlags.PreventNetMismatch, 1, int.MaxValue)]
        public int maximumEnemyDroneCount { get; private set; } = 2;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("The max number of TC-280 Prototypes each player can own.", AutoConfigFlags.PreventNetMismatch, 0, int.MaxValue)]
        public int maxPrototypePlayerCount { get; private set; } = 1;

        [AutoConfig("Spawn Weight of the Strike Drone.", AutoConfigFlags.None, 0, 20)]
        public int backupDroneSpawnWeight { get; private set; } = 1;

        [AutoConfig("Spawn Weight of the Gunner Drone.", AutoConfigFlags.None, 0, 20)]
        public int drone1SpawnWeight { get; private set; } = 8;

        [AutoConfig("Spawn Weight of the Healer Drone.", AutoConfigFlags.None, 0, 20)]
        public int drone2SpawnWeight { get; private set; } = 8;

        [AutoConfig("Spawn Weight of the Emergency Drone.", AutoConfigFlags.None, 0, 20)]
        public int emergencyDroneSpawnWeight { get; private set; } = 4;

        [AutoConfig("Spawn Weight of the Flame Drone.", AutoConfigFlags.None, 0, 20)]
        public int flameDroneSpawnWeight { get; private set; } = 1;

        [AutoConfig("Spawn Weight of the Missile Drone.", AutoConfigFlags.None, 0, 20)]
        public int missileDroneSpawnWeight { get; private set; } = 4;

        [AutoConfig("Spawn Weight of the Gunner Turret.", AutoConfigFlags.None, 0, 20)]
        public int gunnerTurretSpawnWeight { get; private set; } = 1;

        [AutoConfig("Spawn Weight of the TC-280 Prototype for enemies.", AutoConfigFlags.None, 0, 20)]
        public int tc280SpawnWeight { get; private set; } = 0;

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

        private readonly List<GameObject> EnemyDrones = new List<GameObject>();

        private readonly List<string> GroundedDrones = new List<string>()
        {
            "Turret1"
        };

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

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            AddEnemyDroneType(backupDroneMaster, backupDroneSpawnWeight);
            AddEnemyDroneType(drone1Master, drone1SpawnWeight);
            AddEnemyDroneType(drone2Master, drone2SpawnWeight);
            AddEnemyDroneType(emergencyDroneMaster, emergencyDroneSpawnWeight);
            AddEnemyDroneType(flameDroneMaster, flameDroneSpawnWeight);
            AddEnemyDroneType(missileDroneMaster, missileDroneSpawnWeight);
            AddEnemyDroneType(turret1Master, gunnerTurretSpawnWeight);
            AddEnemyDroneType(tc280DroneMaster, tc280SpawnWeight);
        }

        private bool IsPrototypeCountUncapped(CharacterMaster master)
        {
            int prototypeNumber = 0;
            master.LoopMinions(minion =>
            {
                if (minion.name.Contains("MegaDroneMaster")) prototypeNumber++;
            });
            return prototypeNumber < maxPrototypePlayerCount;
        }

        private GameObject RandomlySelectDrone()
        {
            int randomIndex = Random.Range(0, EnemyDrones.Count);
            return EnemyDrones[randomIndex];
        }

        private void SummonDrone(CharacterBody ownerBody, GameObject droneMasterPrefab, int counter = 1)
        {
            if (!ownerBody || !droneMasterPrefab)
            {
                Log.Warning("Machines.SummonDrone: ownerBody or droneMasterPrefab is null. Cancel drone summon.");
                return;
            }
            CharacterMaster characterMaster = new MasterSummon
            {
                masterPrefab = droneMasterPrefab,
                position = ownerBody.transform.position - (ownerBody.transform.forward.normalized * counter) + (.25f * counter * Vector3.up),
                rotation = ownerBody.transform.rotation,
                summonerBodyObject = ownerBody.gameObject,
                ignoreTeamMemberLimit = true,
                useAmbientLevel = true
            }.Perform();
            if (characterMaster)
            {
                GameObject bodyObject = characterMaster.GetBodyObject();
                if (bodyObject)
                {
                    ModelLocator component = bodyObject.GetComponent<ModelLocator>();
                    if (component && component.modelTransform)
                    {
                        TemporaryOverlay temporaryOverlay = component.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                        temporaryOverlay.duration = 0.5f;
                        temporaryOverlay.animateShaderAlpha = true;
                        temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                        temporaryOverlay.destroyComponentOnEnd = true;
                        temporaryOverlay.originalMaterial = summonDroneMaterial;
                        temporaryOverlay.AddToCharacerModel(component.modelTransform.GetComponent<CharacterModel>());
                    }
                }
                RepositionGroundedDrones(characterMaster.GetBody(), ownerBody.transform.position);
            }
        }

        private void RepositionGroundedDrones(CharacterBody droneBody, Vector3 desiredPosition)
        {
            bool isGrounded = false;
            foreach (string subname in GroundedDrones)
            {
                if (droneBody.name.Contains(subname))
                {
                    isGrounded = true;
                    break;
                }
            }
            if (!isGrounded) return;
            SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
            spawnCard.hullSize = droneBody.hullClassification;
            spawnCard.nodeGraphType = GraphType.Ground;
            spawnCard.prefab = helperPrefab;
            GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                position = desiredPosition,
                minDistance = 0,
                maxDistance = 50
            }, Run.instance.runRNG));
            if (gameObject)
            {
                TeleportHelper.TeleportBody(droneBody, gameObject.transform.position);
                Object.Destroy(gameObject);
            }
            else
            {
                Log.Warning($"Machines.RepositionGroundedDrones: Failed to reposition {droneBody.name}. It might be floating now.");
            }
            Object.Destroy(spawnCard);
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}