#undef DEBUG

using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;
using static R2API.DirectorAPI;

namespace Chen.GradiusMod
{
    public class LaserDrone1 : Drone<LaserDrone1>
    {
        public float laserDuration { get; private set; } = 4f;
        public float laserCooldown { get; private set; } = 4f;
        public float damageCoefficient { get; private set; } = 1f;

        public static InteractableSpawnCard iSpawnCard { get; private set; }
        public static GameObject brokenObject { get; private set; }
        public static DirectorCardHolder iDirectorCardHolder { get; private set; }
        public static GameObject droneBody { get; private set; }
        public static GameObject droneMaster { get; private set; }

        protected override void SetupConfig()
        {
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
        }

        protected override void SetupComponents()
        {
            base.SetupComponents();
            AddLanguageTokens();
            InteractableSpawnCard origIsc = Resources.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenDrone1");
            brokenObject = origIsc.prefab;
            brokenObject = brokenObject.InstantiateClone("LaserDrone1Broken");
            ModifyBrokenObject();
            iSpawnCard = Object.Instantiate(origIsc);
            ModifyInteractableSpawnCard();
            DirectorCard directorCard = new DirectorCard
            {
                spawnCard = iSpawnCard,
#if DEBUG
                selectionWeight = 1000,
                minimumStageCompletions = 0,
#else
                selectionWeight = 1,
                minimumStageCompletions = 2,
#endif
                spawnDistance = DirectorCore.MonsterSpawnDistance.Close,
                allowAmbushSpawn = true,
                preventOverhead = false,
                requiredUnlockable = "",
                forbiddenUnlockable = ""
            };
            iDirectorCardHolder = new DirectorCardHolder
            {
                Card = directorCard,
                MonsterCategory = MonsterCategory.None,
                InteractableCategory = InteractableCategory.Drones,
            };
        }

        protected override void SetupBehavior()
        {
            base.SetupBehavior();
            GradiusOption.instance.SupportMinionType("LaserDrone1");
            InteractableActions += DirectorAPI_InteractableActions;
        }

        private void AddLanguageTokens()
        {
            LanguageAPI.Add("LASER_DRONE1_NAME", "Beam Drone");
            LanguageAPI.Add("LASER_DRONE1_CONTEXT", "Repair Beam Drone");
            LanguageAPI.Add("LASER_DRONE1_INTERACTABLE_NAME", "Broken Beam Drone");
        }

        private void ModifyBrokenObject()
        {
            Highlight highlight = brokenObject.GetComponent<Highlight>();
            SkinnedMeshRenderer renderer = highlight.targetRenderer as SkinnedMeshRenderer;
            renderer.material.color = Color.yellow;
            SummonMasterBehavior summonMasterBehavior = brokenObject.GetComponent<SummonMasterBehavior>();
            droneMaster = summonMasterBehavior.masterPrefab.InstantiateClone("LaserDrone1Master");
            MasterCatalog.getAdditionalEntries += (list) => list.Add(droneMaster);
            CharacterMaster master = droneMaster.GetComponent<CharacterMaster>();
            droneBody = master.bodyPrefab.InstantiateClone("LaserDrone1Body");
            BodyCatalog.getAdditionalEntries += (list) => list.Add(droneBody);
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
        }

        private void ModifyDroneBody()
        {
            CharacterBody body = droneBody.GetComponent<CharacterBody>();
            body.baseNameToken = "LASER_DRONE1_NAME";
            body.baseMaxHealth *= 1.2f;
            body.baseRegen *= 1.2f;
            body.portraitIcon = Resources.Load<Texture>("@ChensGradiusMod:Assets/Drones/LaserDrone1/texLaserDrone1Icon.png");
            ModelLocator modelLocator = droneBody.GetComponent<ModelLocator>();
            GameObject modelObject = modelLocator.modelTransform.gameObject;
            CharacterModel model = modelObject.GetComponent<CharacterModel>();
            Material material = Object.Instantiate(model.baseRendererInfos[0].defaultMaterial);
            material.color = Color.yellow;
            model.baseRendererInfos[0].defaultMaterial = material;
            ModifySkill();
        }

        private void ModifySkill()
        {
            LoadoutAPI.AddSkill(typeof(FireLaser));
            SkillDef origSkillDef = Resources.Load<SkillDef>("skilldefs/drone1body/Drone1BodyGun");
            SkillDef newSkillDef = Object.Instantiate(origSkillDef);
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
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node("", false, null)
            };
            locator.primary.SetFieldValue("_skillFamily", newSkillFamily);
            LoadoutAPI.AddSkillFamily(newSkillFamily);
        }

        private void ModifyInteractableSpawnCard()
        {
            iSpawnCard.name = "iscBrokenLaserDrone1";
            iSpawnCard.prefab = brokenObject;
        }

        private void DirectorAPI_InteractableActions(List<DirectorCardHolder> arg1, StageInfo arg2)
        {
            if (!arg1.Contains(iDirectorCardHolder))
            {
                arg1.Add(iDirectorCardHolder);
            }
        }
    }
}