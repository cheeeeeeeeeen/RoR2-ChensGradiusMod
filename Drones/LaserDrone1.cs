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
        public static InteractableSpawnCard iSpawnCard { get; private set; }
        public static GameObject brokenObject { get; private set; }
        public static DirectorCardHolder iDirectorCardHolder { get; private set; }
        public static GameObject droneBody { get; private set; }
        public static GameObject droneMaster { get; private set; }

        public override void SetupConfig()
        {
            base.SetupConfig();
        }

        public override void SetupComponents()
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
                selectionWeight = 1000,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Close,
                allowAmbushSpawn = true,
                preventOverhead = false,
                minimumStageCompletions = 0,
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

        public override void SetupBehavior()
        {
            base.SetupBehavior();
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
            CharacterMaster master = droneMaster.GetComponent<CharacterMaster>();
            droneBody = master.bodyPrefab.InstantiateClone("LaserDrone1Body");
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
            SkillDef origSkillDef = Resources.Load<SkillDef>("skilldefs/titanbody/TitanBodyLaser");
            SkillDef newSkillDef = Object.Instantiate(origSkillDef);
            LoadoutAPI.AddSkillDef(newSkillDef);
            SkillLocator locator = droneBody.GetComponent<SkillLocator>();
            SkillFamily newSkillFamily = Object.Instantiate(locator.primary.skillFamily);
            LoadoutAPI.AddSkillFamily(newSkillFamily);
            locator.primary.SetFieldValue("_skillFamily", newSkillFamily);
            newSkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = newSkillDef,
                unlockableName = "",
                viewableNode = new ViewablesCatalog.Node("", false, null)
            };
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
