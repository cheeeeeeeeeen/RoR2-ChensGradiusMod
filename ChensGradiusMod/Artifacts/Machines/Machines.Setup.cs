using RoR2;
using System.Collections.Generic;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;
using static RoR2.Navigation.MapNodeGroup;

namespace Chen.GradiusMod.Artifacts.Machines
{
    /// <summary>
    /// An artifact class which provides the main API related to controlling which drones the enemy may spawn with.
    /// </summary>
    public partial class Machines : ArtifactBase<Machines>
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public override string displayName => "Artifact of Machines";

        public float hasDroneChance { get; private set; } = 25f;
        public int minimumEnemyDroneCount { get; private set; } = 1;
        public int maximumEnemyDroneCount { get; private set; } = 2;
        public int maxPrototypePlayerCount { get; private set; } = 1;
        public int backupDroneSpawnWeight { get; private set; } = 1;
        public int drone1SpawnWeight { get; private set; } = 8;
        public int drone2SpawnWeight { get; private set; } = 8;
        public int emergencyDroneSpawnWeight { get; private set; } = 4;
        public int flameDroneSpawnWeight { get; private set; } = 1;
        public int missileDroneSpawnWeight { get; private set; } = 4;
        public int gunnerTurretSpawnWeight { get; private set; } = 1;
        public int tc280SpawnWeight { get; private set; } = 0;

        protected override string GetNameString(string langid = null) => displayName;

        protected override string GetDescString(string langid = null)
        {
            string str = "Enemies ";
            if (hasDroneChance >= 100f) str += "will ";
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

            hasDroneChance = Bind(nameof(hasDroneChance), hasDroneChance, "Percentage chance of enemies getting drones. 25 means 25% chance.", 0f, 100f);
            minimumEnemyDroneCount = Bind(nameof(minimumEnemyDroneCount), minimumEnemyDroneCount, "The least number of drones an enemy can spawn with.", 1, int.MaxValue);
            maximumEnemyDroneCount = Bind(nameof(maximumEnemyDroneCount), maximumEnemyDroneCount, "The max number of drones an enemy can spawn with.", 1, int.MaxValue);
            maxPrototypePlayerCount = Bind(nameof(maxPrototypePlayerCount), maxPrototypePlayerCount, "The max number of TC-280 Prototypes each player can own.", 0, int.MaxValue);
            backupDroneSpawnWeight = Bind(nameof(backupDroneSpawnWeight), backupDroneSpawnWeight, "Spawn Weight of the Strike Drone.", 0, 20);
            drone1SpawnWeight = Bind(nameof(drone1SpawnWeight), drone1SpawnWeight, "Spawn Weight of the Gunner Drone.", 0, 20);
            drone2SpawnWeight = Bind(nameof(drone2SpawnWeight), drone2SpawnWeight, "Spawn Weight of the Healer Drone.", 0, 20);
            emergencyDroneSpawnWeight = Bind(nameof(emergencyDroneSpawnWeight), emergencyDroneSpawnWeight, "Spawn Weight of the Emergency Drone.", 0, 20);
            flameDroneSpawnWeight = Bind(nameof(flameDroneSpawnWeight), flameDroneSpawnWeight, "Spawn Weight of the Flame Drone.", 0, 20);
            missileDroneSpawnWeight = Bind(nameof(missileDroneSpawnWeight), missileDroneSpawnWeight, "Spawn Weight of the Missile Drone.", 0, 20);
            gunnerTurretSpawnWeight = Bind(nameof(gunnerTurretSpawnWeight), gunnerTurretSpawnWeight, "Spawn Weight of the Gunner Turret.", 0, 20);
            tc280SpawnWeight = Bind(nameof(tc280SpawnWeight), tc280SpawnWeight, "Spawn Weight of the TC-280 Prototype for enemies.", 0, 20);

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

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private bool IsPrototypeCountUncapped(CharacterMaster master)
        {
            int prototypeNumber = 0;
            foreach (CharacterMaster minion in CharacterMaster.readOnlyInstancesList)
            {
                if (minion && minion.minionOwnership && minion.minionOwnership.ownerMaster == master && minion.name.Contains("MegaDroneMaster"))
                {
                    prototypeNumber++;
                }
            }
            return prototypeNumber < maxPrototypePlayerCount;
        }

        private GameObject RandomlySelectDrone()
        {
            if (EnemyDrones.Count <= 0) return null;
            return EnemyDrones[Random.Range(0, EnemyDrones.Count)];
        }

        private void SummonDrone(CharacterBody ownerBody, GameObject droneMasterPrefab, bool cleanupWithOwner, int counter = 1)
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
                useAmbientLevel = true,
                preSpawnSetupCallback = (master) =>
                {
                    MachinesDrone marker = master.gameObject.AddComponent<MachinesDrone>();
                    marker.cleanupWithOwner = cleanupWithOwner;
                }
            }.Perform();
            if (!characterMaster)
            {
                Log.Warning($"Machines.SummonDrone: Failed to summon {droneMasterPrefab.name} for {ownerBody.name}.");
                return;
            }
            CharacterBody droneBody = characterMaster.GetBody();
            if (!droneBody) return;
            ModelLocator modelLocator = droneBody.modelLocator;
            if (modelLocator && modelLocator.modelTransform)
            {
                CharacterModel characterModel = modelLocator.modelTransform.GetComponent<CharacterModel>();
                if (characterModel)
                {
                    TemporaryOverlayInstance overlay = TemporaryOverlayManager.AddOverlay(modelLocator.modelTransform.gameObject);
                    overlay.duration = 0.5f;
                    overlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    overlay.animateShaderAlpha = true;
                    overlay.destroyComponentOnEnd = true;
                    overlay.originalMaterial = summonDroneMaterial;
                    overlay.AddToCharacterModel(characterModel);
                }
            }
            RepositionGroundedDrones(droneBody, ownerBody.transform.position);
        }

        private void RepositionGroundedDrones(CharacterBody droneBody, Vector3 desiredPosition)
        {
            if (!GroundedDrones.Exists(subname => droneBody.name.Contains(subname))) return;
            SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
            spawnCard.hullSize = droneBody.hullClassification;
            spawnCard.nodeGraphType = GraphType.Ground;
            spawnCard.prefab = helperPrefab;
            GameObject gameObject = DirectorCore.instance ? DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                position = desiredPosition,
                minDistance = 0,
                maxDistance = 50
            }, Run.instance.runRNG)) : null;
            if (gameObject)
            {
                TeleportHelper.TeleportBody(droneBody, gameObject.transform.position, false);
                Object.Destroy(gameObject);
            }
            else
            {
                Log.Warning($"Machines.RepositionGroundedDrones: Failed to reposition {droneBody.name}. It might be floating now.");
            }
            Object.Destroy(spawnCard);
        }
    }
}
