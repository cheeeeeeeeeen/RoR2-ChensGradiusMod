#undef DEBUG

using Chen.Helpers.GeneralHelpers;
using Chen.Helpers.UnityHelpers;
using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Skills;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;
using static R2API.DirectorAPI;
using static R2API.DirectorAPI.Helpers;
using Stage = R2API.DirectorAPI.Stage;

namespace Chen.GradiusMod.Drones.BeamDrone
{
    internal class LaserDrone1 : Drone<LaserDrone1>
    {
        public float laserDuration { get; private set; } = 4f;
        public float laserCooldown { get; private set; } = 4f;
        public float damageCoefficient { get; private set; } = 1f;
        public int minimumStageSpawn { get; private set; } = 3;
        public int skyMeadowMinimumStageSpawn { get; private set; } = 5;
        public int spawnWeight { get; private set; } = 1;
        public int skyMeadowSpawnWeight { get; private set; } = 4;

        public override bool canHaveOptions => true;

        public static InteractableSpawnCard iSpawnCard { get; private set; }
        public static GameObject brokenObject { get; private set; }
        public static DirectorCardHolder iDirectorCardHolder { get; private set; }
        public static DirectorCardHolder iHeavyDirectorCardHolder { get; private set; }
        public static GameObject droneBody { get; private set; }
        public static GameObject droneMaster { get; private set; }

        private int minimumStageCompletions { get => minimumStageSpawn - 1; }

        private int skyMeadowMinimumStageCompletions { get => skyMeadowMinimumStageSpawn - 1; }

        protected override GameObject DroneCharacterMasterObject => droneMaster;

        protected override void SetupConfig()
        {
            spawnWeightWithMachinesArtifact = 0;
            base.SetupConfig();

            laserDuration = config.Bind(configCategory,
                "BeamDuration", laserDuration,
                "The duration of the drone's beam in an attack."
            ).Value;

            laserCooldown = config.Bind(configCategory,
                "BeamCooldown", laserCooldown,
                "The cooldown of the beam attack."
            ).Value;

            damageCoefficient = config.Bind(configCategory,
                "DamageCoefficient", damageCoefficient,
                "Damage Coefficient of the beam attack per tick."
            ).Value;

            minimumStageSpawn = config.Bind(configCategory,
                "MinimumStageSpawn", minimumStageSpawn,
                "Minimum stage number for the drone to start spawning."
            ).Value;

            spawnWeight = config.Bind(configCategory,
                "SpawnWeight", spawnWeight,
                "The weight for which the Director is biased towards spawning this drone."
            ).Value;

            skyMeadowMinimumStageSpawn = config.Bind(configCategory,
                "SkyMeadowMinimumStageSpawn", skyMeadowMinimumStageSpawn,
                "Minimum stage number for the drone to start spawning in Sky Meadow."
            ).Value;

            skyMeadowSpawnWeight = config.Bind(configCategory,
                "SkyMeadowSpawnWeight", skyMeadowSpawnWeight,
                "The weight for which the Director is biased towards spawning this drone in Sky Meadow."
            ).Value;
        }

        protected override void SetupComponents()
        {
            base.SetupComponents();
            AddLanguageTokens();
            GameObject pickupDisplay = CreatePickupDisplay(assetBundle.LoadAsset<GameObject>("Assets/Drones/LaserDrone1/Model/mdlBeamDrone.prefab"));
            InteractableSpawnCard origIsc = drone1SpawnCard;
            brokenObject = origIsc.prefab.InstantiateClone($"{name}Broken", true);
            ModifyBrokenObject();
            iSpawnCard = Object.Instantiate(origIsc);
            ModifyInteractableSpawnCard();
            InitializeDirectorCards();
            droneDef = CreateDroneDef(LoadVanillaDroneDef("RoR2/Base/Drones/MissileDrone.asset"), droneBody, brokenObject, iSpawnCard, "LASER_DRONE1",
                                      droneBody.GetComponent<CharacterBody>().portraitIcon, pickupDisplay, LoadVanillaDroneDef("RoR2/Base/Drones/Drone1.asset"));
        }

        protected override void SetupBehavior()
        {
            base.SetupBehavior();
            InteractableActions += DirectorAPI_InteractableActions;
        }

        private void AddLanguageTokens()
        {
            LanguageAPI.Add("LASER_DRONE1_NAME", "Beam Drone");
            LanguageAPI.Add("LASER_DRONE1_CONTEXT", "Repair Beam Drone");
            LanguageAPI.Add("LASER_DRONE1_INTERACTABLE_NAME", "Broken Beam Drone");
            LanguageAPI.Add("LASER_DRONE1_PICKUP", "Sweeps enemies with a sustained beam.");
            LanguageAPI.Add("LASER_DRONE1_DESCRIPTION",
                $"Fires a sustained beam for <style=cIsUtility>{laserDuration} seconds</style> that deals " +
                $"<style=cIsDamage>{damageCoefficient * 800f:0.##}% damage per second</style>. Recharges in <style=cIsUtility>{laserCooldown} seconds</style>.");
            LanguageAPI.Add("LASER_DRONE1_SKILL_DESCRIPTION",
                $"Sustained Beam: Fire a beam for <style=cIsDamage>{damageCoefficient * 800f:0.##}% damage per second</style> over {laserDuration} seconds.");
        }

        private void ModifyBrokenObject()
        {
            SummonMasterBehavior summonMasterBehavior = brokenObject.GetComponent<SummonMasterBehavior>();
            droneMaster = summonMasterBehavior.masterPrefab.InstantiateClone($"{name}Master", true);
            ContentAddition.AddMaster(droneMaster);
            ModifyDroneMaster();
            CharacterMaster master = droneMaster.GetComponent<CharacterMaster>();
            droneBody = master.bodyPrefab.InstantiateClone($"{name}Body", true);
            ContentAddition.AddBody(droneBody);
            ModifyDroneBody();
            master.bodyPrefab = droneBody;
            summonMasterBehavior.masterPrefab = droneMaster;
            PurchaseInteraction purchaseInteraction = brokenObject.GetComponent<PurchaseInteraction>();
            purchaseInteraction.cost *= 3;
            purchaseInteraction.Networkcost = purchaseInteraction.cost;
            purchaseInteraction.contextToken = "LASER_DRONE1_CONTEXT";
            purchaseInteraction.displayNameToken = "LASER_DRONE1_INTERACTABLE_NAME";
            GenericDisplayNameProvider nameProvider = brokenObject.GetComponent<GenericDisplayNameProvider>();
            nameProvider.displayToken = "LASER_DRONE1_NAME";
            GameObject customBrokenModel = assetBundle.LoadAsset<GameObject>("Assets/Drones/LaserDrone1/Model/mdlBeamDroneBroken.prefab");
            MeshRenderer highlightRenderer = customBrokenModel.transform.Find("_mdlBeamDrone").GetComponent<MeshRenderer>();
            ReplaceBrokenModel(brokenObject, customBrokenModel, highlightRenderer, customBrokenModel.transform.Find("Core").gameObject);
            Transform damagePoint = FindBrokenDamagePoint(customBrokenModel);
            if (damagePoint)
            {
                damagePoint.localPosition = Vector3.zero;
                damagePoint.localRotation = Quaternion.identity;
                damagePoint.localScale = Vector3.one;
            }
        }

        private void ModifyDroneMaster()
        {
            AISkillDriver[] skillDrivers = droneMaster.GetComponents<AISkillDriver>();
            if (skillDrivers.Length > 4)
            {
                skillDrivers[3].maxDistance = 45f;
                skillDrivers[4].maxDistance = 135f;
            }
            else Log.Warning($"{name}: The Gunner Drone skill drivers changed. Attack ranges are left at their default values.");
            skillDrivers.SetAllDriversToAimTowardsEnemies();
        }

        private void ModifyDroneBody()
        {
            CharacterBody body = droneBody.GetComponent<CharacterBody>();
            body.baseNameToken = "LASER_DRONE1_NAME";
            body.baseMaxHealth *= 1.2f;
            body.baseRegen *= 1.2f;
            body.levelMaxHealth *= 1.2f;
            body.levelRegen *= 1.2f;
            body.portraitIcon = assetBundle.LoadAsset<Texture>("Assets/Drones/LaserDrone1/Icon/texLaserDrone1Icon.png");
            ModifyDroneModel(body);
            ModifySkill();
            CharacterDeathBehavior death = body.GetOrAddComponent<CharacterDeathBehavior>();
            death.deathState = new SerializableEntityStateType(typeof(DeathState));
        }

        private void ModifyDroneModel(CharacterBody body)
        {
            GameObject customModel = assetBundle.LoadAsset<GameObject>("Assets/Drones/LaserDrone1/Model/mdlBeamDrone.prefab");
            droneBody.ReplaceModel(customModel, DebugCheck());
            customModel.InitializeDroneModelComponents(body, 1.1f);
            customModel.AddComponent<ThrusterFlicker>();
            BodyRotation rotationComponent = customModel.AddComponent<BodyRotation>();
            rotationComponent.maxRotationSpeed = rotationComponent.rotationSpeed = 4f;
            rotationComponent.rotationDirection = 1;
            customModel.transform.Find("Core").gameObject.AddComponent<CoreFlicker>();
        }

        private void ModifySkill()
        {
            ContentAddition.AddEntityState<FireBeam>(out _);
            SkillDef newSkillDef = Object.Instantiate(drone1Skill);
            newSkillDef.activationState = new SerializableEntityStateType(typeof(FireBeam));
            newSkillDef.baseRechargeInterval = laserCooldown;
            newSkillDef.beginSkillCooldownOnSkillEnd = true;
            newSkillDef.baseMaxStock = 1;
            newSkillDef.fullRestockOnAssign = false;
            ContentAddition.AddSkillDef(newSkillDef);
            SkillLocator locator = droneBody.GetComponent<SkillLocator>();
            SkillFamily newSkillFamily = Object.Instantiate(locator.primary.skillFamily);
            newSkillFamily.variants = new SkillFamily.Variant[1];
            newSkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = newSkillDef,
                unlockableDef = null,
                viewableNode = new ViewablesCatalog.Node("", false, null)
            };
            locator.primary.SetFieldValue("_skillFamily", newSkillFamily);
            ContentAddition.AddSkillFamily(newSkillFamily);
        }

        private void ModifyInteractableSpawnCard()
        {
            iSpawnCard.name = "iscBrokenLaserDrone1";
            iSpawnCard.prefab = brokenObject;
            iSpawnCard.slightlyRandomizeOrientation = false;
            iSpawnCard.orientToFloor = true;
            
        }

        private void DirectorAPI_InteractableActions(DccsPool arg1, StageInfo arg2)
        {
            if (!arg1) return;

            if (arg2.CheckStage(Stage.SkyMeadow))
            {
                ForEachPoolEntryInDccsPool(arg1, (poolEntry) => poolEntry.dccs.AddCard(iDirectorCardHolder));
            }
            else
            {
                ForEachPoolEntryInDccsPool(arg1, (poolEntry) => poolEntry.dccs.AddCard(iHeavyDirectorCardHolder));
            }
        }

        private void InitializeDirectorCards()
        {
            DirectorCard directorCard = new DirectorCard
            {
                spawnCard = iSpawnCard,
#if DEBUG
                selectionWeight = 1000,
                minimumStageCompletions = 0,
#else
                selectionWeight = spawnWeight,
                minimumStageCompletions = minimumStageCompletions,
#endif
                spawnDistance = DirectorCore.MonsterSpawnDistance.Close,
                preventOverhead = false
            };
            DirectorCard heavyDirectorCard = new DirectorCard
            {
                spawnCard = iSpawnCard,
                selectionWeight = skyMeadowSpawnWeight,
                minimumStageCompletions = skyMeadowMinimumStageCompletions,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Close,
                preventOverhead = false
            };
            iDirectorCardHolder = new DirectorCardHolder
            {
                Card = directorCard,
                MonsterCategory = MonsterCategory.Invalid,
                InteractableCategory = InteractableCategory.Drones,
            };
            iHeavyDirectorCardHolder = new DirectorCardHolder
            {
                Card = heavyDirectorCard,
                MonsterCategory = MonsterCategory.Invalid,
                InteractableCategory = InteractableCategory.Drones,
            };
        }

        internal static bool DebugCheck()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}