using R2API;
using RoR2;
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

        public override void SetupConfig()
        {
            base.SetupConfig();
        }

        public override void SetupComponents()
        {
            base.SetupComponents();
            InteractableSpawnCard origIsc = Resources.Load<InteractableSpawnCard>("spawncards/interactablespawncard/iscBrokenDrone1");
            brokenObject = origIsc.prefab;
            brokenObject = brokenObject.InstantiateClone("LaserDrone1Broken");
            ModifyBrokenObject();
            iSpawnCard = origIsc.Clone("iscBrokenLaserDrone1");
            ModifyInteractableSpawnCard();
            DirectorCard directorCard = new DirectorCard
            {
                spawnCard = iSpawnCard,
                selectionWeight = 1,
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

        private void ModifyBrokenObject()
        {
            //Highlight highlight = brokenObject.GetComponent<Highlight>();
            //highlight.targetRenderer = highlight.targetRenderer;
            SummonMasterBehavior summonMasterBehavior = brokenObject.GetComponent<SummonMasterBehavior>();
            GameObject masterObject = summonMasterBehavior.masterPrefab;
            masterObject = masterObject.InstantiateClone("LaserDrone1Master");
            CharacterMaster master = masterObject.GetComponent<CharacterMaster>();
            GameObject bodyObject = master.bodyPrefab;
            bodyObject = bodyObject.InstantiateClone("LaserDrone1Body");
            master.bodyPrefab = bodyObject;
            summonMasterBehavior.masterPrefab = masterObject;
            PurchaseInteraction purchaseInteraction = brokenObject.GetComponent<PurchaseInteraction>();
            purchaseInteraction.cost *= 3;
            purchaseInteraction.Networkcost = purchaseInteraction.cost;
            purchaseInteraction.contextToken = "LASER_DRONE1_CONTEXT";
            purchaseInteraction.displayNameToken = "LASER_DRONE1_INTERACTABLE_NAME";
            GenericDisplayNameProvider nameProvider = brokenObject.GetComponent<GenericDisplayNameProvider>();
            nameProvider.displayToken = "LASER_DRONE1_NAME";
            //ModelLocator modelLocator = brokenObject.GetComponent<ModelLocator>();
            //GameObject modelObject = modelLocator.modelTransform.gameObject;
        }

        private void ModifyInteractableSpawnCard()
        {
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
