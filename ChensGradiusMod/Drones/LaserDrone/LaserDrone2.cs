﻿#undef DEBUG

using Chen.Helpers.CollectionHelpers;
using Chen.Helpers.GeneralHelpers;
using Chen.Helpers.UnityHelpers;
using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;
using static R2API.DirectorAPI;
using Stage = R2API.DirectorAPI.Stage;

namespace Chen.GradiusMod.Drones.LaserDrone
{
    internal class LaserDrone2 : Drone<LaserDrone2>
    {
        public float laserCooldown { get; private set; } = 6f;
        public float chargeTime { get; private set; } = 4f;
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

            laserCooldown = config.Bind(configCategory,
                "LaserCooldown", laserCooldown,
                "The cooldown of the focused laser attack."
            ).Value;

            chargeTime = config.Bind(configCategory,
                "ChargeTime", chargeTime,
                "The charge time of Laser Drone to release a strong focused laser."
            ).Value;

            damageCoefficient = config.Bind(configCategory,
                "DamageCoefficient", damageCoefficient,
                "Damage Coefficient of the focused laser."
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
            InteractableSpawnCard origIsc = drone1SpawnCard;
            brokenObject = origIsc.prefab.InstantiateClone($"{name}Broken", true);
            ModifyBrokenObject();
            iSpawnCard = Object.Instantiate(origIsc);
            ModifyInteractableSpawnCard();
            InitializeDirectorCards();
        }

        protected override void SetupBehavior()
        {
            base.SetupBehavior();
            InteractableActions += DirectorAPI_InteractableActions;
        }

        private void AddLanguageTokens()
        {
            LanguageAPI.Add("LASER_DRONE2_NAME", "Laser Drone");
            LanguageAPI.Add("LASER_DRONE2_CONTEXT", "Repair Laser Drone");
            LanguageAPI.Add("LASER_DRONE2_INTERACTABLE_NAME", "Broken Laser Drone");
        }

        private void ModifyBrokenObject()
        {
            SummonMasterBehavior summonMasterBehavior = brokenObject.GetComponent<SummonMasterBehavior>();
            droneMaster = summonMasterBehavior.masterPrefab.InstantiateClone($"{name}Master", true);
            contentProvider.masterObjects.Add(droneMaster);
            ModifyDroneMaster();
            CharacterMaster master = droneMaster.GetComponent<CharacterMaster>();
            droneBody = master.bodyPrefab.InstantiateClone($"{name}Body", true);
            contentProvider.bodyObjects.Add(droneBody);
            ModifyDroneBody();
            master.bodyPrefab = droneBody;
            summonMasterBehavior.masterPrefab = droneMaster;
            PurchaseInteraction purchaseInteraction = brokenObject.GetComponent<PurchaseInteraction>();
            purchaseInteraction.cost *= 3;
            purchaseInteraction.Networkcost = purchaseInteraction.cost;
            purchaseInteraction.contextToken = "LASER_DRONE2_CONTEXT";
            purchaseInteraction.displayNameToken = "LASER_DRONE2_INTERACTABLE_NAME";
            GenericDisplayNameProvider nameProvider = brokenObject.GetComponent<GenericDisplayNameProvider>();
            nameProvider.displayToken = "LASER_DRONE2_NAME";
            GameObject customBrokenModel = assetBundle.LoadAsset<GameObject>("Assets/Drones/LaserDrone2/Model/mdlLaserDroneBroken.prefab");
            brokenObject.ReplaceModel(customBrokenModel);
            Highlight highlight = brokenObject.GetComponent<Highlight>();
            highlight.targetRenderer = customBrokenModel.transform.Find("_mdlLaserDroneBroken").gameObject.GetComponent<MeshRenderer>();
            EntityLocator entityLocator = customBrokenModel.AddComponent<EntityLocator>();
            entityLocator.entity = brokenObject;
            GameObject coreObject = customBrokenModel.transform.Find("Core").gameObject;
            EntityLocator coreEntityLocator = coreObject.AddComponent<EntityLocator>();
            coreEntityLocator.entity = brokenObject;
            AddBrokenEffects(customBrokenModel, (MeshRenderer)highlight.targetRenderer);
        }

        private void AddBrokenEffects(GameObject customBrokenModel, MeshRenderer meshRenderer)
        {
            GameObject brokenEffects = brokenObject.transform.Find("ModelBase").Find("BrokenDroneVFX").gameObject;
            brokenEffects.transform.parent = customBrokenModel.transform;
            GameObject sparks = brokenEffects.transform.Find("Small Sparks, Mesh").gameObject;
            ParticleSystem.ShapeModule sparksShape = sparks.GetComponent<ParticleSystem>().shape;
            sparksShape.shapeType = ParticleSystemShapeType.MeshRenderer;
            sparksShape.meshShapeType = ParticleSystemMeshShapeType.Edge;
            sparksShape.meshRenderer = meshRenderer;
            GameObject damagePoint = brokenEffects.transform.Find("Damage Point").gameObject;
            damagePoint.transform.localPosition = Vector3.zero;
            damagePoint.transform.localRotation = Quaternion.identity;
            damagePoint.transform.localScale = Vector3.one;
        }

        private void ModifyDroneMaster()
        {
            AISkillDriver[] skillDrivers = droneMaster.GetComponents<AISkillDriver>();
            skillDrivers[3].maxDistance = 30f;
            skillDrivers[4].maxDistance = 90f;
            skillDrivers.SetAllDriversToAimTowardsEnemies();
        }

        private void ModifyDroneBody()
        {
            CharacterBody body = droneBody.GetComponent<CharacterBody>();
            body.baseNameToken = "LASER_DRONE2_NAME";
            body.baseMaxHealth *= 1.2f;
            body.baseRegen *= 1.2f;
            body.baseDamage *= 10f;
            body.baseCrit *= 3f;
            body.levelMaxHealth *= 1.2f;
            body.levelRegen *= 1.2f;
            body.levelDamage *= 10f;
            body.levelCrit *= 3f;
            body.portraitIcon = assetBundle.LoadAsset<Texture>("Assets/Drones/LaserDrone2/Icon/texLaserDrone2Icon.png");
            ModifyDroneModel(body);
            ModifySkill();
            CharacterDeathBehavior death = body.GetOrAddComponent<CharacterDeathBehavior>();
            death.deathState = new SerializableEntityStateType(typeof(DeathState));
        }

        private void ModifyDroneModel(CharacterBody body)
        {
            GameObject customModel = assetBundle.LoadAsset<GameObject>("Assets/Drones/LaserDrone2/Model/mdlLaserDrone.prefab");
            droneBody.ReplaceModel(customModel);
            customModel.InitializeDroneModelComponents(body, 1.1f, DebugCheck());
            customModel.transform.Find("AimOrigin").gameObject.AddComponent<ChargeEffect>();
            customModel.transform.Find("Core").gameObject.AddComponent<CoreFlicker>();
            BodyRotation rotationComponent = customModel.AddComponent<BodyRotation>();
            rotationComponent.rotationDirection = -1;
            rotationComponent.rotationSpeed = 2f;
        }

        private void ModifySkill()
        {
            LoadoutAPI.AddSkill(typeof(FireLaser));
            SkillDef newSkillDef = Object.Instantiate(drone1Skill);
            newSkillDef.activationState = new SerializableEntityStateType(typeof(FireLaser));
            newSkillDef.baseRechargeInterval = laserCooldown;
            newSkillDef.beginSkillCooldownOnSkillEnd = true;
            newSkillDef.baseMaxStock = 1;
            newSkillDef.fullRestockOnAssign = false;
            LoadoutAPI.AddSkillDef(newSkillDef);
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
            LoadoutAPI.AddSkillFamily(newSkillFamily);
        }

        private void ModifyInteractableSpawnCard()
        {
            iSpawnCard.name = $"iscBroken{name}";
            iSpawnCard.prefab = brokenObject;
            iSpawnCard.slightlyRandomizeOrientation = false;
            iSpawnCard.orientToFloor = true;
        }

        private void DirectorAPI_InteractableActions(List<DirectorCardHolder> arg1, StageInfo arg2)
        {
            if (!arg2.CheckStage(Stage.SkyMeadow)) arg1.ConditionalAdd(iDirectorCardHolder, card => iDirectorCardHolder == card);
            else arg1.ConditionalAdd(iHeavyDirectorCardHolder, card => iHeavyDirectorCardHolder == card);
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
                allowAmbushSpawn = true,
                preventOverhead = false
            };
            DirectorCard heavyDirectorCard = new DirectorCard
            {
                spawnCard = iSpawnCard,
                selectionWeight = skyMeadowSpawnWeight,
                minimumStageCompletions = skyMeadowMinimumStageCompletions,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Close,
                allowAmbushSpawn = true,
                preventOverhead = false
            };
            iDirectorCardHolder = new DirectorCardHolder
            {
                Card = directorCard,
                MonsterCategory = MonsterCategory.None,
                InteractableCategory = InteractableCategory.Drones,
            };
            iHeavyDirectorCardHolder = new DirectorCardHolder
            {
                Card = heavyDirectorCard,
                MonsterCategory = MonsterCategory.None,
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